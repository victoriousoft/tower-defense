using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTower : MonoBehaviour
{
    public int level = 0;
    public TowerSheetNeo towerData;
    public TowerHelpers.TowerTargetTypes targetType = TowerHelpers.TowerTargetTypes.CLOSEST_TO_FINISH;
    public int evolutionIndex = -1;
    public int[] skillLevels;

    protected bool canShoot = true;
    private PlayerStatsManager playerStats;

    protected abstract IEnumerator AnimateProjectile(GameObject enemy);
    protected abstract void KillProjectile(GameObject projectile, GameObject enemy, Vector3 enemyPosition);

    void Awake()
    {

        playerStats = GameObject.Find("PlayerStats").GetComponent<PlayerStatsManager>();
        if (evolutionIndex != -1) skillLevels = new int[towerData.evolutions[evolutionIndex].skills.Length];
    }

    protected virtual void FixedUpdate()
    {
        if (!canShoot) return;

        GameObject[] enemies = TowerHelpers.GetEnemiesInRange(transform.position, towerData.levels[level].range);
        if (enemies.Length == 0) return;

        GameObject target = TowerHelpers.SelectEnemyToAttack(enemies, targetType);

        StartCoroutine(AnimateProjectile(target));
        canShoot = false;
        StartCoroutine(ResetCooldown());
    }

    IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(towerData.levels[level].cooldown);
        canShoot = true;
    }

    public virtual void UpgradeTower()
    {
        if (playerStats.SubtractGold(towerData.levels[level].price))
        {
            level++;
        }
    }

    public void BuyEvolution(int evolutionIndex)
    {
        if (evolutionIndex != -1) Debug.Log("Cannot buy evolution, evolution index is not -1");
        if (playerStats.SubtractGold(towerData.evolutions[evolutionIndex].price))
        {
            canShoot = false;
            Instantiate(towerData.evolutions[evolutionIndex].prefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public void BuySkill(int skillIndex)
    {
        if (evolutionIndex == -1) Debug.Log("Cannot buy skill, evolution index is -1");
        if (playerStats.SubtractGold(towerData.evolutions[evolutionIndex].skills[skillIndex].upgradeCosts[skillLevels[skillIndex]]))
        {
            skillLevels[skillIndex]++;
        }
    }

    public void SellTower()
    {
        playerStats.AddGold(towerData.levels[level].price / 2);
        Destroy(gameObject);
    }

    public void ChangeTargeting(TowerHelpers.TowerTargetTypes newTargetType)
    {
        targetType = newTargetType;
    }
}