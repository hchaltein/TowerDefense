using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;

// Tipos de Tile para o grid
public enum TileType
{
    Floor,
    Wall,
    Tower,
    EnemyGen,
    Goal
}

// Tipos de torre
public enum TowerType
{
    Basic,
    Slow
}

// Essa Classe faz parte do Grid utilizado para construir torres e pelo pathfinding.
public class Tile : MonoBehaviour
{
    #region Class Variables

    // Variáveis da Classe.
    [SerializeField]
    TileType MyTileType;
    public Vector3 WorldPos;
    public int GridPosX, GridPosZ;
    float tileCheckRadius;

    // Layers de objetos
    int wallLayer;
    int enemyGenLayer;
    int goalLayer;

    // Componentes e outros objetos
    Button tileBtn;
    Transform towerCollector;
    GameMngrBhvr gameMngrScr;

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
    #endregion

    // Inicialização
    void Start()
    {
        // Layers de objetos
        wallLayer = LayerMask.GetMask(new[] { "Walls" });
        enemyGenLayer = LayerMask.GetMask(new[] { "EnemyGen" });
        goalLayer = LayerMask.GetMask(new[] { "Goal" });

        // Componentes e outras referências
        gameMngrScr = GameObject.Find("GameMngr").GetComponent<GameMngrBhvr>();
        tileCheckRadius = gameMngrScr.gameObject.GetComponent<Grid>().TileRadius - 0.1f;

        tileBtn = GetComponentInChildren<Button>();
        towerCollector = transform.GetChild(1);
        
        // Pega posição em coordenadas de mundo da Tile.
        WorldPos = transform.position;

        // Determina Tipo de Tile inicial de acordo com um teste de colisão:
        if (Physics.CheckSphere(transform.position, tileCheckRadius, wallLayer))
        {
            changeTileType(TileType.Wall);
        }
        else if (Physics.CheckSphere(transform.position, tileCheckRadius, enemyGenLayer))
        {
            changeTileType(TileType.EnemyGen);
        }
        else if (Physics.CheckSphere(transform.position, tileCheckRadius, goalLayer))
        {
            changeTileType(TileType.Goal);
        }
        else
        {
            changeTileType(TileType.Floor);
        }
    }

    // Muda tipo da Tile e a configura corretamente de acordo com o tipo.
    public void changeTileType(TileType newType)
    {
        MyTileType = newType;
        // Configura Tile de acordo com seu tipo
        switch (MyTileType)
        {
            case TileType.Floor:
                isWalkable = true;
                tileBtn.interactable = true;
                break;

            case TileType.Wall:
                isWalkable = false;
                tileBtn.interactable = false;
                break;

            case TileType.Tower:
                isWalkable = false;
                tileBtn.interactable = true;
                break;

            case TileType.EnemyGen:
                isWalkable = true;
                tileBtn.interactable = false;
                break;

            case TileType.Goal:
                isWalkable = true;
                tileBtn.interactable = false;
                break;

            default:
                isWalkable = true;
                tileBtn.interactable = true;
                break;
        }
    }

    // Função chamada quando a Tile é clicada.
    public void ButtonPressed()
    {
        TileType curTileType = MyTileType;
        // Muda tipo de tile para fazer teste
        if (MyTileType == TileType.Tower)
        {
            // Player está tentando vender torre
            changeTileType(TileType.Floor);
        }
        else if (MyTileType == TileType.Floor)
        {
            if (!gameMngrScr.canBuildTower())
            {
                // Player tentou construir torre sem dinheiro
                // Manda mensagem de falta de dinheiro e retorna.
                gameMngrScr.sndMsgPlayer("Not enough money!!!");
                return;
            }

            changeTileType(TileType.Tower);
        }

        // Se alguma path foi bloqueada desfaz as mudanças e retorna.
        if (!(GameObject.Find("GameMngr").GetComponent<GameMngrBhvr>().canChangGrid()))
        {
            //Debug.Log("Path de algum inimigo foi bloqueada");
            gameMngrScr.sndMsgPlayer("Ação illegal! Bloqueia caminho");
            changeTileType(curTileType);
            return;
        }

        // Mudanças podem ser feitas
        if (MyTileType == TileType.Tower)
        {
            // Constrói torre
            gameMngrScr.buildTower(towerCollector);
        }
        else if (MyTileType == TileType.Floor)
        {
            // Vende Torres
            gameMngrScr.sellTower(towerCollector);
        }
    }
}
