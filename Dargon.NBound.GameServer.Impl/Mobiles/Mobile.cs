using System.Collections.Generic;

namespace Dargon.NBound.GameServer.Mobiles {
   public class MobilePrototype {
      public double MovementSpeed { get; private set; }
      public double MovementStamina { get; private set; }
      public double SteepnessTolerance { get; private set; }
      public double MobilityWidth { get; private set; }
      public double CannonAngleLow { get; private set; }
      public double CannonAngleHigh { get; private set; }
      public double TrackerExtents { get; private set; }
   }

   public class MobileInstance {
      public MobileInstance(MobilePrototype prototype) {
         this.Prototype = prototype;
      }

      public int Id { get; set; }

      public MobilePrototype Prototype { get; private set; }
      public double X { get; set; }
      public double Y { get; set; }

      public int DeathCounter { get; set; }
      public int DeathRespawnX { get; set; }

      public int RoundDelay { get; set; }

      //    Below fields should be obtainable via tracker position
//      public double Orientation { get; set; }
//      public double OrientationTrackerLeftX { get; set; }
//      public double OrientationTrackerLeftY { get; set; }
//      public double OrientationTrackerRightX { get; set; }
//      public double OrientationTrackerRightY { get; set; }
   }
}
