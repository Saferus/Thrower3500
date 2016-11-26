using UnityEngine;
using UnityEngine.Networking;

    public class PlayerController : NetworkBehaviour
    {
    public float speed = 1;
    public GameObject trajectory;
    public GameObject trajectoryInstance;
    public float trajectoryScale;

    private Rigidbody rb;
    private bool startInputOnObject;
    private Vector2 startPos;
    private Vector2 force;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.green;
    }

    // Update is called once per frame
    void Update ()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        //if (startInputOnObject)
        if (true)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.touches[0];
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            OnPress(touch.position);
                            break;
                        case TouchPhase.Moved:
                            OnDrag(touch.position);
                            break;
                        case TouchPhase.Ended:
                            OnRelease(touch.position);
                            break;
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    OnPress(Input.mousePosition);
                    return;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    OnRelease(Input.mousePosition);
                    return;
                }
                if (Input.GetMouseButton(0))
                {
                    OnDrag(Input.mousePosition);
                    return;
                }
            }
        }
    }

    private void OnPress(Vector3 pos)
    {
        startPos = new Vector3(pos.x, pos.y, pos.z);
        CreateTrajectory();
    }

    private void OnDrag(Vector3 pos)
    {
        RescaleTrajectory(pos);
    }

    private void OnRelease(Vector3 pos)
    {
        rb.AddForce(new Vector3(startPos.x - pos.x, 0, startPos.y - pos.y) * speed);
        startInputOnObject = false;
        DeleteTrajectory();
    }

    private void CreateTrajectory()
    {
        Vector3 pos = new Vector3(rb.position.x, rb.position.y, rb.position.z);
        Quaternion rot = Quaternion.AngleAxis(Mathf.Cos(pos.x / (Mathf.Sqrt(pos.x * pos.x + pos.y * pos.y + pos.z * pos.z))), Vector3.left);
        trajectoryInstance = (GameObject) Instantiate(trajectory, pos, rot);
        RescaleTrajectory(pos);
    }

    private static float TRAJECTORY_START_ANGLE = Mathf.PI / 2;
    private float trajectoryAngle = TRAJECTORY_START_ANGLE;

    private void RescaleTrajectory(Vector3 pos)
    {
        float delta = Mathf.Sqrt(Mathf.Pow((pos.x - startPos.x), 2) + Mathf.Pow((pos.y - startPos.y), 2));
        float angle = 0;
        if (delta > 0)
            angle = Mathf.Acos((pos.y - startPos.y) / delta);
        if (pos.x < startPos.x)
            angle *= -1;
        trajectoryInstance.transform.localScale = new Vector3(delta * trajectoryScale, trajectoryScale, trajectoryScale);
        trajectoryInstance.transform.Rotate(Vector3.down * (trajectoryAngle - angle) * 180 / Mathf.PI);
        trajectoryAngle = angle;
    }

    private void DeleteTrajectory()
    {
        trajectoryAngle = TRAJECTORY_START_ANGLE;
        Destroy(trajectoryInstance);
    }

    private void OnMouseOver()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        startInputOnObject = true;
    }
}
