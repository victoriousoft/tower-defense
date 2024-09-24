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

    void Update()
    {
        GameObject[] enemies = TowerHelpers.GetEnemiesInRange(transform, range);
        Debug.Log("Enemies in range: " + enemies.Length);
        Debug.Log("Can shoot: " + canShoot);
        if (enemies.Length > 0 && canShoot)
        {
            GameObject closestEnemy = enemies.OrderBy(e => e.GetComponent<Movement>().getDistanceToLastPoint()).First();
            Debug.Log("Closest enemy: " + closestEnemy.name);
            Shoot(closestEnemy);
            StartCoroutine(AnimateArrow(closestEnemy));
            canShoot = false;
            StartCoroutine(ResetCooldown());
        }
    }

    IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        canShoot = true;
    }

    IEnumerator AnimateArrow(GameObject enemy)
    {
        GameObject arrow = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        arrow.transform.position = transform.position;
        arrow.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        arrow.GetComponent<Renderer>().material.color = Color.red;

        yield return TowerHelpers.AnimateBezierProjectile(arrow, transform.position, enemy, 2, 1, TowerHelpers.TowerProjectileRotationTypes.LOOK_AT_TARGET);
    }

    public static void KillArrow(GameObject arrow)
    {
        Destroy(arrow);
    }


    void Shoot(GameObject enemy)
    {
        Debug.Log("Shooting at " + enemy.name);
        enemy.GetComponent<Health>().TakeDamage((int)damage);
    }
}
