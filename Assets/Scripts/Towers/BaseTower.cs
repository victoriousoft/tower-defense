using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTower : MonoBehaviour
{
    public int level = 0;
    public TowerSheetNeo towerData;
    private PlayerStatsManager playerStats;
    public TowerHelpers.TowerTargetTypes targetType = TowerHelpers.TowerTargetTypes.CLOSEST_TO_FINISH;
    protected bool canShoot = true;

    protected abstract IEnumerator Shoot(GameObject enemy);
    protected abstract void KillProjectile(GameObject projectile, GameObject enemy, Vector3 enemyPosition);

    void Awake()
    {
        playerStats = GameObject.Find("PlayerStats").GetComponent<PlayerStatsManager>();
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
        yield return Shoot(target);
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
        if (playerStats.SubtractGold(towerData.levels[level + 1].price))
        {
            level++;
        }
    }

    public void ChangeTargeting()
    {
        //změnit targeting
    }
}