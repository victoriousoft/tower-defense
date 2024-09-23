using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] Transform pathParent;
    [SerializeField] float speed = 1f;

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

    // Update is called once per frame
    void Update()
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

        transform.position = Vector3.MoveTowards(transform.position, points[currentPointIndex].position, speed * Time.deltaTime);
    }
}
