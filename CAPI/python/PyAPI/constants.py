class Constants:
    frameDuration = 50
    numofGridPerCell = 1000
    rows = 50
    cols = 50
    maxResourceProgress = 200
    maxWormholeHp = 18000
    robPercent = 0.2
    destroyBuildingBonus = 200
    recoverMultiplier = 1.2
    recycleMultiplier = 0.5

    sizeOfShip = 800


class Home:
    maxHp = 24000
    energySpeed = 1


class Factory:
    maxHp = 8000
    energySpeed = 3


class Community:
    maxHp = 6000


class Fort:
    maxHp = 12000
    attackRange = 8000
    damage = 1200


class CivilianShip:
    Speed = 3000
    basicArmor = 0
    basicShield = 0
    maxHp = 3000
    Cost = 40


class MilitaryShip:
    Speed = 2800
    basicArmor = 400
    basicShield = 400
    maxHp = 4000
    Cost = 120


class FlagShip:
    Speed = 2700
    basicArmor = 800
    basicShield = 800
    maxHp = 12000
    Cost = 500


class Producer:
    energySpeed1 = 5
    energySpeed2 = 7
    energySpeed3 = 10
    Cost1 = 0
    Cost2 = 40
    Cost3 = 80


class Constructor:
    constructSpeed1 = 500
    constructSpeed2 = 750
    constructSpeed3 = 1000
    Cost1 = 0
    Cost2 = 40
    Cost3 = 80


class Armor:
    armor1 = 2000
    armor2 = 3000
    armor3 = 4000
    Cost1 = 60
    Cost2 = 120
    Cost3 = 180


class Shield:
    shield1 = 2000
    shield2 = 3000
    shield3 = 4000
    Cost1 = 60
    Cost2 = 120
    Cost3 = 180


class Weapon:
    LaserCost = 0
    PlasmaCost = 120
    ShellCost = 130
    MissleCost = 180
    ArcCost = 240


class Laser:
    Damage = 1200
    AttackRange = 4000
    ArmorDamageMultiplier = 1.5
    ShieldDamageMultiplier = 0.6
    Speed = 20000
    CastTime = 300
    BackSwing = 300


class Plasma:
    Damage = 1300
    AttackRange = 4000
    ArmorDamageMultiplier = 2
    ShieldDamageMultiplier = 0.4
    Speed = 10000
    CastTime = 400
    BackSwing = 400


class Shell:
    Damage = 1800
    AttackRange = 4000
    ArmorDamageMultiplier = 0.4
    ShieldDamageMultiplier = 1.5
    Speed = 8000
    CastTime = 200
    BackSwing = 200


class Missle:
    Damage = 1600
    AttackRange = 8000
    ExplodeRange = 1600
    ArmorDamageMultiplier = 1.0
    ShieldDamageMultiplier = 0.4
    Speed = 6000
    CastTime = 600
    BackSwing = 600


class Arc:
    Damage = 3200
    AttackRange = 8000
    ArmorDamageMultiplier = 2.0
    ShieldDamageMultiplier = 2.0
    Speed = 8000
    CastTime = 600
    BackSwing = 600
