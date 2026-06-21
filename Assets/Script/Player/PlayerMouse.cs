using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMouse : MonoBehaviour
{
    private float mouseX, mouseY;
    private float mouseRotationY;
    public float mouseDistance;
    public Transform playerTranf;
    void Start()
    {
        
    }

    // Update is called once per frame
    public void MouseMove()
    {
        mouseX = Input.GetAxis("Mouse X") * mouseDistance * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseDistance * Time.deltaTime;
        mouseRotationY -= mouseY;
        mouseRotationY = Mathf.Clamp(mouseRotationY, -70, 70);
        playerTranf.Rotate(Vector3.up * mouseX);
        transform.localRotation = Quaternion.Euler(mouseRotationY, transform.localRotation.y, transform.localRotation.z);
    }
}
