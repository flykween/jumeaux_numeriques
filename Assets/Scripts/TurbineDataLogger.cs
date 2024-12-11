using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbineDataLogger : MonoBehaviour
{
    public TurbineDataContainer turbineDataContainer; // Assignez votre conteneur de données

    void Start()
    {
        Debug.Log("TurbineDataLogger Start() appelé.");

        // Vérifiez que le conteneur de données est assigné
        if (turbineDataContainer == null)
        {
            Debug.LogError("TurbineDataContainer non assigné.");
            return;
        }

        // Afficher les données de chaque éolienne dans la console
        foreach (var turbine in turbineDataContainer.turbines)
        {
            Debug.Log($"Turbine ID: {turbine.turbineID}");
            for (int i = 0; i < turbine.timeIntervals.Length; i++)
            {
                Debug.Log($"Time Interval: {turbine.timeIntervals[i]}");
                Debug.Log($"Event Code: {turbine.eventCodes[i]}");
                Debug.Log($"Event Description: {turbine.eventCodeDescriptions[i]}");
                Debug.Log($"Wind Speed: {turbine.windSpeeds[i]} m/s");
                Debug.Log($"Ambient Temperature: {turbine.ambientTemperatures[i]} °C");
                Debug.Log($"Rotor Speed: {turbine.rotorSpeeds[i]} RPM");
                Debug.Log($"Power: {turbine.powers[i]} kW");
            }
        }
    }
}