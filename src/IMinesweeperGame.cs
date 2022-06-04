namespace GoogleMinesweeperSolver {
  interface IMinesweeperGame {
    public CellState ReadCell(int x, int y);
    public void OpenCell(int x, int y);
    public void FlagCell(int x, int y);

    public int GetWidth();
    public int GetHeight();
  }
}