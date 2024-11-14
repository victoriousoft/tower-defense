using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerHolder : MonoBehaviour
{
    public GameObject UIMenu;

    private GameObject towerInstance;
    private PlayerStatsManager playerStats;
    private SpriteRenderer sprite;

    public GameObject barracksPrefab;
    public GameObject archerPrefab;
    public GameObject magicPrefab;
    public GameObject bombPrefab;
    private Dictionary<string, GameObject> towerPrefabs;

    void Awake()
    {
        UIMenu.SetActive(false);
        playerStats = GameObject.Find("PlayerStats").GetComponent<PlayerStatsManager>();
        sprite = GetComponent<SpriteRenderer>();

        //dal jsem to sem aby se daly accessnout data v sheetu
        towerPrefabs = new Dictionary<string, GameObject>
        {
            { "BARRACKS", barracksPrefab },
            { "ARCHER", archerPrefab },
            { "MAGIC", magicPrefab },
            { "BOMB", bombPrefab }
        };
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.U)) UpgradeTower();
    }

    public void BuildTower(string towerName)
    {
        if (playerStats.SubtractGold(TowerSheet.towerDictionary[towerName].basePrice) && towerInstance == null)
        {
            towerInstance = Instantiate(towerPrefabs[towerName], transform.position, Quaternion.identity, transform);
            towerInstance.GetComponent<BaseTower>().towerName = towerName;
            sprite.enabled = false;
        }
        else if (!playerStats.SubtractGold(100))
        {
            Debug.Log("nedeostatek peněz");
        }

        UIMenu.SetActive(false);
    }

    public void SellTower()
    {
        if (towerInstance != null)
        {

            Destroy(towerInstance);
            towerInstance = null;
            playerStats.AddGold(100);
            sprite.enabled = true;
        }

        UIMenu.SetActive(false);
    }

    void UpgradeTower()
    {
        BaseTower baseTowerScript = towerInstance.GetComponent<BaseTower>();
        if (baseTowerScript != null)
        {
           baseTowerScript.UpgradeTower();
        }
        else
        {
            Debug.LogWarning("No BaseTower-derived script is attached to the tower instance.");
        }
    }

    private void OnMouseDown()
    {
        Debug.Log("clicked");

        UIMenu.SetActive(!UIMenu.activeSelf);
    }
}
