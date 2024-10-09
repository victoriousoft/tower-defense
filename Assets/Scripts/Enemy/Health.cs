using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float health = 100;
    public float maxHealth = 100;
    public int hpSub;

    public HealthBar healthBar;

    void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.SetHealth(health / maxHealth);

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Heal(float amount)
    {
        health = Mathf.Min(health += amount, maxHealth);
        healthBar.SetHealth(health / maxHealth);
    }
}
