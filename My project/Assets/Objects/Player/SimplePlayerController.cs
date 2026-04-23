using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CharacterMovementScript))]
[RequireComponent(typeof(AudioSource))]
public class SimplePlayerController : MonoBehaviour
{
    public float mouseSensitivity = 0.1f;
    public Camera playerCamera;
    public CharacterMovementScript characterMovement;
    public float playerDashCooldown = 1.0f;
    private AudioSource audioSource;
    public AudioClip dashSound;

    void Start()
    {
        characterMovement = GetComponent<CharacterMovementScript>();
        Cursor.lockState = CursorLockMode.Locked;
        cameraLatencyPlayerPosition = transform.position;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        HandlePlayerRotation();
        HandlePlayerMovement();
        HandleCamera();
    }

    void HandlePlayerRotation()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * mouseSensitivity;
        transform.Rotate(0, mouseX, 0);
    }

    private float playerDashLastTime = -999f;

    void HandlePlayerMovement()
    {
        float moveX = 0f;
        float moveZ = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.dKey.isPressed) moveX += 1f;
            if (Keyboard.current.aKey.isPressed) moveX -= 1f;
            if (Keyboard.current.wKey.isPressed) moveZ += 1f;
            if (Keyboard.current.sKey.isPressed) moveZ -= 1f;
        }

        Vector3 inputDir = new Vector3(moveX, 0, moveZ).normalized;
        Vector3 move = transform.right * inputDir.x + transform.forward * inputDir.z;
        characterMovement.MoveInVelocityXZ(move);


        if (Keyboard.current.spaceKey.isPressed)
        {
            characterMovement.TryJump();
        }
        if (Keyboard.current.leftShiftKey.wasPressedThisFrame && playerDashLastTime + playerDashCooldown <= Time.time)
        {
            characterMovement.AddVelocity(move * 15f);
            audioSource.PlayOneShot(dashSound);
            playerDashLastTime = Time.time;
        }
    }


    private float verticalRotation = 0f;
    private Vector3 cameraLatencyPlayerPosition;
    void HandleCamera()
    {
        if (Mouse.current != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            float mouseY = mouseDelta.y * mouseSensitivity;


            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
            playerCamera.transform.rotation = Quaternion.Euler(verticalRotation, transform.rotation.eulerAngles.y, 0);
            Vector3 deltaPos = cameraLatencyPlayerPosition - transform.position;
            float deltaDist = Vector3.Distance(deltaPos, Vector3.zero);
            deltaDist = Mathf.Clamp(0, deltaDist * Mathf.Pow(0.9f, Time.deltaTime * 60f) - Time.deltaTime * 0.001f, 3);
            deltaPos = deltaPos.normalized * deltaDist;
            cameraLatencyPlayerPosition = transform.position + deltaPos;
            playerCamera.transform.position = cameraLatencyPlayerPosition + Vector3.up * 1.5f - 3 * playerCamera.transform.forward;
        }
    }
}