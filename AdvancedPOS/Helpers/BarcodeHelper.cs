using System.Drawing;
using ZXing;
using ZXing.Common;

namespace AdvancedPOS.Helpers
{
    public static class BarcodeHelper
    {
        // Barcode string එකකින් Image එකක් හදනවා (Code 128 format)
        public static Bitmap GenerateBarcode(string content, int width = 300, int height = 100)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Width = width,
                    Height = height,
                    Margin = 5
                }
            };

            return writer.Write(content);
        }
    }
}
