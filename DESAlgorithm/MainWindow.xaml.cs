using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Printing.IndexedProperties;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Media3D;

namespace DESAlgorithm
{
    public partial class MainWindow : Window
    {
        private static byte[] key = {0,1,1,1,0,0,1,0, 0,1,1,0,0,1,0,1, 0,1,1,1,0,1,1,1, 0,1,1,0,1,1,1,1, 0,1,1,0,1,1,0,0, 0,1,1,1,0,1,1,1, 0,1,1,0,0,1,0,1, 0,1,1,1,0,0,1,0, 0,0,0,0,1,0,1,0};
        private static byte[] initialPermutation = {58, 50, 42, 34, 26, 18, 10, 2, 60, 52, 44, 36, 28, 20, 12, 4, 62, 54, 46, 38, 30, 22, 14, 6, 64, 56, 48, 40, 32, 24, 16, 8, 57, 49, 41, 33, 25, 17, 9, 1, 59, 51, 43, 35, 27, 19, 11, 3, 61, 53, 45, 37, 29, 21, 13, 5, 63, 55, 47, 39, 31, 23, 15, 7};
        private static byte[] finalPermutation = {40, 8, 48, 16, 56, 24, 64, 32, 39, 7, 47, 15, 55, 23, 63, 31, 38, 6, 46, 14, 54, 22, 62, 30, 37, 5, 45, 13, 53, 21, 61, 29,36, 4, 44, 12, 52, 20, 60, 28, 35, 3, 43, 11, 51, 19, 59, 27, 34, 2, 42, 10, 50, 18, 58, 26, 33, 1, 41, 9, 49, 17, 57, 25 };
        private static byte[] compressionPermutation = { 14, 17, 11, 24, 1, 5, 3, 28, 15, 6, 21, 10, 23, 19, 12, 4, 26, 8, 16, 7, 27, 20, 13, 2, 41, 52, 31, 37, 47, 55, 30, 40, 51, 45, 33, 48, 44, 49, 39, 56, 34, 53, 46, 42, 50, 36, 29, 32 };
        private static byte[] keyPermutation = { 57, 49, 41, 33, 25, 17, 9, 1, 58, 50, 42, 34, 26, 18, 10, 2, 59, 51, 43, 35, 27, 19, 11, 3, 60, 52, 44, 36, 63, 55, 47, 39, 31, 23, 15, 7, 62, 54, 46, 38, 30, 22, 14, 6, 61, 53, 45, 37, 29, 21, 13, 5, 28, 20, 12, 4 };
        private static byte[] expansionPermutation = { 32, 1, 2, 3, 4, 5, 4, 5, 6, 7, 8, 9, 8, 9, 10, 11, 12, 13, 12, 13, 14, 15, 16, 17, 16, 17, 18, 19, 20, 21, 20, 21, 22, 23, 24, 25, 24, 25, 26, 27, 28, 29, 28, 29, 30, 31, 32, 1 };
        private static byte[] pBoxPermutation = { 16, 7, 20, 21, 29, 12, 28, 17, 1, 15, 23, 26, 5, 18, 31, 10, 2, 8, 24, 14, 32, 27, 3, 9, 19, 13, 30, 6, 22, 11, 4, 25 };
        private static byte[] keyShifts = { 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 };
        private static byte[][] SBoxes = new byte[][]{
            new byte[]{14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7,  0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8,  4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0,  15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13},
            new byte[]{15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10,  3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5,  0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15,  13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9},
            new byte[]{10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8,  13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1,  13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7,  1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12},
            new byte[]{7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15,  13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9,  10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4,  3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14},
            new byte[]{2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9,  14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6,  4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14,  11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3},
            new byte[]{12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11,  10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8,  9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6,  4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13},
            new byte[]{4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1,  13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6,  1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2,  6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12},
            new byte[]{13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7,  1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2,  7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8,  2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11}
        };
        private static byte[][] subKeys = new byte[16][];
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
            string text = OutputText.Text;
            InputText.Text = Decrypt(text);
        }

        //Obsługa przycisku
        private void EncryptFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string encryptedFilePath = filePath + ".enc";
                EncryptFile(filePath, encryptedFilePath);
            }
        }

        //Obsługa przycisku
        private void DecryptFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Encrypted Files (*.enc)|*.enc";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string decryptedFilePath = filePath.Replace(".enc", ".dec");
                DecryptFile(filePath, decryptedFilePath);
            }
        }

        public static string Encrypt(string text)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            return Convert.ToBase64String(DESAlgorithm(data,true));
        }

        public static string Decrypt(string encryptedText)
        {
            byte[] data = Convert.FromBase64String(encryptedText);
            return Encoding.ASCII.GetString(DESAlgorithm(data,false));
        }

        public static void EncryptFile(string inputFilePath, string outputFilePath)
        {
                byte[] fileData = File.ReadAllBytes(inputFilePath);
                byte[] encryptedData = DESAlgorithm(fileData, true);
                File.WriteAllBytes(outputFilePath, encryptedData);
                MessageBox.Show("File has been encrypted.");
        }


        public static void DecryptFile(string inputFilePath, string outputFilePath)
        {
                byte[] encryptedData = File.ReadAllBytes(inputFilePath);
                byte[] decryptedData = DESAlgorithm(encryptedData, false);
                File.WriteAllBytes(outputFilePath, decryptedData);
                MessageBox.Show("File has been decrypted.");
        }



        //Metoda przerabia tekst wpisany przez uzytkownika na bloki bajtów
        private static byte[] DESAlgorithm(byte[] data, bool toEncryption) 
        {
            data = fillMissingBits(data);
            byte[] permutatedKey = Permutate(key, keyPermutation);
            byte[] leftKeyPart = new byte[permutatedKey.Length/2];
            byte[] rightKeyPart = new byte[permutatedKey.Length/2];

            for (int i = 0; i < permutatedKey.Length / 2; i++)
            {
                leftKeyPart[i] = permutatedKey[i];
            }

            for (int i = 0; i < permutatedKey.Length / 2; i++)
            {
                rightKeyPart[i] = permutatedKey[i+permutatedKey.Length/2];
            }

            subKeys = GetSubKeys(leftKeyPart,rightKeyPart);

            byte[] processedData = new byte[data.Length];
            for (int i = 0; i < data.Length; i += 8)
            {
                byte[] block = new byte[8];
                Array.Copy(data, i, block, 0, 8);
                byte[] resultBlock = ProcessBlock(block, toEncryption);
                Array.Copy(resultBlock, 0, processedData, i, 8);
            }
            if (!toEncryption)
            {
                processedData = removeAddedBits(processedData);
            }
            return processedData;
        }

        //Metoda dokonuje permutacji na tablicy bitów zgodznie z zadanym rodzajem permutacji
        private static byte[] Permutate(byte[] tab, byte[] permutationTab)
        {
            byte[] temp = new byte[permutationTab.Length];

            for (int i = 0; i < permutationTab.Length; i++)
            {
                temp[i] = tab[permutationTab[i] - 1];
            }
            return temp;
        }

        //Metoda tworząca nową obróconą tablice w lewą stronę o zadaną ilość miejsc
        private static byte[] RotateLeft(byte[] tab, byte howMany) {
            byte[] temp = new byte[tab.Length];
            for (int i = 0; i < tab.Length; i++)
            {
                if (i - keyShifts[howMany] < 0)
                {
                    temp[tab.Length + (i - keyShifts[howMany])] = tab[i];
                }
                else
                {
                    temp[i - keyShifts[howMany]] = tab[i];
                }
            }
            return temp;
        }

        //Metoda wykonująca szereg przekształceń w celu otrzymania podkluczy
        private static byte[][] GetSubKeys(byte[] leftSide, byte[] rightSide) {
           
            byte[][] keys = new byte[16][];
            
            for(int i = 0; i < 16; i++)
            {
                leftSide = RotateLeft(leftSide, keyShifts[i]);
                rightSide = RotateLeft(rightSide, keyShifts[i]);
                byte[] temp = new byte[leftSide.Length+rightSide.Length];
                Array.Copy(leftSide, 0, temp, 0, leftSide.Length);
                Array.Copy(rightSide, 0, temp, rightSide.Length, rightSide.Length);
                keys[i] = Permutate(temp, compressionPermutation);
            }
            return keys;
        }

   
        //Metoda dzieli pełny 8 bajtowy blok na dwa 4 bajtowe (lewy i prawy) i pozniej stosuje sieć Feistela
        private static byte[] ProcessBlock(byte[] block , bool toEncryption)
        {
            int keyDirection = toEncryption ? 1 : -1;
            int keyOffset = toEncryption ? 0 : 15;

            block = ConvertToBinary(block,8);
            block = Permutate(block,initialPermutation);
            byte[] left = new byte[block.Length/2];
            byte[] right = new byte[block.Length / 2];
            Array.Copy(block, 0, left, 0, block.Length / 2);
            Array.Copy(block, block.Length / 2, right, 0, block.Length / 2);

            for (int i = 0; i < 16; i++)
            {
                byte[] temp = right;
                right = Permutate(right, expansionPermutation);
                right = XOR(right, subKeys[keyOffset + i * keyDirection]);

                byte[][] sBoxGroups = convertToGroups(right, 8, 6);
                byte[] sBoxValues = new byte[8];

                for (int j = 0; j < sBoxGroups.Length; j++)
                {
                    sBoxValues[j] = (SBoxes[j][getSboxIndex(sBoxGroups[j])]);
                }

                byte[] substitutionResult = ConvertToBinary(sBoxValues, 4);
                byte[] pBox = Permutate(substitutionResult, pBoxPermutation);
                right = XOR(left, pBox);
                left = temp;
            }

            byte[] output = new byte[64];
            Array.Copy(right, 0, output, 0, 32);
            Array.Copy(left, 0, output, 32, 32);
            output = Permutate(output, finalPermutation);
            output = convertBitsToBytes(output);

            return output;
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

        //Metoda konwertuje tablice bajtow na tablice binarna
        private static byte[] ConvertToBinary(byte[] value, int size)
        {
            List<byte> wynik = new List<byte>();

            foreach (int liczba in value)
            {
                for (int i = size - 1; i >= 0; i--)
                {
                    wynik.Add((byte)((liczba >> i) & 1)); 
                }
            }

            return wynik.ToArray();
        }

        //Metoda konwetuje tablice binarna na liczbe całkowita
        public static int ConvertToInt(byte[] tab)
        {
            if (tab == null || tab.Length == 0)
            {
                return 0;
            }

            int result = 0;
            for (int i = 0; i < tab.Length; i++)
            {
                result = (result << 1) | tab[i];
            }
            return result;
        }

        //Metoda konwertuje tablice na kilka mniejszych
        private static byte[][] convertToGroups(byte[] tab, int howManyGroups, int sizeOfGroup)
        {
            byte[][] result = new byte[howManyGroups][];
            for (int i = 0; i < howManyGroups; i++)
            {
                result[i] = new byte[sizeOfGroup];
                for (int j = 0; j < sizeOfGroup; j++)
                {
                    result[i][j] = tab[i * sizeOfGroup + j];
                }
            }
            return result;
        }

        //Metoda oblicza index do S-Boxa
        private static int getSboxIndex(byte[] group) {
            byte[] column = new byte[] { group[1], group[2], group[3], group[4] };
            byte[] row = new byte[] { group[0], group[5] };

            return ConvertToInt(row) * 16 + ConvertToInt(column);
        }

        //Metoda konwertuje tablice skladajaca sie z bitow do tablicy skladajacej sie z bajtow
        static byte[] convertBitsToBytes(byte[] bits)
        {
            byte[] bytes = new byte[bits.Length / 8];
            for (int i = 0; i < bytes.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    bytes[i] = (byte)((bytes[i] << 1) | bits[i * 8 + j]);
                }
            }
            return bytes;
        }

        //Metoda dodająca brakujące bajty jeśli wpisana przez uzytkownika wiadomosc nie jest wielokrotnosca 8 bajtow
        private static byte[] fillMissingBits(byte[] data)
        {
            int howManyMissing = 8 - (data.Length % 8);
            if (howManyMissing == 8)
            {
                return data;
            }

            byte[] filledBits = new byte[data.Length + howManyMissing];
            Array.Copy(data, filledBits, data.Length);
            for (int i = data.Length; i < filledBits.Length; i++)
            {
                filledBits[i] = (byte)howManyMissing;
            }
            return filledBits;
        }

        //Metoda usuwa dodatkowe bajty dodane przez metode fillMissingBits
        private static byte[] removeAddedBits(byte[] data)
        {
            int howManyAdded = data[data.Length - 1];
            if (howManyAdded >= 1 && howManyAdded <= 8)
            {
                return data.Take(data.Length - howManyAdded).ToArray();
            }
            return data;
        }

    }
}
