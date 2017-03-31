using UnityEngine;
using System.Collections;

public class BoidAlpha : MonoBehaviour
{
	public float speed = 5f;

	void Update ()
	{
		Vector3 velocity = Vector3.zero;

		if (Input.GetKey(KeyCode.A))
		{
			velocity.x = -1;
		}
		if (Input.GetKey(KeyCode.D))
		{
			velocity.x = 1;
		}
		if (Input.GetKey(KeyCode.W))
		{
			velocity.y = 1;
		}
		if (Input.GetKey(KeyCode.S))
		{
			velocity.y = -1;
		}

		transform.position += velocity.normalized * speed * Time.deltaTime;

		if (velocity != Vector3.zero)
		{
			transform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 90);
		}
	}
}
