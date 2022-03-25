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




    private void Start()
    {
        selectedColour = selectedMaterial.color;
        initialColour = initialMaterial.color;
        matchedColour = matchMaterial.color;

    }
    public void SetAsSelected()
    {

        renderer_.material.color = selectedColour;

    }

    public void SetAsReleased()
    {

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

    public void HandleMatchAreaSpecifications() //if cube is at the match area
    {
        gameObject.transform.DOScale(new Vector3(0.6f, 0.6f, 0.6f), 0.5f);
        gameObject.layer = LayerMask.NameToLayer("UI");
        transform.parent = null;
        transform.rotation = Quaternion.identity;
    }

    private void HandleBoardSpecifications() // if cube went back to board
    {
        isPlaced = false;
        gameObject.transform.DOScale(new Vector3(1, 1, 1), 0.5f);
        gameObject.layer = LayerMask.NameToLayer("Default");
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
