using System.Collections;
using System.Collections.Generic;
using ChartAndGraph;
using UnityEngine;

public class TurbinePowerGraph : MonoBehaviour
{
    public GraphChart graphChart; // Assignez votre graphique dans l'inspecteur
    public TurbineDataContainer turbineDataContainer; // Assignez votre conteneur de donn�es
    public string turbineID; // ID de l'�olienne � afficher
    public Material lineMaterial; // Mat�riau pour la ligne du graphique
    public MaterialTiling lineTiling; // Tiling du mat�riau
    public Material pointMaterial; // Mat�riau pour les points du graphique
    public MaterialTiling pointTiling; // Tiling du mat�riau des points
    public double lineThickness = 1.0; // �paisseur de la ligne
    public double pointSize = 1.0; // Taille des points

    void Start()
    {
        if (graphChart == null || turbineDataContainer == null)
        {
            Debug.LogError("GraphChart ou TurbineDataContainer non assign�.");
            return;
        }

        // Ajouter des cat�gories de donn�es au graphique
        graphChart.DataSource.StartBatch();
        graphChart.DataSource.ClearCategory("Power");
        graphChart.DataSource.AddCategory("Power", lineMaterial, lineThickness, lineTiling, pointMaterial, true, pointMaterial, pointSize, true);
        graphChart.DataSource.EndBatch();

        // Mettre � jour le graphique avec les donn�es de l'�olienne sp�cifi�e
        UpdateGraph();
    }

    void UpdateGraph()
    {
        TurbineData turbineData = turbineDataContainer.GetTurbineDataByID(turbineID);

        if (turbineData == null)
        {
            Debug.LogError($"Aucune donn�e trouv�e pour l'�olienne avec l'ID: {turbineID}");
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