using UnityEngine;

namespace PrisonIsland.Core
{
    public class CameraController : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 20f;
        public float edgeScrollSize = 20f;
        public bool useEdgeScrolling = true;
        public bool useKeyboardMovement = true;

        [Header("Rotation")]
        public float rotationSpeed = 100f;

        [Header("Zoom")]
        public float zoomSpeed = 20f;
        public float minZoom = 10f;
        public float maxZoom = 50f;
        private float currentZoom = 30f;

        [Header("Bounds")]
        public float minX = -50f;
        public float maxX = 50f;
        public float minZ = -50f;
        public float maxZ = 50f;

        [Header("Touch Controls")]
        public float touchPanSpeed = 0.1f;
        public float touchZoomSpeed = 0.1f;

        private Vector3 lastMousePosition;
        private Camera cam;

        private void Start()
        {
            cam = GetComponent<Camera>();
        }

        private void Update()
        {
            HandleKeyboardMovement();
            HandleEdgeScrolling();
            HandleMouseDrag();
            HandleRotation();
            HandleZoom();
            HandleTouchInput();

            ClampPosition();
        }

        private void HandleKeyboardMovement()
        {
            if (!useKeyboardMovement) return;

            Vector3 move = Vector3.zero;

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                move += transform.forward;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                move -= transform.forward;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                move -= transform.right;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                move += transform.right;

            move.y = 0;
            transform.position += move.normalized * moveSpeed * Time.deltaTime;
        }

        private void HandleEdgeScrolling()
        {
            if (!useEdgeScrolling) return;

            Vector3 move = Vector3.zero;

            if (Input.mousePosition.x < edgeScrollSize)
                move -= transform.right;
            if (Input.mousePosition.x > Screen.width - edgeScrollSize)
                move += transform.right;
            if (Input.mousePosition.y < edgeScrollSize)
                move -= transform.forward;
            if (Input.mousePosition.y > Screen.height - edgeScrollSize)
                move += transform.forward;

            move.y = 0;
            transform.position += move.normalized * moveSpeed * Time.deltaTime;
        }

        private void HandleMouseDrag()
        {
            // Middle mouse button drag
            if (Input.GetMouseButtonDown(2))
            {
                lastMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(2))
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;
                Vector3 move = new Vector3(-delta.x, 0, -delta.y);
                transform.position += move * moveSpeed * Time.deltaTime * 0.1f;
                lastMousePosition = Input.mousePosition;
            }
        }

        private void HandleRotation()
        {
            if (Input.GetKey(KeyCode.Q))
            {
                transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime, Space.World);
            }
            if (Input.GetKey(KeyCode.E))
            {
                transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
            }
        }

        private void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            currentZoom -= scroll * zoomSpeed;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

            // Adjust camera position based on zoom
            Vector3 direction = transform.forward;
            direction.Normalize();

            // You might want to adjust this based on your camera setup
            transform.position = transform.position - direction * scroll * zoomSpeed;
        }

        private void HandleTouchInput()
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved)
                {
                    Vector3 move = new Vector3(-touch.deltaPosition.x, 0, -touch.deltaPosition.y);
                    transform.position += move * touchPanSpeed;
                }
            }
            else if (Input.touchCount == 2)
            {
                Touch touch0 = Input.GetTouch(0);
                Touch touch1 = Input.GetTouch(1);

                Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
                Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

                float prevMagnitude = (touch0PrevPos - touch1PrevPos).magnitude;
                float currentMagnitude = (touch0.position - touch1.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;

                Vector3 direction = transform.forward;
                direction.Normalize();
                transform.position -= direction * difference * touchZoomSpeed;
            }
        }

        private void ClampPosition()
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
            transform.position = pos;
        }
    }
}
