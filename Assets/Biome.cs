
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public abstract class Biome
{
    internal TerrainType type;
    internal int chunkpos;
    protected Level level;
    protected Chunk chunk;
    protected static float TREE_SCALE = 0.15f;
    protected static float GRASS_SCALE = 0.5f;
    protected Vector3 FENCE_SCALE = new Vector3(0.2f,0.2f,0.2f);
    public int[] obstaclePool;
    protected Biome(TerrainType type, Level level,Chunk chunk)
    {
        
        this.type = type;   
        this.level = level;
        this.chunk = chunk;
        chunkpos = 0;
    }
    
    private static TerrainType selectBiome(int pos)
    {
     //   return TerrainType.ROCKY_MOUNTAIN;
        float noise = Mathf.PerlinNoise((pos + Level.biomeSeed)/20, 0);
   
        if (noise < 0.3)
        {
            return TerrainType.MOUNTAIN;
        }
        else if (noise < 0.4)
        {
            return TerrainType.ROCKY_MOUNTAIN;
        }
        else if (noise < 0.44)
        {
            return TerrainType.ROCKY;
        }
        else if (noise < 0.63)
        {
            return TerrainType.FOREST;
        }
        else
        {
            return TerrainType.PLAIN;
        }
    }

    public static Biome GetBiome(int pos,Level level, Chunk chunk)
    {
        TerrainType b = selectBiome(pos);
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
    public virtual int getObstacle(int gridPos)
    {
        return obstaclePool[Random.Range(0, obstaclePool.Length)];
       // return Random.Range(0, 18);
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
            float z = Random.Range(Level.leftBound, Level.terrainLeftBound);
            // Debug.Log(x);
            spawnFeatureAt(prefab, x, z, size);
        }
        for (int i = 0; i < attempts / 2; i++)
        {
            float x = Random.Range(startPos, startPos + Level.chunkLength);
            float z = Random.Range(Level.terrainRightBound, Level.rightBound);
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
    public PlainBiome(TerrainType type, Level level, Chunk chunk): base(type, level,chunk)
    {
        obstaclePool = new int[] { 0, 0, 0, 1, 1, 1, 5, 6, 7, 12, 14,18,18 };
    }

    public override void populate()
    {
        int flowerCount = Random.Range(0, 6) * 7;
        for (int i = 0; i < 4; i++)
        {
            spawnFeatures(level.bushes[i], chunkpos, 20, GRASS_SCALE);
            spawnFeatures(level.flowers[i], chunkpos, flowerCount, GRASS_SCALE);
        }
    }
    public override int getObstacle(int gridPos)
    {
        
        return base.getObstacle(gridPos);
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

            obstaclePool = new int[] { 12, 13, 14, 15, 16, 17, 12, 13, 14, 15, 16, 17, 5, 6, 7, 9, 10 };
        }
        else //more dead plants
        {
            obstaclePool = new int[] { 12, 13, 14, 15, 16, 17, 9, 10, 9, 10, 5, 6, 7, 9, 10 };
        }
    }


    public override void populate()
    {
        for(int i = 0; i < 4; i++)
        {
          //  spawnFeatures(level.rocks[i], chunkpos, 10, TREE_SCALE);
            spawnFeatures(level.dead_plants[Random.Range(0,4)], chunkpos, Random.Range(0, 10)-3, TREE_SCALE);
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


        //pine trees
        if (chunk.decorationNoise > 0.40f && chunk.decorationNoise < 0.60f)
        {

            obstaclePool = new int[] {0,1,3,3,3,4,4,4,5, 6,12,14, 18, 18 };
        }
        else //normal trees
        {
            obstaclePool = new int[] { 0, 1, 2,2,2,2, 5, 6, 12, 14, 18, 18 };
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
        //pine trees
        if (chunk.decorationNoise > 0.5f)
        {

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
        //more rocks
        if (chunk.decorationNoise > 0.5f)
        {

            obstaclePool = new int[] { 12, 13, 14, 15, 16, 17, 12, 13, 14, 15, 16, 17, 5, 6, 7, 9, 10 };
        }
        else //more dead plants
        {
            obstaclePool = new int[] { 12, 13, 14, 15, 16, 17, 9, 10, 9, 10, 5, 6, 7, 9, 10 };
        }
    }


    public override void populate()
    {
       // spawnFeatures(level.light_plants[0], chunkpos, 4, GRASS_SCALE);
        for (int i = 0; i < 3; i++)
        {
            //  spawnFeatures(level.rocks[i], chunkpos, 10, TREE_SCALE);
            spawnFeatures(level.dead_plants[Random.Range(0, 4)], chunkpos, Random.Range(0, 6) - 3, TREE_SCALE);
            //spawnFeatures(level.light_plants[Random.Range(1, 3)], chunkpos, 4, GRASS_SCALE);

        }
         spawnFeatures(level.dead_plants[4], chunkpos, Random.Range(0, 40), GRASS_SCALE);
    }
}

