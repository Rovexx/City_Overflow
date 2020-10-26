using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public InputMaster controls;
    public Camera cam;
    public float panSpeed = 500f;
    public float rotateSpeed = 40f;
    [Tooltip("Smoothness factor for moving the camera")][Range(0.1f, 2f)]
    public float cameraSmoothing = 0.5f;
    [Tooltip("Smoothness factor for rotating the camera")][Range(0.1f, 2f)]
    public float rotationSmoothing = 0.5f;
    public bool enableMousePanning = false;
    public float panBorderThickness = 10f;
    public Vector2 panLimit;

    private Vector2 _inputWASD;
    private Vector2 _inputRotate;
    private Vector2 _mousePosition;
    private float _inputZoom;
    private Vector3 velocity = Vector3.zero;
    private Vector3 velocityRot = Vector3.zero;

    void Awake()
    {
        controls = new InputMaster();
        controls.Camera.Movement.performed += X => _inputWASD = X.ReadValue<Vector2>();
        controls.Camera.Rotate.performed += X => _inputRotate = X.ReadValue<Vector2>();
        controls.Camera.Mouse.performed += X => _mousePosition = X.ReadValue<Vector2>();
        controls.Camera.Zoom.performed += X => _inputZoom = X.ReadValue<float>();
    }

    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }

    private void FixedUpdate()
    {
        Transform cameraTransform = cam.transform;
        Transform cameraRigTransform = transform;
        Vector3 cameraRigPosition = transform.position;

        // Move up
        if (_inputWASD.y > 0.1f || (enableMousePanning && _mousePosition.y >= Screen.height - panBorderThickness))
        {
            cameraRigPosition += cameraRigTransform.forward * panSpeed * Time.fixedDeltaTime;
        }
        // Move down
        if (_inputWASD.y < -0.1f || (enableMousePanning && _mousePosition.y <= panBorderThickness))
        {
            cameraRigPosition -= cameraRigTransform.forward * panSpeed * Time.fixedDeltaTime;
        }
        // Move left
        if (_inputWASD.x > 0.1f || (enableMousePanning && _mousePosition.x >= Screen.width - panBorderThickness))
        {
            cameraRigPosition += cameraRigTransform.right * panSpeed * Time.fixedDeltaTime;
        }
        // Move right
        if (_inputWASD.x < -0.1f || (enableMousePanning && _mousePosition.x <= panBorderThickness))
        {
            cameraRigPosition -= cameraRigTransform.right * panSpeed * Time.fixedDeltaTime;
        }
        // Zoom in
        if (_inputZoom > 0)
        {
            cameraRigPosition -= cameraTransform.forward * panSpeed * Time.fixedDeltaTime;
        }
        // Zoom out
        if (_inputZoom < 0)
        {
           cameraRigPosition += cameraTransform.forward * panSpeed * Time.fixedDeltaTime;
        }

        // Rotate up
        if (_inputRotate.x < 0)
        {
            cameraTransform.Rotate(Vector3.left * rotateSpeed * Time.fixedDeltaTime);
        }
        // Rotate down
        if (_inputRotate.x > 0)
        {
            cameraTransform.Rotate(Vector3.right * rotateSpeed * Time.fixedDeltaTime);
        }
        // Rotate left
        if (_inputRotate.y < 0)
        {
            cameraRigTransform.Rotate(Vector3.up * rotateSpeed * Time.fixedDeltaTime);
        }
        // Rotate right
        if (_inputRotate.y > 0)
        {
            cameraRigTransform.Rotate(Vector3.down * rotateSpeed * Time.fixedDeltaTime);
        }

        // Apply new position
        cameraRigPosition.x = Mathf.Clamp(cameraRigPosition.x, -panLimit.x, panLimit.x);
        cameraRigPosition.z = Mathf.Clamp(cameraRigPosition.z, -panLimit.y, panLimit.y);

        // Smooth motion
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, cameraRigPosition, ref velocity, cameraSmoothing);
        Quaternion smoothedRotation = Quaternion.Slerp(transform.rotation, cameraRigTransform.rotation, rotationSmoothing);
        Quaternion smoothedCameraRotation = Quaternion.Slerp(cameraTransform.rotation, cameraTransform.rotation, rotationSmoothing);

        // Apply transform
        transform.position = smoothedPosition;
        transform.rotation = smoothedRotation;
        cam.transform.rotation = smoothedCameraRotation;
    }
}
