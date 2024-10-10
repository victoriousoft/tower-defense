using UnityEngine;

public class Health : MonoBehaviour
{
    public float health = 100;
    public float maxHealth = 100;
    public int hpSub;
    public int cashDrop;
    [Range(0, 4)] public int physicalResistance;
    [Range(0, 4)] public int magicResistance;
    private float[] resistanceValues = new float[] { 1, 0.5f, 0.35f, 0.2f, 0 };

    private HealthBar healthBar;
    private PlayerStatsManager playerStats;

    void Awake()
    {
        health = maxHealth;
        healthBar = GetComponentInChildren<HealthBar>();
        playerStats = GameObject.Find("PlayerStats").GetComponent<PlayerStatsManager>();
    }

    //0- PHYSICAL, 1- MAGIC, 2- BOMB, 3- TRUE
    //res lvl 1- 50%, lvl2-65%, lvl3-80%
    public void TakeDamage(float damage, int damageType)
    {
        health -= (damageType == 0 && physicalResistance > 0) ? damage * resistanceValues[physicalResistance] :
             (damageType == 1 && magicResistance > 0) ? damage * resistanceValues[magicResistance] :
             (damageType == 2 && physicalResistance > 0) ? damage * (resistanceValues[physicalResistance] / 2) : damage;
        health -= damage;

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
