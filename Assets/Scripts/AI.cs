using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : Player
{
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
}
