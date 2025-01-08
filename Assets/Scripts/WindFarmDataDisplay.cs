using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WindFarmDataDisplay : MonoBehaviour
{
    public TurbineDataContainer turbineDataContainer; // Assignez votre conteneur de donn�es
    public TextMeshProUGUI timeText; // Assignez le texte pour l'heure
    public TextMeshProUGUI temperatureText; // Assignez le texte pour la temp�rature
    public TextMeshProUGUI rotorSpeedText; // Assignez le texte pour la vitesse des rotors
    public TextMeshProUGUI powerText; // Assignez le texte pour la puissance
    public TMP_Dropdown turbineDropdown; // Assignez le Dropdown pour la s�lection des �oliennes
    public Slider timeSlider; // Assignez le Slider pour la s�lection du temps

    private void Start()
    {
        // V�rifiez que tout est bien assign�
        if (turbineDataContainer == null || timeText == null || temperatureText == null || rotorSpeedText == null || powerText == null || turbineDropdown == null || timeSlider == null)
        {
            Debug.LogError("Assignez toutes les r�f�rences dans l'inspecteur.");
            return;
        }

        // Initialiser le Dropdown avec les IDs des �oliennes
        InitializeDropdown();

        // Initialiser le Slider avec les intervalles de temps
        InitializeSlider();

        // Afficher les donn�es de la premi�re �olienne et du premier intervalle de temps par d�faut
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
        DisplayDataAtTime((int)timeSlider.value); // Mettre � jour l'affichage des donn�es pour l'�olienne et le temps s�lectionn�s
    }

    private void OnSliderValueChanged()
    {
        DisplayDataAtTime((int)timeSlider.value); // Mettre � jour l'affichage des donn�es pour l'�olienne et le temps s�lectionn�s
    }

    public void DisplayDataAtTime(int timeIndex)
    {
        if (timeIndex < 0 || timeIndex >= turbineDataContainer.turbines[0].timeIntervals.Length)
        {
            Debug.LogError("Index de temps invalide.");
            return;
        }

        // R�cup�rer l'�olienne s�lectionn�e
        string selectedTurbineID = turbineDropdown.options[turbineDropdown.value].text;
        TurbineData selectedTurbine = turbineDataContainer.GetTurbineDataByID(selectedTurbineID);

        if (selectedTurbine == null)
        {
            Debug.LogError($"Aucune donn�e trouv�e pour l'�olienne avec l'ID: {selectedTurbineID}");
            return;
        }

        // Mettre � jour les textes UI avec les donn�es de l'�olienne s�lectionn�e
        timeText.text = $"Heure: {selectedTurbine.timeIntervals[timeIndex]}";
        temperatureText.text = $"Temp�rature Ambiante: {selectedTurbine.ambientTemperatures[timeIndex]} �C";
        rotorSpeedText.text = $"Vitesse des Rotors: {selectedTurbine.rotorSpeeds[timeIndex]} RPM";
        powerText.text = $"Puissance: {selectedTurbine.powers[timeIndex]} kW";
    }
}