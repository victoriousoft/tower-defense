using System.Collections.Generic;
using UnityEngine;

public class TowerInitializer : MonoBehaviour
{
    public GameObject barracksPrefab;
    public GameObject archerPrefab;
    public GameObject magicPrefab;
    public GameObject bombPrefab;

    void Awake()
    {
        TowerTypes.towerDictionary= new Dictionary<string, GameObject>
        {
            { "BARRACKS", barracksPrefab },
            { "ARCHER", archerPrefab },
            { "MAGIC", magicPrefab },
            { "BOMB", bombPrefab },
        };
    }
}