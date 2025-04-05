using System.Collections;
using System.Linq;
using UnityEngine;

public abstract class BaseTroop : MonoBehaviour
{
	public TroopSheet troopData;

	[System.NonSerialized]
	[HideInInspector]
	public float health;

	[System.NonSerialized]
	[HideInInspector]
	public Vector2 targetLocation;

	[System.NonSerialized]
	[HideInInspector]
	public int id = -1;

	[System.NonSerialized]
	[HideInInspector]
	public GameObject homeBase = null;

	[System.NonSerialized]
	[HideInInspector]
	public GameObject currentEnemy;

	protected HealthBar healthBar;
	protected bool canAttack = true;
	protected bool ignoreEnemies = false;
	protected bool isFighting = false;
	private bool dead = false;

	public Animator animator;
	protected abstract void Attack();

	void Awake()
	{
		health = troopData.stats.maxHealth;
		healthBar = GetComponentInChildren<HealthBar>();
		animator = GetComponent<Animator>();
	}

	void Update()
	{
		if (health <= 0)
			return;

		if (canAttack && currentEnemy == null)
		{
			Heal(troopData.stats.maxHealth / 10);
			canAttack = false;
			StartCoroutine(ResetAttackCooldown());
		}

		if (currentEnemy == null && targetLocation != null)
		{
			WalkTo(targetLocation);
		}
		else if (currentEnemy != null)
		{
			float distanceToEnemy = Vector2.Distance(transform.position, currentEnemy.transform.position);
			if (distanceToEnemy > troopData.stats.attackRange)
			{
				animator.SetBool("idle", false);
				WalkTo(currentEnemy.transform.position);
			}
		}

		if (currentEnemy == null)
			isFighting = false;

		if (!ignoreEnemies)
		{
			if (
				currentEnemy == null
				|| currentEnemy.GetComponent<BaseEnemy>().currentTarget != gameObject
				|| currentEnemy.GetComponent<BaseEnemy>().health <= 0
				|| currentEnemy.GetComponent<BaseEnemy>().attacksTroops == false
			)
				FindNewEnemy();

			if (currentEnemy != null && currentEnemy.GetComponent<BaseEnemy>().currentTarget == gameObject)
				currentEnemy.GetComponent<BaseEnemy>().RequestTarget(gameObject);

			if (currentEnemy != null)
			{
				if (
					canAttack
					&& Vector2.Distance(transform.position, currentEnemy.transform.position)
						<= troopData.stats.attackRange
				)
				{
					animator.SetFloat("x", 0);
					float direction = currentEnemy.transform.position.x - transform.position.x;
					animator.SetFloat("x", direction);

					isFighting = true;
					animator.SetTrigger("fighting");
					animator.SetBool("idle", true);
					Attack();
				}
			}
			else
			{
				targetLocation = homeBase.GetComponent<Barracks>().RequestTroopRandezvousPoint(id);
			}
		}
		else
		{
			if (Vector2.Distance(transform.position, targetLocation) < 0.1f)
			{
				ignoreEnemies = false;
				targetLocation = homeBase.GetComponent<Barracks>().RequestTroopRandezvousPoint(id);
			}
		}

		if (currentEnemy == null && Vector2.Distance(transform.position, targetLocation) < 0.1f)
		{
			animator.SetBool("idle", true);
		}
		else if (currentEnemy == null)
		{
			animator.SetBool("idle", false);
		}
	}

	public void TakeDamage(float damage)
	{
		health -= damage;
		healthBar.SetHealth(health / troopData.stats.maxHealth);

		if (health <= 0 && dead == false)
		{
			dead = true;
			StartCoroutine(Die());
		}
	}

	public void Heal(float amount)
	{
		health += amount;
		health = Mathf.Min(health, troopData.stats.maxHealth);
		healthBar.SetHealth(health / troopData.stats.maxHealth);
	}

	public IEnumerator Die()
	{
		healthBar.gameObject.SetActive(false);
		float direction = 0;
		if (currentEnemy != null)
		{
			direction = currentEnemy.transform.position.x - transform.position.x;
			currentEnemy.GetComponent<BaseEnemy>().currentTarget = null;
		}

		animator.SetTrigger("die");
		if (direction > 0)
			GetComponentInChildren<SpriteRenderer>().flipX = true;

		homeBase.GetComponent<Barracks>().RequestTroopRevive(id);
		yield return null;
		yield return null;
		Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
	}

	public void WalkTo(Vector3 target)
	{
		Vector2 currentPosition = transform.position;
		Vector2 newPosition = Vector2.MoveTowards(currentPosition, target, troopData.stats.speed * Time.deltaTime);

		Vector2 movement = newPosition - currentPosition;

		animator.SetFloat("x", movement.x);

		transform.position = newPosition;
	}

	void FindNewEnemy()
	{
		GameObject[] enemiesInTowerRange = TowerHelpers.GetEnemiesInRange(
			homeBase.transform.position,
			homeBase.GetComponent<BaseTower>().towerData.levels[homeBase.GetComponent<BaseTower>().level].range,
			new EnemyTypes[] { EnemyTypes.GROUND }
		);

		GameObject[] enemiesInRange = enemiesInTowerRange
			.Where(enemy =>
				enemy.GetComponent<BaseEnemy>().currentTarget == null
				&& enemy.GetComponent<BaseEnemy>().attacksTroops
				&& troopData.enemyTypes.Contains(enemy.GetComponent<BaseEnemy>().enemyData.enemyType)
				&& enemy.GetComponent<BaseEnemy>().enemyData.stats.attacksTroops == true
			)
			.OrderBy(enemy => enemy.GetComponent<BaseEnemy>().currentTarget != null)
			.ThenBy(enemy => Vector3.Distance(transform.position, enemy.transform.position))
			.ToArray();

		if (enemiesInRange.Length > 0)
			SetEnemy(enemiesInRange[0]);
		else
		{
			isFighting = false;
			animator.SetBool("fighting", false);
			currentEnemy = null;
			targetLocation = homeBase.GetComponent<Barracks>().RequestTroopRandezvousPoint(id);

			if (!isFighting)
			{
				GameObject fightingEnemy = homeBase.GetComponent<Barracks>().FindFightingEnemy();
				if (fightingEnemy != null)
					SetEnemy(fightingEnemy);
			}
		}
	}

	private void SetEnemy(GameObject enemy)
	{
		if (!enemy.GetComponent<BaseEnemy>().attacksTroops)
			return;

		enemy.GetComponent<BaseEnemy>().isPaused = true;
		enemy.GetComponent<BaseEnemy>().RequestTarget(gameObject);
		targetLocation = enemy.GetComponent<BaseEnemy>().GetAttackLocation(troopData.stats.attackRange);
		currentEnemy = enemy;
	}

	public void ForceReposition()
	{
		ForceReposition(Vector2.zero);
	}

	public void ForceReposition(Vector2 position)
	{
		isFighting = false;
		ignoreEnemies = true;

		if (currentEnemy != null && currentEnemy.GetComponent<BaseEnemy>().currentTarget == gameObject)
		{
			currentEnemy.GetComponent<BaseEnemy>().currentTarget = null;
		}

		currentEnemy = null;

		if (position != Vector2.zero)
		{
			targetLocation = position;
		}
		else
		{
			targetLocation = homeBase.GetComponent<Barracks>().RequestTroopRandezvousPoint(id);
		}
	}

	protected IEnumerator ResetAttackCooldown()
	{
		yield return new WaitForSeconds(troopData.stats.attackCooldown);
		canAttack = true;
		isFighting = false;
		animator.SetBool("fighting", false);
	}
}
