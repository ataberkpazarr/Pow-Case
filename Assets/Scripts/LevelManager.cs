using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class LevelManager : Singleton<LevelManager>
{

    [SerializeField] private Cube [] allCubeTypes;
    [SerializeField] private GameObject smokeParticle;


    /* inside of the allCubeTypes array, the order of types are as following
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

        //firstly 3 cubes from all types are being spawned at set to deactive

        for (int i = 0; i < 12; i++) // we have 14 types but I used 12 for ease of development and for having total 3k amount of cubes in board, So I hve 36 cubes in the board
        {
            for (int k = 0; k < 3; k++)
            {
                Cube cube=Instantiate(allCubeTypes[i]);
                spawnedCubes.Add(cube);
                cube.SetItAsNotPlacedInMatchArea();
                cube.gameObject.SetActive(false);
            }
            
        }
        //then after initiliazation of all cubes the relevant coroutine, for placing them randomly, is being called
        StartCoroutine(SpawnCubesRoutine());

    }

    
    public Cube [] GetCubeTypesArray()
    {
        return allCubeTypes;
    }
    
    private IEnumerator SpawnCubesRoutine() //placing cubes randomly and required tweens for fall effect
    {
        yield return new WaitForSeconds(0.2f);

        GameObject go = new GameObject(); // parent gameobject for cubes
        go.transform.position = new Vector3(0, 0, 0);

        Sequence seq_ = DOTween.Sequence(); //fall tween sequence

        for (int x = 0; x < xSize; x++)
        {

            for (int z = 0; z < zSize; z++)
            {
                Sequence seq = DOTween.Sequence();

                for (int y = 0; y < ySize; y++)
                {

                    int num = random.Next(spawnedCubes.Count); //randomly locate already spawned cube at the relevant x,y,z position of board
                    Cube cube = spawnedCubes[num];

                    cube.transform.position = new Vector3(x,y+2,z); // instantiate 2 unit above from actual position, in order to fall it
                    cube.transform.rotation = Quaternion.identity;

                    cube.transform.SetParent(go.transform);
                    cube.gameObject.SetActive(true);

                    seq.Append(cube.transform.DOLocalMoveY(y, 0.01f).OnComplete(()=>SpawnSmoke(new Vector3(x,y,z)))); // fall to actual position

                    cubes[x, y, z] = cube;
                    spawnedCubes.Remove(cube); //remove placed cubes from spawnedCubes list

                }

                seq_.Append(seq);

            }


        }
        //seq_.Join(SpawnSmokes());
        //when sequence is done, let other classes that cube spawn opearation is being done
        seq_.Play().OnComplete(() => onCubeSpawnCompleted?.Invoke());


    }

    private void SpawnSmoke(Vector3 pos)
    {
        int x = random.Next(0,5);
        int y = random.Next(0,5);
        int z = random.Next(0,5);
        Instantiate(smokeParticle, pos+new Vector3(-x,-y,-z), Quaternion.identity);
       
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
