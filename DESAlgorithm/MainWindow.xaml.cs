using System;
using System.Diagnostics;
using System.Printing.IndexedProperties;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DESAlgorithm
{
    public partial class MainWindow : Window
    {
        private static byte[] key = {0,1,1,1,0,0,1,0, 0,1,1,0,0,1,0,1, 0,1,1,1,0,1,1,1, 0,1,1,0,1,1,1,1, 0,1,1,0,1,1,0,0, 0,1,1,1,0,1,1,1, 0,1,1,0,0,1,0,1, 0,1,1,1,0,0,1,0, 0,0,0,0,1,0,1,0};
        private static byte[] permutacjaPoczatkowa = {58, 50, 42, 34, 26, 18, 10, 2, 60, 52, 44, 36, 28, 20, 12, 4, 62, 54, 46, 38, 30, 22, 14, 6, 64, 56, 48, 40, 32, 24, 16, 8, 57, 49, 41, 33, 25, 17, 9, 1, 59, 51, 43, 35, 27, 19, 11, 3, 61, 53, 45, 37, 29, 21, 13, 5, 63, 55, 47, 39, 31, 23, 15, 7};
        private static byte[] permutacjaOdwrotna = {40, 8, 48, 16, 56, 24, 64, 32, 39, 7, 47, 15, 55, 23, 63, 31, 38, 6, 46, 14, 54, 22, 62, 30, 37, 5, 45, 13, 53, 21, 61, 29,36, 4, 44, 12, 52, 20, 60, 28, 35, 3, 43, 11, 51, 19, 59, 27, 34, 2, 42, 10, 50, 18, 58, 26, 33, 1, 41, 9, 49, 17, 57, 25 };
        private static byte[] pertmutacjaKompresujaca = { 14, 17, 11, 24, 1, 5, 3, 28, 15, 6, 21, 10, 23, 19, 12, 4, 26, 8, 16, 7, 27, 20, 13, 2, 41, 52, 31, 37, 47, 55, 30, 40, 51, 45, 33, 48, 44, 49, 39, 56, 34, 53, 46, 42, 50, 36, 29, 32 };
        private static byte[] permutacjaKlucza = { 57, 49, 41, 33, 25, 17, 9, 1, 58, 50, 42, 34, 26, 18, 10, 2, 59, 51, 43, 35, 27, 19, 11, 3, 60, 52, 44, 36, 63, 55, 47, 39, 31, 23, 15, 7, 62, 54, 46, 38, 30, 22, 14, 6, 61, 53, 45, 37, 29, 21, 13, 5, 28, 20, 12, 4 };

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
            Trace.WriteLine(key[0] + " "+ key[1] + " " + key[2] + " " + key[3] + " " + key[4]);
            int x = 0 - 1;
            Trace.WriteLine("hejjjjj " + x);


            byte[] permutatedKey = Permutate(key, permutacjaKlucza);
            byte[] leftKeyPart = new byte[permutatedKey.Length/2];
            byte[] rightKeyPart = new byte[permutatedKey.Length/2];
            Trace.WriteLine("hejjjjj " + rightKeyPart.Length);


            for (int i = 0; i < permutatedKey.Length / 2; i++)
            {
                Trace.WriteLine("xd " + i);
                leftKeyPart[i] = permutatedKey[i];
            }

            for (int i = 0; i < permutatedKey.Length / 2; i++)
            {
                rightKeyPart[i] = permutatedKey[i+permutatedKey.Length/2];
            }

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

        private static byte[] Permutate(byte[] tab, byte[] permutationTab)
        {
            byte[] temp = new byte[permutationTab.Length];

            for (int i = 0; i < permutationTab.Length; i++)
            {
                temp[i] = tab[permutationTab[i] - 1];
                Trace.WriteLine(temp[i]);
            }
            return temp;
        }

        //Metoda tworząca nową obróconą tablice w lewą stronęo zadaną ilość miejsc
        private static byte[] RotateLeft(byte[] tab, int howMany) {
            byte[] temp = new byte[tab.Length];

            for (int i = 0; i < tab.Length; i++)
            {
                if (i - howMany < 0)
                {
                    temp[tab.Length + (i - howMany)] = tab[i];
                }
                else
                {
                    temp[i - howMany] = tab[i];
                }
            }
            return temp;
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
