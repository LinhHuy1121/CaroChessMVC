using CaroChessMVC.Models;

namespace CaroChessMVC.Controllers
{
    class BaseController : System.Mvc.Controller
    {
        static Models.Game game;
        public Game Game
        {
            get
            {
                if (game == null)
                {
                    game = new Models.Game();
                }
                return game;
            }
        }
        public virtual object Index() => View();
    }
}
