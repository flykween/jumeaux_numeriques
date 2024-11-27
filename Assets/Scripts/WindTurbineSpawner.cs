using System.Collections.Generic;
using UnityEngine;
using System.Globalization;  // Pour utiliser NumberStyles et CultureInfo
 
 
public class WindTurbineSpawner : MonoBehaviour
{
    public GameObject windTurbinePrefab; // Assigner le prefab d'éolienne
    public string csvPath;              // Chemin vers le fichier CSV
    private Dictionary<string, List<string[]>> turbineData = new Dictionary<string, List<string[]>>();
    private Dictionary<string, WindTurbineBehavior> turbinesInScene = new Dictionary<string, WindTurbineBehavior>();
 
    public int gridSize = 10000;  // Taille de la grille (par exemple, 10x10)
    public float spacing = 200000f;  // Espacement entre les éoliennes (en unités Unity)
 
    void Start()
    {
        ParseCSV();
        SpawnTurbines();
    }
 
    void Update()
    {
        UpdateSimulation();
    }
 
    private void ParseCSV()
    {
        // Charger le fichier CSV depuis le dossier Resources
        TextAsset csvFile = Resources.Load<TextAsset>("turbine_data");
 
        if (csvFile == null)
        {
            Debug.LogError("CSV file not found in Resources!");
            return;
        }
 
        string[] lines = csvFile.text.Split('\n'); // Lire les lignes depuis TextAsset
 
        for (int i = 1; i < lines.Length; i++) // Ignorer la première ligne si elle est l'en-tête
        {
            string[] columns = lines[i].Split(',');
 
            if (columns.Length < 8) continue; // Vérifier si chaque ligne contient suffisamment de colonnes
 
            string turbineID = columns[0].Trim(); // ID de l'éolienne
 
            // Nettoyer et vérifier les données avant de tenter la conversion
            if (!IsValidNumber(columns[4].Trim(), out float windSpeed))
            {
                Debug.LogWarning($"Invalid windSpeed at line {i}: {columns[4].Trim()}");
                continue; // Si la conversion échoue, passer à la ligne suivante
            }
 
            if (!IsValidNumber(columns[5].Trim(), out float ambientTemperature))
            {
                Debug.LogWarning($"Invalid ambientTemperature at line {i}: {columns[5].Trim()}");
                continue; // Si la conversion échoue, passer à la ligne suivante
            }
 
            if (!IsValidNumber(columns[6].Trim(), out float rotorSpeed))
            {
                Debug.LogWarning($"Invalid rotorSpeed at line {i}: {columns[6].Trim()}");
                continue; // Si la conversion échoue, passer à la ligne suivante
            }
 
            if (!IsValidNumber(columns[7].Trim(), out float power))
            {
                Debug.LogWarning($"Invalid power at line {i}: {columns[7].Trim()}");
                continue; // Si la conversion échoue, passer à la ligne suivante
            }
 
            // Ajouter les données au dictionnaire si toutes les conversions sont réussies
            if (!turbineData.ContainsKey(turbineID))
            {
                turbineData[turbineID] = new List<string[]>();
            }
            turbineData[turbineID].Add(columns); // Ajouter les données de cette ligne
        }
    }
 
    // Fonction pour vérifier si une valeur est un nombre valide
    private bool IsValidNumber(string value, out float result)
    {
        result = 0;
 
        // Nettoyer la chaîne en retirant les espaces
        string cleanedValue = value.Trim();
 
        // Remplacer les virgules par des points si nécessaire
        cleanedValue = cleanedValue.Replace(",", ".");
 
        // Vérifier si la chaîne peut être convertie en float
        bool isValid = float.TryParse(cleanedValue, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
        if (!isValid)
        {
            Debug.LogWarning($"Invalid number format: '{value}'");
        }
        return isValid;
    }
 
    private void SpawnTurbines()
    {
        // Calculer l'emplacement de chaque éolienne sur une grille carrée
        int rowCount = gridSize; // Nombre de lignes de la grille, fixé par la taille de la grille
        int colCount = gridSize; // Nombre de colonnes de la grille, fixé par la taille de la grille

 
        int turbineIndex = 0; // Pour suivre l'index des éoliennes à placer
 
        foreach (var entry in turbineData)
        {
            string turbineID = entry.Key;
 
            // Calculer la position sur la grille
            int row = turbineIndex / colCount;  // Calculer la ligne de la grille
            int col = turbineIndex % colCount;  // Calculer la colonne de la grille
 
            float x = col * spacing;  // Espacement en X
            float z = row * spacing;  // Espacement en Z
            float y = 0f; // Hauteur de l'éolienne
 
            GameObject turbine = Instantiate(windTurbinePrefab, new Vector3(x, y, z), Quaternion.identity);
            WindTurbineBehavior behavior = turbine.GetComponent<WindTurbineBehavior>();
            behavior.turbineID = turbineID;
 
            turbinesInScene[turbineID] = behavior; // Stocker dans le dictionnaire
 
            turbineIndex++; // Passer à l'éolienne suivante
        }
    }
 
    private void UpdateSimulation()
    {
        float time = Time.time; // Temps écoulé
        foreach (var entry in turbineData)
        {
            string turbineID = entry.Key;
            List<string[]> data = entry.Value;
 
            // Déterminer l'intervalle des données
            int interval = (int)(time / 10) % data.Count;
            string[] columns = data[interval];
 
            // Vérifier et loguer les données avant la conversion
            Debug.Log($"Turbine {turbineID}: WindSpeed='{columns[4]}', Temp='{columns[5]}', RotorSpeed='{columns[6]}', Power='{columns[7]}'");
 
            // Appliquer les données à l'éolienne correspondante
            if (turbinesInScene.ContainsKey(turbineID))
            {
                WindTurbineBehavior behavior = turbinesInScene[turbineID];
 
                // Assurez-vous que les chaînes peuvent être converties en float
                if (IsValidNumber(columns[4].Trim(), out float windSpeed) &&
                    IsValidNumber(columns[5].Trim(), out float ambientTemperature) &&
                    IsValidNumber(columns[6].Trim(), out float rotorSpeed) &&
                    IsValidNumber(columns[7].Trim(), out float power))
                {
                    behavior.UpdateData(windSpeed, ambientTemperature, rotorSpeed, power);
                }
                else
                {
                    Debug.LogWarning($"Invalid data at turbine {turbineID}, skipping update.");
                }
            }
        }
    }
}