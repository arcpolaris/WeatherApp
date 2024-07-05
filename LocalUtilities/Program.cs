namespace LocalUtilities
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string sourcePath = @"C:\Users\dsrivastava\source\repos\WeatherApp\LocalUtilities\SirenData.csv";
            string dumpPath = @"C:\Users\dsrivastava\source\repos\WeatherApp\LocalUtilities\SirenData.json";
            CSVHelper csv = new(sourcePath);
            //var res = csv.HeaderTable;
            //foreach (var entry in res)
            //{
            //    Console.Write(entry.Key + ": ");
            //    foreach (var item in entry.Value) Console.Write(item + ", ");
            //    Console.WriteLine();
            //}
            File.WriteAllText(dumpPath, csv.ToJson());
        }
    }
}
