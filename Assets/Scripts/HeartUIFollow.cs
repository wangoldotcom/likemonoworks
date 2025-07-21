using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartUIFollow : MonoBehaviour
{
    public Transform playerTransform;
    public Vector3 offset;

    void Update()
    {
        if (playerTransform != null)
        {
            transform.position = playerTransform.position + offset;
        }
    }
}
