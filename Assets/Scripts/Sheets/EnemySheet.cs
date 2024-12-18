using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemySheet", order = 1)]
public class EnemySheet : ScriptableObject
{
    [System.Serializable]
    public class Info
    {
        public string name;
        public string description;
    }

    [System.Serializable]
    public class Stats
    {
        public float maxHealth;
        public float damage;
        public int cashDrop;
        public float speed;
        [Range(0, 4)] public int physicalResistance;
        [Range(0, 4)] public int magicResistance;
        public float attackRange;
        public float visRange;
        public float attackCooldown;
        public int playerLives;
    }

    public Stats stats;
    public Info info;
    public EnemyTypes enemyType;
}