using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float sensX;
    public float sensY;
    public Transform orientation;

    float yRotacion;
    float xRotacion;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotacion += mouseX;
        xRotacion -= mouseY;
        xRotacion = Mathf.Clamp(xRotacion, -90, 90);

        //rotate
        transform.rotation = Quaternion.Euler(xRotacion, yRotacion, 0);
        orientation.rotation = Quaternion.Euler(0f, yRotacion, 0);
    }
}

