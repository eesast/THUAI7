using Preparation.Utility;
using System;

namespace GameClass.GameObj.Areas;

public class Construction : Immovable
{
    public LongInTheVariableRange HP { get; } = new LongInTheVariableRange(0);
    public override bool IsRigid => constructionType == ConstructionType.Community;
    public override ShapeType Shape => ShapeType.Square;
    private ConstructionType constructionType = ConstructionType.Null;
    public ConstructionType ConstructionType => constructionType;
    public AtomicInt ConstructNum { get; } = new AtomicInt(0);
    public bool Construct(int constructSpeed, ConstructionType constructionType, Ship ship)
    {
        if (this.constructionType != ConstructionType.Null && this.HP > 0)
            return false;
        this.constructionType = constructionType;
        return HP.AddV(constructSpeed) > 0;
    }
    public Construction(XY initPos)
        : base(initPos, GameData.NumOfPosGridPerCell / 2, GameObjType.Construction)
    {
    }
}
