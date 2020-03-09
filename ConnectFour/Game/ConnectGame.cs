using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectFour.Game
{
    internal class ConnectGame : IConnectGame
    {
        private IList<Tuple<Player, Matrix>> _connectSlots = new List<Tuple<Player, Matrix>>();
        private Random _random = new Random();

        public ConnectGame()
        {
            Restart();
        }

        public ConnectGame(uint columns, uint rows) : this()
        {
            Columns = columns;
            Rows = rows;
        }

        public void Restart()
        {
            _connectSlots.Clear();
            WhosInPlay = Player.PlayerOne;
            WhoIsWinner = null;
        }

        public void Stop()
        {
            WhosInPlay = null;
        }

        public uint Columns { get; set;  } = 7;

        public uint Rows { get; set; } = 6;

        public Player? WhosInPlay { get; set; }

        private Player? WhosOutPlay
        {
            get
            { 
                if (WhosInPlay == null)
                    return null;

                return WhosInPlay == Player.PlayerOne ? Player.PlayerTwo : Player.PlayerOne;
            }
        }

        public Player? WhoIsWinner { get; set; }

        public uint AddDisc(uint column)
        {
            // Throw exception if game is not in play
            if (WhoIsWinner != null || WhosInPlay == null)
                throw new GameOverException();

            // Throw exception if column is out of range
            if (column > Columns)
                throw new GameInvalidColumnException();

            // Check and throw exception if column is full
            var row = CalculateNextFreeRow(column);
            if (row == null)
                throw new GameColumnFullException();

            // Add disc to list of slots
            _connectSlots.Add(new Tuple<Player, Matrix>(WhosInPlay.Value, new Matrix((int)column, (int)row.Value)));

            // Check from postion if user has four in a row
            var score = CalculatePotentialScore(WhosInPlay.Value, column, row.Value);

            // If so, then whoever is in play is the winner
            if (score >= 4)
                WhoIsWinner = WhosInPlay;

            // If all slots are filled, then end game with no winner
            else if (_connectSlots.Count() == Rows * Columns)
                Stop();

            // otherwise switch to other player
            else
                WhosInPlay = WhosOutPlay;

            // Return row where disc landed
            return row.Value;
        }

        /// <summary>
        /// Get the next free row for a given column
        /// </summary>
        /// <param name="column">desired column</param>
        /// <returns>next free row</returns>
        private uint? CalculateNextFreeRow(uint column)
        {
            // Return null if column is invalid
            if (column > Columns)
                return null;

            // Retrieve all slots filled for column
            var rows = _connectSlots.Where(f => f.Item2.X == column).OrderBy(f => f.Item2.Y);

            // If there are no slots then first slot is bottom row
            if (rows.Count() == 0)
                return Rows;

            // If top slot is filled then return null
            if (rows.First().Item2.Y == 1)
                return null;

            // Otherwise return slot above last filled slot
            return (uint)rows.First().Item2.Y - 1;
        }

        /// <summary>
        /// Return number of slots in line on column/ row for given player
        /// </summary>
        /// <param name="player">player</param>
        /// <param name="column">column of origin</param>
        /// <param name="row">row of origin</param>
        /// <returns></returns>
        private uint CalculatePotentialScore(Player player, uint column, uint row)
        {
            // define transformation matrix for testing if line exists either vertically, horizontally or diagonally
            var deltaCoordinates = new[]
            {
                new[] { new Matrix(-1, 0), new Matrix(1, 0) },
                new[] { new Matrix(-1, -1), new Matrix(1, 1) },
                new[] { new Matrix(0, -1), new Matrix(0, 1) },
                new[] { new Matrix(1, -1), new Matrix(-1, 1) },
            };

            // test how mant in line in each direction from column/ row origin
            uint highestScore = 0;
            foreach (var deltaCoordinate in deltaCoordinates)
            {
                uint directionScore = 1;

                // test each direction on line
                foreach (var singledeltaCoordinate in deltaCoordinate)
                {
                    bool isInRange = true;
                    Matrix pos = new Matrix((int)column, (int)row);
                    Player? disc = player;

                    // count slots in line for player
                    while (disc != null && disc.Value == player)
                    {
                        pos += singledeltaCoordinate;
                        isInRange = pos.X >= 1 && pos.X <= Columns && pos.Y >= 1 && pos.Y <= Rows;
                        disc = isInRange ? GetDisc((uint)pos.X, (uint)pos.Y) : null;
                        if (disc != null && disc.Value == player)
                            directionScore++;
                    };
                }

                // Retain highest number of slots in line
                if (directionScore > highestScore)
                    highestScore = directionScore;
            }

            // Return highest number of slots in a line
            return highestScore;
        }

        public uint RecommendColumn
        {
            get
            {
                // Reset highest scores for current user and opponent
                var highestScoringInPlay = new Tuple<uint, uint>(0, 0);
                var highestScoringOutPlay = new Tuple<uint, uint>(0, 0);

                // Test each column for its potential
                for (uint column = 1; column <= Columns; column++)
                {
                    // Evaluate if its worth testing current column
                    var row = CalculateNextFreeRow(column);
                    if (row != null)
                    {
                        // What score can user expect to achieve if a disc was added to this column
                        var scoreInPlay = CalculatePotentialScore(WhosInPlay.Value, column, row.Value);
                        if (scoreInPlay > highestScoringInPlay.Item2)
                            highestScoringInPlay = new Tuple<uint, uint>(column, scoreInPlay);

                        // What score would an opponent exect to achieve if a disc was added to this column
                        var scoreOutPlay = CalculatePotentialScore(WhosOutPlay.Value, column, row.Value);
                        if (scoreOutPlay > highestScoringOutPlay.Item2)
                            highestScoringOutPlay = new Tuple<uint, uint>(column, scoreOutPlay);
                    }
                }

                // If this is the first disc for the current user then use randomiser to suggest a location,
                // thus starting the game on an intersting footing.
                if (highestScoringInPlay.Item2 == 1)
                    return (uint)_random.Next(1, (int)Columns);

                // ALways recommend the column than would result in the opponent being blocked.
                if (highestScoringOutPlay.Item2 > highestScoringInPlay.Item1)
                    return highestScoringOutPlay.Item1;

                // Otherwise recommend the column that would result in the highest score
                return highestScoringInPlay.Item1;
            }
        }

        public Player? GetDisc(uint column, uint row)
        {
            // Get slot from list for column/ row
            var slot = _connectSlots.Where(f => f.Item2.X == column && f.Item2.Y == row);

            // Return null if slot is not filled
            if (slot.Count() == 0)
                return null;

            // Otherwise return player in slot
            return slot.First().Item1;
        }
    }

    /// <summary>
    /// Standard matrix class with addition operator override
    /// </summary>
    internal struct Matrix
    {
        public Matrix(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public static Matrix operator +(Matrix lhs, Matrix rhs)
        {
            return new Matrix(lhs.X + rhs.X, lhs.Y + rhs.Y);
        }
    } 

    public class GameOverException : Exception
    {
        public GameOverException(): base("Game is over") { }
    }

    public class GameInvalidColumnException: Exception
    {
        public GameInvalidColumnException(): base("Invalid column number") { }
    }

    public class GameColumnFullException: Exception
    {
        public GameColumnFullException(): base("Column is full up") { }
    }
}
