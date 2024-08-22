using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RandomCardReward : MonoBehaviour
{
    public Button cardReward1;          // Reference to the first card Button component
    public Button cardReward2;          // Reference to the second card Button component

    private CardManager cardManager;   // Reference to the CardManager

    void Start()
    {
        // Find the CardManager in the scene
        cardManager = FindObjectOfType<CardManager>();

        if (cardManager != null)
        {
            AssignRandomCards();
        }
        else
        {
            Debug.LogError("CardManager not found!");
        }
    }

    public void AssignRandomCards()
    {
        if (cardManager.availableCards.Count > 0)
        {
            // Assign a random card from availableCards to cardReward1
            int randomIndex1 = Random.Range(0, cardManager.availableCards.Count);
            Sprite sprite1 = cardManager.availableCards[randomIndex1].cardSprite;
            cardReward1.GetComponent<Image>().sprite = sprite1;  // Assign sprite to the Image component of Button

            // Assign a different random card from availableCards to cardReward2
            int randomIndex2 = Random.Range(0, cardManager.availableCards.Count);
            Sprite sprite2 = cardManager.availableCards[randomIndex2].cardSprite;
            cardReward2.GetComponent<Image>().sprite = sprite2;  // Assign sprite to the Image component of Button
        }
        else
        {
            Debug.LogError("No cards available in availableCards list!");
        }
    }

}
