/*using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class TransformRecorder : MonoBehaviour
{
    public GameObject target;
    private List<string> records = new List<string>();
    private bool isRecording = false;

    private void Start()
    {
        // Prepare the header for CSV
        records.Add("Time,PositionX,PositionY,PositionZ,RotationX,RotationY,RotationZ,RotationW");
    }

    void Update()
    {
        if (isRecording)
        {
            RecordTransform();
        }
    }

    public void ToggleRecording()
    {
        isRecording = !isRecording;
        if (!isRecording)
        {
            SaveRecordsToFile();
        }
    }

    private void RecordTransform()
    {
        Vector3 position = target.transform.position;
        Quaternion rotation = target.transform.rotation;
        string record = Time.time.ToString("F3") + "," +
                        position.x.ToString("F3") + "," +
                        position.y.ToString("F3") + "," +
                        position.z.ToString("F3") + "," +
                        rotation.x.ToString("F3") + "," +
                        rotation.y.ToString("F3") + "," +
                        rotation.z.ToString("F3") + "," +
                        rotation.w.ToString("F3");
        records.Add(record);
    }

    private void SaveRecordsToFile()
    {
        string folderPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "HandMovement");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string filePath = Path.Combine(folderPath, "ObjectData.csv");
        File.WriteAllLines(filePath, records.ToArray());

        Debug.Log("Data saved to " + filePath);
    }
}
*/
//below is working but more frequently than hand
/*
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class TransformRecorder : MonoBehaviour
{
    public GameObject target; // The target GameObject to record
    private bool isRecording = false;
    private List<string> dataLines = new List<string>();
    private string filePath;

    void Start()
    {
        // Setup the CSV file
        string folderPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "HandMovement");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        filePath = Path.Combine(folderPath, "ObjectData.csv");
        PrepareFile();
    }

    void Update()
    {
        if (isRecording)
        {
            RecordData();
        }
    }

    public void ToggleRecording()
    {
        isRecording = !isRecording;
        if (!isRecording)
        {
            // Write to file when recording stops
            WriteToFile();
        }
    }

    private void PrepareFile()
    {
        string fileName = "ObjectData_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
        string folderPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "HandMovement");
        filePath = Path.Combine(folderPath, fileName);
        dataLines.Add("Time,PositionX,PositionY,PositionZ,RotationX,RotationY,RotationZ,RotationW");
    }

    private void RecordData()
    {
        if (!target) return;

        Vector3 position = target.transform.position;
        Quaternion rotation = target.transform.rotation;
        string line = $"{Time.time:F3},{position.x:F3},{position.y:F3},{position.z:F3},{rotation.x:F3},{rotation.y:F3},{rotation.z:F3},{rotation.w:F3}";
        dataLines.Add(line);
    }

    private void WriteToFile()
    {
        File.WriteAllLines(filePath, dataLines);
        Debug.Log($"Data saved to {filePath}");
    }
}
*/
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class TransformRecorder : MonoBehaviour
{
    public GameObject target; // The target GameObject to record
    private bool isRecording = false;
    private List<string> dataLines = new List<string>();
    private string filePath;
    private float nextUpdateTime = 0f;
    private float updateInterval = 0.05f; // Update every 0.05 seconds to match 20Hz

    void Start()
    {
        PrepareFile();
    }

    void Update()
    {
        if (isRecording && Time.time >= nextUpdateTime)
        {
            RecordData();
            nextUpdateTime = Time.time + updateInterval;
        }
    }

    public void ToggleRecording()
    {
        isRecording = !isRecording;
        if (isRecording)
        {
            nextUpdateTime = Time.time + updateInterval; // Reset the next update time
        }
        else
        {
            WriteToFile();
        }
    }

    private void PrepareFile()
    {
        // Path to the desktop HandMovement folder
        string folderPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "HandMovement");
        Directory.CreateDirectory(folderPath); // Ensure the directory exists

        string fileName = "ObjectData_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
        filePath = Path.Combine(folderPath, fileName);
        dataLines.Add("PositionX,PositionY,PositionZ,RotationX,RotationY,RotationZ"); // Header format
    }

    private void RecordData()
    {
        if (!target) return;

        Vector3 position = target.transform.position;
        Vector3 rotation = target.transform.rotation.eulerAngles; // Convert Quaternion to Euler angles
        string line = $"{position.x:F3},{position.y:F3},{position.z:F3},{rotation.x:F3},{rotation.y:F3},{rotation.z:F3}";
        dataLines.Add(line);
    }

    private void WriteToFile()
    {
        File.WriteAllLines(filePath, dataLines);
        Debug.Log($"Data saved to {filePath}");
    }
}
