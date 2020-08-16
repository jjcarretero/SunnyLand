using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
	public GameObject Target;
	private Vector3 FinalPos;
	private float Speed = 5;

	private void FixedUpdate()
	{
		FinalPos = new Vector3(Target.transform.position.x, Target.transform.position.y, -10);
		transform.position = Vector3.Lerp(transform.position, FinalPos, Speed * Time.deltaTime);
		if(transform.position.x < 0)
			transform.position = new Vector3(0, transform.position.y, transform.position.z);
		else if(transform.position.x > 117)
			transform.position = new Vector3(117, transform.position.y, transform.position.z);

		if (transform.position.y < 0)
			transform.position = new Vector3(transform.position.x, 0, transform.position.z);
		else if (transform.position.y > 6)
			transform.position = new Vector3(transform.position.x, 6, transform.position.z);
	}
}
