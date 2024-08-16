using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] Vector2Int gridSize;
    [SerializeField] int unityGridSize;
    public int UnityGridSize { get { return unityGridSize; } }
    public Vector2Int GridSize { get { return gridSize; } }

    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();
    public Dictionary<Vector2Int, Node> Grid { get { return grid; } }

    public Dictionary<Vector2Int, Tile> tileMap = new Dictionary<Vector2Int, Tile>();

    private static GridManager _instance;
    public static GridManager Instance { get { return _instance; } }

    public GameObject tilePrefab;
    public GameObject tileContainer;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        CreateGrid();
    }

    public Tile GetTile(Vector2Int coordinates)
    {
        if (tileMap.ContainsKey(coordinates))
        {
            return tileMap[coordinates];
        }

        return null;
    }

    public List<Tile> GetNeighbourTiles(Tile currentTile, List<Tile> searchableTiles)
    {
        Dictionary<Vector2Int, Tile> tileToSearch = new Dictionary<Vector2Int, Tile>();

        if (searchableTiles.Count > 0)
        {
            foreach (Tile item in searchableTiles)
            {
                tileToSearch.Add(item.cords, item);
            }
        }
        else
        {
            tileToSearch = tileMap;
        }

        List<Tile> neighbours = new List<Tile>();

        //top
        Vector2Int locationToCheck = new Vector2Int(currentTile.cords.x, currentTile.cords.y + 1);

        if (tileToSearch.ContainsKey(locationToCheck))
        {
            neighbours.Add(tileToSearch[locationToCheck]);
        }

        //bottom
        locationToCheck = new Vector2Int(currentTile.cords.x, currentTile.cords.y - 1);

        if (tileToSearch.ContainsKey(locationToCheck))
        {
            neighbours.Add(tileToSearch[locationToCheck]);
        }

        //right
        locationToCheck = new Vector2Int(currentTile.cords.x + 1, currentTile.cords.y);

        if (tileToSearch.ContainsKey(locationToCheck))
        {
            neighbours.Add(tileToSearch[locationToCheck]);
        }

        //left
        locationToCheck = new Vector2Int(currentTile.cords.x - 1, currentTile.cords.y);

        if (tileToSearch.ContainsKey(locationToCheck))
        {
            neighbours.Add(tileToSearch[locationToCheck]);
        }

        return neighbours;
    }

    public void BlockTile(Vector2Int coordinates)
    {
        if (tileMap.ContainsKey(coordinates))
        {
            tileMap[coordinates].isBlocked = true;
        }
    }

    public void ReleaseTile(Vector2Int coordinates)
    {
        if (tileMap.ContainsKey(coordinates))
        {
            tileMap[coordinates].isBlocked = false;
        }
    }

    public Node GetNode(Vector2Int coordinates)
    {
        if (grid.ContainsKey(coordinates))
        {
            return grid[coordinates];
        }

        return null;
    }

    public void BlockNode(Vector2Int coordinates)
    {
        if (grid.ContainsKey(coordinates))
        {
            grid[coordinates].walkable = false;
        }
    } 

    public void ResetNodes()
    {
        foreach (KeyValuePair<Vector2Int, Node> entry in grid)
        {
            entry.Value.connectTo = null;
            entry.Value.explored = false;
            entry.Value.path = false;
        }
    }

    public Vector2Int GetCoordinatesFromPosition(Vector3 position)
    {
        Vector2Int coordinates = new Vector2Int();

        coordinates.x = Mathf.RoundToInt(position.x / unityGridSize);
        coordinates.y = Mathf.RoundToInt(position.z / unityGridSize);

        return coordinates;
    }

    public Vector3 GetPositionFromCoordinates(Vector2Int coordinates, float y)
    {
        Vector3 position = new Vector3();

        position.x = coordinates.x * unityGridSize;
        position.y = y;
        position.z = coordinates.y * unityGridSize;

        return position;
    }

    private void CreateGrid()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2Int cords = new Vector2Int(x, y);
                grid.Add(cords, new Node(cords, true));

                GameObject tile = Instantiate(tilePrefab, tileContainer.transform);
                tile.transform.position = new Vector3(x, 0, y);

                tileMap.Add(cords, tile.gameObject.GetComponent<Tile>());

                //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //Vector3 position = new Vector3(cords.x * unitGridSize, 0f, cords.y * unitGridSize);
                //cube.transform.position = position;
                //cube.transform.SetParent(transform);
            }
        }
    }
}
