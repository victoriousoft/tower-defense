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

	public bool isSkillCharged = false;
	private Coroutine skillCoroutine;
	private bool waitingForFirstWave = false;

	protected abstract IEnumerator Skill(GameObject enemy);

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

	private void Update()
	{
		if (waitingForFirstWave && WaveSheet.instance.currentWave >= 0)
		{
			waitingForFirstWave = false;
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
	}

	void SkillChargeupCallback()
	{
		isSkillCharged = true;
		skillCoroutine = null;
		StartCoroutine(BlinkyBlinky());
	}

	public void UpgradeSkill()
	{
		if (skillCoroutine != null)
			StopCoroutine(skillCoroutine);

		skillLevel++;

		if (WaveSheet.instance.currentWave >= 0)
		{
			healthBar.gameObject.SetActive(true);

			healthBar.SetHealth(0);
			isSkillCharged = false;

			skillCoroutine = StartCoroutine(
				healthBar.Animate(
					0,
					1,
					towerData.evolutions[evolutionIndex].skillLevels[skillLevel].cooldown,
					SkillChargeupCallback
				)
			);
		}
		else
		{
			waitingForFirstWave = true;
		}
	}

	public override IEnumerator ChargeShootAndResetCooldown()
	{
		towerAnimator.SetTrigger("charge");

		yield return null;

		while (towerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
		{
			yield return null;
		}

		while (!enemiesInRange())
		{
			yield return null;
		}

		towerAnimator.SetTrigger("attack");

		GameObject target = TowerHelpers.SelectEnemyToAttack(
			TowerHelpers.GetEnemiesInRange(
				transform.position,
				towerData.evolutions[evolutionIndex].range,
				towerData.evolutionEnemyTypes[evolutionIndex].enemies.ToArray()
			),
			targetType
		);

		yield return Shoot(target);

		towerAnimator.SetTrigger("idle");

		yield return new WaitForSeconds(towerData.evolutions[evolutionIndex].cooldown);
		StartCoroutine(ChargeShootAndResetCooldown());
	}

	public override int CalculateSellPrice()
	{
		int price = base.CalculateSellPrice();

		if (WaveSheet.instance.currentWave == -1)
		{
			return price + towerData.evolutions[evolutionIndex].price;
		}

		return price + towerData.evolutions[evolutionIndex].price;
	}

	public void UseSkill()
	{
		if (isSkillCharged)
		{
			isSkillCharged = false;
			healthBar.SetHealth(0);

			StartCoroutine(UseSkillSequence());
		}
	}

	private IEnumerator UseSkillSequence()
	{
		GameObject target = TowerHelpers.SelectEnemyToAttack(
			TowerHelpers.GetEnemiesInRange(transform.position, towerData.levels[level].range, towerData.enemyTypes),
			targetType
		);

		yield return StartCoroutine(Skill(target));

		skillCoroutine = StartCoroutine(
			healthBar.Animate(
				0,
				1,
				towerData.evolutions[evolutionIndex].skillLevels[skillLevel].cooldown,
				SkillChargeupCallback
			)
		);
	}

	private IEnumerator BlinkyBlinky()
	{
		float initAlpha = circleImage.color.a;
		float minAlpha = 0.3f;

		while (isSkillCharged)
		{
			float elapsedTime = 0f;
			while (isSkillCharged)
			{
				elapsedTime += Time.deltaTime;
				float alpha = Mathf.Lerp(initAlpha, minAlpha, Mathf.PingPong(elapsedTime, 1f));
				circleImage.color = new Color(circleImage.color.r, circleImage.color.g, circleImage.color.b, alpha);
				yield return null;
			}
		}
		circleImage.color = new Color(circleImage.color.r, circleImage.color.g, circleImage.color.b, initAlpha);
	}
}
