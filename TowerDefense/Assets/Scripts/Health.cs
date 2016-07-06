using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// Script do Hp dos inimigos.
public class Health : MonoBehaviour
{
    #region Class Variables
    // Variáveis de customização
    [SerializeField]
    int BaseHealth = 5;
    public int CurrentHealth;
    [SerializeField]
    int cashReward = 5;

    // Objetos de UI
    public GameObject HpPrefab;
    public GameObject HpObj { get; private set; }
    private Slider HpBar;

    // Administrador do Fogo
    GameMngrBhvr GameMngrScr;
    #endregion

    //Inicialização
    void Start()
    {
        // Objetos e referências
        HpObj = GameObject.Instantiate(HpPrefab);
        HpObj.name = gameObject.name + "Health";
        HpObj.transform.SetParent(transform, false);
        HpBar = GetComponentInChildren<Slider>();
        GameMngrScr = GameObject.Find("GameMngr").GetComponent<GameMngrBhvr>();

        // Variáveis de customização
        CurrentHealth = BaseHealth;
        
        // UI
        UpdateHp();
    }

    // Toma dano quando acertado por uma bala.
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            TakeDamage(GameMngrScr.BasicTowerDmge);
        }
    }

    /// <summary>
    /// Aplica dano tomado no inimigo.
    /// </summary>
    /// <param name="value"></param>
    public void TakeDamage(int value)
    {
        CurrentHealth -= value;

        // Avalia Hp Inimigo
        if (CurrentHealth <= 0)
        {
            // Inimigo morreu
            enemyDeath();
        }

        // Limita HP do inimigo ao seu máximo HP
        if (CurrentHealth > BaseHealth)
        {
            CurrentHealth = BaseHealth;
        }

        // Atualiza UI
        UpdateHp();
    }

    // Atualiza Ui com Hp atual.
    void UpdateHp()
    {
        if (HpBar != null)
        {
            HpBar.value = (float)CurrentHealth / (float)BaseHealth;
            HpObj.transform.rotation = Quaternion.identity;
        }
    }

    // Inimigo morreu, adiciona dinheiro ao player e se destrói
    void enemyDeath()
    {
        GameMngrScr.plyrCash += cashReward;
        Destroy(this.gameObject);
    }
}
