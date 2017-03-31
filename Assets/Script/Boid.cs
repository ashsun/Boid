using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boid : MonoBehaviour
{
	public float minSpeed = 4f;
	public float turnSpeed = 20f;
	public float randomFreq = 20f;
	public float randomForce = 5f;

	public float toOriginRange = 10f;
	public float toOriginForce = 50f;

	public float avoidanceRadius = 5f;
	public float avoidanceForce = 20f;

	public float followRadius = 7f;
	public float followVelocity = 5f;

	public float gravity = 2f;


	private Transform transformComponent;
	private Transform origin;
	private List<Boid> otherBoids;

	private Vector3 velocity;
	private Vector3 normalizeVelocity;
	private Vector3 randomPush;
	private Vector3 originPush;


	void Awake()
	{
		randomFreq = 1f / randomFreq;

		transformComponent = transform;
		origin = transform.parent;
		otherBoids = new List<Boid>(origin.GetComponentsInChildren<Boid>());
	}

	void Start()
	{
		transform.parent = null;

		StartCoroutine(UpdateRandom());
	}

	void Update()
	{
		Vector3 avgPosition = Vector3.zero;
		Vector3 avgVelocity = Vector3.zero;
		Vector3 forceV;
		float f = 0f;
		float d = 0f;

		for (int i = 0; i < otherBoids.Count; i++)
		{
			if (otherBoids[i] != this)
			{
				forceV = transform.position - otherBoids[i].transform.position;
				d = forceV.magnitude;

				avgPosition += otherBoids[i].transform.position;

				if (d < followRadius)
				{
					if (d < avoidanceRadius)
					{
						f = 1f - (d / avoidanceRadius);
						if (d > 0)
						{
							avgVelocity += forceV / d * f * avoidanceForce;
						}

						f = d / followRadius;
						avgVelocity += otherBoids[i].normalizeVelocity * f * followVelocity;
					}
				}
			}
		}

		avgPosition /= (otherBoids.Count - 1);
		avgVelocity /= (otherBoids.Count - 1);

		forceV = origin.transform.position - transform.position;
		d = forceV.magnitude;
		f = d / toOriginRange;
		if (d > 0)
		{
			originPush = (forceV / d) * f * toOriginForce;
		}

		float speed = velocity.magnitude;
		if (speed < minSpeed && speed > 0)
		{
			velocity = (velocity / speed) * minSpeed;
		}

		/*
		 * -velocity 惯性
		 * randomPush 随机行为
		 * originPush 队列规则
		 * avgVelocity 分离规则和凝聚规则
		 * (avgPosition - transform.position).normalized * gravity 向心力
		 */
		Vector3 wantedVel = velocity + (-velocity + randomPush + originPush + avgVelocity + (avgPosition - transform.position).normalized * gravity) * Time.deltaTime;
		velocity = Vector3.RotateTowards(velocity, wantedVel, turnSpeed * Time.deltaTime, 100f);

		transform.position += velocity * Time.deltaTime;
		transform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 90);

		normalizeVelocity = velocity.normalized;
	}

	IEnumerator UpdateRandom()
	{
		while (true)
		{
			randomPush = Random.insideUnitCircle * randomForce;
			yield return new WaitForSeconds(randomFreq * Random.Range(0.5f, 1.5f));
		}
	}
}
