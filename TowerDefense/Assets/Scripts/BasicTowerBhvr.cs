using UnityEngine;
using System.Collections;

public class BasicTowerBhvr : MonoBehaviour
{

    [SerializeField]
    float TowerRangeRadius = 4.0f;
    SphereCollider TowerColl;

    Transform curTarget = null;

    [SerializeField]
    GameObject BulletPreFab;
    [SerializeField]
    Transform BulletSpawnPoint;
    [SerializeField]
    float bulletWaitTime = 0.5f;
    [SerializeField]
    float bulletSpeed = 8.0f;

    bool canFire;

    // Use this for initialization
    void Start()
    {
        // Pega e inicializa o colisor
        TowerColl = GetComponent<SphereCollider>();
        TowerColl.radius = TowerRangeRadius;

        // Inicialização de variáveis.
        canFire = true;

    }

    void Update()
    {
        // Se existe alvo e pode atirar, atire.
        if (curTarget != null && canFire)
        {
            fireBullet(curTarget);
        }
    }

    // Detecção de alvos.
    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && curTarget == null)
        {
            curTarget = other.transform;
        }
    }

    // Se alvo sair do alcance, deistir de alvo.
    public void OnTriggerExit(Collider other)
    {
        if (other.transform == curTarget)
        {
            curTarget = null;
            canFire = true;
            StopCoroutine(fireCooldownTimer());
        }
    }

    // Função que cria e atira em um dado alvo.
    void fireBullet(Transform target)
    {
        GameObject Projectile = (GameObject)Instantiate(BulletPreFab, BulletSpawnPoint.position, Quaternion.identity);

        // Calcula direção do tiro:
        Vector3 moveDir = target.position - BulletSpawnPoint.position;

        // Atira.
        Projectile.GetComponent<Rigidbody>().velocity = moveDir * bulletSpeed;

        // Inicia contagem para atirar próxima bala.
        StartCoroutine(fireCooldownTimer());
    }

    // Temporizador para atirar em um alvo
    IEnumerator fireCooldownTimer()
    {
        canFire = false;
        // Wait and then spawn one Bullet
        yield return new WaitForSeconds(bulletWaitTime);
        canFire = true;
    }
}