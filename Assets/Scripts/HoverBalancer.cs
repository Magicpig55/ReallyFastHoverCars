using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class HoverBalancer : MonoBehaviour {

    Collider col;
    Rigidbody rig;

    Vector3 curNormal = Vector3.up;
    Vector3[] points;
    int pointCount;

    public float DownForceSpeed = 1.0f;
    public float TurnSpeed = 10f;
    public float MoveSpeed = 10f;
    public float ReachDistance = 1f;

    public float TopSpeed {
        get {
            return ((MoveSpeed / rig.drag) - Time.fixedDeltaTime * MoveSpeed) / rig.mass;
        }
    }
    public float SpeedPercent {
        get {
            return (rig.velocity.magnitude / TopSpeed);
        }
    }
    public float CurrentSpeed {
        get {
            return (rig.velocity.magnitude);
        }
    }

    void Awake () {
        rig = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        int count = 0;
        points = new Vector3[transform.childCount];
        foreach(Transform child in transform) {
            if(child.name.StartsWith("raytrace")) {
                points[count] = child.localPosition;
                count++;
            }
        }
        pointCount = count;
    }
    void FixedUpdate () {
        RaycastHit[] hits = new RaycastHit[pointCount];
        Vector3 norm = Vector3.zero;
        float dist = 0;
        bool allHit = true;
        int hitCount = 0;
        for(int i = 0; i < hits.Length; i++) {
            if (Physics.Raycast(transform.TransformPoint(points[i]), -curNormal, out hits[i])) {
                if(hitCount == 0) {
                    dist = hits[i].distance;
                } else {
                    dist += hits[i].distance;
                    dist /= 2;
                }
                norm += hits[i].normal;
                hitCount++;
            } else {
                allHit = false;
            }
        }
        if (hitCount != 0) {
            norm /= hitCount;
            curNormal = Vector3.Slerp(curNormal, norm, Time.fixedDeltaTime * 10f);
            Vector3 curForward = Vector3.Cross(transform.right, curNormal);
            Quaternion grndTilt = Quaternion.LookRotation(curForward, curNormal);
            float dir = Input.GetAxis("Horizontal") * TurnSpeed * Time.fixedDeltaTime;
            rig.rotation = grndTilt * Quaternion.Euler(0, dir, 0);
            rig.AddForce(curNormal * DownForceSpeed * 10f * Mathf.Clamp01(1 - (dist / ReachDistance)));
            rig.AddForce(-curNormal * DownForceSpeed);
        } else {
            float dir = Input.GetAxis("Horizontal") * TurnSpeed * Time.fixedDeltaTime;
            Vector3 curForward = Vector3.Cross(transform.right, curNormal);
            rig.rotation = Quaternion.LookRotation(curForward, curNormal) * Quaternion.Euler(0, dir, 0);
            rig.AddForce(-Vector3.up * 800f);
        }
        rig.AddForce(transform.forward * MoveSpeed * Input.GetAxis("Vertical"));
    }
    void OnDrawGizmosSelected () {
        Gizmos.color = Color.black;
        foreach (Transform child in transform) {
            if (child.name.StartsWith("raytrace")) {
                Gizmos.DrawLine(child.position, child.position + transform.TransformDirection(-Vector3.up));
            }
        }
        Gizmos.DrawLine(transform.position, transform.TransformPoint(curNormal * 5f));
    }

    static Vector3 AverageVector3(Vector3[] vecs) {
        Vector3 average = Vector3.zero;
        foreach(Vector3 vec in vecs) {
            average += vec;
        }
        return average / vecs.Length;
    }

    //Get an average (mean) from more then two quaternions (with two, slerp would be used).
    //Note: this only works if all the quaternions are relatively close together.
    //Usage: 
    //-Cumulative is an external Vector4 which holds all the added x y z and w components.
    //-newRotation is the next rotation to be added to the average pool
    //-firstRotation is the first quaternion of the array to be averaged
    //-addAmount holds the total amount of quaternions which are currently added
    //This function returns the current average quaternion
    public static Quaternion AverageQuaternion(ref Vector4 cumulative, Quaternion newRotation, Quaternion firstRotation, int addAmount) {

        float w = 0.0f;
        float x = 0.0f;
        float y = 0.0f;
        float z = 0.0f;

        //Before we add the new rotation to the average (mean), we have to check whether the quaternion has to be inverted. Because
        //q and -q are the same rotation, but cannot be averaged, we have to make sure they are all the same.
        if (!AreQuaternionsClose(newRotation, firstRotation)) {

            newRotation = InverseSignQuaternion(newRotation);
        }

        //Average the values
        float addDet = 1f / (float)addAmount;
        cumulative.w += newRotation.w;
        w = cumulative.w * addDet;
        cumulative.x += newRotation.x;
        x = cumulative.x * addDet;
        cumulative.y += newRotation.y;
        y = cumulative.y * addDet;
        cumulative.z += newRotation.z;
        z = cumulative.z * addDet;

        //note: if speed is an issue, you can skip the normalization step
        return NormalizeQuaternion(x, y, z, w);
    }

    public static Quaternion NormalizeQuaternion(float x, float y, float z, float w) {

        float lengthD = 1.0f / (w * w + x * x + y * y + z * z);
        w *= lengthD;
        x *= lengthD;
        y *= lengthD;
        z *= lengthD;

        return new Quaternion(x, y, z, w);
    }

    //Changes the sign of the quaternion components. This is not the same as the inverse.
    public static Quaternion InverseSignQuaternion(Quaternion q) {

        return new Quaternion(-q.x, -q.y, -q.z, -q.w);
    }

    //Returns true if the two input quaternions are close to each other. This can
    //be used to check whether or not one of two quaternions which are supposed to
    //be very similar but has its component signs reversed (q has the same rotation as
    //-q)
    public static bool AreQuaternionsClose(Quaternion q1, Quaternion q2) {

        float dot = Quaternion.Dot(q1, q2);

        if (dot < 0.0f) {

            return false;
        } else {

            return true;
        }
    }
}
