namespace Dargon.NBound.GameServer.Maps {
   public class MapStencil {
      public MapStencil(byte[] buffer, int bufferWidth, int mapWidth, int mapHeight) {
         Buffer = buffer;
         BufferWidth = bufferWidth;
         MapWidth = mapWidth;
         MapHeight = mapHeight;
      }

      public byte[] Buffer { get; set; }
      public int BufferWidth { get; set; }
      public int MapWidth { get; set; }
      public int MapHeight { get; set; }
   }
}
