using System;
using Osiris.System;
using Prion.Node;

namespace Osiris.Data;

public class OsiBlob(Guid id, string group = "Blob") : OsiData(id, group), IOsiTryFromNode<OsiBlob>
{
    PrionNode Data = new PrionDict();
    public PrionNode GetData(){return Data;}
    public void SetData(PrionNode data){Data = data;}
    public static bool TryFromNode(PrionNode prionNode, out OsiBlob data)
    {
        return TryFromNodeFactory(prionNode, (id,group)=>new OsiBlob(id, group), out data, out PrionDict dict);
    }
    protected override bool TryAppend(PrionDict dict)
    {
        if(!base.TryAppend(dict)) return false;
        if(dict.TryGet("data?", out PrionNode dat)) Data = dat;
        return true;
    }
    public override PrionDict ToNode()
    {
        var dict = base.ToNode();
        if(Data is not null) dict.Set("data?", Data);
        return dict;
    }
}
