using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour
{
    // Start is called before the first frame update
    public float blinkDuration = 0.5f;
    public GameObject light; 
    public GameObject lightBase;
    public Vector3 rotationAxis = Vector3.forward;
    public bool lightIsOn = false;
    void Start()
    {
        StartCoroutine(ToggleObjectRoutine());
    }

    IEnumerator ToggleObjectRoutine()
    {
        while (true) // Infinite loop
        {
            lightIsOn = true;
            // Enable the object
            light.SetActive(lightIsOn);
            lightBase.SetActive(!lightIsOn);

            // Wait for 1 second
            yield return new WaitForSeconds(blinkDuration);

            lightIsOn = false;
            // Disable the object
            light.SetActive(lightIsOn);
            lightBase.SetActive(!lightIsOn);

            // Wait for 1 second
            yield return new WaitForSeconds(blinkDuration);
        }
    }
}
