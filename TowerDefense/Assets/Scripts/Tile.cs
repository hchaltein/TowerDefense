using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum TileType
{
    Floor,
    Wall,
    Tower
}

// there will be 18 (z) by 31 (X) tiles.

public class Tile : MonoBehaviour {

    // Variáveis da Classe.
    TileType MyTileType = TileType.Floor;
    public Vector3 worldPos;

    public int GridPosX, GridPosZ;

    // Variáveis para algoritmo A*
    public bool isWalkable;
    public Tile ParentTile;
    public int gCost, hCost;
    
    // Calcula fCost. Esse valor é apenas de leitura.
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
	
	void Start ()
    {
        // Pega posição em coordenadas de mundo da Tile.
        worldPos = transform.position;
	}

    // Muda tipo da Tile.
    public void changeTileType(TileType newType)
    {
        MyTileType = newType;

        // Determina se tile é andável ou não de acordo com seu novo tipo.
        isWalkable = (MyTileType == TileType.Floor) ? true : false;
    }

    // Funcao chamada quando a Tile é clicada.
    public void SelectButton()
    {
        GetComponentInChildren<Button>().Select();
        Debug.Log("Botao foi apertado");
    }
}
