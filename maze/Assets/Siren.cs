using UnityEngine;
using System.Collections;

public class Siren : MonoBehaviour {
	
	static Light l;
	static Color switch_to;
	static float counter = 0;
	
	void Start () {
		l = gameObject.GetComponent<Light> ();
		switch_to = Color.blue;
		l.intensity = 0;
	}
	
	void Update () {
		if (Time.time >= 210) {
			if(l.intensity == 0) l.intensity = 2;
			if (counter < .3f) {
				if (l.color.r > .9f) {
					switch_to = Color.blue;
					l.color = Color.red;
				} else if (l.color.b > .9f) {
					switch_to = Color.red;
					l.color = Color.blue;
				}
				counter = counter + 6 * Time.fixedDeltaTime;
			} else if (counter < .6f && (switch_to.r > .9f || switch_to.b > .9f)) {
				l.color = Color.Lerp (l.color, switch_to, .5f);
				counter = counter + Time.fixedDeltaTime;
			} else
				counter = 0;
		}
	}
}