using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pipo
{
	internal class KeyBoardandMouse
	{
		// Константы для хуков клавиатуры и мыши
		private const int WH_KEYBOARD_LL = 13;
		private const int WH_MOUSE_LL = 14;
		private const int WM_KEYDOWN = 0x0100;
		private const int WM_MOUSEMOVE = 0x0200;
		private const int WM_LBUTTONDOWN = 0x0201;
		private const int WM_RBUTTONDOWN = 0x0204;

		// Делегаты для обработки хуков
		private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
		private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

		// Импорт функций из user32.dll
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetModuleHandle(string lpModuleName);

		// Приватные поля
		private IntPtr _keyboardHook = IntPtr.Zero;
		private IntPtr _mouseHook = IntPtr.Zero;
		private LowLevelKeyboardProc _keyboardProc;
		private LowLevelMouseProc _mouseProc;
		private StreamWriter _logStreamWriter;

		// Конструктор
		public KeyBoardandMouse()
		{
			_keyboardProc = KeyboardHookProc;
			_mouseProc = MouseHookProc;
		}

		// Метод для запуска глобальных хуков
		public void StartHooks()
		{
			// Создаем файловый поток для записи лога
			File.WriteAllText("log.txt", string.Empty);
			_logStreamWriter = new StreamWriter("log.txt", true, Encoding.Default);

			// Устанавливаем хуки для клавиатуры и мыши
			_keyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, _keyboardProc, GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
			_mouseHook = SetWindowsHookEx(WH_MOUSE_LL, _mouseProc, GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
		}

		// Метод для остановки глобальных хуков
		public void StopHooks()
		{
			// Удаляем хуки для клавиатуры и мыши
			UnhookWindowsHookEx(_keyboardHook);
			UnhookWindowsHookEx(_mouseHook);

			// Закрываем файловый поток для записи лога
			_logStreamWriter.Close();
		}
		// Метод для обработки событий клавиатуры
		private IntPtr KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
			{
				int vkCode = Marshal.ReadInt32(lParam);
				string logEntry = $"[Keyboard] Key Pressed: {(Keys)vkCode}, Time={DateTime.Now}";
				_logStreamWriter.WriteLine(logEntry);
			}

			return CallNextHookEx(_keyboardHook, nCode, wParam, lParam);
		}

		// Метод для обработки событий мыши
		private IntPtr MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode >= 0)
			{
				if (wParam == (IntPtr)WM_MOUSEMOVE)
				{
					int xPos = Marshal.ReadInt32(lParam);
					int yPos = Marshal.ReadInt32(lParam + 4);
					string logEntry = $"[Mouse] Mouse Move: X={xPos}, Y={yPos}, Time={DateTime.Now}";
					_logStreamWriter.WriteLine(logEntry);
				}
				else if (wParam == (IntPtr)WM_LBUTTONDOWN)
				{
					int xPos = Marshal.ReadInt32(lParam);
					int yPos = Marshal.ReadInt32(lParam + 4);
					string logEntry = $"[Mouse] Left Button Down: X={xPos}, Y={yPos}, Time={DateTime.Now}";
					_logStreamWriter.WriteLine(logEntry);
				}
				else if (wParam == (IntPtr)WM_RBUTTONDOWN)
				{
					int xPos = Marshal.ReadInt32(lParam);
					int yPos = Marshal.ReadInt32(lParam + 4);
					string logEntry = $"[Mouse] Right Button Down: X={xPos}, Y={yPos}, Time={DateTime.Now}";
					_logStreamWriter.WriteLine(logEntry);
				}
			}

			return CallNextHookEx(_mouseHook, nCode, wParam, lParam);
		}
	}
}
