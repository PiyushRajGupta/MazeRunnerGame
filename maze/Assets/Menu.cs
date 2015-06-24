using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
//	public void OnMouseEnter(){
//		Renderer.material.color = Color.blue;
//	}
//	public void OnMouseExit(){
//		Renderer.material.color = Color.black;
//	}
	
	public void OnClickPlay(int changescene)
	{
		Application.LoadLevel (changescene);
	}
	public void OnClickQuit()
	{
		Application.Quit ();
	}
}
