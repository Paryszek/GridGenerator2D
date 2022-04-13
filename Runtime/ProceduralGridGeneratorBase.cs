
namespace MParysz.ProceduralGridGenerator2D {
  public enum SquareType {
    EMPTY,
    FILL
  }

  public abstract class ProceduralGridGeneratorBase {
    protected int roomWidth;
    protected int roomHight;

    public ProceduralGridGeneratorBase(int roomWidth, int roomHight) {
      this.roomWidth = roomWidth;
      this.roomHight = roomHight;
    }

    public abstract SquareType[,] GenerateGrid();
    public abstract SquareType[,] NextIteration();
  }
}