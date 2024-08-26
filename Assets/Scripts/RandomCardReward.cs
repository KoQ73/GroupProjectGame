using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class RandomCardReward : MonoBehaviour
{
    [SerializeField] GameObject cardReward1;  
    [SerializeField] GameObject cardReward2;     

    [SerializeField] TextMeshProUGUI cardDescription1;
    [SerializeField] TextMeshProUGUI cardDescription2;

    [SerializeField] TextMeshProUGUI energyCost1;
    [SerializeField] TextMeshProUGUI energyCost2;

    private Card selectedCard;

    private CardManager cardManager;
    [SerializeField] GameObject RewardUI;

    void Start()
    {
        // Find the CardManager in the scene
        cardManager = FindObjectOfType<CardManager>();

        //cardReward1 = GameObject.Find("CardReward1");
        //cardReward2 = GameObject.Find("CardReward2");

        //RewardUI = GameObject.Find("RewardUI");

        if (cardDescription1 == null)
            cardDescription1 = RewardUI.transform.Find("CardDescription1").GetComponent<TextMeshProUGUI>();

        if (cardDescription2 == null)
            cardDescription2 = RewardUI.transform.Find("CardDescription2").GetComponent<TextMeshProUGUI>();

        if (energyCost1 == null)
            energyCost1 = RewardUI.transform.Find("EnergyCost1").GetComponent<TextMeshProUGUI>();

        if (energyCost2 == null)
            energyCost2 = RewardUI.transform.Find("EnergyCost2").GetComponent<TextMeshProUGUI>();

        cardReward1.GetComponent<Button>().onClick.AddListener(() => SelectCard(1));
        cardReward2.GetComponent<Button>().onClick.AddListener(() => SelectCard(2));

        RewardUI.SetActive(false);

    }

    public void AssignRandomCards()
    {
        RewardUI.SetActive(true);

        ResetHighlights();

        if (cardManager.AvailableCards.Count > 1)
        {
            // Assign the first random card
            int randomIndex1 = Random.Range(0, cardManager.AvailableCards.Count);
            Card card1 = cardManager.AvailableCards[randomIndex1];
            cardReward1.GetComponent<Image>().sprite = card1.cardSprite;
            cardDescription1.text = card1.cardDescription;
            energyCost1.text = card1.energyCost.ToString();

            // Assign the second random card
            int randomIndex2;

            do
            {
                randomIndex2 = Random.Range(0, cardManager.AvailableCards.Count);
            } while (randomIndex2 == randomIndex1);

            Card card2 = cardManager.AvailableCards[randomIndex2];
            cardReward2.GetComponent<Image>().sprite = card2.cardSprite;
            cardDescription2.text = card2.cardDescription;
            energyCost2.text = card2.energyCost.ToString();
        }
        else if (cardManager.AvailableCards.Count == 1)
        {
            Card card = cardManager.AvailableCards[0];
            cardReward1.GetComponent<Image>().sprite = card.cardSprite;
            cardReward2.GetComponent<Image>().sprite = card.cardSprite;

            cardDescription1.text = card.cardDescription;
            cardDescription2.text = card.cardDescription;

            energyCost1.text = card.energyCost.ToString();
            energyCost2.text = card.energyCost.ToString();
        }
        else
        {
            Debug.LogError("No cards available in availableCards list!");
        }
    }

    private void SelectCard(int cardIndex)
    {
        if (cardIndex == 1)
        {
            selectedCard = cardManager.AvailableCards.Find(card => card.cardSprite == cardReward1.GetComponent<Image>().sprite);
            HighlightSelectedCard(cardReward1);
            UnhighlightCard(cardReward2);
        }
        else if (cardIndex == 2)
        {
            selectedCard = cardManager.AvailableCards.Find(card => card.cardSprite == cardReward2.GetComponent<Image>().sprite);
            HighlightSelectedCard(cardReward2);
            UnhighlightCard(cardReward1);
        }
        Debug.Log("Selected Card: " + selectedCard.cardName);

    }


    private void HighlightSelectedCard(GameObject cardObject)
    {
        // Add your logic here to visually highlight the selected card
        cardObject.GetComponent<Image>().color = Color.green; // Example of changing color
    }

    private void UnhighlightCard(GameObject cardObject)
    {
        // Add your logic here to revert the visual highlighting
        cardObject.GetComponent<Image>().color = Color.white; // Revert back to original color
    }

    public void ResetHighlights()
    {
        // Reset the highlight color for both cardReward1 and cardReward2
        UnhighlightCard(cardReward1);
        UnhighlightCard(cardReward2);

        // Clear the selected card
        selectedCard = null;
    }

    public void TransferSelectedCardToNextLevel()
    {
        if (selectedCard != null)
        {
            // Add the selected card to the player's deck for the next level
            Debug.Log("Transferring selected card: " + selectedCard.cardName);
            cardManager.deckPile.Add(selectedCard);
        }
        else
        {
            Debug.LogWarning("No card selected!");
        }
    }

}
