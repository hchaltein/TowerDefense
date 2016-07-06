using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

// Classe que administra vários aspectos do jogo, incluindo condições de vitória e UI.
public class GameMngrBhvr : MonoBehaviour
{
    #region Class Variables
    // Váriaveis do player
    public int plyrCash = 100;
    public int totalPlyrHp = 10;
    public int curPlyrHp;

    // Objetos e scripts
    GameObject enemyCollect, goal, enemyGen;
    PathFinding pathFindScr;

    // Variáveis das torres
    public TowerType SltdTower;
    public int BasicTowerDmge = 1;
    [SerializeField]
    int BasicTowerCost = 1;
    [SerializeField]
    int SlowTowerCost = 1;

    // Elementos de UI
    // Dinheiro do player
    [SerializeField]
    Text curCashUI;
    
    // Torre selecionada e custo
    [SerializeField]
    Text curTowerUI;
    [SerializeField]
    Text curTowerCostUI;
    
    // Hp do player
    [SerializeField]
    Slider HpBarUI;

    // Informação sobre ondas
    [SerializeField]
    Text wavesSpawnedUI;
    [SerializeField]
    Text wavesToGoUI;
    
    // Janelas
    [SerializeField]
    Text displayMsgUI;
    float fadeTime = 0.5f;

    [SerializeField]
    GameObject GameOverWindow;

    #endregion

    // Inicialização
    void Start()
    {
        // Objetos e componentes
        enemyCollect = GameObject.Find("EnemyCollector");
        enemyGen = GameObject.Find("EnemyGen");
        goal = GameObject.Find("Goal");
        pathFindScr = GetComponent<PathFinding>();

        // Variáveis
        SltdTower = TowerType.Basic;
        curPlyrHp = totalPlyrHp;

        // Desabilita a mensagem de erro e a tela de Game Over.
        if (displayMsgUI.transform.parent.gameObject.activeInHierarchy)
        {
            displayMsgUI.transform.parent.gameObject.SetActive(false);
        }

        if (GameOverWindow.activeInHierarchy)
        {
            GameOverWindow.SetActive(false);
        }
    }

    // Contorla seleção de torres e atualiza a UI
    void Update()
    {
        // Seleciona Torre a ser Spawnada.
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (SltdTower == TowerType.Basic)
            {
                SltdTower = TowerType.Slow;
            }
            else
            {
                SltdTower = TowerType.Basic;
            }
        }

        UpdateUI();
    }

    // Função que atualiza a UI de fato.
    void UpdateUI()
    {
        curCashUI.text = plyrCash.ToString();
        curTowerUI.text = SltdTower.ToString();
        curTowerCostUI.text = getTowerCost(SltdTower).ToString();
        HpBarUI.value = (float)curPlyrHp / (float)totalPlyrHp;
        wavesSpawnedUI.text = enemyGen.GetComponent<EnemyGenBhvr>().CountWavesSpawned.ToString();
        wavesToGoUI.text = enemyGen.GetComponent<EnemyGenBhvr>().TotalWavesToSpawn.ToString();
    }

    /// <summary>
    /// Função que checa se todos os inimigos e o EnemyGen possuem caminho até o Goal
    /// </summary>
    /// <returns> Retorna falso se algum desses caminhos não existir.</returns>
    bool CheckBlockedPath()
    {
        // variável temporária para armazenar caminho do EnemyGen até o Goal
        var dummyPath = new List<Tile>();
        // Avalia Path do enemy generator até o goal
        if (!pathFindScr.FindPath(enemyGen.transform.position, goal.transform.position, out dummyPath))
        {
            // Falha no teste
            return false;
        }

        if (enemyCollect.transform.childCount > 0)
        {
            // Avalia path de todos inimigos um a um.
            foreach (Transform enemy in enemyCollect.transform)
            {
                var enemyPathFollow = enemy.gameObject.GetComponent<PathFollow>();
                if (!enemyPathFollow.tryFindingNewPath(goal.transform))
                {
                    // Falha no teste
                    return false;
                }
            }
        }

        // Todos os testes tiveram sucesso
        return true;
    }

    // Determina se o grid pode ser mudado i.e: construir/vender torre.
    public bool canChangGrid()
    {
        // Checa por rotas bloqueadas.
        if (CheckBlockedPath())
        {
            // Passou no teste, atualiza rotas dos inimigos
            foreach (Transform enemy in enemyCollect.transform)
            {
                enemy.gameObject.GetComponent<PathFollow>().changeToNewPath();
            }
            return true;
        }
        else
        {
            // Falhou no teste
            //Debug.Log("Ação falhou!");
            return false;
        }
    }

    // Retorna custo da Torre atual selecionada
    int getTowerCost(TowerType SltdTower)
    {
        switch (SltdTower)
        {
            case TowerType.Basic:
                return BasicTowerCost;

            case TowerType.Slow:
                return SlowTowerCost;

            default:
                return BasicTowerCost;
        }
    }

    // Testa se player tem dinheiro suficiente para construir Torre.
    public bool canBuildTower()
    {
        return plyrCash >= getTowerCost(SltdTower);
    }

    // Função que constói a torre dado o tipo selecionado.
    public void buildTower(Transform TowerCollector)
    {
        // Cobra o player o dinheiro necessário.
        plyrCash -= getTowerCost(SltdTower);

        switch (SltdTower)
        {
            // Torre Básica. Atira em inimigos
            case TowerType.Basic:
                TowerCollector.GetChild(0).gameObject.SetActive(true);
                break;

            // Torre Slow. Deixa inimigos lentos.
            case TowerType.Slow:
                TowerCollector.GetChild(1).gameObject.SetActive(true);
                break;

            default:
                TowerCollector.GetChild(0).gameObject.SetActive(true);
                break;
        }
    }

    // Função que vende a torre dado o tipo selecionado.
    public void sellTower(Transform TowerCollector)
    {
        plyrCash += getTowerCost(SltdTower);

        TowerCollector.GetChild(0).gameObject.SetActive(false);
        TowerCollector.GetChild(1).gameObject.SetActive(false);
    }

    // Mostra mensagem de jogo para o player.
    public void sndMsgPlayer(string displaytext)
    {
        displayMsgUI.transform.parent.gameObject.SetActive(true);
        displayMsgUI.text = displaytext;
        StartCoroutine(fadeAwayTimer(fadeTime));
    }

    // Automaticamente faz mensagem de jogo desaparecer após algum tempo.
    IEnumerator fadeAwayTimer(float timeValue)
    {
        yield return new WaitForSeconds(timeValue);
        displayMsgUI.transform.parent.gameObject.SetActive(false);
    }

    // Encerra o jogo.
    public void gameOver(string gameOverText)
    {
        // Pausa o jogo
        Time.timeScale = 0.0f;
        // Mostra mensagem de fim de jogo para o player.
        GameOverWindow.SetActive(true);
        GameOverWindow.transform.GetChild(0).GetComponent<Text>().text = gameOverText;
    }

    // Recarrega cena atual para reiniciar o jogo
    // Nota: Existe algum problema com o reinício do jogo. A cena é recarregada corretamente porém os inimigos não começam a ser spawnados.
    public void restartGame()
    {
        // Despausa o jogo
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //Application.LoadLevel(Application.loadedLevel);
    }

    // Sai do Jogo.
    public void quitGame()
    {
        // Despausa o jogo
        Time.timeScale = 1.0f;
        Application.Quit();
    }
}
