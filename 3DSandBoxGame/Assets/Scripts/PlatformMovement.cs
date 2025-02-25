using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{

    public Transform startPoint;
    public Transform endPoint;
    public float lerpSpeed = 1f;

    private float lerpTime = 0f;


    // Update is called once per frame
    void Update()
    {
        lerpTime += Time.deltaTime * lerpSpeed;
        if (lerpTime > 1)
        {
            lerpTime = 0;
        }

        transform.position = Vector3.Lerp(startPoint.position, endPoint.position, lerpTime);
    }
}