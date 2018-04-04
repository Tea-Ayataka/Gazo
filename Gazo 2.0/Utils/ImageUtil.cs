using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gazo.Utils {
    class ImageUtil {
        public static byte[] ConvertToPng(Bitmap input) {
            using (var memoryStream = new System.IO.MemoryStream()) {
                input.Save(memoryStream, ImageFormat.Png);
                return memoryStream.GetBuffer();
            }
        }
    }
}
