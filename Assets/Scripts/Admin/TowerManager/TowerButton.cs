using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerButton : MonoBehaviour
{
    public GameObject towerHolder;
    public TowerTypes towerType;

    private LineRenderer tempRangeRenderer;

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

    // on hover
    void OnMouseEnter()
    {
        if (towerType == TowerTypes.Upgrade)
        {

            TowerHolder towerHolderScript = towerHolder.GetComponent<TowerHolder>();
            ShowTempRangeCircle(towerHolderScript.baseTowerScript.towerData.levels[towerHolderScript.baseTowerScript.level + 1].range);
        }

        else if (towerType == TowerTypes.Barracks || towerType == TowerTypes.Archer || towerType == TowerTypes.Magic || towerType == TowerTypes.Bomb)
        {
            ShowTempRangeCircle(towerHolder.GetComponent<TowerHolder>().getPrefab(towerType).GetComponent<BaseTower>().towerData.levels[0].range);
        }
    }

    void OnMouseExit()
    {
        if (tempRangeRenderer != null)
        {
            Destroy(tempRangeRenderer.gameObject);
        }
    }

    void ShowTempRangeCircle(float range)
    {
        LineRenderer rangeRenderer = towerHolder.GetComponent<TowerHolder>().rangeRenderer;
        tempRangeRenderer = Instantiate(rangeRenderer, rangeRenderer.transform.position, rangeRenderer.transform.rotation);
        tempRangeRenderer.transform.SetParent(rangeRenderer.transform);

        TowerHelpers.SetRangeCircle(tempRangeRenderer, range, towerHolder.transform.position);
    }
}
