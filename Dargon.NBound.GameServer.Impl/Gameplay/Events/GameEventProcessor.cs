namespace Dargon.NBound.GameServer.Gameplay.Events {
   public interface GameEventProcessor {
      void ProcessEvent(GameState state, object e);
   }

   public abstract class GameEventProcessor<TEvent> : GameEventProcessor {
      public void ProcessEvent(GameState state, object e) {
         ProcessEvent(state, (TEvent)e);
      }

      public abstract void ProcessEvent(GameState state, TEvent e);
   }

   public class SpawnEventProcessor : GameEventProcessor<SpawnEvent> {
      public override void ProcessEvent(GameState state, SpawnEvent e) {
         // Note: IsStartOfGame is for client only.
         var mobile = state.GetMobileById(e.MobileId);
         mobile.X = e.X;
         mobile.Y = e.Y;
      }
   }
}
