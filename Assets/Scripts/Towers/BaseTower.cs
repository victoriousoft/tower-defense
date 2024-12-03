using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTower : MonoBehaviour
{
    public float range = 4;
    public float cooldown = 1;
    public float damage = 0;
    public int level = 1; 
    public string towerName;
    public TowerTypes towerType;
    private PlayerStatsManager playerStats;
    public TowerHelpers.TowerTargetTypes targetType = TowerHelpers.TowerTargetTypes.CLOSEST_TO_FINISH;
    protected bool canShoot = true;

    protected abstract IEnumerator AnimateProjectile(GameObject enemy);
    protected abstract void KillProjectile(GameObject projectile, GameObject enemy, Vector3 enemyPosition);

    void Awake(){
        playerStats = GameObject.Find("PlayerStats").GetComponent<PlayerStatsManager>();
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
    public void UpgradeTower(){
        if(playerStats.SubtractGold(TowerSheet.towerDictionary[towerType].upgradePrices[level])){
            damage = TowerSheet.towerDictionary[towerType].damageValues[level];
            level++;
        }
    }

    public void ChangeTargeting(){
        //změnit targeting
    }

}
