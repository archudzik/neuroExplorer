using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elliatab.Leap.Wpf.Tests
{
    [TestClass]
    public class FrameUnitTest
    {
        [TestMethod]
        public void ZeroHandDeserializationTest()
        {
            string jsonInput = "{\"hands\":[],\"id\":498458,\"pointables\":[],\"r\":[[-0.74774,0.377025,0.546569],[0.348123,-0.47835,0.806221],[0.565416,0.793117,0.22643]],\"s\":11.1697,\"t\":[-957.161,-797.109,1480.63],\"timestamp\":4331549902}";
            
            var frame = Frame.DeserializeFromJson(jsonInput);

            Assert.AreEqual(frame.Fingers.Count, 0);
            Assert.AreEqual(frame.Hands.Count, 0);
            Assert.AreEqual(frame.Id, 498458);
            Assert.AreEqual(frame.Pointables.Count, 0);
            Assert.AreEqual(frame.Timestamp, 4331549902);
            Assert.AreEqual(frame.Tools.Count,0);
        }
    }
}
