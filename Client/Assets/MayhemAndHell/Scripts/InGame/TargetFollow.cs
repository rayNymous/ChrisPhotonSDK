using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TargetFollow : MonoBehaviour
{
    public WorldObject Target;

    public Boolean IgnoreZ = false;

    private Transform _startingPosition;

    private Vector3 _originalScale;

    void Start()
    {
        _startingPosition = transform;
        _originalScale = transform.localScale;

        if (Target != null)
        {
            Enable(Target);
        }
    }

    void Update()
    {
        if (Target != null)
        {
            if (IgnoreZ)
            {
                this.transform.position = new Vector3(Target.BottomPosition.transform.position.x,
                    Target.BottomPosition.transform.position.y,
                    _startingPosition.position.z);
            }
            else
            {
                this.transform.position = Target.BottomPosition.transform.position;
            }
        }
    }

    public void Disable()
    {
        NGUITools.SetActive(gameObject, false);
    }

    public void Enable(WorldObject target)
    {
        Target = target;
        NGUITools.SetActive(gameObject, true);
        transform.localScale = target.BottomPosition.transform.localScale;
    }

}
