using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbineDataLogger : MonoBehaviour
{
    public TurbineDataContainer turbineDataContainer; // Assignez votre conteneur de donn�es

    void Start()
    {
        Debug.Log("TurbineDataLogger Start() appel�.");

        // V�rifiez que le conteneur de donn�es est assign�
        if (turbineDataContainer == null)
        {
            Debug.LogError("TurbineDataContainer non assign�.");
            return;
        }

        // Afficher les donn�es de chaque �olienne dans la console
        foreach (var turbine in turbineDataContainer.turbines)
        {
            Debug.Log($"Turbine ID: {turbine.turbineID}");
            for (int i = 0; i < turbine.timeIntervals.Length; i++)
            {
                Debug.Log($"Time Interval: {turbine.timeIntervals[i]}");
                Debug.Log($"Event Code: {turbine.eventCodes[i]}");
                Debug.Log($"Event Description: {turbine.eventCodeDescriptions[i]}");
                Debug.Log($"Wind Speed: {turbine.windSpeeds[i]} m/s");
                Debug.Log($"Ambient Temperature: {turbine.ambientTemperatures[i]} �C");
                Debug.Log($"Rotor Speed: {turbine.rotorSpeeds[i]} RPM");
                Debug.Log($"Power: {turbine.powers[i]} kW");
            }
        }
    }
}