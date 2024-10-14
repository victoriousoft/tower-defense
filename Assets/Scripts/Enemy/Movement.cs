using UnityEngine;

public class Movement : MonoBehaviour
{
    public Transform pathParent;
    public float speed = 1f;
    public bool isPaused = false;

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
        if (!isPaused) Move();
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

    public float GetDistanceToFinish()
    {
        float distance = 0;
        for (int i = currentPointIndex; i < points.Length - 1; i++)
        {
            distance += Vector3.Distance(points[i].position, points[i + 1].position);
        }

        return distance;
    }

    public float GetDistanceToStart()
    {
        float distance = 0;
        for (int i = currentPointIndex; i >= 0; i--)
        {
            distance += Vector3.Distance(points[i].position, points[i - 1].position);
        }

        return distance;
    }
}
