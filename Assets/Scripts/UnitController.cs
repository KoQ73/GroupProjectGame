using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField] float movementSpeed = 0.5f;
    [SerializeField] GameObject unitObject;
    [SerializeField] GameObject obstacleObject;
    [SerializeField] GameObject loseScreen;

    Unit selectedUnit;
    bool isDefeated;
    //bool enemyCleared;

    List<Unit> units = new List<Unit>();
    List<Unit> obstacles = new List<Unit>();

    public List<Unit> Units { get { return units; } }

    /*public bool EnemyCleared
    {
        get { return enemyCleared; }
        set { enemyCleared = value; }
    }*/

    List<Tile> pathList = new List<Tile>();

    GridManager gridManager;
    CardManager cardManager;
    PathFinderA pathFinder;
    PlayerController playerController;
    GameObject player;

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        cardManager = FindObjectOfType<CardManager>();
        playerController = FindObjectOfType<PlayerController>();
        pathFinder = new PathFinderA();
        player = GameObject.FindGameObjectWithTag("Player");
        isDefeated = false;

        PopulateObstacles(4);
        PopulateUnits(5);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PopulateObstacles(int number)
    {
        //Removes each obstacle currently in the list if any
        foreach (Unit obstacle in obstacles)
        {
            Destroy(obstacle.unitGameObject);
            gridManager.ReleaseTile(obstacle.cords);
            obstacles.Remove(obstacle);
        }

        //obstacles.Clear();

        for (int i = 0; i < number; i++)
        {
            Vector2Int obstacleCords = new Vector2Int();
            bool cordsTaken = true;

            while (cordsTaken)
            {
                obstacleCords = new Vector2Int(Random.Range(0, gridManager.GridSize.x), Random.Range(0, gridManager.GridSize.y));

                cordsTaken = coordsExist(obstacleCords);
            }

            GameObject gameObject = (GameObject)Instantiate(obstacleObject, new Vector3(obstacleCords.x, 0.35f, obstacleCords.y), Quaternion.identity);
            gridManager.BlockTile(obstacleCords);
            obstacles.Add(new Unit(obstacleCords, 0, 0, 5, 0, "Null", gameObject));
        }
    }

    public void PopulateUnits(int number)
    {
        //Removes each unit currently in the list if any
        foreach (Unit unit in units)
        {
            Destroy(unit.unitGameObject);
            gridManager.ReleaseTile(unit.cords);
            units.Remove(unit);
        }

        //units.Clear();

        for (int i = 0; i < number; i++)
        {
            Vector2Int unitCords = new Vector2Int();
            bool cordsTaken = true;

            while (cordsTaken)
            {
                unitCords = new Vector2Int(Random.Range(0, gridManager.GridSize.x), Random.Range(0, gridManager.GridSize.y));

                cordsTaken = coordsExist(unitCords);
            }

            GameObject gameObject = (GameObject)Instantiate(unitObject, new Vector3(unitCords.x, 0.55f, unitCords.y), Quaternion.identity);
            gridManager.BlockTile(unitCords);
            units.Add(new Unit(unitCords, 2, movementSpeed, 5, 2, "Basic", gameObject));
        }
    }

    public void ActivateUnits()
    {
        cardManager.DuringUnitTurn();
        Vector2Int playerCords = new Vector2Int((int)player.transform.position.x, (int)player.transform.position.z);

        StartCoroutine(FollowPath(playerCords));
    }

    private bool coordsExist(Vector2Int unitCords)
    {

        foreach (Unit obstacle in obstacles)
        {
            if (obstacle.cords == unitCords)
            {
                return true;
            }
        }

        foreach (Unit unit in units)
        {
            if (unit.cords == unitCords)
            {
                return true;
            }
        }

        for (int x = (int)player.transform.position.x - 1; x < (int)player.transform.position.x + 2; x++)
        {
            for (int y = (int)player.transform.position.z - 1; y < (int)player.transform.position.z + 2; y++)
            {
                Vector2Int safeSpace = new Vector2Int(x, y);

                if (safeSpace == unitCords)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void AttackPlayerMelee(Unit unit)
    {
        List<Vector2Int> targets = new List<Vector2Int>();
        Vector2Int playerCord = new Vector2Int((int)player.transform.position.x, (int)player.transform.position.z);

        Vector2Int topCord = new Vector2Int(unit.cords.x, unit.cords.y + 1);
        Vector2Int bottomCord = new Vector2Int(unit.cords.x, unit.cords.y - 1);
        Vector2Int leftCord = new Vector2Int(unit.cords.x - 1, unit.cords.y);
        Vector2Int rightCord = new Vector2Int(unit.cords.x + 1, unit.cords.y);

        targets.Add(topCord);
        targets.Add(bottomCord);
        targets.Add(leftCord);
        targets.Add(rightCord);

        for (int i = 0; i < targets.Count; i++)
        {
            Vector2Int target = targets[i];
            if (playerCord == target)
            {
                // Adding shield
                int remainingAttack = 0;
                if (playerController.shield > 0)
                {
                    playerController.shield -= unit.attackDmg;
                    if (playerController.shield < 0)
                    {
                        remainingAttack = -playerController.shield;
                        playerController.playerHealth -= remainingAttack;
                    }
                }
                else
                {
                    //Debug.Log("Player Health Before: " + playerController.playerHealth);
                    playerController.playerHealth -= unit.attackDmg;
                    //Debug.Log("Player Health After: " + playerController.playerHealth);
                }

                if (playerController.playerHealth <= 0)
                {
                    isDefeated = true;
                    loseScreen.SetActive(true);
                }

                break;
            }
        }
    }

    IEnumerator FollowPath(Vector2Int playerCords)
    {

        foreach (Unit unit in units)
        {
            if (!isDefeated)
            {
                //gridManager.ReleaseTile(unit.cords);
                pathList = pathFinder.findPath(gridManager.GetTile(unit.cords), gridManager.GetTile(playerCords));
                if (pathList.Count > 0)
                {

                    for (int i = 0; i < unit.moveDistance; i++)
                    {
                        gridManager.ReleaseTile(unit.cords);

                        //Check player's cords
                        if (pathList[i].cords == new Vector2Int((int)player.transform.position.x, (int)player.transform.position.z))
                        {
                            //pathList.RemoveAt(i);
                            gridManager.BlockTile(unit.cords);
                            break;
                        }
                        float travelPercent = 0f;

                        while (travelPercent < 1f)
                        {
                            travelPercent += Time.deltaTime * movementSpeed;
                            unit.unitGameObject.transform.position = Vector3.MoveTowards(unit.unitGameObject.transform.position, new Vector3(pathList[i].transform.position.x, unit.unitGameObject.transform.position.y, pathList[i].transform.position.z), travelPercent);
                            yield return new WaitForEndOfFrame();

                        }
                        //PositionUnitOnTile(unit, i);
                        unit.unitGameObject.transform.LookAt(new Vector3(pathList[i].transform.position.x, unit.unitGameObject.transform.position.y, pathList[i].transform.position.z));

                        unit.cords = pathList[i].cords;
                        gridManager.BlockTile(unit.cords);
                    }

                }

                AttackPlayerMelee(unit);
                // update the player shield to zero after enemy finished attacking
                playerController.shield = 0;
            }
            
        }

        if (!isDefeated)
        {
            cardManager.StartTurnCardsInHand();
        }
    }

    private void PositionUnitOnTile(Unit unit, int pathIndex)
    {
        unit.unitGameObject.transform.position = new Vector3(pathList[pathIndex].transform.position.x, unit.unitGameObject.transform.position.y, pathList[pathIndex].transform.position.z);
    }

}
