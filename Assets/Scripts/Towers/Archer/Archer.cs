using System.Collections;
using System.Linq;
using UnityEngine;

public class Archer : BaseTower
{
    protected override IEnumerator Shoot(GameObject enemy)
    {
        GameObject arrow = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        arrow.transform.SetParent(transform);
        arrow.transform.position = transform.position;
        arrow.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        arrow.GetComponent<Renderer>().material.color = Color.red;

        yield return TowerHelpers.AnimateBezierProjectile(arrow, transform.position, enemy, 2, 1, KillProjectile, TowerHelpers.TowerProjectileRotationTypes.LOOK_AT_TARGET);
    }

    protected override void KillProjectile(GameObject projectile, GameObject enemy, Vector3 _enemyPosition)
    {
        Destroy(projectile);
        if (enemy == null) return;

        enemy.GetComponent<BaseEnemy>().TakeDamage(towerData.levels[level].damage, DamageTypes.PHYSICAL);
    }
}
