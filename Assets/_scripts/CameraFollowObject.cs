using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//put this component on camera
//drag target object (ball) onto the TargetObject public variable
public class CameraFollowObject : MonoBehaviour
{
    public Transform targetTransform;
    public float followDistance = 5f;
    public float followHeight = 2f;
    public bool smoothedFollow = false;         //toggle this for hard or smoothed follow
    public float smoothSpeed = 5f;
    public bool useFixedLookDirection = false;      //optional different camera mode... fixed look direction
    public Vector3 fixedLookDirection = Vector3.one;

    private void Awake()
    {

        targetTransform = gameObject.GetComponentInParent<CameraRig>().targetTransform;
    }

    // Use this for initialization
    void Start()
    {
        //do something when game object is activated .. if you want to
    }

    // Update is called once per frame
    void Update()
    {
        //get a vector pointing from camera towards the ball

        Vector3 lookToward = targetTransform.position - transform.position;
        if (useFixedLookDirection)
            lookToward = fixedLookDirection;


        //make it stay a fixed distance behind ball
        Vector3 newPos;
        newPos = targetTransform.position - lookToward.normalized * followDistance;
        newPos.y = targetTransform.position.y + followHeight;

        if (!smoothedFollow)
        {
            transform.position = newPos;    //move exactly to follow target
        }
        else   //  smoothed / soft follow
        {
            transform.position += (newPos - transform.position) * Time.deltaTime * smoothSpeed;
        }

        //re- calculate look direction (dont' do this line if you want to lag the look a little
        lookToward = targetTransform.position - transform.position;

        //make this camera look at target
        transform.forward = lookToward.normalized;
    }
}
