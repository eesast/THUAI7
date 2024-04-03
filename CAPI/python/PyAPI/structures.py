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
    Space = 2
    Ruin = 3
    Shadow = 4
    Asteroid = 5
    Resource = 6
    Construction = 7
    Wormhole = 8


class ShapeType(Enum):
    NullShapeType = 0
    Circle = 1
    Square = 2


class PlayerTeam(Enum):
    Red = 0
    Blue = 1
    NullTeam = 2


class PlayerType(Enum):
    NullPlayerType = 0
    Ship = 1
    Team = 2


class ShipType(Enum):
    NullShipType = 0
    CivilianShip = 1
    MilitaryShip = 2
    FlagShip = 3


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


class ShipState(Enum):
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
    Factory = 1
    Community = 2
    Fort = 3


class MessageOfObj(Enum):
    NullMessageOfObj = 0
    ShipMessage = 1
    BulletMessage = 2
    FactoryMessage = 3
    CommunityMessage = 4
    FortMessage = 5
    WormholeMessage = 6
    HomeMessage = 7
    ResourceMessage = 8
    MapMessage = 9
    NewsMessage = 10
    BombedBulletMessage = 11
    TeamMessage = 12


class NewsType(Enum):
    NullNewsType = 0
    TextMessage = 1
    BinaryMessage = 2


class Ship:
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
        self.shipState: ShipState = ShipState.NullStatus
        self.shipType: ShipType = ShipType.NullShipType
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
        self.factoryState: Dict[Tuple[int, int], Tuple[int, int]] = {}
        self.communityState: Dict[Tuple[int, int], Tuple[int, int]] = {}
        self.fortState: Dict[Tuple[int, int], Tuple[int, int]] = {}
        self.homeState: Dict[Tuple[int, int], Tuple[int, int]] = {}
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
