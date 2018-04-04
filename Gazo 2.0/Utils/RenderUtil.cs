using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gazo.Utils {
    class RenderUtil {
        // セレクターの描写用
        internal static Rectangle GetFixedArea(int x, int y, int x2, int y2) {
            if (x > x2) {
                x2 = x ^ (x2 ^= x ^= x2 ^= x);
            }

            if (y > y2) {
                y2 = y ^ (y2 ^= y ^= y2 ^= y);
            }

            return new Rectangle(x, y, x2 - x, y2 - y);
        }

        // 全モニターの合計サイズを算出する
        internal static Rectangle GetFullRegion() {
            var rect = new Rectangle();
            foreach (Screen screen in Screen.AllScreens) {
                rect = Rectangle.Union(rect, screen.Bounds);
            }
            return rect;
        }

        [DllImport("dwmapi.dll")]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref int[] pMargins);
    }
}
