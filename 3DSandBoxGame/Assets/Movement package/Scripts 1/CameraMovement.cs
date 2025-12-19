using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraMovement : MonoBehaviour
{
    // Sensitivity 
    public float sensX;
    public float sensY;


    public Transform orientation;
    public Transform camHolder;

    // Rotations
    float xRotation;
    float yRotation;

    private Camera cam;
    private float defaultFov;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cam = GetComponent<Camera>();
        defaultFov = cam.fieldOfView;


    }

    // Update is called once per frame
    void Update()
    {
        // Get mouse inputs
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;
        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);


        camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }


    public void DoFov(float endValue)
    {
        cam.DOKill(); 
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);

    }

    public void DoTilt(float zTilt)
    {

        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);

    }

    public void ResetFov()
    {
        DoFov(defaultFov);
    }
}
