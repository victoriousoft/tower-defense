using UnityEngine;

public class Health : MonoBehaviour
{
    public float health = 100;
    public float maxHealth = 100;
    public int hpSub;
    public int cashDrop;

    public HealthBar healthBar;
    private PlayerStatsManager psvm;

    void Awake()
    {
        health = maxHealth;
        psvm = GameObject.Find("PlayerStats").GetComponent<PlayerStatsManager>();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.SetHealth(health / maxHealth);

        if (health <= 0)
        {
            Destroy(gameObject);
            psvm.AddGold(cashDrop);//random??
        }
    }

    public void Heal(float amount)
    {
        health = Mathf.Min(health += amount, maxHealth);
        healthBar.SetHealth(health / maxHealth);
    }
}
