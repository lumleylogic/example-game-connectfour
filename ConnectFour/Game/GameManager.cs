using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectFour.Game
{
    public class GameManager : IGameManager
    {
        private static readonly Lazy<GameManager> _manager = new Lazy<GameManager>(() => new GameManager());

        public static GameManager Instance()
        {
            return _manager.Value;
        }

        public IConnectGame CreateRegularConnectFourGame()
        {
            return new ConnectGame();
        }
    }
}
