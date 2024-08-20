using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public int energyCost;
    public int cardValue;
    public int moveValue;
    public string cardName;
    public string cardDescription;
    public Sprite cardSprite;

    public Card(int energyCost, int cardValue, int moveValue, string cardName, string cardDescription, Sprite cardSprite)
    {
        this.energyCost = energyCost;
        this.cardValue = cardValue;
        this.moveValue = moveValue;
        this.cardName = cardName;
        this.cardDescription = cardDescription;
        this.cardSprite = cardSprite;
    }

}
