using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    #region FIELDS

    public GameObject Camera;
    public float Speed = 5f;
    public float SpeedMultiplier = 2f;
    private float SpeedMult = 1f;
    public float AltitudeSpeed = 0.5f;
    public float TurnSpeed = 10f;
    public float ZoomSpeed = 10f;
    private InputActions inputActions;
    public float TransSmoothInputSpeed = 0.2f;
    public float AltSmoothInputSpeed = 0.2f;
    public float YawSmoothInputSpeed = 0.2f;
    public float PitchSmoothInputSpeed = 0.2f;
    public float ZoomSmoothInputSpeed = 0.2f;
    public float anchorOffsst = 1f;
    public Vector2 currTransInputVel, TransSmoothInputVal, currRotInputVal, RotSmoothInputVal;
    public float currAltInputVel, AltSmoothInputVal, currYawInputVel, YawSmoothInput, currPitchInputVel, PitchSmoothInput, currZoomInputVel = 1f, ZoomSmoothInputVal;

    #endregion FIELDS

    #region UNITY METHODS

    public void Awake()
    {
        inputActions = new InputActions();
        inputActions.CameraMovement.TransversePlanePanning.performed += ctx => Transverse(ctx.ReadValue<Vector2>());
        inputActions.CameraMovement.Attitude.performed += ctx => Altitude(ctx.ReadValue<float>());
        inputActions.CameraMovement.Yaw.performed += ctx => Yaw(ctx.ReadValue<float>());
        inputActions.CameraMovement.Pitch.performed += ctx => Pitch(ctx.ReadValue<float>());
        inputActions.CameraMovement.Zoom.performed += ctx => Zoom(ctx.ReadValue<float>());
        inputActions.CameraMovement.Rotation.performed += ctx => rotation(ctx.ReadValue<Vector2>());
        //Camera.transform.parent = null;
    }

    public void OnEnable()
    {
        inputActions.Enable();
    }

    public void OnDisable()
    {
        inputActions.Disable();
    }

    public void Update()
    {
        //CamAnchor.transform.localPosition = Target.transform.position - new Vector3(0, 0, anchorOffsst);
        if (Input.GetKey(KeyCode.LeftShift))
        {
            SpeedMult = SpeedMultiplier;
        }
        else
        {
            SpeedMult = 1f;
        }
        Vector2 movementInput = inputActions.CameraMovement.TransversePlanePanning.ReadValue<Vector2>();
        Transverse(movementInput);
        float attitudeInput = inputActions.CameraMovement.Attitude.ReadValue<float>();
        Altitude(attitudeInput);
        /* float rotationInput = inputActions.CameraMovement.Yaw.ReadValue<float>();
         Yaw(rotationInput);
         float pitchInput = inputActions.CameraMovement.Pitch.ReadValue<float>();
         Pitch(pitchInput);*/
        Vector2 rotInput = inputActions.CameraMovement.Rotation.ReadValue<Vector2>();
        rotation(rotInput);
        //float zoomInput = inputActions.CameraMovement.Zoom.ReadValue<float>();
        //Zoom(zoomInput);
        //UpdateCamPos();
    }

    #endregion UNITY METHODS

    #region METHODS

    public void Transverse(Vector2 direction)
    {
        currTransInputVel = Vector2.SmoothDamp(currTransInputVel, direction, ref TransSmoothInputVal, TransSmoothInputSpeed);
        //Vector3 movement = Vector3.forward * currTransInputVel.y * Speed * SpeedMult * Time.deltaTime;
        //movement += Vector3.right * currTransInputVel.x * Speed * SpeedMult * Time.deltaTime;
        Vector3 movement = Vector3.ProjectOnPlane(Camera.transform.forward, Vector3.up).normalized * currTransInputVel.y * Speed * SpeedMult * Time.deltaTime;
        movement += Vector3.ProjectOnPlane(Camera.transform.right, Vector3.up).normalized * currTransInputVel.x * Speed * SpeedMult * Time.deltaTime;

        //movement = Vector3.ProjectOnPlane(movement, Vector3.up);
        //movement = movement.normalized;
        Camera.transform.Translate(movement, Space.World);
        //Debug.Log("X: " + Camera.transform.position.x + " Y: " + Camera.transform.position.y + " Z: " + Camera.transform.position.z);
        //float x = Mathf.Clamp(Camera.transform.position.x, -10f, 0f);
        Camera.transform.position = new Vector3(Mathf.Clamp(Camera.transform.position.x, -5f, 65f), Mathf.Clamp(Camera.transform.position.y, 1, 35f), Mathf.Clamp(Camera.transform.position.z, -5f, 35f));
    }

    public void rotation(Vector2 rot)
    {
        currRotInputVal = Vector2.SmoothDamp(currRotInputVal, rot, ref RotSmoothInputVal, YawSmoothInputSpeed);
        //Vector3 movement = Target.transform.up * currRotInputVal.y * Speed * SpeedMult * Time.deltaTime;
        //movement += Target.transform.right * currRotInputVal.x * Speed * SpeedMult * Time.deltaTime;
        //movement = movement.normalized * anchorOffsst;
        //CamAnchor.transform.Translate(movement, Space.World);

        float xrotation = currRotInputVal.x * TurnSpeed * SpeedMult * Time.deltaTime;
        float yrotation = -currRotInputVal.y * TurnSpeed * SpeedMult * Time.deltaTime;
        //Debug.Log("X: " + xrotation + " Y: " + yrotation);
        Camera.transform.Rotate(yrotation, xrotation, 0, Space.Self);
        //float x = Camera.transform.rotation.eulerAngles.x < 180f ? Mathf.Clamp(Camera.transform.rotation.eulerAngles.x, 0f, 175f) : Mathf.Clamp(Camera.transform.rotation.eulerAngles.x, 185f, 360f);
        //float y = Camera.transform.rotation.eulerAngles.y < 180f ? Mathf.Clamp(Camera.transform.rotation.eulerAngles.y, 0f, 175f) : Mathf.Clamp(Camera.transform.rotation.eulerAngles.y, 185f, 360f);
        //Debug.Log("X: " + Camera.transform.rotation.eulerAngles.x + " Y: " + Camera.transform.rotation.eulerAngles.y);
        Camera.transform.rotation = Quaternion.Euler(Camera.transform.rotation.eulerAngles.x, Camera.transform.rotation.eulerAngles.y, 0);
        //Target.transform.rotation = Quaternion.Euler(Target.transform.rotation.eulerAngles.x, Target.transform.rotation.eulerAngles.y, 0);
        //rotation = currRotInputVal.y * TurnSpeed * Time.deltaTime;
        //Camera.transform.Rotate(-rotation, 0, 0, Space.World);
    }

    public void Altitude(float Alt)
    {
        currAltInputVel = Mathf.SmoothDamp(currAltInputVel, Alt, ref AltSmoothInputVal, AltSmoothInputSpeed);

        float movement = currAltInputVel * AltitudeSpeed * Time.deltaTime;
        Camera.transform.Translate(0, movement, 0, Space.World);
        //Camera.transform.position = new Vector3(Camera.transform.position.x, Mathf.Clamp(Camera.transform.position.y, 1f, 10f), Camera.transform.position.z);
    }

    public void Yaw(float rot)
    {
        currYawInputVel = Mathf.SmoothDamp(currYawInputVel, rot, ref YawSmoothInput, YawSmoothInputSpeed);
        float rotation = currYawInputVel * TurnSpeed * Time.deltaTime;

        Camera.transform.Rotate(0, -rotation, 0, Space.Self);
    }

    public void Pitch(float rot)
    {
        currPitchInputVel = Mathf.SmoothDamp(currPitchInputVel, rot, ref PitchSmoothInput, PitchSmoothInput);
        float rotation = currPitchInputVel * TurnSpeed * Time.deltaTime;

        Camera.transform.Rotate(-rotation, 0, 0, Space.Self);
    }

    public void Zoom(float zoom)
    {
        //Debug.Log("Zooming");
        currZoomInputVel = Mathf.SmoothDamp(currZoomInputVel, zoom, ref ZoomSmoothInputVal, ZoomSmoothInputSpeed);
        float movement = zoom * ZoomSpeed * Time.deltaTime;
        Vector3 camZoom = Camera.transform.forward * movement;
        Camera.transform.position += camZoom;
        //anchorOffsst += movement;
    }

    public void UpdateCamPos()
    {
        //anchorOffsst = Mathf.Clamp(anchorOffsst, 1f, 10f);
        //CamAnchor.transform.localPosition = Vector3.Normalize(Target.transform.position - Camera.transform.position) * anchorOffsst;
        //Camera.transform.position = Vector3.Lerp(Camera.transform.position, CamAnchor.transform.position, TurnSpeed * Time.deltaTime);
        //Camera.transform.LookAt(Target.transform);
    }

    #endregion METHODS
}