using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : Player
{
    List<System.Action> actions = new List<System.Action>();

    public float aiThinkTime;

    public override void CalculateCardPosition()
    {
        SettingManager sm = SettingManager.Singleton;

        float interval = (cardParent as RectTransform).sizeDelta.x / ((float)handCard.Count);
        interval = Mathf.Clamp(interval, sm.minCardInvervalLength, sm.maxCardInvervalLength);
        float mid = ((float)handCard.Count) / 2f;

        for (int i = 0; i < handCard.Count; ++i)
        {
            Vector3 origin = (handCard[i].transform as RectTransform).localPosition;
            (handCard[i].transform as RectTransform).localPosition = Vector3.right * ((float)i - mid) * interval;
        }
    }

    public override void OnRoundBegin()
    {
        actions = new List<System.Action>();

        GameHandler.Singleton.nextRoundButton.interactable = false;
        GameHandler.Singleton.drawButton.interactable = false;

        CheckCardAvailable();

        if (handCard.Find((c) => c.isAvailable) == null)
        {
            GameHandler.Singleton.Draw(this);
        }

        if (handCard.Find((c) => c.isAvailable) == null)
        {
            OnRoundEnd();
            return;
        }
        else
        {
            List<Card> availableCards = handCard.FindAll((c) => c.isAvailable);
            int[] availableCardPoints = new int[availableCards.Count];

            for (int i = 0; i < availableCardPoints.Length; ++i)
            {
                int point = 0;
                Card current = availableCards[i];

                int sameColor = handCard.FindAll((c) => c.CardColor == current.CardColor).Count - 1;
                int sameNum = handCard.FindAll((c) => c.CardNum == current.CardNum).Count - 1;
                point += (sameColor * 2) + (sameNum * 3);

                // addition point
                // foreach (Card c in handCard.FindAll((c) => c.CardNum == current.CardNum))
                // {
                //     point += 1;
                // }
                availableCardPoints[i] = point;
            }

            int cardIndex = 0;
            for (int i = 1; i < availableCardPoints.Length; ++i)
            {
                if (availableCardPoints[i] > availableCardPoints[cardIndex])
                    cardIndex = i;
            }
            Card finalCard = availableCards[cardIndex];
            List<Card> useCards = handCard.FindAll((c) => c.CardNum == finalCard.CardNum);

            actions.Add(() => finalCard.UseCard());
            foreach (Card c in useCards)
            {
                if (c == finalCard)
                    continue;

                Card tempCard = c;
                actions.Add(() => tempCard.UseCard());
            }

            // 選顏色
            if (useCards.Count - 1 > 0)
            { }
        }

        actions.Add(() => OnRoundEnd());

        StartCoroutine(AiAction());
    }

    IEnumerator AiAction()
    {
        foreach (System.Action action in actions)
        {
            yield return new WaitForSeconds(aiThinkTime);
            action.Invoke();

            if (handCard.Count == 0)

                break;
        }
    }
}
