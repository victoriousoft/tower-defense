using System.Collections;
using System.Linq;
using UnityEngine;

public class Magic : MonoBehaviour
{

    public float range = 4;
    public float cooldown = 1;
    public float damage = 10;

    public TowerHelpers.TowerTargetTypes targetType = TowerHelpers.TowerTargetTypes.CLOSEST_TO_FINISH;

    private bool canShoot = true;

    void FixedUpdate()
    {
        if (!canShoot) return;

        GameObject[] enemies = TowerHelpers.GetEnemiesInRange(transform.position, range);
        if (enemies.Length == 0) return;

        GameObject target = TowerHelpers.SelectEnemyToAttack(enemies, targetType);
        StartCoroutine(AnimateSphere(target));
        canShoot = false;
        StartCoroutine(ResetCooldown());
    }

    IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        canShoot = true;
    }

    IEnumerator AnimateSphere(GameObject enemy)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = transform.position;
        sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        sphere.GetComponent<Renderer>().material.color = Color.blue;

        yield return TowerHelpers.AnimateDirectProjectile(sphere, enemy, 5, KillSphere);
    }

    void KillSphere(GameObject sphere, GameObject enemy, Vector3 _enemyPosition)
    {
        Destroy(sphere);
        if (enemy != null) enemy.GetComponent<Health>().TakeDamage((int)damage, DamageTypes.MAGIC);
    }
}
