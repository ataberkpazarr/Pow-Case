using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] Cube cubePrefab;
    [SerializeField] int xSize,ySize,zSize;

    Cube[,,] cubes;

    public static Action onCubeSpawnCompleted;


    private void Start()
    {
        cubes = new Cube[xSize, ySize, zSize];
        StartCoroutine(spawnCubesRoutine2());

    }

    

    
    private IEnumerator spawnCubesRoutine2()
    {
        yield return new WaitForSeconds(3f);
        GameObject go = new GameObject();
        go.transform.position = new Vector3(0, 0, 0);
        Sequence seq_ = DOTween.Sequence();
        for (int x = 0; x < 4; x++)
        {

            for (int z = 0; z < 4; z++)
            {

                //List<GameObject> instantiatedCubes = new List<GameObject>();
                Sequence seq = DOTween.Sequence();

                for (int y = 0; y < 4; y++)
                {
                    Cube g = Instantiate(cubePrefab, new Vector3(x, y+1, z), Quaternion.identity);
                    g.transform.SetParent(go.transform);
                    seq.Append(g.transform.DOLocalMoveY(y, 0.01f));

                    cubes[x, y, z] = g;

                    //instantiatedCubes.Add(g);
                }

                seq_.Append(seq);




            }


        }
        seq_.Play().OnComplete(()=>onCubeSpawnCompleted?.Invoke());

    }

    public Cube[,,] GetSpawnedCubes()
    {
        return cubes;
    }

    public int[] GetInitialSizes()
    {
        int[] sizes = new int[3];
        sizes[0] = xSize;
        sizes[1] = ySize;
        sizes[2] = zSize;

        return sizes;


    }
}
