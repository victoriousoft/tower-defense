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
                towerHolder.GetComponent<TowerHolder>().BuildTower(TowerTypes.Barracks);
                break;
            case TowerTypes.Archer:
                towerHolder.GetComponent<TowerHolder>().BuildTower(TowerTypes.Archer);
                break;
            case TowerTypes.Magic:
                towerHolder.GetComponent<TowerHolder>().BuildTower(TowerTypes.Magic);
                break;
            case TowerTypes.Bomb:
                towerHolder.GetComponent<TowerHolder>().BuildTower(TowerTypes.Bomb);
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
