using GameClass.GameObj;
using Gaming;
using Grpc.Core;
using Preparation.Utility;
using Utility = Preparation.Utility;
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
            Console.WriteLine($"TRY TryConnection: Player {request.PlayerId} from Team {request.TeamId}");
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
#if DEBUG
            Console.WriteLine("END TryConnection");
#endif 
            return Task.FromResult(onConnection);
        }

        #region 游戏开局调用一次的服务

        protected readonly object addPlayerLock = new();
        public override async Task AddPlayer(PlayerMsg request, IServerStreamWriter<MessageToClient> responseStream, ServerCallContext context)
        {
#if !DEBUG
            Console.WriteLine($"AddPlayer: Player {request.PlayerId} from Team {request.TeamId}");
#endif
            if (request.PlayerId >= spectatorMinPlayerID && options.NotAllowSpectator == false)
            {
#if DEBUG
                Console.WriteLine($"TRY Add Spectator: Player {request.PlayerId}");
#endif
                // 观战模式
                lock (spectatorJoinLock) // 具体原因见另一个上锁的地方
                {
                    if (semaDict0.TryAdd(request.PlayerId, (new SemaphoreSlim(0, 1), new SemaphoreSlim(0, 1))))
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
                    semaDict0[request.PlayerId].Item1.Wait();
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
                        if (semaDict0.TryRemove(request.PlayerId, out var semas))
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
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    finally
                    {
                        try
                        {
                            semaDict0[request.PlayerId].Item2.Release();
                        }
                        catch { }
                    }
                } while (game.GameMap.Timer.IsGaming);
#if DEBUG
                Console.WriteLine("END Add Spectator");
#endif
                return;
            }
#if DEBUG
            Console.WriteLine($"TRY Add Player: Player {request.PlayerId} from Team {request.TeamId}");
#endif
            if (game.GameMap.Timer.IsGaming)
                return;
            if (!ValidPlayerID(request.PlayerId))  //玩家id是否正确
                return;
            if (request.TeamId >= TeamCount)  //队伍只能是0，1
                return;
            if (communicationToGameID[request.TeamId][request.PlayerId] != GameObj.invalidID)  //是否已经添加了该玩家
                return;
#if DEBUG
            Console.WriteLine("Check Correct");
#endif
            lock (addPlayerLock)
            {
                Game.PlayerInitInfo playerInitInfo = new(request.TeamId,
                                                         request.PlayerId,
                                                         Transformation.ShipTypeFromProto(request.ShipType));
                long newPlayerID = game.AddPlayer(playerInitInfo);
                if (newPlayerID == GameObj.invalidID)
                {
#if DEBUG
                    Console.WriteLine("FAIL AddPlayer");
#endif
                    return;
                }
                communicationToGameID[request.TeamId][request.PlayerId] = newPlayerID;
                var temp = (new SemaphoreSlim(0, 1), new SemaphoreSlim(0, 1));
                bool start = false;
                Console.WriteLine($"Player {request.PlayerId} from Team {request.TeamId} joins.");
                lock (spectatorJoinLock)  // 为了保证绝对安全，还是加上这个锁吧
                {
                    if (request.TeamId == 0)
                    {
                        if (semaDict0.TryAdd(request.PlayerId, temp))
                        {
                            start = Interlocked.Increment(ref playerCountNow) == (playerNum * TeamCount);
                            Console.WriteLine($"PlayerCountNow: {playerCountNow}");
                            Console.WriteLine($"PlayerTotalNum: {playerNum * TeamCount}");
                        }
                    }
                    else if (request.TeamId == 1)
                    {
                        if (semaDict1.TryAdd(request.PlayerId, temp))
                        {
                            start = Interlocked.Increment(ref playerCountNow) == (playerNum * TeamCount);
                            Console.WriteLine($"PlayerCountNow: {playerCountNow}");
                            Console.WriteLine($"PlayerNum: {playerNum * TeamCount}");
                        }
                    }
                }
                if (start)
                {
#if DEBUG
                    Console.WriteLine("Game Start");
#endif
                    StartGame();
                }
            }
            bool exitFlag = false;
            do
            {
                Ship? ship = game.GameMap.FindShipInPlayerID(request.TeamId, request.PlayerId);
                (request.TeamId == 0 ? semaDict0 : semaDict1)[request.PlayerId].Item1.Wait();
                if (request.PlayerId > 0 && (ship == null || ship.IsRemoved == true))
                {
                    // Console.WriteLine($"Cannot find ship {request.PlayerId} from Team {request.TeamId}!");
                }
                else
                {
                    try
                    {
                        if (currentGameInfo != null && !exitFlag)
                        {
                            await responseStream.WriteAsync(currentGameInfo);
                            Console.WriteLine($"Send to Player{request.PlayerId} from Team {request.TeamId}!");
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
                }
                (request.TeamId == 0 ? semaDict0 : semaDict1)[request.PlayerId].Item2.Release();
            } while (game.GameMap.Timer.IsGaming);
        }

        public override Task<MessageOfMap> GetMap(NullRequest request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"GetMap: IP {context.Peer}");
#endif
            return Task.FromResult(MapMsg());
        }

        #endregion

        #region 游戏过程中玩家执行操作的服务

        #region 船

        /*public override Task<BoolRes> Activate(ActivateMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"TRY Activate: Player {request.PlayerId} from Team {request.TeamId}");
#endif
            BoolRes boolRes = new();
            if (request.PlayerId >= spectatorMinPlayerID)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            // var gameID = communicationToGameID[request.TeamId][request.PlayerId];
            boolRes.ActSuccess = game.ActivateShip(request.TeamId, Transformation.ShipTypeFromProto(request.ShipType));
            if (!game.GameMap.Timer.IsGaming) boolRes.ActSuccess = false;
#if DEBUG
            Console.WriteLine($"END Activate: {boolRes.ActSuccess}");
#endif
            return Task.FromResult(boolRes);
        }*/

        public override Task<MoveRes> Move(MoveMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"TRY Move: Player {request.PlayerId} from Team {request.TeamId}, TimeInMilliseconds: {request.TimeInMilliseconds}");
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
            // var gameID = communicationToGameID[request.TeamId][request.PlayerId];
            moveRes.ActSuccess = game.MoveShip(request.TeamId, request.PlayerId, (int)request.TimeInMilliseconds, request.Angle);
            if (!game.GameMap.Timer.IsGaming) moveRes.ActSuccess = false;
#if DEBUG
            Console.WriteLine($"END Move: {moveRes.ActSuccess}");
#endif
            return Task.FromResult(moveRes);
        }

        public override Task<BoolRes> Recover(RecoverMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"TRY Recover: Player {request.PlayerId} from Team {request.TeamId}");
#endif
            BoolRes boolRes = new();
            if (request.PlayerId >= spectatorMinPlayerID)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            // var gameID = communicationToGameID[request.TeamId][request.PlayerId];
            boolRes.ActSuccess = game.Recover(request.TeamId, request.PlayerId, request.Recover);
#if DEBUG
            Console.WriteLine("END Recover");
#endif
            return Task.FromResult(boolRes);
        }

        public override Task<BoolRes> Produce(IDMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"TRY Produce: Player {request.PlayerId} from Team {request.TeamId}");
#endif
            BoolRes boolRes = new();
            if (request.PlayerId >= spectatorMinPlayerID)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            // var gameID = communicationToGameID[request.TeamId][request.PlayerId];
            boolRes.ActSuccess = game.Produce(request.TeamId, request.PlayerId);
#if DEBUG
            Console.WriteLine("END Produce");
#endif
            return Task.FromResult(boolRes);
        }

        public override Task<BoolRes> Rebuild(ConstructMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"TRY Rebuild: Player {request.PlayerId} from Team {request.TeamId}");
#endif
            BoolRes boolRes = new();
            if (request.PlayerId >= spectatorMinPlayerID)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            // var gameID = communicationToGameID[request.TeamId][request.PlayerId];
            boolRes.ActSuccess = game.Construct(request.TeamId, request.PlayerId, Transformation.ConstructionFromProto(request.ConstructionType));
#if DEBUG
            Console.WriteLine("END Rebuild");
#endif
            return Task.FromResult(boolRes);
        }

        public override Task<BoolRes> Construct(ConstructMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"TRY Construct: Player {request.PlayerId} from Team {request.TeamId}");
#endif
            BoolRes boolRes = new();
            if (request.PlayerId >= spectatorMinPlayerID)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            // var gameID = communicationToGameID[request.TeamId][request.PlayerId];
            boolRes.ActSuccess = game.Construct(request.TeamId, request.PlayerId, Transformation.ConstructionFromProto(request.ConstructionType));
#if DEBUG
            Console.WriteLine("END Construct");
#endif
            return Task.FromResult(boolRes);
        }

        public override Task<BoolRes> Attack(AttackMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"TRY Attack: Player {request.PlayerId} from Team {request.TeamId}");
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
            // var gameID = communicationToGameID[request.TeamId][request.PlayerId];
            boolRes.ActSuccess = game.Attack(request.TeamId, request.PlayerId, request.Angle);
#if DEBUG
            Console.WriteLine("END Attack");
#endif
            return Task.FromResult(boolRes);
        }

        public override Task<BoolRes> Send(SendMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"TRY Send: From Player {request.PlayerId} To Player {request.ToPlayerId} from Team {request.TeamId}");
#endif
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
#if DEBUG
            Console.WriteLine($"As {request.MessageCase}");
#endif
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
#if DEBUG
                        Console.WriteLine($"END Send");
#endif
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
#if DEBUG
                        Console.WriteLine($"END Send");
#endif
                        return Task.FromResult(boolRes);
                    }
                default:
                    {
                        boolRes.ActSuccess = false;
                        return Task.FromResult(boolRes);
                    }
            }
        }

        #endregion

        #region 大本营

        public override Task<BoolRes> InstallModule(InstallMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"TRY InstallModule: Player {request.PlayerId} from Team {request.TeamId}");
#endif
            BoolRes boolRes = new();
            if (request.PlayerId >= spectatorMinPlayerID)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            // var gameID = communicationToGameID[request.TeamId][request.PlayerId];
            boolRes.ActSuccess = game.InstallModule(request.TeamId, request.PlayerId, Transformation.ModuleFromProto(request.ModuleType));
#if DEBUG
            Console.WriteLine("END InstallModule");
#endif
            return Task.FromResult(boolRes);
        }

        public override Task<BoolRes> Recycle(IDMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"TRY Recycle: Player {request.PlayerId} from Team {request.TeamId}");
#endif
            BoolRes boolRes = new();
            if (request.PlayerId >= spectatorMinPlayerID)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            // var gameID = communicationToGameID[request.TeamId][request.PlayerId];
            boolRes.ActSuccess = game.Recycle(request.TeamId, request.PlayerId);
#if DEBUG
            Console.WriteLine("END Recycle");
#endif
            return Task.FromResult(boolRes);
        }

        public override Task<BoolRes> BuildShip(BuildShipMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"TRY BuildShip: ShipType {request.ShipType} from Team {request.TeamId}");
#endif
            var activateCost = Transformation.ShipTypeFromProto(request.ShipType) switch
            {
                Utility.ShipType.CivilShip => GameData.CivilShipCost,
                Utility.ShipType.WarShip => GameData.WarShipCost,
                Utility.ShipType.FlagShip => GameData.FlagShipCost,
                _ => int.MaxValue
            };
            var teamMoneyPool = game.TeamList[(int)request.TeamId].MoneyPool;
            if (activateCost > teamMoneyPool.Money)
            {
                return Task.FromResult(new BoolRes { ActSuccess = false });
            }
            BoolRes boolRes = new()
            {
                ActSuccess =
                    game.ActivateShip(request.TeamId,
                                      Transformation.ShipTypeFromProto(request.ShipType),
                                      request.BirthpointIndex)
                    != GameObj.invalidID
            };
            if (boolRes.ActSuccess) teamMoneyPool.SubMoney(activateCost);
#if DEBUG
            Console.WriteLine("END BuildShip");
#endif
            return Task.FromResult(boolRes);
        }

        public override Task<BuildShipRes> BuildShipRID(BuildShipMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"TRY BuildShipRID: ShipType {request.ShipType} from Team {request.TeamId}");
#endif
            var activateCost = Transformation.ShipTypeFromProto(request.ShipType) switch
            {
                Utility.ShipType.CivilShip => GameData.CivilShipCost,
                Utility.ShipType.WarShip => GameData.WarShipCost,
                Utility.ShipType.FlagShip => GameData.FlagShipCost,
                _ => int.MaxValue
            };
            var teamMoneyPool = game.TeamList[(int)request.TeamId].MoneyPool;
            if (activateCost > teamMoneyPool.Money)
            {
                return Task.FromResult(new BuildShipRes { ActSuccess = false });
            }
            var playerId = game.ActivateShip(request.TeamId,
                                      Transformation.ShipTypeFromProto(request.ShipType),
                                      request.BirthpointIndex);
            BuildShipRes buildShipRes = new()
            {
                ActSuccess = playerId != GameObj.invalidID,
                PlayerId = playerId
            };
            if (buildShipRes.ActSuccess) teamMoneyPool.SubMoney(activateCost);
#if DEBUG
            Console.WriteLine("END BuildShipRID");
#endif
            return Task.FromResult(buildShipRes);
        }

        public override Task<BoolRes> EndAllAction(IDMsg request, ServerCallContext context)
        {
#if DEBUG
            Console.WriteLine($"TRY EndAllAction: Player {request.PlayerId} from Team {request.TeamId}");
#endif
            BoolRes boolRes = new();
            if (request.PlayerId >= spectatorMinPlayerID)
            {
                boolRes.ActSuccess = false;
                return Task.FromResult(boolRes);
            }
            // var gameID = communicationToGameID[request.TeamId][request.PlayerId];
            boolRes.ActSuccess = game.Stop(request.TeamId, request.PlayerId);
#if DEBUG
            Console.WriteLine("END EndAllAction");
#endif
            return Task.FromResult(boolRes);
        }

        #endregion

        #endregion
    }
}
