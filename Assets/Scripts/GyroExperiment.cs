using System.Collections.Generic;
using UnityEngine;
using PackageUtils;
using SnapUnity;
public class GyroExperiment : Experiment
{

    // Start is called before the first frame update
    public float rotationSpeed = 2.0f;
    public Vector3 rotationVector = Vector3.zero;
    public Vector3 rotationMultiplier = Vector3.one;
    // Store velocity over time
    public Vector3 velocity = Vector3.zero;
    public GameObject controlledObject; // Reference to the object to move

    protected override void Start()
    {
        base.Start();
        // do everything else this experiment needs to setup
        if (connectedToToolbox)
        {
            SetupPackages();
        }
        Debug.Log("Custom setup finished");

    }


    void ApplyAux(Vector3 accelerometerData)
    {
        // DeltaTime to keep calculations frame rate independent
        float deltaTime = Time.deltaTime;

        // Accelerometer data is assumed to be in the form of Vector3 (x, y, z)
        // Acceleration -> Velocity -> Position

        // Update velocity based on accelerometer input
        velocity += accelerometerData * deltaTime;

        // Apply the velocity to the object's position
        controlledObject.transform.Translate(velocity * deltaTime);

        // Optionally, dampen the velocity to avoid drift (friction effect)
        velocity *= 0.98f; // You can tune this value
    }

    private void SetupPackages()
    {
    }

    protected override void OnPackageUpdate(int id, List<float> sample)
    {
        //Debug.Log(id + string.Join(", ", sample));
        // Do something with the position data received from the Client        
        base.OnPackageUpdate(id, sample);
        Dictionary<string, float> sampleDict = base.MapSampleToKeys(id, sample);
        MainThreadDispatcher.QueueOnMainThread(() => {
            applyAux(sampleDict["AUX_0"], sampleDict["AUX_1"], sampleDict["AUX_2"]);
        });
    }

    void applyAux(float aux_0, float aux_1, float aux_2)
    {
        rotationVector = new Vector3(aux_0 * rotationMultiplier.x, aux_1 * rotationMultiplier.y, aux_2 * rotationMultiplier.z);
        this.transform.position += rotationVector;
        //ApplyAux(rotationVector);
        //this.transform.Rotate(rotationVector, rotationSpeed);
    }
    void PrintDictionary(Dictionary<string, string> dict)
    {
        foreach (var kvp in dict)
        {
            Debug.Log("Key: " + kvp.Key + ", Value: " + kvp.Value);
        }
    }

    public void ProcessCommand(string command)
    {
    }
    public override void OnSettingsUpdate(int id, Dictionary<string, object> settings)
    {

    }

    // Update is called once per frame
    void Update()
    { 
        ApplyAux(rotationVector);
    }
}

