using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float walkSpeed = 8f;
    public float runSpeed = 12f;

    public float jumpPower = 5f;

    bool _isJumping = false;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;

    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    Rigidbody _rb;

    Vector3 _movement;

    float _horizontalMove;
    float _verticalMove;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        Move();
        Jump();
        GroundedCheck();
    }
    void Update()
    {
        _horizontalMove = Input.GetKey(KeyCode.W) ? 1 : (Input.GetKey(KeyCode.S) ? -1 : 0);
        _verticalMove = Input.GetKey(KeyCode.D) ? 1 : (Input.GetKey(KeyCode.A) ? -1 : 0);

        if (Input.GetKey(KeyCode.Space))
        {
            _isJumping = true;
        }
    }
    void Move()
    {
        _movement.Set(_horizontalMove, 0, _verticalMove);
        _movement = _movement.normalized * walkSpeed * Time.deltaTime;

        _rb.MovePosition(transform.position + _movement);
    }
    void Jump()
    {
        if(!Grounded || !_isJumping)
            return;

        _rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);

        _isJumping = false;
    }
    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
    }
}
