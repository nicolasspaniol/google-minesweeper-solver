using System;
using System.Drawing;
using System.Collections.Generic;

namespace GoogleMinesweeperSolver {
  class MinesweeperSolver {
    IMinesweeperGame _game;

    public MinesweeperSolver(IMinesweeperGame game) {
      this._game = game;
    }

    public void Solve() {

      // Make sure the game is not completely hidden
      InitiateGame();

      // In the current implementation, as there is no way to know the number
      // of mines left, it is hard to know exactly when the game is over.
      // Therefore, the solver will keep running as long as it is capable of

      for (int iteration = 1;; iteration++) {

        // Each time a cell is read from the game, if its state has changed
        // since the last read, it will wait for the animations to go away and
        // then capture the screen again. We batch the 'open' and 'flag' calls
        // to avoid the slowdown that updating and reading constantly can cause.
        // This problem is specific to the google implementation of the game
        
        var cellsToOpen = new HashSet<Point>();
        var cellsToFlag = new HashSet<Point>();

        foreach (Point pos in Cells()) {
          var cell = _game.ReadCell(pos.X, pos.Y);

          if (IsNumber(cell)) {

            // Get the amount of hidden and flagged neighbors
            int hiddenNeighbors = 0, flaggedNeighbors = 0;
            foreach (Point nb in Neighbors(pos)) {
              if (_game.ReadCell(nb.X, nb.Y) == CellState.Hidden) {
                hiddenNeighbors++;
              }
              else if (_game.ReadCell(nb.X, nb.Y) == CellState.Flagged) {
                flaggedNeighbors++;
              }
            }

            int value = (int) cell;

            // If the sum of the two is equal to the cell's value, flag all
            // hidden neighbors
            if (value == hiddenNeighbors + flaggedNeighbors) {
              foreach (var nb in Neighbors(pos)) {
                if (_game.ReadCell(nb.X, nb.Y) == CellState.Hidden) {
                  cellsToFlag.Add(nb);
                }
              }
            }

            // If the amount of flagged neighbors is equal to the cell's value,
            // open all hidden neighbors
            else if (value == flaggedNeighbors) {
              foreach (var nb in Neighbors(pos)) {
                if (_game.ReadCell(nb.X, nb.Y) == CellState.Hidden) {
                  cellsToOpen.Add(nb);
                }
              }
            }
          }
        }

        foreach (Point p in cellsToFlag) {
          _game.FlagCell(p.X, p.Y);
        }
        foreach (Point p in cellsToOpen) {
          _game.OpenCell(p.X, p.Y);
        }

        if (cellsToOpen.Count + cellsToFlag.Count == 0) {
          return;
        }

        Console.WriteLine(
          "Iteration #{0}: {1} cells opened, {2} cells flagged", 
          iteration, cellsToOpen.Count, cellsToFlag.Count
        );
      }
    }

    void InitiateGame() {
      bool isNew = true;

      for (int x = 0; x < _game.GetWidth(); x++) {
        for (int y = 0; y < _game.GetHeight(); y++) {
          if (_game.ReadCell(x, y) != CellState.Hidden) {
            isNew = false;
            break;
          }
        }

        if (!isNew) break;
      }

      // If the game has not started yet, open cell around the middle
      if (isNew) {
        _game.OpenCell(_game.GetWidth() >> 1, _game.GetHeight() >> 1);
      }
    }

    bool IsNumber(CellState state) {
      return state >= CellState.One && state <= CellState.Eight;
    }

    IEnumerable<Point> Cells() {
      for (int x = 0; x < _game.GetWidth(); x++) {
        for (int y = 0; y < _game.GetHeight(); y++) {
          yield return new Point(x, y);
        }
      }
    }
    IEnumerable<Point> Neighbors(Point p) {
      for (int x = p.X - 1; x <= p.X + 1; x++) {
        for (int y = p.Y - 1; y <= p.Y + 1; y++) {
          if (
              x == p.X && y == p.Y ||           // Exclude middle cell
              x < 0 || x >= _game.GetWidth() || // Exclude out of bounds X
              y < 0 || y >= _game.GetHeight()   // Exclude out of bounds Y
            ) continue;
          
          yield return new Point(x, y);
        }
      }
    }
  }
}