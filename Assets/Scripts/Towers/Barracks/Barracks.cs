using System.Collections;
using UnityEngine;

public class Barracks : BaseTower
{
	public int troopCount = 3;
	public float respawnCooldown = 5;

	[System.NonSerialized]
	[HideInInspector]
	public Vector2 localTroopRandezvousPoint;

	public GameObject troopPrefab;
	public float randezvousOffset = 1;

	private GameObject[] troops;
	private Animator animator;

	// tohle se neimplementuje v barracks
	protected override void FixedUpdate() { }

	protected override void Start()
	{
		base.Start();

		troops = new GameObject[troopCount];
		paths = GameObject.Find("Paths");
		Vector2 globalTroopRandezvous = TowerHelpers.GetClosesPointOnPath(
			transform.position,
			paths,
			transform.position,
			towerData.levels[level].range
		);
		localTroopRandezvousPoint = globalTroopRandezvous - (Vector2)transform.position;
		animator = GetComponent<Animator>();

		SpawnTroops(troopCount);
	}

	protected override IEnumerator Shoot(GameObject enemy)
	{
		yield return null;
	}

	protected override void KillProjectile(GameObject projectile, GameObject enemy, Vector3 enemyPosition) { }

	protected override IEnumerator ChargeUp(GameObject enemy)
	{
		yield return null;
	}

	void SpawnTroops(int count)
	{
		for (int i = 0; i < count; i++)
		{
			SpawnTroop(i);
		}
	}

	void SpawnTroop(int id)
	{
		if (troops[id] != null)
			return;

		GameObject troop = Instantiate(troopPrefab, transform.position, Quaternion.identity, transform);
		troop.GetComponent<BaseTroop>().homeBase = gameObject;
		troop.GetComponent<BaseTroop>().targetLocation = RequestTroopRandezvousPoint(id);
		troop.GetComponent<BaseTroop>().id = id;
		troops[id] = troop;
		animator.SetTrigger("open");
	}

	private int GetAliveTroopCount()
	{
		int aliveTroops = 0;
		foreach (GameObject troop in troops)
		{
			if (troop != null)
				aliveTroops++;
		}
		return aliveTroops;
	}

	public Vector2 RequestTroopRandezvousPoint(int troopId)
	{
		Vector2 basePosition = (Vector2)transform.position + localTroopRandezvousPoint;
		int aliveTroops = GetAliveTroopCount();

		if (aliveTroops == 1)
		{
			return basePosition;
		}
		else if (aliveTroops == 2)
		{
			float offset = troopId == 0 ? -randezvousOffset : randezvousOffset;
			return basePosition + new Vector2(offset, 0);
		}
		else
		{
			float angle = 120 * troopId;
			float radians = angle * Mathf.Deg2Rad;
			return basePosition
				+ new Vector2(randezvousOffset * Mathf.Cos(radians), randezvousOffset * Mathf.Sin(radians));
		}
	}

	public void SetTroopRandezvousPoint(Vector2 point)
	{
		Vector2 direction = (point - (Vector2)transform.position).normalized;
		float distance = Mathf.Min(Vector2.Distance(point, transform.position), towerData.levels[level].range);

		Vector2 pointOnPath = TowerHelpers.GetClosesPointOnPath(
			(Vector2)transform.position + direction * distance,
			paths,
			transform.position,
			towerData.levels[level].range
		);

		localTroopRandezvousPoint = pointOnPath - (Vector2)transform.position;

		for (int i = 0; i < troopCount; i++)
		{
			if (troops[i] != null)
			{
				troops[i].GetComponent<BaseTroop>().ForceReposition(RequestTroopRandezvousPoint(i));
			}
		}
	}

	public void RequestTroopRevive(int troopId)
	{
		Destroy(troops[troopId]);
		troops[troopId] = null;

		StartCoroutine(RespawnTroop(troopId));
	}

	public GameObject FindFightingEnemy()
	{
		foreach (GameObject troop in troops)
		{
			if (troop != null && troop.GetComponent<BaseTroop>().currentEnemy != null)
			{
				return troop.GetComponent<BaseTroop>().currentEnemy;
			}
		}

		return null;
	}

	private IEnumerator RespawnTroop(int troopId)
	{
		yield return new WaitForSeconds(respawnCooldown);
		SpawnTroop(troopId);
	}
}
