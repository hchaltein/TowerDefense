using UnityEngine;
using System.Collections;

// Script que controla a geração de inimigos pelo gerador.
public class EnemyGenBhvr : MonoBehaviour
{
    // Variáveis de temporização
    [SerializeField]
    float WaveWaitTime = 3.0f;
    [SerializeField]
    float spawnWaitTime = 0.2f;
    
    // Variáveis de números de inimigos
    [SerializeField]
    int enemyPerWave = 5;
    [SerializeField]
    int enemyIncreaseWave = 2;

    // Objetos e referências
    [SerializeField]
    GameObject[] EnemyPreFabs;
    [SerializeField]
    Transform spawnPoint;
    GameMngrBhvr gameMngrBhvr;
    Transform enemyCollector;

    // Variáveis de controle do script.
    public int CountWavesSpawned = 0;
    public int TotalWavesToSpawn = 5;
    [SerializeField]
    bool isMixedWave = true;
    [SerializeField]
    bool isEndlessWaves = false;
    int curEnemyType = 0;

    // Inicializa spawn de inimigos
    void Start ()
    {
        gameMngrBhvr = GameObject.Find("GameMngr").GetComponent<GameMngrBhvr>();
        enemyCollector = GameObject.Find("EnemyCollector").transform;

        // Inicia spawn propriamente dito.
        StartCoroutine(spawnWave(curEnemyType, isMixedWave));
    }

    // Verifica condição de vitória do player.
    // Condição de vitória: Todas as ondas já chamadas, não é modo endless e nenhum inimigo mais na tela.
    void Update()
    {
        if (CountWavesSpawned == TotalWavesToSpawn && !isEndlessWaves && enemyCollector.childCount <= 0)
        {
            gameMngrBhvr.gameOver("You win!");
        }
    }

    // Instancia onda de inimigos
    IEnumerator spawnWave( int _enemyType, bool _isMixedWave)
    {
        // Se todas ondas já foram chamadas, para de instanciar novas ondas.
        if (CountWavesSpawned >= TotalWavesToSpawn && !isEndlessWaves)
        {
            yield break;
        }

        // Espera tempo entre ondas antes de instanciar nova onda.
        yield return new WaitForSeconds(WaveWaitTime);
        for (int i = 0; i < enemyPerWave; i++)
        {
            // Instancia um inimigo por vez, esperando brevemente entre eles
            yield return new WaitForSeconds(spawnWaitTime);
            GameObject enemySpawned = (GameObject)Instantiate(EnemyPreFabs[_enemyType], spawnPoint.position, Quaternion.identity);
            enemySpawned.transform.SetParent(enemyCollector);

            // Altera tipo de inimigo a ser spawnado se a onda é mista.
            if (_isMixedWave)
            {
                _enemyType = Mathf.FloorToInt(Random.Range(0, EnemyPreFabs.Length));
            }

        }
        // Se a onda não é mista, vai para próximo tipo de inimigos.
        if (!_isMixedWave)
        {
            _enemyType = changeEnemyType(_enemyType);
        }

        // Incrementa numero de ondas spawnadas.
        CountWavesSpawned++;

        // Incrementa o número de inimigos por onda afim de aumentar a dificuldade.
        enemyPerWave += enemyIncreaseWave;

        // Recomeça ciclo
        StartCoroutine(spawnWave(_enemyType, _isMixedWave));
    }

    // Muda o tipo de inimigo sequencialmente
   int changeEnemyType(int _enemyType)
    {
        _enemyType++;
        if (_enemyType >= EnemyPreFabs.Length)
        {
            _enemyType = 0;
        }
        return _enemyType;
    }

}
