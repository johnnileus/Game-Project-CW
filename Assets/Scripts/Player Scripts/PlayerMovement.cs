using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class PlayerMovement : MonoBehaviour{

    public float speed;
    public float runSpeed;
    public float crouchSpeed;
    public float crouchHeight;
    public float extraGravity;

    public float jumpForce;
    private bool readyToJump = true;
    public float jumpCooldown;
    
    public Transform orientation;
    private Vector3 moveDirection;
    public GameObject playerModel;

    private float horizontalInput;
    private float verticalInput;

    public float playerHeight;
    public LayerMask whatIsGround;
    private bool grounded;
    public float groundDrag;

    [SerializeField] private CapsuleCollider cc;
    private Rigidbody rb;

    [SerializeField] private PhysicMaterial plrStatic;
    [SerializeField] private PhysicMaterial plrUngrounded;
    
    private enum MovementState{
        walk,
        run,
        crouch
    }

    private MovementState currentState = MovementState.walk;
    public void Update(){
        
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.05f, whatIsGround);
        PlayerInput();

        if (grounded) {
            rb.drag = groundDrag;
            cc.material = plrStatic;

        }
        else {
            rb.drag = 0;
            cc.material = plrUngrounded;
        }
        MovePlayer();
        SpeedControl();

    }
    

    void PlayerInput(){
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        if (Input.GetKeyDown(KeyCode.Space) && grounded && readyToJump) {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKey(KeyCode.LeftControl)) {
            currentState = MovementState.crouch;
            transform.GetComponent<CapsuleCollider>().height = crouchHeight;
        } else if (Input.GetKey(KeyCode.LeftShift)) {
            currentState = MovementState.run;
            transform.GetComponent<CapsuleCollider>().height = 2f;
        } else {
            currentState = MovementState.walk;
            transform.GetComponent<CapsuleCollider>().height = 2f;
        }
    }
    
    public void Start(){
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        cc.GetComponent<CapsuleCollider>();
    }

    void MovePlayer(){
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if (currentState == MovementState.run) {
            rb.AddForce(moveDirection.normalized * runSpeed, ForceMode.Force);
        } 
        else if (currentState == MovementState.walk) {
            rb.AddForce(moveDirection.normalized * speed, ForceMode.Force);
        }
        else { // crouch
            rb.AddForce(moveDirection.normalized * crouchSpeed, ForceMode.Force);
        }
    }

    void FixedUpdate () {
        rb.AddForce(Vector3.down * extraGravity * rb.mass);
    }
    
    void SpeedControl(){
        Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        if (flatVel.magnitude > speed) {
            Vector3 limitedVel = flatVel.normalized * speed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    void Jump(){
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
    }

    void ResetJump(){
        readyToJump = true;
    }
}
