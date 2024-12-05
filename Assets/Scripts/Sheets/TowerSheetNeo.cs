using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "ScriptableObjects/TowerSheetNeo", order = 1)]
public class TowerSheetNeo : ScriptableObject
{
    [System.Serializable]
    public class Level
    {
        public int price;
        public int range;
        public int damage;
        public int cooldown;
    }

    [System.Serializable]
    public class Evolution
    {
        public string name;
        public GameObject prefab;
        public int price;
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
    public Level[] levels;
    public Evolution[] evolutions;
}