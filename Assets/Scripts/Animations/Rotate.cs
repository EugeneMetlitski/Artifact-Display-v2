using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float rotationSpeed = 90.0f;
    public GameObject note;

    private Vector3 currentRotation;

    public void RotateAround()
    {
        //currentRotation = gameObject.transform.eulerAngles;

        note.transform.Rotate(
            new Vector3(1f, 0f, 0f),
            rotationSpeed * Time.deltaTime
        );
    }
}
