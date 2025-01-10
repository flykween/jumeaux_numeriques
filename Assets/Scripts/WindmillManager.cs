using System.Collections;
using UnityEngine;

public class WindmillManager : MonoBehaviour
{
    public string turbineID; 
    public TurbineDataContainer turbineDataContainer; 
    public Transform windmillBlades; 
    public Vector3 rotationAxis = Vector3.forward; 

    private TurbineData turbineData; 
    private int currentIntervalIndex = 0; 

    private void Start()
    {
        turbineData = turbineDataContainer.GetTurbineDataByID(turbineID);

        if (turbineData != null)
        {
            Debug.Log($"Windmill {turbineData.turbineID} initialized with {turbineData.timeIntervals.Length} data entries.");
            StartCoroutine(RotateWindmill());
        }
        else
        {
            Debug.LogError($"No TurbineData found for ID: {turbineID}");
        }
    }

    private IEnumerator RotateWindmill()
    {
        while (true)
        {
            if (turbineData == null || turbineData.rotorSpeeds.Length == 0)
            {
                Debug.LogError("No rotor speed data available.");
                yield break;
            }

     
            float rotorSpeed = turbineData.rotorSpeeds[currentIntervalIndex]; 
            string timeInterval = turbineData.timeIntervals[currentIntervalIndex];
            float duration = ParseTimeIntervalToSeconds(timeInterval);

           
            Debug.Log($"Current RPM for Turbine {turbineID}: {rotorSpeed}");

          
            float rotationSpeed = rotorSpeed * 6f; 

            float elapsedTime = 0f;

           
            while (elapsedTime < duration)
            {
                float deltaRotation = rotationSpeed * Time.deltaTime;
                windmillBlades.Rotate(rotationAxis, deltaRotation); 
                elapsedTime += Time.deltaTime;

                CheckForTimeIntervalChange();
                yield return null;
            }

            currentIntervalIndex = (currentIntervalIndex + 1) % turbineData.rotorSpeeds.Length;
        }
    }


    private float ParseTimeIntervalToSeconds(string timeInterval)
    {
        string[] parts = timeInterval.Split('-');
        if (parts.Length != 2)
        {
            Debug.LogWarning($"Invalid time interval format: {timeInterval}. Defaulting to 1 second.");
            return 1f; 
        }

        float startSeconds = TimeToSeconds(parts[0]);
        float endSeconds = TimeToSeconds(parts[1]);

        if (endSeconds < startSeconds)
        {
            Debug.LogWarning("End time is earlier than start time. Using default duration of 1 second.");
            return 1f;
        }

        return endSeconds - startSeconds;
    }


    private float TimeToSeconds(string timeString)
    {
        string[] timeParts = timeString.Split(':');
        int seconds = 0;

        if (timeParts.Length == 3)
        {
            // HH:mm:ss format
            seconds += int.Parse(timeParts[0]) * 3600; 
            seconds += int.Parse(timeParts[1]) * 60;  
            seconds += int.Parse(timeParts[2]);     
        }
        else if (timeParts.Length == 2)
        {
            // mm:ss format
            seconds += int.Parse(timeParts[0]) * 60;   
            seconds += int.Parse(timeParts[1]);    
        }

        return seconds;
    }


    private void CheckForTimeIntervalChange()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentIntervalIndex = (currentIntervalIndex + 1) % turbineData.timeIntervals.Length;
            Debug.Log($"Time interval changed to {turbineData.timeIntervals[currentIntervalIndex]}");
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentIntervalIndex = (currentIntervalIndex - 1 + turbineData.timeIntervals.Length) % turbineData.timeIntervals.Length;
            Debug.Log($"Time interval changed to {turbineData.timeIntervals[currentIntervalIndex]}");
        }
    }
}