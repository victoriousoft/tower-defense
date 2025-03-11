using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "ScriptableObjects/TowerSheetNeo", order = 1)]
public class TowerSheetNeo : ScriptableObject
{
	[System.Serializable]
	public class Level
	{
		public int price;
		public int range;
		public float damage;
		public float cooldown;
	}

	[System.Serializable]
	public class Evolution
	{
		public string name;
		public GameObject prefab;
		public int price;
		public int damage;
		public int range;
		public int cooldown;
		public Skill[] skills;
	}

	[System.Serializable]
	public class Skill
	{
		public string name;
		public int[] upgradeCosts;
	}

	public TowerTypes towerType;
	public string towerName;
	public string description;
	public DamageTypes damageType;

	[SerializeField]
	public EnemyTypes[] enemyTypes;

	public Level[] levels;
	public Evolution[] evolutions;

	public string GetBuyStats()
	{
		Level l = levels[0];
		string stats = "Damage: " + l.damage + "\n";
		stats += "Range: " + l.range + "\n";
		stats += "Cooldown: " + l.cooldown + "\n";
		stats += "Price: " + l.price + "\n";
		return stats;
	}

	public string GetEvolutionBuyStats(int evolutionIndex)
	{
		Evolution e = evolutions[evolutionIndex];
		string stats = "Damage: " + e.damage + "\n";
		stats += "Range: " + e.range + "\n";
		stats += "Cooldown: " + e.cooldown + "\n";
		stats += "Price: " + e.price + "\n";
		return stats;
	}

	public string GetUpgradeStats(int currentLevel)
	{
		Level l = levels[currentLevel];
		Level l2 = levels[currentLevel + 1];
		string stats = "Damage: " + l.damage + " -> " + l2.damage + "\n";
		stats += "Range: " + l.range + " -> " + l2.range + "\n";
		stats += "Cooldown: " + l.cooldown + " -> " + l2.cooldown + "\n";
		stats += "Price: " + l2.price + "\n";
		return stats;
	}
}
