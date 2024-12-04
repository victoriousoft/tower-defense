using System.Collections;
using UnityEngine;

public class Magic : BaseTower
{
    private Animator towerAnimator;
    public Transform origin;

    void Awake()
    {
        towerAnimator = GetComponent<Animator>();
    }

    protected override IEnumerator AnimateProjectile(GameObject enemy) { yield return null; }

    protected override IEnumerator AnimateLaser(GameObject enemy)
    {
        towerAnimator.Play("magic_chargeUp_1");
        yield return new WaitForSeconds(towerAnimator.GetCurrentAnimatorStateInfo(0).length);
        yield return TowerHelpers.AnimateLaser(GetComponent<LineRenderer>(), origin, enemy);

        yield return KillLaser(GetComponent<LineRenderer>(),origin, enemy, enemy.transform.position);
    }

    protected override void KillProjectile(GameObject sphere, GameObject enemy, Vector3 _enemyPosition) { }

    override protected IEnumerator KillLaser(LineRenderer laserRenderer,Transform origin, GameObject enemy, Vector3 enemyPosition)
    {
        laserRenderer.SetPosition(1, origin.position);

        if (enemy == null) yield return null;

        BaseEnemy baseEnemy = enemy.GetComponent<BaseEnemy>();
        if (baseEnemy != null)
        {
            baseEnemy.TakeDamage((int)damage, DamageTypes.MAGIC);
            Debug.Log("Damage applied to enemy: " + enemy.name);
        }
        else
        {
            Debug.LogWarning("BaseEnemy component not found on " + enemy.name);
        }

        yield return null;
    }
}