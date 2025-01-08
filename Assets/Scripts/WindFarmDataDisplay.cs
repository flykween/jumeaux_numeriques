using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WindFarmDataDisplay : MonoBehaviour
{
    public TurbineDataContainer turbineDataContainer; // Assignez votre conteneur de données
    public TextMeshProUGUI timeText; // Assignez le texte pour l'heure
    public TextMeshProUGUI temperatureText; // Assignez le texte pour la température
    public TextMeshProUGUI rotorSpeedText; // Assignez le texte pour la vitesse des rotors
    public TextMeshProUGUI powerText; // Assignez le texte pour la puissance
    public TMP_Dropdown turbineDropdown; // Assignez le Dropdown pour la sélection des éoliennes
    public Slider timeSlider; // Assignez le Slider pour la sélection du temps

    private void Start()
    {
        // Vérifiez que tout est bien assigné
        if (turbineDataContainer == null || timeText == null || temperatureText == null || rotorSpeedText == null || powerText == null || turbineDropdown == null || timeSlider == null)
        {
            Debug.LogError("Assignez toutes les références dans l'inspecteur.");
            return;
        }

        // Initialiser le Dropdown avec les IDs des éoliennes
        InitializeDropdown();

        // Initialiser le Slider avec les intervalles de temps
        InitializeSlider();

        // Afficher les données de la première éolienne et du premier intervalle de temps par défaut
        DisplayDataAtTime(0);
    }

    private void InitializeDropdown()
    {
        turbineDropdown.ClearOptions();
        List<string> options = new List<string>();

        foreach (var turbine in turbineDataContainer.turbines)
        {
            options.Add(turbine.turbineID);
        }

        turbineDropdown.AddOptions(options);
        turbineDropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged(); });
    }

    private void InitializeSlider()
    {
        timeSlider.minValue = 0;
        timeSlider.maxValue = turbineDataContainer.turbines[0].timeIntervals.Length - 1;
        timeSlider.onValueChanged.AddListener(delegate { OnSliderValueChanged(); });
    }

    private void OnDropdownValueChanged()
    {
        DisplayDataAtTime((int)timeSlider.value); // Mettre à jour l'affichage des données pour l'éolienne et le temps sélectionnés
    }

    private void OnSliderValueChanged()
    {
        DisplayDataAtTime((int)timeSlider.value); // Mettre à jour l'affichage des données pour l'éolienne et le temps sélectionnés
    }

    public void DisplayDataAtTime(int timeIndex)
    {
        if (timeIndex < 0 || timeIndex >= turbineDataContainer.turbines[0].timeIntervals.Length)
        {
            Debug.LogError("Index de temps invalide.");
            return;
        }

        // Récupérer l'éolienne sélectionnée
        string selectedTurbineID = turbineDropdown.options[turbineDropdown.value].text;
        TurbineData selectedTurbine = turbineDataContainer.GetTurbineDataByID(selectedTurbineID);

        if (selectedTurbine == null)
        {
            Debug.LogError($"Aucune donnée trouvée pour l'éolienne avec l'ID: {selectedTurbineID}");
            return;
        }

        // Mettre à jour les textes UI avec les données de l'éolienne sélectionnée
        timeText.text = $"Heure: {selectedTurbine.timeIntervals[timeIndex]}";
        temperatureText.text = $"Température Ambiante: {selectedTurbine.ambientTemperatures[timeIndex]} °C";
        rotorSpeedText.text = $"Vitesse des Rotors: {selectedTurbine.rotorSpeeds[timeIndex]} RPM";
        powerText.text = $"Puissance: {selectedTurbine.powers[timeIndex]} kW";
    }
}