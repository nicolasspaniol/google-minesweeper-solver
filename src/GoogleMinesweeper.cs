using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

namespace GoogleMinesweeperSolver {
  class GoogleMinesweeper : IMinesweeperGame {
    static readonly int s_colorDiffThreshold = 10;
    static readonly int s_cellPadding = 5;
    static readonly CellState[] s_cellColorMap = new CellState[] {
      CellState.Hidden, CellState.Hidden,
      CellState.Flagged, CellState.Zero, 
      CellState.Zero, CellState.One, 
      CellState.Two, CellState.Three, 
      CellState.Four, CellState.Five, 
      CellState.Six, CellState.Seven, 
      CellState.Eight
    };

    Rectangle _gameRect = Rectangle.Empty;
    float _CellSide;
    Size _gridSize;
    Color[] _tilemapColors;
    bool _hasGameChanged = false;
    Bitmap _gameCapture;
    Graphics _graph;
    CellState?[,] _CellCache;

    public GoogleMinesweeper() {
      
      // Load tilemap colors
      using (var tilemap = new Bitmap("tilemap.png")) {
        _tilemapColors = new Color[tilemap.Width];
        for (int i = 0; i < tilemap.Width; i++) {
          _tilemapColors[i] = tilemap.GetPixel(i, 0);
        }
      }

      Size screenResolution = new Size(
        WinApi.GetSystemMetrics(0), 
        WinApi.GetSystemMetrics(1)
      );
      
      // Make sure the cursor is not covering the game
      WinApi.SetCursorPos(0, 0);
      using (Bitmap image = new Bitmap(
        screenResolution.Width, 
        screenResolution.Height
      )) {
        // Capture all the screen
        Graphics graph = Graphics.FromImage(image);
        graph.CopyFromScreen(Point.Empty, Point.Empty, screenResolution);

        // Find game top left corner based on the color of the pixels
        bool found = false;
        for (int y = 0; y < screenResolution.Height && !found; y++) {
          for (int x = 0; x < screenResolution.Width && !found; x++) {
            Color pixelColor = image.GetPixel(x, y);

            if (
              pixelColor.DifferenceTo(_tilemapColors[1]) < 5 || 
              pixelColor.DifferenceTo(_tilemapColors[3]) < 5
            ) {
              _gameRect.Location = new Point(x, y);
              found = true;
            }
          }
        }

        // Find game bottom right corner
        found = false;
        for (int y = screenResolution.Height - 1; y >= 0 && !found; y--) {
          for (int x = screenResolution.Width - 1; x >= 0 && !found; x--) {
            Color pixelColor = image.GetPixel(x, y);

            if (
              pixelColor.DifferenceTo(_tilemapColors[1]) < 5 || 
              pixelColor.DifferenceTo(_tilemapColors[3]) < 5
            ) {
              _gameRect.Size = new Size(
                x - _gameRect.X + 1, 
                y - _gameRect.Y + 1
              );
              found = true;
            }
          }
        }

        // Identify problems on game detection
        if (_gameRect.IsEmpty) {
          throw new Exception("Program failed to identify game rectangle");
        }
      
        // Find Cell size
        int gridWidth = 1;
        Color currentColor = image.GetPixel(_gameRect.X, _gameRect.Y);
        for (int i = 0; i < _gameRect.Width; i++) {
          Color c = image.GetPixel(_gameRect.X + i, _gameRect.Y);
          if (c != currentColor) {
            gridWidth++;
            i += 5;
            currentColor = image.GetPixel(_gameRect.X + i, _gameRect.Y);
          }
        }
        
        _CellSide = (float) (_gameRect.Width) / gridWidth;
        _gridSize = new Size(
          gridWidth, 
          (int) Math.Round((_gameRect.Height) / _CellSide)
        );
        _CellCache = new CellState?[_gridSize.Width, _gridSize.Height];
      }

      // Take initial game capture
      _gameCapture = new Bitmap(_gameRect.Width, _gameRect.Height);
      _graph = Graphics.FromImage(_gameCapture);
      _graph.CopyFromScreen(_gameRect.Location, Point.Empty, _gameRect.Size);
    }

    void CaptureGame() {
      // Make sure the cursor is not covering the game
      WinApi.SetCursorPos(0, 0);
      
      // Make sure there is no animation playing
      Thread.Sleep(850);

      _graph.CopyFromScreen(_gameRect.Location, Point.Empty, _gameRect.Size);
      _hasGameChanged = false;
    }

    CellState ReadCellFromPixels(int x, int y) {
      if (_hasGameChanged) {
        CaptureGame();
      }

      Point pos = new Point((int) (x * _CellSide), (int) (y * _CellSide));

      int cellInnerWidth = (int) (_CellSide - s_cellPadding * 2);
      int cellMiddleHeight = (int) _CellSide >> 1;

      for (int i = _tilemapColors.Length - 1; i >= 0; i--) {
        Color tilemapColor = _tilemapColors[i];

        for (int lx = 0; lx < cellInnerWidth; lx++) {
          Color pixelColor = _gameCapture.GetPixel(
            pos.X + lx + s_cellPadding,
            pos.Y + cellMiddleHeight
          );
          if (pixelColor.DifferenceTo(tilemapColor) < s_colorDiffThreshold) {
            return s_cellColorMap[i];
          }
        }
      }

      return CellState.Unknown;
    }

    public CellState ReadCell(int x, int y) {
      CellState? cache = _CellCache[x, y];
      if (cache.HasValue) {
        return cache.Value;
      }

      CellState state = ReadCellFromPixels(x, y);

      if (state >= CellState.Zero) {
        _CellCache[x, y] = state;
      }
      return state;
    }
    
    void MoveMouseToCell(int x, int y) {
      WinApi.SetCursorPos(
        (int) (_gameRect.X + (x + .5) * _CellSide), 
        (int) (_gameRect.Y + (y + .5) * _CellSide)
      );
    }
    public void OpenCell(int x, int y) {
      _hasGameChanged = true;
      MoveMouseToCell(x, y);
      WinApi.MouseLeftClick();
      Thread.Sleep(20);
    }
    public void FlagCell(int x, int y) {
      switch (ReadCell(x, y))
      {
        case CellState.Hidden:
          _CellCache[x, y] = CellState.Flagged;
          break;

        case CellState.Flagged:
          _CellCache[x, y] = CellState.Hidden;
          break;
          
        default:
          return;
      }
      MoveMouseToCell(x, y);
      WinApi.MouseRightClick();
      Thread.Sleep(20);
    }

    public int GetHeight() {
      return _gridSize.Height;
    }
    public int GetWidth() {
      return _gridSize.Width;
    }
  
    ~GoogleMinesweeper() {
      _graph.Dispose();
      _gameCapture.Dispose();
    }
  }

  static class WinApi {
    [DllImport("user32.dll")]
    public static extern int GetSystemMetrics(int smIndex);

    [DllImport("User32")]
    public extern static void SetCursorPos(int x, int y);

    [StructLayout(LayoutKind.Sequential)]
    public struct MousePoint
    {
      public int X;
      public int Y;

      public MousePoint(int x, int y)
      {
        X = x;
        Y = y;
      }
    }

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out MousePoint lpMousePoint);

    [DllImport("user32.dll")]
    static extern void mouse_event(
      int dwFlags, 
      int dx, int dy, 
      int dwData, 
      int dwExtraInfo
    );

    public static void MouseLeftClick() {
      GetCursorPos(out MousePoint pos);
      mouse_event(0x02 | 0x04, pos.X, pos.Y, 0, 0);
    }
    public static void MouseRightClick() {
      GetCursorPos(out MousePoint pos);
      mouse_event(0x08 | 0x10, pos.X, pos.Y, 0, 0);
    }
  }
}