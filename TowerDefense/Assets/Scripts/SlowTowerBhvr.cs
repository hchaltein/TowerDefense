using UnityEngine;
using System.Collections;

public class SlowTowerBhvr : MonoBehaviour
{
    [SerializeField]
    float towerRangeRadius = 4.0f;
    [SerializeField]
    float towerSlowTime = 0.5f;
    SphereCollider towerColl;

    // Use this for initialization
    void Start()
    {
        // Pega e inicializa o colisor
        towerColl = GetComponent<SphereCollider>();
        towerColl.radius = towerRangeRadius;

    }

    // Inicia Slowdonw de qqr inimigo que estiver ao alcance.
    public void OnTriggerStay(Collider other)
    {
        PathFollow pathFollowScr = null;
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") )
        {
            pathFollowScr = other.GetComponent<PathFollow>();
            if (pathFollowScr != null)
            {
                if (!pathFollowScr.isSlowedDown)
                {
                    pathFollowScr.startSlowDown(towerSlowTime);
                }
            }
        }
    }
}