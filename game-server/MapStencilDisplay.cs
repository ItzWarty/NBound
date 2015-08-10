using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Dargon.NBound.GameServer.Maps;

namespace Dargon.NBound {
   public partial class MapStencilDisplay : Form {
      public MapStencilDisplay(MapStencil mapStencil) : this() {
         ShowStencil(mapStencil);
      }

      public MapStencilDisplay() {
         InitializeComponent();

         imageTabs.Controls.Clear();
      }

      private void ShowStencil(MapStencil mapStencil) {
         var tabPage = new TabPage("stencil");
         var pictureBox = new PictureBox();
         pictureBox.Image = StencilToBitmap(mapStencil);
         pictureBox.Dock = DockStyle.Fill;
         pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
         tabPage.Controls.Add(pictureBox);
         imageTabs.Controls.Add(tabPage);
      }

      private unsafe Bitmap StencilToBitmap(MapStencil stencil) {
         var image = new Bitmap(stencil.MapWidth, stencil.MapHeight, PixelFormat.Format32bppRgb);
         var rect = new Rectangle(0, 0, stencil.MapWidth, stencil.MapHeight);
         var data = image.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
         fixed (byte* stencilData = stencil.Buffer) {
            for (var y = 0; y < stencil.MapHeight; y++) {
               var stencilLine = stencilData + y * stencil.BufferWidth;
               var destinationPixel = (uint*)((byte*)data.Scan0.ToPointer() + y * data.Stride);
               var stencilOctet = stencilLine;
               for (var x = 0; x < stencil.MapWidth; ) {
                  for (var bit = 0; bit < 8 && x < stencil.MapWidth; bit++, x++) {
                     *destinationPixel = (*stencilOctet & (1 << bit)) > 0 ? 0xFFFFFFFFU : 0;
                     destinationPixel++;
                  }
                  stencilOctet++;
               }
            }
         }
         image.UnlockBits(data);
         return image;
      }
   }
}
