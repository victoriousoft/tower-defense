using System.Collections;
using UnityEngine;

public class Magic : BaseTower
{
    public Transform projectileOrigin;
    protected override IEnumerator ChargeUp(GameObject enemy)
    {
        yield return null;
    }

    protected override IEnumerator Shoot(GameObject enemy)
    {
        Debug.Log(projectileOrigin == null);
        yield return TowerHelpers.AnimateLaser(GetComponent<LineRenderer>(), projectileOrigin, enemy, 1f, KillProjectile);
    }

    protected override void KillProjectile(GameObject sphere, GameObject enemy, Vector3 _enemyPosition)
    {
        enemy.GetComponent<BaseEnemy>().TakeDamage(towerData.levels[level].damage, DamageTypes.MAGIC);
    }
}
