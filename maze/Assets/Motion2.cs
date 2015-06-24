using UnityEngine;
using System.Collections;

public class Motion2 : MonoBehaviour {
	
	private Animator Controller ;
	private float Vertical ;
	private float dead;
	private AudioSource[] a;
	private Ray plRay;
	private bool isCollided;
	private float timer= 300.0f;
	
	void Start () {
		a = GetComponents<AudioSource> ();
		Controller = GetComponent<Animator> ();
	}
	
	void Update () {
		timer -= Time.deltaTime;
		Controller.SetFloat ("Dead", dead);
		if (timer <= 0.0f) {
			Die ();
		} else {
			dead = 0.0f;
		} 
		plRay = new Ray (transform.position, transform.forward);
		bool collDet = Physics.Raycast (plRay, 3);
		if (!collDet  && Time.timeScale == 1 ) {
			Vertical = Input.GetAxis ("Vertical");
			isCollided = false;
		} else if (collDet && Time.timeScale == 1) {
			if(!isCollided  && Time.timeScale == 1 ){ 
				a[3].pitch = Random.Range(.8f, 1.2f);
				a[3].volume = Random.Range(.8f, 1f);
				a[3].Play();
				//if(Vertical > 0)Vertical = 0;
			} 
			isCollided = true;
		}
		Controller.SetFloat ("Walk", Vertical);
		Vertical = Input.GetAxis("Vertical");
		float rot = 0;
		float rotspeed = 210;
		float v = Mathf.Abs (Vertical), V;
		if (v != 0)
			V = Vertical / v;
		else
			V = 1;
		if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey (KeyCode.D) ){
			rot = 1 * V;
			transform.Rotate (Vector3.up * Time.deltaTime * rotspeed * rot);
		} else if (Input.GetKey (KeyCode.LeftArrow) || Input.GetKey (KeyCode.A) ) {
			rot = -1 * V;
			transform.Rotate (Vector3.up * Time.deltaTime * rotspeed * rot);
		} else {
			rot = 0;
			transform.Rotate (Vector3.up * Time.deltaTime * rotspeed * rot);
		}
		
		if (!a [1].isPlaying && !Input.GetKey (KeyCode.UpArrow) && !Input.GetKey (KeyCode.DownArrow) && Time.timeScale == 1) {
			a [1].volume = Random.Range (.8f, 1);
			a [1].pitch = Random.Range (1f, 1.2f);
			a [1].Play ();
		}
		
		if (Input.GetKeyDown (KeyCode.UpArrow) || Input.GetKeyDown (KeyCode.DownArrow) && Time.timeScale == 1 ) {
			a [0].PlayDelayed (.35f);
			a [1].Stop();
		} 
		if (Vertical > 0 && !a[0].isPlaying && !a[2].isPlaying  && Time.timeScale == 1 ) {
			a[0].pitch = Random.Range(2.1f, 2.3f);
			a[2].pitch = Random.Range(1.2f, 1.5f);
			a[0].volume = Random.Range (.8f, 1);
			a[2].volume = a[0].volume;
			if(!a[0].isPlaying) a[0].Play ();
			if(!a[2].isPlaying) a[2].Play ();
		} else if (Vertical < 0  && Time.timeScale == 1 ) {
			a[0].pitch = Random.Range(.7f, 1.2f);
			a[2].pitch = a[1].pitch;
			a[0].volume = Random.Range (.6f, .79f);
			a[2].volume = a[0].volume;
			if(!a[0].isPlaying) a[0].Play ();
			if(!a[2].isPlaying) a[2].Play ();
		}
		
	}
	void Die(){
		dead = 1.0f;
	}
	
	
	
}