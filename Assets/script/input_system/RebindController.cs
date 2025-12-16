using Proselyte.Sigils;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

////TODO(Jazz): localization support

////TODO(Jazz): deal with composites that have parts bound in different control schemes

/// <summary>
/// A reusable component with a self-contained UI for rebinding a single action.
/// </summary>
public class RebindController : MonoBehaviour
{
    [Header("Binding References")]
    [Tooltip("Reference to action that is to be rebound from the UI.")]
    [SerializeField] InputActionReference actionReference;
    [SerializeField] string bindingId;
    [SerializeField] InputBinding.DisplayStringOptions displayStringOptions;
    [SerializeField] StringReference InputControlScheme; // NOTE(Jazz): used for single player to get the control scheme

    [Header("UI References")]
    [SerializeField] KeybindIconCache keybindIconCache; // Contains all keybind glyphs
    [SerializeField] Image keybindImage;
    [SerializeField] TMP_Text bindingLabel;

    [Header("Game Events OUT")]
    [SerializeField] GameEvent OnRebindCompleted;

    [Header("Runtime Sets")]
    [SerializeField] RebindableRuntimeSet resettableBindingRuntimeSet;

    private InputActionRebindingExtensions.RebindingOperation _rebindOp;

    private void OnEnable()
    {
        resettableBindingRuntimeSet.Add(this);
    }

    private void OnDisable()
    {
        _rebindOp?.Dispose();
        _rebindOp = null;

        resettableBindingRuntimeSet.Remove(this);
    }

    // Return the action and binding index for the binding that is targeted by the component
    public bool ResolveActionAndBinding(out InputAction action, out int bindingIndex)
    {
        bindingIndex = -1;

        action = actionReference?.action;
        if(action == null)
            return false;

        if(string.IsNullOrEmpty(this.bindingId))
            return false;

        // Look up binding index.
        var bindingId = new System.Guid(this.bindingId);
        bindingIndex = action.bindings.IndexOf(x => x.id == bindingId);
        if(bindingIndex == -1)
        {
            Debug.LogError($"Cannot find binding with ID '{bindingId}' on '{action}'", this);
            return false;
        }

        return true;
    }

    // Update the button label to show rebind status as implicit implementation of IResettable
    public void UpdateBindingDisplay()
    {
        if(bindingLabel == null)
            return;

        var displayString = string.Empty;
        var deviceLayoutName = default(string);
        var controlPath = default(string);

        // Get display string from action.
        var action = actionReference?.action;
        if(action != null)
        {
            var bindingIndex = action.bindings.IndexOf(x => x.id.ToString() == bindingId);
            if(bindingIndex != -1)
            {
                displayString = action.GetBindingDisplayString(bindingIndex, out deviceLayoutName, out controlPath, displayStringOptions);
                InputBinding binding = action.bindings[bindingIndex];
            }
        }

        // NOTE(Jazz): Process control path for getting the sprite glyphs
        if(!string.IsNullOrEmpty(controlPath) && !controlPath.Contains("/"))
        {
            controlPath = $"{deviceLayoutName.ToLowerInvariant()}/{controlPath}";
        }

        var icon = default(Sprite);

        switch(InputControlScheme.Value)
        {
            case "KeyboardMouse":
            {
                icon = keybindIconCache.KBMIcons.GetSprite(controlPath);
            }
            break;
            case "DualShockGamepad":
            {
                icon = keybindIconCache.PS4Icons.GetSprite(controlPath);
            }
            break;
            case "Gamepad":
            {
                icon = keybindIconCache.XboxIcons.GetSprite(controlPath);
            }
            break;
            default:
            {
                Debug.LogWarning($"Unknown or unsupported control scheme: {InputControlScheme.Value} or control path: {controlPath}, by {gameObject.name}");
            }
            break;
        }

        // Show glyph icon for keybind if found, otherwise show updated binding label
        if(icon != null)
        {
            bindingLabel.text = string.Empty;
            keybindImage.sprite = icon;
            keybindImage.enabled = true;
        }
        else
        {
            Debug.LogWarning($"Glyph not found for: controlPath = {controlPath}, deviceLayoutName = {deviceLayoutName}, on gameObject = {gameObject.name}. Reverting to text binding label.");
            bindingLabel.text = displayString;
            keybindImage.enabled = false;
        }
    }

    // Initiate an interactive rebind that lets the player actuate a control to choose a new binding for the action.
    public void StartInteractiveRebind()
    {
        if(!ResolveActionAndBinding(out var action, out var bindingIndex))
            return;

        // If the binding is a composite, we need to rebind each part in turn.
        if(action.bindings[bindingIndex].isComposite)
        {
            var firstPartIndex = bindingIndex + 1;
            if(firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
                PerformInteractiveRebind(action, firstPartIndex, allCompositeParts: true);
        }
        else
        {
            PerformInteractiveRebind(action, bindingIndex);
        }
    }

    private void PerformInteractiveRebind(InputAction action, int bindingIndex, bool allCompositeParts = false)
    {
        action.actionMap.Disable();
        _rebindOp?.Cancel(); // Will null out m_RebindOperation.

        void CleanUp()
        {
            _rebindOp?.Dispose();
            _rebindOp = null;
            action.Enable();
        }

        //Fixes the "InvalidOperationException: Cannot rebind action x while it is enabled" error
        action.Disable();
        bindingLabel.text = "Waiting for input...";
        Debug.Log("Performing interactive rebind with action: " + action.name);

        // Configure the rebind.
        _rebindOp = action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("<Pointer>/position")
            .WithControlsExcluding("<Keyboard>/printScreen")
            .WithControlsExcluding("<Keyboard>/scrollLock")
            .WithControlsExcluding("<Keyboard>/pause")
            .WithControlsExcluding("<Keyboard>/contextMenu")
            .WithControlsExcluding("<Keyboard>/capsLock")
            .WithControlsExcluding("<Keyboard>/backquote")
            .WithControlsExcluding("<Keyboard>/quote")
            .WithControlsExcluding("<Keyboard>/slash")

            .WithCancelingThrough("<Keyboard>/escape")

            .OnCancel(operation =>
            {
                CleanUp();
                Debug.Log("Updating rebind display due to cancelled interactive rebinding.");
                UpdateBindingDisplay();
            })
            .OnComplete(operation =>
            {
                CleanUp();

                if(allCompositeParts)
                {
                    var nextBindingIndex = bindingIndex + 1;
                    if(nextBindingIndex < action.bindings.Count && action.bindings[nextBindingIndex].isPartOfComposite)
                    {
                        PerformInteractiveRebind(action, nextBindingIndex, true);
                    }
                    else
                    {
                        // All parts rebound — re-enable the action map now
                        action.actionMap.Enable();
                        OnRebindCompleted.Raise();
                    }
                }
                else
                {
                    // Single rebind — re-enable the action map
                    action.actionMap.Enable();
                    OnRebindCompleted.Raise();
                }
            });

        _rebindOp.Start();
    }

    #region Explicit Interface Implementations
    // Remove currently applied binding overrides.
    public void ResetBinding()
    {
        if(!ResolveActionAndBinding(out var action, out var bindingIndex))
            return;

        if(action.bindings[bindingIndex].isComposite)
        {
            // It's a composite. Remove overrides from part bindings.
            for(var i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; ++i)
                action.RemoveBindingOverride(i);
        }
        else
        {
            action.RemoveBindingOverride(bindingIndex);
        }
        Debug.Log("Binding reset: updating binding display.");
        UpdateBindingDisplay();
    }
    #endregion

#if UNITY_EDITOR
    private void OnValidate()
    {
        UpdateBindingDisplay();
    }
#endif
}
