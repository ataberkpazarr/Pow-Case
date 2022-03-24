using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    [SerializeField] private Material selectedMaterial, initialMaterial;
    [SerializeField] private MeshRenderer cubeMeshRenderer;
    [SerializeField] private Renderer renderer_;

    private bool isPlaced=false;

    private Color selectedColour;
    private Color initialColour;

    private Material[] materialArray;

    private bool notSetAsSelectedYet;

    private void Start()
    {
        selectedColour = selectedMaterial.color;
        initialColour = initialMaterial.color;
        //new Color(138f,138f,224f,255f)
        //materialArray = new Material[2];
        //materialArray[0] = selectedMaterial;
    }
    public void SetAsSelected()
    {

        renderer_.material.color = selectedColour;
        /*
        materialArray = new Material[1];
        materialArray[0] = selectedMaterial;
        cubeMeshRenderer.materials = materialArray;
      */
    }

    public void SetAsReleased()
    {
        /*
        materialArray = new Material[1];
        materialArray[0] = initialMaterial;
        cubeMeshRenderer.materials = materialArray;
        */
        renderer_.material.color = initialColour;
    }

    public void SetItAsPlaced()
    {
        isPlaced = true;
    }
}
