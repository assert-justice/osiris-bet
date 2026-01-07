using System;
using Prion.Node;

namespace Osiris.Data;

public class OsiEntityData(Guid id) : OsiBlob(id), IOsiTryFromNode<OsiMapData>
{
    public static bool TryFromNode(PrionNode prionNode, out OsiMapData data)
    {
        throw new NotImplementedException();
    }
    public override PrionDict ToNode()
    {
        return base.ToNode();
    }
}
