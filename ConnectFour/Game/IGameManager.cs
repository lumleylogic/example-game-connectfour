using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectFour.Game
{
    public interface IGameManager
    {
        /// <summary>
        /// Create a new game with standard seven columns by six rows
        /// </summary>
        /// <returns>game instance</returns>
        IConnectGame CreateRegularConnectFourGame();
    }
}
