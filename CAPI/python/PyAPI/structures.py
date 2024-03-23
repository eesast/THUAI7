from enum import Enum
from typing import List, Dict
import sys

if sys.version_info < (3, 9):
    from typing import Tuple
else:
    Tuple = tuple


class GameState(Enum):
    NullGameState = 0
    GameStart = 1
    GameRunning = 2
    GameEnd = 3


class PlaceType(Enum):
    NullPlaceType = 0
    Home = 1
    Ground = 2
    Wall = 3
    Grass = 4
    River = 5
    Garbage = 6
    Construction = 7
    Bridge = 8


class ShapeType(Enum):
    NullShapeType = 0
    Circle = 1
    Square = 2


class PlayerTeam(Enum):
    NullTeam = 0
    Red = 1
    Blue = 2


playerTeamDict = {PlayerTeam.NullTeam: 0, PlayerTeam.Red: 1, PlayerTeam.Blue: 2}


class PlayerType(Enum):
    NullPlayerType = 0
    Sweeper = 1
    Team = 2


class SweeperType(Enum):
    NullSweeperType = 0
    CivilianSweeper = 1
    MilitarySweeper = 2
    FlagSweeper = 3


class WeaponType(Enum):
    NullWeaponType = 0
    LaserGun = 1
    PlasmaGun = 2
    ShellGun = 3
    MissileGun = 4
    ArcGun = 5


class ConstructorType(Enum):
    NullConstructorType = 0
    Constructor1 = 1
    Constructor2 = 2
    Constructor3 = 3


class ArmorType(Enum):
    NullArmorType = 0
    Armor1 = 1
    Armor2 = 2
    Armor3 = 3


class ShieldType(Enum):
    NullShieldType = 0
    Shield1 = 1
    Shield2 = 2
    Shield3 = 3


class ProducerType(Enum):
    NullProducerType = 0
    Producer1 = 1
    Producer2 = 2
    Producer3 = 3


class ModuleType(Enum):
    NullModuleType = 0
    ModuleProducer1 = 1
    ModuleProducer2 = 2
    ModuleProducer3 = 3
    ModuleConstructor1 = 4
    ModuleConstructor2 = 5
    ModuleConstructor3 = 6
    ModuleArmor1 = 7
    ModuleArmor2 = 8
    ModuleArmor3 = 9
    ModuleShield1 = 10
    ModuleShield2 = 11
    ModuleShield3 = 12
    ModuleLaserGun = 13
    ModulePlasmaGun = 14
    ModuleShellGun = 15
    ModuleMissileGun = 16
    ModuleArcGun = 17


class SweeperState(Enum):
    NullStatus = 0
    Idle = 1
    Producing = 2
    Constructing = 3
    Recovering = 4
    Recycling = 5
    Attacking = 6
    Swinging = 7
    Stunned = 8
    Moving = 9


class BulletType(Enum):
    NullBulletType = 0
    Laser = 1
    Plasma = 2
    Shell = 3
    Missile = 4
    Arc = 5


class ConstructionType(Enum):
    NullConstructionType = 0
    RecycleBank = 1
    ChargeStation = 2
    SignalTower = 3


class MessageOfObj(Enum):
    NullMessageOfObj = 0
    SweeperMessage = 1
    BulletMessage = 2
    RecycleBankMessage = 3
    ChargeStationMessage = 4
    SignalTowerMessage = 5
    BridgeMessage = 6
    HomeMessage = 7
    GarbageMessage = 8
    MapMessage = 9
    NewsMessage = 10
    BombedBulletMessage = 11
    TeamMessage = 12


class NewsType(Enum):
    NullNewsType = 0
    TextMessage = 1
    BinaryMessage = 2


class Sweeper:
    def __init__(self):
        self.x: int = 0
        self.y: int = 0
        self.speed: int = 0
        self.hp: int = 0
        self.armor: int = 0
        self.shield: int = 0
        self.playerID: int = 0
        self.teamID: int = 0
        self.guid: int = 0
        self.sweeperState: SweeperState = SweeperState.NullStatus
        self.sweeperType: SweeperType = SweeperType.NullSweeperType
        self.viewRange: int = 0
        self.producerType: ProducerType = ProducerType.NullProducerType
        self.constructorType: ConstructorType = ConstructorType.NullConstructorType
        self.armorType: ArmorType = ArmorType.NullArmorType
        self.shieldType: ShieldType = ShieldType.NullShieldType
        self.weaponType: WeaponType = WeaponType.NullWeaponType
        self.facingDirection: float = 0.0


class Team:
    def __init__(self):
        self.playerID: int = 0
        self.teamID: int = 0
        self.score: int = 0
        self.energy: int = 0


class Home:
    def __init__(self):
        self.x: int = 0
        self.y: int = 0
        self.hp: int = 0
        self.teamID: int = 0
        self.guid: int = 0


class Bullet:
    def __init__(self):
        self.x: int = 0
        self.y: int = 0
        self.facingDirection: float = 0.0
        self.guid: int = 0
        self.teamID: int = 0
        self.bulletType: BulletType = BulletType.NullBulletType
        self.damage: int = 0
        self.attackRange: int = 0
        self.bombRange: int = 0
        self.explodeRange: float = 0.0
        self.speed: int = 0


class GameMap:
    def __init__(self):
        self.recycleBankState: Dict[Tuple[int, int], Tuple[int, int]] = {}
        self.chargeStationState: Dict[Tuple[int, int], Tuple[int, int]] = {}
        self.signalTowerState: Dict[Tuple[int, int], Tuple[int, int]] = {}
        self.HomeState: Dict[Tuple[int, int], Tuple[int, int]] = {}
        self.bridgeState: Dict[Tuple[int, int], int] = {}
        self.garbageState: Dict[Tuple[int, int], int] = {}


class GameInfo:
    def __init__(self):
        self.gameTime: int = 0
        self.redScore: int = 0
        self.redEnergy: int = 0
        self.redHomeHp: int = 0
        self.blueScore: int = 0
        self.blueEnergy: int = 0
        self.blueHomeHp: int = 0
