using UnityEngine;

[CreateAssetMenu(fileName = "TurbineDataContainer", menuName = "ScriptableObjects/TurbineDataContainer")]
public class TurbineDataContainer : ScriptableObject
{
    public TurbineData[] turbines;

    // Retrieve turbine data by ID
    public TurbineData GetTurbineDataByID(string turbineID)
    {
        foreach (var turbine in turbines)
        {
            if (turbine.turbineID == turbineID)
            {
                return turbine;
            }
        }
        Debug.LogError($"TurbineData for ID {turbineID} not found!");
        return null;
    }
}