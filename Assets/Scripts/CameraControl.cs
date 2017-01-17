//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CameraControl : MonoBehaviour {


	public float dragSpeed = 2;
	//public float angleSpeed = 20;
	public GameObject menuPanel;

	[Header("UI Labels for:")]
	public Text fov;
	public Text height;
	public Text angleX;
	public Text angleY;

	// Internal
	Vector3 dragOrigin;
	Vector3 AngleOrigin;
	Transform cameraTransform;
	Quaternion camRotation;
	float cameraHeight;
	Vector3 pos;


	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	void Start ()
	{
//	    menuPanel = Instantiate(menuPanel, Vector3.zero, Quaternion.identity);

        cameraTransform = Camera.main.transform;
	    fov = cameraTransform.transform.FindChild("InfoPanel").FindChild("FOV").GetComponent<Text>();

        fov.text = Mathf.Ceil(Camera.main.fieldOfView).ToString ();
		height.text = Mathf.Ceil(cameraHeight).ToString ();
		angleX.text = Mathf.Ceil(cameraTransform.localEulerAngles.x).ToString ();
		angleY.text = Mathf.Ceil(cameraTransform.localEulerAngles.y).ToString ();

	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	void Update () 
	{


		if (Input.GetMouseButtonDown(0))
		{
			dragOrigin = Input.mousePosition;
			return;
		}

		/*if (Input.GetMouseButtonDown(1))
		{
			AngleOrigin = Input.mousePosition;
			return;
		}*/



		if (menuPanel.activeSelf) return;


		ChangeZoom (Input.GetAxis ("Mouse ScrollWheel"));

		/*if (Input.GetMouseButton(1))
			{
			  if ((Input.mousePosition - AngleOrigin).x > 1)  SetAngleY(cameraTransform.rotation.eulerAngles.x + Camera.main.ScreenToViewportPoint (Input.mousePosition - AngleOrigin).x * dragSpeed * angleSpeed);
			  if ((Input.mousePosition - AngleOrigin).y > 1)  SetAngleX(cameraTransform.rotation.eulerAngles.y - Camera.main.ScreenToViewportPoint (Input.mousePosition - AngleOrigin).y * dragSpeed * -angleSpeed);
			}*/


		if (Input.GetMouseButton (0)) {
			pos = Camera.main.ScreenToViewportPoint (Input.mousePosition - dragOrigin);

			cameraTransform.Translate (Vector3.left * pos.x * dragSpeed, Space.Self);  

			camRotation = cameraTransform.rotation;
			cameraTransform.rotation = new Quaternion (0, camRotation.y, camRotation.z, camRotation.w);
			cameraTransform.Translate (Vector3.back * pos.y * dragSpeed, Space.Self); 
			cameraTransform.rotation = camRotation;

			cameraTransform.position = new Vector3 (cameraTransform.position.x, cameraHeight, cameraTransform.position.z);
		}


	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	public void ShowMenu ()
	{
		menuPanel.SetActive (!menuPanel.activeSelf);
	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	public void ChangeZoom (float _delta)
	{
		cameraTransform.Translate(Vector3.forward * _delta * dragSpeed * 10, Space.Self);
		cameraHeight = cameraTransform.position.y;
		height.text = Mathf.Ceil(cameraHeight).ToString ();
	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	public void SetZoom (float _zoom)
	{
		cameraTransform.Translate(Vector3.forward * _zoom, Space.Self);
		cameraHeight = cameraTransform.position.y;
		height.text = Mathf.Ceil(cameraHeight).ToString ();
	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	public void ChangeFOV (float _fov)
	{
		Camera.main.fieldOfView = _fov;
		fov.text = Mathf.Ceil(_fov).ToString ();
	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	public void ChangeHeight (float _height)
	{
		cameraTransform.position = new Vector3 (cameraTransform.position.x, _height, cameraTransform.position.z);
		cameraHeight = _height;
		height.text = Mathf.Ceil(cameraHeight).ToString ();
	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	public void SetAngleX (float _angle)
	{
		cameraTransform.localEulerAngles = new Vector3 (_angle, cameraTransform.rotation.eulerAngles.y, cameraTransform.rotation.eulerAngles.z);
		angleX.text = Mathf.Ceil(_angle).ToString ();
	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	public void SetAngleY (float _angle)
	{
		cameraTransform.localEulerAngles = new Vector3 (cameraTransform.rotation.eulerAngles.x, _angle, cameraTransform.rotation.eulerAngles.z);
		angleY.text = Mathf.Ceil(_angle).ToString ();
	}

}
