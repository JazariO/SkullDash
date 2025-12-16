using System;
using UnityEngine;

[CreateAssetMenu(fileName = "KeybindIconCache", menuName = "IconSO/KeybindIconCache")]
public class KeybindIconCache : ScriptableObject
{
    public KeyboardMouseIcons KBMIcons;
    public GamepadIcons PS4Icons;
    public GamepadIcons XboxIcons;
    public GamepadIcons NSwitchProIcons;

    [Serializable]
    public struct KeyboardMouseIcons
    {
        public Sprite Num_0_Key_Light;
        public Sprite Num_1_Key_Light;
        public Sprite Num_2_Key_Light;
        public Sprite Num_3_Key_Light;
        public Sprite Num_4_Key_Light;
        public Sprite Num_5_Key_Light;
        public Sprite Num_6_Key_Light;
        public Sprite Num_7_Key_Light;
        public Sprite Num_8_Key_Light;
        public Sprite Num_9_Key_Light;
        public Sprite Num_10_Key_Light;
        public Sprite Num_11_Key_Light;
        public Sprite Num_12_Key_Light;
        public Sprite A_Key_Light;
        public Sprite B_Key_Light;
        public Sprite C_Key_Light;
        public Sprite D_Key_Light;
        public Sprite E_Key_Light;
        public Sprite F_Key_Light;
        public Sprite G_Key_Light;
        public Sprite H_Key_Light;
        public Sprite I_Key_Light;
        public Sprite J_Key_Light;
        public Sprite K_Key_Light;
        public Sprite L_Key_Light;
        public Sprite M_Key_Light;
        public Sprite N_Key_Light;
        public Sprite O_Key_Light;
        public Sprite P_Key_Light;
        public Sprite Q_Key_Light;
        public Sprite R_Key_Light;
        public Sprite S_Key_Light;
        public Sprite T_Key_Light;
        public Sprite U_Key_Light;
        public Sprite V_Key_Light;
        public Sprite W_Key_Light;
        public Sprite X_Key_Light;
        public Sprite Y_Key_Light;
        public Sprite Z_Key_Light;
        public Sprite Arrow_Down_Key_Light;
        public Sprite Arrow_Left_Key_Light;
        public Sprite Arrow_Right_Key_Light;
        public Sprite Arrow_Up_Key_Light;
        public Sprite Asterisk_Key_Light;
        public Sprite Backspace_Alt_Key_Light;
        public Sprite Backspace_Key_Light;
        public Sprite Bracket_Left_Key_Light;
        public Sprite Bracket_Right_Key_Light;
        public Sprite Alt_Key_Light;
        public Sprite Caps_Lock_Key_Light;
        public Sprite Command_Key_Light;
        public Sprite Ctrl_Key_Light;
        public Sprite Del_Key_Light;
        public Sprite End_Key_Light;
        public Sprite Enter_Alt_Key_Light;
        public Sprite Enter_Key_Light;
        public Sprite Enter_Tall_Key_Light;
        public Sprite Esc_Key_Light;
        public Sprite F1_Key_Light;
        public Sprite F2_Key_Light;
        public Sprite F3_Key_Light;
        public Sprite F4_Key_Light;
        public Sprite F5_Key_Light;
        public Sprite F6_Key_Light;
        public Sprite F7_Key_Light;
        public Sprite F8_Key_Light;
        public Sprite F9_Key_Light;
        public Sprite F10_Key_Light;
        public Sprite F11_Key_Light;
        public Sprite F12_Key_Light;
        public Sprite Home_Key_Light;
        public Sprite Insert_Key_Light;
        public Sprite Mark_Left_Key_Light;
        public Sprite Mark_Right_Key_Light;
        public Sprite Minus_Key_Light;
        public Sprite Mouse_Left_Key_Light;
        public Sprite Mouse_Middle_Key_Light;
        public Sprite Mouse_Right_Key_Light;
        public Sprite Mouse_Simple_Key_Light;
        public Sprite Num_Lock_Key_Light;
        public Sprite Page_Down_Key_Light;
        public Sprite Page_Up_Key_Light;
        public Sprite Plus_Key_Light;
        public Sprite Plus_Tall_Key_Light;
        public Sprite Print_Screen_Key_Light;
        public Sprite Question_Key_Light;
        public Sprite Quote_Key_Light;
        public Sprite Semicolon_Key_Light;
        public Sprite Shift_Alt_Key_Light;
        public Sprite Shift_Key_Light;
        public Sprite Slash_Key_Light;
        public Sprite Space_Key_Light;
        public Sprite Tab_Key_Light;
        public Sprite Tilda_Key_Light;
        public Sprite Win_Key_Light;

        public Sprite GetSprite(string controlPath)
        {
            switch(controlPath)
            {
                // Keyboard keys
                case "keyboard/a": return A_Key_Light;
                case "keyboard/b": return B_Key_Light;
                case "keyboard/c": return C_Key_Light;
                case "keyboard/d": return D_Key_Light;
                case "keyboard/e": return E_Key_Light;
                case "keyboard/f": return F_Key_Light;
                case "keyboard/g": return G_Key_Light;
                case "keyboard/h": return H_Key_Light;
                case "keyboard/i": return I_Key_Light;
                case "keyboard/j": return J_Key_Light;
                case "keyboard/k": return K_Key_Light;
                case "keyboard/l": return L_Key_Light;
                case "keyboard/m": return M_Key_Light;
                case "keyboard/n": return N_Key_Light;
                case "keyboard/o": return O_Key_Light;
                case "keyboard/p": return P_Key_Light;
                case "keyboard/q": return Q_Key_Light;
                case "keyboard/r": return R_Key_Light;
                case "keyboard/s": return S_Key_Light;
                case "keyboard/t": return T_Key_Light;
                case "keyboard/u": return U_Key_Light;
                case "keyboard/v": return V_Key_Light;
                case "keyboard/w": return W_Key_Light;
                case "keyboard/x": return X_Key_Light;
                case "keyboard/y": return Y_Key_Light;
                case "keyboard/z": return Z_Key_Light;

                // Function keys
                case "keyboard/f1": return F1_Key_Light;
                case "keyboard/f2": return F2_Key_Light;
                case "keyboard/f3": return F3_Key_Light;
                case "keyboard/f4": return F4_Key_Light;
                case "keyboard/f5": return F5_Key_Light;
                case "keyboard/f6": return F6_Key_Light;
                case "keyboard/f7": return F7_Key_Light;
                case "keyboard/f8": return F8_Key_Light;
                case "keyboard/f9": return F9_Key_Light;
                case "keyboard/f10": return F10_Key_Light;
                case "keyboard/f11": return F11_Key_Light;
                case "keyboard/f12": return F12_Key_Light;

                // Special keys
                case "keyboard/space": return Space_Key_Light;
                case "keyboard/enter": return Enter_Key_Light;
                case "keyboard/escape": return Esc_Key_Light;
                case "keyboard/tab": return Tab_Key_Light;
                case "keyboard/backspace": return Backspace_Key_Light;
                case "keyboard/leftCtrl": return Ctrl_Key_Light;
                case "keyboard/rightCtrl": return Ctrl_Key_Light;
                case "keyboard/leftAlt": return Alt_Key_Light;
                case "keyboard/rightAlt": return Alt_Key_Light;
                case "keyboard/leftShift": return Shift_Key_Light;
                case "keyboard/rightShift": return Shift_Key_Light;
                case "keyboard/capsLock": return Caps_Lock_Key_Light;
                case "keyboard/printScreen": return Print_Screen_Key_Light;
                case "keyboard/insert": return Insert_Key_Light;
                case "keyboard/delete": return Del_Key_Light;
                case "keyboard/home": return Home_Key_Light;
                case "keyboard/end": return End_Key_Light;
                case "keyboard/pageUp": return Page_Up_Key_Light;
                case "keyboard/pageDown": return Page_Down_Key_Light;
                case "keyboard/upArrow": return Arrow_Up_Key_Light;
                case "keyboard/downArrow": return Arrow_Down_Key_Light;
                case "keyboard/leftArrow": return Arrow_Left_Key_Light;
                case "keyboard/rightArrow": return Arrow_Right_Key_Light;

                // Mouse buttons
                case "mouse/leftButton": return Mouse_Left_Key_Light;
                case "mouse/rightButton": return Mouse_Right_Key_Light;
                case "mouse/middleButton": return Mouse_Middle_Key_Light;

                // Top-row number keys (above letters)
                case "keyboard/1": return Num_1_Key_Light;
                case "keyboard/2": return Num_2_Key_Light;
                case "keyboard/3": return Num_3_Key_Light;
                case "keyboard/4": return Num_4_Key_Light;
                case "keyboard/5": return Num_5_Key_Light;
                case "keyboard/6": return Num_6_Key_Light;
                case "keyboard/7": return Num_7_Key_Light;
                case "keyboard/8": return Num_8_Key_Light;
                case "keyboard/9": return Num_9_Key_Light;
                case "keyboard/0": return Num_0_Key_Light;

                // Numpad keys
                case "keyboard/numpad0": return Num_0_Key_Light;
                case "keyboard/numpad1": return Num_1_Key_Light;
                case "keyboard/numpad2": return Num_2_Key_Light;
                case "keyboard/numpad3": return Num_3_Key_Light;
                case "keyboard/numpad4": return Num_4_Key_Light;
                case "keyboard/numpad5": return Num_5_Key_Light;
                case "keyboard/numpad6": return Num_6_Key_Light;
                case "keyboard/numpad7": return Num_7_Key_Light;
                case "keyboard/numpad8": return Num_8_Key_Light;
                case "keyboard/numpad9": return Num_9_Key_Light;
                case "keyboard/numpadEnter": return Enter_Alt_Key_Light;
                case "keyboard/numpadPlus": return Plus_Key_Light;
                case "keyboard/numpadMinus": return Minus_Key_Light;
                case "keyboard/numLock": return Num_Lock_Key_Light;

                // Symbols
                case "keyboard/slash": return Slash_Key_Light;
                case "keyboard/semicolon": return Semicolon_Key_Light;
                case "keyboard/quote": return Quote_Key_Light;
                case "keyboard/leftBracket": return Bracket_Left_Key_Light;
                case "keyboard/rightBracket": return Bracket_Right_Key_Light;
                case "keyboard/tilde": return Tilda_Key_Light;
                case "keyboard/minus": return Minus_Key_Light;
                case "keyboard/equals": return Plus_Key_Light;

                default:
                    Debug.LogWarning($"No sprite mapped for controlPath: {controlPath}");
                    return null;
            }
        }
    }

    [Serializable]
    public struct GamepadIcons
    {
        public Sprite buttonSouth;
        public Sprite buttonNorth;
        public Sprite buttonEast;
        public Sprite buttonWest;
        public Sprite startButton;
        public Sprite selectButton;
        public Sprite leftTrigger;
        public Sprite rightTrigger;
        public Sprite leftShoulder;
        public Sprite rightShoulder;
        public Sprite dpad;
        public Sprite dpadUp;
        public Sprite dpadDown;
        public Sprite dpadLeft;
        public Sprite dpadRight;
        public Sprite leftStick;
        public Sprite rightStick;
        public Sprite leftStickPress;
        public Sprite rightStickPress;

        public Sprite GetSprite(string controlPath)
        {
            // From the input system, we get the path of the control on device. So we can just
            // map from that to the sprites we have for gamepads.
            switch(controlPath)
            {
                case "buttonSouth": return buttonSouth;
                case "buttonNorth": return buttonNorth;
                case "buttonEast": return buttonEast;
                case "buttonWest": return buttonWest;
                case "start": return startButton;
                case "select": return selectButton;
                case "leftTrigger": return leftTrigger;
                case "rightTrigger": return rightTrigger;
                case "leftShoulder": return leftShoulder;
                case "rightShoulder": return rightShoulder;
                case "dpad": return dpad;
                case "dpad/up": return dpadUp;
                case "dpad/down": return dpadDown;
                case "dpad/left": return dpadLeft;
                case "dpad/right": return dpadRight;
                case "leftStick": return leftStick;
                case "rightStick": return rightStick;
                case "leftStickPress": return leftStickPress;
                case "rightStickPress": return rightStickPress;
                default:
                    return null;
            }
        }
    }
}


