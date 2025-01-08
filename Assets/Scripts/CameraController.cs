using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CameraController : MonoBehaviour
{
    public Transform cameraTransform; // Assignez la caméra à ce champ
    public TurbineDataContainer turbineDataContainer; // Assignez votre conteneur de données
    public TMP_Dropdown turbineDropdown; // Assignez le Dropdown pour la sélection des éoliennes
    public Vector3 defaultCameraPosition = new Vector3(100, 50, -50); // Position fixe de la caméra
    public Vector3 defaultCameraLookAt = Vector3.zero; // Point que la caméra regarde par défaut

    private void Start()
    {
        // Vérifiez que tout est bien assigné
        if (cameraTransform == null || turbineDataContainer == null || turbineDropdown == null)
        {
            Debug.LogError("Assignez toutes les références dans l'inspecteur.");
            return;
        }

        // Ajouter un listener pour le changement de sélection dans le Dropdown
        turbineDropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged(); });

        // Positionner la caméra sur l'ensemble du parc éolien par défaut
        PositionCameraOnWindFarm();
    }

    private void OnDropdownValueChanged()
    {
        Debug.Log("Dropdown value changed.");
        PositionCameraOnSelectedTurbine(); // Mettre à jour la position de la caméra lorsque l'éolienne sélectionnée change
    }

    private void PositionCameraOnSelectedTurbine()
    {
        // Récupérer l'éolienne sélectionnée
        string selectedTurbineID = turbineDropdown.options[turbineDropdown.value].text;
        Debug.Log($"Selected Turbine ID: {selectedTurbineID}");
        TurbineData selectedTurbine = turbineDataContainer.GetTurbineDataByID(selectedTurbineID);

        if (selectedTurbine == null)
        {
            Debug.LogError($"Aucune donnée trouvée pour l'éolienne avec l'ID: {selectedTurbineID}");
            return;
        }

        // Trouver l'objet de l'éolienne dans la scène
        GameObject selectedTurbineObject = GameObject.Find(selectedTurbineID);
        if (selectedTurbineObject == null)
        {
            Debug.LogError($"Aucun objet trouvé pour l'éolienne avec l'ID: {selectedTurbineID}");
            return;
        }

        Debug.Log($"Positioning camera to look at {selectedTurbineID}");

        // Positionner la caméra pour qu'elle regarde l'éolienne
        Vector3 cameraPosition = selectedTurbineObject.transform.position + new Vector3(150, 80, -10); // Ajustez cette position selon vos besoins
        cameraTransform.position = cameraPosition;
        cameraTransform.LookAt(selectedTurbineObject.transform);
    }

    private void PositionCameraOnWindFarm()
    {
        // Positionner la caméra à la position fixe par défaut
        cameraTransform.position = defaultCameraPosition;
        cameraTransform.LookAt(defaultCameraLookAt);
    }
}