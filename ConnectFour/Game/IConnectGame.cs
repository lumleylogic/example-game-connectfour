using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectFour.Game
{
    public enum Player {  PlayerOne = 1, PlayerTwo };

    public interface IConnectGame
    {
        /// <summary>
        /// Restart game, clearing all slots and starting with User in play
        /// </summary>
        void Restart();

        /// <summary>
        /// Stop game, with nobody in play and no winner
        /// </summary>
        void Stop();

        /// <summary>
        /// How many columns (default is 7)
        /// </summary>
        uint Columns { get; }

        /// <summary>
        /// How many rows (default is 6)
        /// </summary>
        uint Rows { get; }

        /// <summary>
        /// Which player is currently in play (null if nobody in play) 
        /// </summary>
        Player? WhosInPlay { get; }

        /// <summary>
        /// Who is the winner.  Calculated when a disc is added.
        /// </summary>
        Player? WhoIsWinner { get; }

        /// <summary>
        /// Add disc for who is currently in play into the desired column slot
        /// Throw an exception if column is out of range, column is full or game is not in play
        /// </summary>
        /// <param name="column">Desired column slot</param>
        /// <returns>row well disc came to rest</returns>
        uint AddDisc(uint column);

        /// <summary>
        /// Recommends column with the greatest chance of winning or can block an opponent
        /// </summary>
        uint RecommendColumn { get; }

        /// <summary>
        /// Retrieve which player's disc sits on a column and row
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        Player? GetDisc(uint column, uint row);
    }
}
