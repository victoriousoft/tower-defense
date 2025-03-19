using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TowerHelpers
{
	public enum TowerProjectileRotationTypes
	{
		NONE,
		LOOK_AT_TARGET,
		SPIN,
	};

	public enum TowerTargetTypes
	{
		CLOSEST_TO_FINISH,
		CLOSEST_TO_START,
		MOST_HP,
		LEAST_HP,
	};

	public static string GetString(this TowerTargetTypes targetType)
	{
		return targetType switch
		{
			TowerTargetTypes.CLOSEST_TO_FINISH => "Closest to Finish",
			TowerTargetTypes.CLOSEST_TO_START => "Closest to Start",
			TowerTargetTypes.MOST_HP => "Most HP",
			TowerTargetTypes.LEAST_HP => "Least HP",
			_ => "Unknown",
		};
	}

	public static TowerTargetTypes GetTargetTypeByIndex(int index)
	{
		if (index >= 0 && index < Enum.GetValues(typeof(TowerTargetTypes)).Length)
		{
			return (TowerTargetTypes)index;
		}
		return TowerTargetTypes.CLOSEST_TO_FINISH;
	}

	public static GameObject[] GetEnemiesInRange(Vector2 position, float range, EnemyTypes[] includedTypes)
	{
		Collider2D[] colliders = Physics2D.OverlapCircleAll(position, range);
		List<GameObject> enemies = new List<GameObject>();
		foreach (Collider2D collider in colliders)
		{
			if (
				collider.CompareTag("Enemy")
				&& includedTypes.Contains(collider.GetComponent<BaseEnemy>().enemyData.enemyType)
			)
				enemies.Add(collider.gameObject);
		}
		return enemies.ToArray();
	}

	public static GameObject SelectEnemyToAttack(GameObject[] enemies, TowerTargetTypes targetType)
	{
		GameObject target = null;

		switch (targetType)
		{
			case TowerTargetTypes.CLOSEST_TO_FINISH:
				target = enemies.OrderBy(e => e.GetComponent<BaseEnemy>().GetDistanceToFinish()).FirstOrDefault();
				break;
			case TowerTargetTypes.CLOSEST_TO_START:
				target = enemies.OrderBy(e => e.GetComponent<BaseEnemy>().GetDistanceToStart()).FirstOrDefault();
				break;
			case TowerTargetTypes.MOST_HP:
				target = enemies.OrderByDescending(e => e.GetComponent<BaseEnemy>().health).FirstOrDefault();
				break;
			case TowerTargetTypes.LEAST_HP:
				target = enemies.OrderBy(e => e.GetComponent<BaseEnemy>().health).FirstOrDefault();
				break;
			default:
				Debug.LogWarning("Invalid target type, fallback to closest to finish");
				target = enemies.OrderBy(e => e.GetComponent<BaseEnemy>().GetDistanceToFinish()).FirstOrDefault();
				break;
		}

		return target;
	}

	// moc nevim co tohle dělá
	public static Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2)
	{
		return Mathf.Pow(1 - t, 2) * p0 + 2 * (1 - t) * t * p1 + Mathf.Pow(t, 2) * p2;
	}

	// animuje bezier projectile (archers, bomb tower)
	public static IEnumerator AnimateBezierProjectile(
		GameObject projectile,
		Vector2 startPosition,
		GameObject target,
		float height,
		float duration,
		Action<GameObject, GameObject, Vector3> destroyCallback,
		TowerProjectileRotationTypes rotationType = TowerProjectileRotationTypes.NONE
	)
	{
		Vector2 targetPosition = target != null ? target.transform.position : Vector3.zero;
		Vector2 controlPoint = (startPosition + targetPosition) / 2 + Vector2.up * height;

		float startTime = Time.time;

		while (Time.time < startTime + duration)
		{
			if (target != null)
			{
				if (target.CompareTag("Enemy"))
					targetPosition = target.transform.position;
				else
					target = null;
			}

			controlPoint = (startPosition + targetPosition) / 2 + Vector2.up * height;

			float t = (Time.time - startTime) / duration;
			Vector2 currentPosition = CalculateBezierPoint(t, startPosition, controlPoint, targetPosition);
			projectile.transform.position = currentPosition;

			Vector2 direction = (
				CalculateBezierPoint(t + 0.01f, startPosition, controlPoint, targetPosition) - currentPosition
			).normalized;

			if (rotationType == TowerProjectileRotationTypes.LOOK_AT_TARGET)
			{
				projectile.transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0);
			}
			else if (rotationType == TowerProjectileRotationTypes.SPIN)
			{
				projectile.transform.Rotate(Vector3.up, 360 * Time.fixedDeltaTime / duration);
			}

			yield return null;
		}

		destroyCallback(projectile, target, target != null ? target.transform.position : Vector3.zero);
	}

	public static IEnumerator AnimateDirectProjectile(
		GameObject projectile,
		GameObject target,
		float speed,
		Action<GameObject, GameObject, Vector3> destroyCallback
	)
	{
		while (target != null && Vector3.Distance(projectile.transform.position, target.transform.position) > 0.1f)
		{
			Vector3 direction = (target.transform.position - projectile.transform.position).normalized;
			projectile.transform.position += speed * Time.fixedDeltaTime * direction;

			projectile.transform.LookAt(target.transform);

			yield return null;
		}

		destroyCallback(projectile, target, target != null ? target.transform.position : Vector3.zero);
	}

	public static IEnumerator AnimateLaser(
		LineRenderer laserRenderer,
		Transform origin,
		GameObject target,
		float duration,
		Action<GameObject, GameObject, Vector3> destroyCallback
	)
	{
		float iterationStep = 0.01f;
		float iterations = duration / iterationStep;
		laserRenderer.enabled = true;

		for (int i = 0; i < iterations; i++)
		{
			if (target != null)
			{
				laserRenderer.SetPosition(0, origin.position);
				laserRenderer.SetPosition(1, target.transform.position);
				destroyCallback(null, target, target != null ? target.transform.position : Vector3.zero);
			}
			else
			{
				laserRenderer.SetPosition(1, origin.position);
			}

			yield return new WaitForSeconds(iterationStep);
		}

		laserRenderer.enabled = false;
	}

	public static bool IsOnPath(Vector2 position, GameObject pathParents, float pathWidth)
	{
		GameObject[] paths = new GameObject[pathParents.transform.childCount];
		for (int i = 0; i < pathParents.transform.childCount; i++)
		{
			paths[i] = pathParents.transform.GetChild(i).gameObject;
		}

		for (int i = 0; i < paths.Length; i++)
		{
			GameObject[] pathPoints = new GameObject[paths[i].transform.childCount];
			for (int j = 0; j < paths[i].transform.childCount; j++)
			{
				pathPoints[j] = paths[i].transform.GetChild(j).gameObject;
			}

			GameObject[] closestPoints = pathPoints
				.OrderBy(p => Vector2.Distance(position, p.transform.position))
				.Take(2)
				.ToArray();

			float a = closestPoints[1].transform.position.y - closestPoints[0].transform.position.y;
			float b = closestPoints[0].transform.position.x - closestPoints[1].transform.position.x;
			float c = a * closestPoints[0].transform.position.x + b * closestPoints[0].transform.position.y;

			float distance = Mathf.Abs(a * position.x + b * position.y - c) / Mathf.Sqrt(a * a + b * b);

			if (distance < pathWidth / 2)
				return true;
		}

		return false;
	}

	public static Vector2 GetClosesPointOnPath(Vector2 origin, GameObject pathParents)
	{
		GameObject[] paths = new GameObject[pathParents.transform.childCount];
		for (int i = 0; i < pathParents.transform.childCount; i++)
		{
			paths[i] = pathParents.transform.GetChild(i).gameObject;
		}

		List<Vector2> closestPoints = new List<Vector2>();

		foreach (GameObject path in paths)
		{
			GameObject[] pathPoints = new GameObject[path.transform.childCount];
			for (int j = 0; j < path.transform.childCount; j++)
			{
				pathPoints[j] = path.transform.GetChild(j).gameObject;
			}

			GameObject[] closestPointsArray = pathPoints
				.OrderBy(p => Vector2.Distance(origin, p.transform.position))
				.Take(2)
				.ToArray();

			Vector2 closestPoint = GetClosestPointOnLineSegment(
				origin,
				closestPointsArray[0].transform.position,
				closestPointsArray[1].transform.position
			);

			closestPoints.Add(closestPoint);
		}

		return closestPoints.OrderBy(p => Vector2.Distance(origin, p)).FirstOrDefault();
	}

	public static Vector2 GetClosestPointOnLineSegment(Vector2 origin, Vector2 lineStart, Vector2 lineEnd)
	{
		Vector2 line = lineEnd - lineStart;
		float lineLength = line.magnitude;
		Vector2 lineDirection = line / lineLength;

		float t = Mathf.Clamp(Vector2.Dot(origin - lineStart, lineDirection), 0, lineLength);
		return lineStart + t * lineDirection;
	}

	public static void SetRangeCircle(LineRenderer rangeRendered, float range, Vector2 centerpoint)
	{
		rangeRendered.startWidth = 0.03f;
		rangeRendered.endWidth = 0.03f;

		float Theta = 0f;
		float radius = range;
		float ThetaScale = 0.01f;
		int Size = (int)((1f / ThetaScale) + 1f);
		rangeRendered.positionCount = Size;
		for (int i = 0; i < Size; i++)
		{
			Theta += 2.0f * Mathf.PI * ThetaScale;
			float x = radius * Mathf.Cos(Theta);
			float y = radius * Mathf.Sin(Theta);
			rangeRendered.SetPosition(i, new Vector2(x, y) + centerpoint);
		}
	}

	public static float GetAngleBetweenPoints(Vector2 origin, float originAngle, Vector2 target)
	{
		Vector2 direction = target - origin;
		float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		float angleDifference = targetAngle - originAngle;

		angleDifference = (angleDifference + 180) % 360 - 180;

		return angleDifference;
	}
}
