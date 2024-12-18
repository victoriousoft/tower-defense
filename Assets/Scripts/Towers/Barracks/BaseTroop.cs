using System.Collections;
using System.Linq;
using UnityEngine;

public abstract class BaseTroop : MonoBehaviour
{
    public TroopSheet troopData;

    [HideInInspector]
    public float health;
    [HideInInspector]
    public Vector2 targetLocation;
    [HideInInspector]
    public int id = -1;
    [HideInInspector]
    public GameObject homeBase = null;

    protected HealthBar healthBar;
    protected GameObject currentEnemy;
    protected bool canAttack = true;
    protected bool ignoreEnemies = false;


    protected abstract void Attack();
    protected abstract void FixedUpdate();

    void Awake()
    {
        health = troopData.stats.maxHealth;
        healthBar = GetComponentInChildren<HealthBar>();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.SetHealth(health / troopData.stats.maxHealth);
        if (health <= 0) Die();
    }

    public void Heal(float amount)
    {
        health += amount;
        health = Mathf.Min(health, troopData.stats.maxHealth);
        healthBar.SetHealth(health / troopData.stats.maxHealth);
    }


    public void Die()
    {
        currentEnemy.GetComponent<BaseEnemy>().currentTarget = null;
        homeBase.GetComponent<Barracks>().RequestTroopRevive(id);
        Destroy(gameObject);

    }

    public void WalkTo(Vector3 target)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, troopData.stats.speed * Time.fixedDeltaTime);
    }

    protected GameObject FindNewEnemy()
    {
        GameObject[] enemiesInTroopRange = TowerHelpers.GetEnemiesInRange(transform.position, troopData.stats.visRange, new EnemyTypes[] { EnemyTypes.GROUND });
        GameObject[] enemiesInTowerRange = TowerHelpers.GetEnemiesInRange(homeBase.transform.position, homeBase.GetComponent<BaseTower>().towerData.levels[homeBase.GetComponent<BaseTower>().level].range, new EnemyTypes[] { EnemyTypes.GROUND });

        GameObject[] enemiesInRange = enemiesInTroopRange.Intersect(enemiesInTowerRange).Where(enemy => enemy.GetComponent<BaseEnemy>().currentTarget == null).ToArray();
        enemiesInRange = enemiesInRange.Where(enemy => troopData.enemyTypes.Contains(enemy.GetComponent<BaseEnemy>().enemyData.enemyType)).ToArray();

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

    public void ForceReposition()
    {
        ForceReposition(new Vector2(0, 0));
    }

    public void ForceReposition(Vector2 position)
    {
        ignoreEnemies = true;

        if (currentEnemy != null && currentEnemy.GetComponent<BaseEnemy>().currentTarget == gameObject)
        {
            currentEnemy.GetComponent<BaseEnemy>().currentTarget = null;
        }

        currentEnemy = null;

        if (position != null)
        {
            targetLocation = position;
        }
        else
        {
            targetLocation = homeBase.GetComponent<Barracks>().RequestTroopRandezvousPoint(id);
        }
    }

    protected IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(troopData.stats.attackCooldown);
        canAttack = true;
    }
}
