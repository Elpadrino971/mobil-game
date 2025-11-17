using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace PrisonIsland.UI
{
    /// <summary>
    /// Manages in-game notifications and alerts
    /// </summary>
    public class NotificationManager : MonoBehaviour
    {
        public static NotificationManager Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject notificationPrefab;
        [SerializeField] private Transform notificationParent;
        [SerializeField] private float notificationDuration = 3f;
        [SerializeField] private int maxNotifications = 5;

        private Queue<GameObject> activeNotifications = new Queue<GameObject>();

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

        private void Start()
        {
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            if (Core.GameEvents.Instance != null)
            {
                Core.GameEvents.Instance.OnInfoMessage += ShowInfo;
                Core.GameEvents.Instance.OnWarning += ShowWarning;
                Core.GameEvents.Instance.OnCriticalAlert += ShowCritical;
            }
        }

        public void ShowInfo(string message)
        {
            ShowNotification(message, NotificationType.Info);
        }

        public void ShowWarning(string message)
        {
            ShowNotification(message, NotificationType.Warning);
        }

        public void ShowCritical(string message)
        {
            ShowNotification(message, NotificationType.Critical);
        }

        private void ShowNotification(string message, NotificationType type)
        {
            // Remove oldest if too many
            if (activeNotifications.Count >= maxNotifications)
            {
                GameObject oldest = activeNotifications.Dequeue();
                Destroy(oldest);
            }

            // Create notification
            GameObject notification = CreateNotification(message, type);
            activeNotifications.Enqueue(notification);

            // Auto-destroy after duration
            Destroy(notification, notificationDuration);
        }

        private GameObject CreateNotification(string message, NotificationType type)
        {
            GameObject notification;

            if (notificationPrefab != null)
            {
                notification = Instantiate(notificationPrefab, notificationParent);
            }
            else
            {
                // Create simple notification if no prefab
                notification = new GameObject("Notification");
                notification.transform.SetParent(notificationParent);

                Image bg = notification.AddComponent<Image>();
                bg.color = GetNotificationColor(type);

                GameObject textObj = new GameObject("Text");
                textObj.transform.SetParent(notification.transform);
                TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
                text.text = message;
                text.fontSize = 14;
                text.alignment = TextAlignmentOptions.Center;
                text.color = Color.white;

                RectTransform rt = notification.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(300, 50);
            }

            // Set message
            TextMeshProUGUI notifText = notification.GetComponentInChildren<TextMeshProUGUI>();
            if (notifText != null)
            {
                notifText.text = message;
            }

            return notification;
        }

        private Color GetNotificationColor(NotificationType type)
        {
            switch (type)
            {
                case NotificationType.Info:
                    return new Color(0.2f, 0.6f, 1f, 0.9f); // Blue
                case NotificationType.Warning:
                    return new Color(1f, 0.8f, 0f, 0.9f); // Yellow
                case NotificationType.Critical:
                    return new Color(1f, 0.2f, 0.2f, 0.9f); // Red
                default:
                    return Color.gray;
            }
        }

        public enum NotificationType
        {
            Info,
            Warning,
            Critical
        }
    }
}
