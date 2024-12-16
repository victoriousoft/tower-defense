using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{
    public EnemySheet enemyData;
    [HideInInspector]
    public float health;
    [HideInInspector]
    public bool isPaused = false;
    [HideInInspector]
    public GameObject currentTarget;

    private int currentPointIndex = 0;
    private Transform[] points;
    private Vector2 positionOffset;

    private readonly float[] resistanceValues = new float[] { 1, 0.5f, 0.35f, 0.2f, 0 };
    private HealthBar healthBar;
    private PlayerStatsManager playerStats;


    protected bool canAttack = true;
    protected abstract void Attack();

    void Awake()
    {
        health = enemyData.stats.maxHealth;
        healthBar = GetComponentInChildren<HealthBar>();
        playerStats = GameObject.Find("PlayerStats").GetComponent<PlayerStatsManager>();

        positionOffset = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
        if (enemyData.enemyType == EnemyTypes.FLYING) positionOffset.y += 1f;
        if (enemyData.enemyType == EnemyTypes.GROUND) Debug.Log("offset: " + positionOffset);
    }

    void FixedUpdate()
    {
        if (currentTarget == null) isPaused = false;

        if (!isPaused) Move();
        if (currentTarget != null && canAttack) Attack();
    }

    public void SetPathParent(Transform pathParent)
    {
        points = new Transform[pathParent.childCount];
        for (int i = 0; i < pathParent.childCount; i++)
        {
            points[i] = pathParent.GetChild(i);
        }

        transform.position = (Vector2)points[currentPointIndex].position + positionOffset;
    }

    void Move()
    {
        if (points == null) return;

        if (Vector2.Distance(transform.position, (Vector2)points[currentPointIndex].position + positionOffset) < 0.1f)
        {
            currentPointIndex++;
            if (currentPointIndex >= points.Length)
            {
                Destroy(gameObject);
                return;
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, (Vector2)points[currentPointIndex].position + positionOffset, enemyData.stats.speed * Time.fixedDeltaTime);
    }

    public float GetDistanceToFinish()
    {
        float distance = Vector2.Distance((Vector2)transform.position - positionOffset, points[currentPointIndex].position);

        for (int i = currentPointIndex; i < points.Length - 1; i++)
        {
            distance += Vector2.Distance(points[i].position, points[i + 1].position);
        }
        return distance;
    }

    public float GetDistanceToStart()
    {
        float distance = Vector2.Distance((Vector2)transform.position - positionOffset, points[currentPointIndex].position);
        for (int i = currentPointIndex; i >= 0; i--)
        {
            distance += Vector2.Distance(points[i].position, points[i - 1].position);
        }

        return distance;
    }

    public void TakeDamage(float damage, DamageTypes damageType)
    {
        health -= (damageType == DamageTypes.PHYSICAL) ? damage * resistanceValues[enemyData.stats.physicalResistance] :
             (damageType == DamageTypes.MAGIC) ? damage * resistanceValues[enemyData.stats.magicResistance] :
             (damageType == DamageTypes.EXPLOSION) ? damage * (resistanceValues[enemyData.stats.physicalResistance] / 2) : damage;

        healthBar.SetHealth(health / enemyData.stats.maxHealth);

        if (health <= 0)
        {
            Destroy(gameObject);
            playerStats.AddGold(enemyData.stats.cashDrop);//random??
        }
    }

    public void Heal(float amount)
    {
        health = Mathf.Min(health += amount, enemyData.stats.maxHealth);
        healthBar.SetHealth(health / enemyData.stats.maxHealth);
    }

    public void RequestTarget(GameObject target)
    {
        if (currentTarget == null) currentTarget = target;
    }

    protected IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(enemyData.stats.attackCooldown);
        canAttack = true;
    }
}
