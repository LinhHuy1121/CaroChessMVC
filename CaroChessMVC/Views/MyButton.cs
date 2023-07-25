
using System;
using Xamarin.Forms;

namespace CaroChessMVC.Views
{
    public partial class MyButton : Button
    {
        public MyButton()
        {
            this.TextColor = Color.FromHex("47B5FF");
            this.BackgroundColor = Color.FromHex("404258");
            this.FontSize = 16;
            this.MinimumWidthRequest = 90;
            this.MinimumHeightRequest = 60;
            this.CornerRadius = 10;
            this.Margin = 10;

        }

        public event EventHandler Clicked;
        protected virtual void RaiseClick() => Clicked?.Invoke(this, EventArgs.Empty);
    }
}
