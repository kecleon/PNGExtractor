using System;
using System.IO;

namespace PNGExtractor
{
	public static class PngExtractor
	{
		private static readonly byte[] PngHeader = {0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A};
		private static readonly byte[] PngFooter = {0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82};
		private static readonly BoyerMooreByteArrayScanner Scanner = new BoyerMooreByteArrayScanner();

		public static void Main(string[] args)
		{
			var filename = Path.GetFileName(args[0]);
			var inputFile = File.ReadAllBytes(args[0]);

			Scanner.SetPattern(PngHeader);
			var headers = Scanner.SearchAll(inputFile);

			Scanner.SetPattern(PngFooter);

			int count = 0;
			foreach (var startOffset in headers)
			{
				var endOffset = Scanner.Search(inputFile, startOffset - 8);
				var length = (endOffset + PngFooter.Length) - startOffset;
				if (length <= 0 || (length + PngFooter.Length) >= endOffset)
					continue;

				byte[] outputFile = new byte[length];
				Array.Copy(inputFile, startOffset, outputFile, 0, length);

				File.WriteAllBytes($"{filename}-{count++}.png", outputFile);
			}
		}
	}
}