using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Newtonsoft.Json;

namespace UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
   Advise _advise = new Advise();
   public MainWindow()
   {
      InitializeComponent();
   }

   private Advise GetAdviseStrings()
   {
      Advise advise = new Advise();
      using (StreamReader reader = new StreamReader("Advise.json"))
      {
         string json = reader.ReadToEnd();
         Debug.WriteLine(json);
         advise = JsonConvert.DeserializeObject<Advise>(json);
      }
      Debug.WriteLine(advise.safe + "..." + advise.hit);
      return advise;
   }

   private void CheckBtn_Click(object sender, RoutedEventArgs e)
   {
      CheckPassword();
   }

   private void Password_Keydown(object sender, KeyEventArgs e)
   {
      if (e.Key == Key.Return)
      {
         CheckPassword();
      }
   }

   private void CheckPassword()
   {
      if (PasswordBox.SecurePassword == null || PasswordBox.SecurePassword.Length == 0)
      {
         resultText.Text = "Please enter your password and click the Check button. This tool is completely offline and does not store your password in any form.";
         return;
      }
      SecureStringWrapper secureString = new SecureStringWrapper(PasswordBox.SecurePassword);
      PasswordBox.Clear();
      byte[] srcBuffer = secureString.ToByteArray();
      HashAlgorithm SHA1 = HashAlgorithm.Create("SHA1");
      byte[] destBuffer = SHA1.ComputeHash(srcBuffer);
      string hashedPassword = System.BitConverter.ToString(destBuffer).Replace("-", "");
      secureString.Dispose();

      SQLiteConnection conn = DataAccess.CreateConnection();
      bool isHit = false;
      SQLiteCommand readCmd = conn.CreateCommand();
      readCmd.CommandText = string.Format("SELECT * FROM Hash WHERE HashValue >= '{0}' AND HashValue <= '{1}:9999999999999';", hashedPassword, hashedPassword);
      SQLiteDataReader reader = readCmd.ExecuteReader();
      string hashValue = string.Empty;
      while (reader.Read())
      {
         hashValue = reader.GetString(0);
         Console.WriteLine(@"Your password's hash: {0}", hashedPassword);
         Console.WriteLine(@"Record in database: {0}", hashValue);
         isHit = true;
      }

      if (!isHit)
      {
         resultText.Text = _advise.safe;
      }
      else
      {
         string count = hashValue.Substring(hashValue.IndexOf(":") + 1, hashValue.Length - hashValue.IndexOf(":") - 1);
         resultText.Text = string.Format(_advise.hit, count);
      }
   }

   private void Window_Loaded(object sender, RoutedEventArgs e)
   {
      _advise = GetAdviseStrings();
      resultText.Text = "Please enter your password and click the Check button. This tool is completely offline and does not store your password in any form.";
   }
}
