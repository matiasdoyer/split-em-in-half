using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.EventSystems;

public class playermovement : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform orientation;
    [SerializeField] float speed;

    Vector3 moveDirection;
    Vector3 dashDirection;

    float horizontalInput;
    float verticalInput;

    public float groundDrag;
    public bool grounded;
    public float playerHeight;
    public LayerMask whatIsGround;

    public float dashSpeed;
    public float dashTime;
    public int dashCD;
    public bool isDashing;
    public Transform dashOrientation;
    public KeyCode dashKey;
    public bool dashIsReady = true;
    public Transform CameraMain;

    [Header("Dash Air Control")]
    public float dashAirControlDelay = 0.1f;
    public float dashAirMaxSpeed = 15f;
    public float dashAirControlTime = 0.2f;

    private bool limitingDashSpeed;

    void Start()
    {
        rb.freezeRotation = true;
    }

    void Update()
    {
        GroundCheck();

        // Leer inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Rotación suave del dashOrientation hacia la cámara
        Quaternion target = CameraMain.transform.rotation;
        dashOrientation.rotation = Quaternion.Slerp(
            dashOrientation.rotation,
            target,
            Time.deltaTime * 5f
        );

        // Drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        // Detectar dash
        if (Input.GetKeyDown(dashKey) && dashIsReady)
        {
            Dash();
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();

        if (limitingDashSpeed)
        {
            if (rb.velocity.magnitude > dashAirMaxSpeed)
            {
                rb.velocity = rb.velocity.normalized * dashAirMaxSpeed;
            }
        }
    }

    private void GroundCheck()
    {
        grounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            playerHeight * 0.5f + 0.3f,
            whatIsGround
        );
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput
                      + orientation.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * speed * 10f, ForceMode.Force);
    }

    private void Dash()
    {
        dashIsReady = false;

        Vector3 cameraForward = CameraMain.forward;
        Vector3 cameraRight = CameraMain.right;

        Vector3 inputDirection = cameraForward * verticalInput
                               + cameraRight * horizontalInput;

        dashDirection = inputDirection != Vector3.zero
                        ? inputDirection.normalized
                        : cameraForward.normalized;

        rb.AddForce(dashDirection * dashSpeed, ForceMode.Impulse);

        // Esperar un pequeño tiempo antes de verificar
        Invoke(nameof(CheckAirAfterDash), dashAirControlDelay);

        Invoke(nameof(ResetDash), dashCD);
    }

    private void CheckAirAfterDash()
    {
        if (!grounded)
        {
            limitingDashSpeed = true;
            Invoke(nameof(StopLimitingDashSpeed), dashAirControlTime);
        }
    }
    private void StopLimitingDashSpeed()
    {
        limitingDashSpeed = false;
    }

    private void ResetDash()
    {
        dashIsReady = true;
    }
}