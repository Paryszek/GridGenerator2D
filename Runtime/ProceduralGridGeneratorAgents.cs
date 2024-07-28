using System.Collections.Generic;
using UnityEngine;

namespace MParysz.ProceduralGridGenerator2D
{
  internal class Agent
  {
    public Vector2Int position;
    public Vector2Int direction;
  }

  public class ProceduralGridGeneratorAgents : ProceduralGridGeneratorBase
  {
    private SquareType[,] grid;
    private List<Agent> agents;
    private bool addBorder;
    private bool removeSingleFillSquares;
    private readonly int maxIterations = 1000000;
    private int maxAgents = 20;
    private int currentCornerIndex = -1;
    private int numberOfEmptySquares;
    private float emptySquaresPercentage = 0.55f;
    private float changeDirectionChance = 0.7f;
    private float addNewAgentChance = 0.3f;
    private float removeAgentChance = 0.1f;

    public ProceduralGridGeneratorAgents(int roomWidth, int roomHeight) : base(roomWidth, roomHeight) { }
    public ProceduralGridGeneratorAgents(
      int roomWidth,
      int roomHeight,
      int maxAgents,
      bool addBorder = true,
      bool removeSingleFillSquares = false
    ) : base(roomWidth, roomHeight)
    {
      this.maxAgents = maxAgents;
      this.addBorder = addBorder;
      this.removeSingleFillSquares = removeSingleFillSquares;
    }
    public ProceduralGridGeneratorAgents(
      int roomWidth,
      int roomHeight,
      int maxAgents,
      float emptySquaresPercentage,
      float changeDirectionChance,
      float addNewAgentChance,
      float removeAgentChance,
      bool addBorder = true,
      bool removeSingleFillSquares = false
    ) : base(roomHeight, roomWidth)
    {
      this.maxAgents = maxAgents;
      this.addBorder = addBorder;
      this.emptySquaresPercentage = emptySquaresPercentage;
      this.changeDirectionChance = changeDirectionChance;
      this.addNewAgentChance = addNewAgentChance;
      this.removeAgentChance = removeAgentChance;
      this.removeSingleFillSquares = removeSingleFillSquares;
    }

    public override SquareType[,] GenerateGrid()
    {
      Setup();
      Generate();
      RemoveSingleWalls();
      AddBorder();

      return this.grid;
    }

    public override SquareType[,] NextIteration()
    {
      if (this.grid == null)
      {
        return GenerateGrid();
      }

      return this.grid;
    }

    private void Setup()
    {
      this.grid = new SquareType[roomWidth, roomHeight];

      for (var i = 0; i < roomWidth; i++)
      {
        for (var j = 0; j < roomHeight; j++)
        {
          this.grid[i, j] = SquareType.FILL;
        }
      }

      this.agents = new List<Agent>();

      var agent = new Agent();
      agent.position = new Vector2Int(Mathf.FloorToInt(roomWidth / 2), Mathf.FloorToInt(roomHeight / 2));
      agent.direction = GetRandomDirection();
      this.agents.Add(agent);
    }

    private void Generate()
    {
      int iteration = 0;

      do
      {
        foreach (var agent in this.agents)
        {
          if (this.grid[agent.position.x, agent.position.y] == SquareType.EMPTY)
          {
            continue;
          }

          this.grid[agent.position.x, agent.position.y] = SquareType.EMPTY;
          this.numberOfEmptySquares++;
        }

        foreach (var agent in this.agents)
        {
          agent.position += agent.direction;

          agent.position.x = Mathf.Clamp(agent.position.x, 0, roomWidth - 1);
          agent.position.y = Mathf.Clamp(agent.position.y, 0, roomHeight - 1);

          if (Random.value < this.changeDirectionChance)
          {
            agent.direction = GetRandomDirection();
          }
        }

        for (var i = 0; i < this.agents.Count; i++)
        {
          if (Random.value > this.removeAgentChance || this.agents.Count <= 1)
          {
            continue;
          }

          agents.RemoveAt(i);
          break;
        }

        for (var i = 0; i < this.agents.Count; i++)
        {
          if (Random.value > this.addNewAgentChance || this.agents.Count >= this.maxAgents)
          {
            continue;
          }

          this.agents.Add(CreateAgent());
          break;
        }

        var emptySquaresPercentageValue = (float)this.numberOfEmptySquares / (float)this.grid.Length;

        if (emptySquaresPercentageValue >= emptySquaresPercentage)
        {
          break;
        }

        iteration++;
      } while (iteration < maxIterations);
    }

    private void RemoveSingleWalls()
    {
      if (!removeSingleFillSquares)
      {
        return;
      }

      for (int row = 0; row < this.roomWidth - 1; row++)
      {
        for (int col = 0; col < this.roomHeight - 1; col++)
        {
          var cell = this.grid[row, col];

          if (cell != SquareType.FILL)
          {
            continue;
          }

          var allEmptySquares = true;

          for (int checkRow = -1; checkRow <= 1; checkRow++)
          {
            for (int checkCol = -1; checkCol <= 1; checkCol++)
            {
              if (checkRow + row < 0 ||
                checkRow + row > this.roomWidth - 1 ||
                checkCol + col < 0 ||
                checkCol + col > this.roomHeight - 1)
              {
                continue;
              }

              if ((checkRow == 0 && checkCol == 0))
              {
                continue;
              }

              if (grid[row + checkRow, col + checkCol] == SquareType.FILL)
              {

                allEmptySquares = false;
              }
            }
          }

          if (allEmptySquares)
          {
            this.grid[row, col] = SquareType.EMPTY;
          }
        }
      }
    }

    private void AddBorder()
    {
      if (!addBorder)
      {
        return;
      }

      for (var i = 0; i < this.roomWidth; i++)
      {
        for (var j = 0; j < this.roomHeight; j++)
        {
          if (i == 0 || j == 0 || i == this.roomWidth - 1 || j == this.roomHeight - 1)
          {
            this.grid[i, j] = SquareType.FILL;
          }
        }
      }
    }


    private Agent CreateAgent()
    {
      Vector2Int corner = Vector2Int.zero;

      currentCornerIndex++;
      if (currentCornerIndex == 4)
      {
        currentCornerIndex = 0;
      }

      switch (currentCornerIndex)
      {
        case 0:
          corner = new Vector2Int(0, 0);
          break;
        case 1:
          corner = new Vector2Int(0, 1);
          break;
        case 2:
          corner = new Vector2Int(1, 1);
          break;
        case 3:
          corner = new Vector2Int(1, 0);
          break;

      }

      var (boundryWidth, boundryHeight) = GetBoundry(corner);

      var agent = new Agent();
      agent.position = new Vector2Int(Random.Range(boundryWidth.x, boundryWidth.y), Random.Range(boundryHeight.x, boundryHeight.y));
      agent.direction = GetRandomDirection();

      return agent;
    }

    private (Vector2Int, Vector2Int) GetBoundry(Vector2Int corner)
    {
      var width = this.roomWidth;
      var height = this.roomHeight;

      var topHeight = new Vector2Int(Mathf.CeilToInt(height / 2), height);
      var bottomHeigh = new Vector2Int(0, Mathf.CeilToInt(height / 2));
      var rightWidth = new Vector2Int(Mathf.CeilToInt(width / 2), width);
      var leftWidth = new Vector2Int(0, Mathf.CeilToInt(width / 2));

      if (corner == new Vector2Int(1, 1))
      {
        return (rightWidth, topHeight);
      }
      else if (corner == new Vector2Int(0, 0))
      {
        return (leftWidth, bottomHeigh);
      }
      else if (corner == new Vector2Int(0, 1))
      {
        return (leftWidth, topHeight);
      }
      else if (corner == new Vector2Int(1, 0))
      {
        return (rightWidth, bottomHeigh);
      }

      return (new Vector2Int(0, 0), new Vector2Int(width, height));
    }

    private Vector2Int GetRandomDirection()
    {
      var rand = Random.Range(1, 5);

      switch (rand)
      {
        case 1:
          return Vector2Int.up;
        case 2:
          return Vector2Int.left;
        case 3:
          return Vector2Int.down;
        default:
          return Vector2Int.right;
      }
    }
  }
}
