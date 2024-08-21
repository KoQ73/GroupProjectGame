using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    int level;
    int unitNumber;
    int obstacleNumber;

    PlayerController playerController;
    UnitController unitController;
    GridManager gridManager;

    // Start is called before the first frame update
    void Start()
    {
        level = 1;
        unitNumber = 1;
        obstacleNumber = 0;

        playerController = FindObjectOfType<PlayerController>();
        unitController = FindObjectOfType<UnitController>();
        gridManager = FindObjectOfType<GridManager>();

        playerController.selectedUnit = GameObject.FindGameObjectWithTag("Player").transform;

        LoadLevel();
    }

    public void LoadNextLevel()
    {
        level++;

        LoadLevel();
    }

    private void LoadLevel()
    {
        gridManager.ResetTiles();

        unitController.PopulateObstacles(obstacleNumber);
        unitController.PopulateUnits(unitNumber);

        
    }
}
