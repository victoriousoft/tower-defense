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
	TOP_LEFT = 0,
	TOP_CENTER = 1,
	TOP_RIGHT = 2,
	BOTTOM_LEFT = 3,
	BOTTOM_CENTER = 4,
	BOTTOM_RIGHT = 5,
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

	// top left to right, bottom left to right
	private GameObject[] menuButtons = new GameObject[6];

	private GameObject towerInstance;
	private LineRenderer rangeRenderer;
	private bool isMenuActive = false;
	private bool isMenuLocked = true;
	private Animator animator;

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
			{ ButtonAction.SELL, null },
			{ ButtonAction.UPGRADE_LEVEL, null },
			{ ButtonAction.BUY_EVOLUTION_1, null },
			{ ButtonAction.BUY_EVOLUTION_2, null },
			{ ButtonAction.UPGRADE_EVOLUTION_0, null },
			{ ButtonAction.UPGRADE_EVOLUTION_1, null },
			{ ButtonAction.UPGRADE_EVOLUTION_2, null },
		};

		animator = GetComponent<Animator>();

		rangeRenderer = gameObject.AddComponent<LineRenderer>();
		rangeRenderer.enabled = false;
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
			menuButtons[i].GetComponent<TowerHolderButton>().towerHolder = gameObject;
		}

		HideButtons();
		ChangeState(MenuState.Initial);
	}

	void OnMouseEnter()
	{
		// TODO: make cursor pointer
	}

	void OnMouseDown()
	{
		if (isMenuActive)
			HideButtons();
		else
			ShowButtons();
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
			case ButtonAction.SELL:
				SellTower();
				break;
			case ButtonAction.UPGRADE_LEVEL:
				UpgradeTower();
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

	private void ShowButtons()
	{
		if (!isMenuLocked)
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

	private void HideButtons()
	{
		isMenuActive = false;
		rangeRenderer.enabled = false;

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
				break;
			case MenuState.EvolutionTower:
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

		isMenuLocked = false;
		HideButtons();
		ChangeState(MenuState.UpgradeTowerBase);

		backgroundSprite.enabled = false;

		animator.SetTrigger("BuildStart");

		prefabToBuild = towerPrefab;
	}

	private void BuyTowerAnimationCompletion()
	{
		towerInstance = Instantiate(prefabToBuild, transform.position, Quaternion.identity);

		BaseTower baseTowerScript = towerInstance.GetComponent<BaseTower>();
		TowerHelpers.SetRangeCircle(
			rangeRenderer,
			baseTowerScript.towerData.levels[baseTowerScript.level].range,
			transform.position
		);

		isMenuLocked = true;
	}

	private void SellTower()
	{
		if (towerInstance == null)
			return;

		BaseTower baseTower = towerInstance.GetComponent<BaseTower>();
		PlayerStatsManager.AddGold(baseTower.towerData.levels[baseTower.level].price / 2);
		Destroy(towerInstance);

		HideButtons();
		ChangeState(MenuState.Initial);
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
}
