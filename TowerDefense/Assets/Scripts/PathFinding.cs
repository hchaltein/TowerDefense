using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Algoritmo de PathFinding A*.
// Usado pelos Inimigos para achar rota.
public class PathFinding : MonoBehaviour
{
    // Reference the node grid.
    Grid TileGrid;
    public List<Tile> path;

    // A* cost variables
    public int DiagCost = 14;               // Default = 14 because square root of 2 *10
    public int StraightCost = 10;           // Default = 10 because I assume a 1x1 square for node

    void Awake()
    {
        TileGrid = GameObject.Find("GameMngr").GetComponent<Grid>();
    }

    // Finds path between the two positions
    public void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        // Converts World Position into Grid Coordinates
        Tile startTile = TileGrid.GetTileFromWorld(startPos);
        Tile targetTile = TileGrid.GetTileFromWorld(targetPos);

        // Lists for A*
        List<Tile> openSet = new List<Tile>();              // List of nodes being evaluated
        HashSet<Tile> closedSet = new HashSet<Tile>();      // Chosen nodes for the path that have being looked at.
        openSet.Add(startTile);

        // A* loop:
        while (openSet.Count > 0)
        {
            Tile currentTile = openSet[0];
            // Look at Tile Loop.
            for (int i = 1; i < openSet.Count; i++)
            {   // Get node with cheaper fCost, in case of tie get node wich cheaper hCost
                if (openSet[i].fCost < currentTile.fCost || openSet[i].fCost == currentTile.fCost && openSet[i].hCost < currentTile.hCost)
                {
                    currentTile = openSet[i];
                }
            }
            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            // Path found
            if (currentTile == targetTile)
            {
                path = retraceTilePath(startTile, targetTile);
                return;
            }

            foreach (Tile neighbour in TileGrid.GetTileNeighbours(currentTile))
            {
                // Ignores  UNwalkable neighbours and neighbours already checked.
                if (!neighbour.isWalkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newMovCostToNeighbour = currentTile.gCost + GetTileDistance(currentTile, neighbour);

                if (newMovCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    // Set the neighbour's fCost
                    neighbour.gCost = newMovCostToNeighbour;
                    neighbour.hCost = GetTileDistance(neighbour, targetTile);

                    neighbour.ParentTile = currentTile;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }
    }

    // Returns the path as an ordered list.
    List<Tile> retraceTilePath(Tile startTile, Tile endTile)
    {
        List<Tile> retracedPath = new List<Tile>();
        Tile currentTile = endTile;

        // Go from end node to start node by going looping through the node's parent
        while (currentTile != startTile)
        {
            retracedPath.Add(currentTile);
            currentTile = currentTile.ParentTile;
        }
        // Path gets retraced in reverse inside loop so un-reverse it.
        retracedPath.Reverse();

        // Draw Path using gizmos
        TileGrid.drawnPath = retracedPath;

        return retracedPath;
    }

    // Get Distance between two different Nodes
    int GetTileDistance(Tile tileA, Tile tileB)
    {
        // Calculates Absulotue distance
        int distX = Mathf.Abs(tileA.GridPosX - tileB.GridPosX);
        int distY = Mathf.Abs(tileA.GridPosZ - tileB.GridPosZ);

        // Check to see which axis-distance is shorter
        // Equation => Distance = DiagCost*ShortestDist + StraightCos*(LongestDist - ShortestDist)
        if (distX > distY)
            // YDistance is shortest
            return DiagCost * distY + StraightCost * (distX - distY);
        // Else
        // XDistance is shortest
        return DiagCost * distX + StraightCost * (distY - distX);
    }
}
