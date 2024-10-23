using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerHolder : MonoBehaviour
{
    private GameObject towerInstance;
    private PlayerStatsManager playerStats;

    void Awake()
    {
        playerStats = GameObject.Find("PlayerStats").GetComponent<PlayerStatsManager>();
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P)) BuildTower(TowerTypes.towerDictionary["ARCHER"]);
        if(Input.GetKeyDown(KeyCode.O)) SellTower();
    }
    void BuildTower(GameObject tower){
        if(playerStats.SubtractGold(0) && towerInstance == null){
            towerInstance = Instantiate(tower,transform.position,Quaternion.identity,transform);
        }
    }
    void SellTower(){
        if(towerInstance!=null){
            
            Destroy(towerInstance);
            towerInstance = null;
        }
    }
    void UpgradeTower(){

    }
}
