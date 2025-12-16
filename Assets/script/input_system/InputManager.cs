using Proselyte.Sigils;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, ISaveableSettings
{
    private static bool isActive;

    [SerializeField] InputDataSO input_data_SO;
    [SerializeField] InputEventDataSO input_event_data_SO;

    [Header("Runtime Sets")]
    [SerializeField] RebindableRuntimeSet rebindableRuntimeSet;
    [SerializeField] SaveableSettingsRuntimeSet saveableSettingsRuntimeSet;

    [Header("References")]
    [SerializeField] InputActionAsset actions;
    [SerializeField] PlayerInput playerInput;
    

    private bool _pauseHandledThisFrame;

    private InputActionMap _playerMap;
    private InputActionMap _uiMap;

    private InputAction _moveAction;
    private InputAction _crouchAction;
    private InputAction _sprintAction;
    private InputAction _jumpAction;
    private InputAction _interactAction;
    private InputAction _lookAction;
    private InputAction _pauseAction;
    private InputAction _quickLoadAction;
    private InputAction _quickSaveAction;
    private InputAction _flashlightAction;
    private InputAction _cameraAction;

    private void Awake()
    {
        if(!InputManager.isActive)
            InputManager.isActive = true;
        else
            return;

        input_data_SO.moveInput = Vector2.zero;
        input_data_SO.lookInput = Vector2.zero;
        input_data_SO.crouchInput = false;
        input_data_SO.sprintInput = false;
        input_data_SO.jumpInput = false;
        input_data_SO.interactInput = false;

        // Init action maps
        _playerMap = playerInput.actions.FindActionMap("Player", true);
        _uiMap = playerInput.actions.FindActionMap("UI", true);

        // Init player actions
        _moveAction = playerInput.actions["Move"];
        _crouchAction = playerInput.actions["Crouch"];
        _sprintAction = playerInput.actions["Sprint"];
        _jumpAction = playerInput.actions["Jump"];
        _flashlightAction = playerInput.actions["Flashlight"];
        _cameraAction = playerInput.actions["Camera"];
        _interactAction = playerInput.actions["Interact"];
        _lookAction = playerInput.actions["Look"];
        _pauseAction = playerInput.actions["Pause"];
        _quickLoadAction = playerInput.actions["QuickLoad"];
        _quickSaveAction = playerInput.actions["QuickSave"];
    }

    private void OnEnable()
    {
        if(!InputManager.isActive)
        {
            gameObject.SetActive(false);
            return;
        }

        // Init Player Prefs (if any)
        var rebinds = PlayerPrefs.GetString("rebinds");
        if(!string.IsNullOrEmpty(rebinds))
        {
            actions.LoadBindingOverridesFromJson(rebinds);
            UpdateAllBindingDisplays();
        }

        if(_playerMap == null && _uiMap == null) return;
        _playerMap.Disable();
        _uiMap.Enable();

        InputSystem.onActionChange += OnInputActionChanged;
        playerInput.onControlsChanged += OnPlayerInputControlsChanged;

        // TODO(Jazz): Do we need a RebindsManager for all this?
        //saveableSettingsRuntimeSet.Add(this);
    }

    private void OnDisable()
    {
        if(InputManager.isActive) return;

        // Save current binds to player prefs
        var rebinds = actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);

        if(_playerMap == null && _uiMap == null) return;
        _uiMap.Disable();
        _playerMap.Disable();

        InputSystem.onActionChange -= OnInputActionChanged;

        //saveableSettingsRuntimeSet.Remove(this);
    }

    private void Start()
    {
        if(InputManager.isActive) return;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        input_data_SO.moveInput = _moveAction.ReadValue<Vector2>();
        input_data_SO.crouchInput = _crouchAction.ReadValue<float>() > 0;
        input_data_SO.sprintInput = _sprintAction.ReadValue<float>() > 0;
        input_data_SO.jumpInput = _jumpAction.ReadValue<float>() > 0;
        input_data_SO.lookInput = _lookAction.ReadValue<Vector2>();
        input_data_SO.interactInput = _interactAction.WasPressedThisFrame();

        if(_pauseAction.WasPressedThisFrame() && !_pauseHandledThisFrame)
        {
            input_event_data_SO.OnPauseInputEvent.Raise();
            _pauseHandledThisFrame = true;
        }
        if(_quickLoadAction.WasPressedThisFrame())
        {
            input_event_data_SO.OnQuickLoadInputEvent.Raise();
        }
        if(_quickSaveAction.WasPressedThisFrame())
        {
            input_event_data_SO.OnQuickSaveInputEvent.Raise();
        }
    }

    public void OnPlayGameEventRaised()
    {
        _uiMap.Disable();
        _playerMap.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnPauseGameEventRaised()
    {
        _playerMap.Disable();
        _uiMap.Enable();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Ensures input visuals stay accurate when bindings change 
    // including external changes like keyboard layout shifts.
    private void OnInputActionChanged(object obj, InputActionChange inputActionChange)
    {
        if(inputActionChange != InputActionChange.BoundControlsChanged)
        {
            return;
        }

        UpdateAllBindingDisplays();
    }

    // Passes its value to all rebindables for glyph control scheme detection.
    private void OnPlayerInputControlsChanged(PlayerInput playerInput)
    {
        input_data_SO.InputControlScheme = playerInput.currentControlScheme;
    }

    public void UpdateAllBindingDisplays()
    {
        // NOTE(Jazz): Just completed a rebind, need to update visuals for all binding displays in the ui
        foreach(var rebindable in rebindableRuntimeSet.Items)
        {
            rebindable.UpdateBindingDisplay();
        }
    }

    public void ResetAllBindingOverrides()
    {
        foreach(var rebindable in rebindableRuntimeSet.Items)
        {
            rebindable.ResetBinding();
        }
    }

    private void LateUpdate()
    {
        input_data_SO.interactInput = false;
        _pauseHandledThisFrame = false;
    }

    void ISaveableSettings.PopulateUserSettingsData()
    {
        var rebinds = actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }

    void ISaveableSettings.ApplyUserSettingsData()
    {
        var rebinds = PlayerPrefs.GetString("rebinds");
        if(!string.IsNullOrEmpty(rebinds))
        {
            actions.LoadBindingOverridesFromJson(rebinds);
            UpdateAllBindingDisplays();
        }
    }
}
