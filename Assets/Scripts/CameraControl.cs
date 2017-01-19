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

	            float dx0 = Input.touches[0].position.x - prevTouch0.position.x;
	            float dx1 = Input.touches[1].position.x - prevTouch1.position.x;


	            float currentDPitch = 0;
	            float currentDYaw = 0;

	            if (dx0 > 0 && dx1 > 0)
	            {
	                currentDPitch = Math.Min(dx0, dx1);
	            }
	            if (dx0 < 0 && dx1 < 0)
	            {
	                currentDPitch = Math.Max(dx0, dx1);
	            }

	            ChangeAngleX(pitchCoefficient * currentDPitch);

//                ChangeAngleY(yawCoefficient * ((float) Math.Acos(Vector2.Dot(currPointsVector, prevPointsVector))));

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
        if (!float.IsNaN(_angle))
//            cameraTransform.rotation = new Quaternion(cameraTransform.rotation.x + _angle, cameraTransform.rotation.y, cameraTransform.rotation.z, cameraTransform.rotation.w);
        cameraTransform.localEulerAngles = new Vector3 (cameraTransform.localRotation.eulerAngles.x + _angle, cameraTransform.localRotation.eulerAngles.y, cameraTransform.localRotation.eulerAngles.z);
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void ChangeAngleY(float _angle)
    {
        if (!float.IsNaN(_angle))
//            cameraTransform.rotation = new Quaternion(cameraTransform.rotation.x + _angle, cameraTransform.rotation.y + _angle, cameraTransform.rotation.z, cameraTransform.rotation.w);
        cameraTransform.localEulerAngles = new Vector3(cameraTransform.localRotation.eulerAngles.x, cameraTransform.localRotation.eulerAngles.y + _angle, cameraTransform.localRotation.eulerAngles.z);
    }
//    private Vector2?[] oldTouchPositions = {
//        null,
//        null
//    };
//    private Vector2 oldTouchVector;
//    private float oldTouchDistance;
//
//    private void UpdateAndroid()
//    {
//        if (Input.touchCount == 0)
//        {
//            oldTouchPositions[0] = null;
//            oldTouchPositions[1] = null;
//        }
//        else if (Input.touchCount == 1)
//        {
//            if (oldTouchPositions[0] == null || oldTouchPositions[1] != null)
//            {
//                oldTouchPositions[0] = Input.GetTouch(0).position;
//                oldTouchPositions[1] = null;
//            }
//            else
//            {
//                Vector2 newTouchPosition = Input.GetTouch(0).position;
//
//                transform.position += transform.TransformDirection((Vector3)((oldTouchPositions[0] - newTouchPosition) * camera.orthographicSize / camera.pixelHeight * 2f));
//
//                oldTouchPositions[0] = newTouchPosition;
//            }
//        }
//        else
//        {
//            if (oldTouchPositions[1] == null)
//            {
//                oldTouchPositions[0] = Input.GetTouch(0).position;
//                oldTouchPositions[1] = Input.GetTouch(1).position;
//                oldTouchVector = (Vector2)(oldTouchPositions[0] - oldTouchPositions[1]);
//                oldTouchDistance = oldTouchVector.magnitude;
//            }
//            else
//            {
//                Vector2 screen = new Vector2(camera.pixelWidth, camera.pixelHeight);
//
//                Vector2[] newTouchPositions = {
//                    Input.GetTouch(0).position,
//                    Input.GetTouch(1).position
//                };
//                Vector2 newTouchVector = newTouchPositions[0] - newTouchPositions[1];
//                float newTouchDistance = newTouchVector.magnitude;
//
//                transform.position += transform.TransformDirection((Vector3)((oldTouchPositions[0] + oldTouchPositions[1] - screen) * camera.orthographicSize / screen.y));
//                transform.localRotation *= Quaternion.Euler(new Vector3(0, 0, Mathf.Asin(Mathf.Clamp((oldTouchVector.y * newTouchVector.x - oldTouchVector.x * newTouchVector.y) / oldTouchDistance / newTouchDistance, -1f, 1f)) / 0.0174532924f));
//                camera.orthographicSize *= oldTouchDistance / newTouchDistance;
//                transform.position -= transform.TransformDirection((newTouchPositions[0] + newTouchPositions[1] - screen) * camera.orthographicSize / screen.y);
//
//                oldTouchPositions[0] = newTouchPositions[0];
//                oldTouchPositions[1] = newTouchPositions[1];
//                oldTouchVector = newTouchVector;
//                oldTouchDistance = newTouchDistance;
//            }
//        }
//    }
}
