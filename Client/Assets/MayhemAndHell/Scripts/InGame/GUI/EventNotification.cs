using UnityEngine;
using System.Collections;

public class EventNotification : MonoBehaviour
{

    public int Index;
    public EventNotifications Notifications;
    public UILabel Text;

	// Use this for initialization
	void Start () {
	
	}

    public void OnDisappear()
    {
        Notifications.InactivateNotification(Index);
    }
}
