using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class KeyManager : MonoBehaviour
{
    private PlayerInput input;

    private InputMode inputMode;

    #region value type actions
    private List<InputAction> vec2Actions;
    private List<InputAction> vec3Actions;
    private List<InputAction> floatActions;
    #endregion

    #region button type actions
    private List<InputAction> buttonActions;
    #endregion

    void Awake()
    {
        input = GetComponent<PlayerInput>(); 

        if (input != null)
        {
            vec2Actions = GetActions(input.actions, InputActionType.Value, "Vector2");
            vec3Actions = GetActions(input.actions, InputActionType.Value, "Vector3");
            floatActions = GetActions(input.actions, InputActionType.Value, "Axis");
            buttonActions = GetActions(input.actions, InputActionType.Button);
        }
        else
        {
            Debug.LogError("Please Check Player Input");
        }
    }
    void Start()
    {
        InputSetting();
    }

    void Update()
    {
        ProcessVec2Actions();
        ProcessVec3Actions();
        ProcessFloatActions();
    }

    private void Change(InputMode mode)
    {
        inputMode = mode;
        input.defaultActionMap = inputMode.ToString();
    }

    private void ProcessVec2Actions()
    {
        if (vec2Actions == null) return;

        for (int i = 0; i < vec2Actions.Count; i++)
        {
            InputAction action = vec2Actions[i];
            if (action != null)
            {
                Vector2 value = action.ReadValue<Vector2>();
                EventManager.Instance.Trigger(action.name, this, new Vec2Args() { vec = value });
            }
        }
    }

    private void ProcessVec3Actions()
    {
        if (vec3Actions == null) return;

        for (int i = 0; i < vec3Actions.Count; i++)
        {
            InputAction action = vec3Actions[i];
            if (action != null)
            {
                Vector3 value = action.ReadValue<Vector3>();
                // Vec3Args 필요시 추가
            }
        }
    }

    private void ProcessFloatActions()
    {
        if (floatActions == null) return;

        for (int i = 0; i < floatActions.Count; i++)
        {
            InputAction action = floatActions[i];
            if (action != null)
            {
                float value = action.ReadValue<float>();
                // FloatArgs 필요시 추가
            }
        }
    }

    private void InputSetting()
    {
        if (buttonActions != null && buttonActions.Count > 0)
        {
            for( int i = 0; i < buttonActions.Count; i++ )
            {
                InputAction action = buttonActions[i];
                if ( action != null )
                {
                    action.performed += (InputAction.CallbackContext context) =>
                    {
                        EventManager.Instance.Trigger(action.name, this);
                    };
                }
            }
        }
        else
        {
            Debug.LogError("Please reset InputMap or check InputActions.");
        }
    }

    private List<InputAction> GetActions(InputActionAsset actionMap, InputActionType type)
    {
        if (!actionMap)
        {
            Debug.LogError("Please reset InputMap or check InputActions.");
            return new();
        }

        return actionMap
            .Where(action => action.type == type)
            .ToList();
    }

    private List<InputAction> GetActions(InputActionAsset actionMap, InputActionType type, string controlType)
    {
        if (!actionMap)
        {
            Debug.LogError("Please reset InputMap or check InputActions.");
            return new();
        }

        return actionMap
            .Where(action =>
                action.type == type &&
                action.expectedControlType == controlType)
            .ToList();
    }
}

public enum InputMode
{
    Player,
    Claw,
}

public class Vec2Args : EventArgs
{
    public Vector2 vec;
}

