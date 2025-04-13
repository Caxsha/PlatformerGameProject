using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    [Header("Ground Check")]
    public float playerHeight;

    public LayerMask whatIsGround;
    private bool _isOnGround;

    public Transform orientation;

    private float _horizontalInput;
    private float _verticalInput;

    private Vector3 _movementDirection;

    private Rigidbody _rigidBody;
    
    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _rigidBody.freezeRotation = false;
    }

    private void InputFromDevice()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        PlayerMove();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if ground
        _isOnGround = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        InputFromDevice();

        if (_isOnGround)
        {
            _rigidBody.drag = groundDrag;
        }
        else
        {
            _rigidBody.drag = 0;
        }
    }

    private void PlayerMove()
    {
        // Colculate direction
        _movementDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;

        _rigidBody.AddForce(_movementDirection * moveSpeed * 10f, ForceMode.Force);
    }
}
