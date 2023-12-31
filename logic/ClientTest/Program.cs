using Grpc.Core;
using Protobuf;

namespace ClientTest
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            Thread.Sleep(3000);
            Channel channel = new("127.0.0.1:8888", ChannelCredentials.Insecure);
            var client = new AvailableService.AvailableServiceClient(channel);
            PlayerMsg playerInfo = new()
            {
                PlayerId = 0,
                TeamId = 0,
                ShipType = ShipType.CivilianShip,
                X = 30000,
                Y = 46000
            };
            var call = client.AddPlayer(playerInfo);
            MoveMsg moveMsg = new()
            {
                PlayerId = 0,
                TeamId = 0,
                TimeInMilliseconds = 100,
                Angle = 0
            };
            int tot = 0;
            /*while (call.ResponseStream.MoveNext().Result)
            {
                var currentGameInfo = call.ResponseStream.Current;
                if (currentGameInfo.GameState == GameState.GameStart) break;
            }*/
            while (true)
            {
                Thread.Sleep(50);
                MoveRes boolRes = client.Move(moveMsg);
                if (boolRes.ActSuccess == false) break;
                tot++;
                if (tot % 10 == 0) moveMsg.Angle += 1;

                Console.WriteLine("Move!");
            }
            return Task.CompletedTask;
        }
    }
}