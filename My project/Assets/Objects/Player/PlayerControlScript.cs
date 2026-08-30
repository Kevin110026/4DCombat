using System;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(CharacterScript))]
[RequireComponent(typeof(AudioSource))]

[RequireComponent(typeof(PlayerScript))]
public class PlayerControlScript : MonoBehaviour
{
    public float mouseSensitivity = 0.1f;
    public Camera playerCamera;
    [HideInInspector] public PlayerScript playerScript;
    [HideInInspector] public CharacterScript characterScript;
    [HideInInspector] public CharacterMovementScript characterMovementScript;
    [HideInInspector] public PlayerParryBoxscript parry;
    [HideInInspector] public GameObject parryBox;
    [HideInInspector] public GameObject attackBox;
    [HideInInspector] public CameraScript cameraScript;
    public float playerDashCooldown = 1.0f;
    private AudioSource audioSource;
    public AudioClip dashSound;

    void Awake()
    {
        playerScript = GetComponent<PlayerScript>();
        parry = playerScript.parry;
        parryBox = parry.gameObject;
        attackBox = playerScript.attackBox;
        cameraScript = playerCamera.GetComponent<CameraScript>();
    }

    void Start()
    {
        characterScript = playerScript;
        characterMovementScript = characterScript.movementScript;
        Cursor.lockState = CursorLockMode.Locked;
        cameraLatencyPlayerPosition = transform.position;
        audioSource = GetComponent<AudioSource>();
    }

    private float RMBTime = 0;

    void Update()
    {
        HandlePlayerRotation();
        HandlePlayerMovement();
        HandleParryBoxPosition();
        HandleAttackBoxPosition();
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            playerScript.Parry();
            playerScript.StartBlocking();
            RMBTime = Time.time;
        }
        else if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            if (!parry.IsParrying() && Time.time >= RMBTime + 0.12f)
                playerScript.Parry();
            playerScript.EndBlocking();
            // Debug.Log("RMB hold time: " + (Time.time - RMBTime));
        }
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            playerScript.NormalSlash();
        }
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            playerScript.DashSlash();
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            playerScript.Heal();
        }
    }

    void LateUpdate()
    {
        HandleCamera();
    }

    void HandlePlayerRotation()
    {
        if (!playerScript.canTurn)
            return;
            
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * mouseSensitivity;
        transform.Rotate(0, mouseX, 0);
    }

    private float playerDashLastTime = -999f;

    void HandlePlayerMovement()
    {
        if (!playerScript.canMove)
            return;

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
        characterMovementScript.MoveInVelocityXZ(move);


        if (Keyboard.current.spaceKey.isPressed)
        {
            characterMovementScript.TryJump();
        }
        if (Keyboard.current.leftShiftKey.wasPressedThisFrame && playerDashLastTime + playerDashCooldown <= Time.time)
        {
            characterMovementScript.AddVelocity(move * 15f);
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
    private RaycastHit[] HandleParryBoxPositionHits = new RaycastHit[10];

    void HandleParryBoxPosition()
    {
        Quaternion ori = Quaternion.Euler(verticalRotation, transform.rotation.eulerAngles.y, 0f);
        Vector3 dir = ori * Vector3.forward;
        Vector3 halfExtents = parryBox.transform.lossyScale / 2f;
        // Debug.Log(halfExtents);
        int hitCount = Physics.BoxCastNonAlloc(transform.position + dir * 3f, halfExtents, -dir, HandleParryBoxPositionHits, ori, 6f);    //  , LayerMask.NameToLayer("Characters")
        // Ray ray = new Ray(transform.position + dir * 3f, -dir);
        // int hitCount = Physics.RaycastNonAlloc(ray, HandleParryBoxPositionHits, 6f);    //  , LayerMask.NameToLayer("Characters")
        Vector3 pos = transform.position + dir * 3f;   //  it will not change if there's no collision
        // Debug.Log("hit count: " + hitCount);
        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit hit = HandleParryBoxPositionHits[i];
            if (hit.collider.gameObject == gameObject)
            {
                pos = transform.position + dir * 3f + (-dir * (hit.distance - 0.2f));
                // Debug.Log("success: " + hit.distance);
                break;
            }
        }
        parryBox.transform.SetPositionAndRotation(pos, ori);
    }
    void HandleAttackBoxPosition()
    {
        Quaternion ori = Quaternion.Euler(verticalRotation, transform.rotation.eulerAngles.y, 0f);
        Vector3 dir = ori * Vector3.forward;
        Vector3 halfExtents = attackBox.transform.lossyScale / 2f;
        // Debug.Log(halfExtents);
        int hitCount = Physics.BoxCastNonAlloc(transform.position + dir * 3f, halfExtents, -dir, HandleParryBoxPositionHits, ori, 6f);    //  , LayerMask.NameToLayer("Characters")
        // Ray ray = new Ray(transform.position + dir * 3f, -dir);
        // int hitCount = Physics.RaycastNonAlloc(ray, HandleParryBoxPositionHits, 6f);    //  , LayerMask.NameToLayer("Characters")
        Vector3 pos = transform.position + dir * 3f;   //  it will not change if there's no collision
        // Debug.Log("hit count: " + hitCount);
        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit hit = HandleParryBoxPositionHits[i];
            if (hit.collider.gameObject == gameObject)
            {
                pos = transform.position + dir * 3f + (-dir * (hit.distance - 0.2f));
                // Debug.Log("success: " + hit.distance);
                break;
            }
        }
        attackBox.transform.SetPositionAndRotation(pos, ori);
    }
}
