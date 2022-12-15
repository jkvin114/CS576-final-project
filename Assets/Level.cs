using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public enum TerrainType
{
    PLAIN = 0,
    ROCKY = 1,
    FOREST = 2,
    MOUNTAIN = 3,
    ROCKY_MOUNTAIN = 4,
}

public class Level : MonoBehaviour
{
    public GameObject plain_terrain;
    public GameObject rocky_terrain;
    public GameObject forest_terrain;
    public GameObject mountain_terrain;
    public GameObject rocky_mountain_terrain;
    public GameObject foxPlayer;
    public GameObject chickenPlayer;
    public GameObject chickenPrey;
    public GameObject rabbitPrey;


    public GameObject fence1;
    public GameObject fence2;

    public GameObject tree;
    public GameObject bush;
    private Level level;

    internal static int chunkLength = 6;
    
    private int offset = 0;
    internal static float bottomY = 1.28f;
    internal static float rightBound = -0.1f;
    internal static float leftBound = 1.95f;
    internal static float terrainLeftBound = 3.8f;
    internal static float terrainRightBound = -1.76f;
    internal static float biomeSeed=0;
    internal static float decorationSeed=0;

    internal static float center = (rightBound + leftBound)/2;
    public GameObject[] terrains;
    public GameObject[] fences;
    public GameObject[] trees;
    public GameObject[] colored_trees;
    public GameObject[] pine_trees;
    public GameObject[] bushes;
    public GameObject[] dead_plants;
    public GameObject[] rocks;
    public GameObject[] stumps;
    public GameObject[] flowers;
    public GameObject[] light_plants;

    public GameObject[] obstacles;
    public GameObject[] longObstacles;

    private LinkedList<Chunk> chunks=new();

    internal static float laneWidth = 0.41f;
    internal static float[] laneCoordinates= new float[5] { 1.745f, 1.335f, 0.925f, 0.515f, 0.105f };
    internal float[] laneBounds = new float[6];
    private int numTerrains=0;
    public int difficulty = 3;
    public int difficultyIncreaseFrequency;

    public GameObject gem1;
    public GameObject gem2;
    public GameObject gem3;

    public GameObject food1;
    public GameObject food2;

    public GameObject bear;
    public GameObject eagle;
    public GameObject fox;

    bool isGameStarted =false;
    float currentPlayerSpeed = 0;
    public bool isChicken=false;

    List<GameObject> activePreys = new();
    List<int> keyFoods= new();
    internal int nextFood = 0;
    public Timer timer;
    bool canGenerateFood = true;
    // Start is called before the first frame update
    void Start()
    {
        biomeSeed = Random.Range(0.0f, 20);
        decorationSeed = Random.Range(0.0f, 0);
        laneBounds[0] = leftBound;
      //  Debug.Log(laneWidth);
        for (int i = 0; i < 5; i++)
        {   
          //  laneCoordinates[i] = leftBound - laneWidth * i - laneWidth/2;
            laneBounds[i] = leftBound - laneWidth * (i+1);

        }
        //  player.AddComponent<BoxCollider>();
        // player.AddComponent<Rigidbody>();
        // player.AddComponent<CharacterController>();

        //  player.AddComponent<Chicken>();
        if (isChicken)
        {

            chickenPlayer.transform.position = new Vector3(-1, bottomY, laneCoordinates[2]);
            chickenPlayer.transform.rotation = Quaternion.Euler(0, 90, 0);
            foxPlayer.SetActive(false);
        }
        else
        {

            foxPlayer.transform.position = new Vector3(-1, bottomY, laneCoordinates[2]);
            foxPlayer.transform.rotation = Quaternion.Euler(0, 90, 0);
            chickenPlayer.SetActive(false);
        }
        
        //Instantiate(player, , );
        // terrains =new GameObject[5]{plain_terrain ,rocky_terrain, forest_terrain ,mountain_terrain,rocky_mountain_terrain};
        // fences = new GameObject[2] { fence1, fence2 };
        StartCoroutine(initlalGeneration());

        
        return;
       
    }
    public void Coroutine(IEnumerator func) {
        StartCoroutine(func);
    }

    public IEnumerator initlalGeneration()
    {
        int count = 2;
        for(int i=0;i<count; i++) {
            createChunk(true);
            yield return new WaitForSeconds(0.3f);

        }
        isGameStarted=true;
        yield return null;
    }
    public GameObject InstantiateObj(GameObject gameObject,Vector3 pos,Quaternion rotation)
    {
        return Instantiate(gameObject, pos, rotation);
    }
    public static void DestroyObj(GameObject gameObject)
    {

        Destroy(gameObject);
    }
    private void checkCreateChunk()
    {
        GameObject player = isChicken ? chickenPlayer : foxPlayer;
        if(player.transform.position.x > chunks.Last.Value.pos*chunkLength - chunkLength * 2 && isGameStarted)
        {
            if (chunks.Last.Value.pos % difficultyIncreaseFrequency == difficultyIncreaseFrequency - 1)
            {
                difficulty++;
                if(isChicken)
                {
                    currentPlayerSpeed = chickenPlayer.GetComponent<Chicken>().increaseSpeed();

                }
                else
                {
                    currentPlayerSpeed = foxPlayer.GetComponent<Fox>().increaseSpeed();

                }
            }

            createChunk(false);
           
            if(chunks.Count > 4)
            {
                deleteEarliestChunk();
            }
            
        }
    }
    public GameObject nextKeyFood()
    {
        Debug.Log(keyFoods.Count);
        if(nextFood < keyFoods.Count)
        {
            return timer.foodPrefabs[keyFoods[nextFood]-1];
        }
        return null;
    }
    public GameObject nextWrongFood()
    {
        List<int> wrongFoods= new List<int>();
        for(int i = 1; i <= timer.foodPrefabs.Count; i++)
        {
            if(!keyFoods.Contains(i)) wrongFoods.Add(i);
        }

        if (nextFood < keyFoods.Count)
        {
            return timer.foodPrefabs[wrongFoods[Random.Range(0,wrongFoods.Count-1)]-1];
        }
        return null;
    }
    public void onGetRightFood()
    {
        nextFood++;
        Debug.Log(nextFood);
        if (nextFood == keyFoods.Count)
        {
            StartCoroutine(spawnFood());
            nextFood = 0;
        }
    }
    IEnumerator spawnFood()
    {
        canGenerateFood= false;
        yield return new WaitForSeconds(8.0f/foxPlayer.GetComponent<Fox>().running_velocity);
        StartCoroutine(initlalGeneration());
        timer.CompletesList();
        canGenerateFood = true;

        yield return null;
    }
    public void onSpawnFood(List<int> keyFoods)
    {

        nextFood = 0;
        this.keyFoods= keyFoods;
        Debug.Log(keyFoods);
    }
    public void addPrey(GameObject p)
    {
        activePreys.Add(p);
    }

    public void createChunk(bool isEmpty)
    {
        Chunk lastChunk=null;
        if (chunks.Count > 0) {
            chunks.Last.Value.populate();
            lastChunk = chunks.Last.Value;
        }
        Chunk chunk = new(numTerrains,this,lastChunk, isEmpty);
        chunk.generate(canGenerateFood&& numTerrains%5==3);//numTerrains%Random.Range(3,5)==2
        chunk.spawnEnemy(currentPlayerSpeed);
        List<GridInit[]> chunkMask = maskChunk(chunk.obsgen.grid);
        updatePreys(chunkMask,chunk.pathEnds);
        numTerrains++;
        chunks.AddLast(chunk);
    }
    void deleteEarliestChunk()
    {
        chunks.First.Value.OnDestroy();
        chunks.RemoveFirst();
    }
    public List<GridInit[]> maskChunk(GridInit[,] grid)
    {
        List<GridInit[]> mask=new();
        for (int i = 0; i < 12; i++)
        {
            GridInit[] row = new GridInit[5];
            for (int j = 0; j < 5; j++)
            {
                if(grid[i, j]==GridInit.OBSTACLE ||
                    grid[i, j] == GridInit.LONG_OBSTACLE_SPACE ||
                    grid[i, j] == GridInit.FIXED_OBSTACLE ||
                    grid[i, j] == GridInit.OBSTACLES_2LONG ||
                    grid[i, j] == GridInit.OBSTACLES_2WIDE ||
                    grid[i, j] == GridInit.OBSTACLES_3WIDE ||
                    grid[i, j] == GridInit.OBSTACLES_3LONG ||
                    grid[i, j] == GridInit.ENEMY
                    )
                    row[j] = GridInit.OBSTACLE;
                else row[j] = GridInit.EMPTY;
            }
            mask.Add(row);
        }
        return mask;
    }
    void updatePreys(List<GridInit[]> maskChunk, List<int> endLane)
    {
        foreach (GameObject g in activePreys)
        {
            if (g.IsDestroyed())
            {
                continue;
            }
            g.GetComponent<Prey>().updateChunks(maskChunk, endLane[Random.Range(0, endLane.Count - 1)], endLane, currentPlayerSpeed);
        }

    }
    // Update is called once per frame
    void Update()
    {
        checkCreateChunk();
        //                    GameObject house = Instantiate(house_prefab, new Vector3(0, 0, 0), Quaternion.identity);
        //house.AddComponent<BoxCollider>();
    }
}
