using System.Collections.Generic;
using UnityEngine;
using PackageUtils;
using SnapUnity;
public class TestExperiment : Experiment
{
    private byte[] rotationSignal = new byte[3];
    private byte[] positionSignal = new byte[3];
    public float rotationSpeed = 1;
    public Camera cam; 
    public Vector3 rotationAxis = Vector3.up; // The axis around which you want to rotate.

    public GameObject earth;
    public GameObject moon;
    public GameObject sun;
    public Vector3 moving = new Vector3(0.51f, 0, 0);
    List<GameObject> stars = new List<GameObject> { };
    List<string> names = new List<string> { "Earth", "Moon"};


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        stars.Add(earth);
        stars.Add(moon);    
        // do everything else this experiment needs to setup
        if (connectedToToolbox)
        {
            SetupPackages();
        }
        Debug.Log("Custom setup finished");

    }
 

    private void SetupPackages()
    {
        Dictionary<string, object> encoding = new Dictionary<string, object>();

        DataType dataTypeParser = new DataType();
        string[] axes = { "X", "Y", "Z" };
        //foreach (string axis in axes)
        //{
        //    encoding[$"rotation_{axis}"] = new Dictionary<string, object>
        //                                    {
        //                                    { "type", dataTypeParser.GetName(typeof(byte)) },
        //                                    { "unit", "degree" },
        //                                    { "scaler", 1}, 
        //                                    { "endian", "little" }
        //                                    };
        //}

        //foreach (string name in names)
        //{
        //    PackageHeader packageHeader = new PackageHeader(name+"_rotation", encoding, 3);
        //    Package package = new Package(packageHeader);
        //    this.connectionHandler.RegisterPackageSending(name, packageHeader);
        //}

        foreach (string axis in axes)
        {
            encoding[$"position_{axis}"] = new Dictionary<string, object>
                                            {
                                            { "type", dataTypeParser.GetName(typeof(sbyte)) },
                                            { "unit", "pos" },
                                            { "scaler", 1},
                                            { "endian", "little" }
                                            };
        }

        foreach (string name in names)
        {
            PackageHeader packageHeader = new PackageHeader(name + "_position", encoding, 3);
            Package package = new Package(packageHeader);
            this.connectionHandler.RegisterPackageSending(name, packageHeader);
        }
    }

    protected override void OnPackageUpdate(int id, List<float> sample)
    {
        // Do something with the position data received from the Client        
        base.OnPackageUpdate(id, sample);
        MainThreadDispatcher.QueueOnMainThread(() => {
            ProcessCommand(controller.GetCommand(id));
        });
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
        Renderer renderer = sun.GetComponent<Renderer>();

        Color color = renderer.material.color;
        // Find the Renderer component attached to the GameObject

        switch (command){
            case "Red":
                color = Color.red;
                break;
            case "Blue":
                color = Color.blue;
                break;
            case "Green":
                color = Color.green;
                break;
            case "Yellow":
                color = Color.yellow;
                break;
            case "Orange":
                color = new Color(1.0f, 0.5f, 0.0f); // Orange
                break;
            case "Purple":
                color = new Color(0.5f, 0.0f, 0.5f); // Purple
                break;
            case "Pink":
                color = new Color(1.0f, 0.41f, 0.71f); // Pink
                break;
            case "Brown":
                color = new Color(0.6f, 0.4f, 0.2f); // Brown
                break;
            case "Gray":
                color = Color.gray;
                break;
            case "Black":
                color = Color.black;
                break;
            default:
                Debug.LogWarning("Unknown color: " + command);
                break;
        }

        if (renderer != null)
        {
            // Create a copy of the material to avoid affecting other objects
            Material material = new Material(renderer.material);

            // Change the color property of the material
            material.color = color;

            // Assign the modified material to the GameObject
            renderer.material = material;

        }
    }
    public override void OnSettingsUpdate(int id, Dictionary<string, object> settings)
    {
        base.OnSettingsUpdate(id, settings);
        rotationSpeed = GetSetting<float>("rotation_speed");
        string ax = GetSetting<string>("rotation_axis");
        switch (ax)
        {
            case "Up":
                rotationAxis = Vector3.up;
                break;
            case "Down":
                rotationAxis = Vector3.down;
                break;
            case "Right":
                rotationAxis = Vector3.right;
                break;
            case "Left":
                rotationAxis = Vector3.left;
                break;
            case "Forward":
                rotationAxis = Vector3.forward;
                break;
            case "Back":
                rotationAxis = Vector3.back;
                break;
            default: break;
        }
    }

    public byte FormatRotation(float rotation)
    {
        return (byte) (int) (((rotation % 360) / 360) * 255); 
    }

    public byte FormatPosition(float position)
    {
        // Shift the value to start from 0 (i.e., -128 becomes 0, 127 becomes 255)
        float wrappedValue = ((position + 128) % 256);

        // Ensure the value is positive after modulus
        if (wrappedValue < 0)
            wrappedValue += 256;

        // Shift it back to the range [-128, 127]
        position = wrappedValue - 128;
        return (byte)(int)(Mathf.Clamp(position, -128, 127));
    }
    // Update is called once per frame
    void Update()
    {
        // Calculate the rotation angle based on the rotation speed and time.
        float rotationAngle = rotationSpeed * Time.deltaTime;

        // Rotate the GameObject smoothly around the specified axis.
        sun.transform.Rotate(rotationAxis, rotationAngle);
        rotationAngle = rotationSpeed * 2 * Time.deltaTime; // make earth spin faster around itself than around sun
        earth.transform.Rotate(rotationAxis, rotationAngle);
        Debug.Log(stars[0].transform.position);

        if (connectedToToolbox)
        {
            for (int i = 0; i < stars.Count; i++)
            {
                //Vector3 rotation = stars[i].transform.eulerAngles;
                //rotationSignal[0] = FormatRotation(rotation.x);
                //rotationSignal[1] = FormatRotation(rotation.y);
                //rotationSignal[2] = FormatRotation(rotation.z);
                Vector3 position = stars[i].transform.position;
                positionSignal[0] = FormatPosition(position.x);
                positionSignal[1] = FormatPosition(position.y);
                positionSignal[2] = FormatPosition(position.z);
                this.connectionHandler.SendToPython(ConnectionHandler.CMD_SEND, names[i], positionSignal);
            }
            
        }
    }
}

