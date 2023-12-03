using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour{

    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;
    public Transform combatLookAt;

    public float horizontalSensitivity;
    public float verticalSensitivity;
    
    public float rotationSpeed;

    [HideInInspector] public static bool cameraActive = true;
    
    public GameObject CM;
    public CinemachineFreeLook CMMainScript;

    

    private void Start(){
        CMMainScript = CM.GetComponent<CinemachineFreeLook>();
        CMMainScript.m_YAxis.m_MaxSpeed = verticalSensitivity;
        CMMainScript.m_XAxis.m_MaxSpeed = horizontalSensitivity;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UpdateCamera(){
        if (cameraActive) {
            CMMainScript.m_YAxis.m_MaxSpeed = verticalSensitivity;
            CMMainScript.m_XAxis.m_MaxSpeed = horizontalSensitivity;
        }
        else {
            CMMainScript.m_YAxis.m_MaxSpeed = 0;
            CMMainScript.m_XAxis.m_MaxSpeed = 0;
        }
    }
    
    private void Update(){
        if (cameraActive) {
            
            Vector3 viewDir = player.position -
                              new Vector3(transform.position.x, player.position.y, transform.position.z);
            orientation.forward = viewDir.normalized;
            
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");
            
            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
            
            if (inputDir != Vector3.zero) {
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
            }
        
            Vector3 dirToLookAt = combatLookAt.position - 
                                  new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
            orientation.forward = dirToLookAt.normalized;

            playerObj.forward = dirToLookAt.normalized;
        }
        
        UpdateCamera();
        
        
    }
}
