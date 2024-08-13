using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] float movementSpeed = 1f;
    [SerializeField] int moveDistance = 3;
    [SerializeField] Material movableTile;
    [SerializeField] Material nonMovableTile;

    Transform selectedUnit;
    List<Vector2Int> movableCords;

    GridManager gridManager;


    // Start is called before the first frame update
    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        movableCords = new List<Vector2Int>();
        selectedUnit = GameObject.FindGameObjectWithTag("Player").transform;

        PopulateMovableArea();
        EnableMovableTiles();

    }

    // Update is called once per frame
    private void Update()
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
                    DisableMovableTiles();

                    Vector2Int targetCords = hit.transform.GetComponent<Labeller>().cords;
                    //Vector2Int startCords = new Vector2Int((int)selectedUnit.position.x, (int)selectedUnit.position.y) / gridManager.UnityGridSize;

                    selectedUnit.transform.position = new Vector3(targetCords.x, selectedUnit.position.y, targetCords.y);

                    PopulateMovableArea();

                    EnableMovableTiles();

                }
            }
        }
    }

    private void PopulateMovableArea()
    {
        movableCords.Clear();
        int xCord = (int)selectedUnit.position.x;
        int yCord = (int)selectedUnit.position.z;


        for (int x = 0; x < moveDistance; x++)
        {
            for (int y = 0; y < moveDistance - x; y++)
            {
                int newXCord = xCord + x;
                int newYCord = yCord + y;

                if (!coordsExist(newXCord, newYCord))
                {
                    movableCords.Add(new Vector2Int(newXCord, newYCord));
                }

                newXCord = xCord + x;
                newYCord = yCord - y;

                if (!coordsExist(newXCord, newYCord))
                {
                    movableCords.Add(new Vector2Int(newXCord, newYCord));
                }

                newXCord = xCord - x;
                newYCord = yCord + y;

                if (!coordsExist(newXCord, newYCord))
                {
                    movableCords.Add(new Vector2Int(newXCord, newYCord));
                }

                newXCord = xCord - x;
                newYCord = yCord - y;

                if (!coordsExist(newXCord, newYCord))
                {
                    movableCords.Add(new Vector2Int(newXCord, newYCord));
                }
            }
        }

        for (int i = 0; i < movableCords.Count; i++)
        {
            Debug.Log(movableCords[i].ToString());
        }
        
    }


    private bool coordsExist(int x, int y)
    {

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
                tileSteppedOn.GetComponentInChildren<MeshRenderer>().material = nonMovableTile;
            }
        }
    }
}
