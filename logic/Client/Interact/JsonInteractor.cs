using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;


namespace Client.Interact
{
    public class CommandInteract
    {
        static string[] JsonInteract()
        {
            string filepath = "";
            string[] cominfo = new string[5];

            if (File.Exists(filepath))
            {
                FileStream fs = new(filepath, FileMode.Open, FileAccess.ReadWrite);
                StreamReader file = new(fs, System.Text.Encoding.Default);
                JsonTextReader reader = new(file);//解析json模块
                JObject o = (JObject)JToken.ReadFrom(reader);
                string ip = o["ip"].ToString();
                string port = o["port"].ToString();
                string teamid = o["teamid"].ToString();
                string playerid = o["playerid"].ToString();
                string shiptype = o["shiptype"].ToString();
                string index = o["index"].ToString();

                reader.Close();
                file.Close();
                fs.Close();
                cominfo[0] = ip;
                cominfo[1] = port;
                cominfo[2] = teamid;
                cominfo[3] = playerid;
                cominfo[4] = shiptype;

                //FileStream writetxt_fs = new FileStream(result1, FileMode.Create);
                //StreamWriter wr = new StreamWriter(writetxt_fs, System.Text.Encoding.Default);
                //wr.WriteLine(info);
                //wr.Close();

            }
            return cominfo;
        }


        public static string[] FileInteract()
        {
            string[] cominfo = new string[10];
            ConfigData d = new();
            if (d.Commands.Launched == false)
            {
                cominfo[0] = d.Commands.IP;
                cominfo[1] = d.Commands.Port;
                cominfo[2] = d.Commands.PlayerID;
                cominfo[3] = d.Commands.TeamID;
                cominfo[4] = d.Commands.ShipType;
            }
        //        d.Commands.Launched = true;
        //    }


        //    return cominfo;
        //}

    }
}
