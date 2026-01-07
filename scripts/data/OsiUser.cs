using System;
using Prion.Node;

namespace Osiris.Data;

public class OsiUser(Guid id) : OsiData(id, "user"), IOsiTryFromNode<OsiUser>
{
    public string DisplayName = "[No username set]";
    public string PfpFilename = "";
    public static bool TryFromNode(PrionNode prionNode, out OsiUser data)
    {
        data = default;
        if(!BaseTryFromNode(prionNode, out PrionDict dict, out Guid id)) return false;
        data = new(id);
        if(dict.TryGet("display_name?", out string displayName)) data.DisplayName = displayName;
        if(dict.TryGet("pfp_filename?", out string pfpFilename)) data.PfpFilename = pfpFilename;
        return true;
    }
    public override PrionDict ToNode()
    {
        var dict = base.ToNode();
        dict.Set("display_name?", DisplayName);
        dict.Set("pfp_filename?", PfpFilename);
        return dict;
    }
}
