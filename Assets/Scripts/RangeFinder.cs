using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangeFinder
{
    public List<Tile> GetTilesInRange(Tile startingTile, int range)
    {
        List<Tile> inRangeTiles = new List<Tile>();
        int stepCount = 0;

        inRangeTiles.Add(startingTile);

        List<Tile> tileForPreviousStep = new List<Tile>();
        tileForPreviousStep.Add(startingTile);

        while (stepCount < range)
        {
            List<Tile> surroundingTiles = new List<Tile>();

            foreach (Tile item in tileForPreviousStep)
            {
                surroundingTiles.AddRange(GridManager.Instance.GetNeighbourTiles(item, new List<Tile>()));
            }

            inRangeTiles.AddRange(surroundingTiles);
            tileForPreviousStep = surroundingTiles.Distinct().ToList();
            stepCount++;
        }

        return inRangeTiles.Distinct().ToList();
    }
}
