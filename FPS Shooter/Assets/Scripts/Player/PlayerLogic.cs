using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerLogic : MonoBehaviour
{
    public Camera PlayerCamera;
    public bool isGrounded;

    [Header("General")]
    public float GravityForce = -9.8f;

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
    private Vector3 groundNormal;
    private Vector3 CharacterVelocity;
    private float cameraVerticalAngle = 0f;
    private float lastTimeJumped = 0f;

    const float JumpGroundingPreventionTime = 0.2f;
    const float GroundCheckDistanceInAir = 0.07f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        GroundCheck();
        Movement();
    }

    private void GroundCheck()
    {
        float checkDistance =
               isGrounded ? (controller.skinWidth + GroundCheckDistance) : GroundCheckDistanceInAir;

        groundNormal = Vector3.up;
        isGrounded = false;

        if (Time.time >= lastTimeJumped + JumpGroundingPreventionTime)
        {
            if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(controller.height),
                controller.radius, Vector3.down, out RaycastHit hit, checkDistance, GroundCheckLayers,
                QueryTriggerInteraction.Ignore))
            {
                groundNormal = hit.normal;

                if (Vector3.Dot(hit.normal, transform.up) > 0f &&
                    IsNormalUnderSlopeLimit(groundNormal))
                {
                    isGrounded = true;

                    if (hit.distance > controller.skinWidth)
                        controller.Move(Vector3.down * hit.distance);
                }
            } 
        }
    }

    private void Movement()
    {
        CameraRotation();

        if (isGrounded && CharacterVelocity.y < 0)
        {
            CharacterVelocity.y = 0f;
        }
        controller.Move(transform.TransformDirection(inputHandler.GetMoveInput()) * MaxSpeed * Time.deltaTime);


        if (isGrounded && inputHandler.GetJumpInputDown())
        {
            CharacterVelocity.y += Mathf.Sqrt(JumpForce * -2f * GravityForce);
            
            lastTimeJumped = Time.time;

            isGrounded = false;
            groundNormal = Vector3.up;
        }

        CharacterVelocity.y += GravityForce * Time.deltaTime;

        controller.Move(CharacterVelocity * Time.deltaTime);
    }

    private void CameraRotation()
    {
        transform.Rotate(new Vector3(0f, (inputHandler.GetLookInputsHorizontal() * RotationSpeed), 0f), Space.Self);

        cameraVerticalAngle += inputHandler.GetLookInputsVertical() * RotationSpeed;
        cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -89f, 89f);
        PlayerCamera.transform.localEulerAngles = new Vector3(cameraVerticalAngle, 0, 0);
    }
   
    Vector3 GetCapsuleBottomHemisphere()
    {
        return transform.position + (transform.up * controller.radius);
    }
 
    Vector3 GetCapsuleTopHemisphere(float atHeight)
    {
        return transform.position + (transform.up * (atHeight - controller.radius));
    }

    bool IsNormalUnderSlopeLimit(Vector3 normal)
    {
        return Vector3.Angle(transform.up, normal) <= controller.slopeLimit;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 from = transform.position + (transform.up * controller.radius);
        Vector3 direction = transform.TransformDirection(Vector3.down) * GroundCheckDistance;
        Gizmos.DrawRay(transform.position, direction);
    }
}
