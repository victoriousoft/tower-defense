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
	SELL,
	UPGRADE_LEVEL,
	BUY_EVOLUTION_1,
	BUY_EVOLUTION_2,
	UPGRADE_EVOLUTION_0,
	UPGRADE_EVOLUTION_1,
	UPGRADE_EVOLUTION_2,
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

	public SpriteRenderer iconSpriteRenderer;

	private LineRenderer lineRenderer;
	private SpriteRenderer backgroundSpriteRenderer;

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
	}

	void OnMouseEnter()
	{
		BaseTower tower = null;

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

			case ButtonAction.UPGRADE_LEVEL:
				tower = towerHolder.GetComponent<TowerHolderNeo>().GetBaseTowerScript(null);
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
				tower = towerHolder.GetComponent<TowerHolderNeo>().GetBaseTowerScript(null);
				TooltipManager.Show(
					"Sell " + tower.towerData.towerName,
					"Sell for " + tower.towerData.levels[0].price / 2
				);
				break;

			case ButtonAction.BUY_EVOLUTION_1:
			case ButtonAction.BUY_EVOLUTION_2:
				int evolutionIndex = buttonAction.GetEvolutionIndex();
				break;

			default:
				TooltipManager.Show("Error", "No action set for this button");
				break;
		}
	}

	void OnMouseExit()
	{
		TooltipManager.Hide();
		lineRenderer.enabled = false;
	}

	public void SetAction(ButtonAction action)
	{
		buttonAction = action;

		try
		{
			Sprite buttonSprite = towerHolder.GetComponent<TowerHolderNeo>().towerIcons[action];
			iconSpriteRenderer.sprite = buttonSprite;

			float buttonWidth = backgroundSpriteRenderer.bounds.size.x;
			float buttonHeight = backgroundSpriteRenderer.bounds.size.y;

			float iconWidth = buttonSprite.bounds.size.x;
			float iconHeight = buttonSprite.bounds.size.y;

			float xScale = buttonWidth / iconWidth;
			float yScale = buttonHeight / iconHeight;

			iconSpriteRenderer.transform.localScale = new Vector2(xScale, yScale);
		}
		catch (Exception e)
		{
			Debug.LogError("Error setting icon sprite for " + action + ": " + e.Message);
		}
	}
}
