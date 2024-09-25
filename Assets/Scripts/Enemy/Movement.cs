using UnityEngine;

public class Movement : MonoBehaviour
{
    public Transform pathParent;
    public float speed = 1f;

    private int currentPointIndex = 0;
    private Transform[] points;

    void Start()
    {
        points = new Transform[pathParent.childCount];
        for (int i = 0; i < pathParent.childCount; i++)
        {
            points[i] = pathParent.GetChild(i);
        }

        transform.position = points[currentPointIndex].position;

    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        if (Vector3.Distance(transform.position, points[currentPointIndex].position) < 0.1f)
        {
            currentPointIndex++;
            if (currentPointIndex >= points.Length)
            {
                Destroy(gameObject);
                return;
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, points[currentPointIndex].position, speed * Time.fixedDeltaTime);
    }

    public float getDistanceToLastPoint()
    {
        return Vector3.Distance(transform.position, points[points.Length - 1].position);
    }
}
