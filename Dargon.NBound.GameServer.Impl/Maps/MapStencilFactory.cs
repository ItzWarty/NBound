using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using ItzWarty;

namespace Dargon.NBound.GameServer.Maps {
   public class MapStencilFactory {
      public unsafe MapStencil FromImage(string path) {
         var sourceBitmap = (Bitmap)Image.FromFile(path);
         var rect = new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height);
         var sourceData = sourceBitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
         bool[][] destLines = Util.Generate(rect.Height, y => new bool[rect.Width]);
         for (var y = 0; y < rect.Height; y++) {
            byte* sourceLine = (byte*)sourceData.Scan0.ToPointer() + y * sourceData.Stride;
            byte* sourcePixel = sourceLine;
            fixed (bool* destLine = destLines[y]) {
               var destPixel = destLine;
               for (var x = 0; x < sourceData.Width; x++) {
                  *destPixel = *sourcePixel > 127;
                  sourcePixel++;
                  destPixel++;
               }
            }
         }
         sourceBitmap.UnlockBits(sourceData);
         return From2D(destLines);
      }

      public unsafe MapStencil From2D(bool[][] image) {
         var imageLines = image;
         var mapWidth = imageLines[0].Length;
         Trace.Assert(imageLines.All(line => line.Length == mapWidth));
         var mapHeight = imageLines.Length;
         var bufferWidth = (mapWidth / 8) + ((mapWidth % 8) == 0 ? 0 : 1);
         var buffer = new byte[bufferWidth * mapHeight];
         fixed (byte* pDestBuffer = buffer) {
            for (var y = 0; y < mapHeight; y++) {
               byte* pDestPixel = pDestBuffer + bufferWidth * y;
               fixed (bool* pSourceLine = imageLines[y]) {
                  byte* pSourcePixel = (byte*)pSourceLine;
                  for (var x = 0; x < mapWidth;) {
                     for (var bit = 0; bit < 8 && x < mapWidth; bit++, x++) {
                        *pDestPixel = (byte)(*pDestPixel | ((*pSourcePixel) << bit));
                        pSourcePixel++;
                     }
                     pDestPixel++;
                  }
               }
            }
         }
         return new MapStencil(buffer, bufferWidth, mapWidth, mapHeight);
      }
   }
}