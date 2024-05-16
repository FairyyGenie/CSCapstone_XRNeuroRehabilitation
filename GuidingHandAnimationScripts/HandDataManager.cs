using System;
using System.Data;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/*public static class CSVUtlity {
    public static void ToCSV(this DataTable dtDataTable, string strFilePath, string delimeter=",")
    {
       // if (!Directory.Exists("./HandMovement/"))
        //{
          //  Directory.CreateDirectory("./HandMovement/");
        //}
        StreamWriter sw = new StreamWriter(strFilePath, false);
        //headers    
        for (int i = 0; i < dtDataTable.Columns.Count; i++)
        {
            sw.Write(dtDataTable.Columns[i]);
            if (i < dtDataTable.Columns.Count - 1)
            {
                sw.Write(delimeter);
            }
        }
        sw.Write(sw.NewLine);
        foreach (DataRow dr in dtDataTable.Rows)
        {
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                if (!Convert.IsDBNull(dr[i]))
                {
                    string value = dr[i].ToString();
                    if (value.Contains(','))
                    {
                        value = String.Format("\"{0}\"", value);
                        sw.Write(value);
                    }
                    else
                    {
                        sw.Write(dr[i].ToString());
                    }
                }
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write(delimeter);
                }
            }
            sw.Write(sw.NewLine);
        }
        sw.Close();
    }
}

//try
public enum Delimeter
{
    Comma=0,
    Tab
};

public class HandDataManager : MonoBehaviour
{
    //StreamWriter handMovementWriter;
    [SerializeField]
    string handMovementFilePath = "";
    [SerializeField]
    public OVRSkeleton.SkeletonType skeletonType = OVRSkeleton.SkeletonType.HandRight;
    public Delimeter delimeterUsed;
    string boneTransforms;
#if UNITY_EDITOR
    string info;
    int idx = 0;
#endif

    private float updateLogFrequency = 0.05f; //20Hz update

    [SerializeField]
    private OVRSkeleton handSkeleton;

    [SerializeField]
    public TextMeshProUGUI debugText;

    [SerializeField]
    bool showDebug = false;

    [SerializeField]
    public Transform[] pegsToTrack;

    bool isLoggingData = true;

    float timeOffset = 0;

    DataTable userHandData;

    void Awake()
    {
        handMovementFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "HandMovement");
        //handMovementFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/HandMovement";
         if (!Directory.Exists(handMovementFilePath))
        {
            Directory.CreateDirectory(handMovementFilePath);
        }
        if (handSkeleton == null)
        {
            handSkeleton = GetComponent<OVRSkeleton>();
        }
        timeOffset = 0;

    }

    public void InitializeHandDataWriter()
    {
        //if (!Directory.Exists(handMovementFilePath))
        //{
        //    Directory.CreateDirectory(handMovementFilePath);
        //}
        //handMovementWriter = new StreamWriter(handMovementFilePath + GameController.currentPlayerID + "_Hand_" + skeletonType.ToString() + ".txt", false);

        //string header = "Timestamp, Session_ID, Skeleton_Type";
        //for (OVRSkeleton.BoneId bID = OVRSkeleton.BoneId.Hand_Start; bID < OVRSkeleton.BoneId.Hand_End; bID++)
        //{
        //    header += $", {OVRSkeleton.BoneLabelFromBoneId(skeletonType, bID)}_POS, {OVRSkeleton.BoneLabelFromBoneId(skeletonType, bID)}_ROT";
        //}
        //handMovementWriter.WriteLine(header); //Write header into the file

        if (userHandData == null)
        {
            userHandData = new DataTable("Participant_Record");
            userHandData.Columns.Add("Timestamp", typeof(string));
            userHandData.Columns.Add("Session_ID", typeof(string));
            userHandData.Columns.Add("Skeleton_Type", typeof(string));
            for (OVRSkeleton.BoneId bID = OVRSkeleton.BoneId.Hand_Start; bID < OVRSkeleton.BoneId.Hand_End; bID++)
            {
                userHandData.Columns.Add($"{OVRSkeleton.BoneLabelFromBoneId(skeletonType, bID)}_POS", typeof(string));
                userHandData.Columns.Add($"{OVRSkeleton.BoneLabelFromBoneId(skeletonType, bID)}_ROT", typeof(string));
            }
            for (int i = 0; i < pegsToTrack.Length; i++)
            {
                userHandData.Columns.Add($"Peg_{i}_POS", typeof(string));
                userHandData.Columns.Add($"Peg_{i}_ROT", typeof(string));
            }
        }
        else
        {
            userHandData.Clear();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isLoggingData)
        {
            timeOffset += Time.fixedDeltaTime;
            if (timeOffset > updateLogFrequency)
            {
                timeOffset = 0;
                UpdateBoneData();
                LogData();
            }
        }
    }

    void LogData()
    {
        //if (handMovementWriter.BaseStream != null)
        //{
            string trimmedData = String.Concat(boneTransforms.Where(c => !Char.IsWhiteSpace(c)));
        //handMovementWriter.WriteLine(trimmedData);
        string[] data = trimmedData.Split('$');
            userHandData.Rows.Add(data);

        Debug.Log("Here is row data: "+ userHandData.Rows[3][3]);
        //}
    }

    public void enableLogging(bool status)
    {
        isLoggingData = status;
    }

    public void saveLoggingData()
    {
        enableLogging(false);
        if (userHandData.Rows.Count > 0)
        {
            if(delimeterUsed == Delimeter.Comma) {
                userHandData.ToCSV(handMovementFilePath + skeletonType.ToString() + ".csv", ",");
            }
            else if (delimeterUsed == Delimeter.Tab)
            {
                userHandData.ToCSV(handMovementFilePath + skeletonType.ToString() + ".tsv", "\t");
            }
            
        }
        InitializeHandDataWriter();
    }

    private void OnDestroy()
    {
        if (userHandData != null)
        {
            userHandData.Dispose();
        }
        //if(handMovementWriter.BaseStream != null)
        //{
        //    handMovementWriter.Close();
        //}
    }

    private void OnApplicationQuit()
    {
        saveLoggingData();
    }

    void UpdateBoneData()
    {
        boneTransforms = $"{DateTime.Now.ToString("hh:mm:ss.fff")}$Session${handSkeleton.GetSkeletonType()}";
#if UNITY_EDITOR
        info = $"{DateTime.Now.ToString("hh:mm:ss.fff")},{handSkeleton.GetSkeletonType()},";
        idx = 0;
#endif
        foreach (var bone in handSkeleton.Bones)
        {
            if (handSkeleton.IsValidBone(bone.Id))
            {
                //boneTransforms += $"{OVRSkeleton.BoneLabelFromBoneId(handSkeleton.GetSkeletonType(), bone.Id)} P {bone.Transform.position} R {bone.Transform.rotation.eulerAngles}";
                Vector3 bonePos = bone.Transform.position;
                Vector3 boneRot = bone.Transform.rotation.eulerAngles;
                boneTransforms += "$" + bonePos.x + "," + bonePos.y + "," + bonePos.z + "$" + boneRot.x + "," +boneRot.y + "," + boneRot.z;
                
#if UNITY_EDITOR
                info += $"{idx} {OVRSkeleton.BoneLabelFromBoneId(handSkeleton.GetSkeletonType(), bone.Id)} P {bone.Transform.position} R {bone.Transform.rotation.eulerAngles}\n";
#endif
            }
#if UNITY_EDITOR
            idx++;
#endif
        }
#if UNITY_EDITOR
        if (showDebug)
        {
            debugText.text = info;
            Debug.Log(boneTransforms);
        }
#endif

        //Update Peg Positions and add to list;
        for (int i = 0; i < pegsToTrack.Length; i++) {
            boneTransforms += "$" + pegsToTrack[i].transform.position.x + "," + pegsToTrack[i].transform.position.y + "," + pegsToTrack[i].transform.position.z;
            boneTransforms += "$" + pegsToTrack[i].transform.eulerAngles.x + "," + pegsToTrack[i].transform.eulerAngles.y + "," + pegsToTrack[i].transform.eulerAngles.z;
        }

    }
}
*/

//THE CODE BELOW SAVES THE POSITIONS AND ROTATIONS TO THE CSV FILE WITH TIMESTAMP AND JOINTS NAME AS HEADERS

using System;
using System.Data;
using System.IO;
using UnityEngine;

public class HandDataManager : MonoBehaviour
{
    [SerializeField] private OVRSkeleton handSkeleton;
    public OVRSkeleton.SkeletonType skeletonType = OVRSkeleton.SkeletonType.HandRight;
    public enum Delimeter { Comma, Tab };
    public Delimeter delimiterUsed = Delimeter.Comma;

    private DataTable userHandData;
    private bool isLoggingData = false;
    private float updateLogFrequency = 0.05f; // 20Hz update
    private float timeOffset = 0;
    private string handMovementFilePath;

    void Awake()
    {
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        handMovementFilePath = Path.Combine(desktopPath, "HandMovement");
        if (!Directory.Exists(handMovementFilePath))
        {
            Directory.CreateDirectory(handMovementFilePath);
        }
        InitializeHandDataWriter();
    }

    public void InitializeHandDataWriter()
    {
        userHandData = new DataTable("Participant_Record");
        userHandData.Columns.Add("Timestamp", typeof(string));

        // Adjusted to match the BoneLabelFromBoneId pattern
        for (OVRSkeleton.BoneId bID = OVRSkeleton.BoneId.Hand_Start; bID < OVRSkeleton.BoneId.Hand_End; bID++)
        {
            string boneName = OVRSkeleton.BoneLabelFromBoneId(skeletonType, bID);
            userHandData.Columns.Add($"{boneName}_POS", typeof(string));
            userHandData.Columns.Add($"{boneName}_ROT", typeof(string));
        }
    }

    void Update()
    {
        if (isLoggingData)
        {
            timeOffset += Time.deltaTime;
            if (timeOffset >= updateLogFrequency)
            {
                timeOffset = 0;
                CaptureAndLogHandData();
            }
        }
    }

    private void CaptureAndLogHandData()
    {
        DataRow newRow = userHandData.NewRow();
        newRow["Timestamp"] = DateTime.Now.ToString("o");

        foreach (var bone in handSkeleton.Bones)
        {
            if (handSkeleton.IsValidBone(bone.Id) && (bone.Id >= OVRSkeleton.BoneId.Hand_Start && bone.Id < OVRSkeleton.BoneId.Hand_End))
            {
                string boneName = OVRSkeleton.BoneLabelFromBoneId(skeletonType, bone.Id);
                Vector3 pos = bone.Transform.position;
                Quaternion rot = bone.Transform.rotation;
                newRow[$"{boneName}_POS"] = $"{pos.x},{pos.y},{pos.z}";
                newRow[$"{boneName}_ROT"] = $"{rot.eulerAngles.x},{rot.eulerAngles.y},{rot.eulerAngles.z}";
            }
        }

        userHandData.Rows.Add(newRow);
    }

    public void StartRecording()
    {
        Debug.Log("Starting recording...");
        isLoggingData = true;
    }

    public void StopRecording()
    {
        Debug.Log("Stopping recording...");
        isLoggingData = false;
        SaveLoggingData();
    }

    private void SaveLoggingData()
    {
        if (userHandData.Rows.Count > 0)
        {
            string fileName = $"HandData_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            string fullPath = Path.Combine(handMovementFilePath, fileName);
            ToCSV(userHandData, fullPath, delimiterUsed == Delimeter.Comma ? "," : "\t");
            Debug.Log($"Data saved to: {fullPath}");
        }
        else
        {
            Debug.Log("No data to save.");
        }
    }

    public static void ToCSV(DataTable dtDataTable, string strFilePath, string delimiter = ",")
    {
        using (StreamWriter sw = new StreamWriter(strFilePath, false))
        {
            foreach (DataColumn col in dtDataTable.Columns)
            {
                sw.Write(col.ColumnName + delimiter);
            }
            sw.WriteLine();

            foreach (DataRow row in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    string value = row[i].ToString();
                    if (value.Contains(delimiter)) value = $"\"{value}\"";
                    sw.Write(value + (i < dtDataTable.Columns.Count - 1 ? delimiter : ""));
                }
                sw.WriteLine();
            }
        }
    }
}
