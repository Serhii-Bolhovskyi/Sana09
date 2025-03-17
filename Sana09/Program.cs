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
        Console.WriteLine($"Загальний BoxOffice: ${total}");
        return total;
    }

    // All Movies in Ukrainian Box Office For 2024
    static public async Task<int> CountMoviesOnPage()
    {
        var response = await HttpHelper.HttpClientBoxOffice.GetStringAsync(HttpHelper.BoxOfficeUrl);
        if (string.IsNullOrWhiteSpace(response))
        {
            Console.WriteLine("Помилка: Порожня відповідь від сервера.");
            return 0;
        }

        string pattern = @"<td class=""a-text-left mojo-field-type-release mojo-cell-wide""[^>]*><a class=""a-link-normal""[^>]*>(.*?)</a></td>";
        
        int totalPages = Regex.Matches(response, pattern).Count;
        Console.WriteLine($"Загальну к-ть фільмів: {totalPages}");
        
        return totalPages;
    }
    static async Task Main()
    {
        var tasks = new List<Task>
        {
            GetTotalGrosses(),
            CountMoviesOnPage()
        };
        
        await Task.WhenAll(tasks);
        Console.WriteLine("Всі запити завершено!");
        
        
    }
}