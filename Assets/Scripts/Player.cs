using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public string playerName;
    public Transform cardParent;
    public List<Card> handCard = new List<Card>();

    public bool isLocalPlayer;

    public int currentHoveringCard;

    protected void Start()
    {
        currentHoveringCard = 0;
    }

    protected void Update()
    {
        if (isLocalPlayer)
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
    }

    public virtual void OnRoundBegin()
    {

    }

    public virtual void OnRoundEnd()
    {

    }
}
