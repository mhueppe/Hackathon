using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    // Start is called before the first frame update
    public float rotationSpeed = 2; 
    public Vector3 rotationAxis = Vector3.forward;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);

    }
}
