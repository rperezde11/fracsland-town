using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// USed to show notificactions of imporant events in the game like
/// getting fractions (this notifications are more imporant than
/// the floating texts)
/// </summary>
public class NotificationManager : MonoBehaviour {

    public static NotificationManager instance;

    public struct Notification
    {
        public Sprite image;
        public string title;
        public string description;
        public float time;
        public int top;
        public int down;

        public Notification(Sprite img, string tit, string desc, float t, int tp, int dw)
        {
            image = img;
            title = tit;
            description = desc;
            time = t;
            top = tp;
            down = dw;
        }
    }

    GameObject notification;
    Animator notificationAnimator;
    private float hideNotificationTime;

    private const int SHOW_NOTIFICATION = 1;
    private const int HIDE_NOTIFICATION = 0;

    public Sprite messageSprite, alertSprite, starSprite;

    private bool isNotificating, isHidingNotification;

    public enum NotificationType { MESSAGE, ALERT, STAR }
    public List<Notification> notificationQueue;

    void Awake()
    {
        notification = GameObject.Find("Notification");
        notificationAnimator = notification.GetComponent<Animator>();
        isNotificating = false;
        isHidingNotification = false;
        notificationQueue = new List<Notification>();
        instance = this;
    }
	
	void Update () {

        if(isNotificating && Time.time > hideNotificationTime)
        {
            HideCurrentNotification();
        }

        if(isHidingNotification && Time.time > hideNotificationTime)
        {
            isHidingNotification = false;
            if(notificationQueue.Count > 0)
            {
                showNotification(notificationQueue[0]);
            }
        }
	}

    private void HideCurrentNotification()
    {
        notificationAnimator.SetInteger("STATE", HIDE_NOTIFICATION);
        isHidingNotification = true;
        hideNotificationTime = Time.time + 1f;
        isNotificating = false;
    }

    private void showNotification(Notification notif)
    {
        notification.transform.Find("NotificationTitle").GetComponent<Text>().text = notif.title;
        notification.transform.Find("NotificationDescription").GetComponent<Text>().text = notif.description;
        notification.transform.Find("NotificationImage").GetComponent<Image>().sprite = notif.image;
        //We need to handle fractions...
        if (notif.down != int.MinValue && notif.top != int.MinValue)
        {
            notification.transform.Find("NotificationTop").GetComponent<Text>().text = notif.top.ToString();
            notification.transform.Find("NotificationDown").GetComponent<Text>().text = notif.down.ToString();
        }
        else
        {
            notification.transform.Find("NotificationTop").GetComponent<Text>().text = "";
            notification.transform.Find("NotificationDown").GetComponent<Text>().text = "";
        }
        notificationAnimator.SetInteger("STATE", SHOW_NOTIFICATION);
        hideNotificationTime = Time.time + notif.time;
        isNotificating = true;
        notificationQueue.RemoveAt(0);
    }

    public void ClearAndHideNotifications()
    {
        notificationQueue.Clear();
        HideCurrentNotification();
    }

    public void addNotification(NotificationType type, string title, string description, float time, int top = int.MinValue, int down = int.MinValue)
    {
        Sprite image = null;
        switch (type)
        {
            case NotificationType.MESSAGE:
                image = messageSprite;
                break;
            case NotificationType.ALERT:
                image = alertSprite;
                break;
            case NotificationType.STAR:
                image = starSprite;
                break;
            default:
                break;
        }
        addNotification(image, title, description, time);

    }

    public void addNotification(Sprite image, string title, string description, float time, int top = int.MinValue, int down = int.MinValue)
    {
        notificationQueue.Add(new Notification(image, title, description, time, top, down));

        if (!isHidingNotification && !isNotificating)
        {
            showNotification(notificationQueue[0]);
        }
    }

    public void showNotification(NotificationType type, string title, string description, float time)
    {
        notification.transform.Find("NotificationTitle").GetComponent<Text>().text = title;
        notification.transform.Find("NotificationDescription").GetComponent<Text>().text = description;
        notificationAnimator.SetInteger("STATE", SHOW_NOTIFICATION);
        hideNotificationTime = Time.time + time;
    }
}
