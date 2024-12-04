using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ProcessData : MonoBehaviour
{
    [SerializeField] private TextAsset csvFile; // Drag and drop the CSV file
    [SerializeField] private TurbineDataContainer turbineDataContainer; // Target ScriptableObject

    public void GenerateTurbineData()
    {
#if UNITY_EDITOR
        if (csvFile == null)
        {
            Debug.LogError("No CSV file assigned!");
            return;
        }

        if (turbineDataContainer == null)
        {
            Debug.LogError("No TurbineDataContainer assigned!");
            return;
        }

        Dictionary<string, List<string[]>> turbineDataGroups = new Dictionary<string, List<string[]>>();
        string[] lines = csvFile.text.Split('\n');

        // Process lines (skip header)
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] data = line.Split(',');
            if (data.Length < 8) continue; // Ensure all columns exist

            string turbineID = data[0].Trim();
            if (!turbineDataGroups.ContainsKey(turbineID))
            {
                turbineDataGroups[turbineID] = new List<string[]>();
            }
            turbineDataGroups[turbineID].Add(data);
        }

        List<TurbineData> turbinesList = new List<TurbineData>();

        // Create TurbineData objects
        foreach (var entry in turbineDataGroups)
        {
            string turbineID = entry.Key;
            List<string[]> rows = entry.Value;

            int rowCount = rows.Count;
            string[] timeIntervals = new string[rowCount];
            int[] eventCodes = new int[rowCount];
            string[] eventCodeDescriptions = new string[rowCount];
            float[] windSpeeds = new float[rowCount];
            float[] ambientTemperatures = new float[rowCount];
            float[] rotorSpeeds = new float[rowCount];
            float[] powers = new float[rowCount];

            for (int i = 0; i < rowCount; i++)
            {
                string[] data = rows[i];
                timeIntervals[i] = data[1].Trim();
                eventCodes[i] = int.Parse(data[2].Trim());
                eventCodeDescriptions[i] = data[3].Trim();
                windSpeeds[i] = float.Parse(data[4].Trim());
                ambientTemperatures[i] = float.Parse(data[5].Trim());
                rotorSpeeds[i] = float.Parse(data[6].Trim());
                powers[i] = float.Parse(data[7].Trim());
            }

            TurbineData turbineData = new TurbineData
            {
                turbineID = turbineID,
                timeIntervals = timeIntervals,
                eventCodes = eventCodes,
                eventCodeDescriptions = eventCodeDescriptions,
                windSpeeds = windSpeeds,
                ambientTemperatures = ambientTemperatures,
                rotorSpeeds = rotorSpeeds,
                powers = powers
            };

            turbinesList.Add(turbineData);
        }

        turbineDataContainer.turbines = turbinesList.ToArray();

        EditorUtility.SetDirty(turbineDataContainer);
        AssetDatabase.SaveAssets();

        Debug.Log("TurbineDataContainer updated successfully!");
#endif
    }
}