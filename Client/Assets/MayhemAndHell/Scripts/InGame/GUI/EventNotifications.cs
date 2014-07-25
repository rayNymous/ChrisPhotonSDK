using System;
using System.Linq;
using ExitGames.Client.Photon;
using UnityEngine;
using System.Collections;

public class EventNotifications : MonoBehaviour
{
    private UITable _table;
    private GameObject _notificationPrefab;

    private const int MaxNotifications = 5;

    private GameObject[] _notifications;
    private int[] _ids;
    private int _lastId = 1;


	// Use this for initialization
	void Start ()
	{
        _notificationPrefab = Resources.Load<GameObject>("Prefabs/Gui/EventNotification");
	    _table = GetComponent<UITable>();

	    _ids = new int [MaxNotifications];
        _notifications = new GameObject[MaxNotifications];

	    for (int i = 0; i < MaxNotifications; i++)
	    {
	        _ids[i] = 0;
            _notifications[i] = NGUITools.AddChild(_table.gameObject, _notificationPrefab);
            _notifications[i].GetComponent<EventNotification>().Index = i;
	        _notifications[i].GetComponent<EventNotification>().Notifications = this;
            NGUITools.SetActive(_notifications[i], false);
	    }
	}

    public void AddNotification(String notification)
    {
        int index = GetLastIndex();
        _ids[index] = _lastId++;
        _notifications[index].name = _ids[index] + "";
        _notifications[index].GetComponent<TweenAlpha>().ResetToBeginning();
        _notifications[index].GetComponent<TweenAlpha>().PlayForward();
        _notifications[index].GetComponent<UILabel>().text = notification;
        NGUITools.SetActive(_notifications[index], true);
        _table.Reposition();
    }

    private int GetLastIndex()
    {
        int index = 0;
        int minValue = int.MaxValue;
        for (int i = 0; i < MaxNotifications; i++)
        {
            if (_ids[i] < minValue)
            {
                minValue = _ids[i];
                index = i;
            }
        }
        return index;
    }

    public void InactivateNotification(int index)
    {
        NGUITools.SetActive(_notifications[index], false);
        _table.Reposition();
    }

    public void Test()
    {
        AddNotification("A rather long notification");
    }
}
