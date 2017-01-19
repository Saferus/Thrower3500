using System;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
	public float dragSpeed = 2;
    public float androidZoomSpeed = 0.5f;

    // Internal
    Transform cameraTransform;
    float cameraHeight;
    public bool isFixed;

    private float prevDistance = 0;
    private bool isAndroidDevice;

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void Start ()
	{
        cameraTransform = Camera.main.transform;
	    if (Application.platform == RuntimePlatform.Android)
	        isAndroidDevice = true;
	}

    private Touch prevTouch0;
    private Touch prevTouch1;
    private Vector2 prevPointsVector;
    private bool startMultitouch = false;
    public float pitchCoefficient = 0.1f;
    public float yawCoefficient = 100f;

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void Update ()
	{
	    if (isFixed)
	        return;

	    if (isAndroidDevice)
	    {
	        if (Input.touchCount > 1)
	        {
	            float currentDistance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
	            Vector2 currPointsVector = (Input.touches[0].position - Input.touches[1].position).normalized;
	            if (!startMultitouch)
	            {
	                startMultitouch = true;
	                prevTouch0 = Input.touches[0];
	                prevTouch1 = Input.touches[1];
	                prevDistance = currentDistance;
	                prevPointsVector = currPointsVector;
	                return;
	            }
	            float currentDZoom = currentDistance - prevDistance;

	            float dy0 = Input.touches[0].position.y - prevTouch0.position.y;
	            float dy1 = Input.touches[1].position.y - prevTouch1.position.y;


	            float currentDPitch = 0;
	            float currentDYaw = 0;

	            if (dy0 > 0 && dy1 > 0)
	            {
	                currentDPitch = Math.Min(dy0, dy1);
	            }
	            if (dy0 < 0 && dy1 < 0)
	            {
	                currentDPitch = Math.Max(dy0, dy1);
	            }

	            ChangeAngleX(pitchCoefficient * currentDPitch);
                ChangeAngleY(IsOnTheLeftSide(prevTouch0.position, prevTouch1.position, Input.touches[0].position) * yawCoefficient * ((float) Math.Acos(Vector2.Dot(currPointsVector, prevPointsVector))));

	            ChangeZoom(androidZoomSpeed * currentDZoom);
	            prevDistance = currentDistance;
	            prevPointsVector = currPointsVector;
	            prevTouch0 = Input.touches[0];
	            prevTouch1 = Input.touches[1];
	        }
	        else
	        {
	            startMultitouch = false;
                if (Input.touchCount == 1)
                { 
                    Vector3 translate = new Vector3(-1 * Input.touches[0].deltaPosition.x * dragSpeed, 0, -1 * Input.touches[0].deltaPosition.y * dragSpeed);
                    Quaternion camRotation = cameraTransform.rotation;
                    cameraTransform.rotation = new Quaternion(0, camRotation.y, camRotation.z, camRotation.w);
                    cameraTransform.Translate(translate, Space.Self);
                    cameraTransform.rotation = camRotation;

                    cameraTransform.position = new Vector3(cameraTransform.position.x, cameraHeight, cameraTransform.position.z);
                }
            }
	        return;
	    }

        ChangeZoom (Input.GetAxis ("Mouse ScrollWheel"));

		if (Input.GetMouseButton (0))
        {
            Vector3 translate;

            if (isAndroidDevice)
                translate = new Vector3(-1 * Input.touches[0].deltaPosition.x * dragSpeed, 0, -1 * Input.touches[0].deltaPosition.y * dragSpeed);
            else
                translate = new Vector3(-1 * Input.GetAxis("Mouse X") * dragSpeed, 0, -1 * Input.GetAxis("Mouse Y") * dragSpeed);

            Quaternion camRotation = cameraTransform.rotation;
			cameraTransform.rotation = new Quaternion (0, camRotation.y, camRotation.z, camRotation.w);
			cameraTransform.Translate(translate, Space.Self);
			cameraTransform.rotation = camRotation;

			cameraTransform.position = new Vector3 (cameraTransform.position.x, cameraHeight, cameraTransform.position.z);
		}
    }

    private static int IsOnTheLeftSide(Vector2 pointX, Vector2 pointY, Vector2 pointXnew)
    {
        if ((pointY.x - pointX.x) * (pointXnew.y - pointX.y) - (pointY.y - pointX.y) * (pointXnew.x - pointX.x) > 0)
            return -1;
        return 1;
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void ChangeZoom (float _delta)
	{
		cameraTransform.Translate(Vector3.forward * _delta * dragSpeed * 10, Space.Self);
		cameraHeight = cameraTransform.position.y;
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	public void SetZoom (float _zoom)
	{
		cameraTransform.Translate(Vector3.forward * _zoom, Space.Self);
		cameraHeight = cameraTransform.position.y;
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	public void ChangeFOV (float _fov)
	{
		Camera.main.fieldOfView = _fov;
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	public void ChangeHeight (float _height)
	{
		cameraTransform.position = new Vector3 (cameraTransform.position.x, _height, cameraTransform.position.z);
		cameraHeight = _height;
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	public void ChangeAngleX (float _angle)
    {
        SetEulerAngle(new Vector3 (cameraTransform.localRotation.eulerAngles.x + _angle, cameraTransform.localRotation.eulerAngles.y, cameraTransform.localRotation.eulerAngles.z));
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void ChangeAngleY(float _angle)
    {
        SetEulerAngle(new Vector3(cameraTransform.localRotation.eulerAngles.x, cameraTransform.localRotation.eulerAngles.y + _angle, cameraTransform.localRotation.eulerAngles.z));
    }

    private void SetEulerAngle(Vector3 angle)
    {
        if (float.IsNaN(angle.x) || float.IsNaN(angle.y) || float.IsNaN(angle.z))
            return;
        if (angle.x > 80)
        {
            angle.x = 80;
        }
        if (angle.x < -80)
        {
            angle.x = -80;
        }
        cameraTransform.localEulerAngles = angle;
    }
}
