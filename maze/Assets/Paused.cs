using UnityEngine;
using System.Collections;

public class Paused : MonoBehaviour {

	public bool ispaused;
	void timescale(){
		if (Time.timeScale == 1){
			Time.timeScale =0; 
		}
		else {
			Time.timeScale =1 ;
		}
	}
	// Use this for initialization
	void Start () {
		ispaused = false;
	}
	   
	public void isPause (){
	
		ispaused = !ispaused;
		timescale ();
	}
}
