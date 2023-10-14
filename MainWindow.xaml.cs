using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LocalChat
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private bool isInitialized;
		private bool isLogged;
		private string userName;
		private IChatClient chatClient;
		private Dictionary<string, Brush> userColors;
		public MainWindow()
		{
			InitializeComponent();
			chatClient = new ChatClient(false);
			userName = "";
			userColors = new Dictionary<string, Brush>();
		}

		public event EventHandler<MessageEventArgs>? MessageSent;

		private void OnLogin(object sender, RoutedEventArgs e)
		{
			if (isLogged || string.IsNullOrWhiteSpace(userNameBox.Text)) return;
			userName = userNameBox.Text;
			isLogged = true;
			SendLogonMessage($"{userName} вошел в чат");
			chatClient.Run();
		}
		private void OnLogout(object sender, RoutedEventArgs e)
		{
			if (!isLogged) return;
			SendLogonMessage($"{userName} покинул чат");
			userName = "";
			isLogged= false;
			chatClient.Stop();
		}

		private void SendLogonMessage(string text)
		{
			if (userName == "") return;

			var message = new ChatMessage()
			{
				Content = text
			};
			this.MessageSent?.Invoke(this, new MessageEventArgs(message));
			OnMessageReceived(this, new MessageEventArgs(message));
		}

		private void OnMessageSend(object sender, RoutedEventArgs e)
		{
			if (userName == "" || string.IsNullOrWhiteSpace(messageTextBox.Text)) return;

			var message = new Message()
			{
				Sender = userName,
				Content = messageTextBox.Text
			};
			messageTextBox.Text = "";
			this.MessageSent?.Invoke(this, new MessageEventArgs(message));
			OnMessageReceived(this, new MessageEventArgs(message));
		}

		private void OnLoad(object sender, RoutedEventArgs e)
		{
			this.MessageSent += new EventHandler<MessageEventArgs>(chatClient.OnMessageSend);
			chatClient.MessageReceived += new EventHandler<MessageEventArgs>(OnMessageReceived);
		}

		private void OnMessageReceived(object? sender, MessageEventArgs e)
		{
			switch (e.Message.MessageType)
			{
				case MessageType.UserMessage:
					
					var messageBlock = new MessageBlock()
					{
						Time = $"{((Message)e.Message).Time.ToString("HH:mm")}",
						Username = $"{((Message)e.Message).Sender}",
						Message = $"{e.Message.Content}",
						UserColor = GetUserColor(((Message)e.Message).Sender)
					};
					//var messageBlock = new TextBlock() { Text = $"{e.Message.Time.ToString("HH:mm")}| ({e.Message.Sender}): {e.Message.Content}"};
					messageStack.Children.Add(messageBlock);
					break;
				case MessageType.ChatMessage:
					messageStack.Children.Add(new ChatMessageBlock() { Message = e.Message.Content});
					break;
				default: break;
			}
			
		}

		private Brush GetUserColor(string user)
		{
			if (userColors.ContainsKey(user)) return userColors[user];

			Random r = new Random();
			Brush brush = new SolidColorBrush(Color.FromRgb((byte)r.Next(30, 150),
							  (byte)r.Next(30, 150), (byte)r.Next(30, 150)));
			userColors.Add(user, brush);
			return brush;
		}
	}

	public interface IChatClient
	{
		event EventHandler<MessageEventArgs> MessageReceived;
		void OnMessageSend(object? sender, MessageEventArgs e);
		void Run();
		void Stop();
	}
}
