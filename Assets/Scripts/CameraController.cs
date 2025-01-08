using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CameraController : MonoBehaviour
{
    public Transform cameraTransform; // Assignez la cam�ra � ce champ
    public TurbineDataContainer turbineDataContainer; // Assignez votre conteneur de donn�es
    public TMP_Dropdown turbineDropdown; // Assignez le Dropdown pour la s�lection des �oliennes
    public Vector3 defaultCameraPosition = new Vector3(100, 50, -50); // Position fixe de la cam�ra
    public Vector3 defaultCameraLookAt = Vector3.zero; // Point que la cam�ra regarde par d�faut

    private void Start()
    {
        // V�rifiez que tout est bien assign�
        if (cameraTransform == null || turbineDataContainer == null || turbineDropdown == null)
        {
            Debug.LogError("Assignez toutes les r�f�rences dans l'inspecteur.");
            return;
        }

        // Ajouter un listener pour le changement de s�lection dans le Dropdown
        turbineDropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged(); });

        // Positionner la cam�ra sur l'ensemble du parc �olien par d�faut
        PositionCameraOnWindFarm();
    }

    private void OnDropdownValueChanged()
    {
        Debug.Log("Dropdown value changed.");
        PositionCameraOnSelectedTurbine(); // Mettre � jour la position de la cam�ra lorsque l'�olienne s�lectionn�e change
    }

    private void PositionCameraOnSelectedTurbine()
    {
        // R�cup�rer l'�olienne s�lectionn�e
        string selectedTurbineID = turbineDropdown.options[turbineDropdown.value].text;
        Debug.Log($"Selected Turbine ID: {selectedTurbineID}");
        TurbineData selectedTurbine = turbineDataContainer.GetTurbineDataByID(selectedTurbineID);

        if (selectedTurbine == null)
        {
            Debug.LogError($"Aucune donn�e trouv�e pour l'�olienne avec l'ID: {selectedTurbineID}");
            return;
        }

        // Trouver l'objet de l'�olienne dans la sc�ne
        GameObject selectedTurbineObject = GameObject.Find(selectedTurbineID);
        if (selectedTurbineObject == null)
        {
            Debug.LogError($"Aucun objet trouv� pour l'�olienne avec l'ID: {selectedTurbineID}");
            return;
        }

        Debug.Log($"Positioning camera to look at {selectedTurbineID}");

        // Positionner la cam�ra pour qu'elle regarde l'�olienne
        Vector3 cameraPosition = selectedTurbineObject.transform.position + new Vector3(150, 80, -10); // Ajustez cette position selon vos besoins
        cameraTransform.position = cameraPosition;
        cameraTransform.LookAt(selectedTurbineObject.transform);
    }

    private void PositionCameraOnWindFarm()
    {
        // Positionner la cam�ra � la position fixe par d�faut
        cameraTransform.position = defaultCameraPosition;
        cameraTransform.LookAt(defaultCameraLookAt);
    }
}