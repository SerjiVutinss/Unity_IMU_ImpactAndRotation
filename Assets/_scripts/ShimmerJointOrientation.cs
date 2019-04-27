using UnityEngine;
using ShimmerRT.models;
using UnityEngine.UI;

// Orient the object to match that of the Shimmer IMU device.
// 'The Transform' referred to in the comments of this class refers to the transform of 
// the object to which this script is attached (i.e. the 'Joint' object in the Unity scene)
public class ShimmerJointOrientation : MonoBehaviour
{
    #region User adjustable variables
    // value used to determine whether an impact has occurred
    // TODO: should be possible for the user to set this in the UI
    public float impactThreshold = 1.0f;
    // allow adjustment of transform to desired rotation
    // TODO: should be possible for the user to set this in the UI, not just in inspector
    public Vector3 offset = new Vector3(0, 0, 0);
    #endregion

    #region References to Unity GameObjects and Scripts
    // reference to a shimmer device - live data can be retrieved from this device and applied to the transform
    private ShimmerDevice shimmerDevice;
    // reference to the SceneManager which controls most of the UI
    public GameObject SceneManager;
    // UI element used to inform user when an impact has occurred
    public Text txtImpact;
    #endregion

    #region Private script variables
    // variable used in impact checking to compare a model to the previous one applied to the transform
    private Shimmer3DModel lastShimmerModel = null;
    #endregion

    #region Playback
    private bool isLive = true;
    private double firstModelTime; // first model time - timestamp of first model in playback list
    private double timeSinceLastModelApplied; // time since last model applied
    #endregion

    #region reference variables for roll and anti-yaw
    private float _referenceRoll = 0.0f;
    private Quaternion _antiYaw = Quaternion.identity;
    #endregion

    void Start()
    {
        // get the script from the ShimmerDevice object available through the reference to the SceneManager
        shimmerDevice = SceneManager.GetComponent<ShimmerDevice>();
    }

    // main logic used to decide what to do with the transform, i.e. live streaming or playback
    // TODO: tidy up the logic in this method if needed
    private void Update()
    {
        // handle live streaming from shimmer
        if (!shimmerDevice.IsPlayback)
        {
            isLive = true;
            // if data is available, use it
            if (shimmerDevice.Queue.Count > 0)
            {
                var s = shimmerDevice.Queue.Dequeue();
                // see if there was an 'impact' between this data and the last received data
                if (lastShimmerModel != null)
                {
                    //Debug.Log("Checking Impact");
                    if (CheckImpact(s))
                    {
                        //txtImpact.text = "--IMPACT--" + Time.time;
                        Debug.Log("--IMPACT--" + Time.time);
                    }
                }
                UpdateTransform(s);
                lastShimmerModel = s;
            }
        } // end handle live streaming
        else // handle playback
        {
            // handle playback from memory if data is present and playback mode is enabled
            Shimmer3DModel s;
            if (shimmerDevice.RecordList.Count > 0 && shimmerDevice.RecordList != null && shimmerDevice.IsPlayback == true)
            {
                // get the first model from the list
                s = shimmerDevice.RecordList[0];
                // check whether this is the first model in the playback list
                // - if it is the first model, isLive will still be set to true
                if (isLive)
                {
                    // first frame, set these variables
                    isLive = false;
                    // firstModelTime is set as the timestamp of the first model in the list, all other models
                    // will be applied to the transform relative to this first model
                    firstModelTime = s.Timestamp_CAL;

                    // update the model's transform accordingly
                    UpdateTransform(s);
                    // reset the time since last model applied, since one was just applied
                    timeSinceLastModelApplied = 0.0;
                    // and remove the 'used' model from the list
                    shimmerDevice.RecordList.Remove(s);
                }

                double timeToWait = 0.0;
                if (!isLive)
                {
                    timeToWait = (s.Timestamp_CAL - firstModelTime) / 1000;
                }

                // check if the next model should be applied to the transform
                // i.e. wait the correct amount of time to display the next frame
                if (timeSinceLastModelApplied >= timeToWait)
                {
                    // update the model's transform accordingly
                    UpdateTransform(s);
                    // and remove the 'used' model from the list
                    shimmerDevice.RecordList.Remove(s);
                }
                else
                {
                    // still waiting, increment the time since last frame with time between this 
                    timeSinceLastModelApplied += Time.deltaTime;
                }
            }
            else
            {
                // playback is either disabled or has ended, simply turn playback off to save on processing
                shimmerDevice.IsPlayback = false;
            }
        }
    }

    // apply a Shimmer model to the transform
    private void UpdateTransform(Shimmer3DModel s)
    {

        transform.parent.transform.eulerAngles = offset;

        transform.eulerAngles = new Vector3(
            (float)s.Axis_Angle_X_CAL,
            (float)s.Axis_Angle_Y_CAL,
            (float)s.Axis_Angle_Z_CAL
            );

        transform.localRotation = new Quaternion(
            -(float)s.Quaternion_2_CAL,
            -(float)s.Quaternion_0_CAL,
            (float)s.Quaternion_1_CAL,
            -(float)s.Quaternion_3_CAL);


        accelerometer = new Vector3(
            (float)s.Low_Noise_Accelerometer_Y_CAL,
            (float)s.Low_Noise_Accelerometer_Z_CAL,
            -(float)s.Low_Noise_Accelerometer_X_CAL);

        gyroscope = new Vector3(
            (float)s.Gyroscope_Y_CAL,
            (float)s.Gyroscope_Z_CAL,
            -(float)s.Gyroscope_X_CAL);
    }

    #region Impact 
    private bool CheckImpact(Shimmer3DModel s)
    {

        float dX = Mathf.Abs((float)(lastShimmerModel.Low_Noise_Accelerometer_X_CAL - s.Low_Noise_Accelerometer_X_CAL));
        float dY = Mathf.Abs((float)(lastShimmerModel.Low_Noise_Accelerometer_Y_CAL - s.Low_Noise_Accelerometer_Y_CAL));
        float dZ = Mathf.Abs((float)(lastShimmerModel.Low_Noise_Accelerometer_Z_CAL - s.Low_Noise_Accelerometer_Z_CAL));

        //Debug.Log(dX);
        //Debug.Log(dY);
        //Debug.Log(dZ);

        if (dX > impactThreshold)
        {
            return true;
        }
        if (dY > impactThreshold)
        {
            return true;
        }
        if (dZ > impactThreshold)
        {
            return true;
        }
        return false;
    }
    #endregion

    #region Legacy accelerometer and gyro variables
    private Vector3 accelerometer;
    private Vector3 gyroscope;
    #endregion
    #region Legacy code from Myo code samples
    // Compute the angle of rotation clockwise about the forward axis relative to the provided zero roll direction.
    // As the armband is rotated about the forward axis this value will change, regardless of which way the
    // forward vector of the Myo is pointing. The returned value will be between -180 and 180 degrees.
    //float rollFromZero(Vector3 zeroRoll, Vector3 forward, Vector3 up)
    //{
    //    // The cosine of the angle between the up vector and the zero roll vector. Since both are
    //    // orthogonal to the forward vector, this tells us how far the Myo has been turned around the
    //    // forward axis relative to the zero roll vector, but we need to determine separately whether the
    //    // Myo has been rolled clockwise or counterclockwise.
    //    float cosine = Vector3.Dot(up, zeroRoll);

    //    // To determine the sign of the roll, we take the cross product of the up vector and the zero
    //    // roll vector. This cross product will either be the same or opposite direction as the forward
    //    // vector depending on whether up is clockwise or counter-clockwise from zero roll.
    //    // Thus the sign of the dot product of forward and it yields the sign of our roll value.
    //    Vector3 cp = Vector3.Cross(up, zeroRoll);
    //    float directionCosine = Vector3.Dot(forward, cp);
    //    float sign = directionCosine < 0.0f ? 1.0f : -1.0f;

    //    // Return the angle of roll (in degrees) from the cosine and the sign.
    //    return sign * Mathf.Rad2Deg * Mathf.Acos(cosine);
    //}

    // Compute a vector that points perpendicular to the forward direction,
    // minimizing angular distance from world up (positive Y axis).
    // This represents the direction of no rotation about its forward axis.
    //Vector3 computeZeroRollVector(Vector3 forward)
    //{
    //    Vector3 antigravity = Vector3.up;
    //    Vector3 m = Vector3.Cross(shimmerDevice.transform.forward, antigravity);
    //    Vector3 roll = Vector3.Cross(m, shimmerDevice.transform.forward);

    //    return roll.normalized;
    //}

    // Adjust the provided angle to be within a -180 to 180.
    //float normalizeAngle(float angle)
    //{
    //    if (angle > 180.0f)
    //    {
    //        return angle - 360.0f;
    //    }
    //    if (angle < -180.0f)
    //    {
    //        return angle + 360.0f;
    //    }
    //    return angle;
    //}

    // Extend the unlock if ThalmcHub's locking policy is standard, and notifies the given myo that a user action was
    // recognized.
    //void ExtendUnlockAndNotifyUserAction(ThalmicMyo myo)
    //{
    //    ThalmicHub hub = ThalmicHub.instance;

    //    if (hub.lockingPolicy == LockingPolicy.Standard)
    //    {
    //        myo.Unlock(UnlockType.Timed);
    //    }

    //    myo.NotifyUserAction();
    //}

    //void ResetTransform()
    //{
    //    // Current zero roll vector and roll value.
    //    Vector3 zeroRoll = computeZeroRollVector(shimmerDevice.transform.forward);
    //    float roll = rollFromZero(zeroRoll, shimmerDevice.transform.forward, shimmerDevice.transform.up);

    //    // The relative roll is simply how much the current roll has changed relative to the reference roll.
    //    // adjustAngle simply keeps the resultant value within -180 to 180 degrees.
    //    float relativeRoll = normalizeAngle(roll - _referenceRoll);

    //    // antiRoll represents a rotation about the myo Armband's forward axis adjusting for reference roll.
    //    Quaternion antiRoll = Quaternion.AngleAxis(relativeRoll, shimmerDevice.transform.forward);

    //    // Here the anti-roll and yaw rotations are applied to the myo Armband's forward direction to yield
    //    // the orientation of the joint.
    //    transform.rotation = _antiYaw * antiRoll * Quaternion.LookRotation(shimmerDevice.transform.forward);

    //    // Mirror the rotation around the XZ plane in Unity's coordinate system (XY plane in Myo's coordinate
    //    // system). This makes the rotation reflect the arm's orientation, rather than that of the Myo armband.
    //    transform.rotation = new Quaternion(transform.localRotation.x,
    //                                        -transform.localRotation.y,
    //                                        transform.localRotation.z,
    //                                        -transform.localRotation.w);
    //}

    #endregion
}
