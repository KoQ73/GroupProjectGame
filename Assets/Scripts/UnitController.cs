using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField] float movementSpeed = 0.5f;
    [SerializeField] GameObject unitObject;
    [SerializeField] GameObject basicUnit;
    [SerializeField] GameObject StrongerUnit;
    [SerializeField] GameObject obstacleObject;
    [SerializeField] GameObject loseScreen;

    private AudioManager audioManager;

    Unit selectedUnit;
    bool isDefeated;
    //bool enemyCleared;

    List<Unit> units = new List<Unit>();
    List<Unit> obstacles = new List<Unit>();

    public List<Unit> Units { get { return units; } }
    public List<Unit> Obstacles { get { return obstacles; } }

    List<Unit> typesOfUnits = new List<Unit>();
    public List<Unit> TypesOfUnits { get { return typesOfUnits; } }

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
    Animator animator;

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        cardManager = FindObjectOfType<CardManager>();
        playerController = FindObjectOfType<PlayerController>();
        pathFinder = new PathFinderA();
        player = GameObject.FindGameObjectWithTag("Player");
        isDefeated = false;

        PopulateTypesOfUnits();
        //PopulateObstacles(4);
        //PopulateUnits(5);
        audioManager = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PopulateTypesOfUnits()
    {
        typesOfUnits.Clear();
        //Basic Enemy [0]
        typesOfUnits.Add(new Unit(new Vector2Int(), 2, movementSpeed, 15, 15, 5, "Basic", "Basic Enemy", null));

        //Stronger Enemy [1]
        typesOfUnits.Add(new Unit(new Vector2Int(), 1, movementSpeed, 30, 30, 9, "Basic", "Stronger Enemy", null));
    }

    public void PopulateObstacles(int number)
    {
        //Removes each obstacle currently in the list if any
        /*foreach (Unit obstacle in obstacles)
        {
            Destroy(obstacle.unitGameObject);
            gridManager.ReleaseTile(obstacle.cords);
            obstacles.Remove(obstacle);
        }*/

        //obstacles.Clear();

        for (int i = 0; i < number; i++)
        {
            Vector2Int obstacleCords = new Vector2Int();
            bool cordsTaken = true;

            while (cordsTaken)
            {
                obstacleCords = new Vector2Int(UnityEngine.Random.Range(0, gridManager.GridSize.x), UnityEngine.Random.Range(0, gridManager.GridSize.y));

                cordsTaken = coordsExist(obstacleCords);
            }

            GameObject gameObject = (GameObject)Instantiate(obstacleObject, new Vector3(obstacleCords.x, 0.35f, obstacleCords.y), Quaternion.identity);
            gridManager.BlockTile(obstacleCords);
            obstacles.Add(new Unit(obstacleCords, 0, 0, 1, 1, 0, "Null", "Obstacle", gameObject));
        }
    }

    public void ClearAllUnits()
    {
        //Removes each obstacle currently in the list if any
        foreach (Unit obstacle in obstacles)
        {
            Destroy(obstacle.unitGameObject);
            gridManager.ReleaseTile(obstacle.cords);
            //obstacles.Remove(obstacle);
        }
        obstacles.Clear();

        //Removes each unit currently in the list if any
        foreach (Unit unit in units)
        {
            Destroy(unit.unitGameObject);
            gridManager.ReleaseTile(unit.cords);
            //units.Remove(unit);
        }
        units.Clear();
    }

    public void PopulateUnits(int number, Unit selectedUnit)
    {
        //Removes each unit currently in the list if any
        /*foreach (Unit unit in units)
        {
            Destroy(unit.unitGameObject);
            gridManager.ReleaseTile(unit.cords);
            units.Remove(unit);
        }*/

        //units.Clear();

        for (int i = 0; i < number; i++)
        {
            Vector2Int unitCords = new Vector2Int();
            bool cordsTaken = true;

            while (cordsTaken)
            {
                unitCords = new Vector2Int(UnityEngine.Random.Range(0, gridManager.GridSize.x), UnityEngine.Random.Range(0, gridManager.GridSize.y));

                cordsTaken = coordsExist(unitCords);
            }

            GameObject gameObject = new GameObject();

            if (selectedUnit.unitName == "Basic Enemy")
            {
                gameObject = (GameObject)Instantiate(basicUnit, new Vector3(unitCords.x, 0.1f, unitCords.y), Quaternion.identity);
            }
            else if (selectedUnit.unitName == "Stronger Enemy")
            {
                gameObject = (GameObject)Instantiate(StrongerUnit, new Vector3(unitCords.x, 0.1f, unitCords.y), Quaternion.identity);
            }

            gridManager.BlockTile(unitCords);

            //Add unit
            units.Add(new Unit(unitCords, selectedUnit.moveDistance, movementSpeed, selectedUnit.health, selectedUnit.maxHealth, selectedUnit.attackDmg, selectedUnit.attackName, selectedUnit.unitName, gameObject));
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

        // Get animator
        Animator a1;
        a1 = unit.unitGameObject.GetComponent<Animator>();

        for (int i = 0; i < targets.Count; i++)
        {
            Vector2Int target = targets[i];
            if (playerCord == target)
            {
                // Add animation
                unit.unitGameObject.transform.LookAt(new Vector3(playerCord.x, unit.unitGameObject.transform.position.y, playerCord.y));
                StartCoroutine(WaitAndAnimate(1f, "isAttack", a1));
                // Adding shield
                int remainingAttack = 0;
                if (playerController.shield > 0)
                {
                    playerController.shield -= unit.attackDmg;

                    StartCoroutine(audioManager.WaitAndPlaySFX(0.7f,"shield"));
                    if (playerController.shield < 0)
                    {
                        remainingAttack = -playerController.shield;
                        playerController.playerHealth -= remainingAttack;
                        StartCoroutine(audioManager.WaitAndPlaySFX(1.2f,"attack"));


                    }
                }
                else
                {
                    //Debug.Log("Player Health Before: " + playerController.playerHealth);
                    playerController.playerHealth -= unit.attackDmg;
                    StartCoroutine(audioManager.WaitAndPlaySFX(0.7f,"attack"));

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
            // Move animation for unit
            animator = unit.unitGameObject.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.Log("Animator is not on");
            }
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
                        // Animate moving
                        animator.SetBool("isRunning", true);
                        while (travelPercent < 1f)
                        {
                            // Direction to face for enemy
                            unit.unitGameObject.transform.LookAt(new Vector3(pathList[i].transform.position.x, unit.unitGameObject.transform.position.y, pathList[i].transform.position.z));
                            travelPercent += Time.deltaTime * movementSpeed;

                            var step = movementSpeed * Time.deltaTime;
                            unit.unitGameObject.transform.position = Vector3.MoveTowards(unit.unitGameObject.transform.position, new Vector3(pathList[i].transform.position.x, unit.unitGameObject.transform.position.y, pathList[i].transform.position.z), step);
                            yield return new WaitForEndOfFrame();
                        }
                        //PositionUnitOnTile(unit, i);
                        unit.unitGameObject.transform.LookAt(new Vector3(pathList[i].transform.position.x, unit.unitGameObject.transform.position.y, pathList[i].transform.position.z));

                        unit.cords = pathList[i].cords;

                        gridManager.BlockTile(unit.cords);

                    }
                }
                // Revert back to idle animation
                animator.SetBool("isRunning", false);

                AttackPlayerMelee(unit);
                
            }
            
        }
        // update the player shield to zero after enemy finished attacking
        playerController.shield = 0;
        
        if (!isDefeated)
        {
            cardManager.StartTurnCardsInHand();
        }
    }

    private void PositionUnitOnTile(Unit unit, int pathIndex)
    {
        unit.unitGameObject.transform.position = new Vector3(pathList[pathIndex].transform.position.x, unit.unitGameObject.transform.position.y, pathList[pathIndex].transform.position.z);
    }

    // For animations
    IEnumerator WaitAndAnimate(float sec, string s, Animator a1)
    {
        a1.SetBool(s, true);
        yield return new WaitForSeconds(sec);
        a1.SetBool(s, false);
    }

}
