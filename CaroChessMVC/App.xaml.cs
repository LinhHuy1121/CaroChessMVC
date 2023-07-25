using Xamarin.Forms;

namespace CaroChessMVC
{
    public partial class App : Application
    {
        public static void Request(string url, params object[] args) => System.Mvc.Engine.Execute(url, args);


        public App()
        {
            InitializeComponent();

            System.Mvc.Engine.Register(this, result =>
            {
                var view = result.View;
                var page = view.Content as Page;

                if (page != null)
                {
                    MainPage = page;
                }
            });

            Request("home/start");

        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
