using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Algoritmo de PathFinding A*.
// Usado pelos Inimigos para achar rota.
public class PathFinding : MonoBehaviour
{
    // Referencia ao Grid de Tiles
    Grid TileGrid;

    // Variáveis de custo do A* 
    public int DiagCost = 14;               // Default = 14 because square root of 2 *10
    public int StraightCost = 10;           // Default = 10 because I assume a 1x1 square for node

    void Awake()
    {
        // Pega referência ao grid.
        TileGrid = GameObject.Find("GameMngr").GetComponent<Grid>();
    }

    /// <summary>
    /// Encontra o caminnho entre duas posições.
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="targetPos"></param>
    public bool FindPath(Vector3 startPos, Vector3 targetPos, out List<Tile> path)
    {
        // Converte posições de mundo em coordenadas do Grid.
        Tile startTile = TileGrid.GetTileFromWorld(startPos);
        Tile targetTile = TileGrid.GetTileFromWorld(targetPos);

        // Listas para o A*
        List<Tile> openSet = new List<Tile>();              // Lista de Tiles sendo avaliadas
        HashSet<Tile> closedSet = new HashSet<Tile>();      // Lista "ordenada" de Tiles que foram avaliadas 
        // Inicializa a Lista de Tiles.
        openSet.Add(startTile);

        // A* loop:
        while (openSet.Count > 0)
        {
            Tile currentTile = openSet[0];
            
            // Loop de Avaliação de Tile.
            for (int i = 1; i < openSet.Count; i++)
            {   // Pega a tile com o fCost menor, em caso de empate pega a tile com o hCost menor.
                if (openSet[i].fCost < currentTile.fCost || openSet[i].fCost == currentTile.fCost && openSet[i].hCost < currentTile.hCost)
                {
                    currentTile = openSet[i];
                }
            }

            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            // Encontrou caminho?
            if (currentTile == targetTile)
            {
                path = retraceTilePath(startTile, targetTile);
                return true;
            }

            foreach (Tile neighbour in TileGrid.GetTileNeighbours(currentTile))
            {
                // Ignora vizinhos não andáveis e já checados
                if (!neighbour.isWalkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newMovCostToNeighbour = currentTile.gCost + GetTileDistance(currentTile, neighbour);

                if (newMovCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    // Altera fCost do vizinho
                    neighbour.gCost = newMovCostToNeighbour;
                    neighbour.hCost = GetTileDistance(neighbour, targetTile);

                    neighbour.ParentTile = currentTile;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }// foreach

        } // while

        // A* falhou, não existe caminho.
        path = null;
        return false;
    }

    // Retorna caminho como uma lista ordenada
    List<Tile> retraceTilePath(Tile startTile, Tile endTile)
    {
        List<Tile> retracedPath = new List<Tile>();
        Tile currentTile = endTile;

        // Sai da últma tile até a primeira atrávez das Tiles pais
        while (currentTile != startTile)
        {
            retracedPath.Add(currentTile);
            currentTile = currentTile.ParentTile;
        }
        // Caminho´está ordenado inversamente, então reverte lista.
        retracedPath.Reverse();

        // ARmazena trilha para usar gizmos.
        TileGrid.drawnPath = retracedPath;

        return retracedPath;
    }

    // Pega a distância entre duas Tiles.
    int GetTileDistance(Tile tileA, Tile tileB)
    {
        // Disntância absoluta
        int distX = Mathf.Abs(tileA.GridPosX - tileB.GridPosX);
        int distZ = Mathf.Abs(tileA.GridPosZ - tileB.GridPosZ);

        // Checa qual é o eixo menor
        // Equação heurística=> Distância = DiagCost*MenorDist + StraightCos*(MaiorDist - MenorDist)
        if (distX > distZ)
            // distZ é menor
            return DiagCost * distZ + StraightCost * (distX - distZ);
        // Else
        // distX é menor
        return DiagCost * distX + StraightCost * (distZ - distX);
    }
}
