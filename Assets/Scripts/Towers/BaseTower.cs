using System.Collections;
using UnityEngine;

public abstract class BaseTower : MonoBehaviour
{
	[HideInInspector]
	public int level = 0;
	public TowerSheetNeo towerData;
	private PlayerStatsManager playerStats;
	private Animator towerAnimator;
	public TowerHelpers.TowerTargetTypes targetType = TowerHelpers.TowerTargetTypes.CLOSEST_TO_FINISH;
	public TowerTypes towerType;
	protected bool canShoot = true;
	protected GameObject paths;
	private Coroutine shootCoroutine;

	protected abstract IEnumerator Shoot(GameObject enemy);
	protected abstract IEnumerator ChargeUp(GameObject enemy);
	protected abstract void KillProjectile(GameObject projectile, GameObject enemy, Vector3 enemyPosition);

	protected virtual void ExtendedAwake() { }

	void Awake()
	{
		playerStats = GameObject.Find("PlayerStats").GetComponent<PlayerStatsManager>();
		towerAnimator = GetComponent<Animator>();
		ExtendedAwake();
	}

	protected virtual void FixedUpdate()
	{
		if (!canShoot)
			return;

		GameObject[] enemies = TowerHelpers.GetEnemiesInRange(
			transform.position,
			towerData.levels[level].range,
			towerData.enemyTypes
		);
		if (enemies.Length == 0)
			return;

		shootCoroutine = StartCoroutine(ShootAndResetCooldown());
	}

	private IEnumerator ShootAndResetCooldown()
	{
		canShoot = false;
		towerAnimator.SetTrigger("attack");

		while (towerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
		{
			yield return null;
		}

		GameObject[] enemies = TowerHelpers.GetEnemiesInRange(
			transform.position,
			towerData.levels[level].range,
			towerData.enemyTypes
		);
		if (enemies.Length == 0)
		{
			canShoot = true;
			Debug.Log("issue");
			yield break;
		}
		GameObject target = TowerHelpers.SelectEnemyToAttack(
			TowerHelpers.GetEnemiesInRange(transform.position, towerData.levels[level].range, towerData.enemyTypes),
			targetType
		);

		yield return Shoot(target);

		towerAnimator.SetTrigger("idle");

		yield return new WaitForSeconds(towerData.levels[level].cooldown);
		canShoot = true;
	}

	public void UpgradeTower()
	{
		StartCoroutine(UpgradeRoutine());
	}

	private IEnumerator UpgradeRoutine()
	{
		canShoot = false;

		if (shootCoroutine != null)
		{
			StopCoroutine(shootCoroutine);
			shootCoroutine = null;
		}

		level++;
		towerAnimator.SetTrigger("upgrade");

		yield return null;

		canShoot = true;
	}

	public void ChangeTargeting()
	{
		// Change targeting logic
	}
}
