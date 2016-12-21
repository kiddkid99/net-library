using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace kidd.Common.Utility
{
    /// <summary>
    /// 圖片處理工具類別，提供處理圖片的靜態方法
    /// </summary>
    public class ImageUtility
    {
        /// <summary>
        /// 將圖片轉成位元組
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static byte[] ConvertToByte(Bitmap source)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(source, typeof(byte[]));

        }


        /// <summary>
        /// 指定範圍裁切圖片
        /// </summary>
        /// <param name="sourceImage">來源圖片</param>
        /// <param name="x">左上角原點X座標</param>
        /// <param name="y">左上角原點Y座標</param>
        /// <param name="width">寬度</param>
        /// <param name="height">高度</param>
        /// <returns>裁切後的新圖片，統一將圖片轉成 32 位元色階</returns>
        public static Bitmap CropImage(Bitmap sourceImage, int x, int y, int width, int height)
        {
            //建立新圖片，預設使用 32 位元色階，才可以在 graphic 上畫圖。
            Bitmap result = new Bitmap(width, height);
            
            result.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
            using (Graphics graphic = Graphics.FromImage(result))
            {
                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.DrawImage(sourceImage, new Rectangle(0, 0, width, height), new Rectangle(x, y, width, height), GraphicsUnit.Pixel);
            }

            return result;
        }


        /// <summary>
        /// 依比例轉換圖檔大小(取比例小的值)，儲存新圖片。
        /// </summary>
        /// <param name="sourceImage">圖片</param>
        /// <param name="maxWidth">最大寬度</param>
        /// <param name="maxHeight">最大高度</param>
        /// <param name="quality">壓縮品質，1 ~ 100，越高品質越好，壓縮率越低。</param>
        /// <param name="filePath">儲存檔案路徑</param>    
        /// <param name="force">強制轉換</param>
        public static void RatioResizeAndSave(Bitmap sourceImage, int maxWidth, int maxHeight, int quality, string filePath, bool force)
        {
            // 取得圖片原始寬高。
            int originalWidth = sourceImage.Width;
            int originalHeight = sourceImage.Height;

            // 取得適當的比例。
            float ratioX = (float)maxWidth / (float)originalWidth;
            float ratioY = (float)maxHeight / (float)originalHeight;
            float ratio = Math.Min(ratioX, ratioY);

            int newWidth = originalWidth;
            int newHeight = originalHeight;

            //轉換的判斷。
            //超過最大寬高，就會設定新的寬高。
            if (force || (originalWidth > maxWidth || originalHeight > maxHeight))
            {
                // 依新的比例取得新的高度、寬度。
                newWidth = (int)(originalWidth * ratio);
                newHeight = (int)(originalHeight * ratio);
            }

            ResizeAndSave(sourceImage, newWidth, newHeight, quality, filePath);
        }

        /// <summary>
        /// 指定新的高度及寬度，並儲存新圖片
        /// </summary>
        /// <param name="sourceImage"></param>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <param name="quality"></param>
        /// <param name="filePath"></param>
        public static void ResizeAndSave(Bitmap sourceImage, int newWidth, int newHeight, int quality, string filePath)
        {
            // Convert other formats (including CMYK) to RGB.
            Bitmap newImage = Resize(sourceImage, newWidth, newHeight);
           
            SaveImage(newImage, filePath, quality);
        }

        /// <summary>
        /// 縮放圖片，依設定等比例寬高縮放
        /// </summary>
        /// <param name="sourceImage">圖片</param>
        /// <param name="width">寬度</param>
        /// <param name="height">高度</param>
        /// <param name="force">強制轉換</param>
        /// <param name="mode">縮放比例模式</param>
        /// <returns>調整後的新圖片</returns>
        public static Bitmap RatioResize(Bitmap sourceImage, int width, int height, bool force, ImageRezieRatioMode mode)
        {
            // Get the sourceImage's original width and height
            int originalWidth = sourceImage.Width;
            int originalHeight = sourceImage.Height;

            // 取得適當的比例。
            float ratioX = (float)width / (float)originalWidth;
            float ratioY = (float)height / (float)originalHeight;

            //依模式取得縮放比例。
            float ratio = mode == ImageRezieRatioMode.取小值 ? Math.Min(ratioX, ratioY) : Math.Max(ratioX, ratioY);

            int newWidth = originalWidth;
            int newHeight = originalHeight;

            if (force || (originalWidth > width || originalHeight > height))
            {
                // 依新的比例取得新的高度、寬度。
                newWidth = (int)(originalWidth * ratio);
                newHeight = (int)(originalHeight * ratio);
            }

            // Convert other formats (including CMYK) to RGB.
            Bitmap newImage = (Bitmap)Resize(sourceImage, newWidth, newHeight);
            return newImage;
        }



        /// <summary>
        /// 指定新的高度及寬度，產生新圖片，不考慮圖片比例失真問題。
        /// </summary>
        /// <param name="sourceImage">圖片</param>
        /// <param name="newWidth">寬度</param>
        /// <param name="newHeight">高度</param>
        /// <returns>調整後的新圖片</returns>
        public static Bitmap Resize(Bitmap sourceImage, int newWidth, int newHeight)
        {
            //建立新圖片，預設使用 32 位元色階，才可以在 graphic 上畫圖。
            Bitmap newImage = new Bitmap(newWidth, newHeight);

            // Draws the sourceImage in the specified size with quality mode set to HighQuality
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(sourceImage, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }

        /// <summary>
        /// 儲存圖片
        /// </summary>
        /// <param name="sourceImage">圖片</param>
        /// <param name="filePath">檔案絕對路徑</param>
        /// <param name="quality">壓縮品質，1 ~ 100，越高品質越好，壓縮率越低。</param>
        public static void SaveImage(Bitmap image, string filePath, int quality)
        {
            //TODO:儲存圖片操作，應該移至到負責處理檔案的類別
            //依副檔名取得 ImageFormat
            ImageFormat imageFormat = GetImageFormat(Path.GetExtension(filePath));

            // Get an ImageCodecInfo object that represents the JPEG codec.
            ImageCodecInfo imageCodecInfo = GetEncoderInfo(imageFormat);
            
            // Create an Encoder object for the Quality parameter.
            Encoder encoder = Encoder.Quality;
            
            // Create an EncoderParameters object. 
            EncoderParameters encoderParameters = new EncoderParameters(1);

            // Save the sourceImage as a JPEG file with quality level.
            EncoderParameter encoderParameter = new EncoderParameter(encoder, quality);
            encoderParameters.Param[0] = encoderParameter;

            image.Save(filePath, imageCodecInfo, encoderParameters);
        }

        /// <summary>
        /// 依副檔名取得 ImageFormat 
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        private static ImageFormat GetImageFormat(string extension)
        {
            ImageFormat result = null;
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    result = ImageFormat.Jpeg;
                    break;
                case ".png":
                    result = ImageFormat.Png;
                    break;
                case ".gif":
                    result = ImageFormat.Gif;
                    break;
                case ".bmp":
                    result = ImageFormat.Bmp;
                    break;
                default:
                    result = ImageFormat.Jpeg;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Method to get encoder infor for given sourceImage format.
        /// </summary>
        /// <param name="format">Image format</param>
        /// <returns>sourceImage codec info.</returns>
        private static ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            var imageEncoders = ImageCodecInfo.GetImageEncoders();
            
            return imageEncoders.SingleOrDefault(c => c.FormatID.ToString() == format.Guid.ToString());
        }
    }

    public enum ImageRezieRatioMode
    {
        取小值 = 0,
        取大值 = 1
    }
}
