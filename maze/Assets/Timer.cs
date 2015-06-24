using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour {
	
	private float timer = 300.0f;
//	//public int die =0;
//	//private bool die;
////	private Animator Controller;
		//public Motion2 Dying;
////	public void Died (){
////		Dying.Die ();
////	}

	void  Update (){
		timer -= Time.deltaTime;
//		if (timer <= 0){
//			die = 1;
//		}
	}

//	void  Die() {
//		
//		Controller.SetTrigger ("Dead");
//	}
	
	void  OnGUI (){
		GUI.Box(new Rect (10, 10, 110, 25), "Time Left:" + timer.ToString ("0"));
		if (timer <= 31.0 && timer >=0.0) {
			GUI.Label(new Rect (500, 260, 300, 25), "Run, You have only " + timer.ToString ("0")+ " seconds remaining.");
		}
		if (timer <=0.0){
			GUI.Label(new Rect (500, 300, 250,30), "Oops!! You ran out of time! You Died.");
			//Application.LoadLevel(4);
		}
		if (timer <= -5.0) {
			Application.LoadLevel (4);
		}
	}

}