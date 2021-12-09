using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerEnterHandler, IDragHandler
{
    public Image inner;
    public Text numText;
    public GameObject backSideImage;

    int cardNum;
    CardColor cardColor;

    bool faceUp = false;

    public Player owner;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
        if (owner!=null && owner.isLocalPlayer)
        {
            owner.currentHoveringCard = owner.handCard.IndexOf(this);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // throw new System.NotImplementedException();
    }
}

public enum CardColor
{
    Red, Blue, Green, Yellow
}
