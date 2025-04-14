using UnityEngine;

public class ExitManager : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D col)
	{
		GameObject enemy = col.gameObject;
		if (enemy.CompareTag("Enemy"))
		{
			PlayerStatsManager.SubtractLives(enemy.GetComponent<BaseEnemy>().enemyData.stats.playerLives);
			TowerDefenseAgent.instance.OnEnemyPass(enemy.GetComponent<BaseEnemy>().enemyData.stats.playerLives);
			Destroy(enemy);
		}
	}
}
