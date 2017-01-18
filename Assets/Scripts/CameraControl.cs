using UnityEngine;


public class CameraControl : MonoBehaviour
{
	public float dragSpeed = 2;

	// Internal
	Transform cameraTransform;
    float cameraHeight;
    public bool isFixed;

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void Start ()
	{
        cameraTransform = Camera.main.transform;
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	void Update ()
	{
	    if (isFixed)
	        return;

		ChangeZoom (Input.GetAxis ("Mouse ScrollWheel"));

		if (Input.GetMouseButton (0))
        {
			cameraTransform.Translate (Vector3.left * Input.GetAxis("Mouse X") * dragSpeed, Space.Self);

            Quaternion camRotation = cameraTransform.rotation;
			cameraTransform.rotation = new Quaternion (0, camRotation.y, camRotation.z, camRotation.w);
			cameraTransform.Translate (Vector3.back * Input.GetAxis("Mouse Y") * dragSpeed, Space.Self); 
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
