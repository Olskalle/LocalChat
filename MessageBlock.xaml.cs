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
	/// Логика взаимодействия для MessageBlock.xaml
	/// </summary>
	public partial class MessageBlock : UserControl
	{
		public MessageBlock()
		{
			InitializeComponent();
		}

		public string Username 
		{ 
			get => UsernameBlock.Text;
			set => UsernameBlock.Text = value; 
		}
		public string Time
		{
			get => TimeBlock.Text;
			set => TimeBlock.Text = value;
		}
		public string Message
		{
			get => MessageTextBlock.Text;
			set => MessageTextBlock.Text = value;
		}

		public Brush UserColor
		{
			get => userBorder.Background;
			set => userBorder.Background = value;
		}
	}
}
