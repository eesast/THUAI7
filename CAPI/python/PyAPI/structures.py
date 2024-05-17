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
    """
    :attr x: X坐标, 单位Grid
    :attr y: Y坐标, 单位Grid
    :attr speed: 速度, 单位Grid/s
    :attr hp: 生命值
    :attr armor: 装甲值
    :attr shield: 护盾值
    :attr playerID: 舰船编号, 1~4
    :attr teamID: 所属队伍编号
    :attr guid: GUID
    :attr shipState: 行为状态
    :attr shipType: 舰船类型
    :attr viewRange: 视距
    :attr producerType: 采集器类型
    :attr constructorType: 建造器类型
    :attr armorType: 装甲类型
    :attr shieldType: 护盾类型
    :attr weaponType: 武器类型
    :attr facingDirection: 面朝方向的弧度数, 向下为x轴正方向, 向右为y轴正方向
    """

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
    """
    :attr playerID: 队内编号, 应当为0
    :attr teamID: 所属队伍编号
    :attr score: 得分
    :attr energy: 经济
    """

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

class ConstructionState:
    def __init__(self,teamID,HP,type:ConstructionType):
        self.teamID=teamID
        self.hp=HP
        self.constructionType=type

class Bullet:
    """
    :attr x: X坐标, 单位Grid
    :attr y: Y坐标, 单位Grid
    :attr facingDirection: 运动方向的弧度数, 向下为x轴正方向, 向右为y轴正方向
    :attr guid: GUID
    :attr teamID: 所属队伍编号
    :attr bulletType: 子弹类型
    :attr damage: 伤害
    :attr attackRange: 射程
    :attr bombRange: 爆炸半径
    :attr speed: 运动速度, 单位Grid/s
    """

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
        self.speed: int = 0


class GameMap:
    def __init__(self):
        self.factoryState: Dict[Tuple[int, int], Tuple[int, int]] = {}
        self.communityState: Dict[Tuple[int, int], Tuple[int, int]] = {}
        self.fortState: Dict[Tuple[int, int], Tuple[int, int]] = {}
        self.homeState: Dict[Tuple[int, int], Tuple[int, int]] = {}
        self.wormholeState: Dict[Tuple[int, int], int] = {}
        self.resourceState: Dict[Tuple[int, int], int] = {}


class GameInfo:
    """
    :attr gameTime: 当前游戏时间
    :attr redScore: 红队当前分数
    :attr redEnergy: 红队当前经济
    :attr redHomeHp: 红队当前基地血量
    :attr blueScore: 蓝队当前分数
    :attr blueEnergy: 蓝队当前经济
    :attr blueHomeHp: 蓝队当前基地血量
    """

    def __init__(self):
        self.gameTime: int = 0
        self.redScore: int = 0
        self.redEnergy: int = 0
        self.redHomeHp: int = 0
        self.blueScore: int = 0
        self.blueEnergy: int = 0
        self.blueHomeHp: int = 0
