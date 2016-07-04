using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFollow : MonoBehaviour
{
    PathFinding pathFindScr;

    List<Tile> path;

    public float moveSpeed = 5.0f;

    int pathIndex = 0;

    // Pontos de início (seeker) e fim (target) do pathfinding. 
    [SerializeField]
    Transform target;

    Transform seeker;



    // Use this for initialization
    void Start ()
    {
        pathFindScr = GetComponent<PathFinding>();

        seeker = transform;
        //target = GameObject.Find("Goal").transform;

	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            findPath(target);
        }

        if (path != null)
        {
            followPath();
        }

	}
    // Segue rota.
    void followPath()
    {
        Vector3 moveDir;
        float minDist = 0.1f;

        // Calcula direcao de movimento
        moveDir = (path[pathIndex].transform.position - seeker.position);
        
        // Se ja chegou no alvo, incrementa indice.
        if (moveDir.magnitude < minDist)
        {
            pathIndex++;
            return;
        }

        transform.position += moveDir.normalized * moveSpeed * Time.deltaTime;
    }

    bool findPath(Transform target)
    {
        pathFindScr.FindPath(seeker.position, target.position);
        if (pathFindScr.path != null)
        {
            path = pathFindScr.path;
            pathIndex = 0;
            return true;
        }
        else
        {
            return false;
        }
    }
}
