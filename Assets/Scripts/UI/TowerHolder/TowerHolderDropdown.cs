using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TowerHolderDropdown : MonoBehaviour
{
	[HideInInspector]
	public GameObject towerHolder;

	public void OnValueChanged(int value)
	{
		TowerHelpers.TowerTargetTypes targetType = TowerHelpers.GetTargetTypeByIndex(value);
		towerHolder.GetComponent<TowerHolderNeo>().towerInstance.GetComponent<BaseTower>().targetType = targetType;
		towerHolder.GetComponent<TowerHolderNeo>().HideButtons();
	}
}
