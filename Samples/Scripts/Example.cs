using UnityEngine;
using MParysz.ProceduralGridGenerator2D;
using UnityEngine.UI;

internal enum ProceduralGridGenerator2DType {
  CELLULAR_AUTOMATA,
  AGENTS
}

public class Example : MonoBehaviour {
  [Header("Input")]
  [SerializeField] private ProceduralGridGenerator2DType generationType = ProceduralGridGenerator2DType.CELLULAR_AUTOMATA;
  [SerializeField] private int roomHight = 20;
  [SerializeField] private int roomWidth = 20;

  [Header("References")]
  [SerializeField] private GameObject emptySquare;
  [SerializeField] private GameObject floorSquare;
  [SerializeField] private GameObject squareParent;
  [SerializeField] private Button generateButton;

  private ProceduralGridGeneratorBase generator;

  private void Awake() {
    generateButton.onClick.AddListener(() => Generate());
  }

  private void Generate() {
    CleanSquareParent();
    PickGenerationType();

    var grid = generator.GenerateGrid();

    CreateGrid(grid);
  }

  private void CreateGrid(SquareType[,] grid) {
    for (var i = 0; i < roomWidth; i++) {
      for (var j = 0; j < roomHight; j++) {
        GameObject square = null;

        switch (grid[i, j]) {
          case SquareType.EMPTY:
            square = Instantiate(emptySquare, new Vector2(i, j), Quaternion.identity);
            break;
          case SquareType.FLOOR:
            square = Instantiate(floorSquare, new Vector2(i, j), Quaternion.identity);
            break;
        }

        if (square == null) {
          continue;
        }

        square.transform.SetParent(squareParent.transform);
      }
    }
  }

  private void PickGenerationType() {
    switch (generationType) {
      case ProceduralGridGenerator2DType.CELLULAR_AUTOMATA:
        generator = new ProceduralGridGeneratorCellularAutomata(roomWidth, roomHight);
        break;
      case ProceduralGridGenerator2DType.AGENTS:
        generator = new ProceduralGridGeneratorAgents(roomWidth, roomHight);
        break;
    }
  }

  private void CleanSquareParent() {
    foreach (Transform child in squareParent.transform) {
      Destroy(child.gameObject);
    }
  }
}
