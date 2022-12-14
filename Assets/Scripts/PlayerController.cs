using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float sensitivity = 100f;
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private float jumpCheckSize = 0.7f;
    [Space] [Space] [Space]
    [SerializeField] private Transform feetLevel;
    [SerializeField] private Rigidbody body;
    [Space]
    [SerializeField] private LayerMask floorMask;
    
    
    private float _xRotation;
    private float _yRotation;
    private Transform _mainCamera;
    private bool guiEnabled = false;
    private Vector3 moveVector;


    private void Start()
    {
        Init();
        transform.position = new Vector3(0, 25, 0);
    }
    
    // Update is called once per frame
    private void Update()
    {
        CameraController();
        MovementController();
        
        if (transform.position.y < -10){ transform.position = new Vector3(0, 25, 0); }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            guiEnabled = !guiEnabled;
            if (guiEnabled)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    private void Init()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (Camera.main == null) { return; }
        
        _mainCamera = Camera.main.transform;
        _mainCamera.transform.position = transform.position + Vector3.up;
        _mainCamera.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    private void CameraController()
    {
        if (guiEnabled) { return; }
        
        var mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        var mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        
        _xRotation -= mouseY;
        _yRotation += mouseX;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 75f);
        
        _mainCamera.localRotation = Quaternion.Euler(_xRotation, _yRotation, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void MovementController()
    {
        var x = 0f;
        var z = 0f;
        
        if (!guiEnabled)
        {
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");
        }

        if (Physics.CheckSphere(feetLevel.position, jumpCheckSize, floorMask) && Input.GetButtonDown("Jump") &&
            !guiEnabled)
        {
            body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        moveVector = transform.TransformDirection(new Vector3(x, 0f, z)) * speed; 
        _mainCamera.position = transform.position + Vector3.up;
    }

    private void FixedUpdate()
    {
        body.AddForce(Vector3.down * gravity, ForceMode.Force);
        body.velocity = new Vector3(moveVector.x, body.velocity.y, moveVector.z);
        
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) Destroy(this);
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (guiEnabled)
        {
            GUI.contentColor = Color.black;
            GUILayout.Label("Sensitivity (Default: 100): " + sensitivity);
            sensitivity = GUILayout.HorizontalScrollbar(sensitivity, 1f, 0f, 300f);
            
            GUILayout.Label("Jump Height (Default: 20): " + jumpForce);
            jumpForce = GUILayout.HorizontalScrollbar(jumpForce, 1f, 0f, 300f);
            
            GUILayout.Label("Player Speed (Default: 12): " + speed);
            speed = GUILayout.HorizontalScrollbar(speed, 1f, 0f, 100f);
            
            GUILayout.Label("Gravity (Default: 9.8): " + gravity);
            gravity = GUILayout.HorizontalScrollbar(gravity, 1f, 0f, 100f);
            
            GUILayout.Label("Field of View (Default: 60): " + Camera.main.fieldOfView);
            Camera.main.fieldOfView = GUILayout.HorizontalScrollbar(Camera.main.fieldOfView, 1f, 0f, 100f);
        }
        GUILayout.EndArea();
    }
}
