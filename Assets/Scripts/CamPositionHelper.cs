using UnityEngine;
using System.Collections;

public class CamPositionHelper : MonoBehaviour {

    private Vector3 startPosition;
    private Quaternion startRotation;

	// Use this for initialization
	void Start () {
        startPosition = transform.position;
        startRotation = transform.rotation;
	}
	
	public void ResetTransform () {
        transform.position = startPosition;
        transform.rotation = startRotation;
    }
}
