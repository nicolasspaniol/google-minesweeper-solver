using System;
using System.Drawing;

namespace GoogleMinesweeperSolver
{
  static class ExtensionMethods {
    public static int DifferenceTo(this Color self, Color color) {
      int diffR = Math.Abs(self.R - color.R);
      int diffG = Math.Abs(self.G - color.G);
      int diffB = Math.Abs(self.B - color.B);

      return diffR + diffG + diffB;
    }
  }
}