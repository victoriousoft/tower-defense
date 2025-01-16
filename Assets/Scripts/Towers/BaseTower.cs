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
    protected GameObject paths;
    private Coroutine shootCoroutine;

    protected abstract IEnumerator Shoot(GameObject enemy);
    protected abstract IEnumerator ChargeUp(GameObject enemy);
    protected abstract void KillProjectile(GameObject projectile, GameObject enemy, Vector3 enemyPosition);

    protected virtual void ExtendedAwake() { }

    void Awake()
    {
        playerStats = GameObject.Find("PlayerStats").GetComponent<PlayerStatsManager>();
        towerAnimator = GetComponent<Animator>();
        ExtendedAwake();
    }

    protected virtual void FixedUpdate()
    {
        if (!canShoot) return;

        GameObject[] enemies = TowerHelpers.GetEnemiesInRange(transform.position, towerData.levels[level].range, towerData.enemyTypes);
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

        GameObject[] enemies = TowerHelpers.GetEnemiesInRange(transform.position, towerData.levels[level - 1].range, towerData.enemyTypes);
        if (enemies.Length == 0) { canShoot = true; Debug.Log("issue"); yield break; }
        GameObject target = TowerHelpers.SelectEnemyToAttack(TowerHelpers.GetEnemiesInRange(transform.position, towerData.levels[level].range - 1, towerData.enemyTypes), targetType);

        yield return Shoot(target);

        yield return new WaitForSeconds(towerData.levels[level - 1].cooldown);
        canShoot = true;
    }

    public void UpgradeTower(int evolutionIndex = -1)
    {
        StartCoroutine(UpgradeRoutine(evolutionIndex));
    }

    private IEnumerator UpgradeRoutine(int evolutionIndex)
    {
        canShoot = false;

        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
        }

        Debug.Log($"Level: {level}, max level: {towerData.levels.Length - 1}");
        if (towerData.levels.Length > level + 1)
        {
            // upgrade level
            if (playerStats.SubtractGold(towerData.levels[level + 1].price))
            {
                level++;
                towerAnimator.SetTrigger("upgrade");

                yield return null;
            }
        }
        else
        {
            // upgrade to evolution
            if (evolutionIndex == -1 || evolutionIndex >= towerData.evolutions.Length)
            {
                Debug.LogError($"Invalid evolution index: {evolutionIndex}, max index: {towerData.evolutions.Length - 1}");
                canShoot = true;
                yield break;
            }
            if (playerStats.SubtractGold(towerData.evolutions[evolutionIndex].price))
            {
                towerAnimator.SetTrigger("upgrade");

                Instantiate(towerData.evolutions[evolutionIndex].prefab, transform.position, Quaternion.identity);

                Destroy(gameObject);
            }
        }

        canShoot = true;
    }

    public void ChangeTargeting()
    {
        // Change targeting logic
    }
}