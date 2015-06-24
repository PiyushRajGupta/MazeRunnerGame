using UnityEngine;
using System.Collections;

public class Cam : MonoBehaviour {

	public Transform target;
	Ray myRay;
	RaycastHit hit;
	float rayLength;
	public float speed;
	public Transform camOrig;

	void Start () {
		Vector3 rayV = target.position - transform.parent.position;
		rayLength = rayV.magnitude;
		speed = 100 * Time.deltaTime;
	}

	void FixedUpdate()
	{
		myRay = new Ray (target.position - target.forward + target.up * 5, camOrig.position - target.position);
		bool a = Physics.Raycast (myRay, out hit, rayLength);
		if (a && hit.collider.tag == "wall") {
			transform.position = Vector3.MoveTowards (transform.position, hit.point, speed);
		}
	}
}
