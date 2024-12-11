using System.Collections;
using System.Collections.Generic;
using ChartAndGraph;
using UnityEngine;

public class TurbinePowerGraph : MonoBehaviour
{
    public GraphChart graphChart; // Assignez votre graphique dans l'inspecteur
    public TurbineDataContainer turbineDataContainer; // Assignez votre conteneur de données
    public string turbineID; // ID de l'éolienne à afficher
    public Material lineMaterial; // Matériau pour la ligne du graphique
    public MaterialTiling lineTiling; // Tiling du matériau
    public Material pointMaterial; // Matériau pour les points du graphique
    public MaterialTiling pointTiling; // Tiling du matériau des points
    public double lineThickness = 1.0; // Épaisseur de la ligne
    public double pointSize = 1.0; // Taille des points

    void Start()
    {
        if (graphChart == null || turbineDataContainer == null)
        {
            Debug.LogError("GraphChart ou TurbineDataContainer non assigné.");
            return;
        }

        // Ajouter des catégories de données au graphique
        graphChart.DataSource.StartBatch();
        graphChart.DataSource.ClearCategory("Power");
        graphChart.DataSource.AddCategory("Power", lineMaterial, lineThickness, lineTiling, pointMaterial, true, pointMaterial, pointSize, true);
        graphChart.DataSource.EndBatch();

        // Mettre à jour le graphique avec les données de l'éolienne spécifiée
        UpdateGraph();
    }

    void UpdateGraph()
    {
        TurbineData turbineData = turbineDataContainer.GetTurbineDataByID(turbineID);

        if (turbineData == null)
        {
            Debug.LogError($"Aucune donnée trouvée pour l'éolienne avec l'ID: {turbineID}");
            return;
        }

        graphChart.DataSource.StartBatch();
        for (int i = 0; i < turbineData.timeIntervals.Length; i++)
        {
            float time = i; // Utiliser l'index comme temps pour simplifier
            float power = turbineData.powers[i];
            graphChart.DataSource.AddPointToCategory("Power", time, power);
        }
        graphChart.DataSource.EndBatch();
    }
}