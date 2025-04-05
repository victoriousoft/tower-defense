using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseEnemy : MonoBehaviour, IPointerClickHandler
{
	public EnemySheet enemyData;

	[System.NonSerialized]
	[HideInInspector]
	public float health;

	[System.NonSerialized]
	[HideInInspector]
	public bool isPaused = false;

	[System.NonSerialized]
	[HideInInspector]
	public GameObject currentTarget;

	private int currentPointIndex = 0;
	private Transform[] points;
	private Vector2 positionOffset;

	private readonly float[] resistanceValues = new float[] { 1, 0.5f, 0.35f, 0.2f, 0 };

	[HideInInspector]
	public int currentPhysicalResistance,
		currentMagicResistance;
	private bool nerfed = false;

	[HideInInspector]
	public bool isIdle = false,
		attacksTroops,
		abilityInUse = false;
	private HealthBar healthBar;

	protected bool canAttack = true;
	protected bool isAbilityCharged = false;

	[HideInInspector]
	public float currentSpeed,
		currentDamage;

	[HideInInspector]
	public Animator animator;

	private SpriteRenderer spriteRenderer;

	protected abstract void Attack();

	protected virtual void UseAbility() { }

	protected virtual IEnumerator ExtendedDeath()
	{
		yield return null;
	}

	void Awake()
	{
		currentSpeed = enemyData.stats.speed / 10;
		health = enemyData.stats.maxHealth;
		currentDamage = enemyData.stats.damage;
		attacksTroops = enemyData.stats.attacksTroops;
		healthBar = GetComponentInChildren<HealthBar>();
		positionOffset = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.25f, 0.25f));
		if (enemyData.enemyType == EnemyTypes.FLYING)
			positionOffset.y += 1f;
		currentPhysicalResistance = enemyData.stats.physicalResistance;
		currentMagicResistance = enemyData.stats.magicResistance;
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		animator = GetComponent<Animator>();

		StartCoroutine(ResetAbilityCooldown());
	}

	void Update()
	{
		if (health <= 0)
			return;

		if (currentTarget == null || !attacksTroops)
		{
			currentTarget = FindEnemyInRange();

			if (health > 0)
			{
				Move();
				isIdle = false;
				animator.SetBool("idle", false);
				if (isAbilityCharged && enemyData.stats.hasAbility)
				{
					animator.Play("ability");
					UseAbility();
					isAbilityCharged = false;
					StartCoroutine(ResetAbilityCooldown());
					abilityInUse = true;
				}
			}
			else if (!isIdle)
			{
				animator.SetFloat("y", 0);
				animator.SetFloat("x", currentTarget.transform.position.x - transform.position.x);
				animator.SetBool("idle", true);
				isIdle = true;
			}
		}
		else if (canAttack)
		{
			AttackAnimation();
			Attack();
			isIdle = true;
		}
	}

	private void AttackAnimation()
	{
		animator.SetFloat("y", 0);
		if (
			currentTarget == null
			|| (
				enemyData.enemyType == EnemyTypes.GROUND
				&& Vector3.Distance(transform.position, currentTarget.transform.position) > enemyData.stats.attackRange
			)
			|| (
				enemyData.enemyType == EnemyTypes.FLYING
				&& Vector3.Distance(
					new Vector3(transform.position.x, transform.position.y - 1, transform.position.z),
					currentTarget.transform.position
				) > enemyData.stats.attackRange
			)
		)
		{
			if (!isIdle)
			{
				if (transform.position.x > currentTarget.transform.position.x)
					GetComponentInChildren<SpriteRenderer>().flipX = true;
				animator.SetBool("idle", true);
				animator.SetBool("stop", true);
				isIdle = true;
			}
			return;
		}
		else
			animator.SetBool("stop", false);

		animator.SetBool("idle", true);
		animator.SetTrigger("attack");
		if (transform.position.x > currentTarget.transform.position.x)
			GetComponentInChildren<SpriteRenderer>().flipX = true;
		if (currentTarget != null)
		{
			currentTarget.GetComponent<BaseTroop>().TakeDamage(currentDamage);
		}
		canAttack = false;
		StartCoroutine(ResetAttackCooldown());
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		BottomBar.ShowEnemy(gameObject);
	}

	GameObject FindEnemyInRange()
	{
		Collider2D[] colliders;
		if (enemyData.enemyType == EnemyTypes.GROUND)
			colliders = Physics2D.OverlapCircleAll(transform.position, enemyData.stats.attackRange);
		else
		{
			colliders = Physics2D.OverlapCircleAll(
				new Vector3(transform.position.x, transform.position.y - 1, transform.position.z),
				enemyData.stats.attackRange
			);
		}
		foreach (Collider2D collider in colliders)
		{
			if (collider.gameObject.CompareTag("Troop"))
				return collider.gameObject;
		}
		return null;
	}

	public void SetPathParent(Transform pathParent)
	{
		points = new Transform[pathParent.childCount];
		for (int i = 0; i < pathParent.childCount; i++)
		{
			points[i] = pathParent.GetChild(i);
		}

		transform.position = (Vector2)points[currentPointIndex].position + positionOffset;
	}

	public void SetPathParent(Transform[] newPoints)
	{
		points = newPoints;
	}

	void Move()
	{
		if (points == null || abilityInUse)
			return;
		if (currentTarget == null)
			GetComponentInChildren<SpriteRenderer>().flipX = false;
		Vector2 targetPos = (Vector2)points[currentPointIndex].position + positionOffset;
		Vector2 moveDirection = (targetPos - (Vector2)transform.position).normalized;

		if (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.y))
		{
			animator.SetFloat("y", 0);
			animator.SetFloat("x", moveDirection.x);
		}
		else
		{
			animator.SetFloat("x", 0);
			animator.SetFloat("y", moveDirection.y);
		}

		if (Vector2.Distance(transform.position, (Vector2)points[currentPointIndex].position + positionOffset) < 0.1f)
		{
			currentPointIndex++;
			if (currentPointIndex >= points.Length)
			{
				Destroy(gameObject);
				return;
			}
		}

		transform.position = Vector2.MoveTowards(
			transform.position,
			(Vector2)points[currentPointIndex].position + positionOffset,
			currentSpeed * Time.fixedDeltaTime
		);
	}

	public float GetDistanceToFinish()
	{
		float distance = Vector2.Distance(
			transform.position,
			(Vector2)points[currentPointIndex].position + positionOffset
		);

		for (int i = currentPointIndex; i < points.Length - 1; i++)
		{
			distance += Vector2.Distance(
				(Vector2)points[i].position + positionOffset,
				(Vector2)points[i + 1].position + positionOffset
			);
		}
		return distance;
	}

	public float GetDistanceToStart()
	{
		float distance = Vector2.Distance(
			transform.position,
			(Vector2)points[currentPointIndex].position + positionOffset
		);
		for (int i = currentPointIndex; i > 0; i--)
		{
			distance += Vector2.Distance(
				(Vector2)points[i].position + positionOffset,
				(Vector2)points[i - 1].position + positionOffset
			);
		}

		return distance;
	}

	public void TakeDamage(float damage, DamageTypes damageType)
	{
		health -=
			(damageType == DamageTypes.PHYSICAL) ? damage * resistanceValues[currentPhysicalResistance]
			: (damageType == DamageTypes.MAGIC) ? damage * resistanceValues[currentMagicResistance]
			: (damageType == DamageTypes.EXPLOSION) ? damage * (resistanceValues[currentPhysicalResistance] / 2)
			: damage;

		healthBar.SetHealth(health / enemyData.stats.maxHealth);

		if (health <= 0)
		{
			StartCoroutine(Death());
		}
	}

	public IEnumerator Death()
	{
		if (!nerfed)
			PlayerStatsManager.AddGold(enemyData.stats.cashDrop);
		else
			PlayerStatsManager.AddGold(enemyData.stats.cashDrop * 3);

		healthBar.gameObject.SetActive(false);

		if (animator.GetFloat("x") != 0)
			spriteRenderer.flipX = animator.GetFloat("x") < 0;
		animator.SetBool("death", true);
		yield return null;

		StartCoroutine(ExtendedDeath());

		Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
	}

	public Vector2 GetAttackLocation(float troopAttackRange)
	{
		Vector2 direction = (Vector2)points[currentPointIndex].position + positionOffset - (Vector2)transform.position;

		float minAttackRange = Mathf.Min(troopAttackRange - 0.1f, enemyData.stats.attackRange - 0.1f);
		return (Vector2)transform.position + direction.normalized * minAttackRange;
	}

	public void Heal(float amount)
	{
		health = Mathf.Min(health += amount, enemyData.stats.maxHealth);
		healthBar.SetHealth(health / enemyData.stats.maxHealth);
	}

	public void RequestTarget(GameObject target)
	{
		if (currentTarget == null)
			currentTarget = target;
	}

	public void Slowdown(float slowdownFactor, float duration)
	{
		if (!(enemyData.stats.speed / slowdownFactor >= currentSpeed))
		{
			StopCoroutine(GetSlowedDown(slowdownFactor, duration));
			StartCoroutine(GetSlowedDown(slowdownFactor, duration));
		}
	}

	public void NerfResistance(int resistanceType, int armorSubtract, float duration)
	{
		StartCoroutine(GetNerfed(resistanceType, armorSubtract, duration));
	}

	private IEnumerator GetSlowedDown(float slowdownFactor, float duration)
	{
		float originalSpeed = currentSpeed;
		if (slowdownFactor < 100)
			spriteRenderer.color = new Color(0.68f, 0.85f, 0.9f);
		else
			spriteRenderer.color = Color.blue;
		currentSpeed /= slowdownFactor;
		yield return new WaitForSeconds(duration);
		currentSpeed = originalSpeed;
		spriteRenderer.color = Color.white;
	}

	private IEnumerator GetNerfed(int resistanceType, int armorSubtract, float duration)
	{
		nerfed = true;
		spriteRenderer.color = Color.yellow;
		int originalResistance;
		if (resistanceType == 0)
		{
			originalResistance = currentPhysicalResistance;
			currentPhysicalResistance = Mathf.Max(0, currentPhysicalResistance - armorSubtract);
		}
		else
		{
			originalResistance = currentMagicResistance;
			currentMagicResistance = Mathf.Max(0, currentMagicResistance - armorSubtract);
		}

		yield return new WaitForSeconds(duration);

		if (resistanceType == 0)
		{
			currentPhysicalResistance = originalResistance;
		}
		else
		{
			currentMagicResistance = originalResistance;
		}
		if (spriteRenderer.color == Color.yellow)
			spriteRenderer.color = Color.white;
		nerfed = false;
	}

	public void RageForExit()
	{
		canAttack = false;
		attacksTroops = false;
		currentTarget = null;
		currentSpeed *= 2f;
		animator.SetTrigger("rage");
		animator.ResetTrigger("attack");
		animator.SetBool("idle", false);
		animator.SetBool("stop", false);
	}

	protected IEnumerator SpawnChild(
		GameObject childPrefab,
		float xSpawnOffset,
		System.Action<GameObject> onChildSpawned
	)
	{
		GameObject child = Instantiate(
			childPrefab,
			transform.position /*+ new Vector3(xSpawnOffset, 0, 0)*/
			,
			Quaternion.identity
		);
		child.transform.SetParent(gameObject.transform.parent.gameObject.transform);
		yield return new WaitForSeconds(0.2f);
		for (float j = 0; Mathf.Abs(j) < Mathf.Abs(xSpawnOffset); j += 0.1f * Mathf.Sign(xSpawnOffset))
		{
			child.transform.position = new Vector2(
				child.transform.position.x + 0.1f * Mathf.Sign(xSpawnOffset),
				child.transform.position.y
			);
		}
		child.GetComponent<BaseEnemy>().currentPointIndex = currentPointIndex;
		child.GetComponent<BaseEnemy>().SetPathParent(points);
		onChildSpawned?.Invoke(child);
	}

	protected IEnumerator ResetAttackCooldown()
	{
		yield return new WaitForSeconds(enemyData.stats.attackCooldown);
		canAttack = true;
	}

	protected IEnumerator ResetAbilityCooldown()
	{
		yield return new WaitForSeconds(enemyData.stats.abilityCooldown);
		isAbilityCharged = true;
	}
}
