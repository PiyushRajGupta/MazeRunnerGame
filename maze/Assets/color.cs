using UnityEngine;
using System.Collections;

public class color : MonoBehaviour {

	void Start () {
		GetComponent<Renderer> ().material.color = Color.yellow;
	}
	
}
