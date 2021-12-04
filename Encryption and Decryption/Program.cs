using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Encryption_and_Decryption
{
	class Program
	{
		public static string EncryptText(string plainText, string PasswordHash, string SaltKey, string VIKey)
		{

			byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

			byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.UTF8.GetBytes(SaltKey)).GetBytes(256 / 8);
			var symmetricKey = new RijndaelManaged() { Mode = CipherMode.ECB, Padding = PaddingMode.ISO10126 };
			var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.UTF8.GetBytes(VIKey));

			byte[] cipherTextBytes;

			using (var memoryStream = new MemoryStream())
			{
				using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
				{
					cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
					cryptoStream.FlushFinalBlock();
					cipherTextBytes = memoryStream.ToArray();
					cryptoStream.Close();
				}
				memoryStream.Close();
			}
			return Convert.ToBase64String(cipherTextBytes);
		}

		public static string DecryptText(string encryptedText, string PasswordHash, string SaltKey, string VIKey)
		{


			byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
			byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.UTF8.GetBytes(SaltKey)).GetBytes(256 / 8);
			var symmetricKey = new RijndaelManaged() { Mode = CipherMode.ECB, Padding = PaddingMode.ISO10126 };

			var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.UTF8.GetBytes(VIKey));
			var memoryStream = new MemoryStream(cipherTextBytes);
			var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
			byte[] plainTextBytes = new byte[cipherTextBytes.Length];

			int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
			memoryStream.Close();
			cryptoStream.Close();
			return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
		}


		private static void EncryptFile(string inputFile, string outputFile)
		{

			try
			{
				string password = @"myKey123"; // Your Key Here
				UnicodeEncoding UE = new UnicodeEncoding();
				byte[] key = UE.GetBytes(password);

				string cryptFile = outputFile;
				FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);

				RijndaelManaged RMCrypto = new RijndaelManaged();

				CryptoStream cs = new CryptoStream(fsCrypt,
					RMCrypto.CreateEncryptor(key, key),
					CryptoStreamMode.Write);

				FileStream fsIn = new FileStream(inputFile, FileMode.Open);

				int data;
				while ((data = fsIn.ReadByte()) != -1)
					cs.WriteByte((byte)data);


				fsIn.Close();
				cs.Close();
				fsCrypt.Close();
			}
			catch
			{

			}
		}


		private static void DecryptFile(string inputFile, string outputFile)
		{

			{
				string password = @"myKey123"; // Your Key Here

				UnicodeEncoding UE = new UnicodeEncoding();
				byte[] key = UE.GetBytes(password);

				FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);

				RijndaelManaged RMCrypto = new RijndaelManaged();

				CryptoStream cs = new CryptoStream(fsCrypt,
					RMCrypto.CreateDecryptor(key, key),
					CryptoStreamMode.Read);

				FileStream fsOut = new FileStream(outputFile, FileMode.Create);

				int data;
				while ((data = cs.ReadByte()) != -1)
					fsOut.WriteByte((byte)data);

				fsOut.Close();
				cs.Close();
				fsCrypt.Close();

			}

		}

		static void Main(string[] args)
		{
			string word = Text();

			string PasswordHash = "P@@Sw0rd";
			string SaltKey = "S@LT&KEY";
			string VIKey = "@1B2c3D4e5F6g7H8";


			Console.WriteLine(EncryptText(word, PasswordHash, SaltKey, VIKey));
			Console.WriteLine(DecryptText(EncryptText(word, PasswordHash, SaltKey, VIKey), PasswordHash, SaltKey, VIKey));


			if (File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}/1.zip"))
			{
				EncryptFile($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}/1.zip", $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}/Encrypt File.ttt");
				DecryptFile($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}/Encrypt File.ttt", $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}/12.zip");
			}


			Console.ReadKey();
		}



		public static string Text()
		{
	return @"
Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed imperdiet sagittis eros ut commodo. Quisque vitae ornare massa. Nam facilisis convallis orci, at auctor nisl. Phasellus convallis, nisl sed tempus iaculis, erat diam vestibulum quam, nec tincidunt orci nulla ac tortor. Maecenas a lectus vehicula, ultrices diam sit amet, posuere elit. Donec eget diam vitae ante rhoncus imperdiet imperdiet quis justo. Maecenas iaculis diam sed sollicitudin varius. Cras dignissim purus dictum consectetur tincidunt.
Nulla eleifend augue a arcu porttitor, in gravida arcu accumsan. In sit amet mattis dolor.";

		}
	}
}
