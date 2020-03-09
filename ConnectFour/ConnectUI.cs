using ConnectFour.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectFour.UI
{
    public class ConnectUI
    {
        private IConnectGame _game;
        private Player _playerUser = Player.PlayerOne;
        private Player _playerCPU = Player.PlayerTwo;

        /// <summary>
        /// Instantiate standard game instance
        /// </summary>
        public void Init()
        {
            _game = GameManager.Instance().CreateRegularConnectFourGame();
        }

        /// <summary>
        /// Start game loop
        /// </summary>
        public void Start()
        {
            // Restart and draw connect four board
            _game.Restart();
            Draw();

            // Game loop, repeat while game is in play
            while (_game.WhoIsWinner == null && _game.WhosInPlay != null)
            {
                String choice = null;

                // Get column choice from user
                while (String.IsNullOrWhiteSpace(choice))
                {
                    Console.Write("Enter column number (1 to " + _game.Columns + "), Q to quit or R to Restart? ");
                    choice = Console.ReadLine().ToUpper();
                }

                // If the user chooses to quit then stop the game
                if (choice.StartsWith("Q"))
                    _game.Stop();

                // If the user chooses to retart then restart the game
                else if (choice.StartsWith("R"))
                {
                    _game.Restart();
                    Draw();
                }

                // Otherwise assume user has chosen a column
                else
                {
                    // Evaluate if column number
                    uint selectedColumn = 0;
                    if (uint.TryParse(choice, out selectedColumn))
                    {
                        // Trying adding the user's disc and catch/display any exception
                        try
                        {
                            _game.AddDisc(selectedColumn);
                        }
                        catch (Exception e)
                        {
                            Console.Write(e.Message + ". Press enter key to continue.");
                            Console.ReadLine();
                        }

                        // If player is CPU then add CPU's disk based on the recommendation
                        if (_game.WhoIsWinner == null && _game.WhosInPlay == _playerCPU)
                            _game.AddDisc(_game.RecommendColumn);

                        // Redraw connect four board showing new additions
                        Draw();

                        // Are we nolonger in play?
                        if (_game.WhoIsWinner != null || _game.WhosInPlay == null)
                        {
                            // Display winning status
                            if (_game.WhoIsWinner != null)
                                Console.Write((_game.WhoIsWinner == _playerUser ? "User" : "CPU") + " is the winner. ");
                            else
                                Console.Write("no winner this time. ");

                            // Does user wish to play again.  If yes restart, otherwise will fallthrough
                            Console.Write("Another game, enter (Y)es or (N)o? ");
                            choice = Console.ReadLine().ToUpper();
                            if (choice.StartsWith("Y"))
                            {
                                _game.Restart();
                                Draw();
                            }
                        }
                    }
                }
            }

            // Retain details on screen until enter key pressed
            Console.Write("Press enter key to continue.");
            Console.ReadLine();
        }

        /// <summary>
        /// Draw Connect Four board
        /// </summary>
        private void Draw()
        {
            // Clear screen
            Console.Clear();

            // Draw numbered column headers
            Console.Write("  ");
            for (uint column = 1; column <= _game.Columns; column++)
            {
                Console.Write(column.ToString("00"));
                if (column < _game.Columns) Console.Write("|");
            }
            Console.WriteLine();

            // Draw each disc by row and column
            for (uint row = 1; row <= _game.Rows; row++)
            {
                Console.Write("  ");
                for (uint column = 1; column <= _game.Columns; column++)
                {
                    // get player's disc in location and set background colour accordingly
                    var disc = _game.GetDisc(column, row);
                    if (disc == null)
                        Console.BackgroundColor = ConsoleColor.Black;
                    else if (disc.Value == Game.Player.PlayerOne)
                        Console.BackgroundColor = ConsoleColor.Yellow;
                    else if (disc.Value == Game.Player.PlayerTwo)
                        Console.BackgroundColor = ConsoleColor.Red;

                    Console.Write("  ");

                    Console.BackgroundColor = ConsoleColor.Black;
                    if (column < _game.Columns) Console.Write("|");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("User has Yellow disc.  Computer has Red disc.");
            Console.WriteLine();
        }
    }
}
