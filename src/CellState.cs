namespace GoogleMinesweeperSolver {
  // Notice the 'mine' state is not included in the enum. In the google version 
  // of the game, it is hard to tell if a cell is a mine or not, so it is
  // assumed the cell will never become a mine if the solver works correctly
  
  enum CellState {
    Unknown,
    Hidden = -2,
    Flagged,
    Zero,

    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight
  }
}