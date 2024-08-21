using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardManager : MonoBehaviour
{
    [SerializeField] Sprite basicAttackImage;
    [SerializeField] Sprite basicBlockImage;
    [SerializeField] Sprite moveImage; 

    List<Card> deckPile;
    List<Card> discardPile;
    List<Card> cardsInHand;

    GameObject clickedCard;
    bool movementCardUsed;
    int totalEnergy;

    GameObject cardContainer;
    GameObject endTurnBtn;
    GameObject cardImageUI;
    GameObject confirmationUI;
    GameObject confirmBtn;
    GameObject cancelBtn;

    PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        cardContainer = GameObject.Find("CardDisplay");
        endTurnBtn = GameObject.Find("EndTurnBtn");
        cardImageUI = GameObject.Find("SelectedCardImage");
        confirmationUI = GameObject.Find("ConfirmationDisplay");
        confirmBtn = GameObject.Find("ConfirmationBtn");
        cancelBtn = GameObject.Find("CancelBtn");

        cardImageUI.SetActive(false);
        confirmationUI.SetActive(false);

        clickedCard = new GameObject();
        movementCardUsed = false;
        totalEnergy = 3;

        deckPile = new List<Card>();
        discardPile = new List<Card>();
        cardsInHand = new List<Card>();

        playerController = FindObjectOfType<PlayerController>();

        PopulateDefaultDeck();
        ShuffleDeck();
        StartTurnCardsInHand();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PopulateDefaultDeck()
    {
        for (int i = 0; i < 3; i++)
        {
            int energyCost = 1;
            int cardValue = 4;
            int moveValue = 0;
            string cardName = "Basic Attack";
            string cardDescription = "Deal " + cardValue + " damage 1 space forward";
            Card basicAttackCard = new Card(energyCost, cardValue, moveValue, cardName, cardDescription, basicAttackImage);

            deckPile.Add(basicAttackCard);
        }

        for (int i = 0; i < 3; i++)
        {
            int energyCost = 1;
            int cardValue = 4;
            int moveValue = 0;
            string cardName = "Basic Block";
            string cardDescription = "Block " + cardValue + " damage";
            Card basicBlockCard = new Card(energyCost, cardValue, moveValue, cardName, cardDescription, basicBlockImage);

            deckPile.Add(basicBlockCard);
        }
    }

    private void ShuffleDeck()
    {
        for (int i = deckPile.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            Card temp = deckPile[i];
            deckPile[i] = deckPile[j];
            deckPile[j] = temp;
        }
    }

    //run function when start of player's turn
    public void StartTurnCardsInHand()
    {

        cardContainer.SetActive(true);
        endTurnBtn.SetActive(true);

        totalEnergy = 3;

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            discardPile.Add(cardsInHand[i]);
        }

        cardsInHand.Clear();

        //Add Move Card to first card
        GameObject firstCard = cardContainer.transform.Find("CardBtn1").gameObject;
        firstCard.SetActive(true);
        firstCard.GetComponent<Button>().onClick.RemoveAllListeners();
        firstCard.GetComponent<Image>().sprite = moveImage;
        firstCard.transform.Find("CardDescription").gameObject.GetComponent<TextMeshProUGUI>().text = "Move 2 blocks";
        firstCard.transform.Find("EnergyCost").gameObject.GetComponent<TextMeshProUGUI>().text = "1";
        firstCard.GetComponent<Button>().onClick.AddListener(delegate { SetListenerToConfirmation(1); });
        firstCard.GetComponent<Button>().onClick.AddListener(MoveConfirmCancelImage);
        //firstCard.GetComponent<Button>().onClick.AddListener(playerController.MovementCard);

        movementCardUsed = false;

        for (int i = 2; i < 6;  i++)
        {

            
            if (deckPile.Count <= 0 && discardPile.Count > 0)
            {
                //Debug.Log("Reached shuffle discard pile");
                //Debug.Log(deckPile.Count);
                //Debug.Log(discardPile.Count);

                foreach (Card card in discardPile)
                {
                    deckPile.Add(card);
                }

                discardPile.Clear();

                ShuffleDeck();

                //Debug.Log("After shuffling");
                //Debug.Log(deckPile.Count);
                //Debug.Log(discardPile.Count);

            }

            GameObject targetCard = cardContainer.transform.Find("CardBtn" + i.ToString()).gameObject;
            Card targetCardVariables = deckPile[0];
            targetCard.SetActive(true);
            targetCard.GetComponent<Button>().onClick.RemoveAllListeners();
            targetCard.GetComponent<Image>().sprite = targetCardVariables.cardSprite;
            targetCard.transform.Find("CardDescription").gameObject.GetComponent<TextMeshProUGUI>().text = targetCardVariables.cardDescription;
            targetCard.transform.Find("EnergyCost").gameObject.GetComponent<TextMeshProUGUI>().text = targetCardVariables.energyCost.ToString();
            
            if (targetCardVariables.cardName == "Basic Attack")
            {
                targetCard.GetComponent<Button>().onClick.AddListener(delegate { SetListenerToConfirmation(targetCardVariables.energyCost); });
                targetCard.GetComponent<Button>().onClick.AddListener(delegate { BasicAttackConfirmCancelImage(targetCardVariables); });
               // targetCard.GetComponent<Button>().onClick.AddListener(playerController.AttackCard);
            }
            else
            {
                targetCard.GetComponent<Button>().onClick.AddListener(delegate { DeleteCard(targetCardVariables); });
            }
            

            cardsInHand.Add(targetCardVariables);
            deckPile.RemoveAt(0);
        }
    }

    private void ShowRemaindingCards()
    {
        //Debug.Log(cardsInHand.Count);
        for (int i = 1; i < 6; i++)
        {
            GameObject targetCard = cardContainer.transform.Find("CardBtn" + i.ToString()).gameObject;
            targetCard.SetActive(false);
        }

        if (movementCardUsed == true)
        {
            for (int i = 1; i < cardsInHand.Count + 1; i++)
            {
                GameObject targetCard = cardContainer.transform.Find("CardBtn" + i.ToString()).gameObject;
                targetCard.SetActive(true);
                targetCard.GetComponent<Button>().onClick.RemoveAllListeners();

                Card targetCardVariables = cardsInHand[i-1];
                targetCard.GetComponent<Image>().sprite = targetCardVariables.cardSprite;
                targetCard.transform.Find("CardDescription").gameObject.GetComponent<TextMeshProUGUI>().text = targetCardVariables.cardDescription;
                targetCard.transform.Find("EnergyCost").gameObject.GetComponent<TextMeshProUGUI>().text = targetCardVariables.energyCost.ToString();
                if (targetCardVariables.cardName == "Basic Attack")
                {
                    targetCard.GetComponent<Button>().onClick.AddListener(delegate { SetListenerToConfirmation(targetCardVariables.energyCost); });
                    targetCard.GetComponent<Button>().onClick.AddListener(delegate { BasicAttackConfirmCancelImage(targetCardVariables); });
                    //targetCard.GetComponent<Button>().onClick.AddListener(playerController.AttackCard);
                }
                else
                {
                    targetCard.GetComponent<Button>().onClick.AddListener(delegate { DeleteCard(targetCardVariables); });
                }
            }
        }
        else
        {
            //Show movement card
            cardContainer.transform.Find("CardBtn1").gameObject.SetActive(true);

            //show rest of the cards in hand
            for (int i = 2; i < cardsInHand.Count + 2; i++)
            {
                GameObject targetCard = cardContainer.transform.Find("CardBtn" + i.ToString()).gameObject;
                targetCard.SetActive(true);
                targetCard.GetComponent<Button>().onClick.RemoveAllListeners();

                Card targetCardVariables = cardsInHand[i - 2];
                targetCard.GetComponent<Image>().sprite = targetCardVariables.cardSprite;
                targetCard.transform.Find("CardDescription").gameObject.GetComponent<TextMeshProUGUI>().text = targetCardVariables.cardDescription;
                targetCard.transform.Find("EnergyCost").gameObject.GetComponent<TextMeshProUGUI>().text = targetCardVariables.energyCost.ToString();
                if (targetCardVariables.cardName == "Basic Attack")
                {
                    targetCard.GetComponent<Button>().onClick.AddListener(delegate { SetListenerToConfirmation(targetCardVariables.energyCost); });
                    targetCard.GetComponent<Button>().onClick.AddListener(delegate { BasicAttackConfirmCancelImage(targetCardVariables); });
                    //targetCard.GetComponent<Button>().onClick.AddListener(playerController.AttackCard);
                }
                else
                {
                    targetCard.GetComponent<Button>().onClick.AddListener(delegate { DeleteCard(targetCardVariables); });
                }
            }
        }
        
    }

    //function for testing card usage
    private void DeleteCard(Card card)
    {
        if (totalEnergy >= card.energyCost)
        {
            totalEnergy -= card.energyCost;
            cardsInHand.Remove(card);
            discardPile.Add(card);

            //update cards in hand
            ShowRemaindingCards();
        }
        
    }

    private void SetListenerToConfirmation(int energyCost)
    {
        if (totalEnergy >= energyCost)
        {
            cardImageUI.SetActive(true);
            confirmationUI.SetActive(true);
            cardContainer.SetActive(false);
            endTurnBtn.SetActive(false);
        }
        
    }

    public void BackToCards()
    {
        cardImageUI.SetActive(false);
        confirmationUI.SetActive(false);
        cardContainer.SetActive(true);
        endTurnBtn.SetActive(true);
        ShowRemaindingCards();
    }

    private void DuringMovement()
    {
        cardImageUI.SetActive(false);
        confirmationUI.SetActive(false);

        totalEnergy -= 1;

        movementCardUsed = true;
    }

    //run when player press end turn and starts units' turn
    public void DuringUnitTurn()
    {
        cardContainer.SetActive(false);
        endTurnBtn.SetActive(false);
    }

    private void DeactivateMoveCard()
    {
        cardContainer.transform.Find("CardBtn1").gameObject.SetActive(false);
    }

    //runs when player clicks on move card
    private void MoveConfirmCancelImage()
    {
        if (totalEnergy >= 1)
        {
            confirmBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            cancelBtn.GetComponent<Button>().onClick.RemoveAllListeners();

            confirmBtn.GetComponent<Button>().onClick.AddListener(playerController.ConfirmMovementCard);
            confirmBtn.GetComponent<Button>().onClick.AddListener(DuringMovement);
            confirmBtn.GetComponent<Button>().onClick.AddListener(DeactivateMoveCard);
            cancelBtn.GetComponent<Button>().onClick.AddListener(playerController.CancelMovementCard);
            cancelBtn.GetComponent<Button>().onClick.AddListener(BackToCards);

            cardImageUI.GetComponent<Image>().sprite = moveImage;
            cardImageUI.transform.Find("CardDescription").gameObject.GetComponent<TextMeshProUGUI>().text = "Move 2 blocks";
            cardImageUI.transform.Find("EnergyCost").gameObject.GetComponent<TextMeshProUGUI>().text = "1";

            playerController.MovementCard();
        }
        
    }

    private void BasicAttackConfirmCancelImage(Card card)
    {
        if (totalEnergy >= 1)
        {
            confirmBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            cancelBtn.GetComponent<Button>().onClick.RemoveAllListeners();

            confirmBtn.GetComponent<Button>().onClick.AddListener(delegate { playerController.ConfirmAttackCard(card.cardValue); });
            confirmBtn.GetComponent<Button>().onClick.AddListener(delegate { DeleteCard(card); });
            confirmBtn.GetComponent<Button>().onClick.AddListener(BackToCards);
            //confirmBtn.GetComponent<Button>().onClick.AddListener(DeactivateMoveCard);
            cancelBtn.GetComponent<Button>().onClick.AddListener(playerController.CancelAttackCard);
            cancelBtn.GetComponent<Button>().onClick.AddListener(BackToCards);

            cardImageUI.GetComponent<Image>().sprite = card.cardSprite;
            cardImageUI.transform.Find("CardDescription").gameObject.GetComponent<TextMeshProUGUI>().text = card.cardDescription;
            cardImageUI.transform.Find("EnergyCost").gameObject.GetComponent<TextMeshProUGUI>().text = card.energyCost.ToString();

            playerController.AttackCard();
        }

    }
}
