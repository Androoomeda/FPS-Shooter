using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    [Tooltip("Sensitivity multiplier for moving the camera around")]
    public float LookSensitivity = 1f;

    [Tooltip("Limit to consider an input when using a trigger on a controller")]
    public float TriggerAxisThreshold = 0.4f;

    //GameFlowManager m_GameFlowManager;
    bool m_FireInputWasHeld;

    private GameManager m_GameFlowManager;

    void Start()
    {
        m_GameFlowManager = FindFirstObjectByType<GameManager>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        m_FireInputWasHeld = GetFireInputHeld();
    }

    public bool CanProcessInput()
    {
        return Cursor.lockState == CursorLockMode.Locked && !m_GameFlowManager.GameIsEnding;
    }

    public Vector3 GetMoveInput()
    {
        if (CanProcessInput())
        {
            Vector3 move = new Vector3(Input.GetAxisRaw(GameConstants.AxisNameHorizontalMovement), 0f,
                Input.GetAxisRaw(GameConstants.AxisNameVerticalMovement));

            // constrain move input to a maximum magnitude of 1, otherwise diagonal movement might exceed the max move speed defined
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
            return Input.GetButtonDown(GameConstants.k_ButtonNameJump);

        return false;
    }

    public bool GetJumpInputHeld()
    {
        if (CanProcessInput())
        {
            return Input.GetButton(GameConstants.k_ButtonNameJump);
        }

        return false;
    }

    public bool GetFireInputDown()
    {
        return GetFireInputHeld() && !m_FireInputWasHeld;
    }

    public bool GetFireInputReleased()
    {
        return !GetFireInputHeld() && m_FireInputWasHeld;
    }

    public bool GetFireInputHeld()
    {
        if (CanProcessInput())
            return Input.GetButton(GameConstants.k_ButtonNameFire);

        return false;
    }

    public bool GetAimInputHeld()
    {
        if (CanProcessInput())
            return Input.GetButton(GameConstants.k_ButtonNameAim);

        return false;
    }

    public bool GetReloadButtonDown()
    {
        if (CanProcessInput())
            return Input.GetKeyDown(KeyCode.R);

        return false;
    }

    internal int GetSwitchWeaponInput()
    {
        throw new NotImplementedException();
    }
}