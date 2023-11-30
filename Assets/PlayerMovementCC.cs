using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class PlayerMovementCC : MonoBehaviour{

    [SerializeField] private float speed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float gravityStrength;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    
    private float horizontalInput;
    private float verticalInput;
    private bool grounded;
    private Vector3 vel;

    private CharacterController cc;

    private void Start(){
        cc = GetComponent<CharacterController>();
        vel = new Vector3(0, 0, 0);

    }

    public void Update(){
        PlayerInput();

        vel.y += gravityStrength * Time.deltaTime;
        vel.x += horizontalInput * Time.deltaTime * speed;
        vel.z += verticalInput * Time.deltaTime * speed;

        Vector3 inpVector = new Vector3(horizontalInput * speed, gravityStrength, verticalInput * speed);
        
        cc.Move(inpVector * Time.deltaTime);
        //cc.Move(vel * Time.deltaTime * speed);
    }
    

    void PlayerInput(){
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && grounded) {
            //jump
        }
    }

}