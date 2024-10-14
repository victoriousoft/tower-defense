using UnityEngine;

public class Barracks : MonoBehaviour
{
    public float range = 4;
    public int troopCount = 3;
    public float respawnCooldown = 5; // seconds
    public Transform troopRandezvousPoint;
    public GameObject troopPrefab;

    private GameObject[] troops;

    void Awake()
    {
        troops = new GameObject[troopCount];
        SpawnTroops(troopCount);
    }

    void SpawnTroops(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (troops[i] != null) continue;
            GameObject troop = Instantiate(troopPrefab, transform.position, Quaternion.identity);
            troop.GetComponent<BaseTroop>().targetLocation = troopRandezvousPoint;
            troop.GetComponent<BaseTroop>().homeBase = gameObject;
            troops[i] = troop;
        }
    }

    public Transform RequestTroopRandezvousPoint()
    {
        return troopRandezvousPoint;
    }
}
