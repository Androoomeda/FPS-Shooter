using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerLogic : MonoBehaviour
{
    public Camera PlayerCamera;
    public bool isGrounded;

    [Header("General")]
    public float GravityForce = 9.8f;

    public LayerMask GroundCheckLayers = -1;
    public float GroundCheckDistance = 0.3f;

    [Header("Movement")]
    public float MaxSpeed = 10f;
    public float JumpForce = 9f;
    public float MovementSharpnessOnGround = 15;


    [Header("Rotation")]
    [Tooltip("Rotation speed for moving the camera")]
    public float RotationSpeed = 200f;

    private CharacterController controller;
    private PlayerInputHandler inputHandler;
    private Vector3 m_GroundNormal;
    private Vector3 CharacterVelocity;
    private float cameraVerticalAngle = 0f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        m_GroundNormal = Vector3.up;
        GroundCheck();
        Movement();
    }

    private void GroundCheck()
    {
        Vector3 from = transform.position + (transform.up * controller.radius);
        Vector3 to = transform.TransformDirection(Vector3.down);

        if (Physics.SphereCast(from, controller.radius, to, out RaycastHit hit,
            GroundCheckDistance, GroundCheckLayers, QueryTriggerInteraction.Ignore))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void Movement()
    {
        CameraRotation();

        Vector3 worldspaceMoveInput = transform.TransformVector(inputHandler.GetMoveInput());

        if (isGrounded)
        {
            Vector3 targetVelocity = worldspaceMoveInput * MaxSpeed;
            targetVelocity = GetDirectionReorientedOnSlope(targetVelocity.normalized, m_GroundNormal) *
                                         targetVelocity.magnitude;

            CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVelocity,
                           MovementSharpnessOnGround * Time.deltaTime);

            if (isGrounded && inputHandler.GetJumpInputDown())
            {
                CharacterVelocity = new Vector3(CharacterVelocity.x, 0f, CharacterVelocity.z);
                CharacterVelocity += Vector3.up * JumpForce;
            }
        }
        else
        {
            CharacterVelocity += Vector3.down * GravityForce * Time.deltaTime;
        }

        controller.Move(CharacterVelocity * Time.deltaTime);
    }

    private void CameraRotation()
    {
        transform.Rotate(new Vector3(0f, (inputHandler.GetLookInputsHorizontal() * RotationSpeed), 0f), Space.Self);

        cameraVerticalAngle += inputHandler.GetLookInputsVertical() * RotationSpeed;
        cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -89f, 89f);
        PlayerCamera.transform.localEulerAngles = new Vector3(cameraVerticalAngle, 0, 0);
    }

    public Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
    {
        Vector3 directionRight = Vector3.Cross(direction, transform.up);
        return Vector3.Cross(slopeNormal, directionRight).normalized;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 from = transform.position + (transform.up * controller.radius);
        Vector3 direction = transform.TransformDirection(Vector3.down) * GroundCheckDistance;
        Gizmos.DrawRay(from, direction);
    }
}
