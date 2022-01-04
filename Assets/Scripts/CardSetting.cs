using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card Setting", menuName = "Card/Create Card Setting", order = 0)]
public class CardSetting : ScriptableObject
{
    public string imageName;
    public string author;
    [TextArea(3, 10)]
    public string description;
    public Sprite image;
    public Sprite red, blue, green, yellow;
}
