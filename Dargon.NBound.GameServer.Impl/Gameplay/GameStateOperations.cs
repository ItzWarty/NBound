using Dargon.NBound.GameServer.Gameplay.Events;
using Dargon.NBound.GameServer.Mobiles;
using ItzWarty;
using System.Linq;

namespace Dargon.NBound.GameServer.Gameplay {
   public static class GameStateOperations {
      private static GameStateOperationsInterface instance = new GameStateOperationsImpl();

      public static void SpawnMobiles(this GameState gameState) {
         instance.SpawnMobiles(gameState);
      }

      public static MobileInstance PickNextMobile(this GameState gameState) {
         return instance.PickNextMobile(gameState);
      }

      public static MobileInstance GetMobileById(this GameState gameState, int mobileId) {
         return instance.GetMobileById(gameState, mobileId);
      }
   }

   public interface GameStateOperationsInterface {
      void SpawnMobiles(GameState gameState);
      MobileInstance PickNextMobile(GameState gameState);
      MobileInstance GetMobileById(GameState gameState, int mobileId);
   }

   public class GameStateOperationsImpl : GameStateOperationsInterface {
      public void SpawnMobiles(GameState state) {
         var mobiles = state.Mobiles;
         var spawnXs = state.Map.InitialSpawnXs.Shuffle().Take(mobiles.Count).ToArray();
         for (var i = 0; i < mobiles.Count; i++) {
            var mobile = mobiles[i];
            var spawnX = spawnXs[i];
            int spawnY;
            if (!state.Map.TryFindTopmostPixel(spawnX, out spawnY)) {
               throw GameExceptions.BadSpawn(state.Map, spawnX);
            }
            state.PushEvent(new SpawnEvent(mobile.Id, spawnX, spawnY, true));
         }
      }

      public MobileInstance PickNextMobile(GameState gameState) {
         return gameState.Mobiles.MinBy(mobile => mobile.RoundDelay);
      }

      public MobileInstance GetMobileById(GameState gameState, int mobileId) {
         return gameState.Mobiles.First(mobile => mobile.Id == mobileId);
      }
   }
}
