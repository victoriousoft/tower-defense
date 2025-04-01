using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketAnimation : MonoBehaviour
{
	public Transform sprite;
	private Vector3 lastPosition;
	private ParticleSystem ps;

	void Start()
	{
		lastPosition = transform.position;
		if (GetComponentInChildren<ParticleSystem>() != null)
		{
			ps = GetComponentInChildren<ParticleSystem>();
			ps.Play();
		}
	}

	void Update()
	{
		Vector3 direction = (transform.position - lastPosition).normalized;

		if (direction != Vector3.zero)
		{
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
			sprite.rotation = Quaternion.Euler(0, 0, angle);
		}
		lastPosition = transform.position;
	}
}
