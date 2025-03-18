using System.Text.RegularExpressions;

namespace Sana09;

class Program
{
    public static class HttpHelper
    {
        public static readonly HttpClient HttpClientBoxOffice = new();
        public static readonly string BoxOfficeUrl = "https://www.boxofficemojo.com/year/2024/?area=UA&grossesOption=totalGrosses";
    }
    
    // Ukrainian Box Office For 2024
    static public async Task<int> GetTotalGrosses()
    {
        Console.WriteLine("Починаємо завантаження  Box Office в Україні 2024...");
        try
        {
            var response = await HttpHelper.HttpClientBoxOffice.GetStringAsync(HttpHelper.BoxOfficeUrl);
            int total = 0;

            string pattern = @"\$[\d,]+";

            MatchCollection matches = Regex.Matches(response, pattern);

            foreach (Match match in matches)
            {
                string numberString = match.Value.Replace("$", "").Replace(",", "");
                if (int.TryParse(numberString, out int value))
                {
                    total += value;
                }
            }
            
            return total;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Не вдалося отримати дані: {ex.Message}");
            return 0;
        }
    }

    // All Movies in Ukrainian Box Office For 2024
    static public async Task<int> CountMoviesOnPage()
    {
        Console.WriteLine("Починаємо підрахунок усіх фільмів в Box Office в Україні 2024...");
        try
        {
            var response = await HttpHelper.HttpClientBoxOffice.GetStringAsync(HttpHelper.BoxOfficeUrl);
            if (string.IsNullOrWhiteSpace(response))
            {
                Console.WriteLine("Помилка: Порожня відповідь від сервера.");
                return 0;
            }

            string pattern = @"<td class=""a-text-left mojo-field-type-release mojo-cell-wide""[^>]*><a class=""a-link-normal""[^>]*>(.*?)</a></td>";
        
            int totalPages = Regex.Matches(response, pattern).Count;
        
            return totalPages;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Не вдалося підрахувати фільми: {ex.Message}");
            return 0;
        }
    }
    
    // Current temperature
    static public async Task<string> GetCurrentTemp()
    {
        Console.WriteLine("Починаємо отримання температури...");
        try
        {
            using HttpClient httpClient = new();
            string url = "https://meteofor.com.ua/weather-kyiv-4944/";
            var response = await httpClient.GetStringAsync(url);

            string pattern = @"<temperature-value[^>]*value=""(-?\d+)""";

            Match match = Regex.Match(response, pattern);

            if (match.Success)
            {
                string temperature = match.Groups[1].Value + "°C";
                return temperature;
            }
            else
            {
                throw new Exception("Не вдалося знайти температуру");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Не вдалося отримати температуру: {ex.Message}");
            return "Н/Д"; 
        }
    }
    
    // Count symbol in string
    static public int AmountOfSymbolInStr(string input)
    {
        string pattern = @"\S+"; 
        MatchCollection matches = Regex.Matches(input, pattern);

        int count = 0;
        foreach (Match match in matches)
        {
            count += match.Value.Length;
        }
        
        return count;
    }
    static async Task Main()
    {
        Console.WriteLine("Початок роботи програми!");
        
        var tasks = new List<Task<object>>
        {
            Task.Run<object>(async () => await GetTotalGrosses()),
            Task.Run<object>(async () => await CountMoviesOnPage()),
            Task.Run<object>(async () => await GetCurrentTemp())
        };
        
        // Демонстрація, що асинхронний код не блокує виконання основного потоку
        string text = "Hello World!";
        int total = AmountOfSymbolInStr(text);
        Console.WriteLine($"Головний потік працює! Кількість символів у рядку: {total}");
        
        var results = await Task.WhenAll(tasks);
        Console.WriteLine("Всі запити завершено!");
        
        Console.WriteLine($"Загальний BoxOffice: ${(int)results[0]}");
        Console.WriteLine($"Кількість фільмів: {(int)results[1]}");
        Console.WriteLine($"Температура у Києві: {(string)results[2]}");
    }
}