using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsController : MonoBehaviour {

	public float shrinkSpeed = 1f;

	// Use this for initialization
	void Start () {
		//StartCoroutine(ShrinkBounds());
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		this.gameObject.transform.localScale -= Vector3.one*Time.deltaTime*shrinkSpeed;
	}

	IEnumerator ShrinkBounds(){
		yield return new WaitForSeconds(1);
		
		ShrinkBounds();
	}

}
