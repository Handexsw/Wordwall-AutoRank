using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Enter ARRAffinity cookie:");
        string? arrAffinity = Console.ReadLine();

        Console.WriteLine("Enter ARRAffinitySameSite cookie:");
        string? arrAffinitySameSite = Console.ReadLine();

        Console.WriteLine("Enter __RequestVerificationToken cookie:");
        string? requestVerificationToken = Console.ReadLine();

        string cookies = $"{arrAffinity}; {arrAffinitySameSite}; {requestVerificationToken};";

        Console.WriteLine("Enter Wordwall activity ID (e.g., 1234 from wordwall.net/resource/1234):");
        int activityId = int.Parse(Console.ReadLine());

        Console.WriteLine("Enter template ID (default - 46):");
        int templateId = int.Parse(Console.ReadLine());

        Console.WriteLine("Enter score:");
        int score = int.Parse(Console.ReadLine());

        Console.WriteLine("Enter time:");
        int time = int.Parse(Console.ReadLine());

        Console.WriteLine("Enter name:");
        string? name = Console.ReadLine();

        await AddToLeaderboard(cookies, activityId, templateId, score, time, name);
    }

    static async Task AddToLeaderboard(string cookies, int activityId, int templateId, int score, int time, string name)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 10; K) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36");
            client.DefaultRequestHeaders.Add("Cookie", cookies);

            var payload = new
            {
                score = score.ToString(),
                time = time.ToString(),
                name = name,
                mode = "1",
                activityId = activityId.ToString(),
                templateId = templateId.ToString()
            };

            string jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            HttpResponseMessage addEntryResponse = await client.PostAsync("https://wordwall.net/leaderboardajax/addentry", content);

            if (addEntryResponse.IsSuccessStatusCode)
            {
                HttpResponseMessage getEntriesResponse = await client.GetAsync($"https://wordwall.net/leaderboardajax/getentries?activityId={activityId}&templateId={templateId}");

                if (getEntriesResponse.IsSuccessStatusCode)
                {
                    string addEntryResponseText = await addEntryResponse.Content.ReadAsStringAsync();
                    Console.WriteLine("[+] The new participant has been successfully added to the leaderboard.", addEntryResponseText.Length < 3 ? addEntryResponseText : "");
                }
                else
                {
                    Console.WriteLine("[-] An error occurred while fetching the leaderboard.");
                }
            }
            else
            {
                Console.WriteLine("[-] An error occurred while adding the participant.");
            }
        }
    }
}
