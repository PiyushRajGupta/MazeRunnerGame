using UnityEngine;
using System.Collections;

public class MiniMapScript : MonoBehaviour {

	public Transform target;

	void Start(){
		transform.position = new Vector3(target.position.x, 800, target.position.z);
	}

	void Update () {
		transform.position = new Vector3(target.position.x, 800, target.position.z);
	}
}
