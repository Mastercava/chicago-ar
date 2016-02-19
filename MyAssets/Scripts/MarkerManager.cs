using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class MarkerManager : MonoBehaviour {

	/*** PUBLIC ATTRIBUTES ***/

	public Text markerCountText;
	public float smoothingFactor = 2f;
	public float minimumSmoothingMovement = 3f;

	/*** PRIVATE ATTRIBUTES ***/

	private static MarkerManager instance;

	private List<GameObject> markers = new List<GameObject>();

	private Transform arTransform;
	private Camera vuforiaCamera, arCamera;
	private Transform sensorTransform;
	private Camera sensorCamera;
	private Camera mapCamera;
	private Camera finalCamera;
	private Transform finalTransform;

	private Camera currentCamera;

	private Transform backgroundPlane;
	private Vector3 backgroundPosition;
	private Vector3 backgroundScale;
	private Quaternion backgroundRotation;

	private Vector3 lastArPosition;
	private Quaternion lastArRotation;

	private Quaternion deltaRotation = Quaternion.identity;

	private bool isGPSInitialized = false;
	private Transform userTransform;

	private List<ARClient> arClientScripts = new List<ARClient> ();

	/*** RUNTIME METHOS ***/

	private MarkerManager() {}


	public static MarkerManager GetInstance() {
		if (instance == null) {
			instance = new MarkerManager ();
		}
		return instance;
	}
		

	void Awake () {
		if (instance == null) {
			instance = this;
			LinkGameobjects ();
		}
	}


	void Update () {
		int markerCount = markers.Count;
		markerCountText.text = markerCount.ToString () +  (currentCamera? "\n" + currentCamera.gameObject.name : "");
	}

	void LateUpdate() {
		//AR Camera
		if(smoothingFactor > 0) {
			//Smoothing
			if (!IsRelevantMovement (lastArPosition, arTransform.position)) {
				arTransform.position = Vector3.Lerp (lastArPosition, arTransform.position, Time.deltaTime * smoothingFactor);
				arTransform.rotation = Quaternion.Slerp (lastArRotation, arTransform.rotation, Time.deltaTime * smoothingFactor);
				lastArPosition = arTransform.position;
				lastArRotation = arTransform.rotation;
			}
			//No smoothing already applied
		}

		//Sensor Camera
		if (smoothingFactor > 0) {
			//Smoothing
			sensorTransform.rotation = Quaternion.Slerp (sensorTransform.rotation, GetDeviceOrientation () * deltaRotation, Time.deltaTime * smoothingFactor);
		} else {
			//No smoothing
			sensorTransform.rotation = GetDeviceOrientation ();
			//Compass correction?
			//sensorTransform.rotation = Quaternion.Euler (sensorTransform.rotation.eulerAngles.x, userTransform.rotation.eulerAngles.y, sensorTransform.rotation.eulerAngles.z);
		}
		if (Input.location.status == LocationServiceStatus.Running) {
			//Get position from GPS
			sensorTransform.position = new Vector3(userTransform.position.x, 0.018f, userTransform.position.z);
		} else {
			//Copy position from AR Camera
			sensorTransform.position = arTransform.position;
		}


		//Final Camera
		if (markers.Count > 0) {
			finalTransform.position = arTransform.position;
			finalTransform.rotation = arTransform.rotation;
		} else {
			finalTransform.position = arTransform.position; //To decide...
			finalTransform.rotation = sensorTransform.rotation * deltaRotation; //Without GPS
			//finalTransform.rotation = sensorTransform.rotation //With GPS?
		}

			
	}


	/*** HELPERS ***/

	private void LinkGameobjects() {
		//Get GameObjects
		vuforiaCamera = Camera.main;
		arTransform = vuforiaCamera.transform.parent.transform;
		sensorTransform = GameObject.FindGameObjectWithTag ("SensorCamera").transform;
		sensorCamera = sensorTransform.gameObject.GetComponent<Camera> ();
		finalTransform = GameObject.FindGameObjectWithTag ("FinalCamera").transform;
		finalCamera = finalTransform.gameObject.GetComponent<Camera> ();
		arCamera = GameObject.FindGameObjectWithTag ("ArCamera").GetComponent<Camera> ();
		backgroundPlane = GameObject.FindWithTag ("BackgroundPlane").transform;
		Debug.Log ("Gameobjects linked correctly");

		//MapCamera
		GameObject mapWorld = GameObject.FindGameObjectWithTag ("MapCamera");
		if(mapWorld) {
			mapCamera = mapWorld.GetComponent<Camera> ();
			mapCamera.enabled = false;
		}

		StartCoroutine (InitializeCameras());

		//Gyroscope
		Input.gyro.enabled = true;
		#if UNITY_ANDROID
		SensorHelper.ActivateRotation();
		#endif

		//GPS from map pointer
		userTransform = GameObject.FindGameObjectWithTag("Player").transform;
	}


	private IEnumerator InitializeCameras() {

		yield return new WaitForSeconds (2f);
		
		while(! Vuforia.VuforiaManager.Instance.Initialized) {
			yield return null;
		}
		
		float fov = GetFieldOfView (vuforiaCamera);
		Debug.Log ("Field of view: " + fov);

		//Set field of view
		arCamera.fieldOfView = fov;
		sensorCamera.fieldOfView = fov;
		finalCamera.fieldOfView = fov;

		//Replace Vuforia camera with Unity one
		vuforiaCamera.enabled = false;
		arCamera.enabled = false;
		sensorCamera.enabled = false;
		currentCamera = finalCamera;

		//Keep only final camera

		StoreBackgroundParameters ();

		backgroundPlane.parent = finalTransform;
		ResetBackgroundParameters ();

		Debug.Log ("Cameras correctly initialized");

	}


	public void MarkerFound(GameObject marker) {
		if (markers.Count == 0) {
			AlignCameras ();
		}
		if (!markers.Contains (marker)) {
			markers.Add (marker);
		}
	}


	public void MarkerLost(GameObject marker) {
		markers.Remove (marker);
	}


	private float GetFieldOfView(Camera cam) {
		Matrix4x4 mat = cam.projectionMatrix;

		float a = mat[0];
		float b = mat[5];
		float c = mat[10];
		float d = mat[14];

		float aspect_ratio = b / a;

		float k = (c - 1.0f) / (c + 1.0f);
		float clip_min = (d * (1.0f - k)) / (2.0f * k);
		float clip_max = k * clip_min;

		float RAD2DEG = 180.0f / 3.14159265358979323846f;
		return RAD2DEG * (2.0f * (float) Math.Atan (1.0f / b));
	}


	public bool IsRelevantMovement(Vector3 lastPosition, Vector3 actualPosition) {
		float movement = Vector3.Distance (lastPosition, actualPosition);
		return movement > minimumSmoothingMovement;
	}


	public void SetSmoothingFactor(float value) {
		smoothingFactor = value;
	}


	public Quaternion GetDeviceOrientation() {
		#if UNITY_ANDROID
		return SensorHelper.rotation;
		#else
		Transform temp = new Transform();
		temp.rotation = Input.gyro.attitude;
		temp.Rotate (0f, 0f, 180f, Space.Self);
		temp.Rotate (90f, 180f, 0f, Space.World);
		return temp;
		#endif
	}

	private void StoreBackgroundParameters() {
		//Copy background parameters
		backgroundPosition = new Vector3(backgroundPlane.localPosition.x, backgroundPlane.localPosition.y, backgroundPlane.localPosition.z);
		backgroundScale = new Vector3 (backgroundPlane.localScale.x, backgroundPlane.localScale.y, backgroundPlane.localScale.z);
		backgroundRotation = new Quaternion(backgroundPlane.rotation.x, backgroundPlane.rotation.y, backgroundPlane.rotation.z, backgroundPlane.rotation.w);
		Debug.Log (backgroundRotation);
	}


	private void ResetBackgroundParameters() {
		backgroundPlane.localPosition = backgroundPosition;
		backgroundPlane.localRotation = backgroundRotation;
		backgroundPlane.localScale = backgroundScale;
	}

	/*
	public void UseSensorCamera() {
		if (currentCamera == sensorCamera || vuforiaCamera.enabled) {
			return;
		}
		backgroundPlane.parent = sensorTransform;
		ResetBackgroundParameters ();
		sensorCamera.enabled = true;
		arCamera.enabled = false;
		finalCamera.enabled = false;
		currentCamera = sensorCamera;
	}

	public void UseArCamera() {
		if (currentCamera == arCamera || vuforiaCamera.enabled) {
			return;
		}
		backgroundPlane.parent = arTransform;
		ResetBackgroundParameters ();
		sensorCamera.enabled = false;
		arCamera.enabled = true;
		currentCamera = arCamera;
	}
	*/
		

	public void ToggleMap() {
		if (mapCamera) {
			if (currentCamera == mapCamera) {
				mapCamera.enabled = false;
				finalCamera.enabled = true;
				currentCamera = finalCamera;
			} else {
				mapCamera.enabled = true;
				currentCamera.enabled = false;
				currentCamera = mapCamera;
			}
		}
	}

	public void AlignCameras() {
		sensorTransform.rotation = GetDeviceOrientation ();
		deltaRotation = Quaternion.Inverse(sensorTransform.rotation) * arTransform.rotation;
		Debug.Log ("Cameras aligned: " + deltaRotation);
	}


	public void Subscribe(ARClient client) {
		if (!arClientScripts.Contains (client)) {
			arClientScripts.Add (client);
		}
	}

	public void Unsubscribe(ARClient client) {
		if (arClientScripts.Contains (client)) {
			arClientScripts.Remove (client);
		}
	}
}
