using System.Collections;
using UnityEngine;

public class Bomber : BaseTower
{
    public float splashRadius = 1;

    override protected IEnumerator AnimateProjectile(GameObject enemy)
    {
        GameObject bomb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bomb.transform.SetParent(transform);
        bomb.transform.position = transform.position;
        bomb.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        bomb.GetComponent<Renderer>().material.color = Color.red;

        yield return TowerHelpers.AnimateBezierProjectile(bomb, transform.position, enemy, 2, 1, KillProjectile, TowerHelpers.TowerProjectileRotationTypes.SPIN);
    }
    override protected IEnumerator AnimateLaser(GameObject enemy){ yield return null;}

    override protected IEnumerator KillLaser(LineRenderer laserRenderer,Transform origin, GameObject enemy, Vector3 enemyPosition){ yield return null;}
    override protected void KillProjectile(GameObject bomb, GameObject enemy, Vector3 enemyPosition)
    {
        Destroy(bomb);
        GameObject[] enemies = TowerHelpers.GetEnemiesInRange(enemyPosition, splashRadius);
        foreach (GameObject e in enemies)
        {
            if (e == null) return;
            e.GetComponent<BaseEnemy>().TakeDamage((int)damage, DamageTypes.EXPLOSION);
        }
    }
}
