﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    static SettingManager singleton = null;

    public static SettingManager Singleton
    {
        get
        {
            if (singleton != null)
                return singleton;

            singleton = FindObjectOfType(typeof(SettingManager)) as SettingManager;

            if (singleton == null)
            {
                GameObject g = new GameObject("SettingManager");
                singleton = g.AddComponent<SettingManager>();
            }

            return singleton;
        }
    }

    public GameObject cardPrefab;

    public Color red, blue, yellow, green;

    public int cardCount;
    public int cardRange;

    public float deckCardOffset;

    [Header("玩家手牌設定")]
    public float maxCardInvervalLength;
    public float minCardInvervalLength;

    void Start()
    {

    }

    void Update()
    {

    }
}
