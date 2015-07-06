using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using ItzWarty;

namespace Dargon.NBound {
   public class MapStencil {
      public MapStencil(byte[] buffer, int bufferWidth, int mapWidth, int mapHeight) {
         Buffer = buffer;
         BufferWidth = bufferWidth;
         MapWidth = mapWidth;
         MapHeight = mapHeight;
      }

      public byte[] Buffer { get; set; }
      public int BufferWidth { get; set; }
      public int MapWidth { get; set; }
      public int MapHeight { get; set; }

      public void PunchEllipse(int cx, int cy, int width, int height, Func<bool, bool> punchOperation) {
         int xLow = cx - width / 2;
         int xHigh = cx + width / 2 + (width % 2 == 0 ? 0 : 1);
         int yLow = Math.Max(0, cy - height / 2);
         int yHigh = Math.Min(MapHeight, cy + height / 2 + (height % 2 == 0 ? 0 : 1));

         // x^2/a^2 + y^2/b^2 = 1, when y=0, xmax and when x=0, ymax
         // x^2/a^2 == 1 for y == 0, x = xmax | xmin, similar for y
         double a = (double)width / 2.0;
         double b = (double)height / 2.0;
         for (var y = yLow; y < yHigh; y++) {
            var yRel = y - cy;

            // solve x^2/a^2 + y^2/b^2 = 1 => x^2/a^2 = 1 - y^2/b^2 => x = +-sqrt((a^2)*(1 - y^2/b^2))
            double xRelMag = Math.Sqrt(Math.Pow(a, 2) * (1 - Math.Pow(yRel, 2) / Math.Pow(b, 2)));
            double xMin = Math.Max(0, cx - xRelMag);
            double xMax = Math.Min(MapWidth - 1, cx + xRelMag);

            PunchLine((int)Math.Floor(xMin), y, (int)Math.Ceiling(xMax - xMin + 0.5), punchOperation);
         }
      }

      private unsafe void PunchLine(int left, int y, int width, Func<bool, bool> punchOperation) {
         for (var x = left; x < left + width; x++) {
            PunchPixel(x, y, punchOperation);
         }
      }

      private unsafe void PunchPixel(int left, int top, Func<bool, bool> punchOperation) {
         fixed (byte* pBuffer = Buffer) {
            byte* pLine = pBuffer + top * BufferWidth;
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
   }

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
