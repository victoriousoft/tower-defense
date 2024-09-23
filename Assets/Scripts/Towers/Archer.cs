using System.Collections;
using System.Linq;
using UnityEngine;

public class Archer : MonoBehaviour
{
    public float range = 4;
    public float cooldown = 1;
    public float damage = 10;

    private bool canShoot = true;

    void Start()
    {
        // TODO: draw the range of the tower
        //DrawRangeOutline();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] enemies = GetEnemiesInRange();
        Debug.Log("Enemies in range: " + enemies.Length);
        Debug.Log("Can shoot: " + canShoot);
        if (enemies.Length > 0 && canShoot)
        {
            GameObject closestEnemy = enemies.OrderBy(e => e.GetComponent<Movement>().getDistanceToLastPoint()).First();
            Debug.Log("Closest enemy: " + closestEnemy.name);
            Shoot(closestEnemy);
            StartCoroutine(AnimateArrow(closestEnemy));
            canShoot = false;
            StartCoroutine(ResetCooldown());
        }
    }

    IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        canShoot = true;
    }

    // 100% Copilot, 0% muj kod
    IEnumerator AnimateArrow(GameObject enemy)
    {
        GameObject arrow = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        arrow.transform.position = transform.position;
        arrow.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        arrow.GetComponent<Renderer>().material.color = Color.red;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = enemy.transform.position;
        Vector3 controlPoint = (startPosition + targetPosition) / 2 + Vector3.up * 2;

        float duration = 1f;
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            if (enemy == null)
            {
                // TODO: do not destroy arrows mid air
                Destroy(arrow);
                yield break;
            }

            targetPosition = enemy.transform.position;
            controlPoint = (startPosition + targetPosition) / 2 + Vector3.up * 2;

            float t = (Time.time - startTime) / duration;
            Vector3 currentPosition = CalculateBezierPoint(t, startPosition, controlPoint, targetPosition);
            arrow.transform.position = currentPosition;

            Vector3 direction = (CalculateBezierPoint(t + 0.01f, startPosition, controlPoint, targetPosition) - currentPosition).normalized;
            arrow.transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0);

            yield return null;
        }

        Destroy(arrow);
    }

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return Mathf.Pow(1 - t, 2) * p0 + 2 * (1 - t) * t * p1 + Mathf.Pow(t, 2) * p2;
    }


    void Shoot(GameObject enemy)
    {
        Debug.Log("Shooting at " + enemy.name);
        enemy.GetComponent<Health>().TakeDamage((int)damage);
    }

    GameObject[] GetEnemiesInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);
        return hitColliders.Where(c => c.CompareTag("Enemy")).Select(c => c.gameObject).ToArray();
    }
}
