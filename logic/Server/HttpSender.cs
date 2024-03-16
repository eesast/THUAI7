using System.Net.Http.Json;

namespace Server
{
    class HttpSender(string url, string token)
    {
        private readonly string url = url;
        private readonly string token = token;

        // void Test()
        // {
        //     this.SendHttpRequest(new()).Wait();
        // }
        public async Task SendHttpRequest(int[] scores, int mode)
        {
            try
            {
                var request = new HttpClient();
                request.DefaultRequestHeaders.Authorization = new("Bearer", token);
                using (var response = await request.PutAsync(url, JsonContent.Create(new
                {
                    result = new TeamScore[]
                    {
                        new() { team_id = 0, score = scores[0], },
                        new() { team_id = 1, score = scores[1], },
                    },
                    mode = mode
                })))
                {
                    Console.WriteLine("Send to web successfully!");
                    Console.WriteLine($"Web response: {await response.Content.ReadAsStringAsync()}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Fail to send msg to web!");
                Console.WriteLine(e);
            }
        }
    }

    internal class TeamScore
    {
        public int team_id { get; set; } = 0;
        public int score { get; set; } = 0;
    }
}
