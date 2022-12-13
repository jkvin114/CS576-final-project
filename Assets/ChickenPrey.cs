using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;


class GridNode
{
    public int x;
    public int z;
    public GridNode parent;
    public GridNode(int x,int z)
    {
        this.z= z;
        this.x= x;
    }
    public GridNode setParent(GridNode parent)
    {
        this.parent = parent;
        return this;
    }

}

public class ChickenPrey : MonoBehaviour
{
    Animator animator;
    //private List<GridInit[]> map=new();
    private int posX;
    private int posZ;
    private int endLane=0;
    List<GridNode> path=new();
    int pathPos = 1;
    int chunkOffset= 0;
    float lifeTime = 0;
    int lifeSpan = 10;
    float playerSpeed = 2;
    bool isMoving = false;
    // Start is called before the first frame update
    void Start()
    {
        animator= GetComponent<Animator>();
        animator.SetBool("Eat", true);
        gameObject.tag = "prey";
        transform.rotation=Quaternion.Euler(0,Random.Range(0,360),0);
    }
    public void init(List<GridInit[]> newChunk, int endLane, int chunkPos, int x, int z)
    {

        posX = x;
        posZ = z;
      //  map.AddRange(newChunk);
        this.endLane = endLane;
        this.chunkOffset = chunkPos;

        //path.AddRange(findPath(0, posZ, newChunk));

        //Debug.Log(path.Count);
    }
    public void move()
    {
        animator.SetBool("Eat", false);
        animator.SetBool("Run", true);
        transform.rotation = Quaternion.Euler(0, 90, 0);
        isMoving = true;

     //   Debug.Log("move");
        // pathPos = 1;
    }
    public void caught() {
        Destroy(gameObject);
    }
 
    public void updateChunks(List<GridInit[]> newChunk,int endLane,int chunkPos,float playerSpeed)
    {
        this.playerSpeed = playerSpeed;
       // this.chunkOffset = chunkPos;
        //map.AddRange(newChunk);
        this.endLane= endLane;
        int last=posZ;
        if(path.Count>0 )
        {
        last = path[path.Count - 1].z;
        }
        List<GridNode>  newpath=findPath(0, last, newChunk);
        if (newpath.Count < 12)
        {
            Destroy(gameObject);
        }
        else
        {
            path.AddRange(newpath);
        }
        
     //   Debug.Log(path.Count);
        //  Debug.Log("chunk");
      //  Debug.Logpath[pathPos].x);
        //Debug.Log(posX);
    }
    protected void Shuffle<T>(ref List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    private List<int[]> getNeighborGrid(int x,int z, ref HashSet<int> visited, List<GridInit[]> newChunk)
    {
        visited.Add(x*10+z);
        List<int[]> list = new();
        if(x+1>= newChunk.Count) { return list; }
        //Debug.Log(map[x + 1][z + 1]);

        if (newChunk[x + 1][z] == GridInit.EMPTY && !visited.Contains((x + 1) * 10 + z))
        {
            list.Add(new int[] { x + 1, z });
        }
        if (z<4 && newChunk[x + 1][z + 1] == GridInit.EMPTY && !visited.Contains((x+1)*10+z+1))
        {
            list.Add(new int[] {x+1,z+1});
        }
        
        if (z >0 && newChunk[x+1][z-1] == GridInit.EMPTY && !visited.Contains((x + 1) * 10 + z - 1))
        {
            list.Add(new int[] { x+1, z-1 });
        }
        Shuffle(ref list);
        return list;
    }
    float getXCoord(int x)
    {
        return (chunkOffset * Level.chunkLength + (((float)x - 0.5f) / 12.0f) * Level.chunkLength);
    }
    private List<GridNode> findPath(int x, int z, List<GridInit[]> newChunk)
    {
        List<GridNode> path = new();
        Stack<GridNode> queue = new();
        queue.Push(new GridNode(x,z));
        GridNode pos = new GridNode(x, z);
        HashSet<int> visited = new HashSet<int>();
        //  Debug.Log(x);
        int i = 0;
        while (queue.Count > 0)
        {
           // Debug.Log(newChunk[pos.x][ pos.z]);
            i++;

            if (i > 2000) {
                Debug.Log("maxit");
                break;
            }
            pos = queue.Pop();
            if (pos.x == newChunk.Count - 1)
            {
                break;
            }
            List<int[]> neighbors= getNeighborGrid(pos.x, pos.z, ref visited, newChunk);
            //   Debug.Log(map.Count);
            for (int j= 0;j < neighbors.Count;++j)
            {
               // Debug.Log(neighbor[0]);
                queue.Push(new GridNode(neighbors[j][0], neighbors[j][1]).setParent(pos));
            }
        }
        path.Add(pos);
        while (pos.parent!= null)
        {
            pos = pos.parent;
            path.Add(pos);
        }
        path.Reverse();
        Debug.Log(path.Count);
        return path;
    }


    // Update is called once per frame
    void Update()
    {

        if (!isMoving) return;
        lifeTime += Time.deltaTime;
        float tol = 0.2f;

        if(lifeTime>lifeSpan)
        {
            Destroy(gameObject);
            return;
            Vector3 pos = transform.position;
            pos.x += playerSpeed*4 * Time.deltaTime;
            pos.z= Level.laneCoordinates[posZ] - Level.laneWidth / 2;
            transform.position= pos;
            if(lifeTime>lifeSpan+15) 
            return;
        }

        if (transform.position.x > (getXCoord(posX)-0.01f)
            && transform.position.x < getXCoord(posX) + tol)
        {
            pathPos++;
            if (path.Count > pathPos)
            {
                posZ = path[posX].z;
            }
            posX++;
        }
        if (path.Count < 0 || path.Count <= posX) { return; }

        Vector3 targetPos = new Vector3(getXCoord(posX), Level.bottomY, Level.laneCoordinates[path[posX].z] );

        Vector3 direction = targetPos - transform.position;
       // Debug.Log(direction);
        direction.Normalize();
        direction *= playerSpeed * 0.9f * Time.deltaTime;
        Vector3 newpos = new Vector3(transform.position.x + direction.x, Level.bottomY + (Mathf.Sin(Time.time * 30 * playerSpeed) + 1) / 70.0f,
            transform.position.z + direction.z);
        transform.position = newpos;



    }
}
