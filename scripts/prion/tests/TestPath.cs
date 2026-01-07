using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prion.Node;

namespace Prion.Tests;

[TestClass]
public class TestPath
{
    [TestMethod]
    public void Basic()
    {
        float val = 100;
        PrionDict stats = new();
        stats.Set("hp", val);
        Assert.IsTrue(stats.TryGetPath("hp", out PrionF32 data));
        Assert.IsTrue(val == data.Value);
        float nextVal = 50;
        Assert.IsTrue(stats.TrySetPath("hp", new PrionF32(nextVal)));
        Assert.IsTrue(stats.TryGetPath("hp", out data));
        if(nextVal != data.Value) Assert.Fail($"data value is {data.Value}");
        float ac = 10;
        Assert.IsFalse(stats.TrySetPath("ac", new PrionF32(ac)));
        Assert.IsTrue(stats.TrySetPath("ac", new PrionF32(ac), true));
        Assert.IsTrue(stats.TryGetPath("ac", out data));
        Assert.IsTrue(ac == data.Value);
        Assert.IsFalse(stats.TrySetPath("ac", new PrionString("butts"), true));
        Assert.IsTrue(stats.TrySetPath("ac", new PrionString("butts"), true, true));
        Assert.IsTrue(stats.TryGetPath("ac", out PrionString dataStr));
        Assert.IsTrue("butts" == dataStr.Value);
        Assert.IsFalse(stats.TrySetPath("attributes", new PrionDict()));
        Assert.IsTrue(stats.TrySetPath("attributes", new PrionDict(), true));
        Assert.IsTrue(stats.TryGetPath("attributes", out PrionDict prionDict));
        Assert.IsTrue(prionDict.Value.Count == 0);
        float str = 12;
        Assert.IsFalse(stats.TrySetPath("attributes/str", new PrionF32(str)));
        Assert.IsTrue(stats.TrySetPath("attributes/str", new PrionF32(str), true));
        Assert.IsTrue(stats.TryGetPath("attributes", out prionDict));
        Assert.IsTrue(prionDict.Value.Count == 1);
        Assert.IsTrue(stats.TryGetPath("attributes/str", out data));
        if(str != data.Value) Assert.Fail($"data value is {data.Value}");
        Assert.IsTrue(stats.TrySetPath("statuses/bleeding", new PrionF32(1), true));
        Assert.IsTrue(stats.TryGetPath("statuses/bleeding", out data));
        Assert.IsTrue(1 == data.Value);
        Assert.IsTrue(stats.TrySetPath("inventory", new PrionArray(), true));
        Assert.IsTrue(stats.TrySetPath("inventory/0/name", new PrionString("greatsword"), true));
        Assert.IsTrue(stats.TryGetPath("inventory/0/name", out dataStr));
        Assert.IsTrue("greatsword" == dataStr.Value);
        Assert.IsTrue(stats.TrySetPath("inventory/50/name", new PrionString("dagger"), true));
        Assert.IsTrue(stats.TryGetPath("inventory/1/name", out dataStr));
        Assert.IsTrue("dagger" == dataStr.Value);
    }
}
