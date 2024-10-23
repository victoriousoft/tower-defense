using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerHolder : MonoBehaviour
{
    private GameObject towerInstance;
    private PlayerStatsManager playerStats;
    private SpriteRenderer sprite;

    void Awake()
    {
        playerStats = GameObject.Find("PlayerStats").GetComponent<PlayerStatsManager>();
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = Color.black;
    }
    void BuildTower(string tower){
        if(playerStats.SubtractGold(100) && towerInstance == null){
            towerInstance = Instantiate(TowerTypes.towerDictionary[tower],transform.position,Quaternion.identity,transform);
        }else if(!playerStats.SubtractGold(100)){
            Debug.Log("nedeostatek peněz");
        }
    }
    void SellTower(){
        if(towerInstance!=null){
            
            Destroy(towerInstance);
            towerInstance = null;
            playerStats.AddGold(100);
        }
    }
    void UpgradeTower(){

    }
    private void OnMouseEnter()
    {
        sprite.color = Color.cyan;
    }

    private void OnMouseExit()
    {
        sprite.color = Color.black;
    }
    private void OnMouseDown(){
        if(towerInstance == null){
            BuildTower("ARCHER");
        }
        else SellTower();
    }
}
