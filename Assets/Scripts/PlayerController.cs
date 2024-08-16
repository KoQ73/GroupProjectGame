using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static UnityEngine.UI.CanvasScaler;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float movementSpeed = 2f;
    [SerializeField] int moveDistance = 2;
    [SerializeField] Material movableTile;
    [SerializeField] Material inactiveTile;
    [SerializeField] Material attackableTile;

    private Vector2Int currentLocation;
    private Vector2Int selectedLocation;
    private bool isMoving = false;
    private bool isAttacking = false;
    private bool move = false;
    private List<Tile> pathList = new List<Tile>();
    private List<Tile> inRangeTiles = new List<Tile>();

    Transform selectedUnit;
    List<Vector2Int> movableCords;
    List<Vector2Int> attackableCords;

    GridManager gridManager;
    PathFinderA pathFinder;
    private RangeFinder rangeFinder;


    // Start is called before the first frame update
    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        pathFinder = new PathFinderA();
        rangeFinder = new RangeFinder();

        movableCords = new List<Vector2Int>();
        attackableCords = new List<Vector2Int>();

        selectedUnit = GameObject.FindGameObjectWithTag("Player").transform;
        currentLocation = new Vector2Int();
        selectedLocation = new Vector2Int();

        //PopulateMovableArea();
        //EnableMovableTiles();

    }

    // Update is called once per frame
    private void Update()
    {
        if (isMoving)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                bool hasHit = Physics.Raycast(ray, out hit);

                if (hasHit)
                {
                    if (hit.transform.tag == "Tile")
                    {
                        selectedLocation = hit.transform.GetComponent<Labeller>().cords;
                        selectedUnit.transform.position = new Vector3(selectedLocation.x, selectedUnit.position.y, selectedLocation.y);

                        pathList = pathFinder.findPath(gridManager.GetTile(currentLocation), gridManager.GetTile(selectedLocation));

                        UnityEngine.Debug.Log(pathList.Count);

                    }
                }
            }
        }

        if (move)
        {
            MoveAlongPath();
        }

        if (isAttacking)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                bool hasHit = Physics.Raycast(ray, out hit);

                if (hasHit)
                {
                    if (hit.transform.tag == "Tile")
                    {
                        selectedLocation = hit.transform.GetComponent<Labeller>().cords;
                        //Check is Selected Location has an enemy
                    }
                }
            }
        }
    }

    private void MoveAlongPath()
    {

        if (pathList.Count > 0)
        {
            var step = movementSpeed * Time.deltaTime;
            selectedUnit.position = Vector3.MoveTowards(selectedUnit.position, new Vector3(pathList[0].transform.position.x, selectedUnit.position.y, pathList[0].transform.position.z), step);

            if (Vector3.Distance(selectedUnit.position, new Vector3(pathList[0].transform.position.x, selectedUnit.position.y, pathList[0].transform.position.z)) < 0.0001f)
            {
                PositionPlayerOnTile();

                pathList.RemoveAt(0);

                if (pathList.Count <= 0)
                {
                    move = false;
                }
                else
                {
                    selectedUnit.LookAt(new Vector3(pathList[0].transform.position.x, selectedUnit.position.y, pathList[0].transform.position.z));
                }
            }
        }
        else
        {
            move = false;
        }


    }

    private void PositionPlayerOnTile()
    {
        selectedUnit.position = new Vector3(pathList[0].transform.position.x, selectedUnit.position.y, pathList[0].transform.position.z);
    }

    public void MovementCard()
    {
        //PopulateMovableArea();

        currentLocation = new Vector2Int((int)selectedUnit.position.x, (int)selectedUnit.position.z);

        GetInRangeTiles();
        EnableMovableTiles();

        isMoving = true;
    }

    public void CancelMovementCard()
    {
        DisableMovableTiles();
        selectedUnit.transform.position = new Vector3(currentLocation.x, selectedUnit.position.y, currentLocation.y);
        isMoving = false;
    }

    public void ConfirmMovementCard()
    {
        DisableMovableTiles();

        selectedUnit.transform.position = new Vector3(currentLocation.x, selectedUnit.position.y, currentLocation.y);

        move = true;
        isMoving = false;
    }

    public void AttackCard()
    {
        PopulateAttackArea("Basic");
        EnableAttackableTiles();

        currentLocation.x = (int)selectedUnit.position.x;
        currentLocation.y = (int)selectedUnit.position.z;
        isAttacking = true;
    }

    public void ConfirmAttackCard()
    {
        DisableAttackableTiles();

        isAttacking = false;
    }

    public void CancelAttackCard()
    {
        DisableAttackableTiles();

        isAttacking = false;
    }

    private void GetInRangeTiles()
    {
        movableCords.Clear();

        inRangeTiles = rangeFinder.GetTilesInRange(gridManager.GetTile(currentLocation), moveDistance);

        foreach (Tile item in inRangeTiles)
        {
            movableCords.Add(item.cords);
        }

        FilterMovableArea();
    }

    private void FilterMovableArea()
    {

        //Check and remove Coordinates that contains enemies
        List<Unit> units = FindObjectOfType<UnitController>().Units;

        foreach (Unit unit in units)
        {
            for (int i = 0; i < movableCords.Count; i++)
            {
                if (new Vector2Int((int)unit.unitGameObject.transform.position.x, (int)unit.unitGameObject.transform.position.z) == movableCords[i])
                {
                    movableCords.RemoveAt(i);
                }
            }
        }

        bool containsImpurities = true;

        while (containsImpurities)
        {
            bool deletedCords = false;

            for (int i = 0; i < movableCords.Count; i++)
            {
                List<Tile> testPath = pathFinder.findPath(gridManager.GetTile(currentLocation), gridManager.GetTile(movableCords[i]));
                if (testPath.Count > moveDistance)
                {
                    movableCords.Remove(movableCords[i]);
                    deletedCords = true;
                }
            }

            if (!deletedCords)
            {
                containsImpurities = false;
            }
        }

        Tile topTile = gridManager.GetTile(new Vector2Int(currentLocation.x, currentLocation.y + 1));
        Tile bottomTile = gridManager.GetTile(new Vector2Int(currentLocation.x, currentLocation.y - 1));
        Tile leftTile = gridManager.GetTile(new Vector2Int(currentLocation.x - 1, currentLocation.y));
        Tile rightTile = gridManager.GetTile(new Vector2Int(currentLocation.x + 1, currentLocation.y));


        if (topTile != null && bottomTile != null && leftTile != null && rightTile != null)
        {
            if (topTile.isBlocked == true && bottomTile.isBlocked == true && leftTile.isBlocked == true && rightTile.isBlocked == true)
            {
                movableCords.Clear();
                movableCords.Add(currentLocation);

            }
        }


        //CheckMoveConnected();
    }

    private void PopulateAttackArea(string type)
    {
        attackableCords.Clear();

        int xCord = (int)selectedUnit.position.x;
        int yCord = (int)selectedUnit.position.z;

        if (type == "Basic")
        {
            //Top
            attackableCords.Add(new Vector2Int(xCord, yCord + 1));
            //Bottom
            attackableCords.Add(new Vector2Int(xCord, yCord - 1));
            //Left
            attackableCords.Add(new Vector2Int(xCord - 1, yCord));
            //Right
            attackableCords.Add(new Vector2Int(xCord + 1, yCord));
        }
    }


    private bool coordsExist(int x, int y)
    {

        if (0 > x || x >= gridManager.GridSize.x || 0 > y || y >= gridManager.GridSize.y)
        {
            return true;
        }

        for (int i = 0; i < movableCords.Count; i++)
        {
            if (movableCords[i].x == x && movableCords[i].y == y)
            {
                return true;
            }
        }

        return false;
    }

    private void EnableMovableTiles()
    {
        for (int i = 0; i < movableCords.Count; i++)
        {
            GameObject tileSteppedOn = GameObject.Find("(" + movableCords[i].x.ToString() + ", " + movableCords[i].y.ToString() + ")");

            if (tileSteppedOn != null)
            {
                tileSteppedOn.GetComponent<BoxCollider>().enabled = true;
                tileSteppedOn.GetComponentInChildren<MeshRenderer>().material = movableTile;
            }
        }
    }

    private void DisableMovableTiles()
    {
        for (int i = 0; i < movableCords.Count; i++)
        {
            GameObject tileSteppedOn = GameObject.Find("(" + movableCords[i].x.ToString() + ", " + movableCords[i].y.ToString() + ")");

            if (tileSteppedOn != null)
            {
                tileSteppedOn.GetComponent<BoxCollider>().enabled = false;
                tileSteppedOn.GetComponentInChildren<MeshRenderer>().material = inactiveTile;
            }
        }
    }

    private void EnableAttackableTiles()
    {
        for (int i = 0; i < attackableCords.Count; i++)
        {
            GameObject tileSteppedOn = GameObject.Find("(" + attackableCords[i].x.ToString() + ", " + attackableCords[i].y.ToString() + ")");

            if (tileSteppedOn != null)
            {
                tileSteppedOn.GetComponent<BoxCollider>().enabled = true;
                tileSteppedOn.GetComponentInChildren<MeshRenderer>().material = attackableTile;
            }
        }
    }

    private void DisableAttackableTiles()
    {
        for (int i = 0; i < attackableCords.Count; i++)
        {
            GameObject tileSteppedOn = GameObject.Find("(" + attackableCords[i].x.ToString() + ", " + attackableCords[i].y.ToString() + ")");

            if (tileSteppedOn != null)
            {
                tileSteppedOn.GetComponent<BoxCollider>().enabled = false;
                tileSteppedOn.GetComponentInChildren<MeshRenderer>().material = inactiveTile;
            }
        }
    }
}