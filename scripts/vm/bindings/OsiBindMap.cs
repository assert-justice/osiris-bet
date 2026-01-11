using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Jint.Native;
using Jint.Runtime.Interop;
using Osiris.Data;
using Osiris.System;
using Prion.Node;

namespace Osiris.Vm;

public class EntityWrapper : BlobWrapper
{
    public EntityWrapper(string group = "Entity"): base(group)
    {
        //
    }
    public string name
    {
        get => GetObject<OsiEntityData>().DisplayName;
        set
        {
            OsiEvent.EmitEventStandard(Id, "set_display_name", new PrionString(value));
        }
    }
    public bool isToken
    {
        get => GetObject<OsiEntityData>().IsToken;
        set
        {
            OsiEvent.EmitEventStandard(Id, "set_is_token", new PrionBoolean(value));
        }
    }
    public float angle
    {
        get => GetObject<OsiEntityData>().Angle;
        set
        {
            OsiEvent.EmitEventStandard(Id, "set_angle", new PrionF32(value));
        }
    }
    public JsArray getSize()
    {
        var size = GetObject<OsiEntityData>().Size;
        return new(OsiSystem.Session.Vm.Engine, [size.X, size.Y, size.Z]);
    }
    public void setSize(int x, int y, int z)
    {
        PrionDict dict = new();
        dict.Set("x", x);
        dict.Set("y", y);
        dict.Set("z", z);
        OsiEvent.EmitEventStandard(Id, "set_size", dict);
    }
    public JsArray getPosition()
    {
        var position = GetObject<OsiEntityData>().Position;
        return new(OsiSystem.Session.Vm.Engine, [position.X, position.Y, position.Z]);
    }
    public void setPosition(int x, int y, int z)
    {
        PrionDict dict = new();
        dict.Set("x", x);
        dict.Set("y", y);
        dict.Set("z", z);
        OsiEvent.EmitEventStandard(Id, "set_position", dict);
    }
    public bool isOwnedBy(Guid id)
    {
        return GetObject<OsiEntityData>().ControlledBy.Contains(id);
    }
    public bool isOwnedBy()
    {
        return GetObject<OsiEntityData>().ControlledBy.Contains(OsiSystem.User.Id);
    }
    public void addOwner(Guid id)
    {
        OsiEvent.EmitEventStandard(Id, "add_owner", new PrionGuid(id));
    }
    public void removeOwner(Guid id)
    {
        OsiEvent.EmitEventStandard(Id, "add_owner", new PrionGuid(id));
    }
    public JsArray listOwners()
    {
        JsString[] controlledBy = [.. GetObject<OsiEntityData>().ControlledBy.Select(static id => (JsString)id.ToString())];
        return new(OsiSystem.Session.Vm.Engine, controlledBy);
    }
}

public class MapWrapper(string group = "Map") : BlobWrapper(group)
{
    public string name
    {
        get => GetObject<OsiMapData>().DisplayName;
        set
        {
            OsiEvent.EmitEventStandard(Id, "set_display_name", new PrionString(value));
        }
    }
    public bool isFogOfWarEnabled
    {
        get => GetObject<OsiMapData>().IsFogOfWarEnabled;
        set
        {
            OsiEvent.EmitEventStandard(Id, "set_fow", new PrionBoolean(value));
        }
    }
    public int getCell(int x, int y, int z = 0, int w = 0)
    {
        return GetObject<OsiMapData>().GetCell((x,y,z,w));
    }
    public void setCells(int[][] coords, int value)
    {
        PrionArray values = new();
        PrionDict dict;
        foreach (var item in coords)
        {
            if(item.Length < 2 || item.Length > 4)
            {
                OsiSystem.Logger.ReportError("Expected array of length of 2 to 4 inclusive.");
                return;
            }
            dict = new();
            dict.Set("x", item[0]);
            dict.Set("y", item[1]);
            if(item.Length > 2) dict.Set("z", item[2]);
            if(item.Length > 3) dict.Set("w", item[3]);
            values.Value.Add(dict);
        }
        dict = new();
        dict.Set("coords", values);
        dict.Set("value", value);
        OsiEvent.EmitEventStandard(Id, "set_cells", dict);
    }
    public EntityWrapper getEntity(Guid id)
    {
        if(GetObject<OsiMapData>().TryGetEntity(id, out var ent)) return WrapInternal<EntityWrapper>(ent.Id);
        return null;
    }
    public EntityWrapper[] listEntities()
    {
        return [.. GetObject<OsiMapData>().ListEntities().Select(e => WrapInternal<EntityWrapper>(e.Id))];
    }
    public EntityWrapper[] listTokens()
    {
        return [.. GetObject<OsiMapData>().ListEntities().Where(e=>e.IsToken).Select(e => WrapInternal<EntityWrapper>(e.Id))];
    }
    public void addEntity(EntityWrapper entityWrapper)
    {
        OsiEvent.EmitEventStandard(Id, "add_entity", new PrionGuid(entityWrapper.getId()));
    }
    public void removeEntity(Guid entityId)
    {
        OsiEvent.EmitEventStandard(Id, "remove_entity", new PrionGuid(entityId));
    }
}

public static class OsiBindMap
{
    public static void Bind(OsiVm vm, Dictionary<string, JsValue> module)
    {
        var handler = OsiSystem.Session.EventHandler;
        var obj = new JsObject(vm.Engine);
        obj.Set("Map", TypeReference.CreateTypeReference<MapWrapper>(vm.Engine));
        obj.Set("Entity", TypeReference.CreateTypeReference<EntityWrapper>(vm.Engine));
        module.Add("Map", obj);
        handler.SetTypeLookup<OsiMapData>("Map");
        handler.SetTypeLookup<OsiEntityData>("Entity");

        // register map group
        void mapSetDisplayName(OsiMapData blob, OsiEvent osiEvent)
        {
            if(!osiEvent.Payload.TryAs(out PrionString prionString)){
                OsiSystem.Logger.ReportError("Could not parse event payload, wrong type.");
                return;
            }
            blob.DisplayName = prionString.Value;
        }
        void mapSetFow(OsiMapData blob, OsiEvent osiEvent)
        {
            if(!osiEvent.Payload.TryAs(out PrionBoolean value)){
                OsiSystem.Logger.ReportError("Could not parse event payload, wrong type.");
                return;
            }
            blob.IsFogOfWarEnabled = value.Value;
        }
        void mapSetCells(OsiMapData blob, OsiEvent osiEvent)
        {
            if(!osiEvent.Payload.TryAs(out PrionDict dict)){
                OsiSystem.Logger.ReportError("Could not parse event payload, wrong type.");
                return;
            }
            if(!dict.TryGet("coords", out PrionArray values)) return;
            if(!dict.TryGet("value", out int value)) return;
            List<(int, int, int, int)> coords = [];
            foreach (var item in values.Value)
            {
                if(!item.TryAs(out PrionDict coord)) return;
                coord.TryGet("x", out int x);
                coord.TryGet("y", out int y);
                if(!coord.TryGet("z", out int z)) z = 0;
                if(!coord.TryGet("w", out int w)) w = 0;
                coords.Add((x,y,z,w));
            }
            blob.SetCells([..coords], value);
        }
        void mapAddEntity(OsiMapData blob, OsiEvent osiEvent)
        {
            if(!osiEvent.Payload.TryAs(out PrionGuid value)){
                OsiSystem.Logger.ReportError("Could not parse event payload, wrong type.");
                return;
            }
            if(!OsiSystem.Session.TryGetObject(value.Value, out OsiEntityData entity))
            {
                OsiSystem.Logger.ReportError("Invalid id, not an entity.");
                return;
            }
            blob.AddEntity(entity);
        }
        void mapRemoveEntity(OsiMapData blob, OsiEvent osiEvent)
        {
            if(!osiEvent.Payload.TryAs(out PrionGuid value)){
                OsiSystem.Logger.ReportError("Could not parse event payload, wrong type.");
                return;
            }
            blob.RemoveEntity(value.Value);
            // Todo: delete entity from session? Probably.
        }
        OsiGroup.CreateGroup<OsiMapData, MapWrapper>("Map", [
            ("set_display_name", mapSetDisplayName),
            ("set_fow", mapSetFow),
            ("set_cells", mapSetCells),
            ("add_entity", mapAddEntity),
            ("remove_entity", mapRemoveEntity),
        ], (id,group)=>new OsiMapData(id, group), "Blob");

        // register entity group

        void entitySetDisplayName(OsiEntityData entity, OsiEvent osiEvent)
        {
            if(!osiEvent.Payload.TryAs(out PrionString value)){
                OsiSystem.Logger.ReportError("Could not parse event payload, wrong type.");
                return;
            }
            entity.DisplayName = value.Value;
        }
        void entitySetIsToken(OsiEntityData entity, OsiEvent osiEvent)
        {
            if(!osiEvent.Payload.TryAs(out PrionBoolean value)){
                OsiSystem.Logger.ReportError("Could not parse event payload, wrong type.");
                return;
            }
            entity.IsToken = value.Value;
        }
        void entitySetAngle(OsiEntityData entity, OsiEvent osiEvent)
        {
            if(!osiEvent.Payload.TryAs(out PrionF32 value)){
                OsiSystem.Logger.ReportError("Could not parse event payload, wrong type.");
                return;
            }
            entity.Angle = value.Value;
        }
        void entitySetPosition(OsiEntityData entity, OsiEvent osiEvent)
        {
            if(!osiEvent.Payload.TryAs(out PrionDict value)){
                OsiSystem.Logger.ReportError("Could not parse event payload, wrong type.");
                return;
            }
            value.TryGet("x", out int x);
            value.TryGet("y", out int y);
            value.TryGet("z", out int z);
            entity.Position = new(x, y, z);
        }
        void entitySetSize(OsiEntityData entity, OsiEvent osiEvent)
        {
            if(!osiEvent.Payload.TryAs(out PrionDict value)){
                OsiSystem.Logger.ReportError("Could not parse event payload, wrong type.");
                return;
            }
            value.TryGet("x", out int x);
            value.TryGet("y", out int y);
            value.TryGet("z", out int z);
            entity.Size = new(x, y, z);
        }
        void entityAddOwner(OsiEntityData entity, OsiEvent osiEvent)
        {
            if(!osiEvent.Payload.TryAs(out PrionGuid value)){
                OsiSystem.Logger.ReportError("Could not parse event payload, wrong type.");
                return;
            }
            entity.ControlledBy.Add(value.Value);
        }
        void entityRemoveOwner(OsiEntityData entity, OsiEvent osiEvent)
        {
            if(!osiEvent.Payload.TryAs(out PrionGuid value)){
                OsiSystem.Logger.ReportError("Could not parse event payload, wrong type.");
                return;
            }
            entity.ControlledBy.Remove(value.Value);
        }
        OsiGroup.CreateGroup<OsiEntityData, EntityWrapper>("Entity", [
            ("set_display_name", entitySetDisplayName),
            ("set_is_token", entitySetIsToken),
            ("set_angle", entitySetAngle),
            ("set_size", entitySetSize),
            ("set_position", entitySetPosition),
            ("add_owner", entityAddOwner),
            ("remove_owner", entityRemoveOwner),
        ], (id, group)=> new OsiEntityData(id, group), "Blob");
    }
}
