using Unity.VisualScripting;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float health = 100;
    public float maxHealth = 100;
    public int hpSub;
    public int cashDrop;
    [Range(0, 4)] public int physicalResistance;
    [Range(0, 4)] public int magicResistance;
    private readonly float[] resistanceValues = new float[] { 1, 0.5f, 0.35f, 0.2f, 0 };

    private HealthBar healthBar;
    private PlayerStatsManager playerStats;
    public GameObject currentTarget { get; set; }

    void Awake()
    {
        health = maxHealth;
        healthBar = GetComponentInChildren<HealthBar>();
        playerStats = GameObject.Find("PlayerStats").GetComponent<PlayerStatsManager>();
    }

    void FixedUpdate()
    {
        if (currentTarget != null) currentTarget.GetComponent<BaseTroop>().TakeDamage(hpSub);
    }

    public void TakeDamage(float damage, DamageTypes damageType)
    {
        health -= (damageType == DamageTypes.PHYSICAL) ? damage * resistanceValues[physicalResistance] :
             (damageType == DamageTypes.MAGIC) ? damage * resistanceValues[magicResistance] :
             (damageType == DamageTypes.EXPLOSION) ? damage * (resistanceValues[physicalResistance] / 2) : damage;

        healthBar.SetHealth(health / maxHealth);

        if (health <= 0)
        {
            Destroy(gameObject);
            playerStats.AddGold(cashDrop);//random??
        }
    }

    public void Heal(float amount)
    {
        health = Mathf.Min(health += amount, maxHealth);
        healthBar.SetHealth(health / maxHealth);
    }
}
