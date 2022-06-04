using System;
using System.Drawing;

namespace GoogleMinesweeperSolver {
  class Program {
    static void Main(string[] args) {
      Console.WriteLine("Google minesweeper solver:\n\nPress any key to start...");
      Console.ReadLine();

      RunGame();
      
      Console.Write("\nApplication exiting");
      Console.ReadLine();
      return;
    }

    static void RunGame() {
      var game = new GoogleMinesweeper();
      var solver = new MinesweeperSolver(game);

      Console.WriteLine("Game started");
      solver.Solve();
      Console.WriteLine("Game finished");
    }
  }
}