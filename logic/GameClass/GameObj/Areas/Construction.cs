using Preparation.Utility;
using System;

namespace GameClass.GameObj.Areas;

public class Construction : Immovable
{
    public LongWithVariableRange HP => throw new NotImplementedException();
    public override bool IsRigid => constructionType == ConstructionType.Community;
    public override ShapeType Shape => ShapeType.Square;
    private ConstructionType constructionType = ConstructionType.Null;
    public ConstructionType ConstructionType => constructionType;
    public Construction(XY initPos)
        : base(initPos, GameData.NumOfPosGridPerCell / 2, GameObjType.Construction)
    {
    }
}
