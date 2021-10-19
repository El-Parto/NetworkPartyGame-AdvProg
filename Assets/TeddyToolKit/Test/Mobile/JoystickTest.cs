using TeddyToolKit.Mobile.InputHandler;
using UnityEngine;

namespace TeddyToolKit.Test.Mobile
{
    /// <summary>
    /// just a test class to demonstrate the joystick control
    /// </summary>
    public class JoystickTest : MonoBehaviour
    {
        // Update is called once per frame
        private void Update()
        {
            Vector2 joystickAxis = MobileInputManager.GetJoystickAxis();
            
            transform.position += transform.right * (joystickAxis.x * Time.deltaTime);
            transform.position += transform.forward * (joystickAxis.y * Time.deltaTime);
        }
    }
}