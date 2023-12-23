using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace installer.Model
{
    [Serializable]
    record LoginResponse
    {
        // Map `Token` to `token` when serializing

        public string Token { get; set; } = "";
    }

    public class EEsast
    {
        public enum LangUsed { cpp, py };
        private string token = string.Empty;
        public string Token
        {
            get => token; protected set
            {
                if (token != value)
                    Token_Changed?.Invoke(this, new EventArgs());
                token = value;
            }
        }
        public event EventHandler? Token_Changed;
        public string ID { get; protected set; } = string.Empty;
        public string Email { get; protected set; } = string.Empty;

        public ConcurrentQueue<Exception> Exceptions = new ConcurrentQueue<Exception>();
        public enum WebStatus
        {
            disconnected, offline, logined
        }
        public WebStatus Status = WebStatus.disconnected;
        public Tencent_Cos EEsast_Cos { get; protected set; } = new Tencent_Cos("1255334966", "ap-beijing", "eesast");
        public async Task LoginToEEsast(HttpClient client, string useremail, string userpassword)
        {
            try
            {
                using (var response = await client.PostAsync("https://api.eesast.com/users/login", JsonContent.Create(new
                {
                    email = useremail,
                    password = userpassword,
                })))
                {
                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.OK:
                            var info = JsonSerializer.Deserialize<Dictionary<string, string>>(await response.Content.ReadAsStringAsync());
                            ID = info.Keys.Contains("_id") ? info["_id"] : string.Empty;
                            Email = info.Keys.Contains("email") ? info["email"] : string.Empty;
                            Token = info.Keys.Contains("token") ? info["token"] : string.Empty;
                            Status = WebStatus.logined;
                            break;
                        default:
                            int code = ((int)response.StatusCode);
                            if (code == 401)
                            {
                                Exceptions.Enqueue(new Exception("邮箱或密码错误！"));
                            }
                            else
                            {
                                Exceptions.Enqueue(new Exception($"HTTP错误，错误码：{code}"));
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.Enqueue(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client">http client</param>
        /// <param name="tarfile">代码源位置</param>
        /// <param name="type">编程语言，格式为"cpp"或"python"</param>
        /// <param name="plr">第x位玩家，格式为"player_x"</param>
        /// <returns>-1:tokenFail;-2:FileNotExist;-3:CosFail;-4:loginTimeout;-5:Fail;-6:ReadFileFail;-7:networkError</returns>
        async public Task<int> UploadFiles(HttpClient client, string tarfile, string type, string plr)    //用来上传文件
        {
            if (Status != WebStatus.logined)   // 
            {
                Exceptions.Append(new UnauthorizedAccessException("用户未登录"));
                return -1;
            }
            try
            {
                string content;
                client.DefaultRequestHeaders.Authorization = new("Bearer", Token);
                if (!File.Exists(tarfile))
                {
                    Exceptions.Append(new IOException("用户不存在"));
                    return -2;
                }
                using FileStream fs = new FileStream(tarfile, FileMode.Open, FileAccess.Read);
                using StreamReader sr = new StreamReader(fs);
                content = sr.ReadToEnd();
                string targetUrl = $"https://api.eesast.com/static/player?team_id={await GetTeamId()}";
                using (var response = await client.GetAsync(targetUrl))
                {
                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.OK:
                            var res = JsonSerializer.Deserialize<Dictionary<string, string>>(await response.Content.ReadAsStringAsync());
                            string tmpSecretId = res["TmpSecretId"];        //"临时密钥 SecretId";
                            string tmpSecretKey = res["TmpSecretKey"];      //"临时密钥 SecretKey";
                            string tmpToken = res["SecurityToken"];         //"临时密钥 token";
                            long tmpExpiredTime = Convert.ToInt64(res["ExpiredTime"]);  //临时密钥有效截止时间，精确到秒
                            EEsast_Cos.UpdateSecret(tmpSecretId, tmpSecretKey, tmpExpiredTime, tmpToken);

                            string cosPath = $"/THUAI7/{GetTeamId()}/{type}/{plr}"; //对象在存储桶中的位置标识符，即称对象键
                            string srcPath = tarfile;//本地文件绝对路径
                            EEsast_Cos.UploadFileAsync(srcPath, cosPath).Wait();

                            break;
                        case System.Net.HttpStatusCode.Unauthorized:
                            //Console.WriteLine("您未登录或登录过期，请先登录");
                            return -4;
                        default:
                            //Console.WriteLine("上传失败！");
                            return -5;
                    }
                }
            }
            catch (IOException)
            {
                //Console.WriteLine("文件读取错误！请检查文件是否被其它应用占用！");
                return -6;
            }
            catch
            {
                //Console.WriteLine("请求错误！请检查网络连接！");
                return -7;
            }
            return 0;
        }

        async public Task UserDetails(HttpClient client)  // 用来测试访问网站
        {
            if (Status != WebStatus.logined)  // 读取token失败
            {
                Exceptions.Append(new UnauthorizedAccessException("用户未登录"));
                return;
            }
            try
            {
                client.DefaultRequestHeaders.Authorization = new("Bearer", Token);
                Console.WriteLine(Token);
                using (var response = await client.GetAsync("https://api.eesast.com/application/info"))
                {
                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.OK:
                            Console.WriteLine("Require OK");
                            Console.WriteLine(await response.Content.ReadAsStringAsync());
                            break;
                        default:
                            int code = ((int)response.StatusCode);
                            if (code == 401)
                            {
                                Console.WriteLine("您未登录或登录过期，请先登录");
                            }
                            return;
                    }
                }
            }
            catch
            {
                Console.WriteLine("请求错误！请检查网络连接！");
            }
        }

        async public Task<string?> GetTeamId()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.eesast.com/dev/v1/graphql");
            request.Headers.Add("x-hasura-admin-secret", "hasuraDevAdminSecret");
            //var content = new StringContent($@"
            //    {{
            //        ""query"": ""query MyQuery {{contest_team_member(where: {{user_id: {{_eq: \""{Downloader.UserInfo._id}\""}}}}) {{ team_id  }}}}"",
            //        ""variables"": {{}},
            //    }}", null, "application/json");
            var content = new StringContent("{\"query\":\"query MyQuery {\\r\\n  contest_team_member(where: {user_id: {_eq: \\\"" + ID + "\\\"}}) {\\r\\n    team_id\\r\\n  }\\r\\n}\",\"variables\":{}}", null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var info = await response.Content.ReadAsStringAsync();
            var s1 = (info is null) ? null : JsonSerializer.Deserialize<Dictionary<string, string>>(info)["data"];
            var s2 = (s1 is null) ? null : JsonSerializer.Deserialize<Dictionary<string, List<string>>>(s1 ?? "")["contest_team_member"];
            var sres = (s2 is null) ? null : JsonSerializer.Deserialize<Dictionary<string, string>>(s2[0] ?? "")["team_id"];
            return sres;
        }

        public async Task<string> GetUserId(string learnNumber)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.eesast.com/dev/v1/graphql");
            request.Headers.Add("x-hasura-admin-secret", "hasuraDevAdminSecret");
            var content = new StringContent("{\"query\":\"query MyQuery {\r\n  user(where: {id: {_eq: \""
                + learnNumber + "\"}}) {\r\n    _id\r\n  }\r\n}\r\n\",\"variables\":{}}", null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
