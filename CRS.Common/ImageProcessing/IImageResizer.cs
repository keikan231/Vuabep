using System.Drawing;

namespace CRS.Common.ImageProcessing
{
    public interface IImageResizer
    {
        /// <summary>
        /// Gets the image to be processed.
        /// </summary>
        Image Source { get; }

        /// <summary>
        /// Gets or sets the quality of resized image. Max is 100.
        /// </summary>
        long Quality { get; set; }

        /// <summary>
        /// Resizes the image to a specific resolution.
        /// </summary>
        void Resize(int width, int height);

        /// <summary>
        /// Scales the image resolution by a specific percentage.
        /// </summary>
        void ScaleByPercentage(float wPercent, float hPercent);

        /// <summary>
        /// Crops the image to a specified resolution.
        /// </summary>
        void Crop(int width, int height, CropSide cropSide);

        /// <summary>
        /// Scales the image to ensure both dimensions do not exceed the specified resolution.
        /// </summary>
        void ScaleToFit(int maxWidth, int maxHeight);

        /// <summary>
        /// Saves the image to a file.
        /// </summary>
        void SaveToFile(string filePath);
    }
}