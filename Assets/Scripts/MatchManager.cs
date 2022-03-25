using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using DG.Tweening;

public class MatchManager : Singleton<MatchManager>
{
    [SerializeField] private Transform slot1;
    [SerializeField] private GameObject matchParticle;

    private List<Cube> activeCubesInMatchArea;

    private List<Tuple<CubeType, int>> typesAndCurrentCounts;

    //private int currentAmountOfMatches;

    public static Action onMatchOccured;
    private void Start()
    {
        activeCubesInMatchArea = new List<Cube>();
        typesAndCurrentCounts = new List<Tuple<CubeType, int>>();

        Cube[] allTypes = LevelManager.Instance.GetCubeTypesArray();

        for (int i = 0; i < 12; i++)
        {
            Tuple<CubeType,int> tuple = Tuple.Create(allTypes[i].CubeTypeInfo,0);
            typesAndCurrentCounts.Add(tuple);
        }
    }

    public void AddCubeToActiveCubesList(Cube cube)
    {
        Debug.Log("aa");
        activeCubesInMatchArea.Add(cube);

        int index =typesAndCurrentCounts.FindIndex(x=>x.Item1==cube.CubeTypeInfo);

        
        int updatedAmountOfCurrenType = typesAndCurrentCounts[index].Item2+1;
        Tuple<CubeType, int> tuple = Tuple.Create(cube.CubeTypeInfo, updatedAmountOfCurrenType);

        typesAndCurrentCounts[index] = tuple;


        if (activeCubesInMatchArea.Count>=3)
        {

            if (CheckIfMatchExists())
            {

            }
            else if(activeCubesInMatchArea.Count >= 7)
            {
                GameManager.Instance.GameOver();
            }
            
        }
        if (typesAndCurrentCounts.Count == 0)
        {
            GameManager.Instance.Win();

        }
        /*
        if (activeCubesInMatchArea.Count>=7)
        {
            GameManager.Instance.GameOver();
        }
        else if (typesAndCurrentCounts.Count==0)
        {
            GameManager.Instance.Win();
        }
        */

    }
    public void RemoveCubeFromActiveCubesList(Cube cube)
    {
        activeCubesInMatchArea.Remove(cube);
    }

    private bool CheckIfMatchExists()
    {
        for (int i = 0; i < typesAndCurrentCounts.Count; i++)
        {
            if (typesAndCurrentCounts[i].Item2==3)
            {
                typesAndCurrentCounts.RemoveAt(i);
                StartCoroutine(DestroyNewlyMatchedCubesRoutine());
                return true;

            }
        }
        return false;
    }

    private IEnumerator DestroyNewlyMatchedCubesRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        DestroyNewlyMatchedCubes();

    }

    private void DestroyNewlyMatchedCubes()
    {
        Cube lastAddedCube =activeCubesInMatchArea.Last();
        CubeType type_ = lastAddedCube.CubeTypeInfo;
        int[] indexes = new int[3];
        int k = 0;

        Cube[] matchedCubes = new Cube[3];

        for (int i = 0; i < activeCubesInMatchArea.Count; i++)
        {
            if (activeCubesInMatchArea[i].CubeTypeInfo==type_)
            {
                Cube cube = activeCubesInMatchArea[i];
                //cube.SetAsSelected();
                cube.SetAsMatched();
                indexes[k] = i;
                //Destroy(cube.gameObject);
                matchedCubes[k] = cube;
                k = k + 1;

            }
        }

        Sequence seq = DOTween.Sequence();
        Vector3 midPositionOfMatch = matchedCubes[1].transform.position;
        StarManager.Instance.MoveStarToStarIndicator(midPositionOfMatch);

        GameObject matchParticle_ = Instantiate(matchParticle,midPositionOfMatch,Quaternion.identity);


        RemoveNewlyMatchedCubesFromActiveList(matchedCubes);

        StartCoroutine(DestroyGameObjectWithDelay(matchParticle_));
        seq.Join(matchedCubes[0].transform.DOMove(midPositionOfMatch,0.3f));
        seq.Join(matchedCubes[2].transform.DOMove(midPositionOfMatch, 0.3f));

        seq.Play().OnComplete(()=>HandleAfterMatch(indexes,matchedCubes));


  
        onMatchOccured.Invoke();
    }

    private IEnumerator DestroyGameObjectWithDelay(GameObject g)
    {
        yield return new WaitForSeconds(1f);
        Destroy(g);
    }

    private void RemoveNewlyMatchedCubesFromActiveList(Cube[]matchedCubes_)
    {
        for (int i = 0; i < matchedCubes_.Length; i++)
        {
            activeCubesInMatchArea.Remove(matchedCubes_[i]);
        }
    }

    private void HandleAfterMatch(int[]indexes,Cube [] matchedCubes)
    {
      
        /*
        for (int i = 0; i < indexes.Length; i++)
        {
            Cube cube = matchedCubes[i];
            activeCubesInMatchArea.Remove(cube);
            Destroy(matchedCubes[i].gameObject);

        }
        */

        for (int i = 0; i < matchedCubes.Length; i++)
        {
            Destroy(matchedCubes[i].gameObject);
        }
        ShiftUpdatedListAccordingly();
        
    }

    public void HandleCubesThatBackedToBoard()
    {
        for (int i = 0; i < activeCubesInMatchArea.Count; i++)
        {
            if (!activeCubesInMatchArea[i].IsItTakenToMatchArea())
            {
                Cube cube = activeCubesInMatchArea[i];
                activeCubesInMatchArea.RemoveAt(i);
                i = 0;

                
                int index = typesAndCurrentCounts.FindIndex(x => x.Item1 == cube.CubeTypeInfo);

                int updatedAmountOfCurrentType = typesAndCurrentCounts[index].Item2 + -1;
                Tuple<CubeType, int> tuple = Tuple.Create(cube.CubeTypeInfo, updatedAmountOfCurrentType);

                typesAndCurrentCounts[index] = tuple;


            }
        }
        ShiftUpdatedListAccordingly();

    }

    private void ShiftUpdatedListAccordingly()
    {
        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < activeCubesInMatchArea.Count; i++)
        {
            Vector3 posToPlaceX = activeCubesInMatchArea[i].transform.position;


            posToPlaceX.x = slot1.position.x + i * 0.61f;
            seq.Join(activeCubesInMatchArea[i].transform.DOMove(posToPlaceX, 0.3f));

            //activeCubesInMatchArea[i].transform.position = posToPlaceX;

        }

        seq.Play();
    }

}
