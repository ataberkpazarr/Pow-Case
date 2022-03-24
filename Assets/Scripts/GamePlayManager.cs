using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using DG.Tweening;
public class GamePlayManager : Singleton<GamePlayManager>
{

    [SerializeField] private Transform slot1, slot2, slot3;
    private bool TouchActive = false;
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
        previouslySelectedCube.SetAsReleased();
        previouslySelectedCube=null;
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
                    selectedCube.gameObject.transform.DOScale(new Vector3(0.6f, 0.6f, 0.6f), 0.5f);
                    selectedCube.gameObject.layer = LayerMask.NameToLayer("UI");
                    selectedCube.transform.parent = null;
                    selectedCube.transform.rotation = Quaternion.identity;
                    selectedCube.SetItAsPlaced();
                    placedCubes.Add(selectedCube);

                }
            }

            
        }
        notRotated = true;

    }

    private void OnFingerUpdate(LeanFinger leanFinger)
    {

        if (TouchActive)
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
                    Debug.Log(pos);
                   
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

    }

    private void OnDisable()
    {
        LeanTouch.OnFingerDown -= OnFingerDown;
        LeanTouch.OnFingerUpdate -= OnFingerUpdate;
        LeanTouch.OnFingerUp -= OnFingerUp;
        LevelManager.onCubeSpawnCompleted -= GetSpawnedCubes;


    }
}
