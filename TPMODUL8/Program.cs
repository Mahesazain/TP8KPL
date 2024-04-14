using System;
using System.IO;
using Newtonsoft.Json;

internal class Program
{
    private static void Main(string[] args)
    {
        double suhuBadan;
        int hariDemam;

        CovidData defaultConf = new CovidData();

        Console.Write($"Berapa suhu badan Anda saat ini? Dalam nilai {defaultConf.CovidConf.SatuanSuhu}: ");
        if (!double.TryParse(Console.ReadLine(), out suhuBadan))
        {
            Console.WriteLine("Input tidak valid. Harap masukkan nilai numerik untuk suhu badan.");
            return;
        }

        Console.Write("Berapa hari yang lalu (perkiraan) anda terakhir memiliki gejala demam? ");
        if (!int.TryParse(Console.ReadLine(), out hariDemam))
        {
            Console.WriteLine("Input tidak valid. Harap masukkan nilai numerik untuk jumlah hari demam.");
            return;
        }

        bool tepatWaktu = hariDemam < defaultConf.CovidConf.BatasHariDemam;
        bool terimaFahrenheit = (defaultConf.CovidConf.SatuanSuhu == "Fahrenheit") && (suhuBadan >= 97.7 && suhuBadan <= 99.5);
        bool terimaCelcius = (defaultConf.CovidConf.SatuanSuhu == "Celcius") && (suhuBadan >= 36.5 && suhuBadan <= 37.5);

        if (tepatWaktu && (terimaCelcius || terimaFahrenheit))
        {
            Console.WriteLine(defaultConf.CovidConf.PesanDiterima);
        }
        else
        {
            Console.WriteLine(defaultConf.CovidConf.PesanDitolak);
        }
    }
}

public class CovidConfig
{
    public string SatuanSuhu { get; set; }
    public int BatasHariDemam { get; set; }
    public string PesanDitolak { get; set; }
    public string PesanDiterima { get; set; }

    public CovidConfig() { }
    public CovidConfig(string satuanSuhu, int batasHariDemam, string pesanDitolak, string pesanDiterima)
    {
        SatuanSuhu = satuanSuhu;
        BatasHariDemam = batasHariDemam;
        PesanDitolak = pesanDitolak;
        PesanDiterima = pesanDiterima;
    }
}

public class CovidData
{
    public CovidConfig CovidConf { get; private set; }

    private static string FilePath
    {
        get
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string fileName = "covid_config.json";
            return Path.Combine(currentDirectory, fileName);
        }
    }

    public CovidData()
    {
        try
        {
            ReadConfig();
        }
        catch (FileNotFoundException)
        {
            WriteNewConfig();
        }
        catch (JsonException)
        {
            Console.WriteLine("File konfigurasi tidak valid. Membuat konfigurasi default.");
            WriteNewConfig();
        }
    }

    private void ReadConfig()
    {
        if (File.Exists(FilePath))
        {
            string jsonData = File.ReadAllText(FilePath);
            CovidConf = JsonConvert.DeserializeObject<CovidConfig>(jsonData);
        }
        else
        {
            WriteNewConfig();
        }
    }

    private void WriteNewConfig()
    {
        CovidConf = new CovidConfig("Celcius", 14, "Anda tidak dapat masuk", "Anda dapat masuk");
        string jsonString = JsonConvert.SerializeObject(CovidConf, Formatting.Indented);
        File.WriteAllText(FilePath, jsonString);
    }

    public void UbahSatuan(string satuanBaru)
    {
        bool satuanValid = (satuanBaru == "Celcius" || satuanBaru == "Fahrenheit");

        if (!satuanValid)
        {
            throw new ArgumentException("Satuan suhu tidak valid.");
        }

        CovidConf.SatuanSuhu = satuanBaru;
        WriteNewConfig();
    }
}
