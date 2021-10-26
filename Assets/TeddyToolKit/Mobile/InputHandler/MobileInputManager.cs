using System.Collections;
using System.Collections.Generic;
using TeddyToolKit.Core;
using UnityEngine;

namespace TeddyToolKit.Mobile.InputHandler
{
    public class MobileInputManager : MonoSingleton<MobileInputManager>
    {
        public JoystickInputHandler joystick;
        public SwipeInputHandler swiping;

        /// <summary> Attempt to get the axis of the joystick attached to the system. </summary>
        public static Vector2 GetJoystickAxis() => Instance.joystick != null ? Instance.joystick.Axis : Vector2.zero;

        // Start is called before the first frame update
        private void Start()
        {
            RunnableUtils.Setup(ref joystick, gameObject, true, this);
            RunnableUtils.Setup(ref swiping, gameObject, true);
        }

        // Update is called once per frame
        private void Update()
        {
            RunnableUtils.Run(ref joystick, gameObject, true);
            RunnableUtils.Run(ref swiping, gameObject, true);
        }
    }
}