using System.Collections.Generic;
using UnityEngine;

namespace MParysz.ProceduralGridGenerator2D {
  internal class Agent {
    public Vector2 position;
    public Vector2 direction;
  }

  public class ProceduralGridGeneratorAgents : ProceduralGridGeneratorBase {
    private int maxAgents = 20;
    private float fillPercentage = 0.4f;
    private float changeDirectionChance = 0.5f;
    private float addNewAgentChance = 0.05f;
    private float removeAgentChance = 0.05f;
    private readonly int maxIterations = 100000;

    SquareType[,] grid;
    List<Agent> agents;

    public ProceduralGridGeneratorAgents(int roomWidth, int roomHight) : base(roomWidth, roomHight) { }
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
      grid = new SquareType[roomWidth, roomHight];

      for (var i = 0; i < roomWidth; i++) {
        for (var j = 0; j < roomHight; j++) {
          grid[i, j] = SquareType.FILL;
        }
      }

      agents = new List<Agent>();

      var agent = new Agent();
      agent.position = new Vector2(Mathf.FloorToInt(roomWidth / 2), Mathf.FloorToInt(roomHight / 2));
      agent.direction = GetRandomDirection();
      agents.Add(agent);
    }

    private Vector2 GetRandomPosition() {
      return new Vector2(Random.Range(0, roomWidth - 1), Random.Range(0, roomHight - 1));
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
          agent.position.y = Mathf.Clamp(agent.position.y, 0, roomHight - 1);

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

          var newAgent = new Agent();
          newAgent.direction = GetRandomDirection();
          newAgent.position = agents[i].position;
          agents.Add(newAgent);
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
        for (var j = 0; j < roomHight; j++) {
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
  }
}
