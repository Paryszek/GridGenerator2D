
namespace MParysz.ProceduralGridGenerator2D {
  public enum SquareType {
    EMPTY,
    FILL
  }

  public abstract class ProceduralGridGeneratorBase {
    protected int roomWidth;
    protected int roomHeight;

    public ProceduralGridGeneratorBase(int roomWidth, int roomHeight) {
      this.roomWidth = roomWidth;
      this.roomHeight = roomHeight;
    }

    public abstract SquareType[,] GenerateGrid();
    public abstract SquareType[,] NextIteration();
  }
}