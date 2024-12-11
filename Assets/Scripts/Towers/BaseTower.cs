using System.Collections;
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

    private Coroutine shootCoroutine;

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

        shootCoroutine = StartCoroutine(ShootAndResetCooldown());
    }

    private IEnumerator ShootAndResetCooldown()
    {
        canShoot = false;
        towerAnimator.SetTrigger("attack");

        while (towerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        GameObject[] enemies = TowerHelpers.GetEnemiesInRange(transform.position, towerData.levels[level - 1].range);
        if (enemies.Length == 0) { canShoot = true; Debug.Log("issue"); yield break; }
        GameObject target = TowerHelpers.SelectEnemyToAttack(TowerHelpers.GetEnemiesInRange(transform.position, towerData.levels[level].range - 1), targetType);

        yield return Shoot(target);

        yield return new WaitForSeconds(towerData.levels[level - 1].cooldown);
        canShoot = true;
    }

    public void UpgradeTower()
    {
        StartCoroutine(UpgradeRoutine());
    }

    private IEnumerator UpgradeRoutine()
    {
        canShoot = false;

        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
        }

        // Upgrade the tower
        if (playerStats.SubtractGold(towerData.levels[level].price))
        {
            level++;
            towerAnimator.SetTrigger("upgrade");

            yield return null;
        }

        canShoot = true;
    }

    public void ChangeTargeting()
    {
        // Change targeting logic
    }
}