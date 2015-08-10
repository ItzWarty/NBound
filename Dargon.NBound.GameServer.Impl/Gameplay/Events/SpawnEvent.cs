using Dargon.PortableObjects;

namespace Dargon.NBound.GameServer.Gameplay.Events {
   public class SpawnEvent : GameEvent {
      public SpawnEvent() { }

      public SpawnEvent(int mobileId, int x, int y, bool isStartOfGame) {
         this.MobileId = mobileId;
         this.X = x;
         this.Y = y;
         this.IsStartOfGame = isStartOfGame;
      }

      public int MobileId { get; set; }
      public int X { get; set; }
      public int Y { get; set; }
      public bool IsStartOfGame { get; set; }

      public void Serialize(IPofWriter writer) {
         writer.WriteS32(0, MobileId);
         writer.WriteS32(1, X);
         writer.WriteS32(2, Y);
         writer.WriteBoolean(3, IsStartOfGame);
      }

      public void Deserialize(IPofReader reader) {
         MobileId = reader.ReadS32(0);
         X = reader.ReadS32(1);
         Y = reader.ReadS32(2);
         IsStartOfGame = reader.ReadBoolean(3);
      }
   }
}
