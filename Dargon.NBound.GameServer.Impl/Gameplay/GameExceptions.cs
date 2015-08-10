using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dargon.NBound.GameServer.Maps;

namespace Dargon.NBound.GameServer.Gameplay {
   public static class GameExceptions {
      public static InvalidOperationException BadSpawn(ManagableMap map, int spawnX) {
         return new InvalidOperationException(
            $"Failed to spawn at x={spawnX} in map."
         );
      }
   }
}
