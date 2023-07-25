using CaroChessMVC.Models;
using MQTTnet;
using MQTTnet.Client;
using System;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CaroChessMVC.Views.Home
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        private IMqttClient mqttClient;
        private string topic = "gamecaro"; // Replace with the desired topic
        private Models.Game game;
        private string gameIdentifier;
        private bool isGameOver = false;

        public HomePage()
        {
            InitializeComponent();


            foreach (Button b in this.header_tools.Children)
            {
                b.TextColor = Color.FromHex("47B5FF");
                b.BackgroundColor = Color.FromHex("404258");
                b.FontSize = 16;
                b.MinimumWidthRequest = 90;
                b.MinimumHeightRequest = 60;
                b.CornerRadius = 10;
                b.Margin = 10;
            }

            InitializeMqttClient();


        }


        private async void InitializeMqttClient()
        {
            string brokerAddress = "broker.hivemq.com"; // Replace with the actual broker address

            var factory = new MqttFactory();
            mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(brokerAddress, 1883)
                .Build();


            await mqttClient.ConnectAsync(options);
            if (mqttClient.IsConnected)
            {
                await mqttClient.SubscribeAsync(topic);
                await mqttClient.SubscribeAsync("gameOver"); // Subscribe to the "gameOver" topic
                mqttClient.ApplicationMessageReceivedAsync += Client_ApplicationMessageReceivedAsync;
                
            }
        }


        private Task Client_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs args)
        {
            String mess = Encoding.UTF8.GetString(args.ApplicationMessage.Payload);

            if (!isGameOver && args.ApplicationMessage.Topic == "gameOver" + gameIdentifier)
            {
                isGameOver = true; // Set the flag to indicate that the game is over for this game identifier
                string result = mess.Substring("gameOver|".Length);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("End Game", result, "OK");
                });
            }
            else
            {
                string[] arr = mess.Split('|');
                var cell = (Button)BanCo.Children[int.Parse(arr[0]) * game.Size + int.Parse(arr[1])];
                cell.Text = arr[2];
            }
            
            return Task.CompletedTask;
        }


        protected override async void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            //this.Dispatcher.BeginInvokeOnMainThread(() =>
            //{

            //});

            game = (Models.Game)BindingContext;

            btnHome.Clicked += (s, e) => App.Request("home/start");
            btnRestart.Clicked += (s, e) => App.Request("home");
            btnUndo.Clicked += (s, e) => game.Undo();

            game.Changed += async (s, e) =>
            {
                var m = game.CurrentMove;
                var cell = (Button)BanCo.Children[m.Row * game.Size + m.Column];

                string message = m.Row.ToString() + "|" + m.Column.ToString() + "|" + new string(m.Player, 1);
                var mqttMessage = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(message)
                    .Build();
                try
                {
                    await mqttClient.PublishAsync(mqttMessage);
                }
                catch (Exception ex)
                {

                }
                // cell.Text = new string(m.Player, 1);

            };
            checkGame();


        }

        private void checkGame()
        {

            game.GameOver += async (s, e) =>
            {
                if (!isGameOver && e.Move.Player == game.Player) // Ensure the event is triggered by the current player (winning player)
                {
                    isGameOver = true; // Set the flag to indicate that the game is over for this game identifier
                    
             
                    // Gửi thông báo về trạng thái trận đấu cho cả hai người chơi
                    string message = "gameOver|" + game.Player + " win!!!";
                    var mqttMessage = new MqttApplicationMessageBuilder()
                        .WithTopic("gameOver")
                        .WithPayload(message)
                        .Build();
                    try
                    {
                        await mqttClient.PublishAsync(mqttMessage);
                    }
                    catch (Exception ex)
                    {
                        // Xử lý lỗi nếu không thể gửi thông báo
                    }

                    await DisplayAlert("End Game", game.Player + " win!!!", "OK");
                }
            };

            for (int i = 0; i < game.Size; i++)
            {
                BanCo.ColumnDefinitions.Add(new ColumnDefinition());
                BanCo.RowDefinitions.Add(new RowDefinition());
            }

            for (int r = 0; r < game.Size; r++)
            {
                for (int c = 0; c < game.Size; c++)
                {

                    var cell = new Button
                    {
                        BackgroundColor = Color.White,
                        TextColor = Color.Red,
                        FontSize = 16,
                        Padding = -5,
                    };
                    BanCo.Children.Add(cell);

                    cell.SetValue(Grid.RowProperty, r);
                    cell.SetValue(Grid.ColumnProperty, c);

                    cell.Clicked += (s, e) =>
                    {
                        game.PutAndCheckGameOver((int)cell.GetValue(Grid.RowProperty), (int)cell.GetValue(Grid.ColumnProperty));
                    };
                }
            }
        }

    }

    internal class Index : BaseView<HomePage, Models.Game>
    {

    }
}