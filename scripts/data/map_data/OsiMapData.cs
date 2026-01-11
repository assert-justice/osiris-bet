using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Osiris.System;
using Prion.Node;

namespace Osiris.Data;

using CellCoord = (int, int, int, int);

public class OsiMapData(Guid id, string group) : OsiBlob(id, group), IOsiTryFromNode<OsiMapData>
{
    public string DisplayName = "[A Mysterious Location]";
    public Vector3I Size = Vector3I.Zero;
    public Vector3I Origin = Vector3I.Zero;
    public bool IsFogOfWarEnabled = false;
    readonly Dictionary<CellCoord, int> Cells = [];
    readonly HashSet<Guid> EntityIds = [];
    readonly Dictionary<Guid, OsiEntityData> Entities = [];
    public static bool TryFromNode(PrionNode prionNode, out OsiMapData data)
    {
        return TryFromNodeFactory(prionNode, (id,group)=> new OsiMapData(id, group), out data);
    }
    protected override bool TryAppend(PrionDict dict)
    {
        if(!base.TryAppend(dict)) return false;
        if(!dict.TryGet("display_name", out DisplayName)) return false;
        // if(!dict.TryGet("size", out PrionVector3I size)) return false;
        // Size = new(size.X, size.Y, size.Z);
        if(dict.TryGet("fog_of_war_enabled?", out bool fow)) IsFogOfWarEnabled = fow;
        if(dict.TryGet("cells?", out PrionArray cells))
        {
            foreach (var item in cells.Value)
            {
                if(!item.TryAs(out PrionString str)) return false;
                var values = str.Value.Split(',');
                if(values.Length != 5) return false;
                if(!int.TryParse(values[0], out int x)) return false;
                if(!int.TryParse(values[0], out int y)) return false;
                if(!int.TryParse(values[0], out int z)) return false;
                if(!int.TryParse(values[0], out int w)) return false;
                if(!int.TryParse(values[0], out int val)) return false;
                Cells[(x,y,z,w)] = val;
            }
            // Todo: find extents (size and origin)
        }
        if(dict.TryGet("entities?", out HashSet<Guid> ids))
        {
            foreach (var item in ids)
            {
                if(OsiSystem.Session.TryGetObject<OsiEntityData>(item, out var ent)) Entities.Add(ent.Id, ent);
                else EntityIds.Add(item);
            }
        }
        return true;

    }

    public override PrionDict ToNode()
    {
        var dict = base.ToNode();
        dict.Set("display_name", DisplayName);
        // dict.Set("size", new PrionVector3I(Size.X, Size.Y, Size.Z));
        dict.Set("fog_of_war_enabled?", IsFogOfWarEnabled);
        if(Cells.Count > 0) dict.Set("cells?", new PrionArray([..Cells.Select(c => {
            var(coord, val) = c;
            var(x,y,z,w) = coord;
            return new PrionString(string.Join(',', x, y, z, w, val));
        })]));
        HashSet<Guid> entIds = [];
        foreach (var item in EntityIds)
        {
            entIds.Add(item);
        }
        foreach (var item in Entities.Keys)
        {
            entIds.Add(item);
        }
        if(entIds.Count > 0) dict.Set("entities?", entIds);
        return dict;
    }
    public int GetCell(CellCoord coord)
    {
        if(Cells.TryGetValue(coord, out int value)) return value;
        return 0;
    }
    public void SetCells(CellCoord[] coords, int value)
    {
        foreach (var item in coords)
        {
            Cells[item] = value;
        }
    }
    public bool TryGetEntity(Guid id, out OsiEntityData ent)
    {
        if(Entities.TryGetValue(id, out ent)) return true;
        else if(!EntityIds.Contains(id)) return false;
        if(!OsiSystem.Session.TryGetObject(id, out ent)) return false;
        Entities.Add(id, ent);
        EntityIds.Remove(id);
        return true;
    }
    public OsiEntityData[] ListEntities()
    {
        List<OsiEntityData> entities = [..Entities.Values];
        foreach (var item in EntityIds) // will iterating over and modifying entity ids at the same time be a problem?
        {
            if(TryGetEntity(item, out var ent)) entities.Add(ent);
        }
        return [..entities];
    }
    public void AddEntity(OsiEntityData entity)
    {
        Entities.Add(entity.Id, entity);
    }
    public bool RemoveEntity(Guid entityId)
    {
        return Entities.Remove(entityId);
    }
}
