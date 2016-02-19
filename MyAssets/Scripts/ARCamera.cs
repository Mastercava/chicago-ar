using UnityEngine;
using System.Collections;

public class ARCamera : MonoBehaviour {

	public float smoothingFactor = 2f;
	public float minimumSmoothingMovement = 1f;

	private Vector3 lastPosition;
	private Quaternion lastRotation;

	// Use this for initialization
	void Start () {
		UpdateProjectionMatrix ();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(smoothingFactor > 0 && !IsRelevantMovement()) {
			transform.position = Vector3.Lerp(lastPosition, transform.position, Time.deltaTime * smoothingFactor);
			transform.rotation = Quaternion.Slerp(lastRotation, transform.rotation, Time.deltaTime * smoothingFactor);
			lastPosition = transform.position;
			lastRotation = transform.rotation;
		}
	}


	public void SetSmoothingFactor(float value) {
		smoothingFactor = value;
	}

	public bool IsRelevantMovement() {
		float movement = Vector3.Distance (lastPosition, transform.position);
		return movement > minimumSmoothingMovement;
	}

	public void UpdateProjectionMatrix() {
		
		/*
		Camera camera = GetComponentInChildren<Camera> ();
		Matrix4x4 m = camera.projectionMatrix;

		Debug.Log (m);

		float nearPlane = camera.nearClipPlane;
		float farPlane = camera.farClipPlane;

		Debug.Log ("Near: " + nearPlane + ", Far: " + farPlane);

		nearPlane = 0.01f;
		farPlane = 10000f;

		float c = (farPlane + nearPlane) / (farPlane - nearPlane);
		float d = -nearPlane * (1.0f + c);
		m[2, 2] = -c;
		m[2, 3] = d;

		camera.projectionMatrix = m;

		Debug.Log (m);

		Debug.Log ("Near: " + camera.nearClipPlane + ", Far: " + camera.farClipPlane);
		*/
	}
}
