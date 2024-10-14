using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseTroop : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public float speed;
    public float damage;
    public float attackCooldown;
    public float attackRange;
    public Transform targetLocation;

    public GameObject homeBase = null;

    private GameObject currentEnemy;
    private bool canAttack = true;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0) Die();
    }

    public void Heal(float amount)
    {
        health += amount;
        health = Mathf.Min(health, maxHealth);
    }


    public void Die()
    {
        Destroy(gameObject);
    }

    public void WalkTo(Transform target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (targetLocation != null) WalkTo(targetLocation);


        if (currentEnemy == null) currentEnemy = FindNewEnemy();
        if (currentEnemy != null)
        {
            currentEnemy.GetComponent<Movement>().isPaused = true;
            targetLocation = currentEnemy.transform;
            if (canAttack) Attack();
        }
        else
        {
            targetLocation = homeBase.GetComponent<Barracks>().RequestTroopRandezvousPoint();
        }
    }

    GameObject FindNewEnemy()
    {
        GameObject[] enemiesInRange = TowerHelpers.GetEnemiesInRange(transform.position, attackRange);
        enemiesInRange = enemiesInRange.OrderBy(enemy => Vector3.Distance(transform.position, enemy.transform.position)).ToArray();
        if (enemiesInRange.Length > 0) return enemiesInRange[0];
        return null;
    }

    void Attack()
    {
        if (currentEnemy == null) return;

        currentEnemy.GetComponent<Health>().TakeDamage(damage, DamageTypes.PHYSICAL);
        canAttack = false;
        StartCoroutine(ResetAttackCooldown());
    }

    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
