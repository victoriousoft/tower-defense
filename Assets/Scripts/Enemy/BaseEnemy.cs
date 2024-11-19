using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{
    public float health = 100;
    public float maxHealth = 100;
    public int damage = 10;
    public int cashDrop = 2;
    [Range(0, 4)] public int physicalResistance;
    [Range(0, 4)] public int magicResistance;
    public float speed = 1f;
    public bool isPaused = false;
    public GameObject currentTarget;
    public float range = 1f;
    public float attackCooldown = 1f; // seconds

    private int currentPointIndex = 0;
    private Transform[] points;


    private readonly float[] resistanceValues = new float[] { 1, 0.5f, 0.35f, 0.2f, 0 };
    private HealthBar healthBar;
    private PlayerStatsManager playerStats;


    protected bool canAttack = true;
    protected abstract void Attack();

    void Awake()
    {
        health = maxHealth;
        healthBar = GetComponentInChildren<HealthBar>();
        playerStats = GameObject.Find("PlayerStats").GetComponent<PlayerStatsManager>();
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

        transform.position = points[currentPointIndex].position;
    }

    void Move()
    {
        if (points == null) return;

        if (Vector2.Distance(transform.position, points[currentPointIndex].position) < 0.1f)
        {
            currentPointIndex++;
            if (currentPointIndex >= points.Length)
            {
                Destroy(gameObject);
                return;
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, points[currentPointIndex].position, speed * Time.fixedDeltaTime);
    }

    public float GetDistanceToFinish()
    {
        float distance = 0;
        for (int i = currentPointIndex; i < points.Length - 1; i++)
        {
            distance += Vector2.Distance(points[i].position, points[i + 1].position);
        }
        return distance;
    }

    public float GetDistanceToStart()
    {
        float distance = 0;
        for (int i = currentPointIndex; i >= 0; i--)
        {
            distance += Vector2.Distance(points[i].position, points[i - 1].position);
        }

        return distance;
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

    protected IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
