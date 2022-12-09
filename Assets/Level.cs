using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
    public GameObject player;
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
    internal int difficulty = 0;

    public GameObject gem1;
    public GameObject gem2;
    public GameObject gem3;

    public GameObject food1;
    public GameObject food2;

    public GameObject bear;
    public GameObject eagle;

    bool isGameStarted =false;
    float currentPlayerSpeed = 0;
    // Start is called before the first frame update
    void Start()
    {
        biomeSeed = Random.Range(0.0f, 20);
        decorationSeed = Random.Range(0.0f, 0);
        laneBounds[0] = leftBound;
        Debug.Log(laneWidth);
        for (int i = 0; i < 5; i++)
        {   
          //  laneCoordinates[i] = leftBound - laneWidth * i - laneWidth/2;
            laneBounds[i] = leftBound - laneWidth * (i+1);

        }
        //  player.AddComponent<BoxCollider>();
        // player.AddComponent<Rigidbody>();
        // player.AddComponent<CharacterController>();

        //  player.AddComponent<Chicken>();
        player.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        player.transform.position = new Vector3(-1, bottomY, laneCoordinates[2]);
        player.transform.rotation = Quaternion.Euler(0, 90, 0);
        
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
        int count = 1;
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
        if(player.transform.position.x > chunks.Last.Value.pos*chunkLength - chunkLength * 2 && isGameStarted)
        {
            if (chunks.Last.Value.pos % 3 == 2)
            {
                difficulty++;
                currentPlayerSpeed = player.GetComponent<Chicken>().increaseSpeed();
            }

            Debug.Log("create");
            createChunk(false);
           
            if(chunks.Count > 4)
            {
                deleteEarliestChunk();
            }
            
        }
    }

    public void createChunk(bool isEmpty)
    {
        Chunk lastChunk=null;
        if (chunks.Count > 0) {
            chunks.Last.Value.populate();
            lastChunk = chunks.Last.Value;
        }
        Chunk chunk = new(numTerrains,this,lastChunk, isEmpty);
        chunk.generate(false);
        chunk.spawnEnemy(currentPlayerSpeed);
        numTerrains++;
        chunks.AddLast(chunk);
    }
    void deleteEarliestChunk()
    {
        chunks.First.Value.OnDestroy();
        chunks.RemoveFirst();
    }
   

    // Update is called once per frame
    void Update()
    {
        checkCreateChunk();
        //                    GameObject house = Instantiate(house_prefab, new Vector3(0, 0, 0), Quaternion.identity);
        //house.AddComponent<BoxCollider>();
    }
}
