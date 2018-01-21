using UnityEngine;
using System.Collections;

public class ParticleSystemDestroyer : MonoBehaviour {
	void Start () {
		StartCoroutine (Destroy ());
	}
	
	IEnumerator Destroy(){
		yield return new WaitForSeconds (4);
		Destroy (gameObject);
	}
}
