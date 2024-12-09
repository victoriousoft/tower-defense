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
                StartCoroutine(towerHolder.GetComponent<TowerHolder>().BuildTower(TowerTypes.Barracks));
                break;
            case TowerTypes.Archer:
                StartCoroutine(towerHolder.GetComponent<TowerHolder>().BuildTower(TowerTypes.Archer));
                break;
            case TowerTypes.Magic:
                StartCoroutine(towerHolder.GetComponent<TowerHolder>().BuildTower(TowerTypes.Magic));
                break;
            case TowerTypes.Bomb:
                StartCoroutine(towerHolder.GetComponent<TowerHolder>().BuildTower(TowerTypes.Bomb));
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
    }
}
