# google-minesweeper-solver

A basic minesweeper algorithm implementation along with an interface for it to interact with [Google's Minesweeper game](https://www.google.com/search?q=google+minesweeper). 

## Usage

The program can by compiled with .NET Core 3.1 by running `dotnet build` inside the root folder.

Using it requires Google Minesweeper to be open and fully visible on the screen, since the program interacts with the game by taking captures of the screen and moving the user's cursor to the desired locations. The console window should not be covering the game. And lastly, for your ear's sake, turn off the sounds before starting the application!

Uses Windows API (WinAPI) to control the cursor and to take captures of the screen, so the program will not work on any other OS.
