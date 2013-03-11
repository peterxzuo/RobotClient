using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using System.Windows.Controls.Primitives;
using Newtonsoft.Json;

namespace RobotProxy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HubConnection hubConnection;
        IHubProxy hub;

        private string MyRobotId;
        public MainWindow()
        {
            InitializeComponent();
            MyRobotId = Guid.NewGuid().ToString();
        }

        private void OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            // Start connect
            ThreadPool.QueueUserWorkItem(OnStartConnect, textBoxUrl.Text);

            this.MyRobotId = robotKey.Text;
            startButton.IsEnabled = false;
            stopButton.IsEnabled = true;
        }

        private void OnStartConnect(object state)
        {
            string url = state as string;

            this.hubConnection = new HubConnection(url);
            this.hub = this.hubConnection.CreateHubProxy("RobotSignalHub");
            this.hubConnection.Start();
            this.hubConnection.StateChanged += OnHubConnectionStateChanged;
        }

        private void OnHubConnectionStateChanged(StateChange stateChange)
        {
            if (stateChange.OldState == ConnectionState.Connecting &&
                stateChange.NewState == ConnectionState.Connected)
            {
                this.hub.Invoke("Register", this.MyRobotId);
                this.hub.On<string>("send", message => ReceiveWalk(message));
            }
        }

        private void ReceiveWalk(string message)
        {
            // Post back to Status.
            SetStatus(message);

            string robotReceiverUrl = textBoxRobotReceiverUrl.Text;
            if (!string.IsNullOrEmpty(robotReceiverUrl))
            {
                PostMessageToRobotReceiver(robotReceiverUrl, message);
            }
        }

        private void PostMessageToRobotReceiver(string robotReceiverUrl, string message)
        {
            RobotControlSignal signal = null;
            try
            {
                // Deserialize message
                signal = JsonConvert.DeserializeObject(message, typeof(RobotControlSignal)) as RobotControlSignal;
            }
            catch (Exception ex)
            {
                SetStatus(ex.Message);
            }

            // Send Message to robotReceiverUrl
            PostRobotSignalToRobotReceiver(robotReceiverUrl, signal);
        }

        // Todo: (Joseph) This is the function to post robot receiver.
        private void PostRobotSignalToRobotReceiver(string robotReceiverUrl, RobotControlSignal signal)
        {
            throw new NotImplementedException();
        }

        private void SetStatus(string message)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                SetStatus(message);
                StringBuilder sb = new StringBuilder();
                sb.Append(message);
                sb.AppendLine();
                sb.Append(statusText.Text);
                statusText.Text = sb.ToString();
            }), null);
        }

        private void OnStopButtonClick(object sender, RoutedEventArgs e)
        {
            this.hubConnection.Disconnect();
            this.hubConnection.StateChanged -= OnHubConnectionStateChanged;
            this.hub = null;
            this.hubConnection = null;

            this.startButton.IsEnabled = true;
            this.stopButton.IsEnabled = false;
        }
    
    }
}
