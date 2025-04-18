using System.Collections;
using UnityEngine;

public class Barracks : BaseTower
{
	public int troopCount = 3;
	public float respawnCooldown = 5;

	[System.NonSerialized]
	[HideInInspector]
	public Vector2 localTroopRandezvousPoint;

	public GameObject[] troopPrefabs = new GameObject[3];
	public float randezvousOffset = 1;

	private GameObject[] troops;
	private Animator animator;

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

	protected override void ExtendedUpgrade()
	{
		for (int i = 0; i < troopCount; i++)
		{
			if (troops[i] != null)
			{
				Destroy(troops[i]);
				troops[i] = null;
			}
		}

		SpawnTroops(troopCount);
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

		GameObject troop = Instantiate(troopPrefabs[level], transform.position, Quaternion.identity, transform);
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
		float range = towerData.levels[level].range;
		Vector2 direction = (point - (Vector2)transform.position);
		float distance = Vector2.Distance(point, transform.position);

		if (distance > range)
		{
			point = (Vector2)transform.position + direction.normalized * range;
		}

		Vector2 pointOnPath = TowerHelpers.GetClosesPointOnPath(point, paths, transform.position, range);

		Vector2 offsetDirection = (point - pointOnPath).normalized;
		float maxOffset = 0.5f;
		float offsetDistance = Mathf.Min((point - pointOnPath).magnitude, maxOffset);
		Vector2 finalPoint = pointOnPath + (offsetDirection * offsetDistance);

		localTroopRandezvousPoint = finalPoint - (Vector2)transform.position;

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

	public bool IsInRange(GameObject obj)
	{
		float distanceToTower = Vector2.Distance(obj.transform.position, transform.position);
		float range = towerData.levels[level].range - 0.2f;

		if (distanceToTower > range)
			return false;

		return true;
	}

	private IEnumerator RespawnTroop(int troopId)
	{
		yield return new WaitForSeconds(respawnCooldown);
		SpawnTroop(troopId);
	}
}
