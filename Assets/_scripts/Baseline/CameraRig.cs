using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform targetTransform;


    void Start()
    {
        // find each camera - they are all childrean of this game object
        var m = transform.Find("Main Camera");
        var x = transform.Find("X_Camera");
        //var y = transform.Find("Y_Camera"); // may create this camera later...
        var z = transform.Find("Z_Camera");

        // set up each camera with values
        SetupChildCamera(m, 200f, 100f, Vector3.one);
        SetupChildCamera(x, 100f, 0f, Vector3.right);
        //SetupChildCamera(y, 200f, 100f); // may create this later
        SetupChildCamera(z, 100f, 0f, Vector3.forward);
    }

    /// <summary>
    /// Setup a child camera of this game object
    /// </summary>
    /// <param name="t"></param>
    /// <param name="distance"></param>
    /// <param name="height"></param>
    /// <param name="fixedLookDirection"></param>
    private void SetupChildCamera(Transform t, float distance, float height, Vector3? fixedLookDirection = null)
    {
        var t_script = t.GetComponent<CameraFollowObject>();
        // set target, distance and height
        //t_script.targetTransform = this.targetTransform;
        t_script.followDistance = distance;
        t_script.followHeight = height;
        t_script.useFixedLookDirection = fixedLookDirection != null;
        t_script.fixedLookDirection = fixedLookDirection != null ? fixedLookDirection.Value : Vector3.one;
    }
}
