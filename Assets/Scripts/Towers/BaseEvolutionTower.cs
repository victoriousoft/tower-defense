using System.Collections;
using UnityEngine;

public class BaseEvolutionTower : BaseTower
{
    public int evolutionIndex;
    public int[] skillLevels;

    protected override IEnumerator AnimateProjectile(GameObject enemy)
    {
        throw new System.NotImplementedException();
    }

    protected override void KillProjectile(GameObject projectile, GameObject enemy, Vector3 enemyPosition)
    {
        throw new System.NotImplementedException();
    }

    protected override void ExtendedAwake()
    {
        skillLevels = new int[TowerSheet.towerDictionary[towerType].evolutions[evolutionIndex].skills.Length];
    }

    public void BuySkill(int skillIndex)
    {
        if (playerStats.SubtractGold(TowerSheet.towerDictionary[towerType].evolutions[level].skills[skillIndex].upgradeCosts[level]))
        {
            skillLevels[skillIndex]++;
        }
    }
}
