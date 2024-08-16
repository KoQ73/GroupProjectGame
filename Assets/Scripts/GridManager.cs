using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] Vector2Int gridSize;
    [SerializeField] int unityGridSize;
    public int UnityGridSize { get { return unityGridSize; } }
    public Vector2Int GridSize { get { return gridSize; } }

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

        CreateTile();
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

    private void CreateTile()
    {
        tileMap.Clear();
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2Int cords = new Vector2Int(x, y);

                GameObject tile = Instantiate(tilePrefab, tileContainer.transform);
                tile.transform.position = new Vector3(x, 0, y);

                tileMap.Add(cords, tile.gameObject.GetComponent<Tile>());
            }
        }
    }
}
