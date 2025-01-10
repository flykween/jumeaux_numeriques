using System.Collections.Generic;
using ChartAndGraph;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurbinePowerGraph : MonoBehaviour
{
    public GraphChart graphChart; // Assignez graphique dans l'inspecteur
    public TurbineDataContainer turbineDataContainer; // Assignez conteneur de donn�es
    public List<string> turbineIDs; // Liste des IDs des �oliennes � afficher
    public Material lineMaterial; // Mat�riau pour la ligne du graphique
    public Material pointMaterial; // Mat�riau pour les points du graphique
    public double lineThickness = 1.0; // �paisseur de la ligne
    public double pointSize = 1.0; // Taille des points
    public Slider timeSlider; // Slider pour s�lectionner le temps
    public TextMeshProUGUI powerText; // Texte pour afficher la puissance totale

    void Start()
    {
        // V�rifiez que tout est bien assign�
        if (graphChart == null || turbineDataContainer == null || turbineIDs == null || turbineIDs.Count == 0 || timeSlider == null || powerText == null)
        {
            Debug.LogError("GraphChart, TurbineDataContainer, turbineIDs, timeSlider ou powerText non assign�.");
            return;
        }

        // Configurer le slider
        timeSlider.minValue = 0;
        timeSlider.maxValue = turbineDataContainer.turbines[0].timeIntervals.Length - 1;
        timeSlider.onValueChanged.AddListener(UpdatePowerText);

        // Ajouter la cat�gorie "Power" si elle n'existe pas
        graphChart.DataSource.StartBatch();
        if (!graphChart.DataSource.HasCategory("Power"))
        {
            graphChart.DataSource.AddCategory("Power", lineMaterial, lineThickness, new MaterialTiling(), pointMaterial, true, pointMaterial, pointSize, true);
        }
        graphChart.DataSource.EndBatch();

        // Mettre � jour le graphique avec les donn�es des �oliennes
        UpdateGraph();
    }

    void UpdateGraph()
    {
        // Dictionnaire pour stocker la somme des puissances par heure
        Dictionary<int, float> powerSums = new Dictionary<int, float>();

        // Initialiser le dictionnaire pour chaque heure de 0 � 23
        for (int i = 0; i < 24; i++)
        {
            powerSums[i] = 0f;
        }

        // Parcourir chaque �olienne
        foreach (var turbineID in turbineIDs)
        {
            TurbineData turbineData = turbineDataContainer.GetTurbineDataByID(turbineID);
            if (turbineData == null)
            {
                Debug.LogError($"Aucune donn�e trouv�e pour l'�olienne avec l'ID: {turbineID}");
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
            graphChart.DataSource.AddPointToCategory("Power", i + 1, powerSums[i]); // Utiliser i + 1 pour que l'axe X commence � 1
        }
        graphChart.DataSource.EndBatch();
    }

    void UpdatePowerText(float sliderValue)
    {
        int timeIndex = Mathf.RoundToInt(sliderValue);
        float totalPower = 0f;

        // Calculer la puissance totale pour l'index de temps s�lectionn�
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
        // Extraire l'heure de d�but de l'intervalle de temps (format "HH:mm-HH:mm")
        string[] parts = timeInterval.Split('-');
        if (parts.Length > 0)
        {
            string[] timeParts = parts[0].Split(':');
            if (timeParts.Length > 0 && int.TryParse(timeParts[0], out int hour))
            {
                return hour;
            }
        }
        return -1; // Retourner -1 si l'heure n'a pas pu �tre extraite
    }
}