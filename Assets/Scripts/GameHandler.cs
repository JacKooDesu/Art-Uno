using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameHandler : MonoBehaviour
{
    public List<Card> deck = new List<Card>();
    public List<Card> cardOnTable = new List<Card>();
    public List<Card> cardInGame = new List<Card>();

    public Transform deckParent;

    public UnityEngine.UI.Button testButton;

    public void InitDeck()
    {
        deck = new List<Card>();
        SettingManager sm = SettingManager.Singleton;
        for (int x = 0; x < 4; ++x)
        {
            for (int i = 0; i < sm.cardCount; ++i)
            {
                for (int j = 0; j < sm.cardRange; ++j)
                {
                    Card c = Instantiate(sm.cardPrefab).GetComponent<Card>();
                    c.transform.SetParent(deckParent);
                    (c.transform as RectTransform).localPosition = new Vector2(0, ((float)(i * j)) * sm.deckCardOffset);

                    c.Init((CardColor)x, j);

                    deck.Add(c);
                }
            }
        }

        Shuffle();

    }

    public void ReturnToDeck()
    {
        for (int i = 0; i < cardOnTable.Count; ++i)
        {
            // Vector2 targetPosition = (c.transform as RectTransform).localPosition = new Vector2(0, ((float)(i * j)) * sm.deckCardOffset);
            // cardOnTable[i].transform.DOMove()
        }
    }

    void Shuffle()
    {
        int cardCount = deck.Count;

        for (int i = cardCount - 1; i >= 0; --i)
        {
            int rand = UnityEngine.Random.Range(0, i);
            Card tmpCard = deck[i];
            deck[i] = deck[rand];
            deck[rand] = tmpCard;
        }
    }

    public void Draw(Player p)
    {
        if (deck.Count == 0)
        {
            deck.AddRange(cardOnTable);
            cardOnTable.Clear();
            Shuffle();
        }

        Card c = deck[0];
        cardInGame.Add(c);
        deck.RemoveAt(0);

        c.transform.SetParent(FindObjectOfType<Canvas>().transform);
        c.transform.DOMove(p.cardParent.position, .5f).OnComplete(() => p.AddCard(c));
    }

    void Start()
    {
        InitDeck();

        testButton.onClick.AddListener(() => Draw(FindObjectOfType<Player>()));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
