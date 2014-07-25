using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{

    public Transform target;
    private float smoothTime = 0.3f;
    private Vector2 _velocity = new Vector2(10,10);

	// Use this for initialization
	void Start ()
	{
        GameObject obj = GameObject.FindGameObjectWithTag("Player");

        if (obj != null)
        {
            target = obj.transform;
        }
	    
	}
	
	// Update is called once per frame
	void LateUpdate () {
	    if (target == null)
	    {
	        return;
	    }

	    Vector3 position = transform.position;

        position.x = Mathf.SmoothDamp(transform.position.x,
	        target.position.x,ref _velocity.x, smoothTime);
        position.y = Mathf.SmoothDamp(transform.position.y,
            target.position.y, ref _velocity.y, smoothTime);

	    transform.position = position;
	}
}
