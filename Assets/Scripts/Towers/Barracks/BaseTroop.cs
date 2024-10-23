using System.Collections;
using System.Linq;
using UnityEngine;

public abstract class BaseTroop : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public float speed;
    public float damage;
    public float attackCooldown;
    public float attackRange;
    public Vector3 targetLocation;
    public int id = -1;

    public GameObject homeBase = null;

    protected HealthBar healthBar;
    protected GameObject currentEnemy;
    protected bool canAttack = true;

    protected abstract void Attack();
    protected abstract void FixedUpdate();

    void Awake()
    {
        health = maxHealth;
        healthBar = GetComponentInChildren<HealthBar>();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.SetHealth(health / maxHealth);
        if (health <= 0) Die();
    }

    public void Heal(float amount)
    {
        health += amount;
        health = Mathf.Min(health, maxHealth);
        healthBar.SetHealth(health / maxHealth);
    }


    public void Die()
    {
        currentEnemy.GetComponent<Movement>().isPaused = false;
        homeBase.GetComponent<Barracks>().RequestTroopRevive(id);
        Destroy(gameObject);

    }

    public void WalkTo(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    protected GameObject FindNewEnemy()
    {
        GameObject[] enemiesInRange = TowerHelpers.GetEnemiesInRange(transform.position, attackRange);
        enemiesInRange = enemiesInRange.OrderBy(enemy => Vector3.Distance(transform.position, enemy.transform.position)).ToArray();
        if (enemiesInRange.Length > 0)
        {
            return enemiesInRange[0];
        }

        return null;
    }

    protected IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
