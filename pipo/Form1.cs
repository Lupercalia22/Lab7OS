using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pipo
{
	public partial class Form1 : Form
	{
		KeyBoardandMouse hook = new KeyBoardandMouse();
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			hook.StartHooks();
			richTextBox1.Clear();
		}

		async private void button2_Click(object sender, EventArgs e)
		{
			hook.StopHooks();
			using (FileStream fstream = File.OpenRead(@"log.txt"))
			{
				// выделяем массив для считывания данных из файла
				byte[] buffer = new byte[fstream.Length];
				// считываем данные
				await fstream.ReadAsync(buffer, 0, buffer.Length);
				// декодируем байты в строку
				string textFromFile = Encoding.Default.GetString(buffer);
				richTextBox1.Clear();
				richTextBox1.AppendText(textFromFile);
			}
		}
	}
}
