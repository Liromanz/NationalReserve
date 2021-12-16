using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace NationalReserve.Helpers
{
    public class CsvHelper
    {
        public static void WriteCSV(List<string> tableList, string tableName)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            if (folder.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists($"{folder.SelectedPath}\\{tableName}.csv"))
                    File.Delete($"{folder.SelectedPath}\\{tableName}.csv");

                using (StreamWriter sw = File.CreateText($"{folder.SelectedPath}\\{tableName}.csv"))
                {
                    foreach (string item in tableList)
                        sw.WriteLine(item);
                }

                MessageBox.Show("Экспорт таблицы завершен!");
            }
        }

        public static List<string[]> ReadCSV(int propertyAmount)
        {
            List<string[]> csvMatrix = new List<string[]>();

            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "CSV Файлы|*.csv";
            if (file.ShowDialog() == DialogResult.OK)
            {
                using (StreamReader sr = File.OpenText(file.FileName))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        csvMatrix.Add(line.Split(", "));
                    }
                }
            }

            var result = CSVValidation(csvMatrix, propertyAmount);
            if (result != null)
                MessageBox.Show(result);
            return csvMatrix;
        }

        public static string CSVValidation(List<string[]> itemToCheck, int amount)
        {
            if (!itemToCheck.Any()) return "Файл пустой";
            if (itemToCheck.First().Length == amount) return "Выбранный файл не подходит для этой таблицы";
            return null;
        }
    }
}
