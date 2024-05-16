/*using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;
using System.Globalization;
using System.Linq;
using System.Text;

public class UnifiedPlayback : MonoBehaviour
{
    public GameObject objectTarget; // Assign the object target in the Inspector
    public OVRHand handSkeleton; // Assign the custom hand in the Inspector

    private string objectFilePath = "C:/Users/AIMLAB/Desktop/HandMovement/ObjectData_20240428_150922.csv";
    private string handFilePath = "C:/Users/AIMLAB/Desktop/HandMovement/HandData_20240428_150950.csv";

    private DataTable objectData;
    private DataTable handData;

    private int objectCurrentRow = 0;
    private int handCurrentRow = 0;

    private bool isPlaying = false;

    void Start()
    {
        objectData = LoadCSVData(objectFilePath);
        handData = LoadCSVData(handFilePath);
        Debug.Log($"Object data loaded: {objectData != null && objectData.Rows.Count > 0}");
        Debug.Log($"Hand data loaded: {handData != null && handData.Rows.Count > 0}");
    }

    void Update()
    {
        if (isPlaying)
        {
            UpdatePlayback();
        }
    }

    DataTable LoadCSVData(string filePath)
    {
        DataTable dt = new DataTable();
        using (StreamReader sr = new StreamReader(filePath))
        {
            var headers = sr.ReadLine().Split(',');
            foreach (var header in headers)
            {
                dt.Columns.Add(header);
            }

            while (!sr.EndOfStream)
            {
                var rows = sr.ReadLine().Split(',');
                DataRow dr = dt.NewRow();
                for (int i = 0; i < headers.Length; i++)
                {
                    dr[i] = rows[i];
                }
                dt.Rows.Add(dr);
            }
        }
        return dt;
    }

    void UpdatePlayback()
    {
        if (objectCurrentRow < objectData.Rows.Count && handCurrentRow < handData.Rows.Count)
        {
            Debug.Log($"Updating row {objectCurrentRow} for object and {handCurrentRow} for hand");
            DataRow objectRow = objectData.Rows[objectCurrentRow];
            DataRow handRow = handData.Rows[handCurrentRow];

            ApplyObjectData(objectRow);
            ApplyHandData(handRow);

            objectCurrentRow++;
            handCurrentRow++;
        }
        else
        {
            isPlaying = false; // Stop playback if end of any data is reached
            Debug.Log("Playback ended.");
        }
    }

    void ApplyObjectData(DataRow row)
    {
        try
        {
            Vector3 position = new Vector3(
                float.Parse(row["PositionX"].ToString().Trim(), CultureInfo.InvariantCulture),
                float.Parse(row["PositionY"].ToString().Trim(), CultureInfo.InvariantCulture),
                float.Parse(row["PositionZ"].ToString().Trim(), CultureInfo.InvariantCulture));

            // Assuming rotation data is given as Euler angles and needs conversion to Quaternion
            Quaternion rotation = Quaternion.Euler(
                float.Parse(row["RotationX"].ToString().Trim(), CultureInfo.InvariantCulture),
                float.Parse(row["RotationY"].ToString().Trim(), CultureInfo.InvariantCulture),
                float.Parse(row["RotationZ"].ToString().Trim(), CultureInfo.InvariantCulture));

            objectTarget.transform.position = position;
            objectTarget.transform.rotation = rotation;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error applying object data: {ex.Message}");
        }
    }

    void ApplyHandData(DataRow row)
    {
        OVRCustomSkeleton skeleton = handSkeleton.GetComponent<OVRCustomSkeleton>();
        if (skeleton == null)
        {
            Debug.LogError("Skeleton reference is null.");
            return;
        }

        foreach (var bone in skeleton.Bones)
        {
            string boneName = OVRSkeleton.BoneLabelFromBoneId(skeleton.GetSkeletonType(), bone.Id);
            if (bone != null && row.Table.Columns.Contains($"{boneName}_POS") && row.Table.Columns.Contains($"{boneName}_ROT"))
            {
                Vector3 position = ParseVector3(row[$"{boneName}_POS"].ToString());
                Quaternion rotation = ParseEulerToQuaternion(row[$"{boneName}_ROT"].ToString());

                bone.Transform.position = position;
                bone.Transform.rotation = rotation;
            }
            else
            {
                Debug.LogError($"Bone or data for {boneName} is missing.");
            }
        }
    }

    *//*    Vector3 ParseVector3(string data)
        {
            string[] vals = data.Split(',');
            if (vals.Length != 3)
            {
                Debug.LogError($"Invalid Vector3 data format: {data}");
                return Vector3.zero;
            }
            try
            {
                //string[] vals = data.Split(',');
                return new Vector3(
                    float.Parse(vals[0], CultureInfo.InvariantCulture),
                    float.Parse(vals[1], CultureInfo.InvariantCulture),
                    float.Parse(vals[2], CultureInfo.InvariantCulture));
            }
            catch (FormatException ex)
            {
                Debug.LogError($"Failed to parse Vector3 from '{data}': {ex}");
                return Vector3.zero;
            }
        }*//*
    Vector3 ParseVector3(string data)
    {
        // Clean the data string to remove quotes and extra whitespaces
        data = data.Replace("\"", "").Trim();
        string[] vals = data.Split(',');
        if (vals.Length != 3)
        {
            Debug.LogError($"Invalid Vector3 data format: {data}, expected 3 elements but found {vals.Length}");
            return Vector3.zero;
        }

        try
        {
            return new Vector3(
                float.Parse(vals[0], CultureInfo.InvariantCulture),
                float.Parse(vals[1], CultureInfo.InvariantCulture),
                float.Parse(vals[2], CultureInfo.InvariantCulture));
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to parse Vector3 from '{data}': {ex}");
            return Vector3.zero;
        }
    }

    Quaternion ParseEulerToQuaternion(string data)
    {
        data = data.Replace("\"", "").Trim();
        string[] vals = data.Split(',');
        if (vals.Length != 3)
        {
            Debug.LogError($"Invalid Quaternion data format: {data}, expected 3 elements but found {vals.Length}");
            return Quaternion.identity;
        }

        try
        {
            float x = float.Parse(vals[0], CultureInfo.InvariantCulture);
            float y = float.Parse(vals[1], CultureInfo.InvariantCulture);
            float z = float.Parse(vals[2], CultureInfo.InvariantCulture);
            return Quaternion.Euler(x, y, z);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to parse Quaternion from '{data}': {ex}");
            return Quaternion.identity;
        }
    }


    public void TogglePlayback()
    {
        isPlaying = !isPlaying;
        Debug.Log($"Playback toggled: {isPlaying}");
        if (isPlaying)
        {
            // Reset the playback indices when starting
            objectCurrentRow = 0;
            handCurrentRow = 0;
        }

    }
}
*/

// the bollean does not turns into true 
//if i disable  the playback in the sesnsing part, then it will not show at all which is weird. It tracks the right object but still triggers the playback as per log


using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Oculus.Interaction;
using OculusSampleFramework;
using Oculus.Interaction.HandGrab;

public class UnifiedPlayback : MonoBehaviour
{
    public GameObject objectTarget; // Assign the object target in the Inspector
    public GameObject objectTarget2; // Assign the object target in the Inspector
    public GameObject objectTarget3; // Assign the object target in the Inspector

    public OVRHand handSkeleton; // Assign the custom hand in the Inspector
    public GameObject handsL;
    public GameObject handsR;
    [SerializeField] private OVRHand trackedHand;
    public GameObject Interactobject;
    public GameObject Tracked;
    public GameObject LockAnimation;
    public GameObject LockPlayer;
    public GameObject playerbox;
    public GameObject playerpen;
    public GameObject Touch;

    private string KeytolockobjectFilePath = "C:/Users/AIMLAB/Desktop/HandMovement/ObjectData_LibraryTask1.csv";
    private string Task1handFilePath = "C:/Users/AIMLAB/Desktop/HandMovement/HandData_LibraryTask1.csv";

    private string OpenBoxobjectFilePath = "C:/Users/AIMLAB/Desktop/HandMovement/Lit.csv";
    private string Task2handFilePath = "C:/Users/AIMLAB/Desktop/HandMovement/LitHand.csv";

    private string task3objectFilePath = "C:/Users/AIMLAB/Desktop/HandMovement/pen.csv";
    private string Task3handFilePath = "C:/Users/AIMLAB/Desktop/HandMovement/HandData_pen.csv";

    private DataTable objectData;
    private DataTable handData;

    private int objectCurrentRow = 0;
    private int handCurrentRow = 0;
    private int countreplay = 0;
    private int count2replay = 0;
    private int count3replay = 0;

    private bool isPlaying;
    private bool task1;
    private bool task2;

    private float inactivityTimer = 0f;
    private float maxInactivityTime = 15f; // Maximum time for hand inactivity in seconds

    void Start()
    {
        isPlaying = true;

         objectData = LoadCSVData(KeytolockobjectFilePath);
         handData = LoadCSVData(Task1handFilePath);
        ultraleap.Instance.startRecord();
         

        //Debug.Log($"Object data loaded: {objectData != null && objectData.Rows.Count > 0}");
        //Debug.Log($"Hand data loaded: {handData != null && handData.Rows.Count > 0}");
    }

    void Update()
    {

        CollisionHandler collisionhandler = Tracked.GetComponent<CollisionHandler>();
        Cube cube = Touch.GetComponent<Cube>();

        task1 = collisionhandler.task1done;
        task2 = cube.touched;
        if (task1 == true)
        {
            objectTarget = objectTarget2;
            objectData = LoadCSVData(OpenBoxobjectFilePath);
            handData = LoadCSVData(Task2handFilePath);
            Interactobject = playerbox;
            if (count2replay == 0)
            {
                isPlaying = true;
            }
        }
        if (task2 == true)
        {
            objectTarget = objectTarget3;
            objectData = LoadCSVData(task3objectFilePath);
            handData = LoadCSVData(Task3handFilePath);
            Interactobject = playerpen;
            if (count3replay == 0)
            {
                isPlaying = true;
            }
        }

        if (isPlaying) //playing the animation
        {
            if (!task1)
            {
                LockAnimation.SetActive(true);
                LockPlayer.SetActive(false);
                Interactobject.SetActive(false);
                countreplay += 1;

            }
            else if (task1 && !task2) //second task
            {
                playerbox.SetActive(false);
                Interactobject.SetActive(false);

            
                count2replay += 1;
            }
            else if (task1 && task2)
            {
                playerpen.SetActive(false);
                Interactobject.SetActive(false);
                objectTarget3.SetActive(true);
                count3replay += 1;
            }

            objectTarget.SetActive(true);

            handsR.SetActive(true);
            handsL.SetActive(false);
            UpdatePlayback();
            
        }
        else //not playing the animation
        {

            if (!task1) //task 1 
            {
                LockAnimation.SetActive(false);
                LockPlayer.SetActive(true);
                Interactobject.SetActive(true);
                //Debug.Log("BOOlean for task 2 " + task2);

            }
            else if (task1 && !task2) //second exercise
            {
                playerbox.SetActive(true);
                //Interactobject.SetActive(true);
                Debug.Log("user should be able to open the lit ");
                //Debug.Log("BOOlean for task 2 " + task2);
            }
            else if (task1 && task2)
            {
                playerpen.SetActive(true);
                objectTarget3.SetActive(false);
                //Interactobject.SetActive(false);
                //Interactobject.SetActive(true);
            }

            objectTarget.SetActive(false);

            handsR.SetActive(false);
            handsL.SetActive(false);
        }

        var grabbed = Interactobject.GetComponent<OVRGrabbable>(); //the object the user is supposed to interact with
        //Debug.Log("Object Inactivity check " + grabbed);
        //Debug.Log("I am thinking that you did not touch the " + grabbed + "here is the boolean " + grabbed.isGrabbed);
        // Check for hand movement
        if (!isPlaying && !grabbed.isGrabbed)//&& trackedHand.GetFingerIsPinching(OVRHand.HandFinger.Index))
        {
            //Debug.Log("I am thinking that you did not touch the " + grabbed + "here is the boolean" + grabbed.isGrabbed);
            // Increment inactivity timer if hand is not moving
            inactivityTimer += Time.deltaTime;
            if (inactivityTimer >= maxInactivityTime)
            {
                UpdatePlayback(); // Start replay if hand is inactive for too long
                inactivityTimer = 0f;
            }
            // Reset inactivity timer if hand is moving
        }
        else //the user picked up the object
        {
            inactivityTimer = 0f;

        }
    }
    

    DataTable LoadCSVData(string filePath)
    {
        DataTable dt = new DataTable();
        using (StreamReader sr = new StreamReader(filePath))
        {
            string headerLine = sr.ReadLine();
            var headers = SplitCsvLine(headerLine, ',');
            foreach (string header in headers)
            {
                dt.Columns.Add(header);
            }

            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                var fields = SplitCsvLine(line, ',');
                if (fields.Length == headers.Length)
                {
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = fields[i];
                    }
                    dt.Rows.Add(dr);
                }
                else
                {
                    Debug.LogError("Row length does not match header length");
                }
            }
        }
        return dt;
    }
    string[] SplitCsvLine(string line, char delimiter)
    {
        List<string> fields = new List<string>();
        StringBuilder fieldBuilder = new StringBuilder();
        bool inQuotes = false;
        foreach (char c in line)
        {
            if (c == delimiter && !inQuotes)
            {
                fields.Add(fieldBuilder.ToString());
                fieldBuilder = new StringBuilder();
            }
            else if (c == '\"')
            {
                inQuotes = !inQuotes;
            }
            else
            {
                fieldBuilder.Append(c);
            }
        }
        fields.Add(fieldBuilder.ToString()); // Add the last field
        return fields.Select(field => field.Trim()).ToArray(); // Trim fields to remove unnecessary spaces
    }

    void UpdatePlayback()
    {
        objectTarget.SetActive(true);
        handsR.SetActive(true);

        if (!task1) //task1
        {
            LockAnimation.SetActive(true);
            LockPlayer.SetActive(false);
        }
        else if (task1 && !task2)
        {
            playerbox.SetActive(false);
            Debug.Log("disabled user box " );
            Debug.Log("BOOlean for task 2 " + task2);
        }
        else if (task1 && task2)
        {
            playerpen.SetActive(false);
            objectTarget3.SetActive(true);
        }


        if (objectCurrentRow < objectData.Rows.Count && handCurrentRow < handData.Rows.Count)
        {
            isPlaying = true;
            //handSkeleton.enabled= true;
            
            //Debug.Log($"PLAYING BACK ! Updating row {objectCurrentRow} for object and {handCurrentRow} for hand");
            DataRow objectRow = objectData.Rows[objectCurrentRow];
            DataRow handRow = handData.Rows[handCurrentRow];

            ApplyObjectData(objectRow);
            ApplyHandData(handRow);

            objectCurrentRow++;
            handCurrentRow++;
        }
        else //the animation ended, no more lines in the csv
        {
            isPlaying = false;
            objectTarget.SetActive(false);

            if (!task1)
            {
                LockAnimation.SetActive(false);
                LockPlayer.SetActive(true);
            }
            else if (task1 && !task2) //if the switch happened to the second task
            {
                objectTarget2.SetActive(false);
                playerbox.SetActive(true);

                Debug.Log("The player box must be on");
            }
            else if (task1 && task2)
            {
                objectTarget3.SetActive(false);
                playerpen.SetActive(true);
            }

            handsR.SetActive(false);
            //handSkeleton.enabled = false;

            // Stop playback if end of any data is reached
            Debug.Log("Playback ended.");
            objectCurrentRow = 0;
            handCurrentRow = 0;
        }
    }

    void ApplyObjectData(DataRow row)
    {
        try
        {
            Vector3 position = ParseVector3($"{row["PositionX"]},{row["PositionY"]},{row["PositionZ"]}");
            Quaternion rotation = ParseEulerToQuaternion($"{row["RotationX"]},{row["RotationY"]},{row["RotationZ"]}");
            objectTarget.transform.position = position;
            objectTarget.transform.rotation = rotation;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in ApplyObjectData: {ex.Message}");
        }
    }

/*    void ApplyHandData(DataRow row)
    {
        OVRCustomSkeleton skeleton = handSkeleton.GetComponent<OVRCustomSkeleton>();
        if (skeleton == null)
        {
            Debug.LogError("Skeleton reference is null.");
            return;
        }

        foreach (var bone in skeleton.Bones)
        {
            string boneName = OVRSkeleton.BoneLabelFromBoneId(skeleton.GetSkeletonType(), bone.Id);
            if (bone != null && row.Table.Columns.Contains($"{boneName}_POS") && row.Table.Columns.Contains($"{boneName}_ROT"))
            {
                Vector3 position = ParseVector3(row[$"{boneName}_POS"].ToString());
                Quaternion rotation = ParseEulerToQuaternion(row[$"{boneName}_ROT"].ToString());

                bone.Transform.position = position;
                bone.Transform.rotation = rotation;
            }
            else
            {
                Debug.LogError($"Bone or data for {boneName} is missing.");
            }
        }
    }*/
    private void ApplyHandData(DataRow dataRow)
    {
        OVRCustomSkeleton skeleton = handSkeleton.GetComponent<OVRCustomSkeleton>();
        if (skeleton == null)
        {
            Debug.LogError("Skeleton reference is null.");
            return;
        }

        foreach (var bone in skeleton.Bones)
        {
            if (bone == null)
            {
                Debug.LogError("Bone reference is null.");
                continue;
            }
            //Debug.Log($"Bone: {bone.name}");

            string boneName = OVRSkeleton.BoneLabelFromBoneId(skeleton.GetSkeletonType(), bone.Id);

            if (dataRow.Table.Columns.Contains($"{boneName}_POS") && dataRow.Table.Columns.Contains($"{boneName}_ROT"))
            {
                string posString = dataRow[$"{boneName}_POS"] as string;
                Vector3 position = ParseVector3(posString);

                string rotString = dataRow[$"{boneName}_ROT"] as string;
                Quaternion rotation = ParseEulerToQuaternion(rotString);

                //Debug.Log($"Bone: {boneName}, POS string: {posString}, ROT string: {rotString}");

                if (bone.Transform != null)
                {
                    bone.Transform.position = position;
                    bone.Transform.rotation = rotation;
                }
                else
                {
                   // Debug.LogError($"Transform for bone '{boneName}' is null.");
                }
            }
        }
    }
    /*    Vector3 ParseVector3(string data)
        {
            string[] vals = data.Split(',');
            if (vals.Length != 3)
            {
                Debug.LogError($"Invalid Vector3 data format: {data}");
                return Vector3.zero;
            }
            try
            {
                //string[] vals = data.Split(',');
                return new Vector3(
                    float.Parse(vals[0], CultureInfo.InvariantCulture),
                    float.Parse(vals[1], CultureInfo.InvariantCulture),
                    float.Parse(vals[2], CultureInfo.InvariantCulture));
            }
            catch (FormatException ex)
            {
                Debug.LogError($"Failed to parse Vector3 from '{data}': {ex}");
                return Vector3.zero;
            }
        }*/
    Vector3 ParseVector3(string data)
    {
        string[] vals = data.Split(',').Select(v => v.Trim()).ToArray();
        if (vals.Length != 3)
        {
            Debug.LogError("Invalid Vector3 format");
            return Vector3.zero;
        }
        return new Vector3(
            float.Parse(vals[0], CultureInfo.InvariantCulture),
            float.Parse(vals[1], CultureInfo.InvariantCulture),
            float.Parse(vals[2], CultureInfo.InvariantCulture));
    }

    Quaternion ParseEulerToQuaternion(string data)
    {
        string[] vals = data.Split(',').Select(v => v.Trim()).ToArray();
        if (vals.Length != 3)
        {
            Debug.LogError("Invalid Quaternion format");
            return Quaternion.identity;
        }
        return Quaternion.Euler(
            float.Parse(vals[0], CultureInfo.InvariantCulture),
            float.Parse(vals[1], CultureInfo.InvariantCulture),
            float.Parse(vals[2], CultureInfo.InvariantCulture));
    }



    public void TogglePlayback()
    {
        isPlaying = !isPlaying;
        Debug.Log($"Playback toggled: {isPlaying}");
        if (isPlaying)
        {
            // Reset the playback indices when starting
            objectCurrentRow = 0;
            handCurrentRow = 0;
        }

    }
}
