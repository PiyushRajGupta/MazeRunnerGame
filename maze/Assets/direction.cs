using UnityEngine;
using System.Collections;

public class direction : MonoBehaviour {

	public Transform player;

	void Start () {
		transform.rotation = player.rotation;
	}
	
	void Update () {
		transform.rotation = player.rotation;
		transform.position = new Vector3 (player.position.x - 18, 450, player.position.z);
	}
}
