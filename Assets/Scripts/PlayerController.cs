using UnityEngine;
using UnityEngine.Networking;

    public class PlayerController : NetworkBehaviour
    {
    public float speed = 1;

    private Rigidbody rb;
    private bool startInputOnObject;
    private Vector2 startPos;

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
                            startPos = touch.position;
                            break;
                        case TouchPhase.Ended:
                            rb.AddForce(new Vector3(startPos.x - touch.position.x, 0, startPos.y - touch.position.y) * speed);
                            startInputOnObject = false;
                            break;
                    }
                }
            }
        }
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
