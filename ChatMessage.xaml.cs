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
	/// Логика взаимодействия для ChatMessage.xaml
	/// </summary>
	public partial class ChatMessageBlock : UserControl
	{
		public ChatMessageBlock()
		{
			InitializeComponent();
		}

		public string Message
		{
			get => messageBlock.Text;
			set => messageBlock.Text = value;
		}
	}
}
