using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IDragHandler, IBeginDragHandler
{
    public Image outer;
    public Image inner;
    // public Text numText;
    public Image numImage;  // 2022.1.7 added, no longer using text to display card num
    public Text cnText;
    public Text engText;
    public Image image;
    public GameObject backSideImage;

    int cardNum;
    public int CardNum { get => cardNum; }
    CardColor cardColor;
    public CardColor CardColor { get => cardColor; }

    public System.Action specialAction;

    bool faceUp = false;

    public Player owner;

    bool isDragging;
    static Card isHoveringCard;
    public bool hasShow = false;
    [SerializeField] float hoverTime = 3f;
    static float hoveringCounter;

    public bool isAvailable = false;

    void Start()
    {
        isHoveringCard = null;
        isDragging = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInteract();

        if (hoveringCounter >= hoverTime && isHoveringCard == this)
        {
            GameHandler.Singleton.ShowCardDescription(this);
        }

        if (owner != null)
        {
            if (owner.isLocalPlayer && isAvailable)
            {
                outer.enabled = true;
            }
            else
            {
                outer.enabled = false;
            }
        }
        else
        {
            outer.enabled = false;
        }
    }

    void CheckInteract()
    {
        if (owner == null || owner.isLocalPlayer)
        {
            if (isHoveringCard == this)
            {
                hoveringCounter += Time.deltaTime;
            }
        }

        if (isDragging)
        {
            hoveringCounter = 0;
            transform.position = Input.mousePosition;
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                GameHandler.Singleton.ShowCardPlacingRect(false);

                RectTransform rt = GameHandler.Singleton.cardPlacingRect as RectTransform;
                RectTransform self = transform as RectTransform;
                if (Utils.CheckRecttransformOverlaps(self, rt))
                    UseCard();
            }
        }
    }

    public void UseCard()
    {
        SwitchSide(true);

        GameHandler.Singleton.DropCard(this);

        if (specialAction != null)
            specialAction.Invoke();

        owner.RemoveCard(this);

        owner = null;
        isAvailable = false;
    }

    public void Init(CardColor color, int num)
    {
        this.cardColor = color;
        this.cardNum = num;

        this.gameObject.name = $"{color.ToString()} - {num}";

        SettingManager sm = SettingManager.Singleton;
        Sprite numImage = null;
        Color textColor = Color.white;
        switch (cardColor)
        {
            case CardColor.Red:
                textColor = sm.redTextColor;
                inner.sprite = sm.red;
                numImage = sm.redNum[num];
                break;

            case CardColor.Blue:
                textColor = sm.blueTextColor;
                inner.sprite = sm.blue;
                numImage = sm.blueNum[num];
                break;

            case CardColor.Yellow:
                textColor = sm.yellowTextColor;
                inner.sprite = sm.yellow;
                numImage = sm.yellowNum[num];
                break;

            case CardColor.Green:
                textColor = sm.greenTextColor;
                inner.sprite = sm.green;
                numImage = sm.greenNum[num];
                break;
        }

        var setting = sm.cardSettings[GameHandler.Singleton.cardIdList[num]];
        image.sprite = setting.imageSliced;

        this.numImage.sprite = numImage;

        cnText.text = $"{setting.author} {setting.imageName}";
        Utils.VerticalText(this.cnText);
        engText.text = setting.imageNameEng;
        cnText.color = textColor;
        engText.color = textColor;

        SwitchSide(false);
        // gameObject.SetActive(false);
    }

    public void SwitchSide(bool faceUp)
    {
        inner.gameObject.SetActive(faceUp);
        backSideImage.SetActive(!faceUp);

        this.faceUp = faceUp;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHoveringCard = this;
        if (owner != null && owner.isLocalPlayer)
        {
            owner.currentHoveringCard = owner.handCard.IndexOf(this);
        }
    }

    // no longer use because not stable
    public void OnDrag(PointerEventData eventData) { }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isHoveringCard = null;
        GameHandler.Singleton.HideCardDescription();
        // throw new System.NotImplementedException();
        if (owner != null && owner.isLocalPlayer && GameHandler.Singleton.CurrentRoundHost == owner && isAvailable)
        {
            GameHandler.Singleton.ShowCardPlacingRect(true);
            isDragging = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(PointerTemper(eventData));
        // if (eventData.pointer.tag != "Card")
        //     isHovering = false;
    }

    IEnumerator<bool> PointerTemper(PointerEventData eventData)
    {
        yield return false;
        if (eventData.pointerEnter == null || eventData.pointerEnter.tag != "Card")
        {
            isHoveringCard = null;
            GameHandler.Singleton.HideCardDescription();
            hoveringCounter = 0;
        }
    }
}

public enum CardColor
{
    Red, Blue, Green, Yellow, Black
}
