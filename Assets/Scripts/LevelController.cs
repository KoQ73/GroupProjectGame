using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    int level;
    int basicUnitNumber;
    int strongerUnitNumber;
    int obstacleNumber;

    Vector2Int playerPosition;
    string playerDirection;
    [SerializeField] GameObject transitionPanel;

    PlayerController playerController;
    UnitController unitController;
    CardManager cardManager;
    GridManager gridManager;

    // Start is called before the first frame update
    void Start()
    {
        level = 1;
        basicUnitNumber = 1;
        strongerUnitNumber = 0;
        obstacleNumber = 0;
        playerPosition = new Vector2Int();

        playerController = FindObjectOfType<PlayerController>();
        unitController = FindObjectOfType<UnitController>();
        gridManager = FindObjectOfType<GridManager>();
        cardManager = FindObjectOfType<CardManager>();

        //playerController.selectedUnit = GameObject.FindGameObjectWithTag("Player").transform;
        playerDirection = "Left";
        playerPosition = new Vector2Int(0, Mathf.FloorToInt(gridManager.GridSize.y/2));

        LoadLevel();

    }

    public void LoadNextLevel()
    {
        level++;

        //LoadLevel();
        StartCoroutine(FadeImage(false));
    }

    private void LoadLevel()
    {
        List<Unit> typesOfEnemies = unitController.TypesOfUnits;

        gridManager.ResetTiles();

        //Set player location
        if (playerDirection == "Left")
        {
            playerController.selectedUnit.transform.position = new Vector3(playerPosition.x, playerController.selectedUnit.transform.position.y, playerPosition.y);

            playerPosition = new Vector2Int(gridManager.GridSize.x - 1, Mathf.FloorToInt(gridManager.GridSize.y / 2));

            playerDirection = "Right";
        }
        else if (playerDirection == "Right")
        {
            playerController.selectedUnit.transform.position = new Vector3(playerPosition.x, playerController.selectedUnit.transform.position.y, playerPosition.y);

            playerPosition = new Vector2Int(0, Mathf.FloorToInt(gridManager.GridSize.y / 2));

            playerDirection = "Left";
        }

        cardManager.resetDeck();
        cardManager.StartTurnCardsInHand();
        //Set units and obstacles

        unitController.ClearAllUnits();

        if (level == 1)
        {
            unitController.PopulateObstacles(obstacleNumber);
            unitController.PopulateUnits(basicUnitNumber, typesOfEnemies[0]);
        }
        else if (level == 2)
        {
            basicUnitNumber = 2;
            unitController.PopulateObstacles(obstacleNumber);
            unitController.PopulateUnits(basicUnitNumber, typesOfEnemies[0]);
        }
        else if (level == 3)
        {
            obstacleNumber = 4;
            basicUnitNumber = 3;
            unitController.PopulateObstacles(obstacleNumber);
            unitController.PopulateUnits(basicUnitNumber, typesOfEnemies[0]);
        }
        else if (level == 4)
        {
            obstacleNumber = 4;
            basicUnitNumber = 4;
            unitController.PopulateObstacles(obstacleNumber);
            unitController.PopulateUnits(basicUnitNumber, typesOfEnemies[0]);
        }
        else if (level == 5)
        {
            obstacleNumber = 4;
            basicUnitNumber = 2;
            strongerUnitNumber = 1;
            unitController.PopulateObstacles(obstacleNumber);
            unitController.PopulateUnits(basicUnitNumber, typesOfEnemies[0]);
            unitController.PopulateUnits(strongerUnitNumber, typesOfEnemies[1]);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        StartCoroutine(FadeImage(true));
    }

    IEnumerator FadeImage(bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            for (float i = 1; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                transitionPanel.GetComponent<Image>().color = new Color(0, 0, 0, i);
                yield return null;
            }
        }
        // fade from transparent to opaque
        else
        {
            // loop over 1 second
            for (float i = 0; i <= 1; i += Time.deltaTime)
            {
                // set color with i as alpha
                transitionPanel.GetComponent<Image>().color = new Color(0, 0, 0, i);
                yield return null;
            }

            LoadLevel();
        }

    }
}
