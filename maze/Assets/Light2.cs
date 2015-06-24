using UnityEngine;
using System.Collections;

public class Light2 : MonoBehaviour {
	private float timer = 0, timer2 = 0;
	
	void Start(){
		timer = 0;
		timer2 = 0;
	}
	
	void Update () {
		Light l = GetComponent<Light> ();
		if (timer2 >= 298 && timer2 <= 300) {
			l.intensity = Mathf.Lerp(l.intensity, 0, .1f);
		}
		if (timer2 >= 300 && timer > .05f) {
			l.intensity = Random.Range(0, 1.1f);
			GetComponent<Light>().color = new Color(Random.Range(0,25), Random.Range(0, 25), Random.Range(0, 25));
			timer = 0;
		}
		timer = timer + Time.fixedDeltaTime;
		timer2 = timer2 + Time.fixedDeltaTime;
	}
}