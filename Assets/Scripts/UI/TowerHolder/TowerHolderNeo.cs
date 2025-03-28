using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MenuState
{
	Initial,
	UpgradeTowerBase,
	UpgradeTowerFinal,
	EvolutionTower,
}

public enum ButtonIndex
{
	TOP_LEFT,
	TOP_CENTER,
	TOP_RIGHT,
	CENTER_LEFT,
	CENTER_RIGHT,
	BOTTOM_LEFT,
	BOTTOM_CENTER,
	BOTTOM_RIGHT,
}

public class TowerHolderNeo : MonoBehaviour
{
	public GameObject archerPrefab;
	public GameObject barracksPrefab;
	public GameObject magicPrefab;
	public GameObject bombPrefab;
	public Sprite archerIcon;
	public Sprite barracksIcon;
	public Sprite magicIcon;
	public Sprite bombIcon;

	public SpriteRenderer backgroundSprite;
	public GameObject statusBar;

	private Dictionary<TowerTypes, GameObject> towerPrefabs;
	public Dictionary<ButtonAction, Sprite> towerIcons;

	[HideInInspector]
	public MenuState menuState;

	// top left to right, center left to right, bottom left to right
	private GameObject[] menuButtons = new GameObject[8];

	[HideInInspector]
	public int targetTypeIndex = 0;

	[HideInInspector]
	public GameObject towerInstance;

	private LineRenderer rangeRenderer;
	private bool isMenuActive = false;
	private bool isMenuLocked = false;
	private Animator animator;
	private bool isMouseOver = false;
	private bool isMouseDown = false;

	private GameObject prefabToBuild;

	void Awake()
	{
		towerPrefabs = new Dictionary<TowerTypes, GameObject>
		{
			{ TowerTypes.Archer, archerPrefab },
			{ TowerTypes.Barracks, barracksPrefab },
			{ TowerTypes.Magic, magicPrefab },
			{ TowerTypes.Bomb, bombPrefab },
		};

		towerIcons = new Dictionary<ButtonAction, Sprite>
		{
			{ ButtonAction.NONE, null },
			{ ButtonAction.BUILD_ARCHER, archerIcon },
			{ ButtonAction.BUILD_BARRACKS, barracksIcon },
			{ ButtonAction.BUILD_MAGIC, magicIcon },
			{ ButtonAction.BUILD_BOMB, bombIcon },
			{ ButtonAction.CYCLE_RETARGET, null },
			{ ButtonAction.REPOSITION_BARRACKS, null },
			{ ButtonAction.SELL, null },
			{ ButtonAction.UPGRADE_LEVEL, null },
			{ ButtonAction.BUY_EVOLUTION_1, null },
			{ ButtonAction.BUY_EVOLUTION_2, null },
			{ ButtonAction.UPGRADE_EVOLUTION, null },
		};

		animator = GetComponent<Animator>();

		rangeRenderer = GetComponent<LineRenderer>();
		rangeRenderer.enabled = false;
	}

	void Update()
	{
		if (Input.GetMouseButton(0))
		{
			if (isMouseDown)
				return;
			isMouseDown = true;

			if (isMenuActive && !IsMouseOverAllButtons())
			{
				HideButtons();
				return;
			}
		}
		else
		{
			if (isMouseDown)
			{
				isMouseDown = false;
				return;
			}
		}
	}

	void Start()
	{
		Transform buttons = transform.Find("Buttons");
		if (buttons.childCount != menuButtons.Length)
		{
			Debug.LogError("menuButtons length does not match child count");
			return;
		}

		for (int i = 0; i < buttons.childCount; i++)
		{
			menuButtons[i] = buttons.GetChild(i).gameObject;
			menuButtons[i].SetActive(true);
			menuButtons[i].GetComponent<TowerHolderButton>().towerHolder = gameObject;
		}

		HideButtons();
		ChangeState(MenuState.Initial);
	}

	void OnMouseEnter()
	{
		isMouseOver = true;
	}

	void OnMouseExit()
	{
		isMouseOver = false;
	}

	void OnMouseDown()
	{
		if (isMenuActive)
			HideButtons();
		else
			ShowButtons();
		if ((towerInstance != null && towerInstance.GetComponent<BaseEvolutionTower>() != null))
		{
			towerInstance.GetComponent<BaseEvolutionTower>().UseSkill();
		}
	}

	public void ButtonClicked(ButtonAction buttonAction)
	{
		TowerTypes? towerType = buttonAction.GetTowerType();

		if (towerType != null)
		{
			StartCoroutine(BuyTower(towerPrefabs[(TowerTypes)towerType]));
			return;
		}

		switch (buttonAction)
		{
			case ButtonAction.CYCLE_RETARGET:
				targetTypeIndex =
					(targetTypeIndex + 1)
					% TowerHelpers.TowerTargetTypes.GetValues(typeof(TowerHelpers.TowerTargetTypes)).Length;
				towerInstance.GetComponent<BaseTower>().targetType = (TowerHelpers.TowerTargetTypes)targetTypeIndex;
				//HideButtons();
				Debug.Log("Target type: " + targetTypeIndex);
				break;

			case ButtonAction.SELL:
				SellTower();
				break;

			case ButtonAction.UPGRADE_LEVEL:
				UpgradeTower();
				break;

			case ButtonAction.BUY_EVOLUTION_1:
			case ButtonAction.BUY_EVOLUTION_2:
				BuildEvolutionTower(buttonAction.GetEvolutionIndex());
				break;

			case ButtonAction.UPGRADE_EVOLUTION:
				UpgradeEvolutionSkillLevel();
				break;

			case ButtonAction.REPOSITION_BARRACKS:
				RepositionBarracks();
				break;

			default:
				Debug.LogError("not implemented");
				break;
		}
	}

	public BaseTower GetBaseTowerScript(TowerTypes? prefabTowerType = null)
	{
		if (towerInstance == null && prefabTowerType != null)
		{
			return towerPrefabs[(TowerTypes)prefabTowerType].GetComponent<BaseTower>();
		}

		return towerInstance.GetComponent<BaseTower>();
	}

	public BaseEvolutionTower GetBaseEvolutionTowerScript()
	{
		return towerInstance.GetComponent<BaseEvolutionTower>();
	}

	private void ShowButtons()
	{
		if (isMenuLocked)
			return;

		isMenuActive = true;
		if (towerInstance != null)
			rangeRenderer.enabled = true;

		for (int i = 0; i < menuButtons.Length; i++)
		{
			if (menuButtons[i].GetComponent<TowerHolderButton>().buttonAction != ButtonAction.NONE)
				menuButtons[i].SetActive(true);
		}
	}

	public void HideButtons(bool hideRangeRenderer = true)
	{
		isMenuActive = false;
		if (hideRangeRenderer)
		{
			rangeRenderer.enabled = false;
		}

		for (int i = 0; i < menuButtons.Length; i++)
		{
			menuButtons[i].SetActive(false);
		}
	}

	private void ChangeState(MenuState newState)
	{
		menuState = newState;

		for (int i = 0; i < menuButtons.Length; i++)
		{
			menuButtons[i].GetComponent<TowerHolderButton>().buttonAction = ButtonAction.NONE;
			menuButtons[i].GetComponent<TowerHolderButton>().lineRenderer.enabled = false;
		}

		switch (newState)
		{
			case MenuState.Initial:
				menuButtons[(int)ButtonIndex.TOP_LEFT]
					.GetComponent<TowerHolderButton>()
					.SetAction(ButtonAction.BUILD_ARCHER);
				menuButtons[(int)ButtonIndex.TOP_RIGHT]
					.GetComponent<TowerHolderButton>()
					.SetAction(ButtonAction.BUILD_BARRACKS);
				menuButtons[(int)ButtonIndex.BOTTOM_LEFT]
					.GetComponent<TowerHolderButton>()
					.SetAction(ButtonAction.BUILD_MAGIC);
				menuButtons[(int)ButtonIndex.BOTTOM_RIGHT]
					.GetComponent<TowerHolderButton>()
					.SetAction(ButtonAction.BUILD_BOMB);
				break;

			case MenuState.UpgradeTowerBase:
				menuButtons[(int)ButtonIndex.TOP_CENTER]
					.GetComponent<TowerHolderButton>()
					.SetAction(ButtonAction.UPGRADE_LEVEL);
				menuButtons[(int)ButtonIndex.BOTTOM_CENTER]
					.GetComponent<TowerHolderButton>()
					.SetAction(ButtonAction.SELL);
				menuButtons[(int)ButtonIndex.CENTER_LEFT]
					.GetComponent<TowerHolderButton>()
					.SetAction(ButtonAction.CYCLE_RETARGET);
				break;

			case MenuState.UpgradeTowerFinal:
				menuButtons[(int)ButtonIndex.TOP_LEFT]
					.GetComponent<TowerHolderButton>()
					.SetAction(ButtonAction.BUY_EVOLUTION_1);
				menuButtons[(int)ButtonIndex.TOP_RIGHT]
					.GetComponent<TowerHolderButton>()
					.SetAction(ButtonAction.BUY_EVOLUTION_2);
				menuButtons[(int)ButtonIndex.BOTTOM_CENTER]
					.GetComponent<TowerHolderButton>()
					.SetAction(ButtonAction.SELL);
				menuButtons[(int)ButtonIndex.CENTER_LEFT]
					.GetComponent<TowerHolderButton>()
					.SetAction(ButtonAction.CYCLE_RETARGET);
				break;

			case MenuState.EvolutionTower:
				menuButtons[(int)ButtonIndex.TOP_CENTER]
					.GetComponent<TowerHolderButton>()
					.SetAction(ButtonAction.UPGRADE_EVOLUTION);
				menuButtons[(int)ButtonIndex.BOTTOM_CENTER]
					.GetComponent<TowerHolderButton>()
					.SetAction(ButtonAction.SELL);
				menuButtons[(int)ButtonIndex.CENTER_LEFT]
					.GetComponent<TowerHolderButton>()
					.SetAction(ButtonAction.CYCLE_RETARGET);
				break;

			default:
				break;
		}
	}

	private IEnumerator BuyTower(GameObject towerPrefab)
	{
		if (!PlayerStatsManager.SubtractGold(towerPrefab.GetComponent<BaseTower>().towerData.levels[0].price))
		{
			Debug.Log("Not enough gold");
			HideButtons();
			yield break;
		}

		isMenuLocked = true;
		HideButtons();
		ChangeState(MenuState.UpgradeTowerBase);

		backgroundSprite.enabled = false;

		animator.SetTrigger("BuildStart");

		prefabToBuild = towerPrefab;
	}

	private void BuyTowerAnimationCompletion()
	{
		towerInstance = Instantiate(prefabToBuild, transform.position, Quaternion.identity, transform);

		BaseTower baseTowerScript = towerInstance.GetComponent<BaseTower>();
		TowerHelpers.SetRangeCircle(
			rangeRenderer,
			baseTowerScript.towerData.levels[baseTowerScript.level].range,
			transform.position
		);

		if (towerInstance.GetComponent<BaseTower>().towerType == TowerTypes.Barracks)
		{
			menuButtons[(int)ButtonIndex.CENTER_LEFT]
				.GetComponent<TowerHolderButton>()
				.SetAction(ButtonAction.REPOSITION_BARRACKS);
		}

		isMenuLocked = false;
	}

	private void SellTower()
	{
		if (towerInstance == null)
			return;

		if (isEvolutionTower())
		{
			BaseEvolutionTower evolutionTower = GetBaseEvolutionTowerScript();
			PlayerStatsManager.AddGold(evolutionTower.CalculateSellPrice());
		}
		else if (towerInstance.GetComponent<BaseTower>() != null)
		{
			BaseTower baseTower = towerInstance.GetComponent<BaseTower>();
			PlayerStatsManager.AddGold(baseTower.CalculateSellPrice());
		}

		Destroy(towerInstance);

		HideButtons();
		ChangeState(MenuState.Initial);
		TowerHelpers.SetRangeCircle(rangeRenderer, 0, transform.position);
		towerInstance = null;
		rangeRenderer.enabled = false;
		backgroundSprite.enabled = true;
	}

	private void UpgradeTower()
	{
		if (towerInstance == null)
			return;

		BaseTower baseTower = towerInstance.GetComponent<BaseTower>();
		if (!PlayerStatsManager.SubtractGold(baseTower.towerData.levels[baseTower.level + 1].price))
		{
			Debug.Log("Not enough gold");
			HideButtons();
			return;
		}

		baseTower.UpgradeTower();
		TowerHelpers.SetRangeCircle(
			rangeRenderer,
			baseTower.towerData.levels[baseTower.level].range,
			transform.position
		);

		HideButtons();
		if (baseTower.level == 2)
			ChangeState(MenuState.UpgradeTowerFinal);
	}

	void BuildEvolutionTower(int evolutionIndex)
	{
		if (
			!PlayerStatsManager.SubtractGold(
				towerInstance.GetComponent<BaseTower>().towerData.evolutions[evolutionIndex].price
			)
		)
		{
			Debug.Log("Not enough gold");
			HideButtons();
			return;
		}

		GameObject evolutionPrefab = towerInstance
			.GetComponent<BaseTower>()
			.towerData.evolutions[evolutionIndex]
			.prefab;

		Destroy(towerInstance);
		towerInstance = Instantiate(evolutionPrefab, transform.position, Quaternion.identity, transform);
		towerInstance.GetComponent<BaseEvolutionTower>().evolutionIndex = evolutionIndex;

		TowerHelpers.SetRangeCircle(
			rangeRenderer,
			towerInstance.GetComponent<BaseTower>().towerData.evolutions[evolutionIndex].range,
			transform.position
		);

		HideButtons();
		ChangeState(MenuState.EvolutionTower);
	}

	void UpgradeEvolutionSkillLevel()
	{
		BaseEvolutionTower evolutionTower = GetBaseEvolutionTowerScript();

		if (
			evolutionTower.skillLevel + 1
			>= evolutionTower.towerData.evolutions[evolutionTower.evolutionIndex].skillLevels.Length
		)
			return;

		evolutionTower.UpgradeSkill();
		HideButtons();
	}

	private void RepositionBarracks()
	{
		HideButtons(false);
		StartCoroutine(WaitForMouseReleaseAndReposition());
	}

	private IEnumerator WaitForMouseReleaseAndReposition()
	{
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

		Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		towerInstance.GetComponent<Barracks>().SetTroopRandezvousPoint(mousePosition);
		HideButtons();
	}

	bool IsMouseOverAllButtons()
	{
		if (isMouseOver)
			return true;

		for (int i = 0; i < menuButtons.Length; i++)
		{
			if (menuButtons[i].GetComponent<TowerHolderButton>().IsMouseOver())
			{
				return true;
			}
		}

		return false;
	}

	public bool isEvolutionTower()
	{
		return towerInstance != null && towerInstance.GetComponent<BaseEvolutionTower>() != null;
	}
}
