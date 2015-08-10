namespace Dargon.NBound.GameServer.Gameplay {
   public class GamePhaseManager {
      private GamePhase currentPhase;

      public void Transition(GamePhase phase) {
         currentPhase = phase;
         phase.HandleEnter();
      }
   }
}
