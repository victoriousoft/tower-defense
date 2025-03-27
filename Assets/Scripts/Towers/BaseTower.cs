using System.Collections;
using UnityEngine;

public abstract class BaseTower : MonoBehaviour
{
	[System.NonSerialized]
	[HideInInspector]
	public int level = 0;

	public TowerSheetNeo towerData;
	private Animator towerAnimator;

	[System.NonSerialized]
	[HideInInspector]
	public TowerHelpers.TowerTargetTypes targetType = TowerHelpers.TowerTargetTypes.CLOSEST_TO_FINISH;

	public TowerTypes towerType;
	protected GameObject paths;
	private Coroutine shootCoroutine;

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

	[HideInInspector]
	public bool enemiesInRange()
	{
		GameObject[] enemies = TowerHelpers.GetEnemiesInRange(
			transform.position,
			towerData.levels[level].range,
			towerData.enemyTypes
		);

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
		StopCoroutine(shootCoroutine);
		shootCoroutine = null;

		if (GetComponent<LineRenderer>() != null)
			ResetLaserPosition();

		level++;

		towerAnimator.SetTrigger("upgrade");

		yield return new WaitForSeconds(0.5f);

		if (shootCoroutine == null)
			shootCoroutine = StartCoroutine(ChargeShootAndResetCooldown());
	}

	public IEnumerator EnhanceTemoprarily(int factor, float duration)
	{
		yield return null;
	}

	private void ResetLaserPosition()
	{
		GetComponent<LineRenderer>().SetPosition(0, transform.Find("shotOrigin").position);
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
