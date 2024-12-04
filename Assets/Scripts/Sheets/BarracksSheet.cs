using UnityEngine;

[CreateAssetMenu(fileName = "BarracksData", menuName = "ScriptableObjects/BarracksSheet", order = 2)]
public class BarracksSheet : TowerSheetNeo
{
    public class ExtendedLevel : Level
    {
        public int troopCount;
    }

    public class BarracksEvolution : Evolution
    {
        public int troopCount;
    }

    public new ExtendedLevel[] levels;
    public new BarracksEvolution[] evolutions;
}