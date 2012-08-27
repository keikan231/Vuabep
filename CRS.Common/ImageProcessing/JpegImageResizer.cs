using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace CRS.Common.ImageProcessing
{
    public class JpegImageResizer : IImageResizer
    {
        public Image Source { get; private set; }
        public long Quality { get; set; }
        public Image Target { get; set; }

        public JpegImageResizer(Image source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Source = source;
            Target = source;
            Quality = 65L;
        }

        public JpegImageResizer(string fileName) : this(Image.FromFile(fileName))
        {
        }

        public void Resize(int width, int height)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width");
            if (height <= 0)
                throw new ArgumentOutOfRangeException("height");

            Image canvas = new Bitmap(width, height);
            Graphics graphic = Graphics.FromImage(canvas);

            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;

            graphic.DrawImage(Source, 0, 0, width, height);

            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, Quality);

            Stream tempSavingStream = new MemoryStream();
            canvas.Save(tempSavingStream, jgpEncoder, encoderParameters);

            // Create a new Image object from temp stream
            Target = Image.FromStream(tempSavingStream);
        }

        public void ScaleByPercentage(float wPercent, float hPercent)
        {
            if (hPercent <= 0)
                throw new ArgumentOutOfRangeException("hPercent");
            if (wPercent <= 0)
                throw new ArgumentOutOfRangeException("wPercent");

            int sourceWidth = Source.Width;
            int sourceHeight = Source.Height;

            int destWidth = (int)(sourceWidth * wPercent / 100);
            int destHeight = (int)(sourceHeight * hPercent / 100);

            Resize(destWidth, destHeight);
        }

        public void Crop(int width, int height, CropSide cropSide)
        {
            throw new NotImplementedException();
        }

        public void ScaleToFit(int maxWidth, int maxHeight)
        {
            if (maxWidth <= 0)
                throw new ArgumentOutOfRangeException("maxWidth");
            if (maxHeight <= 0)
                throw new ArgumentOutOfRangeException("maxHeight");

            int width = Source.Width;
            int height = Source.Height;
            if (width > maxWidth || height > maxHeight)
            {
                float wp = (float) maxWidth/width * 100;
                float hp = (float) maxHeight/height * 100;
                float min = Math.Min(wp, hp);
                ScaleByPercentage(min, min);
            }
            else
            {
                // Still call resize with the same width and height to perform compression
                Resize(width, height);
            }
        }

        public void SaveToFile(string fileName)
        {
            // Create directory first if not exist
            FileInfo f = new FileInfo(fileName);
            f.Directory.Create();
            Target.Save(fileName);
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            return codecs.FirstOrDefault(codec => codec.FormatID == format.Guid);
        }
    }
}