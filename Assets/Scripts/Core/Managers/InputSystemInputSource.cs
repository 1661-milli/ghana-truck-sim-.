#if ENABLE_INPUT_SYSTEM
using TrotroNation.Core.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TrotroNation.Core.Managers
{
    /// <summary>
    /// Default <see cref="IInputSource"/> backed by Unity's Input System package.
    /// Compiled only when the Input System is active, so the core code has no hard package dependency.
    ///
    /// Mapping:
    ///   Steering : gamepad left stick X, keyboard A/D, or touch position relative to screen centre
    ///   Throttle : gamepad right trigger, keyboard W, or any touch on the right half of the screen
    ///   Brake    : gamepad left trigger, keyboard S/Space, or any touch on the left half
    ///   Camera   : gamepad right stick press, keyboard C
    ///   Horn     : gamepad east button, keyboard H
    ///   Pause    : Start/Menu button, keyboard Escape
    /// </summary>
    public sealed class InputSystemInputSource : IInputSource
    {
        public RawInputState Poll()
        {
            float steering = 0f;
            float throttle = 0f;
            float brake = 0f;
            bool camera = false;
            bool horn = false;
            bool pause = false;

            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.aKey.isPressed) steering -= 1f;
                if (keyboard.dKey.isPressed) steering += 1f;
                if (keyboard.wKey.isPressed) throttle = 1f;
                if (keyboard.sKey.isPressed || keyboard.spaceKey.isPressed) brake = 1f;
                camera |= keyboard.cKey.wasPressedThisFrame;
                horn |= keyboard.hKey.wasPressedThisFrame;
                pause |= keyboard.escapeKey.wasPressedThisFrame;
            }

            Gamepad gamepad = Gamepad.current;
            if (gamepad != null)
            {
                float stickX = gamepad.leftStick.x.ReadValue();
                if (Mathf.Abs(stickX) > Mathf.Abs(steering)) steering = stickX;
                throttle = Mathf.Max(throttle, gamepad.rightTrigger.ReadValue());
                brake = Mathf.Max(brake, gamepad.leftTrigger.ReadValue());
                camera |= gamepad.rightStickButton.wasPressedThisFrame;
                horn |= gamepad.buttonEast.wasPressedThisFrame;
                pause |= gamepad.startButton.wasPressedThisFrame;
            }

            Touchscreen touch = Touchscreen.current;
            if (touch != null && touch.primaryTouch.press.isPressed)
            {
                Vector2 position = touch.primaryTouch.position.ReadValue();
                float halfWidth = Screen.width * 0.5f;
                if (halfWidth > 0f)
                {
                    float touchSteering = Mathf.Clamp((position.x - halfWidth) / halfWidth, -1f, 1f);
                    if (Mathf.Abs(touchSteering) > Mathf.Abs(steering)) steering = touchSteering;
                }

                if (position.x >= halfWidth) throttle = 1f;
                else brake = 1f;
            }

            return new RawInputState(
                Mathf.Clamp(steering, -1f, 1f),
                Mathf.Clamp01(throttle),
                Mathf.Clamp01(brake),
                camera,
                horn,
                pause);
        }
    }
}
#endif
