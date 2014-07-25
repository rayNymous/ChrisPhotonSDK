using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class EnvironmentObject : MonoBehaviour
{
    void Start()
    {
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            HelperMethods.CalculateZ(transform.position.y));
    }

}