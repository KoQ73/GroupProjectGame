using UnityEngine;
using UnityEngine.UI;

public class RewardUI : MonoBehaviour
{
    public Image cardReward1;          // Reference to the first card Image component
    public Image cardReward2;          // Reference to the second card Image component
    public Sprite[] cardSprites;       // Array of sprites to choose from

    void Start()
    {
        AssignRandomCards();
    }

    void AssignRandomCards()
    {
        if (cardSprites.Length > 0)
        {
            // Assign a random sprite to cardReward1
            int randomIndex1 = Random.Range(0, cardSprites.Length);
            cardReward1.sprite = cardSprites[randomIndex1];

            // Assign a different random sprite to cardReward2
            int randomIndex2 = Random.Range(0, cardSprites.Length);
            cardReward2.sprite = cardSprites[randomIndex2];
        }
    }
}
