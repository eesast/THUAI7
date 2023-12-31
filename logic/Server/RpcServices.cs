using GameClass.GameObj;
using Gaming;
using Grpc.Core;
using Preparation.Utility;
using Protobuf;

namespace Server
{
    partial class GameServer : ServerBase
    {
        private int playerCountNow = 0;
        protected object spectatorLock = new();
        protected bool isSpectatorJoin = false;
        protected bool IsSpectatorJoin
        {
            get
            {
                lock (spectatorLock)
                    return isSpectatorJoin;
            }

            set
            {
                lock (spectatorLock)
                    isSpectatorJoin = value;
            }
        }
        public override Task<BoolRes> TryConnection(IDMsg request, ServerCallContext context)
        {

#if DEBUG
            Console.WriteLine($"TryConnection ID: {request.PlayerId} from Team {request.TeamId}");
#endif 
            var onConnection = new BoolRes();
            lock (gameLock)
            {
                if (0 <= request.PlayerId && request.PlayerId < playerNum)
                {
                    onConnection.ActSuccess = true;
                    Console.WriteLine(onConnection.ActSuccess);
                    return Task.FromResult(onConnection);
                }
            }
            onConnection.ActSuccess = false;
            return Task.FromResult(onConnection);
        }

        protected readonly object addPlayerLock = new();
        public override async Task AddPlayer(PlayerMsg request, IServerStreamWriter<MessageToClient> responseStream, ServerCallContext context)
        {

            Console.WriteLine($"AddPlayer: {request.PlayerId} from Team{request.TeamId}");
            if (request.PlayerId >= spectatorMinPlayerID && options.NotAllowSpectator == false)
            {
                // 观战模式
                lock (spectatorJoinLock) // 具体原因见另一个上锁的地方
                {
                    if (semaDict.TryAdd(request.PlayerId, (new SemaphoreSlim(0, 1), new SemaphoreSlim(0, 1))))
                    {
                        Console.WriteLine("A new spectator comes to watch this game.");
                        IsSpectatorJoin = true;
                    }
                    else
                    {
                        Console.WriteLine($"Duplicated Spectator ID {request.PlayerId}");
                        return;
                    }
                }
                do
                {
                    semaDict[request.PlayerId].Item1.Wait();
                    try
                    {
                        if (currentGameInfo != null)
                        {
                            await responseStream.WriteAsync(currentGameInfo);
                            Console.WriteLine("Send!");
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        if (semaDict.TryRemove(request.PlayerId, out var semas))
                        {
                            try
                            {
                                semas.Item1.Release();
                                semas.Item2.Release();
                            }
                            catch { }
                            Console.WriteLine($"The spectator {request.PlayerId} exited");
                            return;
                        }
                    }
                    catch
                    {
                        // Console.WriteLine(ex);
                    }
                    finally
                    {
                        try
                        {
                            semaDict[request.PlayerId].Item2.Release();
                        }
                        catch { }
                    }
                } while (game.GameMap.Timer.IsGaming);
                return;
            }

            if (game.GameMap.Timer.IsGaming)
                return;
            if (!ValidPlayerID(request.PlayerId))  //玩家id是否正确
                return;
            if (request.TeamId >= TeamCount)  //队伍只能是0，1
                return;
            if (communicationToGameID[request.TeamId][request.PlayerId] != GameObj.invalidID)  //是否已经添加了该玩家
                return;


            lock (addPlayerLock)
            {
                XY birthPoint = new(request.X, request.Y);
                Game.ShipInitInfo playerInitInfo = new(request.TeamId, request.PlayerId, birthPoint, Transformation.ShipTypeFromProto(request.ShipType));
                long newPlayerID = game.AddShip(playerInitInfo);
                if (newPlayerID == GameObj.invalidID)
                    return;
                communicationToGameID[request.TeamId][request.PlayerId] = newPlayerID;
                var temp = (new SemaphoreSlim(0, 1), new SemaphoreSlim(0, 1));
                bool start = false;
                Console.WriteLine($"Id: {request.PlayerId} joins.");
                lock (spectatorJoinLock)  // 为了保证绝对安全，还是加上这个锁吧
                {
                    if (semaDict.TryAdd(request.PlayerId, temp))
                    {
                        start = Interlocked.Increment(ref playerCountNow) == playerNum;
                    }
                }
                if (start) StartGame();
            }

            bool exitFlag = false;
            do
            {
                semaDict[request.PlayerId].Item1.Wait();
                try
                {
                    if (currentGameInfo != null && !exitFlag)
                    {
                        await responseStream.WriteAsync(currentGameInfo);
                        Console.WriteLine("Send!");
                    }
                }
                catch
                {
                    if (!exitFlag)
                    {
                        Console.WriteLine($"The client {request.PlayerId} exited");
                        exitFlag = true;
                    }
                }
                finally
                {
                    semaDict[request.PlayerId].Item2.Release();
                }
            } while (game.GameMap.Timer.IsGaming);
        }

        public override Task<MessageOfMap> GetMap(NullRequest request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"GetMap IP: {context.Peer}");
#endif 
            return Task.FromResult(MapMsg());
        }

        public override Task<BoolRes> Attack(AttackMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"Attack ID: {request.PlayerId}");
#endif 
            BoolRes boolRes = new();
            if (request.PlayerId >= spectatorMinPlayerID)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            if (double.IsNaN(request.Angle))
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            var gameID = communicationToGameID[request.TeamId][request.PlayerId];
            boolRes.ActSuccess = game.Attack(gameID, request.Angle);
            return Task.FromResult(boolRes);
        }

        public override Task<MoveRes> Move(MoveMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"Move ID: {request.PlayerId}, TimeInMilliseconds: {request.TimeInMilliseconds}");
#endif            
            MoveRes moveRes = new();
            if (request.PlayerId >= spectatorMinPlayerID)
            {
                moveRes.ActSuccess = false;
                return Task.FromResult(moveRes);
            }
            if (double.IsNaN(request.Angle))
            {
                moveRes.ActSuccess = false;
                return Task.FromResult(moveRes);
            }
            var gameID = communicationToGameID[request.TeamId][request.PlayerId];
            moveRes.ActSuccess = game.MoveShip(gameID, (int)request.TimeInMilliseconds, request.Angle);
            if (!game.GameMap.Timer.IsGaming) moveRes.ActSuccess = false;
            return Task.FromResult(moveRes);
        }

        public override Task<BoolRes> Send(SendMsg request, ServerCallContext context)
        {
            var boolRes = new BoolRes();
            if (request.PlayerId >= spectatorMinPlayerID || PlayerDeceased((int)request.PlayerId))
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            if (!ValidPlayerID(request.PlayerId) || !ValidPlayerID(request.ToPlayerId) || request.PlayerId == request.ToPlayerId)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }

            switch (request.MessageCase)
            {
                case SendMsg.MessageOneofCase.TextMessage:
                    {
                        if (request.TextMessage.Length > 256)
                        {
#if DEBUG
                            Console.WriteLine("Text message string is too long!");
#endif
                            boolRes.ActSuccess = false;
                            return Task.FromResult(boolRes);
                        }
                        MessageOfNews news = new()
                        {
                            TextMessage = request.TextMessage,
                            FromId = request.PlayerId,
                            ToId = request.ToPlayerId
                        };
                        lock (newsLock)
                        {
                            currentNews.Add(news);
                        }
#if DEBUG
                        Console.WriteLine(news.TextMessage);
#endif
                        boolRes.ActSuccess = true;
                        return Task.FromResult(boolRes);
                    }
                case SendMsg.MessageOneofCase.BinaryMessage:
                    {

                        if (request.BinaryMessage.Length > 256)
                        {
#if DEBUG
                            Console.WriteLine("Binary message string is too long!");
#endif
                            boolRes.ActSuccess = false;
                            return Task.FromResult(boolRes);
                        }
                        MessageOfNews news = new()
                        {
                            BinaryMessage = request.BinaryMessage,
                            FromId = request.PlayerId,
                            ToId = request.ToPlayerId
                        };
                        lock (newsLock)
                        {
                            currentNews.Add(news);
                        }
#if DEBUG
                        Console.Write("BinaryMessageLength: ");
                        Console.WriteLine(news.BinaryMessage.Length);
#endif
                        boolRes.ActSuccess = true;
                        return Task.FromResult(boolRes);
                    }
                default:
                    {
                        boolRes.ActSuccess = false;
                        return Task.FromResult(boolRes);
                    }
            }


        }

        public override Task<BoolRes> Recover(RecoverMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"Recover ID: {request.PlayerId}");
#endif 
            BoolRes boolRes = new();
            if (request.PlayerId >= spectatorMinPlayerID)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            var gameID = communicationToGameID[request.TeamId][request.PlayerId];
            boolRes.ActSuccess = game.Recover(gameID, request.Recover);
            return Task.FromResult(boolRes);
        }

        public override Task<BoolRes> Produce(IDMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"Produce ID: {request.PlayerId}");
#endif 
            BoolRes boolRes = new();
            if (request.PlayerId >= spectatorMinPlayerID)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            var gameID = communicationToGameID[request.TeamId][request.PlayerId];
            boolRes.ActSuccess = game.Produce(gameID);
            return Task.FromResult(boolRes);
        }

        public override Task<BoolRes> Rebuild(ConstructMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"Rebuild ID: {request.PlayerId}");
#endif 
            BoolRes boolRes = new();
            if (request.PlayerId >= spectatorMinPlayerID)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            var gameID = communicationToGameID[request.TeamId][request.PlayerId];
            boolRes.ActSuccess = game.Construct(gameID, Transformation.ConstructionFromProto(request.ConstructionType));
            return Task.FromResult(boolRes);
        }

        public override Task<BoolRes> Recycle(IDMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"Recycle ID: {request.PlayerId}");
#endif 
            BoolRes boolRes = new();
            if (request.PlayerId >= spectatorMinPlayerID)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            var gameID = communicationToGameID[request.TeamId][request.PlayerId];
            boolRes.ActSuccess = game.Recycle(gameID);
            return Task.FromResult(boolRes);
        }

        public override Task<BoolRes> InstallModule(InstallMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"InstallModule ID: {request.PlayerId}");
#endif 
            BoolRes boolRes = new();
            if (request.PlayerId >= spectatorMinPlayerID)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            var gameID = communicationToGameID[request.TeamId][request.PlayerId];
            boolRes.ActSuccess = game.InstallModule(gameID, Transformation.ModuleFromProto(request.ModuleType));
            return Task.FromResult(boolRes);
        }

        public override Task<BoolRes> Construct(ConstructMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"Construct ID: {request.PlayerId}");
#endif 
            BoolRes boolRes = new();
            if (request.PlayerId >= spectatorMinPlayerID)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            var gameID = communicationToGameID[request.TeamId][request.PlayerId];
            boolRes.ActSuccess = game.Construct(gameID, Transformation.ConstructionFromProto(request.ConstructionType));
            return Task.FromResult(boolRes);
        }

        public override Task<BoolRes> EndAllAction(IDMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"EndAllAction ID: {request.PlayerId}");
#endif     
            BoolRes boolRes = new();
            if (request.PlayerId >= spectatorMinPlayerID)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            var gameID = communicationToGameID[request.TeamId][request.PlayerId];
            boolRes.ActSuccess = game.Stop(gameID);
            return Task.FromResult(boolRes);
        }



    }
}
