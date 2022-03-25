using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private Cube cubePrefab;
    [SerializeField] private Cube [] allCubeTypes;

    /* inside of the allCubeTypes array
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
    */
              
    [SerializeField] int xSize,ySize,zSize;

    Cube[,,] cubes;

    List<Cube> spawnedCubes;

    public static Action onCubeSpawnCompleted;

    System.Random random = new System.Random();

    private void Start()
    {
        spawnedCubes = new List<Cube>();
        cubes = new Cube[xSize, ySize, zSize];

        for (int i = 0; i < 12; i++)
        {
            for (int k = 0; k < 3; k++)
            {
                Cube cube=Instantiate(allCubeTypes[i]);
                spawnedCubes.Add(cube);
                cube.SetItAsNotPlacedInMatchArea();
                cube.gameObject.SetActive(false);
            }
            
        }
        StartCoroutine(spawnCubesRoutine2());



    }

    
    public Cube [] GetCubeTypesArray()
    {
        return allCubeTypes;
    }
    
    private IEnumerator spawnCubesRoutine2()
    {
        yield return new WaitForSeconds(3f);
        GameObject go = new GameObject();
        go.transform.position = new Vector3(0, 0, 0);
        Sequence seq_ = DOTween.Sequence();
        for (int x = 0; x < xSize; x++)
        {

            for (int z = 0; z < zSize; z++)
            {

                //List<GameObject> instantiatedCubes = new List<GameObject>();
                Sequence seq = DOTween.Sequence();

                for (int y = 0; y < ySize; y++)
                {
                    //int num = random.Next(14);
                    //Cube g = Instantiate(allCubeTypes[num], new Vector3(x, y+1, z), Quaternion.identity);
                    //g.transform.SetParent(go.transform);
                    //seq.Append(g.transform.DOLocalMoveY(y, 0.01f));

                    //cubes[x, y, z] = g;

                    int num = random.Next(spawnedCubes.Count);

                    Cube cube = spawnedCubes[num];
                    cube.transform.position = new Vector3(x,y+2,z);
                    cube.transform.rotation = Quaternion.identity;
                    cube.transform.SetParent(go.transform);
                    cube.gameObject.SetActive(true);
                    seq.Append(cube.transform.DOLocalMoveY(y, 0.01f));
                    cubes[x, y, z] = cube;
                    spawnedCubes.Remove(cube);

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
