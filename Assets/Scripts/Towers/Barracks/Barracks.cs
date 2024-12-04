using System.Collections;
using UnityEngine;

public class Barracks : BaseTower
{
    public int troopCount = 3;
    public float respawnCooldown = 5; // seconds
    public Vector3 localTroopRandezvousPoint;
    public GameObject troopPrefab;
    public float randezvousOffset = 1;

    private GameObject[] troops;

    void Awake()
    {
        troops = new GameObject[troopCount];
        SpawnTroops(troopCount);
    }

    // tohle se neimplementuje v barracks
    protected override void FixedUpdate() { }
    protected override IEnumerator AnimateProjectile(GameObject enemy) { yield return null; }
    protected override void KillProjectile(GameObject projectile, GameObject enemy, Vector3 enemyPosition) { }
    override protected IEnumerator AnimateLaser(GameObject enemy){ yield return null;}
    
    override protected IEnumerator KillLaser(LineRenderer laserRenderer,Transform origin, GameObject enemy, Vector3 enemyPosition){ yield return null;}

    void SpawnTroops(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnTroop(i);
        }
    }

    void SpawnTroop(int id)
    {
        if (troops[id] != null) return;

        GameObject troop = Instantiate(troopPrefab, transform.position, Quaternion.identity, transform);
        troop.GetComponent<BaseTroop>().homeBase = gameObject;
        troop.GetComponent<BaseTroop>().targetLocation = RequestTroopRandezvousPoint(id);
        troop.GetComponent<BaseTroop>().id = id;
        troops[id] = troop;
    }

    private int GetAliveTroopCount()
    {
        int aliveTroops = 0;
        foreach (GameObject troop in troops)
        {
            if (troop != null) aliveTroops++;
        }
        return aliveTroops;
    }

    public Vector3 RequestTroopRandezvousPoint(int troopId)
    {
        Vector3 position = transform.position + localTroopRandezvousPoint;
        float angle = 120 * troopId;
        float radians = angle * Mathf.Deg2Rad;
        position.x += randezvousOffset * Mathf.Cos(radians);
        position.y += randezvousOffset * Mathf.Sin(radians);

        return position;
    }

    public void RequestTroopRevive(int troopId)
    {
        Destroy(troops[troopId]);
        troops[troopId] = null;

        StartCoroutine(RespawnTroop(troopId));
    }

    private IEnumerator RespawnTroop(int troopId)
    {
        yield return new WaitForSeconds(respawnCooldown);
        SpawnTroop(troopId);
    }
}
