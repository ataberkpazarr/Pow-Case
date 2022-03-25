using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using DG.Tweening;
public class GamePlayManager : Singleton<GamePlayManager>
{

    [SerializeField] private Transform slot1;
    [SerializeField] private Camera canvasCamera;

    private bool TouchActive = false;
    private bool cubeHolded = false;
    private Cube previouslySelectedCube;

    private GameObject cubesParent;
    private Cube[,,] cubes;
    private Vector2 previousTouchPos;
    private Vector2 startTouchPos;
    
    private int lastPlacedSlotPosition=0;
    //private bool notRotatedYet = true;

    private bool notRotated=true;

    private List<Cube> placedCubes;


    private void Start()
    {
        int[] spawnSizes = LevelManager.Instance.GetInitialSizes();
        cubes = new Cube[spawnSizes[0], spawnSizes[1], spawnSizes[2]];
    }
    private void OnFingerDown(LeanFinger leanFinger) //first touch
    {
        TouchActive = true;

        previousTouchPos = leanFinger.StartScreenPosition;
        Ray ray = leanFinger.GetRay();
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo)) //check if a cube in board holded when user inputs a touch
        {
            if (hitInfo.collider != null && hitInfo.collider.gameObject.CompareTag("Cube"))
            {
                cubeHolded = true;
            }
        }


        RaycastHit hit_;
        Ray ray_ = canvasCamera.ScreenPointToRay(leanFinger.StartScreenPosition); // ray through the canvas camera

        if (Physics.Raycast(ray_, out hit_)) //check if user trying to touch into a cube in the match area 
        {
            
            if (hit_.collider != null && hit_.collider.gameObject.CompareTag("Cube"))
            {
                Cube cube = hit_.collider.gameObject.GetComponent<Cube>();
                if (cube.GoToFirstBoardPosition()) //if user touched into a cube in the match area, try it send back to board, this will be succeded if related cube has not already matched
                {
                    MatchManager.Instance.HandleCubesThatBackedToBoard();
                    lastPlacedSlotPosition -= 1; //update last placed slot position for the cubes that will newly placed in match area
                }
                
                
            }
        }
           
        }

    private void OnFingerUp(LeanFinger leanFinger) // touch released
    {

        TouchActive = false;
        if (previouslySelectedCube!=null)
        {
            previouslySelectedCube.SetAsReleased();
            previouslySelectedCube = null;
        }
      
        //notRotatedYet = true;

        if (notRotated) // if not rotated, than take touched cube into the match area
        {

            Ray ray = leanFinger.GetRay();
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.collider != null && hitInfo.collider.gameObject.CompareTag("Cube"))
                {
                    Vector3 pos = hitInfo.collider.gameObject.transform.localPosition;
                    int x = (int)pos.x;
                    int y = (int)pos.y;
                    int z = (int)pos.z;
                    Cube selectedCube = cubes[x, y, z];

                    Vector3 posToPlace = slot1.position;
                    posToPlace.x = posToPlace.x + lastPlacedSlotPosition * 0.61f;

                    selectedCube.transform.position = posToPlace;
                    lastPlacedSlotPosition += 1;

                    selectedCube.SetItAsTakenToMatchArea();
                    selectedCube.HandleMatchAreaSpecifications();
        
                    MatchManager.Instance.AddCubeToActiveCubesList(selectedCube);

                }
            }

            
        }
        notRotated = true;
        cubeHolded = false;

    }

    private void OnFingerUpdate(LeanFinger leanFinger) // touch continues
    {

        if (TouchActive && cubeHolded) // if touch continues and a cube in board is holded
        {
            
            Ray ray = leanFinger.GetRay();

            Cube selectedCube=null;
            Vector2 touchPos = leanFinger.ScreenPosition;
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.collider != null && hitInfo.collider.gameObject.CompareTag("Cube")) // if a cube is continued to being holded
                {
                    //cubes indexed as x,y,z local positions under a parent object and all are being restored in the cubes array
                    //rather than reaching cube by hitInfo.collider.gameObject.GetCompenent<Cube>(), I check the local position of holded cube
                    //and reaching that cube from cubes array with respected to its local position x,y,x and cubes[x,y,z]
                 
                    Vector3 pos = hitInfo.collider.gameObject.transform.localPosition;
                    int x = (int)pos.x;
                    int y = (int)pos.y;
                    int z = (int)pos.z;

                    selectedCube = cubes[x,y,z];

                    if (previouslySelectedCube!=null&&previouslySelectedCube!=selectedCube)
                    {
                        previouslySelectedCube.SetAsReleased();
                        selectedCube.SetAsSelected();
                        previouslySelectedCube = selectedCube;
                    }
                    else if (previouslySelectedCube == null)
                    {
                        previouslySelectedCube = selectedCube;
                        selectedCube.SetAsSelected();
                    }
           
                }
            }

     
            
            if (previousTouchPos!=touchPos) //time to rotate
            {
                Vector3 posDelta = touchPos - previousTouchPos;
                Vector2 posDelta_ = touchPos - previousTouchPos;

                float xDelta = startTouchPos.x - touchPos.x;
                float rotX = touchPos.x * 10f * Mathf.Deg2Rad * Time.deltaTime;

                cubesParent.transform.Rotate(transform.up, -Vector2.Dot(posDelta_, Camera.main.transform.right)*15f*Time.deltaTime, Space.World);
                cubesParent.transform.Rotate(Camera.main.transform.right, Vector2.Dot(posDelta_, Camera.main.transform.up) * 15f * Time.deltaTime, Space.World);
                notRotated = false;
 
                previousTouchPos = touchPos;


            }

        }
    }
    public Transform GetCubesParentTransform()
    {

        return cubesParent.transform;
    }
    private void HandleNewMatch()
    {
        lastPlacedSlotPosition -= 3;
    }
   

    private void GetSpawnedCubes() //when spawn operation done by levelManager, onCubeSpawnCompleted event is being fired and comes to this function
    {

        cubes= LevelManager.Instance.GetSpawnedCubes();
        cubesParent = cubes[0, 0, 0].transform.parent.gameObject;
    }


    private void OnEnable()
    {
        LeanTouch.OnFingerDown += OnFingerDown;
        LeanTouch.OnFingerUpdate += OnFingerUpdate;
        LeanTouch.OnFingerUp += OnFingerUp;
        LevelManager.onCubeSpawnCompleted += GetSpawnedCubes;
        MatchManager.onMatchOccured += HandleNewMatch;


    }

    private void OnDisable()
    {
        LeanTouch.OnFingerDown -= OnFingerDown;
        LeanTouch.OnFingerUpdate -= OnFingerUpdate;
        LeanTouch.OnFingerUp -= OnFingerUp;
        LevelManager.onCubeSpawnCompleted -= GetSpawnedCubes;
        MatchManager.onMatchOccured -= HandleNewMatch;


    }
}
