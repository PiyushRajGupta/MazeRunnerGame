using UnityEngine;
using System.Collections;

public class halt : MonoBehaviour {
	private bool ispaused =false;
	// Use this for initialization
	// Update is called once per fme
	void Update () {
		if (Input.GetKey(KeyCode.LeftShift)){
			ispaused= !ispaused;
			//Time.timeScale = !(bool)Time.timeScale ;
			if (Time.timeScale==1){
				Time.timeScale =0;
			}
			else {
				Time.timeScale =1 ;
			}
		}
	
	}
}
