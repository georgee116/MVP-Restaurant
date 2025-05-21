// Restaurant.Domain/AppSettings.cs
using System;
using System.IO;
using System.Text.Json;

namespace Restaurant.Domain
{
    public class AppSettings
    {
        private static AppSettings? _instance;
        private static readonly object _lock = new();

        // Singleton instance
        public static AppSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = Load() ?? new AppSettings();
                        }
                    }
                }
                return _instance;
            }
        }

        // Configurări pentru meniuri
        public decimal MeniuDiscount { get; set; } = 0.15m; // Reducere de 15% pentru meniuri

        // Configurări pentru comenzi
        public decimal TransportPriceLimitFree { get; set; } = 50m; // Transport gratuit peste 50 lei
        public decimal TransportPrice { get; set; } = 10m; // Cost transport standard
        public decimal OrderValueDiscountLimit { get; set; } = 100m; // Discount pentru comenzi peste 100 lei
        public decimal OrderValueDiscountPercent { get; set; } = 0.1m; // Discount 10% pentru comenzi mari

        // Configurări pentru monitorizare stoc
        public float LowStockThreshold { get; set; } = 1000; // g sau ml

        // Pentru discount la comenzi frecvente
        public int FrequentOrderCount { get; set; } = 5; // Număr de comenzi
        public int FrequentOrderDays { get; set; } = 30; // În perioada (zile)
        public decimal FrequentOrderDiscountPercent { get; set; } = 0.05m; // 5% discount pentru clienți fideli

        // Metode pentru încărcare/salvare configurare
        private static AppSettings? Load()
        {
            try
            {
                string configPath = GetConfigFilePath();

                if (File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    return JsonSerializer.Deserialize<AppSettings>(json);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
            }

            return null;
        }

        public void Save()
        {
            try
            {
                string configPath = GetConfigFilePath();
                string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(configPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
            }
        }

        private static string GetConfigFilePath()
        {
            string appFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "RestaurantApp");

            if (!Directory.Exists(appFolder))
                Directory.CreateDirectory(appFolder);

            return Path.Combine(appFolder, "settings.json");
        }
    }
}