using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CharacterMovementScript))]
public class CharacterMovementScript : MonoBehaviour
{
    public float maxMoveSpeed = 5f;
    private float oriMaxMoveSpeed;
    public float maxJumpHeight = 1.5f;
    public float maxAcceleration = 30f;

    private float oriMaxAcceleration;
    public float gravity = -9.81f;
    private CharacterController characterController;

    [HideInInspector] public Vector3 velocity;
    private Vector3 idealVelocity;
    private Vector3 idealAcceleration;
    private Vector3 optimizedAcceleration;
    private float velocityDecay = 0.1f;
    private Vector3 acceleration = Vector3.zero;
    private CharacterMovementScript movement;
    [HideInInspector]
    public bool doneMoveTo = true;
    [HideInInspector]
    public float moveToSpeed;
    [HideInInspector]
    public Vector3 moveToDestination;
    [HideInInspector]
    public Vector3 moveToFinalVelocity;

    void Awake()
    {
        movement = GetComponent<CharacterMovementScript>();
        oriMaxMoveSpeed = maxMoveSpeed;
        oriMaxAcceleration = maxAcceleration;
        moveToFinalVelocity = Vector3.zero;
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        acceleration = new Vector3(0, gravity, 0);
    }

    void Update()
    {
        if (!doneMoveTo)
        {
            //  1/2*at^2 = diff
            //  t
            Vector3 diff = moveToDestination - transform.position;
            float speed = (float)Math.Sqrt(2 * maxAcceleration * diff.magnitude);
            speed = Math.Min(speed, maxMoveSpeed);
            Vector3 speedVec = diff.normalized * speed + moveToFinalVelocity;
            speed = Math.Min(speedVec.magnitude, maxMoveSpeed);
            MoveInVelocityXZ(speedVec, speed);
        }

        velocity.x = Mathf.Sign(velocity.x) * Mathf.Max(0, Mathf.Abs(velocity.x) - velocityDecay * Time.deltaTime);
        velocity.z = Mathf.Sign(velocity.z) * Mathf.Max(0, Mathf.Abs(velocity.z) - velocityDecay * Time.deltaTime);

        idealAcceleration = idealVelocity - velocity;
        idealAcceleration.y = 0;
        optimizedAcceleration = idealAcceleration.normalized * Mathf.Min(maxAcceleration * Time.deltaTime, idealAcceleration.magnitude);
        acceleration = optimizedAcceleration;
        acceleration.y += gravity * Time.deltaTime;
        velocity += acceleration;

        characterController.Move(velocity * Time.deltaTime);
        if (characterController.isGrounded)
        {
            velocity.y = 0;
        }

        if ((moveToDestination - transform.position).magnitude <= 0.05f)
        {
            DoneMoveTo();
        }
    }

    // on xz plane
    public void MoveInVelocityXZ(Vector3 direction, float speed = -1)
    {
        direction.y = 0;
        direction = direction.normalized;
        if (speed == -1)
            speed = maxMoveSpeed;
        idealVelocity = direction * speed;
    }

    public void AddVelocity(Vector3 addedVelocity)
    {
        velocity += addedVelocity;
    }

    public void TryJump(float height = -1)
    {
        if (height == -1)
            height = maxJumpHeight;
        if (characterController.isGrounded)
            velocity.y = (float)Math.Sqrt(2 * -gravity * height);
    }

    public void MoveToXZ(Vector3 destination, float speed, Vector3 finalVelocity)
    {
        MoveToXZ(destination, speed);
        moveToFinalVelocity = finalVelocity;
    }
    public void MoveToXZ(Vector3 destination, float speed = -1)
    {
        doneMoveTo = false;
        moveToSpeed = speed;
        moveToDestination = destination;
        moveToFinalVelocity = Vector3.zero;
    }

    public void DoneMoveTo()
    {
        doneMoveTo = true;
        MoveInVelocityXZ(Vector3.one, 0);
        moveToFinalVelocity = Vector3.zero;
    }

    public void ResetMaxSpeedAndAcceletarion()
    {
        maxMoveSpeed = oriMaxMoveSpeed;
        maxAcceleration = oriMaxAcceleration;
    }
}
