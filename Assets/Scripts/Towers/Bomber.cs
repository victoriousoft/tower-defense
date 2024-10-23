using System.Collections;
using System.Linq;
using UnityEngine;

public class Bomber : MonoBehaviour
{

    public float range = 4;
    public float cooldown = 1;
    public float damage = 10;
    public float splashRadius = 1;

    public TowerHelpers.TowerTargetTypes targetType = TowerHelpers.TowerTargetTypes.CLOSEST_TO_FINISH;

    private bool canShoot = true;

    // TODO: draw the range of the tower

    void FixedUpdate()
    {
        if (!canShoot) return;

        GameObject[] enemies = TowerHelpers.GetEnemiesInRange(transform.position, range);
        if (enemies.Length == 0) return;

        GameObject target = TowerHelpers.SelectEnemyToAttack(enemies, targetType);
        StartCoroutine(AnimateBomb(target));
        canShoot = false;
        StartCoroutine(ResetCooldown());
    }

    IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        canShoot = true;
    }

    IEnumerator AnimateBomb(GameObject enemy)
    {
        GameObject bomb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bomb.transform.position = transform.position;
        bomb.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        bomb.GetComponent<Renderer>().material.color = Color.red;

        yield return TowerHelpers.AnimateBezierProjectile(bomb, transform.position, enemy, 2, 1, ExplodeBomb, TowerHelpers.TowerProjectileRotationTypes.SPIN);
    }

    void ExplodeBomb(GameObject bomb, GameObject enemy, Vector3 enemyPosition)
    {
        Destroy(bomb);
        GameObject[] enemies = TowerHelpers.GetEnemiesInRange(enemyPosition, splashRadius);
        foreach (GameObject e in enemies)
        {
            if (e != null) e.GetComponent<BaseEnemy>().TakeDamage((int)damage, DamageTypes.EXPLOSION);
        }
    }
}
