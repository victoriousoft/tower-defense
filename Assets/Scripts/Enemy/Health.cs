using UnityEngine;

public class Health : MonoBehaviour
{
    public float health = 100;
    public float maxHealth = 100;
    public int hpSub;
    public int cashDrop;

    public HealthBar healthBar;
    private PlayerStatsManager playerStats;

    void Awake()
    {
        health = maxHealth;
        playerStats = GameObject.Find("PlayerStats").GetComponent<PlayerStatsManager>();
    }

    public void TakeDamage(float damage)
    {
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
