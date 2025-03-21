using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEvolutionTower : BaseTower
{
	[System.NonSerialized]
	[HideInInspector]
	public int skillLevel = -1;

	[HideInInspector]
	public int evolutionIndex = -1;

	private HealthBar healthBar;

	private bool isSkillCharged = false;
	private Coroutine skillCoroutine;

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();

		level = towerData.levels.Length - 1;
		healthBar = GetComponentInChildren<HealthBar>();
		healthBar.gameObject.SetActive(false);
	}

	void SkillChargeupCallback()
	{
		isSkillCharged = true;
		skillCoroutine = null;
	}

	public void UpgradeSkill()
	{
		if (skillCoroutine != null)
			StopCoroutine(skillCoroutine);
		healthBar.SetHealth(0);
		isSkillCharged = false;

		skillLevel++;

		if (skillLevel >= 0)
			healthBar.gameObject.SetActive(true);

		skillCoroutine = StartCoroutine(
			healthBar.Animate(
				0,
				1,
				towerData.evolutions[evolutionIndex].skillLevels[skillLevel].cooldown,
				SkillChargeupCallback
			)
		);
	}

	public override int CalculateSellPrice()
	{
		int price = base.CalculateSellPrice();

		Debug.Log("base price: " + price);
		Debug.Log("evo price: " + towerData.evolutions[evolutionIndex].price / 2);
		return price + towerData.evolutions[evolutionIndex].price / 2;
	}
}
