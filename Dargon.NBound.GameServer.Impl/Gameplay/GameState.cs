using System;
using Dargon.NBound.GameServer.Gameplay.Events;
using Dargon.NBound.GameServer.Maps;
using Dargon.NBound.GameServer.Mobiles;
using ItzWarty.Collections;
using SCG = System.Collections.Generic;

namespace Dargon.NBound.GameServer.Gameplay {
   public class GameState {
      public GameState(ManagableMap map, SCG.IReadOnlyList<MobileInstance> mobiles, EventManager eventManager) {
         Map = map;
         Mobiles = mobiles;
         EventManager = eventManager;
      }

      public ManagableMap Map { get; private set; }
      public SCG.IReadOnlyList<MobileInstance> Mobiles { get; private set; }
      public EventManager EventManager { get; private set; }

      public void PushEvent(SpawnEvent spawnEvent) {
         EventManager.PushEvent(spawnEvent);
      }
   }
}
