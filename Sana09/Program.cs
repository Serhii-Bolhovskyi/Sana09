using System.Text.RegularExpressions;

namespace Sana09;

class Program
{
    // Ukrainian Box Office For 2024
    static public async Task<int> GetTotalGrosses()
    {
        using HttpClient httpClient = new();
        string url = "https://www.boxofficemojo.com/year/2024/?area=UA&grossesOption=totalGrosses";
        
        var response = await httpClient.GetStringAsync(url);
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
    static async Task Main()
    {
        Console.WriteLine("Загальну сума BoxOffice в Україні 2024");
        var totalGrosses = await GetTotalGrosses();
        Console.WriteLine($"Загальний BoxOffice: ${totalGrosses}");
    }
}