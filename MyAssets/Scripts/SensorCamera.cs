using UnityEngine;
using System.Collections;
using System;
using Vuforia;

public class SensorCamera : MonoBehaviour {

	private Camera arCamera;
	private Transform arCameraTransform;

	private Vector3 arCameraPosition;
	private Quaternion arCameraRotation;

	private Quaternion deltaRotation;

	private Transform backgroundPlane;

	// Use this for initialization
	void Start () {
		Input.gyro.enabled = true;
		arCamera = Camera.main;
		arCameraTransform = arCamera.transform.parent.transform;
		deltaRotation = Quaternion.identity;
		backgroundPlane = GameObject.FindWithTag ("BackgroundPlane").transform;

		#if UNITY_ANDROID
		SensorHelper.ActivateRotation();
		#endif
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(Input.GetKeyDown(KeyCode.Space)) {

			SyncCameras ();

		}
		else if(Input.GetKeyDown(KeyCode.KeypadEnter)) {
			SwitchCameras ();
		}
			
		transform.position = arCameraTransform.position;
			
		RotateWithSensors ();
		transform.rotation = transform.rotation * deltaRotation;

	}


	public void SyncCameras() {
		RotateWithSensors ();

		GetComponent<Camera> ().fieldOfView = GetFieldOfView ();

		deltaRotation = Quaternion.Inverse(transform.rotation) * arCameraTransform.rotation;

		Debug.Log (deltaRotation);

		transform.position = arCameraTransform.position;
		transform.rotation = arCameraTransform.rotation;
	}


	public void SwitchCameras() {
		if (arCamera.enabled) {
			arCamera.enabled = false;
			Vector3 localPosition = backgroundPlane.localPosition;
			Quaternion localRotation = backgroundPlane.localRotation;
			Vector3 localScale = backgroundPlane.localScale;
			backgroundPlane.transform.parent = transform;
			backgroundPlane.localPosition = localPosition;
			backgroundPlane.localRotation = localRotation;
			backgroundPlane.localScale = localScale;
		} else {
			arCamera.enabled = true;
			Vector3 localPosition = backgroundPlane.localPosition;
			Quaternion localRotation = backgroundPlane.localRotation;
			Vector3 localScale = backgroundPlane.localScale;
			backgroundPlane.transform.parent = arCameraTransform;
			backgroundPlane.localPosition = localPosition;
			backgroundPlane.localRotation = localRotation;
			backgroundPlane.localScale = localScale;
		}
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


	private void RotateWithSensors() {
		#if UNITY_ANDROID
		transform.rotation = SensorHelper.rotation;
		#else
		transform.rotation = Input.gyro.attitude;
		transform.Rotate (0f, 0f, 180f, Space.Self);
		transform.Rotate (90f, 180f, 0f, Space.World);
		#endif
	}
}
