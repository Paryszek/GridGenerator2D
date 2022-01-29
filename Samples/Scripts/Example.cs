using UnityEngine;
using MParysz.ProceduralGridGenerator2D;

internal enum ProceduralGridGenerator2DType {
  CELLULAR_AUTOMATA,
  AGENTS
}

public class Example : MonoBehaviour {
  [Header("Input")]
  [SerializeField] ProceduralGridGenerator2DType generationType = ProceduralGridGenerator2DType.CELLULAR_AUTOMATA;
  [SerializeField] int roomHight = 20;
  [SerializeField] int roomWidth = 20;

  [Header("References")]
  [SerializeField] GameObject emptySquare;
  [SerializeField] GameObject floorSquare;

  private ProceduralGridGeneratorBase generator;

  private void Awake() {
    switch (generationType) {
      case ProceduralGridGenerator2DType.CELLULAR_AUTOMATA:
        generator = new ProceduralGridGeneratorCellularAutomata(roomWidth, roomHight);
        break;
      case ProceduralGridGenerator2DType.AGENTS:
        generator = new ProceduralGridGeneratorAgents(roomWidth, roomHight);
        break;
    }
  }

  void Start() {
    var grid = generator.GenerateGrid();

    for (var i = 0; i < roomWidth; i++) {
      for (var j = 0; j < roomHight; j++) {
        switch (grid[i, j]) {
          case SquareType.EMPTY:
            Instantiate(emptySquare, new Vector2(i, j), Quaternion.identity);
            break;
          case SquareType.FLOOR:
            Instantiate(floorSquare, new Vector2(i, j), Quaternion.identity);
            break;
        }
      }
    }
  }
}
