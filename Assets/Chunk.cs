using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum GridEntry
{
    EMPTY=0,OBSTACLE=1,LONG_OBSTACLE=2,ENEMY=3
}
public enum GridInit
{
    NONE=0, //obstacle can be placed here
    GEM=1,  //gems will be placed
    GEM2 = 2,  //gems will be placed
    GEM_SPECIAL = 3,
    EMPTY =4, //no obstacles or gems will be placed here
    OBSTACLE=5, //obstacle will be placed here
    OBSTACLES_2LONG = 6,
    OBSTACLES_3LONG = 7,
    OBSTACLES_2WIDE = 8,
    OBSTACLES_3WIDE = 9,
    LONG_OBSTACLE_SPACE=10,
    FIXED_OBSTACLE=11,
    KEY_FOOD=12,
    OTHER_FOOD=13,
    ENEMY=14,
    ENEMY_PATH=15,
    PREY=16,
    EXTRA_BRIDGE=17,
    BRIDGE_PLACED = 18,
}
public class Chunk
{
    internal int pos;
    internal Biome biome;
    private List<GameObject> objects = new();
    private GameObject ground;
    internal float biomeNoise;
    internal float decorationNoise;
    private Level level;
    internal float[] obstacleScales;
    private Chunk lastChunk;
    internal List<int> pathEnds;
    internal bool isEmpty =false;
    internal ObstacleGenerator obsgen;
    internal int food = -1;


    
    public Chunk(int pos, Level level, Chunk lastChunk, bool isEmpty,int biome)
    {
        this.isEmpty= isEmpty;
        this.lastChunk= lastChunk;
        biomeNoise = Mathf.PerlinNoise((pos + Level.biomeSeed) / 20, 0);
        decorationNoise = Mathf.PerlinNoise((pos + Level.decorationSeed) / 10, 0);
        this.pos = pos;
        this.biome = Biome.GetBiome(pos,level,this, biome);
        Debug.Log(this.biome);
        this.biome.chunkpos = pos;
        this.level = level;

        //difficulty 0~1 : 0%, difficulty 5: 50%, difficulty 9: 100%

    }

    Vector3 getCoord(int x, int lane)
    {
        return new Vector3(pos * Level.chunkLength + 0.5f*x + 0.25f-2.25f,Level.bottomY, Level.laneCoordinates[lane]);
    }
    Vector3 getItemCoord(int x, int lane)
    {
        return new Vector3(pos * Level.chunkLength + 0.5f * x + 0.25f - 2.25f, 1.5f, Level.laneCoordinates[lane]);
    }//(((float)x - 4.0f) / 12.0f) * Level.chunkLength
    public void spawnEnemy(float playerSpeed)
    {
        if(isEmpty) return;
        foreach (int[] coord in obsgen.bears)
        {
            if (coord[1]==4 || (Random.Range(0, 2) == 0 && coord[1] != 0))
            {
                Vector3 pos = getCoord(coord[0], coord[1]);
                GameObject b = placeObject(level.bear, new Vector3(pos.x, pos.y, Level.laneCoordinates[0] + Level.laneWidth * 0.5f), Quaternion.Euler(0, 180, 0));
                level.Coroutine(moveEnemy(b, (12.5f + coord[0] * 0.2f )/ playerSpeed , pos,(int)Direction.RIGHT ,playerSpeed));
            }
            else
            {
                Vector3 pos = getCoord(coord[0], coord[1]);
                GameObject b = placeObject(level.bear, new Vector3(pos.x, pos.y, Level.laneCoordinates[4] - Level.laneWidth * 0.5f), Quaternion.Euler(0, 0, 0));
                level.Coroutine(moveEnemy(b, (11.5f + coord[0] * 0.2f )/ playerSpeed , pos, (int)Direction.LEFT, playerSpeed));
            }
        }
        foreach (int[] coord in obsgen.eagles)
        {
            Vector3 goalPos = getCoord(coord[0], coord[1]);
            Vector3 pos = new Vector3(Random.Range(Level.chunkLength * (this.pos-2), Level.chunkLength * (this.pos - 1)), 1.8f
                , Random.Range(-5,5));
            Quaternion rot = Quaternion.Euler(0, 90, 0);
            if (Random.Range(0, 2) == 0)
            {
                rot = Quaternion.Euler(0, -90, 0);
                pos.x = Random.Range(Level.chunkLength * (this.pos+1), Level.chunkLength * (this.pos + 2));
            }
            GameObject eagle = placeObject(level.eagle, pos, rot);
            level.Coroutine(moveFlyingEnemy(eagle, (7f + coord[0] * 0.4f) / playerSpeed, 5.5f/playerSpeed, goalPos));
        }
        foreach (int[] coord in obsgen.bees)
        {
            spawnBeesAt(coord[0], coord[1]);
        }
        foreach (int[] coord in obsgen.preys)
        {
            Vector3 pos = getCoord(coord[0], coord[1]);
            
            Quaternion rot = Quaternion.Euler(0, 90, 0);
            string type = "chicken";

            GameObject obj = level.chickenPrey;
            if(Random.Range(0, 2) == 0)
            {
                type = "rabbit";
                obj = level.rabbitPrey;
                if (biome.type == TerrainType.DESERT) obj = level.desertRabbitPrey;
            }
            GameObject prey = level.InstantiateObj(obj, pos, rot);
            level.addPrey(prey);
            if (type == "chicken")
            {
                prey.GetComponent<ChickenPrey>().init(level.maskChunk(obsgen.grid), pathEnds[Random.Range(0, pathEnds.Count - 1)], this.pos, coord[0], coord[1]);

            }
            else if (type == "rabbit")
                prey.GetComponent<RabbitPrey>().init(level.maskChunk(obsgen.grid), pathEnds[Random.Range(0, pathEnds.Count - 1)], this.pos, coord[0], coord[1]);

        }
    }
    void spawnBeesAt(int x,int z)
    {
        Vector3 goalPos = getCoord(x, z);
        obsgen.Shuffle(ref Level.beeDiffs);
        for(int i = 0; i < Random.Range(1, 3); i++)
        {
            spawnBee(goalPos + Level.beeDiffs[i]);
        }
    }
    void spawnBee(Vector3 goalPos)
    {
        Vector3 pos = goalPos;
        pos.x += 2 * Level.chunkLength;
        pos.y = Level.bottomY + 0.15f;
        GameObject bee = placeObject(biome.type == TerrainType.DARK ? level.bee_glowing : level.bee, pos, Quaternion.Euler(0, -90, 0));
        bee.GetComponent<BeeEnemy>().StartMove(goalPos, level.foxPlayer.GetComponent<Fox>());
    }
    public IEnumerator moveEnemy(GameObject enemy,float delay, Vector3 pos, int d,float speed)
    {
        yield return new WaitForSeconds(delay);
        if (enemy == null) yield return null;
        else
        {
            if (Random.Range(0, 2) == 0)
                SFXManager.sfx_instance.Audio.PlayOneShot(SFXManager.sfx_instance.Bear);
            else
                SFXManager.sfx_instance.Audio.PlayOneShot(SFXManager.sfx_instance.Bear2);
            enemy.GetComponent<Enemy>().StartMoveTo(speed, pos, d);
            yield return null;
        }
    }
    public IEnumerator moveFoxEnemy(GameObject enemy, float delay, Vector3 pos, int d, float speed)
    {
        yield return new WaitForSeconds(delay);
        if (enemy != null)
            enemy.GetComponent<FoxEnemy>().StartMoveTo(speed, pos, d);
        yield return null;
    }
    public IEnumerator moveFlyingEnemy(GameObject enemy, float delay, float timeSpan, Vector3 pos)
    {
        yield return new WaitForSeconds(delay);
        SFXManager.sfx_instance.Audio.PlayOneShot(SFXManager.sfx_instance.Eagle);
        if (enemy != null)
            enemy.GetComponent<FlyingEnemy>().StartMoveTo(timeSpan, pos);
        yield return null;
    }
    public void placeGem(int row, int lane,int type)
    {
        //if (isEmpty) return;
        GameObject gem = level.gem1;
        if (type == 2) gem = level.gem2;
        if (type == 3) gem = level.gem3;
        gem = placeObject(gem, getItemCoord(row, lane), Quaternion.identity);
        gem.AddComponent<gem>();
        if (type == 3) gem.tag = "gem_special";
        else gem.tag = "gem";
       // gem.AddComponent<BoxCollider>();
       // gem.GetComponent<BoxCollider>().size = new Vector3(2f,2f,2f);
    }
    void placeManyGem(int row, int lane, int type)
    {
        if(isEmpty) return;
        GameObject gem = level.gem1;
        if (type == 2) gem = level.gem2;
        for (int i = 0;i< 2;i++)
        {
            Vector3 coord = getItemCoord(row, lane);
            if (i == 0) coord.z += 0.08f;
            if (i == 1) coord.z -= 0.08f;

            GameObject g = placeObject(gem, coord, Quaternion.identity);
            g.AddComponent<gem>();
            if (type == 3) g.tag = "gem_special";
            else g.tag = "gem";
        }
    }
    public void placeFood(int row, int lane, bool isKey)
    {
        if (isEmpty) return;
        GameObject food = level.nextWrongFood();
        if (isKey) food = level.nextKeyFood();

        if (food == null) return;
        food = placeObject(food, getItemCoord(row, lane), Quaternion.identity);
        food.AddComponent<food>();
        food.AddComponent<BoxCollider>();
        if (isKey) food.tag = "key_food";
        else food.tag = "wrong_food";
    }
    public void placeObstacle(int obs,int row,int lane)
    {
        if (isEmpty) return;
        Quaternion rot = Quaternion.Euler(0, 0, 0);
        if (obs == 18 || obs == 19 || obs == 11)
        {
            rot = Quaternion.Euler(0, 90, 0);
        }
        else
        {
            rot = Quaternion.Euler(0, Random.Range(0,180), 0);
        }
        GameObject obstacle = placeObject(level.obstacles[obs], getCoord(row, lane), rot);
        obstacle.tag = "obstacle";

        //dummy obstacle for river biome
        if (obs == 35)
        {
            obstacle.tag = "obstacle_water";
        }
        //obstacle.AddComponent<BoxCollider>();
    }
    public void placeLongObstacle(int obs, int row, int lane, int length, int width)
    {
        if (isEmpty) return;
        Vector3 pos = getCoord(row, lane);
        int yRot = 0;
        if (length == 2) pos = (pos + getCoord(row + 1, lane)) / 2.0f;
        if (length == 3) pos = getCoord(row + 1, lane);
        if (width == 2) pos = (pos + getCoord(row, lane + 1)) / 2.0f;
        if (width == 3) pos = getCoord(row, lane + 1);

        if (obs == 5 || obs == 6 || obs == 1 || obs == 2 || obs == 9)
        {
            yRot=90;
        }
        if (length > 1) yRot -= 90;
            
        GameObject obstacle = placeObject(level.longObstacles[obs],pos , Quaternion.Euler(0,yRot,0));
        obstacle.tag = "obstacle";
    }

    public GameObject placeObject(GameObject prefab, Vector3 pos,Quaternion rot)
    {
        GameObject feature = level.InstantiateObj(prefab, pos, rot);
    //    objects.Add(feature);
        feature.transform.parent = ground.transform;
        return feature;
    }
    public void placeBridge(int row, int lane, int length, int width)
    {
        int obs = biome.getBridge(Mathf.Max(width, length));
        if (obs == -1) return;

        Vector3 pos = getCoord(row, lane);
        int yRot = 0;
        if (length == 2) pos = (pos + getCoord(row + 1, lane)) / 2.0f;
        if (length == 3) pos = getCoord(row + 1, lane);
        if (width == 2) pos = (pos + getCoord(row , lane + 1)) / 2.0f;
        if (width == 3) pos = getCoord(row, lane + 1);
        if (obs == 3)
        {
            //xRot = 90;
            yRot = 90;
            pos.y = 1.16f;
        }
        if (obs == 0)
        {
            pos.y = 0.953f;
        }
        if (obs == 1)
        {
            pos.y = 1.113f;
        }
        if(obs == 4)
        {
            pos.y = 1.163f;
        }
        if (obs == 2)
        {
            pos.y = 0.983f;
        }
        if (obs == 5 || obs == 6)
        {
            yRot = 90;
            pos.y = 1.027f;
        }

        if (length > 1) yRot -= 90;

        placeObject(level.bridges[obs], pos, Quaternion.Euler(0, yRot, 0));
    }
    public void placeInlaneDecoration(int row, int lane)
    {
        GameObject prefab = biome.getInlaneDecoration();
        if (!prefab) return;

        Vector3 pos = getCoord(row, lane);
        if (biome.type == TerrainType.RIVER) pos.y = Level.riverBottomY;

        GameObject obj=placeObject(prefab, pos, Quaternion.Euler(0, Random.Range(0,180), 0));
        obj.transform.localScale= Vector3.one*0.1f;
    }
    public void OnDestroy()
    {
        foreach(GameObject obj in objects)
        {
            Level.DestroyObj(obj);
        }
        Level.DestroyObj(ground);
    }
    public void generate(bool hasFood)
    {
      //  Debug.Log(biome.getTerrainObject());
        float startPos = pos * Level.chunkLength;
        ground = level.InstantiateObj(biome.getTerrainObject(), new Vector3(0, 0, 0), Quaternion.identity);

        ground.AddComponent<BoxCollider>();
        ground.GetComponent<BoxCollider>().size = new Vector3(1.0f, 1.0f, 1.0f);
        ground.transform.position = new Vector3(startPos, pos%2==0?0:0.005f, 0);
        biome.addFence(startPos);

        List<int> lastPathEnds = new();
        if (lastChunk != null)
        {
            lastPathEnds = lastChunk.pathEnds;
        }
        obsgen = ObstacleGenerator.create(pos, level.difficulty, lastPathEnds,biome.type);
        obsgen.Generate(hasFood,biome.longObstacleChance,biome.generateBear,biome.generateEagle,biome.beeFrequency);
        pathEnds = obsgen.pathEnds;

        obsgen.addObstacles(this);
    }
    public void populate()
    {
        biome.populate();
    }

}
