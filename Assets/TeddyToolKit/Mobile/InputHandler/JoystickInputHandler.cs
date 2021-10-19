using TeddyToolKit.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TeddyToolKit.Mobile.InputHandler
{
    /// <summary>
    /// handles the onscreen joystick input on mobile
    /// </summary>
    public class JoystickInputHandler : RunnableBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public Vector2 Axis { get; private set; } = Vector2.zero;
        
        [Tooltip("drag the sprite to act as the joystick handle")]
        [SerializeField] private RectTransform handle;
        [Tooltip("drag the sprite to act as the joystick background")]
        [SerializeField] private RectTransform background;
        [Tooltip("higher deadzone = less sensitive joystick")]
        [SerializeField, Range(0, 1)] private float deadzone = .25f;
        
        private MobileInputManager manager;
        
        // The position the handle was originally in.
        private Vector3 initialPosition = Vector3.zero;
        
        protected override void OnSetup(params object[] parameters)
        {
            manager = (MobileInputManager) parameters[0];

            // keep the initial position of the handle
            initialPosition = handle.position;
        }

        protected override void OnRun(params object[] parameters)
        {
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Calculate the half size difference between the background and handle rects
            float xDifference = (background.rect.size.x - handle.rect.size.x) * .5f;
            float yDifference = (background.rect.size.y - handle.rect.size.y) * .5f;
            
            // Calculate the axis of the input based on the event data and the relative
            // position to the background's centre
            Axis = new Vector2()
            {
                x = (eventData.position.x - background.position.x) / xDifference,
                y = (eventData.position.y - background.position.y) / yDifference
            };
            Axis = Vector2.ClampMagnitude(Axis, 1f);
            
            // Apply the axis position to the handles position
            handle.position = new Vector3()
            {
                x = (Axis.x * xDifference) + background.position.x,
                y = (Axis.y * yDifference) + background.position.y
            };
            
            // Apply the deadzone effect after the handle has been placed
            // to prevent the handle from visually being stuck in the deadzone
            Axis = (Axis.magnitude < deadzone) ? Vector2.zero : Axis;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // We have let go so reset the axis and the position of the handle
            Axis = Vector2.zero;
            handle.position = initialPosition;
        }

        public void OnPointerDown(PointerEventData eventData) => OnDrag(eventData);
        public void OnPointerUp(PointerEventData eventData) => OnEndDrag(eventData);
    }

}