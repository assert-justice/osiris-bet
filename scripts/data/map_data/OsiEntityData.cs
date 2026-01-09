using System;
using System.Collections.Generic;
using Godot;
using Prion.Node;

namespace Osiris.Data;

public class OsiEntityData(Guid id, string group = "Entity") : OsiBlob(id, group), IOsiTryFromNode<OsiEntityData>
{
    public Guid MapId;
    public string DisplayName = "[Unnamed Entity]";
    public HashSet<Guid> ControlledBy = [];
    public HashSet<Guid> VisibleTo = [];
    public bool IsVisibleToAll = false;
    public Vector3I Size = Vector3I.One;
    public Vector3I Position = Vector3I.Zero;
    public float Angle = 0;
    public float VisionRadius = 10;
    public bool IsToken = false;
    // Todo: add lights
    // Todo: add auras?
    // Todo: add renderer data
    public static bool TryFromNode(PrionNode prionNode, out OsiEntityData data)
    {
        return TryFromNodeFactory(prionNode, (id, group)=>new(id, group), out data);
    }
    protected override bool TryAppend(PrionDict dict)
    {
        if(!base.TryAppend(dict)) return false;
        if(!dict.TryGet("map_id", out MapId)) return false;
        if(dict.TryGet("display_name?", out string displayName)) DisplayName = displayName;
        if(!dict.TryGet("controlled_by", out ControlledBy)) return false;
        if(!dict.TryGet("visible_to", out VisibleTo)) return false;
        if(!dict.TryGet("is_visible_to_all", out IsVisibleToAll)) return false;
        if(!dict.TryGet("size", out PrionVector3I size)) return false;
        Size = new(size.X, size.Y, size.Z);
        if(!dict.TryGet("position", out PrionVector3I position)) return false;
        Position = new(position.X, position.Y, position.Z);
        if(!dict.TryGet("angle", out Angle)) return false;
        if(!dict.TryGet("vision_radius", out VisionRadius)) return false;
        if(!dict.TryGet("is_token", out IsToken)) return false;
        return true;
    }
    public override PrionDict ToNode()
    {
        var dict = base.ToNode();
        dict.Set("map_id", MapId);
        dict.Set("display_name?", DisplayName);
        dict.Set("controlled_by", ControlledBy);
        dict.Set("visible_to", VisibleTo);
        dict.Set("is_visible_to_all", IsVisibleToAll);
        dict.Set("size", new PrionVector3I(Size.X, Size.Y, Size.Z));
        dict.Set("position", new PrionVector3I(Position.X, Position.Y, Position.Z));
        dict.Set("angle", Angle);
        dict.Set("vision_radius", VisionRadius);
        dict.Set("is_token", IsToken);
        return dict;
    }
}
