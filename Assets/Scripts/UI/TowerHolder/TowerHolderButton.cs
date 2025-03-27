using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ButtonAction
{
	NONE,
	BUILD_ARCHER,
	BUILD_BARRACKS,
	BUILD_MAGIC,
	BUILD_BOMB,
	CYCLE_RETARGET,
	SELL,
	UPGRADE_LEVEL,
	BUY_EVOLUTION_1,
	BUY_EVOLUTION_2,
	UPGRADE_EVOLUTION,
}

public static class ButtonActionExtensions
{
	public static TowerTypes? GetTowerType(this ButtonAction action)
	{
		switch (action)
		{
			case ButtonAction.BUILD_ARCHER:
				return TowerTypes.Archer;
			case ButtonAction.BUILD_BARRACKS:
				return TowerTypes.Barracks;
			case ButtonAction.BUILD_MAGIC:
				return TowerTypes.Magic;
			case ButtonAction.BUILD_BOMB:
				return TowerTypes.Bomb;
			default:
				return null;
		}
	}

	public static int GetEvolutionIndex(this ButtonAction action)
	{
		switch (action)
		{
			case ButtonAction.BUY_EVOLUTION_1:
				return 0;
			case ButtonAction.BUY_EVOLUTION_2:
				return 1;
			default:
				return -1;
		}
	}
}

public class TowerHolderButton : MonoBehaviour
{
	[HideInInspector]
	public ButtonAction buttonAction;

	[HideInInspector]
	public GameObject towerHolder;

	[HideInInspector]
	public LineRenderer lineRenderer;

	[HideInInspector]
	[System.NonSerialized]
	public bool isMouseOver = false;

	public SpriteRenderer iconSpriteRenderer;

	private SpriteRenderer backgroundSpriteRenderer;
	private readonly TowerHelpers.TowerTargetTypes[] targetTypes =
	{
		TowerHelpers.TowerTargetTypes.CLOSEST_TO_FINISH,
		TowerHelpers.TowerTargetTypes.CLOSEST_TO_START,
		TowerHelpers.TowerTargetTypes.MOST_HP,
		TowerHelpers.TowerTargetTypes.LEAST_HP,
	};

	public void Awake()
	{
		backgroundSpriteRenderer = GetComponent<SpriteRenderer>();
		lineRenderer = gameObject.AddComponent<LineRenderer>();
		lineRenderer.enabled = false;
	}

	public void OnMouseDown()
	{
		TooltipManager.Hide();
		towerHolder.GetComponent<TowerHolderNeo>().ButtonClicked(buttonAction);

		if (buttonAction == ButtonAction.CYCLE_RETARGET)
		{
			ShowTooltip();
		}
		else
		{
			isMouseOver = false;
		}
	}

	void OnMouseEnter()
	{
		isMouseOver = true;
		ShowTooltip();
	}

	void OnMouseExit()
	{
		isMouseOver = false;
		TooltipManager.Hide();
		lineRenderer.enabled = false;
	}

	void ShowTooltip()
	{
		BaseTower tower = null;
		BaseEvolutionTower evolutionTower = null;

		switch (buttonAction)
		{
			case ButtonAction.BUILD_ARCHER:
			case ButtonAction.BUILD_BARRACKS:
			case ButtonAction.BUILD_MAGIC:
			case ButtonAction.BUILD_BOMB:
				tower = towerHolder.GetComponent<TowerHolderNeo>().GetBaseTowerScript(buttonAction.GetTowerType());
				string stats = tower.towerData.GetBuyStats();
				TooltipManager.Show(tower.towerData.towerName, stats);
				TowerHelpers.SetRangeCircle(
					lineRenderer,
					tower.towerData.levels[0].range,
					towerHolder.transform.position
				);
				lineRenderer.enabled = true;

				break;

			case ButtonAction.CYCLE_RETARGET:
				int targetTypeIndex = towerHolder.GetComponent<TowerHolderNeo>().targetTypeIndex;

				TooltipManager.Show(
					"Cycle targeting strategy",
					"Current: "
						+ targetTypes[targetTypeIndex].GetString()
						+ "\nChange to: "
						+ targetTypes[(targetTypeIndex + 1) % targetTypes.Length].GetString()
				);
				break;

			case ButtonAction.UPGRADE_LEVEL:
				tower = towerHolder.GetComponent<TowerHolderNeo>().GetBaseTowerScript();
				string upgradeStats = tower.towerData.GetUpgradeStats(tower.level);
				TooltipManager.Show(tower.towerData.towerName + " " + (tower.level + 2), upgradeStats);
				TowerHelpers.SetRangeCircle(
					lineRenderer,
					tower.towerData.levels[tower.level + 1].range,
					towerHolder.transform.position
				);
				lineRenderer.enabled = true;
				break;

			case ButtonAction.SELL:
				if (!towerHolder.GetComponent<TowerHolderNeo>().isEvolutionTower())
				{
					tower = towerHolder.GetComponent<TowerHolderNeo>().GetBaseTowerScript();
					TooltipManager.Show(
						"Sell " + tower.towerData.towerName,
						"Sell for " + tower.CalculateSellPrice() + " gold"
					);
					break;
				}
				else
				{
					evolutionTower = towerHolder.GetComponent<TowerHolderNeo>().GetBaseEvolutionTowerScript();
					TooltipManager.Show(
						"Sell " + evolutionTower.towerData.evolutions[evolutionTower.evolutionIndex].name,
						"Sell for " + evolutionTower.CalculateSellPrice() + " gold"
					);
				}

				break;

			case ButtonAction.BUY_EVOLUTION_1:
			case ButtonAction.BUY_EVOLUTION_2:
				int evolutionIndex = buttonAction.GetEvolutionIndex();
				tower = towerHolder.GetComponent<TowerHolderNeo>().GetBaseTowerScript();
				string EvolutionStats = tower.towerData.GetEvolutionBuyStats(evolutionIndex);
				TooltipManager.Show(tower.towerData.evolutions[evolutionIndex].name, EvolutionStats);

				TowerHelpers.SetRangeCircle(
					lineRenderer,
					tower.towerData.evolutions[evolutionIndex].range,
					towerHolder.transform.position
				);
				lineRenderer.enabled = true;
				break;

			case ButtonAction.UPGRADE_EVOLUTION:
				evolutionTower = towerHolder.GetComponent<TowerHolderNeo>().GetBaseEvolutionTowerScript();
				if (
					evolutionTower.skillLevel + 1
					>= evolutionTower.towerData.evolutions[evolutionTower.evolutionIndex].skillLevels.Length
				)
				{
					TooltipManager.Show("Max level reached", "No more upgrades available");
					return;
				}

				string skillStats =
					evolutionTower.skillLevel == -1
						? evolutionTower.towerData.GetEvolutionSkillStats(evolutionTower.evolutionIndex, 0)
						: evolutionTower.towerData.GetEvolutionSkillUpgradeStats(
							evolutionTower.evolutionIndex,
							evolutionTower.skillLevel
						);

				TooltipManager.Show(
					evolutionTower.towerData.evolutions[evolutionTower.evolutionIndex].skillName
						+ " "
						+ (evolutionTower.skillLevel + 2),
					skillStats
				);
				break;

			default:
				TooltipManager.Show("Error", "No action set for this button");
				break;
		}
	}

	public void SetAction(ButtonAction action)
	{
		buttonAction = action;

		Sprite buttonSprite = towerHolder.GetComponent<TowerHolderNeo>().towerIcons[action];

		iconSpriteRenderer.sprite = buttonSprite;
	}
}
