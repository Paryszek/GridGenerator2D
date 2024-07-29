using UnityEngine;

namespace MParysz.ProceduralGridGenerator2D
{
  public class ProceduralGridGeneratorCellularAutomata : ProceduralGridGeneratorBase
  {
    private float _noiseDensity = 0.45f;
    private int _iterations = 4;
    private SquareType[,] _grid;

    public ProceduralGridGeneratorCellularAutomata(int roomWidth, int roomHeight) : base(roomWidth, roomHeight) { }
    public ProceduralGridGeneratorCellularAutomata(int roomWidth, int roomHeight, int iterations, float noiseDensity) : base(roomWidth, roomHeight)
    {
      this._iterations = iterations;
      this._noiseDensity = noiseDensity;
    }

    public override SquareType[,] GenerateGrid()
    {
      Setup();
      GenerateNoise();
      CellularAutomata();

      return _grid;
    }

    public override SquareType[,] NextIteration()
    {
      if (_grid == null)
      {
        return GenerateGrid();
      }

      CellularAutomata();

      return _grid;
    }

    private void Setup()
    {
      _grid = new SquareType[roomWidth, roomHeight];
    }

    private void GenerateNoise()
    {
      for (var i = 0; i < roomWidth; i++)
      {
        for (var j = 0; j < roomHeight; j++)
        {
          if (Random.value > _noiseDensity)
          {
            _grid[i, j] = SquareType.FILL;
            continue;
          }

          _grid[i, j] = SquareType.EMPTY;
        }
      }
    }

    private void CellularAutomata()
    {
      for (int iteration = 0; iteration < _iterations; iteration++)
      {
        SquareType[,] tempGrid = (SquareType[,])_grid.Clone();

        for (int i = 0; i < roomWidth; i++)
        {
          for (int j = 0; j < roomHeight; j++)
          {
            var fillNumber = 0;
            var border = false;

            for (int k = i - 1; k <= i + 1; k++)
            {
              for (int l = j - 1; l <= j + 1; l++)
              {
                if (k >= 0 && l >= 0 && k < roomWidth && l < roomHeight)
                {
                  if (k != i || l != j)
                  {
                    if (tempGrid[k, l] == SquareType.FILL)
                    {
                      fillNumber++;
                    }
                  }
                }
                else
                {
                  border = true;
                }
              }
            }

            if (fillNumber > 4 || border)
            {
              _grid[i, j] = SquareType.FILL;
            }
            else
            {
              _grid[i, j] = SquareType.EMPTY;
            }
          }
        }
      }
    }
  }
}
