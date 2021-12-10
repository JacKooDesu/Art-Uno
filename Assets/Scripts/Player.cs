using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public string playerName;
    public Transform cardParent;
    public List<Card> handCard = new List<Card>();

    public bool isLocalPlayer;
    [SerializeField] bool hasAction;

    public int currentHoveringCard;

    protected void Start()
    {
        currentHoveringCard = 0;
    }

    protected void Update()
    {
        // if (isLocalPlayer)
        CalculateCardPosition();
    }

    public virtual void CalculateCardPosition()
    {
        SettingManager sm = SettingManager.Singleton;

        float interval = (cardParent as RectTransform).sizeDelta.x / ((float)handCard.Count);
        interval = Mathf.Clamp(interval, sm.minCardInvervalLength, sm.maxCardInvervalLength);
        float mid = ((float)handCard.Count) / 2f;

        for (int i = 0; i < handCard.Count; ++i)
        {
            Vector3 origin = (handCard[i].transform as RectTransform).localPosition;
            if (i == currentHoveringCard)
            {
                (handCard[i].transform as RectTransform).localPosition =
                    Vector3.Lerp(origin, (Vector3.right * ((float)i - mid) * interval) + (Vector3.up * 20f), .2f);
            }
            else if (i < currentHoveringCard)
            {
                (handCard[i].transform as RectTransform).localPosition =
                    Vector3.Lerp(origin, Vector3.right * ((float)i - mid) * interval, .2f);
            }
            else
            {
                (handCard[i].transform as RectTransform).localPosition =
                    Vector3.Lerp(origin, Vector3.right * (((float)i - mid - 1) * interval + 140f), .2f);    // 140 是暫定的卡片寬度
            }
        }
    }

    public void AddCard(Card c)
    {
        c.owner = this;

        handCard.Add(c);
        if (isLocalPlayer)
            c.SwitchSide(true);

        c.transform.SetParent(cardParent);

        currentHoveringCard = handCard.Count - 1;

        CalculateCardPosition();

        if (GameHandler.Singleton.CurrentRoundHost == this)
            CheckCardAvailable();
    }

    // 使用卡片
    public void RemoveCard(Card c)
    {
        hasAction = true;

        handCard.Remove(c);

        // c.transform.SetParent(FindObjectOfType<Canvas>().transform);
        currentHoveringCard = handCard.Count - 1;

        CalculateCardPosition();
        CheckCardAvailable();

        if (handCard.Count == 0)
        {
            // 贏了
            GameHandler.Singleton.Log($"{playerName} 贏了");
        }
    }

    public virtual void OnRoundBegin()
    {
        hasAction = false;

        UnityEngine.UI.Button nextBtn = GameHandler.Singleton.nextRoundButton;
        UnityEngine.UI.Button drawBtn = GameHandler.Singleton.drawButton;

        if (!isLocalPlayer)
        {
            nextBtn.interactable = false;
            drawBtn.interactable = false;
            return;
        }

        nextBtn.interactable = false;
        drawBtn.interactable = true;

        UnityEngine.Events.UnityAction onDraw = null;
        onDraw = () =>
        {
            drawBtn.interactable = false;
            hasAction = true;
            drawBtn.onClick.RemoveListener(onDraw);
            GameHandler.Singleton.nextRoundButton.interactable = true;
        };
        drawBtn.onClick.AddListener(onDraw);

        UnityEngine.Events.UnityAction endRound = null;
        endRound = () =>
        {
            OnRoundEnd();
            nextBtn.interactable = false;
            nextBtn.onClick.RemoveListener(endRound);
        };
        nextBtn.onClick.AddListener(endRound);

        CheckCardAvailable();
    }

    protected void CheckCardAvailable()
    {
        Card topCard;

        if (GameHandler.Singleton.cardOnTable.Count == 0)
        {
            foreach (Card c in handCard)
            {
                c.isAvailable = true;
            }
        }
        else
        {
            topCard = GameHandler.Singleton.cardOnTable[GameHandler.Singleton.cardOnTable.Count - 1];
            foreach (Card c in handCard)
            {
                if ((c.CardColor == topCard.CardColor && !hasAction) || c.CardNum == topCard.CardNum)
                {
                    c.isAvailable = true;
                }
                else
                {
                    c.isAvailable = false;
                }
            }
        }

        if (hasAction && isLocalPlayer)
        {
            GameHandler.Singleton.drawButton.interactable = false;
            GameHandler.Singleton.nextRoundButton.interactable = true;
        }
    }

    public virtual void OnRoundEnd()
    {
        // if(丟了很多張卡，要選顏色)
        GameHandler.Singleton.NextRound();
    }
}
