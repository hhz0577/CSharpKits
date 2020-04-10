using System;
using System.Collections.Generic;
using System.Text;

namespace HhzUtil.image
{
    using System.Windows.Forms;
    using System.Drawing;
    using FreeImageAPI;
    using FreeImageAPI.Metadata;
    using FreeImageAPI.Plugins;

    public class ProcessImage
    {
        private FreeImageBitmap bitmap = null;
        private Bitmap resultbmp = null;

        public Bitmap Resultbmp
        {
            get { return resultbmp; }
        }

        public ProcessImage(string bmpfilename)
        {
            FreeImageEngine.Message += new OutputMessageFunction(FreeImageEngine_Message);
            try
            {
                FreeImageBitmap fib = new FreeImageBitmap(bmpfilename);
                FreeImagePlugin plug = PluginRepository.Plugin(fib.ImageFormat);
                ReplaceBitmap(fib);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private ImageMetadata mData = null;

        public ImageMetadata MData
        {
            get { return mData; }
        }
        static void FreeImageEngine_Message(FREE_IMAGE_FORMAT fif, string message)
        {
            MessageBox.Show(message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool isBitmap
        {
            //get { return ((bitmap != null) && (!bitmap.IsDisposed)); }
            get { return (bitmap != null); }
        }
        private void ReplaceBitmap(FreeImageBitmap newBitmap)
        {
            // Checks whether the bitmap is usable
            if (newBitmap == null || newBitmap.IsDisposed)
            {
                MessageBox.Show("Unexpected error.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check whether the image type of the new bitmap is 'FIT_BITMAP'.
            // If not convert to 'FIT_BITMAP'.
            if (newBitmap.ImageType != FREE_IMAGE_TYPE.FIT_BITMAP)
            {
                if (!newBitmap.ConvertType(FREE_IMAGE_TYPE.FIT_BITMAP, true))
                {
                    MessageBox.Show("Error converting bitmap to standard type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            if ((bitmap != null) && !object.ReferenceEquals(bitmap, newBitmap))
            {
                bitmap.Dispose();
            }
            if (resultbmp != null)
            {
                resultbmp.Dispose();
            }
            bitmap = newBitmap;
            resultbmp = (Bitmap)bitmap;
            UpdateBitmapInformations();
        }
        private void UpdateBitmapInformations()
        {
            if (isBitmap)
            {
                mData = bitmap.Metadata;
                mData.HideEmptyModels = false;
            }
            else
            {
                mData = null;
            }
        }
        public void SaveImage(string destfilename)
        {
            if (isBitmap)
            {
                try
                {
                    using (FreeImageBitmap temp = new FreeImageBitmap(resultbmp))
                    {
                        temp.Save(destfilename);
                    }
                }
                catch
                {
                }
            }
        }
        /// <summary>
        /// 旋转图片
        /// </summary>
        /// <param name="vRotate">旋转角度</param>
        public void RotateImage(double vRotate)
        {
            if (isBitmap)
            {
                // Create a temporary rescaled bitmap
                using (FreeImageBitmap temp = bitmap.GetScaledInstance(resultbmp.Width, resultbmp.Height, FREE_IMAGE_FILTER.FILTER_CATMULLROM))
                {
                    if (temp != null)
                    {
                        // Rotate the bitmap
                        temp.Rotate(vRotate);
                        if (resultbmp != null)
                        {
                            resultbmp.Dispose();
                        }
                        // Display the result
                        resultbmp = (Bitmap)temp;
                    }
                }
            }
        }
        /// <summary>
        /// 灰度
        /// </summary>
        public void GreyscaleImage()
        {
            if (isBitmap)
            {
                // Convert the bitmap to 8bpp and greyscale
                ReplaceBitmap(bitmap.GetColorConvertedInstance(
                    FREE_IMAGE_COLOR_DEPTH.FICD_08_BPP |
                    FREE_IMAGE_COLOR_DEPTH.FICD_FORCE_GREYSCALE));
            }
        }
        /// <summary>
        /// 调整图片伽马值
        /// </summary>
        /// <param name="vGamma">伽马值</param>
        public void AdjustGammaImage(double vGamma)
        {
            if (isBitmap)
            {
                bitmap.AdjustGamma(vGamma);
                ReplaceBitmap(bitmap);
            }
        }
        /// <summary>
        /// 获得红色频道
        /// </summary>
        public void RedChannelImage()
        {
            SetColorChannels(0xFF, 0x00, 0x00);
        }
        /// <summary>
        /// 获得绿色频道
        /// </summary>
        public void GreenChannelImage()
        {
            SetColorChannels(0x00, 0xFF, 0x00);
        }
        /// <summary>
        /// 获得蓝色频道
        /// </summary>
        public void BlueChannelImage()
        {
            SetColorChannels(0x00, 0x00, 0xFF);
        }
        public void AllChannelsImage()
        {
            if (isBitmap)
            {
                // Restore the bitmap using the original
                ReplaceBitmap(bitmap);
            }
        }
        private void SetColorChannels(int redmask, int greenmask, int bluemask)
        {
            if (isBitmap)
            {
                // Create a temporary clone.
                using (FreeImageBitmap bitmap = (FreeImageBitmap)this.bitmap.Clone())
                {
                    if (bitmap != null)
                    {
                        // Check whether the bitmap has a palette
                        if (bitmap.HasPalette)
                        {
                            // Use the Palette class to handle the bitmap's
                            // palette. A palette always consist of RGBQUADs.
                            Palette palette = bitmap.Palette;
                            // Apply the new values for all three color components.
                            for (int i = 0; i < palette.Length; i++)
                            {
                                RGBQUAD rgbq = palette[i];

                                rgbq.rgbRed = (byte)(rgbq.rgbRed & redmask);
                                rgbq.rgbGreen = (byte)(rgbq.rgbGreen & greenmask);
                                rgbq.rgbBlue = (byte)(rgbq.rgbBlue & bluemask);

                                palette[i] = rgbq;
                            }
                        }
                        // In case the bitmap has no palette it must have a color depth
                        // of 16, 24 or 32. Each color depth needs a different wrapping
                        // structure for the bitmaps data. These structures can be accessed
                        // by using the foreach clause.
                        else if (bitmap.ColorDepth == 16)
                        {
                            // Iterate over each scanline
                            // For 16bpp use either Scanline<FI16RGB555> or Scanline<FI16RGB565>
                            if (bitmap.IsRGB555)
                            {
                                foreach (Scanline<FI16RGB555> scanline in bitmap)
                                {
                                    for (int x = 0; x < scanline.Length; x++)
                                    {
                                        FI16RGB555 pixel = scanline[x];
                                        pixel.Red = (byte)(pixel.Red & redmask);
                                        pixel.Green = (byte)(pixel.Green & greenmask);
                                        pixel.Blue = (byte)(pixel.Blue & bluemask);
                                        scanline[x] = pixel;
                                    }
                                }
                            }
                            else if (bitmap.IsRGB565)
                            {
                                foreach (Scanline<FI16RGB565> scanline in bitmap)
                                {
                                    for (int x = 0; x < scanline.Length; x++)
                                    {
                                        FI16RGB565 pixel = scanline[x];
                                        pixel.Red = (byte)(pixel.Red & redmask);
                                        pixel.Green = (byte)(pixel.Green & greenmask);
                                        pixel.Blue = (byte)(pixel.Blue & bluemask);
                                        scanline[x] = pixel;
                                    }
                                }
                            }
                        }
                        else if (bitmap.ColorDepth == 24)
                        {
                            // Iterate over each scanline
                            // For 24bpp Scanline<RGBTRIPLE> must be used
                            foreach (Scanline<RGBTRIPLE> scanline in bitmap)
                            {
                                for (int x = 0; x < scanline.Length; x++)
                                {
                                    RGBTRIPLE pixel = scanline[x];
                                    pixel.rgbtRed = (byte)(pixel.rgbtRed & redmask);
                                    pixel.rgbtGreen = (byte)(pixel.rgbtGreen & greenmask);
                                    pixel.rgbtBlue = (byte)(pixel.rgbtBlue & bluemask);
                                    scanline[x] = pixel;
                                }
                            }
                        }
                        else if (bitmap.ColorDepth == 32)
                        {
                            // Iterate over each scanline
                            // For 32bpp Scanline<RGBQUAD> must be used
                            foreach (Scanline<RGBQUAD> scanline in bitmap)
                            {
                                for (int x = 0; x < scanline.Length; x++)
                                {
                                    RGBQUAD pixel = scanline[x];
                                    pixel.rgbRed = (byte)(pixel.rgbRed & redmask);
                                    pixel.rgbGreen = (byte)(pixel.rgbGreen & greenmask);
                                    pixel.rgbBlue = (byte)(pixel.rgbBlue & bluemask);
                                    scanline[x] = pixel;
                                }
                            }
                        }
                        // Dispose only the picturebox's bitmap
                        if (resultbmp != null)
                        {
                            resultbmp.Dispose();
                        }
                        resultbmp = (Bitmap)bitmap;
                    }
                }
            }
        }
    }
}
