using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class GridNode
{
    public int x;
    public int z;
    public GridNode parent;
    public bool isObstacle;
    public GridNode(int x, int z)
    {
        this.z = z;
        this.x = x;
        isObstacle = false;
    }
    public GridNode setParent(GridNode parent)
    {
        this.parent = parent;
        return this;
    }

}
public class Prey : MonoBehaviour
{
    protected Animator animator;
    //private List<GridInit[]> map=new();
    protected int posX;
    protected int posZ;
    protected int endLane = 0;
    private List<GridNode> path = new();
    protected int pathPos = 1;
    protected int chunkOffset = 0;
    protected float lifeTime = 0;
    protected int lifeSpan = 60;
    protected float playerSpeed = 2;
    protected bool isMoving = false;
    protected GameObject player;
    protected float reactionDistance = 0;
    protected bool dead=false;
    Vector3 targetGridOffset;
    float speedMultiplier=1;
    // Start is called before the first frame update
    void Start()
    {
    }
    protected virtual void onStart()
    {

    }
    public virtual void Move() {

        transform.rotation = Quaternion.Euler(0, 90, 0);
        isMoving = true;
    }
    public void init(List<GridInit[]> newChunk, int endLane, int chunkPos, int x, int z)
    {
        speedMultiplier = Random.Range(0.75f, 0.95f);
        posX = x;
        posZ = z;
        //  map.AddRange(newChunk);
        this.endLane = endLane;
        this.chunkOffset = chunkPos;
        targetGridOffset = new Vector3(Random.Range(-0.15f, 0.15f), 0, Random.Range(-0.15f, 0.15f));
        //path.AddRange(findPath(0, posZ, newChunk));


        //Debug.Log(path.Count);
    }
    public virtual void caught()
    {
        retire();
        //Destroy(gameObject);
    }
    protected virtual void setAnimatorSpeed(float speed)
    {
        if(animator!=null)
        animator.speed=speed/1.5f;
    }
    protected virtual void retire()
    {

    }
    public void updateChunks(List<GridInit[]> newChunk, int endLane, List<int> endLanes, float playerSpeed)
    {
        this.playerSpeed = playerSpeed;
        setAnimatorSpeed(playerSpeed);
        // this.chunkOffset = chunkPos;
        //map.AddRange(newChunk);
        this.endLane = endLane;
        int last = posZ;
        if (path.Count > 0)
        {
            last = path[path.Count - 1].z;
        }
        List<GridNode> newpath = findPath(0, last, newChunk, endLanes,false);
        if(newpath.Count < 12)
        {
           newpath = findPath(0, last, newChunk, endLanes, true);
        }
        
        
        path.AddRange(newpath);
        

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
    private List<GridNode> getNextGrid(int x, int z, ref HashSet<int> visited, List<GridInit[]> newChunk, bool obstacleAllowed)
    {
        visited.Add(x * 10 + z);
        List<GridNode> list = new();
        if (x + 1 >= newChunk.Count) { return list; }
        //Debug.Log(map[x + 1][z + 1]);

        if (newChunk[x + 1][z] == GridInit.EMPTY && !visited.Contains((x + 1) * 10 + z))
        {
            list.Add(new GridNode(x+1,z));
            if(Random.Range(0,3)>0)
                return list;
        }
        if (z < 4 && newChunk[x + 1][z + 1] == GridInit.EMPTY && !visited.Contains((x + 1) * 10 + z + 1))
        {
            list.Add(new GridNode(x + 1, z+1));
        }

        if (z > 0 && newChunk[x + 1][z - 1] == GridInit.EMPTY && !visited.Contains((x + 1) * 10 + z - 1))
        {
            list.Add(new GridNode(x + 1, z-1));
        }
        Shuffle(ref list);
        if (list.Count == 0 && obstacleAllowed)
        {

           // Debug.Log("obstacle");
            GridNode g = new GridNode(x + 1, z);
            g.isObstacle=true;
            list.Add(g);
            if (z < 4)
            {
            GridNode g2 = new GridNode(x + 1, z+1);
            g2.isObstacle = true;
            list.Add(g2);

            }
            if (z > 0)
            {

                GridNode g3 = new GridNode(x + 1, z - 1);
                g3.isObstacle = true;
                list.Add(g3);
            }
        }

        return list;
    }
    float getXCoord(int x)
    {
        return (chunkOffset * Level.chunkLength + (((float)x - 0.5f) / 12.0f) * Level.chunkLength);
    }
    private List<GridNode> findPath(int x, int z, List<GridInit[]> newChunk, List<int> endLanes,bool obstacleAllowed)
    {
        List<GridNode> path = new();
        Queue<GridNode> stack = new();
        stack.Enqueue(new GridNode(x, z));
        GridNode pos = new GridNode(x, z);
        HashSet<int> visited = new HashSet<int>();
        //  Debug.Log(x);
        int i = 0;
        while (stack.Count > 0)
        {
            // Debug.Log(newChunk[pos.x][ pos.z]);
            i++;

            if (i > 2000)
            {
                Debug.Log("maxit");
                break;
            }
            pos = stack.Dequeue();
            if (pos.x == newChunk.Count - 1 && (endLanes.Contains(pos.z) || obstacleAllowed))
            {
                break;
            }
            List<GridNode> neighbors = getNextGrid(pos.x, pos.z, ref visited, newChunk, obstacleAllowed);
            //   Debug.Log(map.Count);
            for (int j = 0; j < neighbors.Count; ++j)
            {
                // Debug.Log(neighbor[0]);
                stack.Enqueue(neighbors[j].setParent(pos));
            }
        }
        path.Add(pos);
        while (pos.parent != null)
        {
            pos = pos.parent;
            path.Add(pos);
        }
        path.Reverse();
        //Debug.Log(path.Count);
        return path;
    }
    // Update is called once per frame
    void Update()
    {
        if (player!=null && transform.position.x-player.transform.position.x  < reactionDistance && !isMoving && !dead) Move();
        

        if (!isMoving && !dead) return;
        lifeTime += Time.deltaTime;
        float tol = 0.2f;

        if (player.transform.position.x - transform.position.x > 10)
        {
            Destroy(gameObject);
            return;
        }
        if (lifeTime > lifeSpan)
        {
            Debug.Log("life over");
            Destroy(gameObject);
            return;
        }

        if (dead) return;
        if (transform.position.x > (getXCoord(posX) - 0.01f+targetGridOffset.x)
            && transform.position.x < getXCoord(posX) + tol)
        {

           // if (path.Count > posX && path[posX].isObstacle)
        //    {
         //       Debug.Log("retire");
        //        return;
         //   }
            pathPos++;
            if (path.Count > pathPos)
            {
                posZ = path[posX].z;
            }
            posX++;
            if (path.Count > posX && path[posX].isObstacle)
            {
                Debug.Log("retire");
                retire();
                return;
            }

        }
        if (path.Count < 0 || path.Count <= posX) { return; }

        Vector3 targetPos = new Vector3(getXCoord(posX)+targetGridOffset.x, Level.bottomY, Level.laneCoordinates[path[posX].z] + targetGridOffset.z);

        Vector3 direction = targetPos - transform.position;
        // Debug.Log(direction);
        direction.Normalize();
        direction *= playerSpeed * speedMultiplier * Time.deltaTime;
        Vector3 newpos = new Vector3(transform.position.x + direction.x, Level.bottomY + (Mathf.Sin(Time.time * 30 * playerSpeed) + 1) / 70.0f,
            transform.position.z + direction.z);
        transform.position = newpos;
        if(path[posX].z > posZ)
        {
            transform.rotation = Quaternion.Euler(0, 135, 0);

        }
        else if (path[posX].z < posZ)
        {
            transform.rotation = Quaternion.Euler(0, 45, 0);

        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);

        }


    }
}
