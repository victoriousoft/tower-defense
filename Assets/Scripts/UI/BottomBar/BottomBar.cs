using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomBar : MonoBehaviour
{
	public static BottomBar instance;
	private static BaseEnemy currentEnemy;
	private static bool isOpen = false;

	public TMPro.TextMeshProUGUI nameText;
	public TMPro.TextMeshProUGUI healthText;
	public TMPro.TextMeshProUGUI damageText;
	public TMPro.TextMeshProUGUI livesText;
	public TMPro.TextMeshProUGUI descriptionText;

	public ProgressBarManager physicalImmunityBar;
	public ProgressBarManager magicImmunityBar;

	void Start()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}

		Hide();
	}

	void Update()
	{
		if (currentEnemy != null && isOpen)
			UpdateStats();
		else if (isOpen && currentEnemy == null)
		{
			Debug.Log("Enemy is null, hiding bottom bar.");
			Hide();
			currentEnemy = null;
			isOpen = false;
		}
	}

	public static bool EnsureInstance()
	{
		if (instance == null)
		{
			Debug.LogError("BottomBar instance is null. Make sure it is initialized in the scene.");

			return false;
		}

		return true;
	}

	public static void ShowEnemy(GameObject enemy)
	{
		if (!EnsureInstance())
			return;

		currentEnemy = enemy.GetComponent<BaseEnemy>();
		isOpen = true;

		instance.gameObject.transform.GetChild(0).gameObject.SetActive(true);
		UpdateStats();
	}

	private static void UpdateStats()
	{
		if (!EnsureInstance())
			return;

		instance.nameText.text = currentEnemy.enemyData.info.name;

		instance.healthText.text =
			"Health: "
			+ currentEnemy.health.ToString("F0")
			+ "/"
			+ currentEnemy.enemyData.stats.maxHealth.ToString("F0");
		instance.damageText.text = "Damage: " + currentEnemy.enemyData.stats.damage.ToString("F0");
		instance.livesText.text = "Player lives: " + currentEnemy.enemyData.stats.playerLives.ToString("F0");

		instance.descriptionText.text = currentEnemy.enemyData.info.description;

		instance.physicalImmunityBar.SetProgress(currentEnemy.currentPhysicalResistance);
		instance.magicImmunityBar.SetProgress(currentEnemy.currentMagicResistance);

		instance.gameObject.transform.GetChild(0).gameObject.SetActive(true);
	}

	public static void Hide()
	{
		if (!EnsureInstance())
			return;

		isOpen = false;
		currentEnemy = null;

		instance.gameObject.transform.GetChild(0).gameObject.SetActive(false);
	}
}
