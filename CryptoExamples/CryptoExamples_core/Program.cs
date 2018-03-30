using System;
using System.Security.Cryptography;

namespace CryptoExamples_core
{
    class Program
    {
        static void Main(string[] args)
        {
			using (var random = new RNGCryptoServiceProvider())
			{
			}
			Console.WriteLine("Hello World!");
        }
    }
}
