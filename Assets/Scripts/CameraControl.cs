using UnityEngine;


public class CameraControl : MonoBehaviour
{
	public float dragSpeed = 2;
    public float androidZoomSpeed = 0.5f;

    // Internal
    Transform cameraTransform;
    float cameraHeight;
    public bool isFixed;

    private float initalDistance = 0;
    private bool isAndroidDevice;

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void Start ()
	{
        cameraTransform = Camera.main.transform;
	    if (Application.platform == RuntimePlatform.Android)
	        isAndroidDevice = true;
	}

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

                if (initalDistance == 0)
	            {
	                initalDistance = currentDistance;
	                return;
	            }

                ChangeZoom(androidZoomSpeed * (currentDistance - initalDistance));
	            initalDistance = currentDistance;
	        }
            else initalDistance = 0;
        }
        else
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
	public void SetAngleX (float _angle)
	{
		cameraTransform.localEulerAngles = new Vector3 (_angle, cameraTransform.rotation.eulerAngles.y, cameraTransform.rotation.eulerAngles.z);
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	public void SetAngleY (float _angle)
	{
		cameraTransform.localEulerAngles = new Vector3 (cameraTransform.rotation.eulerAngles.x, _angle, cameraTransform.rotation.eulerAngles.z);
	}
}
