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
                towerHolder.GetComponent<TowerHolder>().BuildTower("BARRACKS");
                break;
            case TowerTypes.Archer:
                towerHolder.GetComponent<TowerHolder>().BuildTower("ARCHER");
                break;
            case TowerTypes.Magic:
                towerHolder.GetComponent<TowerHolder>().BuildTower("MAGIC");
                break;
            case TowerTypes.Bomb:
                towerHolder.GetComponent<TowerHolder>().BuildTower("BOMB");
                break;
            case TowerTypes.Destroy:
                towerHolder.GetComponent<TowerHolder>().SellTower();
                break;
            case TowerTypes.Upgrade:
                towerHolder.GetComponent<TowerHolder>().UpgradeTower();
                break;
            case TowerTypes.Retarget:
                towerHolder.GetComponent<TowerHolder>().ChangeTargeting();
                break;
        }
        towerHolder.GetComponent<TowerHolder>().UIAnimator.SetTrigger("enable");
        towerHolder.GetComponent<TowerHolder>().UIMenu.SetActive(!towerHolder.GetComponent<TowerHolder>().UIMenu.activeSelf);
    }
}
