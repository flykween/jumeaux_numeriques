using System.Collections.Generic;
using ChartAndGraph;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurbinePowerGraph : MonoBehaviour
{
    public GraphChart graphChart; // Assignez graphique dans l'inspecteur
    public TurbineDataContainer turbineDataContainer; // Assignez conteneur de données
    public List<string> turbineIDs; // Liste des IDs des éoliennes à afficher
    public Material lineMaterial; // Matériau pour la ligne du graphique
    public Material pointMaterial; // Matériau pour les points du graphique
    public double lineThickness = 1.0; // Épaisseur de la ligne
    public double pointSize = 1.0; // Taille des points
    public Slider timeSlider; // Slider pour sélectionner le temps
    public TextMeshProUGUI powerText; // Texte pour afficher la puissance totale

    void Start()
    {
        // Vérifiez que tout est bien assigné
        if (graphChart == null || turbineDataContainer == null || turbineIDs == null || turbineIDs.Count == 0 || timeSlider == null || powerText == null)
        {
            Debug.LogError("GraphChart, TurbineDataContainer, turbineIDs, timeSlider ou powerText non assigné.");
            return;
        }

        // Configurer le slider
        timeSlider.minValue = 0;
        timeSlider.maxValue = turbineDataContainer.turbines[0].timeIntervals.Length - 1;
        timeSlider.onValueChanged.AddListener(UpdatePowerText);

        // Ajouter la catégorie "Power" si elle n'existe pas
        graphChart.DataSource.StartBatch();
        if (!graphChart.DataSource.HasCategory("Power"))
        {
            graphChart.DataSource.AddCategory("Power", lineMaterial, lineThickness, new MaterialTiling(), pointMaterial, true, pointMaterial, pointSize, true);
        }
        graphChart.DataSource.EndBatch();

        // Mettre à jour le graphique avec les données des éoliennes
        UpdateGraph();
    }

    void UpdateGraph()
    {
        // Dictionnaire pour stocker la somme des puissances par heure
        Dictionary<int, float> powerSums = new Dictionary<int, float>();

        // Initialiser le dictionnaire pour chaque heure de 0 à 23
        for (int i = 0; i < 24; i++)
        {
            powerSums[i] = 0f;
        }

        // Parcourir chaque éolienne
        foreach (var turbineID in turbineIDs)
        {
            TurbineData turbineData = turbineDataContainer.GetTurbineDataByID(turbineID);
            if (turbineData == null)
            {
                Debug.LogError($"Aucune donnée trouvée pour l'éolienne avec l'ID: {turbineID}");
                continue;
            }

            // Ajouter les puissances pour chaque intervalle de temps
            for (int i = 0; i < turbineData.timeIntervals.Length; i++)
            {
                int hour = GetHourFromTimeInterval(turbineData.timeIntervals[i]);
                if (hour >= 0 && hour < 24)
                {
                    powerSums[hour] += turbineData.powers[i];
                }
            }
        }

        // Ajouter les points au graphique
        graphChart.DataSource.StartBatch();
        graphChart.DataSource.ClearCategory("Power");
        for (int i = 0; i < 24; i++)
        {
            graphChart.DataSource.AddPointToCategory("Power", i + 1, powerSums[i]); // Utiliser i + 1 pour que l'axe X commence à 1
        }
        graphChart.DataSource.EndBatch();
    }

    void UpdatePowerText(float sliderValue)
    {
        int timeIndex = Mathf.RoundToInt(sliderValue);
        float totalPower = 0f;

        // Calculer la puissance totale pour l'index de temps sélectionné
        foreach (var turbineID in turbineIDs)
        {
            TurbineData turbineData = turbineDataContainer.GetTurbineDataByID(turbineID);
            if (turbineData != null && timeIndex < turbineData.powers.Length)
            {
                totalPower += turbineData.powers[timeIndex];
            }
        }

        // Afficher la puissance totale
        powerText.text = $"Puissance Totale: {totalPower} kW";
    }

    int GetHourFromTimeInterval(string timeInterval)
    {
        // Extraire l'heure de début de l'intervalle de temps (format "HH:mm-HH:mm")
        string[] parts = timeInterval.Split('-');
        if (parts.Length > 0)
        {
            string[] timeParts = parts[0].Split(':');
            if (timeParts.Length > 0 && int.TryParse(timeParts[0], out int hour))
            {
                return hour;
            }
        }
        return -1; // Retourner -1 si l'heure n'a pas pu être extraite
    }
}