using UnityEngine;
using System.Collections;

public class SmoothCameraFollow : MonoBehaviour {

    public Transform target;

	// Update is called once per frame
	void Update () {
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * 16f);
        transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime * 4f);
	}
}
