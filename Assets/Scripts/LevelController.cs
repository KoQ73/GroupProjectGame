using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    int level;
    int basicUnitNumber;
    int strongerUnitNumber;
    int obstacleNumber;

    Vector2Int playerPosition;
    string playerDirection;

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

        LoadLevel();
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

        cardManager.ShuffleDiscardBack();
        cardManager.StartTurnCardsInHand();
        //Set units and obstacles

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
            basicUnitNumber = 3;
            unitController.PopulateObstacles(obstacleNumber);
            unitController.PopulateUnits(basicUnitNumber, typesOfEnemies[0]);
        }
        else if (level == 4)
        {
            basicUnitNumber = 4;
            unitController.PopulateObstacles(obstacleNumber);
            unitController.PopulateUnits(basicUnitNumber, typesOfEnemies[0]);
        }
        else if (level == 5)
        {
            basicUnitNumber = 2;
            strongerUnitNumber = 1;
            unitController.PopulateObstacles(obstacleNumber);
            unitController.PopulateUnits(basicUnitNumber, typesOfEnemies[0]);
            unitController.PopulateUnits(strongerUnitNumber, typesOfEnemies[1]);
        }
    }
}
