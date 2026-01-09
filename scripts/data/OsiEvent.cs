using System;
using System.Collections.Generic;
using Osiris.System;
using Prion.Node;

namespace Osiris.Data;

public class OsiEvent : OsiData, IOsiTryFromNode<OsiEvent>
{
    public enum Scope
    {
        Host = 1,
        AllGms = 2,
        AllSpectators = 4,
        AllPlayers = 8,
    }
    public readonly Guid UserId;
    public readonly Guid TargetId;
    public readonly string Verb;
    public readonly DateTime Timestamp;
    public readonly PrionNode Payload;
    public readonly Scope EventScope = 0;
    readonly HashSet<Guid> VisibleTo;
    OsiEvent(Guid id, Guid userId, Guid targetId, string verb, DateTime timestamp, PrionNode payload, Scope eventScope, HashSet<Guid> visibleTo) : base(id, "event")
    {
        UserId = userId;
        TargetId = targetId;
        Verb = verb;
        Timestamp = timestamp;
        Payload = payload;
        EventScope = eventScope;
        VisibleTo = visibleTo ?? [];
    }

    public static bool TryFromNode(PrionNode prionNode, out OsiEvent data)
    {
        data = default;
        if(!BaseTryFromNode(prionNode, out PrionDict dict, out Guid id)) return false;
        if(!dict.TryGet("user_id", out Guid userId)) return false;
        if(!dict.TryGet("target_id", out Guid targetId)) return false;
        if(!dict.TryGet("verb", out string verb)) return false;
        if(!dict.TryGet("timestamp", out ulong timestamp)) return false;
        if(!dict.TryGet("payload", out PrionNode payload)) return false;
        if(!dict.TryGet("event_scope", out int scope)) return false;
        if(!dict.TryGet("visible_to", out HashSet<Guid> visibleTo)) return false;
        data = new(id, userId, targetId, verb, new((long)timestamp), payload, (Scope)scope, visibleTo);
        return true;
    }
    public override PrionDict ToNode()
    {
        var dict = base.ToNode();
        dict.Set("user_id", UserId);
        dict.Set("target_id", TargetId);
        dict.Set("timestamp", (ulong)Timestamp.Ticks);
        dict.Set("payload", Payload);
        dict.Set("event_scope", (int)EventScope);
        dict.Set("visible_to", VisibleTo);
        return dict;
    }
    public bool ShouldExecute()
    {
        if(VisibleTo.Contains(OsiSystem.User.Id)) return true;
        bool isHost = OsiSystem.IsHost();
        bool isGm = OsiSystem.IsGm();
        bool isPlayer = OsiSystem.IsPlayer();
        bool isSpectator = !(isHost || isGm || isPlayer);
        if((EventScope & Scope.Host) != 0 && isHost) return true;
        if((EventScope & Scope.AllGms) != 0 && isGm) return true;
        if((EventScope & Scope.AllPlayers) != 0 && isPlayer) return true;
        if((EventScope & Scope.AllSpectators) != 0 && isSpectator) return true;
        return false;
    }
    public static void EmitEvent(
        Guid targetId, 
        string verb,
        PrionNode payload, 
        Scope eventScope = Scope.Host,
        Guid[] visibleTo = null)
    {
        OsiEvent osiEvent = new(
            Guid.NewGuid(), 
            OsiSystem.User.Id, 
            targetId, verb, 
            DateTime.UtcNow, 
            payload,
            eventScope,
            (visibleTo is not null) ? [..visibleTo] : []);
        OsiSystem.Session.EventHandler.DispatchEvent(osiEvent);
    }
    public static void EmitEventAll(
        Guid targetId, 
        string verb,
        PrionNode payload)
    {
        EmitEvent(targetId, verb, payload, Scope.Host | Scope.AllGms | Scope.AllPlayers | Scope.AllSpectators);
    }
    public static void EmitEventSingle(
        Guid targetId, 
        string verb,
        PrionNode payload,
        Guid visibleTo)
    {
        EmitEvent(targetId, verb, payload, 0, [visibleTo]);
    }
    public static void EmitEventStandard(
        Guid targetId, 
        string verb,
        PrionNode payload)
    {
        EmitEvent(targetId, verb, payload, Scope.Host | Scope.AllGms, [OsiSystem.User.Id]);
    }
}
