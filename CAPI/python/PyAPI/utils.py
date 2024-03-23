import proto.MessageType_pb2 as MessageType
import proto.Message2Server_pb2 as Message2Server
import proto.Message2Clients_pb2 as Message2Clients
import PyAPI.structures as THUAI7
from typing import Final, List

numOfGridPerCell: Final[int] = 1000


class AssistFunction:
    @staticmethod
    def CellToGrid(cell: int) -> int:
        return cell * numOfGridPerCell + numOfGridPerCell // 2

    @staticmethod
    def GridToCell(grid: int) -> int:
        return grid // numOfGridPerCell

    @staticmethod
    def HaveView(
        viewRange: int,
        x: int,
        y: int,
        newX: int,
        newY: int,
        map: List[List[THUAI7.PlaceType]],
    ) -> bool:
        deltaX = newX - x
        deltaY = newY - y
        distance = deltaX**2 + deltaY**2
        myPlace = map[AssistFunction.GridToCell(x)][AssistFunction.GridToCell(y)]
        newPlace = map[AssistFunction.GridToCell(newX)][AssistFunction.GridToCell(newY)]
        if myPlace != THUAI7.PlaceType.Grass and newPlace == THUAI7.PlaceType.Grass:
            return False
        if distance <= viewRange * viewRange:
            divide = max(abs(deltaX), abs(deltaY)) // 100
            if divide == 0:
                return True
            dx = deltaX / divide
            dy = deltaY / divide
            selfX = float(x)
            selfY = float(y)
            if (newPlace == THUAI7.PlaceType.Grass and myPlace == THUAI7.PlaceType.Grass):
                for _ in range(divide):
                    selfX += dx
                    selfY += dy
                    if (map[AssistFunction.GridToCell(int(selfX))][AssistFunction.GridToCell(int(selfY))]
                            != THUAI7.PlaceType.Grass):
                        return False
                else:
                    return True
            else:
                for _ in range(divide):
                    selfX += dx
                    selfY += dy
                    if (map[AssistFunction.GridToCell(int(selfX))][AssistFunction.GridToCell(int(selfY))]
                            == THUAI7.PlaceType.Wall):
                        return False
                else:
                    return True
        else:
            return False


class Proto2THUAI7:
    gameStateDict: Final[dict] = {
        MessageType.NULL_GAME_STATE: THUAI7.GameState.NullGameState,
        MessageType.GAME_START: THUAI7.GameState.GameStart,
        MessageType.GAME_RUNNING: THUAI7.GameState.GameRunning,
        MessageType.GAME_END: THUAI7.GameState.GameEnd,
    }

    placeTypeDict: Final[dict] = {
        MessageType.NULL_PLACE_TYPE: THUAI7.PlaceType.NullPlaceType,
        MessageType.HOME: THUAI7.PlaceType.Home,
        MessageType.GROUND: THUAI7.PlaceType.Ground,
        MessageType.WALL: THUAI7.PlaceType.Wall,
        MessageType.GRASS: THUAI7.PlaceType.Grass,
        MessageType.RIVER: THUAI7.PlaceType.River,
        MessageType.GARBAGE: THUAI7.PlaceType.Garbage,
        MessageType.CONSTRUCTION: THUAI7.PlaceType.Construction,
        MessageType.BRIDGE: THUAI7.PlaceType.Bridge,
    }

    shapeTypeDict: Final[dict] = {
        MessageType.NULL_SHAPE_TYPE: THUAI7.ShapeType.NullShapeType,
        MessageType.CIRCLE: THUAI7.ShapeType.Circle,
        MessageType.SQUARE: THUAI7.ShapeType.Square,
    }

    playerTypeDict: Final[dict] = {
        MessageType.NULL_PLAYER_TYPE: THUAI7.PlayerType.NullPlayerType,
        MessageType.SWEEPER: THUAI7.PlayerType.Sweeper,
        MessageType.TEAM: THUAI7.PlayerType.Team,
    }

    sweeperTypeDict: Final[dict] = {
        MessageType.NULL_SWEEPER_TYPE: THUAI7.SweeperType.NullSweeperType,
        MessageType.CIVILIAN_SWEEPER: THUAI7.SweeperType.CivilianSweeper,
        MessageType.MILITARY_SWEEPER: THUAI7.SweeperType.MilitarySweeper,
        MessageType.FLAG_SWEEPER: THUAI7.SweeperType.FlagSweeper,
    }

    sweeperStateDict: Final[dict] = {
        MessageType.NULL_STATUS: THUAI7.SweeperState.NullStatus,
        MessageType.IDLE: THUAI7.SweeperState.Idle,
        MessageType.PRODUCING: THUAI7.SweeperState.Producing,
        MessageType.CONSTRUCTING: THUAI7.SweeperState.Constructing,
        MessageType.RECOVERING: THUAI7.SweeperState.Recovering,
        MessageType.RECYCLING: THUAI7.SweeperState.Recycling,
        MessageType.ATTACKING: THUAI7.SweeperState.Attacking,
        MessageType.SWINGING: THUAI7.SweeperState.Swinging,
        MessageType.STUNNED: THUAI7.SweeperState.Stunned,
        MessageType.MOVING: THUAI7.SweeperState.Moving,
    }

    weaponTypeDict: Final[dict] = {
        MessageType.NULL_WEAPON_TYPE: THUAI7.WeaponType.NullWeaponType,
        MessageType.LASERGUN: THUAI7.WeaponType.LaserGun,
        MessageType.PLASMAGUN: THUAI7.WeaponType.PlasmaGun,
        MessageType.SHELLGUN: THUAI7.WeaponType.ShellGun,
        MessageType.MISSILEGUN: THUAI7.WeaponType.MissileGun,
        MessageType.ARCGUN: THUAI7.WeaponType.ArcGun,
    }

    constructorTypeDict: Final[dict] = {
        MessageType.NULL_CONSTRUCTOR_TYPE: THUAI7.ConstructorType.NullConstructorType,
        MessageType.CONSTRUCTOR1: THUAI7.ConstructorType.Constructor1,
        MessageType.CONSTRUCTOR2: THUAI7.ConstructorType.Constructor2,
        MessageType.CONSTRUCTOR3: THUAI7.ConstructorType.Constructor3,
    }

    armorTypeDict: Final[dict] = {
        MessageType.NULL_ARMOR_TYPE: THUAI7.ArmorType.NullArmorType,
        MessageType.ARMOR1: THUAI7.ArmorType.Armor1,
        MessageType.ARMOR2: THUAI7.ArmorType.Armor2,
        MessageType.ARMOR3: THUAI7.ArmorType.Armor3,
    }

    shieldTypeDict: Final[dict] = {
        MessageType.NULL_SHIELD_TYPE: THUAI7.ShieldType.NullShieldType,
        MessageType.SHIELD1: THUAI7.ShieldType.Shield1,
        MessageType.SHIELD2: THUAI7.ShieldType.Shield2,
        MessageType.SHIELD3: THUAI7.ShieldType.Shield3,
    }

    producerTypeDict: Final[dict] = {
        MessageType.NULL_PRODUCER_TYPE: THUAI7.ProducerType.NullProducerType,
        MessageType.PRODUCER1: THUAI7.ProducerType.Producer1,
        MessageType.PRODUCER2: THUAI7.ProducerType.Producer2,
        MessageType.PRODUCER3: THUAI7.ProducerType.Producer3,
    }

    moduleTypeDict: Final[dict] = {
        MessageType.NULL_MODULE_TYPE: THUAI7.ModuleType.NullModuleType,
        MessageType.MODULE_PRODUCER1: THUAI7.ModuleType.ModuleProducer1,
        MessageType.MODULE_PRODUCER2: THUAI7.ModuleType.ModuleProducer2,
        MessageType.MODULE_PRODUCER3: THUAI7.ModuleType.ModuleProducer3,
        MessageType.MODULE_CONSTRUCTOR1: THUAI7.ModuleType.ModuleConstructor1,
        MessageType.MODULE_CONSTRUCTOR2: THUAI7.ModuleType.ModuleConstructor2,
        MessageType.MODULE_CONSTRUCTOR3: THUAI7.ModuleType.ModuleConstructor3,
        MessageType.MODULE_ARMOR1: THUAI7.ModuleType.ModuleArmor1,
        MessageType.MODULE_ARMOR2: THUAI7.ModuleType.ModuleArmor2,
        MessageType.MODULE_ARMOR3: THUAI7.ModuleType.ModuleArmor3,
        MessageType.MODULE_SHIELD1: THUAI7.ModuleType.ModuleShield1,
        MessageType.MODULE_SHIELD2: THUAI7.ModuleType.ModuleShield2,
        MessageType.MODULE_SHIELD3: THUAI7.ModuleType.ModuleShield3,
        MessageType.MODULE_LASERGUN: THUAI7.ModuleType.ModuleLaserGun,
        MessageType.MODULE_PLASMAGUN: THUAI7.ModuleType.ModulePlasmaGun,
        MessageType.MODULE_SHELLGUN: THUAI7.ModuleType.ModuleShellGun,
        MessageType.MODULE_MISSILEGUN: THUAI7.ModuleType.ModuleMissileGun,
        MessageType.MODULE_ARCGUN: THUAI7.ModuleType.ModuleArcGun,
    }

    bulletTypeDict: Final[dict] = {
        MessageType.NULL_BULLET_TYPE: THUAI7.BulletType.NullBulletType,
        MessageType.LASER: THUAI7.BulletType.Laser,
        MessageType.PLASMA: THUAI7.BulletType.Plasma,
        MessageType.SHELL: THUAI7.BulletType.Shell,
        MessageType.MISSILE: THUAI7.BulletType.Missile,
        MessageType.ARC: THUAI7.BulletType.Arc,
    }

    constructionTypeDict: Final[dict] = {
        MessageType.NULL_CONSTRUCTION_TYPE: THUAI7.ConstructionType.NullConstructionType,
        MessageType.RECYCLEBANK: THUAI7.ConstructionType.RecycleBank,
        MessageType.CHARGESTATION: THUAI7.ConstructionType.ChargeStation,
        MessageType.SIGNALTOWER: THUAI7.ConstructionType.SignalTower,
    }

    playerTeamDict: Final[dict] = {
        MessageType.NULL_TEAM: THUAI7.PlayerTeam.NullTeam,
        MessageType.RED: THUAI7.PlayerTeam.Red,
        MessageType.BLUE: THUAI7.PlayerTeam.Blue,
    }

    newsTypeDict: Final[dict] = {
        MessageType.NULL_NEWS_TYPE: THUAI7.NewsType.NullNewsType,
        MessageType.TEXT: THUAI7.NewsType.TextMessage,
        MessageType.BINARY: THUAI7.NewsType.BinaryMessage,
    }

    @staticmethod
    def Protobuf2THUAI7Sweeper(sweeperMsg: Message2Clients.MessageOfSweeper) -> THUAI7.Sweeper:
        sweeper = THUAI7.Sweeper()
        sweeper.x = sweeperMsg.x
        sweeper.y = sweeperMsg.y
        sweeper.speed = sweeperMsg.speed
        sweeper.hp = sweeperMsg.hp
        sweeper.armor = sweeperMsg.armor
        sweeper.shield = sweeperMsg.shield
        sweeper.teamID = sweeperMsg.team_id
        sweeper.playerID = sweeperMsg.player_id
        sweeper.guid = sweeperMsg.guid
        sweeper.sweeperState = Proto2THUAI7.sweeperStateDict[sweeperMsg.sweeper_state]
        sweeper.sweeperType = Proto2THUAI7.sweeperTypeDict[sweeperMsg.sweeper_type]
        sweeper.viewRange = sweeperMsg.view_range
        sweeper.producerType = Proto2THUAI7.producerTypeDict[sweeperMsg.producer_type]
        sweeper.constructorType = Proto2THUAI7.constructorTypeDict[
            sweeperMsg.constructor_type
        ]
        sweeper.armorType = Proto2THUAI7.armorTypeDict[sweeperMsg.armor_type]
        sweeper.shieldType = Proto2THUAI7.shieldTypeDict[sweeperMsg.shield_type]
        sweeper.weaponType = Proto2THUAI7.weaponTypeDict[sweeperMsg.weapon_type]
        sweeper.facingDirection = sweeperMsg.facing_direction
        return sweeper

    @staticmethod
    def Protobuf2THUAI7Bullet(
        bulletMsg: Message2Clients.MessageOfBullet,
    ) -> THUAI7.Bullet:
        bullet = THUAI7.Bullet()
        bullet.bulletType = Proto2THUAI7.bulletTypeDict[bulletMsg.type]
        bullet.x = bulletMsg.x
        bullet.y = bulletMsg.y
        bullet.facingDirection = bulletMsg.facing_direction
        bullet.damage = bulletMsg.damage
        bullet.teamID = bulletMsg.team_id
        bullet.guid = bulletMsg.guid
        bullet.speed = bulletMsg.speed
        bullet.bombRange = bulletMsg.bomb_range
        return bullet

    @staticmethod
    def Protobuf2THUAI7Home(homeMsg: Message2Clients.MessageOfHome) -> THUAI7.Home:
        home = THUAI7.Home()
        home.x = homeMsg.x
        home.y = homeMsg.y
        home.hp = homeMsg.hp
        home.teamID = homeMsg.team_id
        return home

    @staticmethod
    def Protobuf2THUAI7Team(teamMsg: Message2Clients.MessageOfTeam) -> THUAI7.Team:
        team = THUAI7.Team()
        team.playerID = teamMsg.player_id
        team.teamID = teamMsg.team_id
        team.score = teamMsg.score
        team.energy = teamMsg.energy
        return team

    @staticmethod
    def Protobuf2THUAI7GameInfo(
        allMsg: Message2Clients.MessageOfAll,
    ) -> THUAI7.GameInfo:
        gameInfo = THUAI7.GameInfo()
        gameInfo.gameTime = allMsg.game_time
        gameInfo.redScore = allMsg.red_team_score
        gameInfo.blueScore = allMsg.blue_team_score
        return gameInfo


class THUAI72Proto:
    gameStateDict: Final[dict] = {
        THUAI7.GameState.NullGameState: MessageType.NULL_GAME_STATE,
        THUAI7.GameState.GameStart: MessageType.GAME_START,
        THUAI7.GameState.GameRunning: MessageType.GAME_RUNNING,
        THUAI7.GameState.GameEnd: MessageType.GAME_END,
    }

    placeTypeDict: Final[dict] = {
        THUAI7.PlaceType.NullPlaceType: MessageType.NULL_PLACE_TYPE,
        THUAI7.PlaceType.Home: MessageType.HOME,
        THUAI7.PlaceType.Ground: MessageType.GROUND,
        THUAI7.PlaceType.Wall: MessageType.WALL,
        THUAI7.PlaceType.Grass: MessageType.GRASS,
        THUAI7.PlaceType.River: MessageType.RIVER,
        THUAI7.PlaceType.Garbage: MessageType.GARBAGE,
        THUAI7.PlaceType.Construction: MessageType.CONSTRUCTION,
        THUAI7.PlaceType.Bridge: MessageType.BRIDGE,
    }

    shapeTypeDict: Final[dict] = {
        THUAI7.ShapeType.NullShapeType: MessageType.NULL_SHAPE_TYPE,
        THUAI7.ShapeType.Circle: MessageType.CIRCLE,
        THUAI7.ShapeType.Square: MessageType.SQUARE,
    }

    playerTypeDict: Final[dict] = {
        THUAI7.PlayerType.NullPlayerType: MessageType.NULL_PLAYER_TYPE,
        THUAI7.PlayerType.Sweeper: MessageType.SWEEPER,
        THUAI7.PlayerType.Team: MessageType.TEAM,
    }

    sweeperTypeDict: Final[dict] = {
        THUAI7.SweeperType.NullSweeperType: MessageType.NULL_SWEEPER_TYPE,
        THUAI7.SweeperType.CivilianSweeper: MessageType.CIVILIAN_SWEEPER,
        THUAI7.SweeperType.MilitarySweeper: MessageType.MILITARY_SWEEPER,
        THUAI7.SweeperType.FlagSweeper: MessageType.FLAG_SWEEPER,
    }

    sweeperStateDict: Final[dict] = {
        THUAI7.SweeperState.NullStatus: MessageType.NULL_STATUS,
        THUAI7.SweeperState.Idle: MessageType.IDLE,
        THUAI7.SweeperState.Producing: MessageType.PRODUCING,
        THUAI7.SweeperState.Constructing: MessageType.CONSTRUCTING,
        THUAI7.SweeperState.Recovering: MessageType.RECOVERING,
        THUAI7.SweeperState.Recycling: MessageType.RECYCLING,
        THUAI7.SweeperState.Attacking: MessageType.ATTACKING,
        THUAI7.SweeperState.Swinging: MessageType.SWINGING,
        THUAI7.SweeperState.Stunned: MessageType.STUNNED,
        THUAI7.SweeperState.Moving: MessageType.MOVING,
    }

    weaponTypeDict: Final[dict] = {
        THUAI7.WeaponType.NullWeaponType: MessageType.NULL_WEAPON_TYPE,
        THUAI7.WeaponType.LaserGun: MessageType.LASERGUN,
        THUAI7.WeaponType.PlasmaGun: MessageType.PLASMAGUN,
        THUAI7.WeaponType.ShellGun: MessageType.SHELLGUN,
        THUAI7.WeaponType.MissileGun: MessageType.MISSILEGUN,
        THUAI7.WeaponType.ArcGun: MessageType.ARCGUN,
    }

    constructorTypeDict: Final[dict] = {
        THUAI7.ConstructorType.NullConstructorType: MessageType.NULL_CONSTRUCTOR_TYPE,
        THUAI7.ConstructorType.Constructor1: MessageType.CONSTRUCTOR1,
        THUAI7.ConstructorType.Constructor2: MessageType.CONSTRUCTOR2,
        THUAI7.ConstructorType.Constructor3: MessageType.CONSTRUCTOR3,
    }

    armorTypeDict: Final[dict] = {
        THUAI7.ArmorType.NullArmorType: MessageType.NULL_ARMOR_TYPE,
        THUAI7.ArmorType.Armor1: MessageType.ARMOR1,
        THUAI7.ArmorType.Armor2: MessageType.ARMOR2,
        THUAI7.ArmorType.Armor3: MessageType.ARMOR3,
    }

    shieldTypeDict: Final[dict] = {
        THUAI7.ShieldType.NullShieldType: MessageType.NULL_SHIELD_TYPE,
        THUAI7.ShieldType.Shield1: MessageType.SHIELD1,
        THUAI7.ShieldType.Shield2: MessageType.SHIELD2,
        THUAI7.ShieldType.Shield3: MessageType.SHIELD3,
    }

    producerTypeDict: Final[dict] = {
        THUAI7.ProducerType.NullProducerType: MessageType.NULL_PRODUCER_TYPE,
        THUAI7.ProducerType.Producer1: MessageType.PRODUCER1,
        THUAI7.ProducerType.Producer2: MessageType.PRODUCER2,
        THUAI7.ProducerType.Producer3: MessageType.PRODUCER3,
    }

    moduleTypeDict: Final[dict] = {
        THUAI7.ModuleType.NullModuleType: MessageType.NULL_MODULE_TYPE,
        THUAI7.ModuleType.ModuleProducer1: MessageType.MODULE_PRODUCER1,
        THUAI7.ModuleType.ModuleProducer2: MessageType.MODULE_PRODUCER2,
        THUAI7.ModuleType.ModuleProducer3: MessageType.MODULE_PRODUCER3,
        THUAI7.ModuleType.ModuleConstructor1: MessageType.MODULE_CONSTRUCTOR1,
        THUAI7.ModuleType.ModuleConstructor2: MessageType.MODULE_CONSTRUCTOR2,
        THUAI7.ModuleType.ModuleConstructor3: MessageType.MODULE_CONSTRUCTOR3,
        THUAI7.ModuleType.ModuleArmor1: MessageType.MODULE_ARMOR1,
        THUAI7.ModuleType.ModuleArmor2: MessageType.MODULE_ARMOR2,
        THUAI7.ModuleType.ModuleArmor3: MessageType.MODULE_ARMOR3,
        THUAI7.ModuleType.ModuleShield1: MessageType.MODULE_SHIELD1,
        THUAI7.ModuleType.ModuleShield2: MessageType.MODULE_SHIELD2,
        THUAI7.ModuleType.ModuleShield3: MessageType.MODULE_SHIELD3,
        THUAI7.ModuleType.ModuleLaserGun: MessageType.MODULE_LASERGUN,
        THUAI7.ModuleType.ModulePlasmaGun: MessageType.MODULE_PLASMAGUN,
        THUAI7.ModuleType.ModuleShellGun: MessageType.MODULE_SHELLGUN,
        THUAI7.ModuleType.ModuleMissileGun: MessageType.MODULE_MISSILEGUN,
        THUAI7.ModuleType.ModuleArcGun: MessageType.MODULE_ARCGUN,
    }

    bulletTypeDict: Final[dict] = {
        THUAI7.BulletType.NullBulletType: MessageType.NULL_BULLET_TYPE,
        THUAI7.BulletType.Laser: MessageType.LASER,
        THUAI7.BulletType.Plasma: MessageType.PLASMA,
        THUAI7.BulletType.Shell: MessageType.SHELL,
        THUAI7.BulletType.Missile: MessageType.MISSILE,
        THUAI7.BulletType.Arc: MessageType.ARC,
    }

    constructionTypeDict: Final[dict] = {
        THUAI7.ConstructionType.NullConstructionType: MessageType.NULL_CONSTRUCTION_TYPE,
        THUAI7.ConstructionType.RecycleBank: MessageType.RECYCLEBANK,
        THUAI7.ConstructionType.ChargeStation: MessageType.CHARGESTATION,
        THUAI7.ConstructionType.SignalTower: MessageType.SIGNALTOWER,
    }

    playerTeamDict: Final[dict] = {
        THUAI7.PlayerTeam.NullTeam: MessageType.NULL_TEAM,
        THUAI7.PlayerTeam.Red: MessageType.RED,
        THUAI7.PlayerTeam.Blue: MessageType.BLUE,
    }

    @staticmethod
    def THUAI72ProtobufMoveMsg(
        playerID: int, teamID: int, time: int, angle: float
    ) -> Message2Server.MoveMsg:
        return Message2Server.MoveMsg(
            player_id=playerID,
            team_id=teamID,
            time_in_milliseconds=time,
            angle=angle
        )

    @staticmethod
    def THUAI72ProtobufIDMsg(playerID: int, teamID: int) -> Message2Server.IDMsg:
        return Message2Server.IDMsg(
            player_id=playerID,
            team_id=teamID
        )

    @staticmethod
    def THUAI72ProtobufConstructMsg(
        playerID: int, teamID: int, constructionType: THUAI7.ConstructionType
    ) -> Message2Server.ConstructMsg:
        return Message2Server.ConstructMsg(
            player_id=playerID,
            team_id=teamID,
            construction_type=THUAI72Proto.constructionTypeDict[constructionType]
        )

    @staticmethod
    def THUAI72ProtobufAttackMsg(
        playerID: int, teamID: int, angle: float
    ) -> Message2Server.AttackMsg:
        return Message2Server.AttackMsg(
            player_id=playerID,
            team_id=teamID,
            angle=angle
        )

    @staticmethod
    def THUAI72ProtobufRecoverMsg(
        playerID: int, teamID: int, recover: int
    ) -> Message2Server.RecoverMsg:
        return Message2Server.RecoverMsg(
            player_id=playerID,
            team_id=teamID,
            recover=recover
        )

    @staticmethod
    def THUAI72ProtobufSendMsg(
        playerID: int, toPlayerID: int, teamID: int, msg: str, binary: bool
    ) -> Message2Server.SendMsg:
        if binary:
            return Message2Server.SendMsg(
                player_id=playerID,
                team_id=teamID,
                binary_message=msg,
                to_player_id=toPlayerID
            )
        else:
            return Message2Server.SendMsg(
                player_id=playerID,
                team_id=teamID,
                text_message=msg,
                to_player_id=toPlayerID
            )

    @staticmethod
    def THUAI72ProtobufInstallMsg(
        playerID: int, teamID: int, moduleType: THUAI7.ModuleType
    ) -> Message2Server.InstallMsg:
        return Message2Server.InstallMsg(
            module_type=THUAI72Proto.moduleTypeDict[moduleType],
            player_id=playerID,
            team_id=teamID
        )

    @staticmethod
    def THUAI72ProtobufBuildSweeperMsg(
        teamID: int, sweeperType: THUAI7.SweeperType
    ) -> Message2Server.BuildSweeperMsg:
        return Message2Server.BuildSweeperMsg(
            team_id=teamID,
            sweeper_type=THUAI72Proto.sweeperTypeDict[sweeperType]
        )

    @staticmethod
    def THUAI72ProtobufPlayerMsg(
        playerID: int, teamID: int, sweeperType: THUAI7.SweeperType
    ) -> Message2Server.PlayerMsg:
        return Message2Server.PlayerMsg(
            player_id=playerID,
            team_id=teamID,
            sweeper_type=THUAI72Proto.sweeperTypeDict[sweeperType]
        )

    @staticmethod
    def THUAI72ProtobufRecoverMsg(
        playerID: int, recover: int, teamID: int
    ) -> Message2Server.RecoverMsg:
        return Message2Server.RecoverMsg(player_id=playerID, team_id=teamID, recover=recover)
