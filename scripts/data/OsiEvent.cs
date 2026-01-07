using System;
using Osiris.System;
using Prion.Node;

namespace Osiris.Data;

public class OsiEvent : OsiData, IOsiTryFromNode<OsiEvent>
{
    public readonly Guid UserId;
    public Guid TargetId;
    public string Verb;
    public DateTime Timestamp;
    public PrionNode Payload;
    OsiEvent(Guid id, Guid userId, Guid targetId, string verb, DateTime timestamp, PrionNode payload) : base(id, "event")
    {
        UserId = userId;
        TargetId = targetId;
        Verb = verb;
        Timestamp = timestamp;
        Payload = payload;
    }

    public static bool TryFromNode(PrionNode prionNode, out OsiEvent data)
    {
        data = default;
        if(!BaseTryFromNode(prionNode, out PrionDict dict, out Guid id)) return false;
        // if(!prionNode.TryAs(out PrionDict dict)) return false;
        // if(!dict.TryGet("id", out Guid id)) return false;
        if(!dict.TryGet("user_id", out Guid userId)) return false;
        if(!dict.TryGet("target_id", out Guid targetId)) return false;
        if(!dict.TryGet("verb", out string verb)) return false;
        if(!dict.TryGet("timestamp", out ulong timestamp)) return false;
        if(!dict.TryGet("payload", out PrionNode payload)) return false;
        data = new(id, userId, targetId, verb, new((long)timestamp), payload);
        // return BaseTryFromNode(prionNode, (id,_)=>new )
        return true;
    }
    public override PrionDict ToNode()
    {
        var dict = base.ToNode();
        dict.Set("user_id", UserId);
        dict.Set("target_id", TargetId);
        dict.Set("timestamp", (ulong)Timestamp.Ticks);
        dict.Set("payload", Payload);
        return dict;
    }
    public static void EmitEvent(Guid targetId, string verb, PrionNode payload)
    {
        OsiEvent osiEvent = new(Guid.NewGuid(), OsiSystem.User.Id, targetId, verb, DateTime.UtcNow, payload);
        OsiSystem.Session.EventHandler.DispatchEvent(osiEvent);
    }
}
