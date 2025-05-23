using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "ScriptableObjects/TowerSheetNeo", order = 1)]
public class TowerSheetNeo : ScriptableObject
{
	[System.Serializable]
	public class Level
	{
		public int price;
		public float range;
		public float damage;
		public float cooldown;
	}

	[System.Serializable]
	public class Evolution
	{
		public string name;
		public string description;
		public GameObject prefab;
		public int price;
		public float damage;
		public float range;
		public float cooldown;
		public string skillName;
		public string skillDescription;
		public Skill[] skillLevels;

		public AudioClip shootSound;
		public AudioClip hitSound;
		public AudioClip skillSound;
	}

	[System.Serializable]
	public class Skill
	{
		public int cooldown;
		public int upragdeCost;
	}

	public TowerTypes towerType;
	public string towerName;
	public string description;
	public DamageTypes damageType;

	[SerializeField]
	public EnemyTypes[] enemyTypes;

	[System.Serializable]
	public class EnemyTypeList
	{
		public List<EnemyTypes> enemies = new List<EnemyTypes>();
	}

	[SerializeField]
	public List<EnemyTypeList> evolutionEnemyTypes = new List<EnemyTypeList>();

	public Level[] levels;
	public Evolution[] evolutions;

	public AudioClip shootSound;
	public AudioClip hitSound;

	public string GetBuyStats()
	{
		Level l = levels[0];
		string stats = description + "\n";
		stats += "Damage: " + l.damage + "\n";
		stats += "Range: " + l.range + "\n";
		stats += "Cooldown: " + l.cooldown + "\n";
		stats += "Price: " + l.price + "\n";
		return stats;
	}

	public string GetEvolutionBuyStats(int evolutionIndex)
	{
		Evolution e = evolutions[evolutionIndex];
		string stats = e.description + "\n";
		stats += "Damage: " + e.damage + "\n";
		stats += "Range: " + e.range + "\n";
		stats += "Cooldown: " + e.cooldown + "\n";
		stats += "Price: " + e.price + "\n";
		return stats;
	}

	public string GetEvolutionSkillStats(int evolutionIndex, int skillLevel)
	{
		Skill s = evolutions[evolutionIndex].skillLevels[skillLevel];
		string stats = evolutions[evolutionIndex].skillDescription + "\n";
		stats += "Cooldown: " + s.cooldown + "\n";
		stats += "Price: " + s.upragdeCost + "\n";
		return stats;
	}

	public string GetEvolutionSkillUpgradeStats(int evolutionIndex, int currentLevel)
	{
		Skill s = evolutions[evolutionIndex].skillLevels[currentLevel];
		Skill s2 = evolutions[evolutionIndex].skillLevels[currentLevel + 1];
		string stats = "Cooldown: " + s.cooldown + " -> " + s2.cooldown + "\n";
		stats += "Price: " + s2.upragdeCost + "\n";
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
