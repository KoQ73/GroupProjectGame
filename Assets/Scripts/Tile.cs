using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int cords;

    GridManager gridManager;

    public int G;
    public int H;
    public int F { get { return G + H; } }
    public bool isBlocked = false;

    public Tile previous;

    // Start is called before the first frame update
    void Start()
    {
        SetCords();

        if (isBlocked)
        {
            gridManager.BlockTile(cords);
        }
    }

    private void SetCords()
    {
        gridManager = FindObjectOfType<GridManager>();
        int x = (int)transform.position.x;
        int z = (int)transform.position.z;

        cords = new Vector2Int(x / gridManager.UnityGridSize, z / gridManager.UnityGridSize);
    }
}
