using System;
using System.Collections.Generic;
using System.Reflection;
using Jint.Native;
using Jint.Runtime.Interop;
using Osiris.Data;
using Osiris.System;
using Prion.Node;

namespace Osiris.Vm;

// public class CellWrapper(int x, int y, int z)
// {
//     OsiCellData Data = new(new(x,y,z));
//     public BitfieldWrapper volume
//     {
//         get => new(Data.Volume);
//         set{Data.Volume = OsiBindBitfield.GetData(value);}
//     }
//     public BitfieldWrapper top
//     {
//         get => new(Data.Top);
//         set{Data.Top = OsiBindBitfield.GetData(value);}
//     }
//     public BitfieldWrapper bottom
//     {
//         get => new(Data.Bottom);
//         set{Data.Bottom = OsiBindBitfield.GetData(value);}
//     }
//     public BitfieldWrapper north
//     {
//         get => new(Data.North);
//         set{Data.North = OsiBindBitfield.GetData(value);}
//     }
//     public BitfieldWrapper south
//     {
//         get => new(Data.South);
//         set{Data.South = OsiBindBitfield.GetData(value);}
//     }
//     public BitfieldWrapper east
//     {
//         get => new(Data.East);
//         set{Data.East = OsiBindBitfield.GetData(value);}
//     }
//     public BitfieldWrapper west
//     {
//         get => new(Data.West);
//         set{Data.West = OsiBindBitfield.GetData(value);}
//     }
// }

public class EntityWrapper(MapWrapper map) : BlobWrapper
{
    MapWrapper Map = map;
}

public class MapWrapper(string group = "Map") : BlobWrapper(group)
{
    protected override void Constructor(string group)
    {
        var blob = new OsiMapData(Id, group);
        OsiEvent.EmitEvent(Id, group, blob.ToNode());
    }
}

public static class OsiBindMap
{
    public static void Bind(OsiVm vm, Dictionary<string, JsValue> module)
    {
        var obj = new JsObject(vm.Engine);
        // obj.Set("Cell", TypeReference.CreateTypeReference<CellWrapper>(vm.Engine));
        obj.Set("Map", TypeReference.CreateTypeReference<MapWrapper>(vm.Engine));
        module.Add("Map", obj);

        // register event handlers
        // var handler = OsiSystem.Session.EventHandler;
        // handler.RegisterConstructor("map_create", osiEvent =>
        // {
        //     if(!OsiMapData.TryFromNode(osiEvent.Payload, out var blob))
        //     {
        //         OsiSystem.Logger.Log("Failed to parse event payload as blob.");
        //         return null;
        //     }
        //     return blob;
        // });
        // handler.RegisterNamedEventHandler<OsiMapData>("map_set_display_name", (blob, osiEvent) =>
        // {
        //     if(!osiEvent.Payload.TryAs(out PrionString prionString)){
        //         OsiSystem.Logger.ReportError("Could not parse event payload, wrong type.");
        //         return;
        //     }
        //     blob.DisplayName = prionString.Value;
        // });
        // handler.RegisterNamedEventHandler<OsiMapData>("map_set_size", (blob, osiEvent) =>
        // {
        //     if(!osiEvent.Payload.TryAs(out PrionVector3I vec)){
        //         OsiSystem.Logger.ReportError("Could not parse event payload, wrong type.");
        //         return;
        //     }
        //     blob.Size = new(vec.X, vec.Y, vec.Z);
        // });
        // handler.RegisterNamedEventHandler<OsiMapData>("map_set_fow", (blob, osiEvent) =>
        // {
        //     if(!osiEvent.Payload.TryAs(out PrionBoolean b)){
        //         OsiSystem.Logger.ReportError("Could not parse event payload, wrong type.");
        //         return;
        //     }
        //     blob.FogOfWarEnabled = b.Value;
        // });
        // handler.RegisterNamedEventHandler<OsiMapData>("map_set_cells", (blob, osiEvent) =>
        // {
        //     if(!osiEvent.Payload.TryAs(out PrionArray array)){
        //         OsiSystem.Logger.ReportError("Could not parse event payload, wrong type.");
        //         return;
        //     }
        //     foreach (var item in array.Value)
        //     {
        //         if(!OsiCellData.TryFromNode(item, out var cell))
        //         {
        //             OsiSystem.Logger.ReportError("Could not parse event payload, array element had wrong type.");
        //             return;
        //         }
        //         blob.Cells[cell.Position] = cell;
        //     }
        // });
    }
    // public static OsiCellData GetData(CellWrapper cellWrapper)
    // {
    //     FieldInfo info = typeof(CellWrapper).GetField("Data", BindingFlags.NonPublic | BindingFlags.Instance);
    //     return info.GetValue(cellWrapper) as OsiCellData;
    // }
}
