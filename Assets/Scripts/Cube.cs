using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum CubeType
{
    Avacado,
    Bell,
    BlueBerry,
    Book,
    Bottle,
    Branch,
    Cake,
    Carrot,
    Cherry,
    Cup,
    Cupcake,
    Diamond,
    Donut,
    IceCream
}

public class Cube : MonoBehaviour
{
    [SerializeField] private Material selectedMaterial, initialMaterial,matchMaterial;
    [SerializeField] private MeshRenderer cubeMeshRenderer;
    [SerializeField] private Renderer renderer_;
    [SerializeField] private CubeType cubeType;

    public CubeType CubeTypeInfo => cubeType;
    private Vector3 firstPosition;
    private Transform cubesParent;

    private bool isPlaced=false;
    private bool isMatched = false;


    private Color selectedColour;
    private Color initialColour;
    private Color matchedColour;


    private Material[] materialArray;

    private bool notSetAsSelectedYet;

    private void Start()
    {
        selectedColour = selectedMaterial.color;
        initialColour = initialMaterial.color;
        matchedColour = matchMaterial.color;
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
    public void SetAsMatched()
    {
        renderer_.material.color = matchedColour;
        isMatched = true;
    }

    public void SetItAsTakenToMatchArea()
    {
        isPlaced = true;
    }

    public bool IsItTakenToMatchArea()
    {

        return isPlaced;
    }

    public void SetItAsNotPlacedInMatchArea()
    {
        isPlaced = false;
    }

    public bool GoToFirstBoardPosition()
    {
        if (!isMatched) // if already matched, then dont let it go back to board again
        {
            HandleBoardSpecifications();
            return true;

        }
        else
        {
            return false;
        }
    }

    public void HandleMatchAreaSpecifications()
    {
        gameObject.transform.DOScale(new Vector3(0.6f, 0.6f, 0.6f), 0.5f);
        gameObject.layer = LayerMask.NameToLayer("UI");
        transform.parent = null;
        transform.rotation = Quaternion.identity;
    }

    private void HandleBoardSpecifications()
    {
        isPlaced = false;
        gameObject.transform.DOScale(new Vector3(1, 1, 1), 0.5f);
        gameObject.layer = LayerMask.NameToLayer("Default");
        //transform.rotation = Quaternion.identity;
        transform.parent = cubesParent;
        this.transform.localPosition = firstPosition;
        this.transform.rotation = cubesParent.rotation;

    }

    private void SaveFirstPositionOnBoard()
    {
        firstPosition = this.transform.localPosition;
        cubesParent = GamePlayManager.Instance.GetCubesParentTransform();
    }

    private void OnEnable()
    {
        LevelManager.onCubeSpawnCompleted += SaveFirstPositionOnBoard;
    }

    private void OnDisable()
    {
        LevelManager.onCubeSpawnCompleted -= SaveFirstPositionOnBoard;

    }
}
