using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

enum Direction
{
    FORWARD=0,LEFT=1,RIGHT=2
}

public abstract class ObstacleGenerator
{

    internal GridInit[,] grid;
    protected int[,] fixedObstacles;
    protected bool isHard;
    protected bool hasFood;
    protected List<int> prevEnds;
    internal List<int> pathEnds = new();
    protected int difficulty;
    protected static int ROWS=12;
    protected static int LANES = 5;
    internal TerrainType biome;
    internal List<int[]> bears= new();
    internal List<int[]> eagles = new();
    internal List<int[]> foxes = new();
    internal List<int[]> preys = new();
    internal List<int[]> bees = new();
    public abstract void Generate(bool hasFood, float longObstacleChance, bool bear, bool eagle, float beeFrequency);
    protected ObstacleGenerator(int pos,int difficulty, List<int> prevEnds, TerrainType biome)
    {
        isHard = Random.Range(0, 15) <= difficulty - 2;
        //difficulty 0~1 : 0%, difficulty 5: 50%, difficulty 9: 100%
        this.difficulty = difficulty;
        this.prevEnds = prevEnds;
        grid = new GridInit[ROWS, LANES];
        for (int i = 0; i < ROWS; i++)
        {
            for (int j = 0; j < LANES; j++)
            {
                grid[i, j] = GridInit.NONE;
            }
        }

        this.biome = biome;
    }
    public static ObstacleGenerator create(int pos,  int difficulty, List<int> prevEnds,TerrainType biome)
    {
        if (pos % 20 ==55)
        {
            return new FixedObstacleGenerator(pos,difficulty, prevEnds , biome);
        }
        return new RandomObstacleGenerator(pos,  difficulty, prevEnds,biome);
    }
    protected bool randBool()
    {
        return Random.Range(0, 2) == 0;
    }
    // a helper function that randomly shuffles the elements of a list (useful to randomize the solution to the CSP)
    public void Shuffle<T>(ref List<T> list)
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

    public virtual void addObstacles(Chunk chunk)
    {
        for (int i = 0; i < 12; i++)
        {

            for (int j = 0; j < 5; j++)
            {
               // Debug.Log(grid[i, j]);
                int longobs = 0;
                switch (grid[i, j])
                {
                    case GridInit.GEM:
                        chunk.placeGem(i, j, 1);
                        
                        break;
                    case GridInit.GEM2:
                        chunk.placeGem(i, j, 2);
                        break;
                    case GridInit.GEM_SPECIAL:
                        chunk.placeGem(i, j, 3);
                        break;
                    case GridInit.KEY_FOOD:
                        chunk.placeFood(i, j, true);
                        break;
                    case GridInit.OTHER_FOOD:
                        chunk.placeFood(i, j, false);
                        break;
                    case GridInit.OBSTACLE:
                        int obs = chunk.biome.getObstacle();
                        chunk.placeObstacle(obs, i, j);
                        break;
                    case GridInit.OBSTACLES_2LONG:
                        longobs = chunk.biome.getLongObstacle(2);
                        chunk.placeLongObstacle(longobs, i, j, 2, 1);
                        break;
                    case GridInit.OBSTACLES_2WIDE:
                        longobs = chunk.biome.getLongObstacle(2);
                        chunk.placeLongObstacle(longobs, i, j, 1, 2);
                        break;
                    case GridInit.OBSTACLES_3LONG:
                        longobs = chunk.biome.getLongObstacle(3);
                        chunk.placeLongObstacle(longobs, i, j, 3, 1);
                        break;
                    case GridInit.OBSTACLES_3WIDE:
                        longobs = chunk.biome.getLongObstacle(3);
                        chunk.placeLongObstacle(longobs, i, j, 1, 3);
                        break;
                    case GridInit.ENEMY:
                        //chunk.placeObstacle(8, i, j);
                        if(isHard && Random.Range(0,8)==0) chunk.placeGem(i, j, 1);
                        break;
                    case GridInit.PREY:
                        preys.Add(new int[] { i, j });
                        break;
                }
            }
        }

        if (biome == TerrainType.RIVER)
        {
            addRiverBridges(chunk);
        }
    }

    void addRiverBridges(Chunk chunk)
    {


        for (int i = 0; i < ROWS - 1; i++)
        {
            for (int j = 0; j < LANES - 1; j++)
            {
                if (grid[i, j] != GridInit.OBSTACLE && grid[i, j] != GridInit.BRIDGE_PLACED)
                {
                    if (canPlaceBridgeAtFront(i, j, 3) && Random.Range(0.0f, 1.0f) < 0.5f)
                    {
                        grid[i, j] = GridInit.BRIDGE_PLACED;
                        grid[i + 1, j] = GridInit.BRIDGE_PLACED;
                        grid[i + 2, j] = GridInit.BRIDGE_PLACED;
                        chunk.placeBridge(i,j,3,1);
                    }
                    else if (canPlaceBridgeAtFront(i, j, 2) && Random.Range(0.0f, 1.0f) < 0.4f)
                    {
                        grid[i, j] = GridInit.BRIDGE_PLACED;
                        grid[i + 1, j] = GridInit.BRIDGE_PLACED;
                        chunk.placeBridge(i, j, 2, 1);
                    }
                    else if (canPlaceBridgeAtRight(i, j, 3) && Random.Range(0.0f, 1.0f) < 0.5f)
                    {
                        grid[i, j] = GridInit.BRIDGE_PLACED;
                        grid[i, j + 1] = GridInit.BRIDGE_PLACED;
                        grid[i, j + 2] = GridInit.BRIDGE_PLACED;
                        chunk.placeBridge(i, j, 1, 3);
                    }
                    else if (canPlaceBridgeAtRight(i, j, 2) && Random.Range(0.0f, 1.0f) < 0.4f)
                    {
                        grid[i, j] = GridInit.BRIDGE_PLACED;
                        grid[i, j + 1] = GridInit.BRIDGE_PLACED;
                        chunk.placeBridge(i, j, 1, 2);
                    }
                }
            }
        }
        for (int i = 0; i < ROWS ; i++)
        {
            for (int j = 0; j < LANES ; j++)
            {
                if (grid[i, j] != GridInit.OBSTACLE && grid[i, j] != GridInit.BRIDGE_PLACED) {
                    grid[i, j] = GridInit.BRIDGE_PLACED;
                    chunk.placeBridge(i, j, 1, 1);
                }
            }
        }
        for (int i = 0; i < ROWS; i++)
        {
            for (int j = 0; j < LANES; j++)
            {
                if (grid[i, j] == GridInit.OBSTACLE)
                {
                    if (i > 0 && grid[i - 1, j] == GridInit.BRIDGE_PLACED) continue;
                    if (i < ROWS-1 && grid[i + 1, j] == GridInit.BRIDGE_PLACED) continue;
                    if (j > 0 && grid[i , j-1] == GridInit.BRIDGE_PLACED) continue;
                    if (j<LANES-1 && grid[i , j+1] == GridInit.BRIDGE_PLACED) continue;
                    chunk.placeInlaneDecoration(i, j);
                }
            }
        }
               
    }

    bool canPlaceBridgeAtFront(int row, int lane, int distance)
    {
        for (int i = 0; i < distance; i++)
        {
            if (row + i >= ROWS) return false;
            if (grid[row + i, lane] == GridInit.OBSTACLE || grid[row + i, lane] == GridInit.BRIDGE_PLACED) return false;
        }
        return true;
    }
    bool canPlaceBridgeAtRight(int row, int lane, int distance)
    {
        for (int i = 0; i < distance; i++)
        {
            if (lane + i >= LANES) return false;
            if (grid[row, lane + i] == GridInit.OBSTACLE || grid[row, lane + i] == GridInit.BRIDGE_PLACED) return false;
        }
        return true;
    }
}


class RandomObstacleGenerator : ObstacleGenerator
{
    List<int[]>[] pathList = new List<int[]>[3];
    public RandomObstacleGenerator(int pos, int difficulty, List<int> prevEnds,TerrainType biome) :base(pos, difficulty, prevEnds,biome)
    {

    }
    public override void Generate(bool hasFood,float longObstacleChance,bool bear,bool eagle, float beeFrequency)
    {
        List<int> pathStarts = new();
        int startingRow = 0;
        int branchCount = Random.Range(0, 2);
        //branchCount = 0;
        if (isHard && hasFood) branchCount = 1;
        if (biome==TerrainType.RIVER) branchCount = 0;
        if (prevEnds.Count == 0)
        {
            pathStarts.Add(2);
        }
        else if(prevEnds.Count>1)
        {
            if (randBool() && biome != TerrainType.RIVER)
            {
                pathStarts.Add(prevEnds[0]);
                pathStarts.Add(prevEnds[1]);
                branchCount = 0;
            }
            else
            {
                pathStarts.Add(2);
                startingRow = 1;
                for (int j = 0; j < LANES; j++)
                {
                   grid[startingRow-1, j] = GridInit.EMPTY;
                }
                
                grid[startingRow, 1] = GridInit.EMPTY;
                grid[startingRow, 3] = GridInit.EMPTY;
            }
        }
        else
        {
            pathStarts.Add(prevEnds[0]);
        }


        for (int i = 0; i < pathStarts.Count; i++)
        {
            pathList[i] = new List<int[]>();
            makePath(pathStarts[i], startingRow, i);
        }
        for (int i = 0; i < branchCount; i++)
        {
            startingRow = Random.Range(startingRow, 6);
            pathList[i + 1] = new List<int[]>();
            makePath(findLaneAtRowOfPathNum(0,startingRow), startingRow, i + 1);
        }

        placeObstacleAndGem();


        int foodRow = placeFood(hasFood);

        if (biome == TerrainType.RIVER)
        {

            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < LANES; j++)
                {
                    bool bee = (Random.Range(0.0f, 1.0f) < beeFrequency && difficulty > 10);
                    if (j < LANES - 1 && grid[i, j] != GridInit.OBSTACLE && grid[i, j + 1] == GridInit.OBSTACLE && Random.Range(0.0f, 1.0f) < 0.3f)
                    {
                        grid[i, j + 1] = GridInit.EMPTY;
                        if (bee)
                        {
                            bees.Add(new int[] { i, j + 1 });
                            grid[i, j + 1] = GridInit.ENEMY;
                        }
                        else if (randBool())
                        {
                            grid[i, j + 1] = GridInit.GEM_SPECIAL;
                        }
                        else if (Random.Range(0, 4) == 0 && difficulty>20)
                        {
                            eagles.Add(new int[] { i, j + 1 });
                            grid[i, j + 1] = GridInit.ENEMY;
                        }
                        break;
                    }
                    if (j < LANES - 1 && grid[i, j] == GridInit.OBSTACLE && grid[i, j + 1] != GridInit.OBSTACLE && Random.Range(0.0f, 1.0f) < 0.3f)
                    {
                        grid[i, j ] = GridInit.EMPTY;
                        if (bee)
                        {
                            bees.Add(new int[] { i, j  });
                            grid[i, j] = GridInit.ENEMY;
                        }
                        else if (randBool())
                        {
                            grid[i, j] = GridInit.GEM_SPECIAL;
                        }
                        else if (Random.Range(0,4)==0 && difficulty > 20)
                        {
                            eagles.Add(new int[] { i, j });
                            grid[i, j] = GridInit.ENEMY;
                        }
                        break;
                    }
                }
            }
        }

        placeBees(beeFrequency);

        placeEnemies(foodRow, hasFood, bear, eagle);
        convertLongObstacles(longObstacleChance);
    }
    void placeObstacleAndGem()
    {
        float obstacleFrequency = Mathf.Log(difficulty + 2, 2);

        //place obstacle and gems
        for (int i = 0; i < ROWS; i++)
        {
            for (int j = 0; j < LANES; j++)
            {
                if (grid[i, j] == GridInit.NONE)
                {
                    if (biome == TerrainType.RIVER)
                    {
                        grid[i, j] = GridInit.OBSTACLE;
                        continue;
                    }

                    if (Random.Range(0, obstacleFrequency + 2) < obstacleFrequency)
                        grid[i, j] = GridInit.OBSTACLE;
                    else if (randBool())
                    {
                        // 1/6 chance for special gem, 2/6 chance for normal gem
                        if (isHard)
                        {
                            grid[i, j] = Random.Range(0, 3) == 0 ? GridInit.GEM : GridInit.GEM_SPECIAL; //additional gem out of the path
                        }
                        else if (Random.Range(0, 6) == 0)//   1/12 chance for special gem
                        {
                            grid[i, j] = GridInit.GEM_SPECIAL; //additional special gem out of the path
                        }
                    }
                }

            }
        }
    }
    int placeFood(bool hasFood)
    {
        int foodRow = Random.Range(7, 10);
        //place food 
        if (hasFood)
        {
            if (isHard && biome!=TerrainType.RIVER)
            {
                int totalFoodCount = Random.Range(2, 5);
                bool keyFoodOnMainPath = randBool();
                if (keyFoodOnMainPath)
                {
                    placeFoodAtPath(1, foodRow, totalFoodCount - 2, false);
                    placeFoodAtPath(0, foodRow, 2, true);
                }
                else
                {
                    placeFoodAtPath(0, foodRow, totalFoodCount - 2, false);
                    placeFoodAtPath(1, foodRow, 2, true);
                }
            }
            else
            {
                placeFoodAtPath(0, foodRow, 2, true);
            }
        }
        return foodRow;
    }
    void placeBees(float beeFrequency)
    {
        if (beeFrequency == 0) return;
        for (int i = 0; i < ROWS - 1; i++)
        {
            for (int j = 0; j < LANES - 1; j++)
            {

                if (Random.Range(0.0f, 1.0f) < beeFrequency && difficulty > 4)
                {
                    if(grid[i, j] == GridInit.OBSTACLE && biome != TerrainType.RIVER)
                    {
                        bees.Add(new int[] { i, j });
                        grid[i, j] = GridInit.ENEMY;
                    }
                }
            }

        }
    }
    void placeFoodAtPath(int pathNum,int row, int count, bool hasKeyFood)
    {
        if (count == 1)
        {
            int lane=findLaneAtRowOfPathNum(pathNum, row);
            if (hasKeyFood) grid[row, lane] = GridInit.KEY_FOOD;
            else grid[row, lane] = GridInit.OTHER_FOOD;
        }
        else if (count == 2)
        {
            int lane = findLaneAtRowOfPathNum(pathNum, row);
            int otherLane = randBool() ? lane + 1 : lane - 1;

            if (lane == 0) otherLane=1; 
            else if(lane==LANES-1) otherLane=3;

            //create some space around food
            for (int i = -2; i < 3; i++)
            {
                grid[row + i, otherLane] = GridInit.EMPTY;
                grid[row + i, lane] = GridInit.EMPTY;
            }

            if (randBool())
            {
                grid[row, lane]= hasKeyFood?GridInit.KEY_FOOD: GridInit.OTHER_FOOD;
                grid[row,otherLane]=GridInit.OTHER_FOOD;
            }
            else
            {
                grid[row, lane] = GridInit.OTHER_FOOD;
                grid[row, otherLane] = hasKeyFood ? GridInit.KEY_FOOD : GridInit.OTHER_FOOD;
            }


        }
    }
    
    void convertLongObstacles(float longObstacleChance)
    {
        if (longObstacleChance==0) return;
        //convert some obstacle into long obstacles
        for (int i = 0; i < ROWS - 1; i++)
        {
            for (int j = 0; j < LANES - 1; j++)
            {
                if (grid[i, j] == GridInit.OBSTACLE)
                {
                    if (canPlaceObstacleAtRight(i, j, 3) && Random.Range(0.0f, 1.0f) < longObstacleChance)
                    {
                        grid[i, j] = GridInit.OBSTACLES_3WIDE;
                        grid[i, j + 1] = GridInit.LONG_OBSTACLE_SPACE;
                        grid[i, j + 2] = GridInit.LONG_OBSTACLE_SPACE;
                    }
                    else if (canPlaceObstacleAtRight(i, j, 2) && Random.Range(0.0f, 1.0f) < longObstacleChance)
                    {
                        grid[i, j] = GridInit.OBSTACLES_2WIDE;
                        grid[i, j + 1] = GridInit.LONG_OBSTACLE_SPACE;
                    }
                    else if (canPlaceObstacleAtFront(i, j, 3) && Random.Range(0.0f, 1.0f) < longObstacleChance)
                    {
                        grid[i, j] = GridInit.OBSTACLES_3LONG;
                        grid[i + 1, j] = GridInit.LONG_OBSTACLE_SPACE;
                        grid[i + 2, j] = GridInit.LONG_OBSTACLE_SPACE;
                    }

                    else if (canPlaceObstacleAtFront(i, j, 2) && Random.Range(0.0f, 1.0f) < longObstacleChance)
                    {
                        grid[i, j] = GridInit.OBSTACLES_2LONG;
                        grid[i + 1, j] = GridInit.LONG_OBSTACLE_SPACE;
                    }
                }
            }
        }
    }

    void placeEnemies(int foodRow,bool hasFood,bool bear, bool eagle)
    {
        for (int i = 0; i < ROWS - 1; i++)
        {
            if (hasFood && foodRow == i) continue;


            int enemyLane = Random.Range(0, LANES);
            if (Random.Range(0, 50) <= difficulty - 2 && grid[i, enemyLane] == GridInit.OBSTACLE)
            {
                int type = Random.Range(0, 10);
                //bear
                if (type < 6)
                {
                    if (!bear) continue;
                    for (int j = 0; j < LANES; j++)
                    {
                        //   if (j == enemyLane) grid[i, enemyLane] = GridInit.ENEMY;
                        if (grid[i, j] == GridInit.OBSTACLE) grid[i, j] = GridInit.ENEMY_PATH;
                    }
                    bears.Add(new int[] { i, enemyLane });
                    grid[i, enemyLane] = GridInit.ENEMY;
                }
                else//eagle
                {
                    if (!eagle) continue;
                    if (difficulty < 30 && (biome == TerrainType.FOREST || biome == TerrainType.MOUNTAIN)) continue;
                    //  grid[i, enemyLane] = GridInit.ENEMY;
                    eagles.Add(new int[] { i, enemyLane });
                    grid[i, enemyLane] = GridInit.ENEMY;
                }
            }
        }
    }
    bool canPlaceObstacleAtFront(int row,int lane,int distance)
    {
        for(int i = 0; i < distance; i++)
        {
             if (row + i >= ROWS) return false;
             if (grid[row + i, lane] != GridInit.OBSTACLE) return false;
        }
        return true;
    }
    bool canPlaceObstacleAtRight(int row, int lane, int distance)
    {
        for(int i=0; i < distance; i++)
        {
            if (lane + i >= LANES) return false;
            if (grid[row, lane+i] != GridInit.OBSTACLE) return false;
        }
        return true;
    }
    int findLaneAtRowOfPathNum(int pathNum,int row)
    {
        foreach (int[] block in pathList[pathNum])
        {
            if (block[0]==row) return block[1];
        }
        return 0;
    }
    void makePath(int startLane, int startRow, int pathNum)
    {
        int row = startRow;
        int lane = startLane;
        Direction direction = Direction.FORWARD;
        while (row < ROWS-1)
        {
            if (direction==Direction.FORWARD &&  Random.Range(0, 9) > 0)
            {
                if (pathNum > 0 && grid[row, lane] != GridInit.GEM)
                    grid[row, lane] = GridInit.GEM2;
                else {
                    grid[row, lane] = GridInit.GEM;

                    if (Random.Range(0, 10) == 0)
                    {
                        grid[row, lane] = GridInit.PREY;
                    }
                }
            }
            else grid[row, lane] = GridInit.EMPTY;

            if(biome==TerrainType.RIVER && Random.Range(0.0f, difficulty/10.0f+2.0f) >=1)
            {
                int extraLane = lane;
                if (lane == 0)
                {
                    extraLane = lane + 1;
                }   
                else if (lane == 4)
                {
                    extraLane = lane - 1;
                }
                else
                {
                    extraLane = lane + (randBool() ? 1 : -1);
                }

                if(grid[row, extraLane] != GridInit.GEM2 && grid[row, extraLane] != GridInit.GEM)
                {
                 //   grid[row, extraLane] = GridInit.EXTRA_BRIDGE;
                }
            }
            Direction lastDirection = direction;

            pathList[pathNum].Add(new int[] { row, lane });

            // if(!isHard)
            if (!(difficulty > 7 && isHard && Random.Range(6, 40) < difficulty))
            {
                grid[row + 1, lane] = GridInit.EMPTY;
            }
                

            if(direction==Direction.LEFT || direction == Direction.RIGHT)
               direction= Direction.FORWARD;
            else if(lane==0) //left end line
            {
                //encourage path not to keep stick at the end
                if (Random.Range(0, 3) == 0) direction= Direction.FORWARD; 
                else direction= Direction.RIGHT;
            }
            else if (lane == LANES-1)//right end line
            {//encourage path not to keep stick at the end
                if (Random.Range(0, 3) == 0) direction = Direction.FORWARD;
                else direction = Direction.LEFT;
            }
            else
            {
                direction = (Direction)Random.Range(0, 3);

                //favors moving to the direction that it was previously going
                if(direction==Direction.RIGHT && lastDirection == Direction.LEFT && randBool()) direction= Direction.LEFT;
                if (direction == Direction.LEFT && lastDirection == Direction.RIGHT && randBool()) direction = Direction.RIGHT;
            }

            
            switch(direction)
            {
                case Direction.LEFT:
                    lane -= 1;
                    break;
                case Direction.RIGHT:
                    lane += 1; break;
                case Direction.FORWARD:
                    row+=1;
                    
                    
                    break;
            }

            lane = Mathf.Clamp(lane, 0, LANES-1);
           // Debug.Log(direction);

        }
        grid[ROWS-1, lane] = GridInit.EMPTY;
        pathEnds.Add(lane);
    }
}
class FixedObstacleGenerator : ObstacleGenerator
{
    bool isRandom;
    private readonly int[] fixedObstacles; //each index corresponds to fixed 1-wide, 2-wide, 3-wide obstacles
    public FixedObstacleGenerator(int pos, int difficulty, List<int> prevEnds, TerrainType biome) : base(pos, difficulty, prevEnds,  biome)
    {
        isRandom = Random.Range(0, 5) == 0;
        fixedObstacles = new int[] { Random.Range(0, 2), Random.Range(2, 4), Random.Range(4, 6) };
    }
    public override void Generate(bool hasFood, float longObstacleChance, bool bear, bool eagle, float beeFrequency)
    {
        for (int i = 1; i < 11; i++)
        {
            bool type=Random.Range(0, 2)==0;
            grid[i, 2] = GridInit.GEM;
            grid[i, 0] = GridInit.ENEMY;
            grid[i, 4] = GridInit.ENEMY;
            grid[i, 1] = GridInit.ENEMY;
            grid[i, 3] = GridInit.ENEMY;
            foxes.Add(new int[] { i, type?0:1 });
            foxes.Add(new int[] { i, type ? 4 : 3 });
            eagles.Add(new int[] { i, type ? 1 : 0 });
            eagles.Add(new int[] { i, type ? 3 : 4 });
        }
    }
    void gen()
    {
        for (int i = 0; i < 12; i++)
        {

            for (int j = 0; j < 5; j++)
            {
                if (j == 0)
                {
                    grid[i, 0] = GridInit.OBSTACLES_2WIDE;

                    grid[i, 1] = GridInit.LONG_OBSTACLE_SPACE;

                }
                else if (j == 1)
                {
                    grid[i, j] = GridInit.GEM;
                }
                else if (j == 3)
                {
                    grid[i, j] = GridInit.GEM2;
                }
            }
        }
    }
    public override void addObstacles(Chunk chunk)
    {
        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                int longobs = 0;
                switch (grid[i, j])
                {
                    case GridInit.OBSTACLE:
                        int obs = !isRandom? chunk.biome.getFixedObstacle(fixedObstacles[0]):chunk.biome.getRandomFixedObstacle(0);
                        chunk.placeObstacle(obs, i, j);
                        grid[i, j] = GridInit.FIXED_OBSTACLE;
                        break;
                    case GridInit.OBSTACLES_2LONG:
                        longobs = !isRandom ? chunk.biome.getFixedObstacle(fixedObstacles[1]) : chunk.biome.getRandomFixedObstacle(1);
                        chunk.placeLongObstacle(longobs, i, j, 2, 1);
                        grid[i, j] = GridInit.FIXED_OBSTACLE;
                        break;
                    case GridInit.OBSTACLES_2WIDE:
                        longobs = !isRandom ? chunk.biome.getFixedObstacle(fixedObstacles[1]) : chunk.biome.getRandomFixedObstacle(1);
                        chunk.placeLongObstacle(longobs, i, j, 1, 2);
                        grid[i, j] = GridInit.FIXED_OBSTACLE;
                        break;
                    case GridInit.OBSTACLES_3LONG:
                        longobs = !isRandom ? chunk.biome.getFixedObstacle(fixedObstacles[2]) : chunk.biome.getRandomFixedObstacle(2);
                        chunk.placeLongObstacle(longobs, i, j, 3, 1);
                        grid[i, j] = GridInit.FIXED_OBSTACLE;
                        break;
                    case GridInit.OBSTACLES_3WIDE:
                        longobs = !isRandom ? chunk.biome.getFixedObstacle(fixedObstacles[2]) : chunk.biome.getRandomFixedObstacle(2);
                        chunk.placeLongObstacle(longobs, i, j, 1, 3);
                        grid[i, j] = GridInit.FIXED_OBSTACLE;
                        break;
                }
            }
        }
        base.addObstacles(chunk);
    }
}
