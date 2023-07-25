using Xamarin.Forms;

namespace TestXamarinApp.Views.Images
{
    public static class ImageExtension
    {
        public static void SetForegroundColor(this Image img, Color color)
        {
            Effect tc = null;
            foreach (var e in img.Effects)
            {
                if (e is TintColor)
                {
                    tc = e;
                    break;
                }
            }
            if (tc != null)
            {
                img.Effects.Remove(tc);
            }
            img.Effects.Add(new TintColor
            {
                Foreground = color
            });
        }
    }
    public class TintColor : RoutingEffect
    {
        public Color Foreground { get; set; }
        public TintColor() : base($"Vst.{nameof(TintColor)}")
        {
        }
    }
}
