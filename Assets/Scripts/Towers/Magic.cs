using System.Collections;
using System.Linq;
using UnityEngine;

public class Magic : MonoBehaviour
{

    public float range = 4;
    public float cooldown = 1;
    public float damage = 10;

    private bool canShoot = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GameObject[] enemies = TowerHelpers.GetEnemiesInRange(transform, range);
        if (enemies.Length > 0 && canShoot)
        {
            GameObject closestEnemy = enemies.OrderBy(e => e.GetComponent<Movement>().getDistanceToLastPoint()).First();
            StartCoroutine(AnimateSphere(closestEnemy));
            canShoot = false;
            StartCoroutine(ResetCooldown());
        }
    }

    IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        canShoot = true;
    }

    IEnumerator AnimateSphere(GameObject enemy)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = transform.position;
        sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        sphere.GetComponent<Renderer>().material.color = Color.blue;

        yield return TowerHelpers.AnimateDirectProjectile(sphere, transform.position, enemy, 5, () => KillSphere(sphere, enemy));
    }

    void KillSphere(GameObject sphere, GameObject enemy)
    {
        Destroy(sphere);
        enemy.GetComponent<Health>().TakeDamage((int)damage);
    }
}
