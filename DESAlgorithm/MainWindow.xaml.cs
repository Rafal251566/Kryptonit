using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DESAlgorithm
{
    public partial class MainWindow : Window
    {
        private static byte[] key = Encoding.UTF8.GetBytes("rewolwer");

        public MainWindow()
        {
            InitializeComponent();
        }

        //Obsługa przycisku
        private void Encrypt_Click(object sender, RoutedEventArgs e)
        {
            string text = InputText.Text;
            OutputText.Text = Encrypt(text);
        }

        //Obsługa przycisku
        private void Decrypt_Click(object sender, RoutedEventArgs e)
        {
            string text = InputText.Text;
            OutputText.Text = Decrypt(text);
        }

        public static string Encrypt(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(DESAlgorithm(data));
        }

        public static string Decrypt(string encryptedText)
        {
            byte[] data = Convert.FromBase64String(encryptedText);
            return Encoding.UTF8.GetString(DESAlgorithm(data));
        }

        //Metoda przerabia tekst wpisany przez uzytkownika na bloki bajtów
        private static byte[] DESAlgorithm(byte[] data) //wersja początkowa cosik nie chodzi dekrypcja
        {
            byte[] processedData = new byte[data.Length];
            for (int i = 0; i < data.Length; i += 8)
            {
                byte[] block = new byte[8];
                Array.Copy(data, i, block, 0, 8); //wywali exception jak tekst nie bedzie skladal sie z 8 bajtowych blokow
                byte[] resultBlock = ProcessBlock(block);
                Array.Copy(resultBlock, 0, processedData, i, 8); 
            }
            return processedData;
        }

        //Metoda dzieli pełny 8 bajtowy blok na dwa 4 bajtowe (lewy i prawy) i pozniej stosuje sieć Feistela
        private static byte[] ProcessBlock(byte[] block)
        {
            byte[] left = new byte[4];
            byte[] right = new byte[4];
            Array.Copy(block, 0, left, 0, 4);
            Array.Copy(block, 4, right, 0, 4);

            for (int i = 0; i < 16; i++)
            {
                byte[] temp = right;
                right = XOR(left, Feistel(right, key));
                left = temp;
            }

            byte[] output = new byte[8];
            Array.Copy(left, 0, output, 0, 4);
            Array.Copy(right, 0, output, 4, 4);
            return output;
        }

        //Do spawdzenia = każde źródło podaje innego feistela
        private static byte[] Feistel(byte[] halfBlock, byte[] key) //
        {
            byte[] result = new byte[halfBlock.Length];
            for (int i = 0; i < halfBlock.Length; i++)
            {
                result[i] = (byte)(halfBlock[i] ^ key[i % key.Length]);
            }
            return result;
        }

        //Metoda dokonująca operacji XOR na zadanych bajtach
        private static byte[] XOR(byte[] a, byte[] b)
        {
            byte[] result = new byte[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                result[i] = (byte)(a[i] ^ b[i]);
            }
            return result;
        }
    }
}
