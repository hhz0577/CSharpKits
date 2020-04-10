using System;
using System.Collections.Generic;
using System.Text;

namespace HhzUtil.image
{
    using System.Drawing;
    using System.IO;
    using FreeImageAPI;
    using FreeImageAPI.Metadata;
    using FreeImageAPI.Plugins;

    public class ImageP
    {
        /// <summary>
        /// 验证码
        /// </summary>
        /// <param name="yzm">验证码值</param>
        /// <param name="width">图片宽度</param>
        /// <param name="height">图片高度</param>
        /// <returns></returns>
        public byte[] GetYzm(string yzm, int width, int height)
        {
            if (width <= 0) width = 80;
            if (height <= 0) height = 20;
            Bitmap image = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(image);
            MemoryStream ms = new MemoryStream();
            try
            {
                g.Clear(Color.White);
                Random random = new Random();
                //画图片的背景噪声线
                for (int i = 0; i < 25; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.LightGray), x1, y1, x2, y2);
                }
                Font font = new Font("Arial", 12, (FontStyle.Bold | FontStyle.Italic));
                System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.Gray, 1.2F, true);
                g.DrawString(yzm, font, brush, 2, 2);
                //画图片的前景噪声线
                for (int i = 0; i < 100; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
            finally
            {
                g.Dispose();
                image.Dispose();
                ms.Dispose();
            }
        }
        /// <summary>
        /// 将图片保存成指定的格式图片
        /// </summary>
        /// <param name="sourceFile">原始图片</param>
        /// <param name="distationFile">转换后的图片</param>
        /// <param name="format">转换的格式信息</param>
        /// <returns></returns>
        public bool ConvertImageStyle(string sourceFile, string distationFile, string format)
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(sourceFile);
            bool rtn = true;
            switch (format.ToLower())
            {
                case "bmp":
                    bitmap.Save(distationFile, System.Drawing.Imaging.ImageFormat.Bmp);
                    break;
                case "emf":
                    bitmap.Save(distationFile, System.Drawing.Imaging.ImageFormat.Emf);
                    break;
                case "gif":
                    bitmap.Save(distationFile, System.Drawing.Imaging.ImageFormat.Gif);
                    break;
                case "ico":
                    bitmap.Save(distationFile, System.Drawing.Imaging.ImageFormat.Icon);
                    break;
                case "jpg":
                    bitmap.Save(distationFile, System.Drawing.Imaging.ImageFormat.Jpeg);
                    break;
                case "png":
                    bitmap.Save(distationFile, System.Drawing.Imaging.ImageFormat.Png);
                    break;
                case "tif":
                    bitmap.Save(distationFile, System.Drawing.Imaging.ImageFormat.Tiff);
                    break;
                case "wmf":
                    bitmap.Save(distationFile, System.Drawing.Imaging.ImageFormat.Wmf);
                    break;
                default:
                    rtn = false;
                    break;
            }
            return rtn;
        }
        public bool ConvertImageStyle(Bitmap bitmap, string distationFile, string format)
        {
            bool rtn = true;
            switch (format.ToLower())
            {
                case "bmp":
                    bitmap.Save(distationFile, System.Drawing.Imaging.ImageFormat.Bmp);
                    break;
                case "emf":
                    bitmap.Save(distationFile, System.Drawing.Imaging.ImageFormat.Emf);
                    break;
                case "gif":
                    bitmap.Save(distationFile, System.Drawing.Imaging.ImageFormat.Gif);
                    break;
                case "ico":
                    bitmap.Save(distationFile, System.Drawing.Imaging.ImageFormat.Icon);
                    break;
                case "jpg":
                    bitmap.Save(distationFile, System.Drawing.Imaging.ImageFormat.Jpeg);
                    break;
                case "png":
                    bitmap.Save(distationFile, System.Drawing.Imaging.ImageFormat.Png);
                    break;
                case "tif":
                    bitmap.Save(distationFile, System.Drawing.Imaging.ImageFormat.Tiff);
                    break;
                case "wmf":
                    bitmap.Save(distationFile, System.Drawing.Imaging.ImageFormat.Wmf);
                    break;
                default:
                    rtn = false;
                    break;
            }
            return rtn;
        }
    }
}
