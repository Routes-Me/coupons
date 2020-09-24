using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace CouponService.Helper
{
    public class Common
    {

        public static Byte[] BitmapToBytesCode(Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                return stream.ToArray();
            }
        }

        public static JArray SerializeJsonForIncludedRepo(List<dynamic> objList)
        {
            var modelsJson = JsonConvert.SerializeObject(objList,
                                 new JsonSerializerSettings
                                 {
                                     NullValueHandling = NullValueHandling.Ignore,
                                 });

            return JArray.Parse(modelsJson);
        }
    }
}
