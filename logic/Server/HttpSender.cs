using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace Server
{
    class HttpSender(string url, string token)
    {
        private string url = url;
        private string token = token;

        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        public string Token
        {
            get { return token; }
            set { token = value; }
        }

        // void Test()
        // {
        //     this.SendHttpRequest(new()).Wait();
        // }
        public async Task SendHttpRequest(int[] scores, string state)
        {
            try
            {
                var request = new HttpClient();
                request.DefaultRequestHeaders.Authorization = new("Bearer", token);
                using var response = await request.PostAsync(url, JsonContent.Create(new
                {
                    status = state,
                    scores = new int[] { scores[0], scores[1] },
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

        public async Task<double[]> GetLadderScore(double[] scores)
        {
            try
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new("Bearer", token);
                var response = await httpClient.PostAsync(url, null);

                // 读取响应内容为字符串
                var jsonString = await response.Content.ReadAsStringAsync();

                // 解析 JSON 字符串
                var result = JsonConvert.DeserializeObject<ResponseResult>(jsonString);
                return result.Scores.Select(score => (double)score).ToArray();;
            }
            catch (Exception e)
            {
                GameServerLogging.logger.ConsoleLog("Error when pulling ladder score!");
                GameServerLogging.logger.ConsoleLog(e.ToString());
                return new double[0];
            }

        }
    }

    internal class TeamScore
    {
        public int TeamID { get; set; } = 0;
        public int Score { get; set; } = 0;
    }
}
