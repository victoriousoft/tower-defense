using UnityEngine;

public class ExitManager : MonoBehaviour
{
    private PlayerStatsManager playerStats;
    void Awake()
    {
        playerStats = GameObject.Find("PlayerStats").GetComponent<PlayerStatsManager>();
    }
    void Update() { }
    void OnTriggerEnter2D(Collider2D col)
    {
        GameObject enemy = col.gameObject;
        if (enemy.CompareTag("Enemy"))
        {
            playerStats.SubtractLives(enemy.GetComponent<Health>().hpSub);
            Destroy(enemy);
        }
    }
}
