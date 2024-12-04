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
    public float visRange;
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
        currentEnemy.GetComponent<BaseEnemy>().currentTarget = null;
        homeBase.GetComponent<Barracks>().RequestTroopRevive(id);
        Destroy(gameObject);

    }

    public void WalkTo(Vector3 target)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);
    }

    protected GameObject FindNewEnemy()
    {
        GameObject[] enemiesInTroopRange = TowerHelpers.GetEnemiesInRange(transform.position, visRange);
        GameObject[] enemiesInTowerRange = TowerHelpers.GetEnemiesInRange(homeBase.transform.position, homeBase.GetComponent<BaseTower>().towerData.levels[homeBase.GetComponent<BaseTower>().level].range);
        GameObject[] enemiesInRange = enemiesInTroopRange.Intersect(enemiesInTowerRange).Where(enemy => enemy.GetComponent<BaseEnemy>().currentTarget == null).ToArray();

        enemiesInRange = enemiesInRange
            .OrderBy(enemy => enemy.GetComponent<BaseEnemy>().currentTarget != null)
            .ThenBy(enemy => Vector3.Distance(transform.position, enemy.transform.position))
            .ToArray();

        if (enemiesInRange.Length > 0)
        {
            enemiesInRange[0].GetComponent<BaseEnemy>().isPaused = true;
            enemiesInRange[0].GetComponent<BaseEnemy>().RequestTarget(gameObject);
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
