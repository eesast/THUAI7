using GameClass.GameObj;
using Gaming;
using Grpc.Core;
using Preparation.Utility;
using Protobuf;

namespace Server
{
    partial class GameServer : ServerBase
    {
        private int shipCountNow = 0;
        protected object spectatorLock = new object();
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
            Console.WriteLine($"TryConnection ID: {request.ShipId}");
#endif 
            var onConnection = new BoolRes();
            lock (gameLock)
            {
                if (0 <= request.ShipId && request.ShipId < shipNum)  // 注意修改
                {
                    onConnection.ActSuccess = true;
                    Console.WriteLine(onConnection.ActSuccess);
                    return Task.FromResult(onConnection);
                }
            }
            onConnection.ActSuccess = false;
            return Task.FromResult(onConnection);
        }

        protected readonly object addShipLock = new();
        public override async Task AddShip(ShipMsg request, IServerStreamWriter<MessageToClient> responseStream, ServerCallContext context)
        {

            Console.WriteLine($"AddShip: {request.ShipId}");
            if (request.ShipId >= spectatorMinShipID && options.NotAllowSpectator == false)
            {
                // 观战模式
                lock (spetatorJoinLock) // 具体原因见另一个上锁的地方
                {
                    if (semaDict.TryAdd(request.ShipId, (new SemaphoreSlim(0, 1), new SemaphoreSlim(0, 1))))
                    {
                        Console.WriteLine("A new spectator comes to watch this game.");
                        IsSpectatorJoin = true;
                    }
                    else
                    {
                        Console.WriteLine($"Duplicated Spectator ID {request.ShipId}");
                        return;
                    }
                }
                do
                {
                    semaDict[request.ShipId].Item1.Wait();
                    try
                    {
                        if (currentGameInfo != null)
                        {
                            await responseStream.WriteAsync(currentGameInfo);
                            //Console.WriteLine("Send!");
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        if (semaDict.TryRemove(request.ShipId, out var semas))
                        {
                            try
                            {
                                semas.Item1.Release();
                                semas.Item2.Release();
                            }
                            catch { }
                            Console.WriteLine($"The spectator {request.ShipId} exited");
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
                            semaDict[request.ShipId].Item2.Release();
                        }
                        catch { }
                    }
                } while (game.GameMap.Timer.IsGaming);
                return;
            }

            if (game.GameMap.Timer.IsGaming)
                return;
            if (!ValidShipID(request.ShipId))  //玩家id是否正确
                return;
            if (communicationToGameID[request.ShipId] != GameObj.invalidID)  //是否已经添加了该玩家
                return;

            Preparation.Utility.CharacterType characterType = Preparation.Utility.CharacterType.Null;
            characterType = Transformation.ToShipType(request.ShipType);
            // if (request.PlayerType == PlayerType.StudentPlayer)
            //     characterType = Transformation.ToStudentType(request.StudentType);
            // else if (request.PlayerType == PlayerType.TrickerPlayer)
            //     characterType = Transformation.ToTrickerType(request.TrickerType);

            lock (addShipLock)
            {
                Game.ShipInitInfo shipInitInfo = new(GetBirthPointIdx(request.ShipId), ShipTypeToTeamID(request.PlayerTeam), request.ShipId, characterType);
                long newShipID = game.AddShip(shipInitInfo);
                if (newShipID == GameObj.invalidID)
                    return;
                communicationToGameID[request.ShipId] = newShipID;
                var temp = (new SemaphoreSlim(0, 1), new SemaphoreSlim(0, 1));
                bool start = false;
                Console.WriteLine($"Id: {request.ShipId} joins.");
                lock (spetatorJoinLock)  // 为了保证绝对安全，还是加上这个锁吧
                {
                    if (semaDict.TryAdd(request.ShipId, temp))
                    {
                        start = Interlocked.Increment(ref shipCountNow) == shipNum;
                    }
                }
                if (start) StartGame();
            }

            bool exitFlag = false;
            do
            {
                semaDict[request.ShipId].Item1.Wait();
                try
                {
                    if (currentGameInfo != null && !exitFlag)
                    {
                        await responseStream.WriteAsync(currentGameInfo);
                        //Console.WriteLine("Send!");
                    }
                }
                catch
                {
                    if (!exitFlag)
                    {
                        Console.WriteLine($"The client {request.ShipId} exited");
                        exitFlag = true;
                    }
                }
                finally
                {
                    semaDict[request.ShipId].Item2.Release();
                }
            } while (game.GameMap.Timer.IsGaming);
        }


        public override Task<BoolRes> Attack(AttackMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"Attack ID: {request.ShipId}");
#endif 
            BoolRes boolRes = new();
            if (request.ShipId >= spectatorMinShipID)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            if (request.Angle == double.NaN)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            var gameID = communicationToGameID[request.ShipId];
            boolRes.ActSuccess = game.Attack(gameID, request.Angle);
            return Task.FromResult(boolRes);
        }


        public override Task<MoveRes> Move(MoveMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"Move ID: {request.ShipId}, TimeInMilliseconds: {request.TimeInMilliseconds}");
#endif            
            MoveRes moveRes = new();
            if (request.ShipId >= spectatorMinShipID)
            {
                moveRes.ActSuccess = false;
                return Task.FromResult(moveRes);
            }
            if (request.Angle == double.NaN)
            {
                moveRes.ActSuccess = false;
                return Task.FromResult(moveRes);
            }
            var gameID = communicationToGameID[request.ShipId];
            moveRes.ActSuccess = game.MoveShip(gameID, (int)request.TimeInMilliseconds, request.Angle);
            if (!game.GameMap.Timer.IsGaming) moveRes.ActSuccess = false;
            return Task.FromResult(moveRes);
        }

        public override Task<BoolRes> SendMessage(SendMsg request, ServerCallContext context)
        {
            var boolRes = new BoolRes();
            if (request.ShipId >= spectatorMinShipID || shipDeceased((int)request.ShipId))
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            if (!ValidShipID(request.ShipId) || !ValidShipID(request.ToShipId)
                || ShipIDToTeamID(request.ShipId) != ShipIDToTeamID(request.ToShipId) || request.ShipId == request.ToShipId)
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
                        MessageOfNews news = new();
                        news.TextMessage = request.TextMessage;
                        news.FromId = request.ShipId;
                        news.ToId = request.ToShipId;
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
                        MessageOfNews news = new();
                        news.BinaryMessage = request.BinaryMessage;
                        news.FromId = request.ShipId;
                        news.ToId = request.ToShipId;
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

        public override Task<BoolRes> StartRecovering(IDMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"StartRecovering ID: {request.ShipId}");
#endif
            BoolRes boolRes = new();
            if (request.ShipId >= spectatorMinShipID)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            var gameID = communicationToGameID[request.ShipId];
            boolRes.ActSuccess = game.Recover(gameID);
            return Task.FromResult(boolRes);
        }
        public override Task<BoolRes> StartProducing(IDMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"StartProducing ID: {request.ShipId}");
#endif
            BoolRes boolRes = new();
            if (request.ShipId >= spectatorMinShipID)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            var gameID = communicationToGameID[request.ShipId];
            boolRes.ActSuccess = game.Produce(gameID);
            return Task.FromResult(boolRes);
        }
        public override Task<BoolRes> StartRecycling(IDMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"StartRecycling ID: {request.ShipId}");
#endif
            BoolRes boolRes = new();
            if (request.ShipId >= spectatorMinShipID)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            var gameID = communicationToGameID[request.ShipId];
            boolRes.ActSuccess = game.Recycle(gameID);
            return Task.FromResult(boolRes);
        }
        public override Task<BoolRes> StartBuilding(BuildMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"StartBuilding ID: {request.ShipId}");
#endif
            BoolRes boolRes = new();
            if (request.ShipId >= spectatorMinShipID)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            var gameID = communicationToGameID[request.ShipId];
            boolRes.ActSuccess = game.BuildBuilding(gameID);
            return Task.FromResult(boolRes);

        }
        public override Task<BoolRes> BuildShip(BuildMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"StartBuilding ID: {request.BuildingId}");
#endif
            BoolRes boolRes = new();
            if (request.ShipId >= spectatorMinShipID)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            var gameID = communicationToGameID[request.ShipId];
            boolRes.ActSuccess = game.BuildShip(gameID);
            return Task.FromResult(boolRes);
        }
        public override Task<BoolRes> InstallCollectorModule(CollectorMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"StartInstalling ID: {request.ShipId}");
#endif
            BoolRes boolRes = new();
            if (request.ShipId >= spectatorMinShipID)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            var gameID = communicationToGameID[request.ShipId];
            boolRes.ActSuccess = game.InstallCollectorModule(gameID, request.CollectorID, request.CollectorType);
            return Task.FromResult(boolRes);
        }
        public override Task<BoolRes> InstallArmorModule(ArmorMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"StartInstalling ID: {request.ShipId}");
#endif
            BoolRes boolRes = new();
            if (request.ShipId >= spectatorMinShipID)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            var gameID = communicationToGameID[request.ShipId];
            boolRes.ActSuccess = game.InstallArmorModule(gameID, request.ArmorID, request.ArmorType);
            return Task.FromResult(boolRes);
        }
        public override Task<BoolRes> InstallShieldModule(ShieldMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"StartInstalling ID: {request.ShipId}");
#endif
            BoolRes boolRes = new();
            if (request.ShipId >= spectatorMinShipID)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            var gameID = communicationToGameID[request.ShipId];
            boolRes.ActSuccess = game.InstallShieldModule(gameID, request.ShieldID, request.ShieldType);
            return Task.FromResult(boolRes);
        }
        public override Task<BoolRes> InstallBuilderModule(BuilderMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"StartInstalling ID: {request.ShipId}");
#endif
            BoolRes boolRes = new();
            if (request.ShipId >= spectatorMinShipID)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            var gameID = communicationToGameID[request.ShipId];
            boolRes.ActSuccess = game.InstallBuilderModule(gameID, request.BuilderID, request.BuilderType);
            return Task.FromResult(boolRes);
        }

        public override Task<BoolRes> EndAllAction(IDMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"EndAllAction ID: {request.ShipId}");
#endif     
            BoolRes boolRes = new();
            if (request.ShipId >= spectatorMinShipID)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            var gameID = communicationToGameID[request.ShipId];
            boolRes.ActSuccess = game.Stop(gameID);
            return Task.FromResult(boolRes);
        }
    }

}
