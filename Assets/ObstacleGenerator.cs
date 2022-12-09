using System.Collections.Generic;
using UnityEngine;

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

    internal List<int[]> bears= new();
    internal List<int[]> eagles = new();
    public abstract void Generate(bool hasFood);
    protected ObstacleGenerator(int pos,int difficulty,List<int> prevEnds)
    {
        isHard = Random.Range(0, 8) <= difficulty - 2;
        //difficulty 0~1 : 0%, difficulty 5: 50%, difficulty 9: 100%
        this.difficulty= difficulty;
        this.prevEnds = prevEnds;
        grid = new GridInit[ROWS, LANES];
        for (int i = 0; i < ROWS; i++)
        {
            for (int j = 0; j < LANES; j++)
            {
                grid[i, j] = GridInit.NONE;
            }
        }
    }
    public static ObstacleGenerator create(int pos,  int difficulty, List<int> prevEnds)
    {
     //   if (pos % 10 > 8)
      //  {
      //      return new FixedObstacleGenerator(pos,difficulty, prevEnds);
      //  }
        return new RandomObstacleGenerator(pos,  difficulty, prevEnds);
    }
    protected bool randBool()
    {
        return Random.Range(0, 2) == 0;
    }
    // a helper function that randomly shuffles the elements of a list (useful to randomize the solution to the CSP)
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

    public virtual void addObstacles(Chunk chunk)
    {
        for (int i = 0; i < 12; i++)
        {

            for (int j = 0; j < 5; j++)
            {
                Debug.Log(grid[i, j]);
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
                        if(isHard && Random.Range(0,4)==0) chunk.placeGem(i, j, 1);
                        break;
                }
            }
        }
    }
}


class RandomObstacleGenerator : ObstacleGenerator
{
    List<int[]>[] pathList = new List<int[]>[3];
    public RandomObstacleGenerator(int pos, int difficulty, List<int> prevEnds) :base(pos, difficulty, prevEnds)
    {

    }
    public override void Generate(bool hasFood)
    {
        List<int> pathStarts = new();
        int startingRow = 0;
        int branchCount = Random.Range(0, 2);
        //branchCount = 0;
        if (isHard && hasFood) branchCount = 1;

        if (prevEnds.Count == 0)
        {
            pathStarts.Add(2);
        }
        else if(prevEnds.Count>1)
        {
            if (randBool())
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


        float obstacleFrequency = Mathf.Log(difficulty + 2, 2);

        //place obstacle and gems
        for (int i = 0; i < ROWS; i++)
        {
            for (int j = 0; j < LANES; j++)
            {
                if (grid[i, j] == GridInit.NONE)
                {


                    if (Random.Range(0, obstacleFrequency + 2) < obstacleFrequency)
                        grid[i, j] = GridInit.OBSTACLE;
                    else if (randBool())
                    {
                        // 1/6 chance for special gem, 2/6 chance for normal gem
                        if (isHard)
                        {
                            grid[i, j] = Random.Range(0,3)==0? GridInit.GEM:GridInit.GEM_SPECIAL; //additional gem out of the path
                        }
                        else if(Random.Range(0, 6) == 0)//   1/12 chance for special gem
                        {
                            grid[i, j] = GridInit.GEM_SPECIAL; //additional special gem out of the path
                        }
                    }
                }

            }
        }
        int foodRow = Random.Range(7, 10);
        //place food 
        if (hasFood)
        {
            if (isHard)
            {
                int totalFoodCount= Random.Range(2,5);
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
                placeFoodAtPath(0, foodRow, 2,true);
            }   
        }

        for (int i = 0; i < ROWS - 1; i++)
        {
            if (hasFood && foodRow == i) continue;

            int enemyLane = Random.Range(0, LANES);
            if(Random.Range(0, 30) <= difficulty - 1 && grid[i, enemyLane] == GridInit.OBSTACLE)
            {
                //bear
                if (Random.Range(0,3)<2)
                {
                    for (int j = 0; j < LANES - 1; j++) {
                        if (j == enemyLane) grid[i, enemyLane] = GridInit.ENEMY;
                        else if(grid[i, j]==GridInit.OBSTACLE) grid[i,j]= GridInit.ENEMY_PATH;
                    }
                    bears.Add(new int[] { i, enemyLane });
                }
                else//eagle
                {
                    grid[i, enemyLane] = GridInit.ENEMY;
                    eagles.Add(new int[] { i, enemyLane });
                }
            }
        }


            //convert some obstacle into long obstacles
        for (int i = 0; i < ROWS - 1; i++)
        {
            for (int j = 0; j < LANES - 1; j++)
            {
                if (grid[i, j] == GridInit.OBSTACLE)
                {
                    if (canPlaceObstacleAtRight(i, j, 3) && randBool())
                    {
                        grid[i, j] = GridInit.OBSTACLES_3WIDE;
                        grid[i, j + 1] = GridInit.LONG_OBSTACLE_SPACE;
                        grid[i, j + 2] = GridInit.LONG_OBSTACLE_SPACE;
                    }
                    else if (canPlaceObstacleAtRight(i, j, 2) && randBool())
                    {
                        grid[i, j] = GridInit.OBSTACLES_2WIDE;
                        grid[i, j + 1] = GridInit.LONG_OBSTACLE_SPACE;
                    }
                    else if (canPlaceObstacleAtFront(i, j, 3) && randBool())
                    {
                        grid[i, j] = GridInit.OBSTACLES_3LONG;
                        grid[i + 1, j] = GridInit.LONG_OBSTACLE_SPACE;
                        grid[i + 2, j] = GridInit.LONG_OBSTACLE_SPACE;
                    }
                    
                    else if (canPlaceObstacleAtFront(i, j, 2) && randBool())
                    {
                        grid[i, j] = GridInit.OBSTACLES_2LONG;
                        grid[i + 1, j] = GridInit.LONG_OBSTACLE_SPACE;
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
                if(pathNum>0 && grid[row, lane]!=GridInit.GEM)
                    grid[row, lane] = GridInit.GEM2;
                else grid[row, lane] = GridInit.GEM;
            }
                
            else grid[row, lane] = GridInit.EMPTY;
            Direction lastDirection = direction;

            pathList[pathNum].Add(new int[] { row, lane });

            if(!isHard) grid[row+1, lane] = GridInit.EMPTY;

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
    public FixedObstacleGenerator(int pos, int difficulty, List<int> prevEnds) : base(pos, difficulty, prevEnds)
    {
        isRandom = Random.Range(0, 5) == 0;
        fixedObstacles = new int[] { Random.Range(0, 2), Random.Range(2, 4), Random.Range(4, 6) };
    }
    public override void Generate(bool hasFood)
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
                Debug.Log(grid[i, j]);
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
