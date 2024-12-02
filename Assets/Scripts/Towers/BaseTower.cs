using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTower : MonoBehaviour
{
    public float range = 4;
    public float cooldown = 1;
    public float damage = 0;
    public int level = 1;
    public TowerTypes towerType;
    protected PlayerStatsManager playerStats;
    public TowerHelpers.TowerTargetTypes targetType = TowerHelpers.TowerTargetTypes.CLOSEST_TO_FINISH;

    public GameObject[] evolutionPrefabs;

    protected bool canShoot = true;
    protected abstract IEnumerator AnimateProjectile(GameObject enemy);
    protected abstract void KillProjectile(GameObject projectile, GameObject enemy, Vector3 enemyPosition);
    protected virtual void ExtendedAwake() { }

    void Awake()
    {
        playerStats = GameObject.Find("PlayerStats").GetComponent<PlayerStatsManager>();
        ExtendedAwake();
    }
    protected virtual void FixedUpdate()
    {
        if (!canShoot) return;

        GameObject[] enemies = TowerHelpers.GetEnemiesInRange(transform.position, range);
        if (enemies.Length == 0) return;

        GameObject target = TowerHelpers.SelectEnemyToAttack(enemies, targetType);

        StartCoroutine(AnimateProjectile(target));
        canShoot = false;
        StartCoroutine(ResetCooldown());
    }

    IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        canShoot = true;
    }
    public void UpgradeTower()
    {
        if (playerStats.SubtractGold(TowerSheet.towerDictionary[towerType].upgradePrices[level]))
        {
            damage = TowerSheet.towerDictionary[towerType].damageValues[level];
            level++;
        }
    }

    public void BuyEvolution(int evolutionIndex)
    {
        if (playerStats.SubtractGold(TowerSheet.towerDictionary[towerType].evolutions[evolutionIndex].basePrice))
        {
            canShoot = false;
            Instantiate(evolutionPrefabs[evolutionIndex], transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    public void SellTower()
    {
        playerStats.AddGold(TowerSheet.towerDictionary[towerType].upgradePrices[level] / 2);
        Destroy(gameObject);
    }

    public void ChangeTargeting()
    {
        //změnit targeting
    }

}
