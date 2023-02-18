
using System.Collections.Generic;
using UnityEngine;


public abstract class Biome
{
    internal TerrainType type;
    internal int chunkpos;
    protected Level level;
    protected Chunk chunk;
    protected static float TREE_SCALE = 0.15f;
    protected static float GRASS_SCALE = 0.5f;
    protected Vector3 FENCE_SCALE = new Vector3(0.2f,0.2f,0.2f);
    internal int[] obstaclePool;
    internal int[] longObstacles_2=new int[3];
    internal int[] longObstacles_3 = new int[3];

    internal int[] fixedObstacle = new int[6];
    internal float longObstacleChance = 0.5f;
    internal bool generateBear=true;
    internal bool generateEagle = true;
    internal float beeFrequency = 0.2f;
    protected Biome(TerrainType type, Level level,Chunk chunk)
    {
        
        this.type = type;   
        this.level = level;
        this.chunk = chunk;
        chunkpos = 0;
    }
    
    private static TerrainType selectBiome(int pos,int type)
    {
     //   return TerrainType.ROCKY_MOUNTAIN;
        float noise = Mathf.PerlinNoise((pos + Level.biomeSeed)/15, 0); 
        if (type == 0) return TerrainType.PLAIN;
        if (type == 1)
        {
            if (noise < 0.5)
            {
                return TerrainType.FOREST;
            }
            else
            {
                return TerrainType.MOUNTAIN;
            }
        }
        if (type == 2)
        {
            if (noise < 0.5)
            {
                return TerrainType.ROCKY;
            }
            else
            {
                return TerrainType.ROCKY_MOUNTAIN;
            }
        }
        if (type == 3) return TerrainType.SNOWY;
        if (type == 4) return TerrainType.DARK;
        if (type == 5) return TerrainType.DESERT;
        if (type == 6) return TerrainType.RIVER;

        TerrainType biome;
        if (noise< 0.27)
        {
            biome= TerrainType.SNOWY;
        }
        else if (noise < 0.35)
        {
            biome = TerrainType.MOUNTAIN;
        }
        else if (noise < 0.4)
        {
            biome = TerrainType.ROCKY_MOUNTAIN;
        }
        else if (noise < 0.45)
        {
            biome = TerrainType.ROCKY;
        }
        else if (noise < 0.55)
        {
            biome = TerrainType.FOREST;
        }
        else if (noise < 0.7)
        {
            biome = TerrainType.PLAIN;
        }
        else
        {
            biome = TerrainType.DESERT;
        }

        if(biome!= TerrainType.DESERT && biome!= TerrainType.SNOWY && Random.Range(0,9)==0)
            biome= TerrainType.RIVER;
        return biome;

    }

    public static Biome GetBiome(int pos,Level level, Chunk chunk,int biome)
    {


        TerrainType b = selectBiome(pos, biome);


        switch (b)
        {
            case TerrainType.PLAIN:
                return new PlainBiome(b,level, chunk);
            case TerrainType.ROCKY:
                return new RockBiome(b,level, chunk);
            case TerrainType.FOREST: 
                return new ForestBiome(b,level, chunk);
            case TerrainType.MOUNTAIN:
                return new MountainBiome(b, level, chunk);
            case TerrainType.ROCKY_MOUNTAIN:
                return new RockyMountainBiome(b, level, chunk);
            case TerrainType.SNOWY:
                return new SnowyBiome(b, level, chunk);
            case TerrainType.DARK:
                return new DarkBiome(b, level, chunk);
            case TerrainType.DESERT:
                return new DesertBiome(b, level, chunk);
            case TerrainType.RIVER:
                if(chunk.isEmpty) return new PlainBiome(TerrainType.PLAIN, level, chunk);
                return new RiverBiome(b, level, chunk);
        }
        return new PlainBiome(b, level, chunk);
    }
    public GameObject getTerrainObject()
    {
        return level.terrains[(int)type];
    }
    protected virtual GameObject getFenceObject()
    {
        return level.fences[Random.Range(0,2)];
    }
    protected virtual float getFenceInterval()
    {
        return Random.Range(0.6f, 1.5f);
    }
    public virtual int getObstacle()
    {
        return obstaclePool[Random.Range(0, obstaclePool.Length)];
       // return Random.Range(0, 18);
    }
    public int getRandomFixedObstacle(int type)
    {
        return fixedObstacle[type*2+Random.Range(0,2)];
    }
    public int getFixedObstacle(int index)
    {
        return fixedObstacle[index];
    }


    public virtual int getLongObstacle(int length)
    {
        if (length == 2) return longObstacles_2[Random.Range(0, longObstacles_2.Length)];
        else return longObstacles_3[Random.Range(0, longObstacles_3.Length)];

    }
    public virtual int getBridge(int length)
    {
        return -1;
    }
    public virtual GameObject getInlaneDecoration()
    {
        return null;
    }
    public virtual void populate()
    {

        spawnFeatures(level.trees[0], chunkpos, 10, TREE_SCALE);
        spawnFeatures(level.bushes[0], chunkpos, 20, GRASS_SCALE);

    }
    protected void spawnFeatures(GameObject prefab, int chunkPos, int attempts, float size)
    {
        float startPos = chunkPos * Level.chunkLength - 1;
        for (int i = 0; i < attempts / 2; i++)
        {

            float x = Random.Range(startPos, startPos + Level.chunkLength);
            float z = Random.Range(Level.leftBound+0.15f, Level.terrainLeftBound);
            // Debug.Log(x);
            spawnFeatureAt(prefab, x, z, size);
        }
        for (int i = 0; i < attempts / 2; i++)
        {
            float x = Random.Range(startPos, startPos + Level.chunkLength);
            float z = Random.Range(Level.terrainRightBound, Level.rightBound-0.15f);
            spawnFeatureAt(prefab, x, z, size);
        }
    }
    protected void spawnFeatureAt(GameObject prefab, float x, float z, float size)
    {
       // Debug.Log("raycast");
        RaycastHit hit;
        Vector3 pos = new Vector3(x, 5, z);
        if (Physics.Raycast(pos, Vector3.down, out hit))
        {
           // Debug.Log("hit");
            if (!hit.collider.gameObject.CompareTag("feature"))
            {

                Quaternion rot = Quaternion.Euler(0, Random.Range(0, 180), 0);
                GameObject feature = chunk.placeObject(prefab, hit.point, rot);

                //feature.transform.position = hit.point;
                size *= Random.Range(0.9f,1.1f);
                feature.transform.localScale = new Vector3(size, size, size);
                feature.tag = "feature";
            }
        }
      //  else Debug.Log("raycast fail");
    }
    protected void addFenceAt(GameObject prefab,Vector3 pos,Vector3 scale)
    {
        GameObject fence = chunk.placeObject(prefab, pos, Quaternion.identity);
        fence.transform.localScale = scale;
    }
    public virtual void addFence(float startPos)
    {
        if (type == TerrainType.RIVER) return;

        float fencePos = startPos - 2;
        while (fencePos < startPos + Level.chunkLength / 2)
        {
            fencePos += getFenceInterval();
            addFenceAt(getFenceObject(), new Vector3(fencePos, Level.bottomY, Level.rightBound-0.1f), FENCE_SCALE);
        }
        fencePos = startPos - 2;
        while (fencePos < startPos + Level.chunkLength / 2)
        {
            fencePos += getFenceInterval();
            addFenceAt(getFenceObject(), new Vector3(fencePos, Level.bottomY, Level.leftBound+0.1f), FENCE_SCALE);
        }
    }
}

class PlainBiome : Biome
{
    int flowerCount;
    public PlainBiome(TerrainType type, Level level, Chunk chunk): base(type, level,chunk)
    {
        obstaclePool = new int[] { 0, 0, 0, 1, 1, 1, 5, 6, 7, 12, 14,18,18 };
        longObstacles_2 = new int[4] {0,1,2,9 };
        longObstacles_3 = new int[3] {4,5,6};
        fixedObstacle=new int[6] { 0,5,0,2,4,5};
        flowerCount = Random.Range(0, 6) * 7;
        if (flowerCount > 25)
        {
            beeFrequency = 0.6f;
        }
        else beeFrequency = 0.3f ;
    }

    public override void populate()
    {
        
        for (int i = 0; i < 4; i++)
        {
            spawnFeatures(level.bushes[i], chunkpos, 20, GRASS_SCALE);
            spawnFeatures(level.flowers[i], chunkpos, flowerCount, GRASS_SCALE);
        }
    }

}
class RockBiome : Biome
{
    public RockBiome(TerrainType type, Level level, Chunk chunk) : base(type, level, chunk)
    {
        FENCE_SCALE = new Vector3(0.26f, 0.26f, 0.12f);
        //more rocks
        if (chunk.decorationNoise > 0.5f)
        {
            obstaclePool = new int[] {11, 12, 13, 14, 15, 16, 17, 11, 12, 13, 14, 15, 16, 17, 5, 6, 7, 9, 10 };
        }
        else //more dead plants
        {
            obstaclePool = new int[] { 11, 12, 14, 15, 9, 10, 9, 10, 11, 9, 10, 11, 9, 10, 11, 5, 6, 7, 9, 10 };
        }
        longObstacles_2 = new int[] { 2,3,3 };
        longObstacles_3 = new int[] { 6,7,7 };
        fixedObstacle = new int[6] {12,11, 2,3,6,7 };
        beeFrequency = 0.0f;
    }


    public override void populate()
    {
        for(int i = 0; i < 4; i++)
        {
            int treeCount = Random.Range(5, 10);
            if(chunk.decorationNoise > 0.5f)
            {
                treeCount = Random.Range(0, 6) - 3;
            }
          //  spawnFeatures(level.rocks[i], chunkpos, 10, TREE_SCALE);
            spawnFeatures(level.dead_plants[Random.Range(0,4)], chunkpos, treeCount, TREE_SCALE);
        }
        spawnFeatures(level.dead_plants[4], chunkpos, Random.Range(0, 70), GRASS_SCALE);

    }
    protected override GameObject getFenceObject()
    {
        return level.fences[Random.Range(3, 6)];
    }
    protected override float getFenceInterval()
    {
        return Random.Range(0.05f, 0.15f);
    }
}
class ForestBiome : Biome
{
    public ForestBiome(TerrainType type, Level level, Chunk chunk) : base(type, level, chunk)
    {
        longObstacles_2 = new int[4] { 0, 1, 2,9 };
        longObstacles_3 = new int[3] { 4, 5, 6 };
        fixedObstacle = new int[6] { 2,4, 0, 2, 5,6 };
        //pine trees
        if (chunk.decorationNoise > 0.40f && chunk.decorationNoise < 0.60f)
        {
            beeFrequency = 0.1f;
            obstaclePool = new int[] {0,1,3,3,3,4,4,4,5, 6,12,14, 18, 18 };
        }
        else //normal trees
        {
            obstaclePool = new int[] { 0, 1, 2,2,2,2, 5, 6, 12, 14, 18, 18 };
            if (chunk.decorationNoise >= 0.60f)
            {
                beeFrequency = 0.3f;
            }
        }
        
    }

    protected override GameObject getFenceObject()
    {
        return level.fences[2];
    }
    public override void populate()
    {
        spawnFeatures(level.bushes[0], chunkpos, 10, GRASS_SCALE);
        spawnFeatures(level.bushes[1], chunkpos, 10, GRASS_SCALE);
        float vegetationNoise = chunk.decorationNoise;
        int treeCount= Random.Range(0, 6) + 6;
        GameObject[] trees;
        float scale = TREE_SCALE;
        if (vegetationNoise < 0.40f)
        {
            trees = level.trees;
        }
        else if (vegetationNoise < 0.60f)
        {
            trees = level.pine_trees;
            scale = 0.3f;
        }
        else
        {
            trees = level.colored_trees;
        }
        for (int i = 0; i < 4; i++)
        {
            spawnFeatures(trees[i], chunkpos, treeCount, scale);
        }
    }
}

class MountainBiome : Biome
{
    public MountainBiome(TerrainType type, Level level, Chunk chunk) : base(type, level, chunk)
    {
        longObstacles_2 = new int[3] { 0, 2, 2 };
        longObstacles_3 = new int[3] { 4, 5, 6 };
        fixedObstacle = new int[6] { 4, 6, 0, 2, 4,5 };
        //pine trees
        if (chunk.decorationNoise > 0.5f)
        {
            beeFrequency = 0.1f;
            obstaclePool = new int[] { 0, 0, 1, 1, 3, 3, 3, 4, 4, 4, 5, 6, 7, 9, 10, 12, 14 };
        }
        else //normal trees
        {
            obstaclePool = new int[] { 0, 0, 1, 1, 2, 2, 2, 2, 5, 6, 7, 9, 10, 12, 14 };
        }
    }


    public override void populate()
    {
        bool isPineTree = chunk.decorationNoise > 0.5f;

        for(int i=0;i<3;i++)
        {
            if(isPineTree)
            {
                spawnFeatures(level.pine_trees[0], chunkpos, Random.Range(1, 4), TREE_SCALE);
            }
            else
            {
                spawnFeatures(level.trees[0], chunkpos, Random.Range(1, 4), TREE_SCALE);
            }
        }
        spawnFeatures(level.bushes[0], chunkpos, 20, GRASS_SCALE);
    }
}

class RockyMountainBiome : Biome
{
    public RockyMountainBiome(TerrainType type, Level level, Chunk chunk) : base(type, level, chunk)
    {
        fixedObstacle = new int[6] { 13,10, 2, 3, 6, 7 };
        longObstacles_2 = new int[] { 2, 3, 3 };
        longObstacles_3 = new int[] { 6, 7, 7 };
        //more rocks
        if (chunk.decorationNoise > 0.5f)
        {

            obstaclePool = new int[] { 11, 12, 13, 14, 15, 16, 17, 12, 13, 14, 15, 16, 17, 5, 6, 7, 9, 10 };
        }
        else //more dead plants
        {
            obstaclePool = new int[] { 11, 12, 14, 15, 9,10,11,9,10, 11, 9,10, 11, 5, 6, 7, 9, 10, 11 };
        }
        beeFrequency = 0.0f;
    }


    public override void populate()
    {
       // spawnFeatures(level.light_plants[0], chunkpos, 4, GRASS_SCALE);
        for (int i = 0; i < 3; i++)
        {
            int treeCount = Random.Range(5, 10);
            if (chunk.decorationNoise > 0.5f)
            {
                treeCount = Random.Range(0, 4) - 3;
            }
            //  spawnFeatures(level.rocks[i], chunkpos, 10, TREE_SCALE);
            spawnFeatures(level.dead_plants[Random.Range(0, 4)], chunkpos, treeCount, TREE_SCALE);
            //spawnFeatures(level.light_plants[Random.Range(1, 3)], chunkpos, 4, GRASS_SCALE);

        }
         spawnFeatures(level.dead_plants[4], chunkpos, Random.Range(0, 40), GRASS_SCALE);
    }
}


class SnowyBiome : Biome
{
    public SnowyBiome(TerrainType type, Level level, Chunk chunk) : base(type, level, chunk)
    {
        longObstacles_2 = new int[4] { 0, 3, 8,8 };
        longObstacles_3 = new int[2] { 6,7};
        fixedObstacle = new int[6] { 4, 6, 0, 2, 4, 5 };
        if (chunk.decorationNoise > 0.5f)
        {
            obstaclePool = new int[] { 21, 21, 12, 14, 22, 22, 22, 10, 21 ,22,21};
        }
        else
        {
            obstaclePool = new int[] { 11,11, 12, 14, 20,  20,  20, 10, 11,11,20 };//only dead trees
        }
            
        longObstacleChance = 0.3f;
        beeFrequency = 0.0f;
    }


    public override void populate()
    {
        for (int i = 0; i < 3; i++)
        {
            if (chunk.decorationNoise > 0.5f)
            {
                spawnFeatures(level.snowy_trees[Random.Range(0, 2)], chunkpos, Random.Range(10, 15), TREE_SCALE);

            }
            else
            {
                spawnFeatures(level.snowy_trees[Random.Range(2, 4)], chunkpos, Random.Range(10, 15), TREE_SCALE);

            }
        }
        spawnFeatures(level.dead_plants[4], chunkpos, 20, GRASS_SCALE);
    }
}
class DarkBiome : Biome
{
    public DarkBiome(TerrainType type, Level level, Chunk chunk) : base(type, level, chunk)
    {
        longObstacles_2 = new int[2] { 2, 1 };
        longObstacles_3 = new int[2] {5,4};
        fixedObstacle = new int[6] { 4, 6, 0, 2, 4, 5 };
        if (chunk.decorationNoise > 0.5f)
        {
            obstaclePool = new int[] { 23, 24, 30, 30, 31, 31, 31, 31, 29, 29, 24, 30, 31, 29, 29,32,33,34, 5, 7, 6 };
        }
        else //more mushroom
        {
            obstaclePool = new int[] {30,  29,  31, 32, 33, 34, 32, 33, 34, 32, 33, 34, 5, 7, 7,6 };
        }

        longObstacleChance = 0.1f;
        generateBear = false;
        generateEagle=false;
    }

    public override void populate()
    {
        for (int i = 0; i < 3; i++)
        {
            spawnFeatures(level.tall_grass[i], chunkpos, 5, GRASS_SCALE + 0.3f);
        }
        //spawnFeatures(level.light_plants[0], chunkpos, 7, GRASS_SCALE);
        for(int i = 0; i < 2; i++)
        {
            spawnFeatures(level.light_plants[i+1], chunkpos, 15, GRASS_SCALE);
        }
    }
}
class DesertBiome : Biome
{
    public DesertBiome(TerrainType type, Level level, Chunk chunk) : base(type, level, chunk)
    {
        fixedObstacle = new int[6] { 4, 6, 0, 2, 4, 5 };
        obstaclePool = new int[] { 25,26,27,28,9,10,26,26,26,26,26,27,27};
        longObstacleChance = 0.0f;
        generateBear= false;
        beeFrequency= 0.0f;
    }

    protected override GameObject getFenceObject()
    {
        return level.fences[Random.Range(6, 8)];
    }
    public override void populate()
    {
        for (int i = 0; i < 3; i++)
        {
            spawnFeatures(level.desert_plants[i], chunkpos, 15, TREE_SCALE);
        }
        spawnFeatures(level.desert_plants[3], chunkpos, 5, TREE_SCALE);
    }
}
class RiverBiome : Biome
{
    public RiverBiome(TerrainType type, Level level, Chunk chunk) : base(type, level, chunk)
    {
        fixedObstacle = new int[6] { 4, 6, 0, 2, 4, 5 };
        obstaclePool = new int[] {35 };
        longObstacleChance = 0.0f;
        generateBear = false;
        generateEagle=false;
        beeFrequency = 0.4f;
    }

    public override void populate()
    {
        for (int i = 0; i < 3; i++)
        {
            spawnFeatures(level.trees[0], chunkpos, Random.Range(1, 4), TREE_SCALE);
        }
    
        spawnFeatures(level.bushes[0], chunkpos, 20, GRASS_SCALE);
    }
    public override int getBridge(int length)
    {
        if (length == 1)
        {
            return Random.Range(0, 3);
        }
        if (length == 2)
        {
            return Random.Range(0, 5) == 0 ? 4 : 3;
        }
        return 5;
    }
    public override GameObject getInlaneDecoration()
    {
        int rand = Random.Range(0, 15);
        if (rand == 0) return level.river_plants[3];
        else if (rand <=2) return level.river_plants[2];
        else if (rand <=5) return level.river_plants[1];
        else if (rand <=8) return level.river_plants[0];
        return null;
    }
}