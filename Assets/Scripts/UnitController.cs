using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField] float movementSpeed = 0.5f;
    [SerializeField] GameObject unitObject;

    Unit selectedUnit;
    int selectedUnitMove = 0;
    bool unitSelected = false;
    bool isMoving = false;

    List<Unit> units = new List<Unit>();

    public List<Unit> Units { get { return units; } }

    List<Tile> pathList = new List<Tile>();

    GridManager gridManager;
    PathFinderA pathFinder;
    GameObject player;

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        pathFinder = new PathFinderA();
        player = GameObject.FindGameObjectWithTag("Player");
        PopulateUnits();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void PopulateUnits()
    {
        units.Clear();

        for (int i = 0; i < 5; i++)
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
        Vector2Int playerCords = new Vector2Int((int)player.transform.position.x, (int)player.transform.position.z);

        StartCoroutine(FollowPath(playerCords));
    }

    private bool coordsExist(Vector2Int unitCords)
    {

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

    IEnumerator FollowPath(Vector2Int playerCords)
    {

        foreach (Unit unit in units)
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


                    unit.cords = pathList[i].cords;
                    gridManager.BlockTile(unit.cords);
                }

            }
        }

    }

    private void PositionUnitOnTile(Unit unit, int pathIndex)
    {
        unit.unitGameObject.transform.position = new Vector3(pathList[pathIndex].transform.position.x, unit.unitGameObject.transform.position.y, pathList[pathIndex].transform.position.z);
    }

}
