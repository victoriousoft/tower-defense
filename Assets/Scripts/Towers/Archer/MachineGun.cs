using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : BaseEvolutionTower
{
	[SerializeField]
	private Animator spinAnimationAnimator;

	protected override IEnumerator Shoot(GameObject enemy)
	{
		yield return null;
	}

	protected override IEnumerator ChargeUp(GameObject enemy)
	{
		yield return null;
	}

	protected override void KillProjectile(GameObject projectile, GameObject enemy, Vector3 enemyPosition) { }
}
