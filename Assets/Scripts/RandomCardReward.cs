using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RandomCardReward : MonoBehaviour
{
    [SerializeField] GameObject cardReward1;          // Reference to the first card Button component
    [SerializeField] GameObject cardReward2;          // Reference to the second card Button component

    private CardManager cardManager;   // Reference to the CardManager
    [SerializeField] GameObject RewardUI;  // Reference to the Reward UI GameObject

    void Start()
    {
        // Find the CardManager in the scene
        cardManager = FindObjectOfType<CardManager>();

        //cardReward1 = GameObject.Find("CardReward1");
        //cardReward2 = GameObject.Find("CardReward2");

        //RewardUI = GameObject.Find("RewardUI");

        RewardUI.SetActive(false);

    }

    public void AssignRandomCards()
    {
        RewardUI.SetActive(true);

        if (cardManager.AvailableCards.Count > 1)
        {
            int randomIndex1 = Random.Range(0, cardManager.AvailableCards.Count);
            Sprite sprite1 = cardManager.AvailableCards[randomIndex1].cardSprite;
            cardReward1.GetComponent<Image>().sprite = sprite1;

            int randomIndex2;

            do
            {
                randomIndex2 = Random.Range(0, cardManager.AvailableCards.Count);
            } while (randomIndex2 == randomIndex1);

            Sprite sprite2 = cardManager.AvailableCards[randomIndex2].cardSprite;
            cardReward2.GetComponent<Image>().sprite = sprite2;
        }
        else if (cardManager.AvailableCards.Count == 1)
        {
            Sprite sprite1 = cardManager.AvailableCards[0].cardSprite;
            cardReward1.GetComponent<Image>().sprite = sprite1;
            cardReward2.GetComponent<Image>().sprite = sprite1;
        }
        else
        {
            Debug.LogError("No cards available in availableCards list!");
        }
    }


}
