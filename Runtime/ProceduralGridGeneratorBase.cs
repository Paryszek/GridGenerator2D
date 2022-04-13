
namespace MParysz.ProceduralGridGenerator2D {
  public enum SquareType {
    EMPTY,
    FLOOR
  }

  public abstract class ProceduralGridGeneratorBase {
    protected int roomHight;
    protected int roomWidth;

    public ProceduralGridGeneratorBase(int roomHight, int roomWidth) {
      this.roomHight = roomHight;
      this.roomWidth = roomWidth;
    }

    public abstract SquareType[,] GenerateGrid();
    public abstract SquareType[,] NextIteration();
  }
}