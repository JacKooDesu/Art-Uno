using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameHandler : MonoBehaviour
{
    static GameHandler singleton = null;

    public static GameHandler Singleton
    {
        get
        {
            if (singleton != null)
                return singleton;

            singleton = FindObjectOfType(typeof(GameHandler)) as GameHandler;

            if (singleton == null)
            {
                GameObject g = new GameObject("GameHandler");
                singleton = g.AddComponent<GameHandler>();
            }

            return singleton;
        }
    }

    public List<Card> deck = new List<Card>();
    public List<Card> cardOnTable = new List<Card>();
    public List<Card> cardInGame = new List<Card>();

    public Transform deckParent;

    public Transform logParent;

    public Transform cardPlacingRect;   // 拖曳至此位置將牌打出
    public Transform tableCardParent;

    public UnityEngine.UI.Button drawButton;
    public UnityEngine.UI.Button nextRoundButton;

    public List<Player> players = new List<Player>();
    [HideInInspector] public int currentRoundHostIndex;
    public Player CurrentRoundHost
    {
        get => players[currentRoundHostIndex];
    }

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
        c.transform.DOMove(p.cardParent.position, .5f);
        p.AddCard(c);
    }

    public void Draw(Player p, int count)
    {
        for (int i = 0; i < count; ++i)
        {
            Draw(p);
        }
    }

    public void DropCard(Card c)
    {
        c.transform.DOMove(Utils.GetRandomPointInRect(cardPlacingRect as RectTransform), .5f);
        c.transform.DOScale(Vector3.one * .8f, .5f);
        c.transform.DORotate(Vector3.forward * Random.Range(0, 360f), .5f);

        c.transform.SetParent(tableCardParent);
        // c.GetComponent<>

        cardOnTable.Add(c);
        // (c.transform as RectTransform).DOMove(Vector2.zero, .5f);
    }

    void Start()
    {
        SettingManager sm = SettingManager.Singleton;

        InitDeck();

        players = new List<Player>();
        players.AddRange(FindObjectsOfType<Player>());
        print(players.Count);

        Player local = players.Find((p) => (p.isLocalPlayer));

        drawButton.onClick.AddListener(() => Draw(local));
        // nextRoundButton.onClick.AddListener(() => NextRound());
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
        currentRoundHostIndex = index;

        Log($"{CurrentRoundHost.playerName} 的回合");

        CurrentRoundHost.OnRoundBegin();
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

    public void ShowCardPlacingRect(bool b)
    {
        CanvasGroup canvasGroup = cardPlacingRect.GetComponent<CanvasGroup>();

        if (!b)
        {
            DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0f, .4f).
                 OnComplete(() => cardPlacingRect.gameObject.SetActive(b));
        }
        else
        {
            canvasGroup.alpha = Mathf.Max(0, canvasGroup.alpha);
            DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1f, .4f);

            cardPlacingRect.gameObject.SetActive(b);
        }
    }

    public void NextRound()
    {
        int target = 0;
        if (currentRoundHostIndex + 1 < players.Count)
        {
            target = currentRoundHostIndex + 1;
        }
        else
        {
            target = 0;
        }

        ChangePlayer(target);
    }

    void Update()
    {

    }
}
