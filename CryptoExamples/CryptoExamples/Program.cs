using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptoExamples
{
	class Program
	{
		public static void Main()
		{
			try
			{

				string original = "Here is some data to encrypt!";

				// Create a new instance of the Aes 
				// class.  This generates a new key and initialization  
				// vector (IV). 
				using (var random = new RNGCryptoServiceProvider())
				{
					var key = new byte[16];
					random.GetBytes(key);

					// Encrypt the string to an array of bytes. 
					byte[] encrypted = EncryptStringToBytes_Aes(original, key);

					// Decrypt the bytes to a string. 
					string roundtrip = DecryptStringFromBytes_Aes(encrypted, key);

					//Display the original data and the decrypted data.
					Console.WriteLine("Original:   {0}", original);
					Console.WriteLine("Encrypted (b64-encode): {0}", Convert.ToBase64String(encrypted));
					Console.WriteLine("Round Trip: {0}", roundtrip);
					Console.ReadKey();
				}

			}
			catch (Exception e)
			{
				Console.WriteLine("Error: {0}", e.Message);
			}
		}

		static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key)
		{
			byte[] encrypted;
			byte[] IV;

			using (Aes aesAlg = Aes.Create())
			{
				aesAlg.Key = Key;

				aesAlg.GenerateIV();
				IV = aesAlg.IV;

				aesAlg.Mode = CipherMode.CBC;

				var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

				// Create the streams used for encryption. 
				using (var msEncrypt = new MemoryStream())
				{
					using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
					{
						using (var swEncrypt = new StreamWriter(csEncrypt))
						{
							//Write all data to the stream.
							swEncrypt.Write(plainText);
						}
						encrypted = msEncrypt.ToArray();
					}
				}
			}

			var combinedIvCt = new byte[IV.Length + encrypted.Length];
			Array.Copy(IV, 0, combinedIvCt, 0, IV.Length);
			Array.Copy(encrypted, 0, combinedIvCt, IV.Length, encrypted.Length);

			// Return the encrypted bytes from the memory stream. 
			return combinedIvCt;

		}

		static string DecryptStringFromBytes_Aes(byte[] cipherTextCombined, byte[] Key)
		{

			// Declare the string used to hold 
			// the decrypted text. 
			string plaintext = null;

			// Create an Aes object 
			// with the specified key and IV. 
			using (Aes aesAlg = Aes.Create())
			{
				aesAlg.Key = Key;

				byte[] IV = new byte[aesAlg.BlockSize / 8];
				byte[] cipherText = new byte[cipherTextCombined.Length - IV.Length];

				Array.Copy(cipherTextCombined, IV, IV.Length);
				Array.Copy(cipherTextCombined, IV.Length, cipherText, 0, cipherText.Length);

				aesAlg.IV = IV;

				aesAlg.Mode = CipherMode.CBC;

				// Create a decrytor to perform the stream transform.
				ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

				// Create the streams used for decryption. 
				using (var msDecrypt = new MemoryStream(cipherText))
				{
					using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
					{
						using (var srDecrypt = new StreamReader(csDecrypt))
						{

							// Read the decrypted bytes from the decrypting stream
							// and place them in a string.
							plaintext = srDecrypt.ReadToEnd();
						}
					}
				}

			}

			return plaintext;

		}
	}
}
