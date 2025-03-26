using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseEvolutionTower : BaseTower
{
	[System.NonSerialized]
	[HideInInspector]
	public int skillLevel = -1;

	[HideInInspector]
	public int evolutionIndex = -1;

	private HealthBar healthBar;
	private Image circleImage;

	private bool isSkillCharged = false;
	private Coroutine skillCoroutine;

	protected override void Awake()
	{
		base.Awake();
		circleImage = GetComponentInChildren<Image>();
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
		StartCoroutine(BlinkyBlinky());
	}

	public void UpgradeSkill()
	{
		Debug.Log("Upgrading skill");
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

	private IEnumerator BlinkyBlinky()
	{
		float initAlpha = circleImage.color.a;
		while (isSkillCharged)
		{
			for (float alpha = initAlpha; alpha >= 0; alpha -= 0.025f)
			{
				circleImage.color = new Color(circleImage.color.r, circleImage.color.g, circleImage.color.b, alpha);
				yield return new WaitForSeconds(0.05f);
			}
			for (float alpha = 0; alpha <= initAlpha; alpha += 0.025f)
			{
				circleImage.color = new Color(circleImage.color.r, circleImage.color.g, circleImage.color.b, alpha);
				yield return new WaitForSeconds(0.05f);
			}
		}
		circleImage.color = new Color(circleImage.color.r, circleImage.color.g, circleImage.color.b, initAlpha);
	}
}
