using Factions;
using NUnit.Framework;

public class FactionsManagerTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void FactionsManagerTestsSimplePasses()
    {
        var manager = new ServerFactionsManager();

        manager.Initialize();

        Assert.AreEqual(manager.CountOfFactions, 3);
    }
}