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

    public Transform logParent;

    public UnityEngine.UI.Button testButton;

    public List<Player> players = new List<Player>();
    Player currentRoundHost;

    [HideInInspector] public bool isLogging;

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
                    Card c = Instantiate(sm.cardPrefab, deckParent).GetComponent<Card>();
                    // c.transform.SetParent(deckParent);
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

    public void Draw(Player p, int count)
    {
        for (int i = 0; i < count; ++i)
        {
            Draw(p);
        }
    }

    void Start()
    {
        SettingManager sm = SettingManager.Singleton;

        InitDeck();

        players = new List<Player>();
        players.AddRange(FindObjectsOfType<Player>());
        print(players.Count);

        Player local = players.Find((p) => (p.isLocalPlayer));

        testButton.onClick.AddListener(() => Draw(local));
        // testButton.onClick.AddListener(() => Log("你的回合！"));

        foreach (Player p in players)
        {
            Draw(p, sm.startCardCount);
        }

        // 決定起始玩家
        ChangePlayer(Random.Range(0, players.Count));
    }

    public void ChangePlayer(int index)
    {
        currentRoundHost = players[index];

        Log($"{currentRoundHost.playerName} 的回合！");
    }

    public void Log(string message)
    {
        isLogging = true;

        UnityEngine.UI.Text logText = logParent.GetComponentInChildren<UnityEngine.UI.Text>();
        Animator ani = logParent.GetComponent<Animator>();
        CanvasGroup canvasGroup = logParent.GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        logText.text = message;

        logParent.gameObject.SetActive(true);
        // DOTween.To [ getter, setter, targetValue, duration ]
        DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1f, .6f).
            OnComplete(
                () =>
                {
                    ani.SetTrigger("Start");
                    StartCoroutine(CheckAnimationComplete());
                });

        IEnumerator CheckAnimationComplete()
        {
            // 暫用數值定義，看要怎麼判定Animation clip length
            yield return new WaitForSeconds(1.5f);

            DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0f, .6f).
            OnComplete(
                () =>
                {
                    logParent.gameObject.SetActive(false);
                    isLogging = false;
                });
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
