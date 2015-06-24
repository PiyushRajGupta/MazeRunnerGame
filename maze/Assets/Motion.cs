using UnityEngine;
using System.Collections;

public class Motion : MonoBehaviour {

	private Animator Controller ;
	private float Vertical ;

	/*private float Horizontal ;
	private bool turnright;*/

	void Start () {
		Controller = GetComponent<Animator> ();

	
	}


	void Update () {
		Vertical = Input.GetAxis ("Vertical");
		Controller.SetFloat ("Walk", Vertical);
//		while (i!= 600.0f) {
//			i= i + Time.deltaTime ;
//		}
//		if (i == 600) {
//			Controller.SetBool ("Dead", true);
//		}
		float rot = 0;
		float rotspeed = 210;
		float v = Mathf.Abs (Vertical), V;
		if (v != 0)
			V = Vertical / v;
		else
			V = 1;
		if (Input.GetKey (KeyCode.RightArrow)) {
			rot = 1 * V;
			transform.Rotate (Vector3.up * Time.deltaTime * rotspeed * rot);
		} else if (Input.GetKey (KeyCode.LeftArrow)) {
			rot = -1 * V;
			transform.Rotate (Vector3.up * Time.deltaTime * rotspeed * rot);
		} else {
			rot = 0;
			transform.Rotate (Vector3.up * Time.deltaTime * rotspeed * rot);
		}
	}



}

