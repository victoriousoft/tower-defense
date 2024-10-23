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
        TowerTypes.barracksPrefab = barracksPrefab;
        TowerTypes.archerPrefab = archerPrefab;
        TowerTypes.magicPrefab = magicPrefab;
        TowerTypes.bombPrefab = bombPrefab;
    }
}