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
        GameObject[] enemies = getEnemiesInRange();
        Debug.Log("Enemies in range: " + enemies.Length);
        Debug.Log("Can shoot: " + canShoot);
        if (enemies.Length > 0 && canShoot)
        {
            GameObject closestEnemy = enemies.OrderBy(e => e.GetComponent<Movement>().getDistanceToLastPoint()).First();
            Debug.Log("Closest enemy: " + closestEnemy.name);
            Shoot(closestEnemy);
            canShoot = false;
            Invoke("ResetCooldown", cooldown);
        }
    }

    void ResetCooldown()
    {
        canShoot = true;
    }

    void Shoot(GameObject enemy)
    {
        Debug.Log("Shooting at " + enemy.name);
        GameObject arrow = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        arrow.transform.position = transform.position;
        while (Vector3.Distance(arrow.transform.position, enemy.transform.position) > 0.1f)
        {
            arrow.transform.position = Vector3.MoveTowards(arrow.transform.position, enemy.transform.position, 1 * Time.deltaTime);
        }

        Destroy(arrow);
        enemy.GetComponent<Health>().takeDamage((int)damage);
    }

    GameObject[] getEnemiesInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);
        return hitColliders.Where(c => c.CompareTag("Enemy")).Select(c => c.gameObject).ToArray();
    }
}
