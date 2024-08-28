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
    [SerializeField] Sprite healImage;
    [SerializeField] Sprite circularAttackImage;
    [SerializeField] Sprite executeImage;
    [SerializeField] Sprite heavyAttackImage;

    public List<Card> deckPile;

    List<Card> discardPile;
    List<Card> cardsInHand;

    List<Card> availableCards;
    public List<Card> AvailableCards { get { return availableCards; } }

    GameObject clickedCard;
    bool movementCardUsed;
    int totalEnergy;

    GameObject cardContainer;
    GameObject endTurnBtn;
    GameObject cardImageUI;
    GameObject confirmationUI;
    GameObject confirmBtn;
    GameObject cancelBtn;
    GameObject deckAndEnergyContainer;
    GameObject energyNumber;
    GameObject showDeckCount;
    GameObject showDiscardCount;

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
        deckAndEnergyContainer = GameObject.Find("DeckAndEnergyNumbers");
        energyNumber = GameObject.Find("EnergyNumber");
        showDeckCount = GameObject.Find("DeckNumber");
        showDiscardCount = GameObject.Find("DiscardNumber");

        cardImageUI.SetActive(false);
        confirmationUI.SetActive(false);

        clickedCard = new GameObject();
        movementCardUsed = false;
        totalEnergy = 3;

        deckPile = new List<Card>();
        discardPile = new List<Card>();
        cardsInHand = new List<Card>();
        availableCards = new List<Card>();

        playerController = FindObjectOfType<PlayerController>();

        CardsInGame();
        PopulateDefaultDeck();
        ShuffleDeck();
        //StartTurnCardsInHand();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DeckAndEnergyNumberUpdate()
    {
        energyNumber.GetComponent<TextMeshProUGUI>().text = totalEnergy.ToString();
        showDeckCount.GetComponent<TextMeshProUGUI>().text = deckPile.Count.ToString();
        showDiscardCount.GetComponent<TextMeshProUGUI>().text = discardPile.Count.ToString();
    }

    private void CardsInGame()
    {
        //Basic Attack Card [0]
        int energyCost = 1;
        int cardValue = 4;
        int moveValue = 0;
        string cardName = "Basic Attack";
        string cardDescription = "Deal " + cardValue + " damage 1 space forward";
        Card basicAttackCard = new Card(energyCost, cardValue, moveValue, cardName, cardDescription, basicAttackImage);

        availableCards.Add(basicAttackCard);

        //Basic Block Card [1]
        energyCost = 1;
        cardValue = 4;
        moveValue = 0;
        cardName = "Basic Block";
        cardDescription = "Block " + cardValue + " damage";
        Card basicBlockCard = new Card(energyCost, cardValue, moveValue, cardName, cardDescription, basicBlockImage);

        availableCards.Add(basicBlockCard);

        //Heal Card [2]
        energyCost = 1;
        cardValue = 5;
        moveValue = 0;
        cardName = "Heal";
        cardDescription = "Heal " + cardValue;
        Card healCard = new Card(energyCost, cardValue, moveValue, cardName, cardDescription, healImage);

        availableCards.Add(healCard);

        //Circular Attack Card [3]
        energyCost = 1;
        cardValue = 3;
        moveValue = 0;
        cardName = "Circular Attack";
        cardDescription = "Deal " + cardValue + " damage around";
        Card circularAttackCard = new Card(energyCost, cardValue, moveValue, cardName, cardDescription, circularAttackImage);

        availableCards.Add(circularAttackCard);

        //Execute Card [4]
        energyCost = 2;
        cardValue = 2;
        moveValue = 0;
        cardName = "Execute";
        cardDescription = "Deal " + cardValue + " damage\nIf target < 30%, it immediately dies";
        Card executeCard = new Card(energyCost, cardValue, moveValue, cardName, cardDescription, executeImage);

        availableCards.Add(executeCard);

        //Heavy Attack Card [5]
        energyCost = 2;
        cardValue = 12;
        moveValue = 0;
        cardName = "Heavy Attack";
        cardDescription = "Deal " + cardValue + " damage 1 space forward"; ;
        Card heavyAttackCard = new Card(energyCost, cardValue, moveValue, cardName, cardDescription, heavyAttackImage);

        availableCards.Add(heavyAttackCard);
    }

    private void PopulateDefaultDeck()
    {
        //Add Basic Attack Cards
        for (int i = 0; i < 4; i++)
        {
            deckPile.Add(availableCards[0]);
        }

        //Add Basic Block Cards
        for (int i = 0; i < 4; i++)
        {
            deckPile.Add(availableCards[1]);
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
        deckAndEnergyContainer.SetActive(true);

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
        firstCard.GetComponent<Image>().color = Color.white;
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
                ShuffleDiscardBack();

                ShuffleDeck();
            }

            GameObject targetCard = cardContainer.transform.Find("CardBtn" + i.ToString()).gameObject;
            Card targetCardVariables = deckPile[0];
            targetCard.SetActive(true);
            targetCard.GetComponent<Button>().onClick.RemoveAllListeners();
            targetCard.GetComponent<Image>().color = Color.white;

            DesignateCardProperties(targetCard, targetCardVariables);

            cardsInHand.Add(targetCardVariables);
            deckPile.RemoveAt(0);
        }

        //Update Deck and Energy numbers
        DeckAndEnergyNumberUpdate();
    }

    public void ShuffleDiscardBack()
    {
        foreach (Card card in discardPile)
        {
            deckPile.Add(card);
        }

        discardPile.Clear();
    }

    public void resetDeck()
    {
        foreach (Card card in discardPile)
        {
            deckPile.Add(card);
        }

        discardPile.Clear();

        foreach (Card card in cardsInHand)
        {
            deckPile.Add(card);
        }

        cardsInHand.Clear();
    }

    private void ShowRemaindingCards()
    {
        //Debug.Log(cardsInHand.Count);
        for (int i = 1; i < 6; i++)
        {
            GameObject targetCard = cardContainer.transform.Find("CardBtn" + i.ToString()).gameObject;
            targetCard.GetComponent<Image>().color = Color.white;
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

                DesignateCardProperties(targetCard, targetCardVariables);
            }
        }
        else
        {
            //Show movement card
            cardContainer.transform.Find("CardBtn1").gameObject.SetActive(true);

            if (totalEnergy < 1)
            {
                cardContainer.transform.Find("CardBtn1").gameObject.GetComponent<Image>().color = Color.grey;
            }

            //show rest of the cards in hand
            for (int i = 2; i < cardsInHand.Count + 2; i++)
            {
                GameObject targetCard = cardContainer.transform.Find("CardBtn" + i.ToString()).gameObject;
                targetCard.SetActive(true);
                targetCard.GetComponent<Button>().onClick.RemoveAllListeners();

                Card targetCardVariables = cardsInHand[i - 2];

                DesignateCardProperties(targetCard, targetCardVariables);
            }
        }

        //Update Deck and Energy numbers
        DeckAndEnergyNumberUpdate();

    }

    private void DesignateCardProperties(GameObject targetCard, Card targetCardVariables)
    {
        targetCard.GetComponent<Image>().sprite = targetCardVariables.cardSprite;
        targetCard.transform.Find("CardDescription").gameObject.GetComponent<TextMeshProUGUI>().text = targetCardVariables.cardDescription;
        targetCard.transform.Find("EnergyCost").gameObject.GetComponent<TextMeshProUGUI>().text = targetCardVariables.energyCost.ToString();

        if (totalEnergy < targetCardVariables.energyCost)
        {
            targetCard.GetComponent<Image>().color = Color.grey;
        }

        if (targetCardVariables.cardName == "Basic Attack" || targetCardVariables.cardName == "Heavy Attack")
        {
            targetCard.GetComponent<Button>().onClick.AddListener(delegate { SetListenerToConfirmation(targetCardVariables.energyCost); });
            targetCard.GetComponent<Button>().onClick.AddListener(delegate { BasicAttackConfirmCancelImage(targetCardVariables); });
            //targetCard.GetComponent<Button>().onClick.AddListener(playerController.AttackCard);
        }
        else if (targetCardVariables.cardName == "Basic Block")
        {
            targetCard.GetComponent<Button>().onClick.AddListener(delegate { SetListenerToConfirmation(targetCardVariables.energyCost); });
            targetCard.GetComponent<Button>().onClick.AddListener(delegate { BasicBlockConfirmCancelImage(targetCardVariables); });
        }
        else if (targetCardVariables.cardName == "Heal")
        {
            targetCard.GetComponent<Button>().onClick.AddListener(delegate { SetListenerToConfirmation(targetCardVariables.energyCost); });
            targetCard.GetComponent<Button>().onClick.AddListener(delegate { HealConfirmCancelImage(targetCardVariables); });
        }
        else if (targetCardVariables.cardName == "Circular Attack")
        {
            targetCard.GetComponent<Button>().onClick.AddListener(delegate { SetListenerToConfirmation(targetCardVariables.energyCost); });
            targetCard.GetComponent<Button>().onClick.AddListener(delegate { CircularAttackConfirmCancelImage(targetCardVariables); });
        }
        else if (targetCardVariables.cardName == "Execute")
        {
            targetCard.GetComponent<Button>().onClick.AddListener(delegate { SetListenerToConfirmation(targetCardVariables.energyCost); });
            targetCard.GetComponent<Button>().onClick.AddListener(delegate { ExecuteConfirmCancelImage(targetCardVariables); });
        }
        else
        {
            targetCard.GetComponent<Button>().onClick.AddListener(delegate { DeleteCard(targetCardVariables); });
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
            deckAndEnergyContainer.SetActive(false);
        }
        
    }

    public void BackToCards()
    {
        cardImageUI.SetActive(false);
        confirmationUI.SetActive(false);
        cardContainer.SetActive(true);
        endTurnBtn.SetActive(true);
        deckAndEnergyContainer.SetActive(true);
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
        deckAndEnergyContainer.SetActive(false);
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
        if (totalEnergy >= card.energyCost)
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

    private void BasicBlockConfirmCancelImage(Card card)
    {
        if (totalEnergy >= card.energyCost)
        {
            confirmBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            cancelBtn.GetComponent<Button>().onClick.RemoveAllListeners();

            confirmBtn.GetComponent<Button>().onClick.AddListener(playerController.ConfirmShieldCard);
            confirmBtn.GetComponent<Button>().onClick.AddListener(delegate { DeleteCard(card); });
            confirmBtn.GetComponent<Button>().onClick.AddListener(BackToCards);
            //confirmBtn.GetComponent<Button>().onClick.AddListener(DeactivateMoveCard);
            cancelBtn.GetComponent<Button>().onClick.AddListener(delegate { playerController.CancelShieldCard(card.cardValue); });
            cancelBtn.GetComponent<Button>().onClick.AddListener(BackToCards);

            cardImageUI.GetComponent<Image>().sprite = card.cardSprite;
            cardImageUI.transform.Find("CardDescription").gameObject.GetComponent<TextMeshProUGUI>().text = card.cardDescription;
            cardImageUI.transform.Find("EnergyCost").gameObject.GetComponent<TextMeshProUGUI>().text = card.energyCost.ToString();

            playerController.ShieldCard(card.cardValue);
        }

    }

    private void HealConfirmCancelImage(Card card)
    {
        if (totalEnergy >= card.energyCost)
        {
            confirmBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            cancelBtn.GetComponent<Button>().onClick.RemoveAllListeners();

            confirmBtn.GetComponent<Button>().onClick.AddListener(playerController.ConfirmHealCard);
            confirmBtn.GetComponent<Button>().onClick.AddListener(delegate { DeleteCard(card); });
            confirmBtn.GetComponent<Button>().onClick.AddListener(BackToCards);
            //confirmBtn.GetComponent<Button>().onClick.AddListener(DeactivateMoveCard);
            cancelBtn.GetComponent<Button>().onClick.AddListener(playerController.CancelHealCard);
            cancelBtn.GetComponent<Button>().onClick.AddListener(BackToCards);

            cardImageUI.GetComponent<Image>().sprite = card.cardSprite;
            cardImageUI.transform.Find("CardDescription").gameObject.GetComponent<TextMeshProUGUI>().text = card.cardDescription;
            cardImageUI.transform.Find("EnergyCost").gameObject.GetComponent<TextMeshProUGUI>().text = card.energyCost.ToString();

            playerController.HealCard(card.cardValue);
        }

    }

    private void CircularAttackConfirmCancelImage(Card card)
    {
        if (totalEnergy >= card.energyCost)
        {
            confirmBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            cancelBtn.GetComponent<Button>().onClick.RemoveAllListeners();

            confirmBtn.GetComponent<Button>().onClick.AddListener(delegate { playerController.ConfirmSlashAttackCard(card.cardValue); });
            confirmBtn.GetComponent<Button>().onClick.AddListener(delegate { DeleteCard(card); });
            confirmBtn.GetComponent<Button>().onClick.AddListener(BackToCards);
            //confirmBtn.GetComponent<Button>().onClick.AddListener(DeactivateMoveCard);
            cancelBtn.GetComponent<Button>().onClick.AddListener(playerController.CancelSlashAttackCard);
            cancelBtn.GetComponent<Button>().onClick.AddListener(BackToCards);

            cardImageUI.GetComponent<Image>().sprite = card.cardSprite;
            cardImageUI.transform.Find("CardDescription").gameObject.GetComponent<TextMeshProUGUI>().text = card.cardDescription;
            cardImageUI.transform.Find("EnergyCost").gameObject.GetComponent<TextMeshProUGUI>().text = card.energyCost.ToString();

            playerController.SlashAttackCard();
        }

    }

    private void ExecuteConfirmCancelImage(Card card)
    {
        if (totalEnergy >= card.energyCost)
        {
            confirmBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            cancelBtn.GetComponent<Button>().onClick.RemoveAllListeners();

            confirmBtn.GetComponent<Button>().onClick.AddListener(delegate { playerController.ConfirmExecuteCard(card.cardValue); });
            confirmBtn.GetComponent<Button>().onClick.AddListener(delegate { DeleteCard(card); });
            confirmBtn.GetComponent<Button>().onClick.AddListener(BackToCards);
            //confirmBtn.GetComponent<Button>().onClick.AddListener(DeactivateMoveCard);
            cancelBtn.GetComponent<Button>().onClick.AddListener(playerController.CancelExecuteCard);
            cancelBtn.GetComponent<Button>().onClick.AddListener(BackToCards);

            cardImageUI.GetComponent<Image>().sprite = card.cardSprite;
            cardImageUI.transform.Find("CardDescription").gameObject.GetComponent<TextMeshProUGUI>().text = card.cardDescription;
            cardImageUI.transform.Find("EnergyCost").gameObject.GetComponent<TextMeshProUGUI>().text = card.energyCost.ToString();

            playerController.ExecuteCard();
        }

    }
}
