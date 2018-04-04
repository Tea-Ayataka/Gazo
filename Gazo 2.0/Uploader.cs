using Gazo.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Gazo {
    class Uploader {
        private const string CLIENT_ID = "4b696ba615fcf6a";

        internal static string Upload(Bitmap img) {
            var content = ImageUtil.ConvertToPng(img);

            using (var client = new WebClientEx()) {
                client.Headers["Authorization"] = "Client-ID " + CLIENT_ID;

                var values = new NameValueCollection {
                    {"image", Convert.ToBase64String(content)}
                };

                var response = Encoding.UTF8.GetString(client.UploadValues("https://api.imgur.com/3/upload", values));

                var model = new JavaScriptSerializer().Deserialize<dynamic>(response);
                var imagelink = model["data"]["link"];

                return imagelink;
            }
        }
    }
}
