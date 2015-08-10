using System;
using ItzWarty.Collections;

namespace Dargon.NBound.GameServer.Maps {
   public interface ManagableMap {
      MapStencil Stencil { get; }
      IReadOnlySet<int> InitialSpawnXs { get; }
      void PunchEllipse(int cx, int cy, int width, int height, Func<bool, bool> punchOperation);
      bool IsPixelSet(int left, int top);
      bool TryFindTopmostPixel(int x, out int y);
   }
}
