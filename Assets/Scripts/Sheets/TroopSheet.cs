using UnityEngine;

[CreateAssetMenu(fileName = "TroopData", menuName = "ScriptableObjects/TroopSheet", order = 1)]
public class TroopSheet : ScriptableObject
{
	[System.Serializable]
	public class Info
	{
		public string name;
	}

	[System.Serializable]
	public class Stats
	{
		public float maxHealth;
		public float damage;
		public float speed;
		public float attackRange;
		public float visRange;
		public float attackCooldown;
	}

	public Info info;
	public Stats stats;
	public EnemyTypes[] enemyTypes;
}
