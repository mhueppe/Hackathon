using System.Collections.Generic;
using UnityEngine;
using PackageUtils;
using SnapUnity;
public class LampExperiment : Experiment
{

    public Blink lightBulb;
    private byte[] lampSignal = new byte[1];

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        // do everything else this experiment needs to setup
        if (connectedToToolbox)
        {
            SetupPackages();
        }

    }
 

    private void SetupPackages()
    {
        Dictionary<string, object> encoding = new Dictionary<string, object>();
        DataType dataTypeParser = new DataType();

        encoding["on"] = new Dictionary<string, object>
                {
                    { "type", dataTypeParser.GetName(typeof(byte))}, // send the lamp state in one byte
                    { "unit", "state" }, // whether lamp is on or off
                    { "scaler", 1 }, // no scaling needed since bool fits in byte without data loss
                    { "endian", "little" }
                };
        Dictionary<string, object> label_names = new Dictionary<string, object>();
        label_names["on"] = new List<int> { 0, 1 }; // 0 = lamp off, 1 = lamp on

        Dictionary<string, object> additionalInfo = new Dictionary<string, object>
            {
            { "label_names", label_names}, // specify the possible label names for training (here only on)
            { "default", "on"}, // specify the default training label when this game object is present
            };

        // Create a package header under the name lamp with the given encoding and additional Info for training
        // Length of package is one since we only send one bool/byte
        PackageHeader packageHeader = new PackageHeader("state", encoding, 1, additionalInfo); 
        Package package = new Package(packageHeader);
        this.connectionHandler.RegisterPackageSending("Lamp", packageHeader);
    }

    protected override void OnPackageUpdate(int id, List<float> sample)
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (connectedToToolbox)
        {
            lampSignal[0] = (byte)(lightBulb.lightIsOn ? 1 : 0); // convert the bool into 0 or 1 and then into a byte
            this.connectionHandler.SendToPython(ConnectionHandler.CMD_SEND, "Lamp", lampSignal);
        }
    }
}

