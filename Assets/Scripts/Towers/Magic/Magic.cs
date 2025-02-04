using System.Collections;
using UnityEngine;

public class Magic : BaseTower
{
    public Transform projectileOrigin;
    private float damageDealt = 0;
    protected override IEnumerator ChargeUp(GameObject enemy)
    {
        yield return null;
    }

    protected override IEnumerator Shoot(GameObject enemy)
    {
        yield return TowerHelpers.AnimateLaser(GetComponent<LineRenderer>(), projectileOrigin, enemy, 0.5f, KillProjectile);
    }

    protected override void KillProjectile(GameObject sphere, GameObject enemy, Vector3 _enemyPosition)
    {
        if (sphere != null)
        {
            Destroy(sphere);
            GetComponent<Animator>().SetTrigger("idle");
        }
        else if (enemy != null) enemy.GetComponent<BaseEnemy>().TakeDamage(towerData.levels[level - 1].damage, DamageTypes.MAGIC);
        damageDealt += towerData.levels[level - 1].damage;
        Debug.Log("Damage dealt: " + damageDealt);
    }
}
