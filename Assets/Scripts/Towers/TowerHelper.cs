using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TowerHelpers
{
    public enum TowerProjectileRotationTypes { NONE, LOOK_AT_TARGET, SPIN };
    public enum TowerTargetTypes { CLOSEST_TO_FINISH, CLOSEST_TO_START, MOST_HP, LEAST_HP };


    public static GameObject[] GetEnemiesInRange(Vector3 position, float range)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, range);
        List<GameObject> enemies = new List<GameObject>();
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy")) enemies.Add(collider.gameObject);
        }
        return enemies.ToArray();
        /* tady jsem to změnil z 3d box collideru na 2d box collidery, a to i v gamese (nevim jestli je to nutny - kdyz bys to chtel ale vratit bylo by potreba zmenit i componenty na enemakach zpatky na 3d a v exitech checkovat 3d a ne 2d)
        Collider[] hitColliders = Physics.OverlapSphere(position, range);
        return hitColliders.Where(c => c.CompareTag("Enemy")).Select(c => c.gameObject).ToArray();
        */
    }

    public static GameObject SelectEnemyToAttack(GameObject[] enemies, TowerTargetTypes targetType)
    {
        GameObject target = null;

        switch (targetType)
        {
            case TowerTargetTypes.CLOSEST_TO_FINISH:
                target = enemies.OrderBy(e => e.GetComponent<Movement>().GetDistanceToFinish()).FirstOrDefault();
                break;
            case TowerTargetTypes.CLOSEST_TO_START:
                target = enemies.OrderBy(e => e.GetComponent<Movement>().GetDistanceToStart()).FirstOrDefault();
                break;
            case TowerTargetTypes.MOST_HP:
                target = enemies.OrderByDescending(e => e.GetComponent<Health>().health).FirstOrDefault();
                break;
            case TowerTargetTypes.LEAST_HP:
                target = enemies.OrderBy(e => e.GetComponent<Health>().health).FirstOrDefault();
                break;
            default:
                Debug.LogWarning("Invalid target type, fallback to closest to finish");
                target = enemies.OrderBy(e => e.GetComponent<Movement>().GetDistanceToFinish()).FirstOrDefault();
                break;
        }

        return target;
    }

    // moc nevim co tohle dělá
    public static Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return Mathf.Pow(1 - t, 2) * p0 + 2 * (1 - t) * t * p1 + Mathf.Pow(t, 2) * p2;
    }

    // animuje bezier projectile (archers, bomb tower)
    public static IEnumerator AnimateBezierProjectile(
        GameObject projectile,
        Vector3 startPosition,
        GameObject target,
        float height,
        float duration,
        Action<GameObject, GameObject, Vector3> destroyCallback,
        TowerProjectileRotationTypes rotationType = TowerProjectileRotationTypes.NONE
        )
    {
        Vector3 targetPosition = target != null ? target.transform.position : Vector3.zero;
        Vector3 controlPoint = (startPosition + targetPosition) / 2 + Vector3.up * height;

        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            if (target != null)
            {
                if (target.CompareTag("Enemy")) targetPosition = target.transform.position;
                else target = null;
            }

            controlPoint = (startPosition + targetPosition) / 2 + Vector3.up * height;

            float t = (Time.time - startTime) / duration;
            Vector3 currentPosition = CalculateBezierPoint(t, startPosition, controlPoint, targetPosition);
            projectile.transform.position = currentPosition;

            Vector3 direction = (CalculateBezierPoint(t + 0.01f, startPosition, controlPoint, targetPosition) - currentPosition).normalized;

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
}