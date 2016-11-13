using System;
using EntitySystem.Utility;

namespace EntitySystem.Playground
{
	public class ConsoleBuffer
	{
		char[] _buffer;
		
		public int Width => Console.BufferWidth;
		public int Height => Console.BufferHeight - 1;
		
		public ConsoleBuffer()
		{
			Console.OutputEncoding = System.Text.Encoding.Unicode;
			Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
			
			_buffer = new char[Width * Height];
		}
		
		public void Flush()
		{
			Console.SetCursorPosition(0, 0);
			Console.Write(_buffer);
			Console.Write("> ");
		}
		
		public void Clear(char chr = ' ') =>
			_buffer.Fill<char>(chr);
		
		public void Char(int x, int y, char chr) =>
			_buffer[GetIndex(x, y)] = chr;
		
		public void LineHor(int x, int y, int width, char chr)
			{ for (var i = 0; i < width; i++) Char(x + i, y, chr); }
		
		public void BLineVer(int x, int y, int height, char chr)
			{ for (var i = 0; i < height; i++) Char(x, y + i, chr); }
		
		public void Box(int x, int y, int width, int height)
		{
			Char(x, y, '╔');
			Char(x + width - 1, y, '╗');
			Char(x, y + height - 1, '╚');
			Char(x + width - 1, y + height - 1, '╝');
			LineHor(x + 1, y, width - 2, '═');
			LineHor(x + 1, y + height - 1, width - 2, '═');
			BLineVer(x, y + 1, height - 2, '║');
			BLineVer(x + width - 1, y + 1, height - 2, '║');
		}
		
		int GetIndex(int x, int y) => x + y * Width;
	}
}