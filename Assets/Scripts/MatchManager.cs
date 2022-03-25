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

    //types and current counts list, all 12 types in the game and their current amount in match area accordingly like, carrot,2 or blueberry,0 etc.
    //when new cube will be placed to match area, then that type's count will be increased, when cube will be backed to board, that type's count will be decreased
    //when a match occures, relevant type and currrent count tuple will be removed from the typesAndCurrentCounts list
    //so that if this list will reached to 0 length, then it means that player end up with all cubes and passed level
    private List<Tuple<CubeType, int>> typesAndCurrentCounts; 



    public static Action onMatchOccured;

    private void Start()
    {
        activeCubesInMatchArea = new List<Cube>();
        typesAndCurrentCounts = new List<Tuple<CubeType, int>>();

        Cube[] allTypes = LevelManager.Instance.GetCubeTypesArray();

        //types and currentCount newly created and all counts set to 0 like, blueberry 0, cupcake 0, etc.
        for (int i = 0; i < 12; i++) 
        {
            Tuple<CubeType,int> tuple = Tuple.Create(allTypes[i].CubeTypeInfo,0); 
            typesAndCurrentCounts.Add(tuple);
        }
    }

    public void AddCubeToActiveCubesList(Cube cube)
    {

        activeCubesInMatchArea.Add(cube);

        //update total amount of newly added cube type

        int index =typesAndCurrentCounts.FindIndex(x=>x.Item1==cube.CubeTypeInfo);
        int updatedAmountOfCurrenType = typesAndCurrentCounts[index].Item2+1;

        Tuple<CubeType, int> tuple = Tuple.Create(cube.CubeTypeInfo, updatedAmountOfCurrenType);
        typesAndCurrentCounts[index] = tuple;


        if (activeCubesInMatchArea.Count>=3) // if there exists more than 3 cubes in match area, than check if match occured or not
        {

            if (CheckIfMatchExists())
            {

            }
            else if(activeCubesInMatchArea.Count >= 7) //gameover
            {
                GameManager.Instance.GameOver();
            }
            
        }
        if (typesAndCurrentCounts.Count == 0) //win
        {
            GameManager.Instance.Win();

        }

    }
    /*
    public void RemoveCubeFromActiveCubesList(Cube cube)
    {
        activeCubesInMatchArea.Remove(cube);
    }*/

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
        //yield return new WaitForSeconds(0.5f);

        yield return null;
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
                
                cube.SetAsMatched();
                indexes[k] = i;
                
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

        }

        seq.Play();
    }

}
