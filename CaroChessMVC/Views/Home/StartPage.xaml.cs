using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CaroChessMVC.Views.Home
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StartPage : ContentPage
    {
        public StartPage()
        {
            InitializeComponent();
            btnStart.Clicked += (s, e) => App.Request("home/index");

        }
    }

    public class Start : BaseView<StartPage>
    {
        protected override void RenderCore()
        {
            base.RenderCore();
        }
    }
}