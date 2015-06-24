using UnityEngine;
using System.Collections;

public class mainmenu : MonoBehaviour {

    public void OnClickPlay(int changescene)
	{
			Application.LoadLevel (changescene);
	}
//	public void OnClickResume(){
//		Time.timeScale = 0;
//	}
	public void OnClickQuit()
	{
		Application.Quit ();
	}
}
