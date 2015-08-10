using System;
using System.Collections.Generic;
using ItzWarty;
using System.Linq;
using Dargon.NBound.GameServer.Maps;
using Dargon.NBound.GameServer.Mobiles;

namespace Dargon.NBound.GameServer.Gameplay {
   public interface GamePhase {
      void HandleEnter();
   }

   public class InitializationPhase : GamePhase {
      private readonly GameState state;
      private readonly GamePhaseManager gamePhaseManager;

      public InitializationPhase(GameState state, GamePhaseManager gamePhaseManager) {
         this.state = state;
         this.gamePhaseManager = gamePhaseManager;
      }

      public void HandleEnter() {
         state.SpawnMobiles();
         gamePhaseManager.Transition(new PlayerRoundPhase());
      }
   }

   public class PlayerRoundPhase : GamePhase {
      private readonly GameState state;

      public PlayerRoundPhase(GameState state) {
         this.state = state;
      }

      public void HandleEnter() {
         var nextMobile = state.PickNextMobile();
      }
   }
}