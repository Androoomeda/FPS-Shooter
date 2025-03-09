using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    [Tooltip("Sensitivity multiplier for moving the camera around")]
    public float LookSensitivity = 1f;

    [Tooltip("Limit to consider an input when using a trigger on a controller")]
    public float TriggerAxisThreshold = 0.4f;

    private bool fireInputWasHeld;
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        fireInputWasHeld = GetFireInputHeld();
    }

    public bool CanProcessInput()
    {
        return Cursor.lockState == CursorLockMode.Locked && !gameManager.GameIsEnding;
    }

    public Vector3 GetMoveInput()
    {
        if (CanProcessInput())
        {
            Vector3 move = new Vector3(Input.GetAxisRaw(GameConstants.AxisNameHorizontalMovement), 0f,
                Input.GetAxisRaw(GameConstants.AxisNameVerticalMovement));

            move = Vector3.ClampMagnitude(move, 1);

            return move;
        }

        return Vector3.zero;
    }

    public float GetLookInputsHorizontal()
    {
        if(CanProcessInput()) 
            return Input.GetAxisRaw(GameConstants.MouseAxisNameHorizontal) * LookSensitivity * 0.01f;

        return 0f;
    }

    public float GetLookInputsVertical()
    {
        if (CanProcessInput())
            return Input.GetAxisRaw(GameConstants.MouseAxisNameVertical) * LookSensitivity * -0.01f;

        return 0f;
    }

    public bool GetJumpInputDown()
    {
        if (CanProcessInput())
            return Input.GetButtonDown(GameConstants.ButtonNameJump);

        return false;
    }

    public bool GetJumpInputHeld()
    {
        if (CanProcessInput())
        {
            return Input.GetButton(GameConstants.ButtonNameJump);
        }

        return false;
    }

    public bool GetFireInputDown()
    {
        return GetFireInputHeld() && !fireInputWasHeld;
    }

    public bool GetFireInputReleased()
    {
        return !GetFireInputHeld() && fireInputWasHeld;
    }

    public bool GetFireInputHeld()
    {
        if (CanProcessInput())
            return Input.GetButton(GameConstants.ButtonNameFire);

        return false;
    }

    public bool GetAimInputHeld()
    {
        if (CanProcessInput())
            return Input.GetButton(GameConstants.ButtonNameAim);

        return false;
    }

    public bool GetReloadButtonDown()
    {
        if (CanProcessInput())
            return Input.GetKeyDown(KeyCode.R);

        return false;
    }

    public bool GetInventoryButtonDown()
    {
        return Input.GetButtonDown(GameConstants.ButtonNameInventory);
    }
}