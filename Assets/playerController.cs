using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;



public class PlayerController : NetworkBehaviour
{
    public float speed = 12f;
    public float sensitivty = 100f;
    
    private float xRotation = 0f;
    private float yRotation = 0f;

    void Start()
    {
        Camera.main.transform.position = transform.position + Vector3.up;
        Camera.main.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        Cursor.lockState = CursorLockMode.Locked;
    }


    // Update is called once per frame
    void Update()
    {
        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");
        float mouseX = Input.GetAxis("Mouse X") * sensitivty * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivty * Time.deltaTime;

        Vector3 move = transform.right * x + transform.forward * z;
        GetComponent<CharacterController>().Move(move * speed * Time.deltaTime);
        
        xRotation -= mouseY;
        yRotation += mouseX;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        Camera.main.transform.position = transform.position;

            transform.Rotate(Vector3.up * mouseX);
        //Camera.main.transform.Rotate(Vector3.up * mouseX);
        
        
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) Destroy(this);
    }
}
