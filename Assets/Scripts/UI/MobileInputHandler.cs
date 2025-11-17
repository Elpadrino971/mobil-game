using UnityEngine;
using UnityEngine.EventSystems;

namespace PrisonIsland.UI
{
    /// <summary>
    /// Handles mobile-specific input (touch, gestures)
    /// </summary>
    public class MobileInputHandler : MonoBehaviour
    {
        public static MobileInputHandler Instance { get; private set; }

        [Header("Touch Settings")]
        [SerializeField] private float doubleTapTime = 0.3f;
        [SerializeField] private float longPressTime = 0.5f;
        [SerializeField] private float swipeThreshold = 50f;

        private float lastTapTime;
        private Vector2 touchStartPos;
        private float touchStartTime;
        private bool isLongPressing;

        public event System.Action<Vector2> OnTap;
        public event System.Action<Vector2> OnDoubleTap;
        public event System.Action<Vector2> OnLongPress;
        public event System.Action<Vector2, Vector2> OnSwipe;
        public event System.Action<float> OnPinchZoom;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            HandleTouchInput();
        }

        private void HandleTouchInput()
        {
            #if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
            // Single touch
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        OnTouchBegan(touch);
                        break;

                    case TouchPhase.Stationary:
                        CheckLongPress(touch);
                        break;

                    case TouchPhase.Moved:
                        isLongPressing = false;
                        break;

                    case TouchPhase.Ended:
                        OnTouchEnded(touch);
                        break;
                }
            }
            // Pinch zoom with two fingers
            else if (Input.touchCount == 2)
            {
                HandlePinchZoom();
            }
            #endif
        }

        private void OnTouchBegan(Touch touch)
        {
            // Don't process if touching UI
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;

            touchStartPos = touch.position;
            touchStartTime = Time.time;
            isLongPressing = false;
        }

        private void CheckLongPress(Touch touch)
        {
            if (!isLongPressing && Time.time - touchStartTime >= longPressTime)
            {
                isLongPressing = true;
                OnLongPress?.Invoke(touch.position);
                Utilities.HelperFunctions.PlayHaptic();
            }
        }

        private void OnTouchEnded(Touch touch)
        {
            // Don't process if touching UI
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;

            float touchDuration = Time.time - touchStartTime;
            Vector2 touchDelta = touch.position - touchStartPos;

            // Check for swipe
            if (touchDelta.magnitude > swipeThreshold)
            {
                OnSwipe?.Invoke(touchStartPos, touch.position);
                return;
            }

            // Check for long press (already handled)
            if (isLongPressing)
                return;

            // Check for double tap
            if (Time.time - lastTapTime < doubleTapTime)
            {
                OnDoubleTap?.Invoke(touch.position);
                lastTapTime = 0; // Reset to prevent triple tap
            }
            else
            {
                // Single tap
                OnTap?.Invoke(touch.position);
                lastTapTime = Time.time;
            }
        }

        private void HandlePinchZoom()
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float prevMagnitude = (touch0PrevPos - touch1PrevPos).magnitude;
            float currentMagnitude = (touch0.position - touch1.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            OnPinchZoom?.Invoke(difference);
        }

        // Helper method to raycast from touch position
        public bool RaycastFromTouch(Vector2 touchPosition, out RaycastHit hit)
        {
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            return Physics.Raycast(ray, out hit);
        }

        // Helper method to get world position from touch
        public Vector3 GetWorldPositionFromTouch(Vector2 touchPosition, float distance = 100f)
        {
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            return ray.GetPoint(distance);
        }
    }
}
