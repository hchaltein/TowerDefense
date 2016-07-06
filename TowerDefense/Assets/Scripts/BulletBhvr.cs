using UnityEngine;
using System.Collections;

// Script do comportamento de projétil
public class BulletBhvr : MonoBehaviour
{
    // TEmpo de vida
    public float bulletLifeTime = 3.0f;

    // Inicialização
    void Start()
    {
        // Inicia contagem do tempo de vida
        StartCoroutine(KillSelf());
    }

    // Se collidir com um inimigo, se destrua. Impede efeito "perfurante" das balas.
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            StopCoroutine(KillSelf());
            Destroy(this.gameObject);
        }
    }

    // Se destroi depois de algum tempo.
    IEnumerator KillSelf()
    {
        yield return new WaitForSeconds(bulletLifeTime);
        //GameObject.Destroy(this);
        Destroy(this.gameObject);
    }
}
