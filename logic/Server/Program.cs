//using CommandLine;
//using Grpc.Core;
//using Protobuf;

namespace Server
{
    public class Program
    {
        //        const string welcome =
        //@"
        //     _____ _   _ _   _   _    ___ _____ 
        //    |_   _| | | | | | | / \  |_ _|___  |
        //      | | | |_| | | | |/ _ \  | |   / / 
        //      | | |  _  | |_| / ___ \ | |  / /  
        //      |_| |_| |_|\___/_/   \_\___|/_/   
        //";
        //        static ServerBase CreateServer(ArgumentOptions options)
        //        {
        //            //return options.Playback ? new PlaybackServer(options) : new GameServer(options);
        //            return new PlaybackServer(options);
        //        }

        static int Main(string[] args)
        {
            //            foreach (var arg in args)
            //            {
            //                Console.Write($"{arg} ");
            //            }
            //            Console.WriteLine();

            //            ArgumentOptions? options = null;
            //            _ = Parser.Default.ParseArguments<ArgumentOptions>(args).WithParsed(o => { options = o; });
            //            if (options == null)
            //            {
            //                Console.WriteLine("Argument parsing failed!");
            //                return 1;
            //            }

            //            if (options.StartLockFile == "114514")
            //            {
            //                Console.WriteLine(welcome);
            //            }
            //            Console.WriteLine("Server begins to run: " + options.ServerPort.ToString());

            //            try
            //            {
            //                var server = CreateServer(options);
            //                Grpc.Core.Server rpcServer = new Grpc.Core.Server(new[] { new ChannelOption(ChannelOptions.SoReuseport, 0) })
            //                {
            //                    Services = { AvailableService.BindService(server) },
            //                    Ports = { new ServerPort(options.ServerIP, options.ServerPort, ServerCredentials.Insecure) }
            //                };
            //                rpcServer.Start();

            //                Console.WriteLine("Server begins to listen!");
            //                server.WaitForEnd();
            //                Console.WriteLine("Server end!");
            //                rpcServer.ShutdownAsync().Wait();

            //                Thread.Sleep(50);
            //                Console.WriteLine("");
            //                Console.WriteLine("===================  Final Score  ====================");
            //                Console.WriteLine($"Team0: {server.GetScore()[0]}");
            //                Console.WriteLine($"Team1: {server.GetScore()[1]}");
            //            }
            //            catch (Exception ex)
            //            {
            //                Console.WriteLine(ex.ToString());
            //                Console.WriteLine(ex.StackTrace);
            //            }
            return 0;
        }
    }
}
