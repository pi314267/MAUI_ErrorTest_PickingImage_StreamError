
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorTest.Service.PartialMethods
{
    public partial class ImageResizeService
    {
        public partial byte[] ResizeImage(byte[] imageData, float width, float height, int quality, bool blRatioMaintain);
    }
}
