using System;

namespace CaroChessMVC.Controllers
{
    internal class HomeController : BaseController
    {
        public object Start() => View();
        public object Home() => View();
        public override object Index()
        {
            return View(Game.Start());
        }
    }
}
