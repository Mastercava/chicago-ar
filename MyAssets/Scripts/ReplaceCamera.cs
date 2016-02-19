using UnityEngine;
using System.Collections;
using System;

public class ReplaceCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (ReplaceCameras());
	}

	private IEnumerator ReplaceCameras() {
		yield return new WaitForSeconds (2f);
		GetComponent<Camera> ().fieldOfView = GetFieldOfView ();
		Camera.main.enabled = false;
	}
	
	private float GetFieldOfView() {
		Matrix4x4 mat = Camera.main.projectionMatrix;

		float a = mat[0];
		float b = mat[5];
		float c = mat[10];
		float d = mat[14];

		float aspect_ratio = b / a;

		float k = (c - 1.0f) / (c + 1.0f);
		float clip_min = (d * (1.0f - k)) / (2.0f * k);
		float clip_max = k * clip_min;

		float RAD2DEG = 180.0f / 3.14159265358979323846f;
		return RAD2DEG * (2.0f * (float)Math.Atan(1.0f / b));
	}

}
