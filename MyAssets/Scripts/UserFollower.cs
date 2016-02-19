using UnityEngine;
using System.Collections;

public class UserFollower : MonoBehaviour {

	private Transform userTransform;

	// Use this for initialization
	void Start () {
		userTransform = GameObject.FindWithTag ("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3 (userTransform.position.x, transform.position.y, userTransform.position.z);
		transform.rotation = Quaternion.Euler (transform.rotation.eulerAngles.x, userTransform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
	}
}
