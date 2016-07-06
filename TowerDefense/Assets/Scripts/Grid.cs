using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//Grid de Tiles utilizado pelo A*
// Grid é feito no plano XZ
public class Grid : MonoBehaviour
{
    // Variáveis da do grid de Tiles
    Tile[,] tileGrid;

    [SerializeField]
    GameObject TilePreFab;

    public Vector2 gridWorldSize;
    int tilesInX, tilesInZ;         // Total de Tiles em cada eixo

    // Tile Characteristics
    public float TileRadius;           // "Raio" de uma tile.
    float tileDiameter;

    [SerializeField]
    LayerMask unwalkableMask;       // 

    // Variáveis para desenhar Gizmos.
    public Transform TestPlayer;
    public List<Tile> drawnPath;

	void Start ()
    {
        tileDiameter = TileRadius * 2;

        // Não existem meio-tiles então tiles são arredondadas para números inteiros.
        tilesInX = Mathf.RoundToInt(gridWorldSize.x / tileDiameter);
        tilesInZ = Mathf.RoundToInt(gridWorldSize.y / tileDiameter);

        // Cria o Grid
        CreateGrid();

    }
    /// <summary>
    /// Função que cria o grid.
    /// </summary>
    void CreateGrid()
    {
        tileGrid = new Tile[tilesInX, tilesInZ];

        // Calcula posição de mundo do canto inferior esquerdo  do grid (posição 0,0)
        Vector3 gridBotLeftWorldPos = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        // Calcula posição do mundo para a tile e sua coordenada no grid
        for (int x = 0; x < tilesInX; x++)
        {
            for (int z = 0; z < tilesInZ; z++)
            {
                // Posição da tile infeiror esquerda + Centro da Tile em x + centro da Tile em z
                Vector3 tileWorldPos = gridBotLeftWorldPos + Vector3.right * (x * tileDiameter + TileRadius) + Vector3.forward * (z * tileDiameter + TileRadius);

                // Instancia tile na posição do grid.
                GameObject tileGO = Instantiate(TilePreFab, tileWorldPos, Quaternion.identity) as GameObject;
                // Escreve valores iniciais na tile.
                Tile tileScrpt = tileGO.GetComponent<Tile>();
                //tileScrpt.changeTileType(TileType.Floor);
                tileScrpt.GridPosX = x;
                tileScrpt.GridPosZ = z;
                tileGO.transform.SetParent(this.transform);
                // Adiciona tile ao grid.
                tileGrid[x, z] = tileScrpt;
            }
        }
    }

    // Retorna a Tile em uma posição do mundo
    public Tile GetTileFromWorld(Vector3 worldPos)
    {
        // Acha posição de mundo dentro do grid em termos localização "percentual" em cada eixo do grid.. 
        float percentX = (worldPos.x / gridWorldSize.x) + TileRadius;
        float percentZ = (worldPos.z / gridWorldSize.y) + TileRadius;    // gridWorldSize é Vec2 logo Y=Z 

        // Limita valor para entre 0 e 1.
        percentX = Mathf.Clamp01(percentX);
        percentZ = Mathf.Clamp01(percentZ);

        // Transforma percentual em cada eixo em coordenada do grid.
        int gridCoordX = Mathf.RoundToInt((tilesInX - 1) * percentX);
        int gridCoordZ = Mathf.RoundToInt((tilesInZ - 1) * percentZ);

        return tileGrid[gridCoordX, gridCoordZ];
    }

    // Retorna uma Lista de Tiles em um quadrado 3x3 centrados na Tile dada.
    public List<Tile> GetTileNeighbours (Tile centerTile)
    {
        List<Tile> Neighbours = new List<Tile>();

        // Coordenadas do Grid para vizinhos em potencial
        int CheckX, CheckZ;

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                // Pula tile central.
                if (x == 0 && z == 0) continue;

                CheckX = centerTile.GridPosX + x;
                CheckZ = centerTile.GridPosZ + z;

                // Testa se "Tile" está dentro do grid.
                if (CheckX >= 0 && CheckX < tilesInX && CheckZ >= 0 && CheckZ < tilesInZ)
                {
                    Neighbours.Add(tileGrid[CheckX, CheckZ]);
                }
            }
        }

        return Neighbours;
    }


    /*// Desenha gizmos para visualização.
    void OnDrawGizmos()
    {
        // Desenha o contorno do grid. 
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        // Desenha todas as na forma de cubos:
        if (tileGrid != null)
        {
            // Seleciona a tile que o testplayer está
            Tile playerTile = GetTileFromWorld(TestPlayer.position);

            foreach (Tile tile in tileGrid)
            {
                // Muda cor do gizmo se a Tile é andável. 
                Gizmos.color = (tile.isWalkable) ? Color.white : Color.red;

                // Colore a tile de preto se ela faz parte da rota.
                if (drawnPath != null)
                    if (drawnPath.Contains(tile)) Gizmos.color = Color.black;

                // Colore a Tile do player de amarelo
                if (playerTile == tile) Gizmos.color = Color.yellow;

                // Desenha cubo em cada Tile com diametro levemente menor do que da tile.
                Gizmos.DrawCube(tile.WorldPos, Vector3.one * (tileDiameter - 0.1f));
            }
        }
    }
    */
}
