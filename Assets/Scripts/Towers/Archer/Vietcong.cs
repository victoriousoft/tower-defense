using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vietcong : BaseEvolutionTower
{
	private Animator flipAnimator;
	private Transform currentOrigin;
	public GameObject spikesPrefab,
		crosshiarIcon;

	[SerializeField]
	private SpriteRenderer spriteRendererLeft,
		spriteRendererRight;

	private bool facingLeft = true;

	protected override void Start()
	{
		base.Start();
		flipAnimator = GetComponent<Animator>();
		spriteRendererLeft.gameObject.SetActive(true);
		spriteRendererRight.gameObject.SetActive(false);
	}

	protected override IEnumerator Shoot(GameObject enemy)
	{
		if (enemy != null)
		{
			GameObject icon = Instantiate(
				crosshiarIcon,
				enemy.transform.position,
				Quaternion.identity,
				enemy.transform
			);
			icon.GetComponent<SelfDestruct>().DestroySelf(0.75f);

			if (facingLeft && enemy.transform.position.x > transform.position.x)
			{
				spriteRendererLeft.gameObject.SetActive(false);
				spriteRendererRight.gameObject.SetActive(true);
				facingLeft = false;
			}
			else if (!facingLeft && enemy.transform.position.x < transform.position.x)
			{
				spriteRendererLeft.gameObject.SetActive(true);
				spriteRendererRight.gameObject.SetActive(false);
				facingLeft = true;
			}
		}
		yield return new WaitForSeconds(0.5f);
		enemy.GetComponent<BaseEnemy>().TakeDamage(towerData.evolutions[1].damage, DamageTypes.PHYSICAL);
		yield return null;
	}

	protected override IEnumerator ChargeUp(GameObject enemy)
	{
		yield return null;
	}

	protected override IEnumerator Skill(GameObject enemy)
	{
		//nesmi byt flying
		GameObject targetEnemy = TowerHelpers.SelectEnemyToAttack(
			TowerHelpers.GetEnemiesInRange(
				transform.position,
				towerData.evolutions[evolutionIndex].range,
				new EnemyTypes[] { EnemyTypes.GROUND }
			),
			targetType
		);

		//pokud closest to finish tak closest to finish, ale pokud cokoli jinyho tak most hp (check jestli nema imunitu)
		if (targetType != TowerHelpers.TowerTargetTypes.CLOSEST_TO_FINISH)
		{
			GameObject[] enemies = TowerHelpers.GetEnemiesInRange(
				transform.position,
				towerData.levels[level].range,
				towerData.enemyTypes
			);
			GameObject target = TowerHelpers.SelectEnemyToAttack(
				TowerHelpers.GetEnemiesInRange(
					transform.position,
					towerData.evolutions[evolutionIndex].range,
					towerData.enemyTypes
				),
				TowerHelpers.TowerTargetTypes.MOST_HP
			);
			if (target.GetComponent<BaseEnemy>().currentPhysicalResistance != 4)
				targetEnemy = target;
		}
		GameObject spikes = Instantiate(
			spikesPrefab,
			new Vector2(targetEnemy.transform.position.x, enemy.transform.position.y - 0.1f),
			Quaternion.identity
		);
		spikes.GetComponent<SelfDestruct>().DestroySelf(1);
		yield return new WaitForSeconds(0.225f);
		targetEnemy.GetComponent<BaseEnemy>().TakeDamage(1000000, DamageTypes.PHYSICAL);
		yield return null;
	}

	protected override void KillProjectile(GameObject projectile, GameObject enemy, Vector3 enemyPosition) { }
}
