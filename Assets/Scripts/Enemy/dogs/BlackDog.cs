using UnityEngine;

public class BlackDog : BaseEnemy
{
	protected override void Attack() { }

	protected override void UseAbility()
	{
		Debug.Log("BlackDog uses ability");
	}
}
