using System;

namespace Dargon.NBound.GameServer.Maps {
   public static class MapStencilOperations {
      private static readonly MapStencilOperationsInterface instance = new MapStencilOperationsImpl();

      public static void PunchEllipse(this MapStencil mapStencil, int cx, int cy, int width, int height, Func<bool, bool> punchOperation) {
         instance.PunchEllipse(mapStencil, cx, cy, width, height, punchOperation);
      }

      public static bool IsPixelSet(this MapStencil mapStencil, int x, int y) {
         return instance.IsPixelSet(mapStencil, x, y);
      }

      public static MapStencil Clone(this MapStencil mapStencil) {
         return instance.Clone(mapStencil);
      }

      public static bool TryFindTopmostPixel(this MapStencil mapStencil, int x, out int y) {
         return instance.TryFindTopmostPixel(mapStencil, x, out y);
      }
   }

   public interface MapStencilOperationsInterface {
      void PunchEllipse(MapStencil mapStencil, int cx, int cy, int width, int height, Func<bool, bool> punchOperation);
      bool IsPixelSet(MapStencil mapStencil, int left, int top);
      MapStencil Clone(MapStencil mapStencil);
      bool TryFindTopmostPixel(MapStencil mapStencil, int x, out int y);
   }

   public class MapStencilOperationsImpl : MapStencilOperationsInterface {
      public void PunchEllipse(MapStencil mapStencil, int cx, int cy, int width, int height, Func<bool, bool> punchOperation) {
         int xLow = cx - width / 2;
         int xHigh = cx + width / 2 + (width % 2 == 0 ? 0 : 1);
         int yLow = Math.Max(0, cy - height / 2);
         int yHigh = Math.Min(mapStencil.MapHeight, cy + height / 2 + (height % 2 == 0 ? 0 : 1));

         // x^2/a^2 + y^2/b^2 = 1, when y=0, xmax and when x=0, ymax
         // x^2/a^2 == 1 for y == 0, x = xmax | xmin, similar for y
         double a = (double)width / 2.0;
         double b = (double)height / 2.0;
         for (var y = yLow; y < yHigh; y++) {
            var yRel = y - cy;

            // solve x^2/a^2 + y^2/b^2 = 1 => x^2/a^2 = 1 - y^2/b^2 => x = +-sqrt((a^2)*(1 - y^2/b^2))
            double xRelMag = Math.Sqrt(Math.Pow(a, 2) * (1 - Math.Pow(yRel, 2) / Math.Pow(b, 2)));
            double xMin = Math.Max(0, cx - xRelMag);
            double xMax = Math.Min(mapStencil.MapWidth - 1, cx + xRelMag);

            PunchLine(mapStencil, (int)Math.Floor(xMin), y, (int)Math.Ceiling(xMax - xMin + 0.5), punchOperation);
         }
      }

      private unsafe void PunchLine(MapStencil mapStencil, int left, int y, int width, Func<bool, bool> punchOperation) {
         for (var x = left; x < left + width; x++) {
            PunchPixel(mapStencil, x, y, punchOperation);
         }
      }

      private unsafe void PunchPixel(MapStencil mapStencil, int left, int top, Func<bool, bool> punchOperation) {
         fixed (byte* pBuffer = mapStencil.Buffer)
         {
            byte* pLine = pBuffer + top * mapStencil.BufferWidth;
            byte* pOctet = pLine + (left / 8);
            int bit = left % 8;
            var octet = *pOctet;
            bool bitValue = (octet & (1 << bit)) > 0;
            bool nextBitValue = punchOperation(bitValue);
            if (bitValue != nextBitValue) {
               if (!nextBitValue) {
                  octet = (byte)(octet & ~(1 << bit));
               } else {
                  octet = (byte)(octet | (1 << bit));
               }
               *pOctet = octet;
            }
         }
      }

      public unsafe bool IsPixelSet(MapStencil mapStencil, int left, int top) {
         fixed (byte* pBuffer = mapStencil.Buffer) {
            byte* pLine = pBuffer + top * mapStencil.BufferWidth;
            byte* pOctet = pLine + (left / 8);
            int bit = left % 8;
            var octet = *pOctet;
            bool bitValue = (octet & (1 << bit)) > 0;
            return bitValue;
         }
      }

      public MapStencil Clone(MapStencil mapStencil) {
         return new MapStencil(
            (byte[])mapStencil.Buffer.Clone(), 
            mapStencil.BufferWidth, 
            mapStencil.MapWidth, 
            mapStencil.MapHeight
         );
      }

      public bool TryFindTopmostPixel(MapStencil mapStencil, int x, out int resultY) {
         for (var y = 0; y < mapStencil.MapHeight; y++) {
            if (IsPixelSet(mapStencil, x, y)) {
               resultY = y;
               return true;
            }
         }
         resultY = int.MinValue;
         return false;
      }
   }
}