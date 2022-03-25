using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using DG.Tweening;
public class GamePlayManager : Singleton<GamePlayManager>
{

    [SerializeField] private Transform slot1, slot2, slot3;
    [SerializeField] private Camera canvasCamera;

    private bool TouchActive = false;
    private bool cubeHolded = false;
    private Cube previouslySelectedCube;

    private GameObject cubesParent;
    private Cube[,,] cubes;
    private Vector2 previousTouchPos;
    private Vector2 startTouchPos;
    //private Vector2 previousTouchPos;
    private int lastPlacedSlotPosition=0;
    private bool notRotatedYet = true;

    private bool notRotated=true;

    private List<Cube> placedCubes;
    private void Start()
    {
        int[] spawnSizes = LevelManager.Instance.GetInitialSizes();
        cubes = new Cube[spawnSizes[0], spawnSizes[1], spawnSizes[2]];
    }
    private void OnFingerDown(LeanFinger leanFinger)
    {
        TouchActive = true;

        previousTouchPos = leanFinger.StartScreenPosition;
        Ray ray = leanFinger.GetRay();
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            if (hitInfo.collider != null && hitInfo.collider.gameObject.CompareTag("Cube"))
            {
                cubeHolded = true;
                Debug.Log("aaaa");
            }
        }


        RaycastHit hit_;
        Ray ray_ = canvasCamera.ScreenPointToRay(leanFinger.StartScreenPosition);

        if (Physics.Raycast(ray_, out hit_))
        {
            //Transform objectHit = hit_.transform;
            if (hit_.collider != null && hit_.collider.gameObject.CompareTag("Cube"))
            {
                Cube cube = hit_.collider.gameObject.GetComponent<Cube>();
                if (cube.GoToFirstBoardPosition())
                {
                    
                    MatchManager.Instance.HandleCubesThatBackedToBoard();
                    lastPlacedSlotPosition -= 1;
                }
                
                
            }
        }
            //Vector3 mousePos = Camera.main.ScreenToWorldPoint();   
            /*
            Ray ray_ = leanFinger.GetStartRay();


            RaycastHit2D hitNormalTile = Physics2D.GetRayIntersection(ray_);

            if (hitNormalTile && hitNormalTile.collider.gameObject.CompareTag("Cube")) //if ray collided to something and that something is a normal tile
            {
                Debug.Log("kkk");
            }
            */
            /*
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
                    Sequence seq = DOTween.Sequence();
                    //seq.Append(selectedCube.gameObject.transform.DOMove(slot1.position,1f));
                    //seq.Append(selectedCube.gameObject.transform.DOScale(new Vector3(0.6f,0.6f,0.6f),0.2f));
                    //seq.Play();
                    selectedCube.gameObject.transform.DOScale(new Vector3(0.6f, 0.6f, 0.6f), 0.5f);
                    selectedCube.gameObject.layer = LayerMask.NameToLayer("UI");
                    selectedCube.SetItAsPlaced();
                    placedCubes.Add(selectedCube);

                }
            }
            */
        }

    private void OnFingerUp(LeanFinger leanFinger)
    {

        TouchActive = false;
        //bak buraya
        if (previouslySelectedCube!=null)
        {
            previouslySelectedCube.SetAsReleased();
            previouslySelectedCube = null;
        }
      
        notRotatedYet = true;

        if (notRotated)
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
                    Sequence seq = DOTween.Sequence();
                    //seq.Append(selectedCube.gameObject.transform.DOMove(slot1.position,1f));
                    //seq.Append(selectedCube.gameObject.transform.DOScale(new Vector3(0.6f,0.6f,0.6f),0.2f));
                    //seq.Play();
                    /*
                    selectedCube.gameObject.transform.DOScale(new Vector3(0.6f, 0.6f, 0.6f), 0.5f);
                    selectedCube.gameObject.layer = LayerMask.NameToLayer("UI");
                    selectedCube.transform.parent = null;
                    selectedCube.transform.rotation = Quaternion.identity;
                    */
                    selectedCube.SetItAsTakenToMatchArea();

                    selectedCube.HandleMatchAreaSpecifications();
                    //placedCubes.Add(selectedCube);
                    MatchManager.Instance.AddCubeToActiveCubesList(selectedCube);

                }
            }

            
        }
        notRotated = true;
        cubeHolded = false;

    }

    private void OnFingerUpdate(LeanFinger leanFinger)
    {

        if (TouchActive && cubeHolded)
        {
            
            Ray ray = leanFinger.GetRay();
            //Vector3 touchPos = ray.origin;
            //Vector3 touchPos = leanFinger.GetWorldPosition(5,Camera.main);
            Cube selectedCube=null;
             Vector2 touchPos = leanFinger.ScreenPosition;
            //leanFinger.
            /*
            if (previousTouchPos==null)
            {
                //Vector2 touchPos_ = leanFinger.ScreenPosition;
                previousTouchPos = touchPos;
                //previousTouchPos = touchPos;

            }
            */
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.collider != null && hitInfo.collider.gameObject.CompareTag("Cube"))
                {
                    
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
                    //Debug.Log(pos);
                   
                }
            }

            //if (previousTouchPos!=touchPos)
            //{
                //float f=leanFinger.GetDeltaDegrees(previousTouchPos);
                
                //Vector3 posDelta = touchPos - previousTouchPos;
            
            if (previousTouchPos!=touchPos)
            {
                Vector3 posDelta = touchPos - previousTouchPos;
                Vector2 posDelta_ = touchPos - previousTouchPos;

                float xDelta = startTouchPos.x - touchPos.x;
                float rotX = touchPos.x * 10f * Mathf.Deg2Rad * Time.deltaTime;

                cubesParent.transform.Rotate(transform.up, -Vector2.Dot(posDelta_, Camera.main.transform.right)*15f*Time.deltaTime, Space.World);
                cubesParent.transform.Rotate(Camera.main.transform.right, Vector2.Dot(posDelta_, Camera.main.transform.up) * 15f * Time.deltaTime, Space.World);
                //cubesParent.transform.Rotate(Camera.main.transform.right, Vector2.Dot(posDelta_, Camera.main.transform.right) * 15f * Time.deltaTime, Space.World);

                notRotated = false;
                if (xDelta>0)
                {
                    //cubesParent.transform.Rotate(Vector3.up,-rotX);

                }
                else if(xDelta<0)
                {
                    //cubesParent.transform.Rotate(Vector3.up, rotX);

                }
                notRotatedYet = false;

                previousTouchPos = touchPos;


                //cubesParent.transform.Rotate(transform.up, -Vector2.Dot(posDelta, Camera.main.transform.right), Space.World);
            }


            /*
            if (selectedCube!=null)
            {
                Vector2 touchPos_ = leanFinger.ScreenPosition;
                float rotX = 10f * touchPos_.x * Mathf.Deg2Rad;
                float rotY = 10f * touchPos_.y * Mathf.Deg2Rad;

                cubesParent.transform.Rotate(Vector3.up, -rotX*Time.deltaTime);
                cubesParent.transform.Rotate(Vector3.up, rotY*Time.deltaTime);
            }*/
                



                //cubesParent.transform.Rotate(transform.up,-Vector3.Dot(posDelta,Camera.main.transform.right),Space.World);
                //cubesParent.transform.Rotate(Camera.main.transform.right, Vector3.Dot(posDelta, Camera.main.transform.up), Space.World);




            //}


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
   

    private void GetSpawnedCubes()
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
