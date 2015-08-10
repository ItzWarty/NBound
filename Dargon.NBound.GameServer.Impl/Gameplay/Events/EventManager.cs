using System;
using System.Collections.Generic;

namespace Dargon.NBound.GameServer.Gameplay.Events {
   public interface EventManager {
      void PushEvent(GameEvent gameEvent);
   }

   public class EventManagerImpl : EventManager {
      private readonly IReadOnlyDictionary<Type, GameEventProcessor> eventProcessorsByEventType; 
      private GameState state;

      public EventManagerImpl(IReadOnlyDictionary<Type, GameEventProcessor> eventProcessorsByEventType) {
         this.eventProcessorsByEventType = eventProcessorsByEventType;
      }

      public void SetGameState(GameState state) {
         this.state = state;
      }

      public void PushEvent(GameEvent e) {
         eventProcessorsByEventType[e.GetType()].ProcessEvent(state, e);
      }
   }
}
