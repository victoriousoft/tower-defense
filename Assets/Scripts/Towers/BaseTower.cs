using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTower : MonoBehaviour
{
    public int level = 0;
    public TowerSheetNeo towerData;
    private PlayerStatsManager playerStats;
    private Animator towerAnimator;
    public TowerHelpers.TowerTargetTypes targetType = TowerHelpers.TowerTargetTypes.CLOSEST_TO_FINISH;
    protected bool canShoot = true;

    protected abstract IEnumerator Shoot(GameObject enemy);
    protected abstract IEnumerator ChargeUp(GameObject enemy);
    protected abstract void KillProjectile(GameObject projectile, GameObject enemy, Vector3 enemyPosition);

    void Awake()
    {
        playerStats = GameObject.Find("PlayerStats").GetComponent<PlayerStatsManager>();
        towerAnimator = GetComponent<Animator>();
    }

    protected virtual void FixedUpdate()
    {
        if (!canShoot) return;

        GameObject[] enemies = TowerHelpers.GetEnemiesInRange(transform.position, towerData.levels[level].range);
        if (enemies.Length == 0) return;

        GameObject target = TowerHelpers.SelectEnemyToAttack(enemies, targetType);

        StartCoroutine(ShootAndResetCooldown(target));
        canShoot = false;
    }

    private IEnumerator ShootAndResetCooldown(GameObject target)
    {   
        towerAnimator.SetTrigger("attack");
        yield return new WaitForSeconds(towerAnimator.GetCurrentAnimatorStateInfo(0).length/2);

        //yield return ChargeUp(target);
        if (Vector2.Distance(transform.position, target.transform.position) > towerData.levels[level].range || target == null)
        {
            GameObject[] enemies = TowerHelpers.GetEnemiesInRange(transform.position, towerData.levels[level].range);
            if (enemies.Length == 0) {canShoot = true; yield break;}
            target = TowerHelpers.SelectEnemyToAttack(TowerHelpers.GetEnemiesInRange(transform.position, towerData.levels[level].range), targetType);
        }

        yield return Shoot(target);

        yield return new WaitForSeconds(towerData.levels[level].cooldown);
        canShoot = true;
    }

    IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(towerData.levels[level].cooldown);
        canShoot = true;
    }
    public void UpgradeTower()
    {
        // TODO: fix pro evoluce (level bude asi out of bounds)
        if (playerStats.SubtractGold(towerData.levels[level].price))
        {
            level++;
            towerAnimator.SetTrigger("upgrade");
        }
    }

    public void ChangeTargeting()
    {
        //změnit targeting
    }
}