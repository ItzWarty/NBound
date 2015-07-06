using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dargon.NBound {
   public class Program {
      public static void Main() {
         var mapStencil = new MapStencilFactory().FromImage("maps/map02.bmp");
         // x goes right, then left.
         //         double windMagnitude = 10;
         //         double windAngle = Math.PI;// * 5 / 8;
         //         double x0 = 150;
         //         double y0 = 150;
         //         double vx0 = 30;
         //         double vy0 = -65;
         //         PunchParabola(windAngle, windMagnitude, vx0, x0, vy0, y0, mapStencil);

         double windMagnitude = 11;
         double windAngle = Math.PI * 5 / 8;
         double x0 = 260;
         double y0 = 150;
         double mag = 71;
         for (var theta = 60; theta < 120; theta++) {
            double angle = theta * Math.PI / 180.0;
            double vx0 = mag * Math.Cos(angle);
            double vy0 = -mag * Math.Sin(angle);
            PunchParabola(windAngle, windMagnitude, vx0, x0, vy0, y0, mapStencil);
         }
         //         mapStencil.PunchEllipse(
         //            200, 150, 200, 100, x => !x
         //            );
         Application.Run(new MapStencilDisplay(mapStencil));
      }

      private static void PunchParabola(double windAngle, double windMagnitude, double vx0, double x0, double vy0, double y0, MapStencil mapStencil) {
         double windVx = Math.Cos(windAngle) * windMagnitude;
         double windVy = -Math.Sin(windAngle) * windMagnitude;
         double g = 25;
         for (var t = 0.0; t < 10; t += 0.01) {
            //            var x = 150 + 20 * t;
            //            var y = 20 * t * t - t * 100 + 150;
            var x = 0.5 * windVx * t * t + vx0 * t + x0;
            var y = 0.5 * (windVy + g) * t * t + vy0 * t + y0;
            mapStencil.PunchEllipse((int)x, (int)y, 2, 2, val => !val);
            if (y > 150 && t > 2) {
               break;
            }
         }
      }
   }
}
// discrete
//double x = 150;
//double y = 150;
//double vx = 20;
//double vy = -30;
//double g = 9.8;
//double dt = 0.01;
//         for (var t = 0.0; t< 10; t += dt) {
//            vy += g* dt;
//x += vx* dt;
//y += vy* dt;
//mapStencil.PunchEllipse((int)x, (int)y, 5, 5, val => !val);
//         }