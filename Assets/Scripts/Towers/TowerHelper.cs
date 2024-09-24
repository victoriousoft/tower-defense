using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public static class TowerHelpers
{
    public static class TowerProjectileRotationTypes
    {
        public const string NONE = "none";
        public const string LOOK_AT_TARGET = "lookAtTarget";
        public const string SPIN = "spin";
    }

    public static GameObject[] GetEnemiesInRange(Transform transform, float range)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);
        return hitColliders.Where(c => c.CompareTag("Enemy")).Select(c => c.gameObject).ToArray();
    }

    public static Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return Mathf.Pow(1 - t, 2) * p0 + 2 * (1 - t) * t * p1 + Mathf.Pow(t, 2) * p2;
    }

    public static IEnumerator AnimateBezierProjectile(
        GameObject projectile,
        Vector3 startPosition,
        GameObject target,
        float height,
        float duration,
        Action destroyCallback,
        string rotationType = TowerProjectileRotationTypes.NONE
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
                projectile.transform.Rotate(Vector3.up, 360 * Time.deltaTime / duration);
            }

            yield return null;
        }

        destroyCallback();
    }
}