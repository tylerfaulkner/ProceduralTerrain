using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCam : MonoBehaviour
{
    [SerializeField]
    public float moveSpeed = 2.0f;
    [SerializeField]
    public float sensitivity = 20.0f;
    [SerializeField]
    public float maxVertical = 85.0f;
    [SerializeField]
    public float speedBoost = 2.0f;

    private Rigidbody body;

    float yaw = 0.0f;
    float pitch = 0.0f;

    private bool wDown;
    private bool shiftDown;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        wDown = Input.GetKey(KeyCode.W);
        shiftDown = Input.GetKey(KeyCode.LeftShift);

        Quaternion rotation = transform.rotation;

        pitch -= sensitivity * Input.GetAxis("Mouse Y");
        yaw += sensitivity * Input.GetAxis("Mouse X");

        if(pitch > maxVertical)
        {
            pitch = maxVertical;
        } else if(pitch < -maxVertical)
        {
            pitch = -maxVertical;
        }

        transform.eulerAngles = new Vector3(pitch, yaw, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float multiplier = 1;
        if (shiftDown)
        {
            multiplier = speedBoost;
        }
        if (wDown)
        {
            Vector3 forward = this.transform.forward;
            body.velocity = forward * moveSpeed * multiplier;
        } else
        {
            body.velocity = Vector3.zero;
        }
    }
}
