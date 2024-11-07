using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerButton : MonoBehaviour
{
    public GameObject towerHolder;
    public TowerTypes towerType;

    void OnMouseDown()
    {
        switch (towerType)
        {
            case TowerTypes.Barracks:
                towerHolder.GetComponent<TowerHolder>().BuildTower(towerHolder.GetComponent<TowerHolder>().barracksPrefab);
                break;
            case TowerTypes.Archer:
                towerHolder.GetComponent<TowerHolder>().BuildTower(towerHolder.GetComponent<TowerHolder>().archerPrefab);
                break;
            case TowerTypes.Magic:
                towerHolder.GetComponent<TowerHolder>().BuildTower(towerHolder.GetComponent<TowerHolder>().magicPrefab);
                break;
            case TowerTypes.Bomb:
                towerHolder.GetComponent<TowerHolder>().BuildTower(towerHolder.GetComponent<TowerHolder>().bombPrefab);
                break;
            case TowerTypes.Destroy:
                towerHolder.GetComponent<TowerHolder>().SellTower();
                break;
        }
    }
}
