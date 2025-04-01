using System.Collections;
using UnityEngine;

public abstract class BaseTower : MonoBehaviour
{
	[System.NonSerialized]
	[HideInInspector]
	public int level = 0;

	public TowerSheetNeo towerData;

	[HideInInspector]
	public Animator towerAnimator;

	[System.NonSerialized]
	[HideInInspector]
	public TowerHelpers.TowerTargetTypes targetType = TowerHelpers.TowerTargetTypes.CLOSEST_TO_FINISH;

	public TowerTypes towerType;
	protected GameObject paths;
	private Coroutine shootCoroutine;
	private float currentDamage;

	protected abstract IEnumerator Shoot(GameObject enemy);
	protected abstract IEnumerator ChargeUp(GameObject enemy);
	protected abstract void KillProjectile(GameObject projectile, GameObject enemy, Vector3 enemyPosition);

	protected virtual void Awake()
	{
		towerAnimator = GetComponent<Animator>();
	}

	protected virtual void Start()
	{
		shootCoroutine = StartCoroutine(ChargeShootAndResetCooldown());
		if (GetComponent<LineRenderer>() != null)
			ResetLaserPosition();
		currentDamage = towerData.levels[level].damage;
	}

	protected virtual void FixedUpdate() { } //mozna zbytecny

	public virtual IEnumerator ChargeShootAndResetCooldown()
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
			//charged up, waiting for enemies
		}

		towerAnimator.SetTrigger("attack");

		GameObject target = TowerHelpers.SelectEnemyToAttack(
			TowerHelpers.GetEnemiesInRange(transform.position, towerData.levels[level].range, towerData.enemyTypes),
			targetType
		);

		yield return Shoot(target);

		towerAnimator.SetTrigger("idle");

		yield return new WaitForSeconds(towerData.levels[level].cooldown);
		StartCoroutine(ChargeShootAndResetCooldown());
	}

	protected bool enemiesInRange()
	{
		EnemyTypes[] targetTypes = towerData.enemyTypes;
		float range = towerData.levels[level].range;

		if (GetComponent<BaseEvolutionTower>() != null)
		{
			targetTypes = towerData
				.evolutionEnemyTypes[GetComponent<BaseEvolutionTower>().evolutionIndex]
				.enemies.ToArray();

			range = towerData.evolutions[GetComponent<BaseEvolutionTower>().evolutionIndex].range;
		}
		GameObject[] enemies = TowerHelpers.GetEnemiesInRange(transform.position, range, targetTypes);

		if (enemies.Length == 0)
		{
			return false;
		}
		return true;
	}

	public void UpgradeTower()
	{
		StartCoroutine(UpgradeRoutine());
	}

	private IEnumerator UpgradeRoutine()
	{
		if (GetComponent<LineRenderer>() != null)
			ResetLaserPosition();

		level++;
		currentDamage = towerData.levels[level].damage;
		GetComponentInChildren<SpriteRenderer>().color = Color.white;

		towerAnimator.SetTrigger("upgrade");

		yield return new WaitForSeconds(0.5f);

		if (shootCoroutine == null)
			shootCoroutine = StartCoroutine(ChargeShootAndResetCooldown());
	}

	public IEnumerator EnhanceTemporarily(float factor, float duration)
	{
		currentDamage *= factor;
		SpriteRenderer[] sr = GetComponentsInChildren<SpriteRenderer>();
		yield return new WaitForSeconds(duration);
		currentDamage = towerData.levels[level].damage;
	}

	private void ResetLaserPosition()
	{
		return;
		// Tohle asi neni potreba, ale pro jistotu to necham tady, u hackermana to tweakuje
		GetComponent<LineRenderer>()
			.SetPosition(0, transform.Find("shotOrigin").position);
		GetComponent<LineRenderer>().SetPosition(1, transform.Find("shotOrigin").position);
	}

	public virtual int CalculateSellPrice()
	{
		int price = 0;
		for (int i = 0; i < level + 1; i++)
		{
			price += towerData.levels[i].price;
		}
		return price / 2;
	}
}
