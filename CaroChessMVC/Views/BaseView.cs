using System.Mvc;
using Xamarin.Forms;

namespace CaroChessMVC.Views
{
    public class BaseView<TPage, TModel> : IView
        where TPage : Page, new()
    {
        public object Content => Page;

        public void Render(object model)
        {
            Model = (TModel)model;
            Page = new TPage
            {
                BindingContext = Model,
            };

            RenderCore();
        }

        public TModel Model { get; set; }
        public TPage Page { get; set; }

        public ViewDataDictionary ViewBag { get; set; }

        protected virtual void RenderCore() { }
    }

    public class BaseView<TPage> : BaseView<TPage, object>
        where TPage : Page, new()
    { }
}
