using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Text.Json;
using System.IO;
using System.Diagnostics;

namespace LocalChat
{
	internal class ChatClient : IChatClient
	{
		private readonly int localPort = 8001;
		IPAddress brodcastAddress;
		private bool isActive;
		private UdpClient client;

        public ChatClient(bool startImmediate)
        {
			brodcastAddress = IPAddress.Parse("255.255.255.255");
			//brodcastAddress = IPAddress.Parse("235.5.5.11");
			client = new UdpClient(localPort);
			if (startImmediate) this.Run();
			isActive = startImmediate;
		}

		public event EventHandler<MessageEventArgs> MessageReceived;

        // отправка сообщений в группу
        async Task SendMessageAsync(IMessage message)
		{
			//using var sender = new UdpClient(); // создаем UdpClient для отправки
												// отправляем сообщения
			if (isActive)
			{
				if (!message.Valid()) return;

				byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

				await client.SendAsync(data, new IPEndPoint(brodcastAddress, localPort));
			}
		}
		// получение сообщений из группы
		async Task ReceiveMessageAsync()
		{
			//using var receiver = new UdpClient(localPort); // UdpClient для получения данных
			
			while (isActive)
			{
				var result = await client.ReceiveAsync();
				string serializedMessage = Encoding.UTF8.GetString(result.Buffer);
				try
				{
					var message = await JsonSerializer.DeserializeAsync<Message>(new MemoryStream(result.Buffer));
					MessageReceived?.Invoke(this, 
						new MessageEventArgs(message 
						?? new Message() 
						{
							Sender = "Unknown", 
							Content = "Empty"
						}));
				}
				catch
				{

				} 
			}
			Debug.WriteLine("Stopped");
		}

		public async void Run()
		{
			isActive = true;
			client.JoinMulticastGroup(brodcastAddress);
			client.MulticastLoopback = false;
			await Task.Run(ReceiveMessageAsync);
		}

		public void Stop()
		{
			client.DropMulticastGroup(brodcastAddress);
			isActive = false;
		}

		public async void OnMessageSend(object? sender, MessageEventArgs e)
		{
			await SendMessageAsync(e.Message);
		}
	}

	public class Message : IMessage
	{
		public MessageType MessageType => MessageType.UserMessage;
		public DateTime Time { get; set; } = DateTime.Now;
		public string Sender { get; set; } = null!;
		public string Content { get; set; } = "";

		public bool Valid()
		{
			return !(string.IsNullOrWhiteSpace(Content) || string.IsNullOrWhiteSpace(Sender));
		}
	}
	public class ChatMessage : IMessage
	{
		public MessageType MessageType => MessageType.ChatMessage;
		public string Content { get; set; } = null!;

		public bool Valid()
		{
			return !string.IsNullOrWhiteSpace(Content);
		}
	}

	public class MessageEventArgs : EventArgs
	{
		public IMessage Message { get; set; }
        public MessageEventArgs(IMessage message) : base()
        {
			Message = message;
        }
    }
	public interface IMessage
	{
		MessageType MessageType { get; }
		string Content { get; }
		bool Valid();
	}

	public enum MessageType
	{
		UserMessage,
		ChatMessage
	}
}
