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
                using var response = await request.PutAsync(url, JsonContent.Create(new
                {
                    result = new TeamScore[]
                    {
                        new() { TeamID = 0, Score = scores[0], },
                        new() { TeamID = 1, Score = scores[1], },
                    },
                    mode
                }));
                GameServerLogging.logger.ConsoleLog("Send to web successfully!");
                GameServerLogging.logger.ConsoleLog($"Web response: {await response.Content.ReadAsStringAsync()}");
            }
            catch (Exception e)
            {
                GameServerLogging.logger.ConsoleLog("Fail to send msg to web!");
                GameServerLogging.logger.ConsoleLog(e.ToString());
            }
        }
    }

    internal class TeamScore
    {
        public int TeamID { get; set; } = 0;
        public int Score { get; set; } = 0;
    }
}
