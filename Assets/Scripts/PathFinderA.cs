using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinderA
{

    //private Dictionary<Vector2Int, Tile> searchableTiles;

    public List<Tile> findPath(Tile start, Tile end)
    {
        List<Tile> openList = new List<Tile>();
        List<Tile> closedList = new List<Tile>();

        openList.Add(start);

        while (openList.Count > 0)
        {
            Tile currentTile = openList.OrderBy(x => x.F).First();

            openList.Remove(currentTile);
            closedList.Add(currentTile);

            if (currentTile == end)
            {
                //finalize path
                return GetFinishedList(start, end);
            }

            List<Tile> neighbourTiles = GridManager.Instance.GetNeighbourTiles(currentTile, new List<Tile>());

            foreach (Tile neighbour in neighbourTiles)
            {
                if (neighbour.isBlocked || closedList.Contains(neighbour))
                {
                    continue;
                }

                neighbour.G = GetManhattenDistance(start, neighbour);
                neighbour.H = GetManhattenDistance(end, neighbour);

                neighbour.previous = currentTile;

                if (!openList.Contains(neighbour))
                {
                    openList.Add(neighbour);
                }
            }
        }

        return new List<Tile>(); 
    }

    private List<Tile> GetFinishedList(Tile start, Tile end)
    {
        List <Tile> finishedList = new List<Tile>();

        Tile currentTile = end;
        while (currentTile != start)
        {
            finishedList.Add(currentTile);
            currentTile = currentTile.previous;
        }

        finishedList.Reverse();

        return finishedList;
    }

    private int GetManhattenDistance(Tile start, Tile neighbour)
    {
        return Mathf.Abs(start.cords.x - neighbour.cords.x) + Mathf.Abs(start.cords.y - neighbour.cords.y);
    }

    
}
