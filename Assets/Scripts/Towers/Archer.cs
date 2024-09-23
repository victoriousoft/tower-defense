using System.Collections;
using System.Linq;
using UnityEngine;

public class Archer : MonoBehaviour
{
    public float range = 4;
    public float cooldown = 1;
    public float damage = 10;

    private bool canShoot = true;

    void Start()
    {
        // TODO: draw the range of the tower
        //DrawRangeOutline();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] enemies = GetEnemiesInRange();
        Debug.Log("Enemies in range: " + enemies.Length);
        Debug.Log("Can shoot: " + canShoot);
        if (enemies.Length > 0 && canShoot)
        {
            GameObject closestEnemy = enemies.OrderBy(e => e.GetComponent<Movement>().getDistanceToLastPoint()).First();
            Debug.Log("Closest enemy: " + closestEnemy.name);
            Shoot(closestEnemy);
            canShoot = false;
            StartCoroutine(ResetCooldown());
        }
    }

    IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        canShoot = true;
    }

    void Shoot(GameObject enemy)
    {
        Debug.Log("Shooting at " + enemy.name);
        enemy.GetComponent<Health>().TakeDamage((int)damage);
    }

    GameObject[] GetEnemiesInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);
        return hitColliders.Where(c => c.CompareTag("Enemy")).Select(c => c.gameObject).ToArray();
    }
}
