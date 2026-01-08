using System;
using System.Collections.Generic;
using System.Linq;
using Osiris.Vm;
using Prion.Node;

namespace Osiris.Data;

public class OsiSession(Guid id) : OsiData(id), IOsiTryFromNode<OsiSession>
{
    public Guid HostId;
    public HashSet<Guid> Gms = [];
    public HashSet<Guid> Players = [];
    public Dictionary<Guid, Guid> UserMapLookup = [];
    readonly Dictionary<Guid, OsiData> Objects = [];
    public readonly OsiVm Vm = new();
    public readonly OsiEventHandler EventHandler = new();

    public static bool TryFromNode(PrionNode prionNode, out OsiSession data)
    {
        data = default;
        if(!BaseTryFromNode(prionNode, out PrionDict dict, out Guid id)) return false;
        data = new(id);
        if(!dict.TryGet("gms", out data.Gms)) return false;
        if(!dict.TryGet("players", out data.Players)) return false;
        if(dict.TryGet("user_map_lookup?", out PrionArray prionArray))
        {
            foreach (var item in prionArray.Value)
            {
                if(!item.TryAs(out PrionDict d)) return false;
                if(!d.TryGet("user_id", out Guid userId)) return false;
                if(!d.TryGet("map_id", out Guid mapId)) return false;
                data.UserMapLookup.Add(userId, mapId);
            }
        }
        if(dict.TryGet("blobs?", out prionArray))
        {
            foreach (var item in prionArray.Value)
            {
                if(!OsiBlob.TryFromNode(item, out OsiBlob blob)) return false;
                data.Objects.Add(blob.Id, blob);
            }
        }
        return true;
    }
    public override PrionDict ToNode()
    {
        var dict = base.ToNode();
        dict.Set("gms", Gms);
        dict.Set("players", Players);
        if(UserMapLookup.Count > 0)
        {
            dict.Set("user_map_lookup?", new PrionArray([..
                UserMapLookup.Select(kvp=>{
                    var(uid, mid) = kvp;
                    var res = new PrionDict();
                    res.Set("user_id", uid);
                    res.Set("user_id", mid);
                    return res;
                })
            ]));
        }
        if(Objects.Count > 0) dict.Set("blobs?", new PrionArray([..Objects.Values.Select(b => b.ToNode())]));
        return dict;
    }
    public bool TryGetObject<T>(Guid id, out T obj) where T : OsiData
    {
        obj = default;
        if(!Objects.TryGetValue(id, out var data)) return false;
        if(!data.TryAs(out obj)) return false;
        return true;
    }
    public bool HasObject(Guid id){return Objects.ContainsKey(id);}
    public void AddObject(OsiData obj){Objects.Add(obj.Id, obj);}
}
