using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CamerMovement : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    void Start()
    {
    }

    void Update()
    {
        transform.position = target.position + offset;
    }
}
