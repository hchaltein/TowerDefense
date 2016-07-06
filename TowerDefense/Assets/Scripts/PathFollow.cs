using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// Script que segue um dado caminho encontrado.
public class PathFollow : MonoBehaviour
{
    #region Variáveis de Classe
    
    // Variáveis de movimento
    public float moveSpeed = 5.0f;
    public bool isSlowedDown;
    [SerializeField]
    int damageToPlyr = 1;

    // Variáveis para seguidor de caminho
    PathFinding pathFindScr;
    List<Tile> curPath, testPath;
    int pathIndex;
    bool hasReachedGoal;

    // Pontos de início (seeker) e fim (target) do pathfinding. 
    [SerializeField]
    Transform target;
    Transform seeker;
    #endregion

    void Start()
    {
        // Pega referências de objetos e scripts na cena.
        pathFindScr = GetComponent<PathFinding>();
        seeker = transform;
        if (target == null)
        {
            target = GameObject.Find("Goal").transform;
        }

        // Inicialização de variáveis
        pathIndex = 0;
        hasReachedGoal = false;
        isSlowedDown = false;
    }

    // Administra o pathfiding
    void Update()
    {
        // Inicia pathfinding
        if (curPath == null && tryFindingNewPath(target))
        {
            changeToNewPath();
        }

        // Se existe caminho, segue ele até o final.
        if (curPath != null && hasReachedGoal == false)
        {
            followPath();
        }
    }
    
    // Avalia se o Inimigo chegou ao destino.
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Goal"))
        {
            enemySuccess();
        }
    }

    // Segue rota.
    void followPath()
    {
        Vector3 moveDir;
        float minDist = 0.1f;

        // Se chegou no final do caminho, para.
        if (pathIndex == curPath.Count)
        {
            hasReachedGoal = true;
            curPath = null;
            return;
        }

        // Calcula direcao de movimento
        moveDir = (curPath[pathIndex].transform.position - seeker.position);

        // Se já chegou na Tile alvo, incrementa índice.
        if (moveDir.magnitude < minDist)
        {
            pathIndex++;
            return;
        }

        // Move na direção da rota. Se lento, reduz velocidade pela metade.
        if (isSlowedDown)
        {
            transform.position += moveDir.normalized * moveSpeed / 2 * Time.deltaTime;
        }
        else
        {
            transform.position += moveDir.normalized * moveSpeed * Time.deltaTime;
        }
    }

    // Procura caminho da posição atual até o dado alvo.
    public bool tryFindingNewPath(Transform target)
    {
        if (pathFindScr.FindPath(seeker.position, target.position, out testPath))
        {
            return true;
        }
        return false;
    }

    // Abandona rota antiga e começa novo caminho
    public void changeToNewPath()
    {
        curPath = testPath;
        testPath = null;
        hasReachedGoal = false;
        pathIndex = 0;
    }

    // Inicia contador de slowdown. Usada pela torre de SlowDown.
    public void startSlowDown(float slowTime)
    {
        StartCoroutine(slowDownTimer(slowTime));
    }

    // Temporizador para o efeito de SlowDown.
    IEnumerator slowDownTimer(float slowTime)
    {
        isSlowedDown = true;
        yield return new WaitForSeconds(slowTime);
        isSlowedDown = false;
    }

    // Inimigo chegou ao destino, da dano ao player.
    void enemySuccess()
    {
        // Aplicao o dano
        var gameMngr = GameObject.Find("GameMngr").GetComponent<GameMngrBhvr>();
        gameMngr.curPlyrHp -= damageToPlyr;

        // Verifica condição de derrota.
        if (gameMngr.curPlyrHp <= 0)
        {
            gameMngr.gameOver("You lose!");
        }
        // Destroi inimigo.
        Destroy(this.gameObject);
    }

}
