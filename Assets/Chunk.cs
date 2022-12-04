using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GridEntry
{
    EMPTY=0,OBSTACLE=1,LONG_OBSTACLE=2,ENEMY=3
}

public class Chunk
{
    internal int pos;
    internal Biome biome;
    private List<GameObject> objects = new();
    private GameObject ground;
    internal float biomeNoise;
    internal float decorationNoise;
    internal GridEntry[,] grid;
    private Level level;
    internal float[] obstacleScales;
    public Chunk(int pos, Level level)
    {
        biomeNoise = Mathf.PerlinNoise((pos + Level.biomeSeed) / 20, 0);
        decorationNoise = Mathf.PerlinNoise((pos + Level.decorationSeed) / 10, 0);
        this.pos = pos;
        biome = Biome.GetBiome(pos,level,this);
        biome.chunkpos = pos;
        this.level = level;
        initializeGrid();
        
    }
    void initializeGrid()
    {

        grid = new GridEntry[12, 5];
        for (int i=0;i<12;i++)
        {
            for(int j = 0; j < 5; j++)
            {
                grid[i,j] = GridEntry.EMPTY;
            }
        }
        grid[1, 3] = GridEntry.OBSTACLE;
        grid[3, 1] = GridEntry.OBSTACLE;
        grid[6, 4] = GridEntry.OBSTACLE;
        grid[9, 2] = GridEntry.OBSTACLE;
        grid[7, 0] = GridEntry.OBSTACLE;
        grid[4, 2] = GridEntry.OBSTACLE;
        grid[2, 4] = GridEntry.OBSTACLE;
        grid[11, 0] = GridEntry.OBSTACLE;
    }
    Vector3 getCoord(int x, int y)
    {
        return new Vector3(pos * Level.chunkLength + (((float)x -0.5f) / 12.0f) * Level.chunkLength,Level.bottomY, level.laneCoordinates[y]);
    }
    void addObstacles()
    {
        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (grid[i,j] == GridEntry.OBSTACLE)
                {
                    Quaternion rot = Quaternion.Euler(0, Random.Range(0, 180), 0);
                    int obs = biome.getObstacle(i);
                    //fence
                    //Debug.Log(obs);
                    if(obs == 18 || obs == 19)
                    {
                     //   Debug.Log("fence");
                        rot = Quaternion.Euler(0, 90, 0);
                    }
                    placeObject(level.obsatcles[obs], getCoord(i, j), rot);
                }
            }
        }
    }
    public GameObject placeObject(GameObject prefab, Vector3 pos,Quaternion rot)
    {
        GameObject feature = Level.InstantiateObj(prefab, pos, rot);
    //    objects.Add(feature);
        feature.transform.parent = ground.transform;
        return feature;
    }
    public void OnDestroy()
    {
        foreach(GameObject obj in objects)
        {
            Level.DestroyObj(obj);
        }
        Level.DestroyObj(ground);
    }
    public void generate()
    {
        Debug.Log(biome.getTerrainObject());
        float startPos = pos * Level.chunkLength;
        ground = Level.InstantiateObj(biome.getTerrainObject(), new Vector3(0, 0, 0), Quaternion.identity);

        ground.AddComponent<BoxCollider>();
        ground.GetComponent<BoxCollider>().size = new Vector3(1.0f, 1.0f, 1.0f);
        ground.transform.position = new Vector3(startPos, Random.Range(0.001f, 0.0f), 0);
        biome.addFence(startPos);
        addObstacles();
    }
    public void populate()
    {
        biome.populate();
    }

}
