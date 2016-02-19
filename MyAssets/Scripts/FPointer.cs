using UnityEngine;
using System.Collections;

public class FPointer : MonoBehaviour {

	private Transform cameraTransform;

	// Use this for initialization
	void Start () {
		cameraTransform = GameObject.FindGameObjectWithTag ("FinalCamera").transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.position = new Vector3 (cameraTransform.position.x, transform.position.y, cameraTransform.position.z);
		transform.rotation = Quaternion.Euler (0f, cameraTransform.rotation.eulerAngles.y, 0f);
	}
}
