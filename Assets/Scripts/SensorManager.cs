
using UnityEngine;

// JSON keys must match ESP8266 output EXACTLY - case sensitive
[System.Serializable]
public class SensorPacket
{
    public int rawMQ2 = 0;
    public int gasPPM = 0;
    public int gasLevel = 0;
    public float temp = 0f;
    public float hum = 0f;
}

public class SensorManager : MonoBehaviour
{
    public static SensorManager Instance;

    [Header("Live Values")]
    public int rawMQ2 = 0;
    public int gasPPM = 0;
    public int gasLevel = 0;
    public float temp = 25f;
    public float humidity = 60f;

    public bool isDanger => gasLevel == 3;
    public bool isMedium => gasLevel == 2;
    public bool isLow => gasLevel == 1;
    public bool isSafe => gasLevel == 0;
    public bool isTempAlert => temp > 40f;

    void Awake() { Instance = this; }

    public void Apply(SensorPacket p)
    {
        rawMQ2 = p.rawMQ2;
        gasPPM = p.gasPPM;
        temp = p.temp;
        humidity = p.hum;

        // Derive gasLevel from gasPPM directly — never trust the ESP calculation
        if (gasPPM >= 3000) gasLevel = 3;  // HIGH
        else if (gasPPM >= 1500) gasLevel = 2;  // MEDIUM  
        else if (gasPPM >= 500) gasLevel = 1;  // LOW
        else gasLevel = 0;  // CLEAN

        Debug.Log("[SM] gas=" + gasPPM + " level=" + gasLevel + " temp=" + temp);
    }
}