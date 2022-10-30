using System.Collections.Generic;
using UnityEngine;

namespace MParysz.ProceduralGridGenerator2D {
  internal class Agent {
    public Vector2 position;
    public Vector2 direction;
  }

  public class ProceduralGridGeneratorAgents : ProceduralGridGeneratorBase {
    private int maxAgents = 20;
    private float fillPercentage = 0.5f;
    private float changeDirectionChance = 0.7f;
    private float addNewAgentChance = 0.25f;
    private float removeAgentChance = 0.1f;
    private readonly int maxIterations = 1000000;
    private int currentCornerIndex = -1;

    SquareType[,] grid;
    List<Agent> agents;

    public ProceduralGridGeneratorAgents(int roomWidth, int roomHight) : base(roomWidth, roomHight) { }
    public ProceduralGridGeneratorAgents(int roomWidth, int roomHight, int maxAgents) : base(roomWidth, roomHight) {
      this.maxAgents = maxAgents;
    }
    public ProceduralGridGeneratorAgents(int roomWidth, int roomHight, int maxAgents, float fillPercentage, float changeDirectionChance, float addNewAgentChance, float removeAgentChance) : base(roomHight, roomWidth) {
      this.maxAgents = maxAgents;
      this.fillPercentage = fillPercentage;
      this.changeDirectionChance = changeDirectionChance;
      this.addNewAgentChance = addNewAgentChance;
      this.removeAgentChance = removeAgentChance;
    }

    public override SquareType[,] GenerateGrid() {
      Setup();
      Generate();

      return grid;
    }

    public override SquareType[,] NextIteration() {
      if (grid == null) {
        return GenerateGrid();
      }

      return grid;
    }

    private void Setup() {
      grid = new SquareType[roomWidth, roomHeight];

      for (var i = 0; i < roomWidth; i++) {
        for (var j = 0; j < roomHeight; j++) {
          grid[i, j] = SquareType.FILL;
        }
      }

      agents = new List<Agent>();

      var agent = new Agent();
      agent.position = new Vector2(Mathf.FloorToInt(roomWidth / 2), Mathf.FloorToInt(roomHeight / 2));
      agent.direction = GetRandomDirection();
      agents.Add(agent);
    }

    private void Generate() {
      int iteration = 0;

      do {
        foreach (var agent in agents) {
          if (grid[(int)agent.position.x, (int)agent.position.y] == SquareType.EMPTY) {
            continue;
          }

          grid[(int)agent.position.x, (int)agent.position.y] = SquareType.EMPTY;
        }

        foreach (var agent in agents) {
          agent.position += agent.direction;

          agent.position.x = Mathf.Clamp(agent.position.x, 0, roomWidth - 1);
          agent.position.y = Mathf.Clamp(agent.position.y, 0, roomHeight - 1);

          if (Random.value < changeDirectionChance) {
            agent.direction = GetRandomDirection();
          }
        }

        var numberOfChecks = agents.Count;
        for (var i = 0; i < numberOfChecks; i++) {
          if (Random.value > removeAgentChance || agents.Count <= 1) {
            continue;
          }

          agents.RemoveAt(i);
          break;
        }

        numberOfChecks = agents.Count;
        for (var i = 0; i < numberOfChecks; i++) {
          if (Random.value > addNewAgentChance || agents.Count >= maxAgents) {
            continue;
          }

          agents.Add(CreateAgent());
          break;
        }

        var floorFillPercentageValue = (float)GetNumberOfFloors() / (float)grid.Length;

        if (floorFillPercentageValue >= fillPercentage) {
          break;
        }

        iteration++;
      } while (iteration < maxIterations);
    }

    private int GetNumberOfFloors() {
      var floors = 0;

      for (var i = 0; i < roomWidth; i++) {
        for (var j = 0; j < roomHeight; j++) {
          if (grid[i, j] != SquareType.EMPTY) {
            continue;
          }

          floors++;
        }
      }

      return floors;
    }

    private Vector2 GetRandomDirection() {
      var rand = Random.Range(1, 5);

      switch (rand) {
        case 1:
          return Vector2.up;
        case 2:
          return Vector2.left;
        case 3:
          return Vector2.down;
        default:
          return Vector2.right;
      }
    }

    private Agent CreateAgent() {
      Vector2Int corner = Vector2Int.zero;

      currentCornerIndex++;
      if (currentCornerIndex == 4) {
        currentCornerIndex = 0;
      }

      switch (currentCornerIndex) {
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
      agent.position = new Vector2(Random.Range(boundryWidth.x, boundryWidth.y), Random.Range(boundryHeight.x, boundryHeight.y));
      agent.direction = GetRandomDirection();

      return agent;
    }

    private (Vector2Int, Vector2Int) GetBoundry(Vector2Int corner) {
      var width = this.roomWidth;
      var height = this.roomHeight;

      var topHeight = new Vector2Int(Mathf.CeilToInt(height / 2), height);
      var bottomHeigh = new Vector2Int(0, Mathf.CeilToInt(height / 2));
      var rightWidth = new Vector2Int(Mathf.CeilToInt(width / 2), width);
      var leftWidth = new Vector2Int(0, Mathf.CeilToInt(width / 2));

      if (corner == new Vector2Int(1, 1)) {
        return (rightWidth, topHeight);
      } else if (corner == new Vector2Int(0, 0)) {
        return (leftWidth, bottomHeigh);
      } else if (corner == new Vector2Int(0, 1)) {
        return (leftWidth, topHeight);
      } else if (corner == new Vector2Int(1, 0)) {
        return (rightWidth, bottomHeigh);
      }

      return (new Vector2Int(0, 0), new Vector2Int(width, height));
    }
  }
}
