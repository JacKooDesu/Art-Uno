using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerEnterHandler, IDragHandler, IBeginDragHandler
{
    public Image inner;
    public Text numText;
    public GameObject backSideImage;

    int cardNum;
    public int CardNum { get => cardNum; }
    CardColor cardColor;
    public CardColor CardColor { get => cardColor; }

    bool faceUp = false;

    public Player owner;

    bool isDragging;

    public bool isAvailable = false;

    void Start()
    {
        isDragging = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInteract();
    }

    void CheckInteract()
    {
        if (isDragging)
        {
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
        owner.RemoveCard(this);
    }

    public void Init(CardColor color, int num)
    {
        this.cardColor = color;
        this.cardNum = num;

        this.gameObject.name = $"{color.ToString()} - {num}";

        SettingManager sm = SettingManager.Singleton;
        switch (cardColor)
        {
            case CardColor.Red:
                inner.color = sm.red;
                break;

            case CardColor.Blue:
                inner.color = sm.blue;
                break;

            case CardColor.Yellow:
                inner.color = sm.yellow;
                break;

            case CardColor.Green:
                inner.color = sm.green;
                break;
        }

        numText.text = num.ToString();

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
        if (owner != null && owner.isLocalPlayer)
        {
            owner.currentHoveringCard = owner.handCard.IndexOf(this);
        }
    }

    // no longer use because not stable
    public void OnDrag(PointerEventData eventData) { }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // throw new System.NotImplementedException();
        if (owner != null && owner.isLocalPlayer && GameHandler.Singleton.CurrentRoundHost == owner && isAvailable)
        {
            GameHandler.Singleton.ShowCardPlacingRect(true);
            isDragging = true;
        }
    }
}

public enum CardColor
{
    Red, Blue, Green, Yellow
}
