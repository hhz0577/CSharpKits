using System;
using System.Collections.Generic;
using System.Text;

namespace HhzUtil.image
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows.Forms;
    using System.Runtime.InteropServices;

    public class Gdip
    {
        private static ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

        private static bool GetCodecClsid(string filename, out Guid clsid)
        {
            clsid = Guid.Empty;
            string ext = Path.GetExtension(filename);
            if (ext == null)
                return false;
            ext = "*" + ext.ToUpper();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FilenameExtension.IndexOf(ext) >= 0)
                {
                    clsid = codec.Clsid;
                    return true;
                }
            }
            return false;
        }


        public static bool SaveDIBAs(string picname, IntPtr bminfo, IntPtr pixdat)
        {
            SaveFileDialog sd = new SaveFileDialog();

            sd.FileName = picname;
            sd.Title = "Save bitmap as...";
            sd.Filter = "Bitmap file (*.bmp)|*.bmp|TIFF file (*.tif)|*.tif|JPEG file (*.jpg)|*.jpg|PNG file (*.png)|*.png|GIF file (*.gif)|*.gif|All files (*.*)|*.*";
            sd.FilterIndex = 1;
            if (sd.ShowDialog() != DialogResult.OK)
                return false;

            Guid clsid;
            if (!GetCodecClsid(sd.FileName, out clsid))
            {
                MessageBox.Show("不知道的图片后缀 " + Path.GetExtension(sd.FileName),
                                "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            IntPtr img = IntPtr.Zero;
            int st = GdipCreateBitmapFromGdiDib(bminfo, pixdat, ref img);
            if ((st != 0) || (img == IntPtr.Zero))
                return false;

            st = GdipSaveImageToFile(img, sd.FileName, ref clsid, IntPtr.Zero);
            GdipDisposeImage(img);
            return st == 0;
        }
        public static bool SaveDIB(string picname, IntPtr bminfo, IntPtr pixdat)
        {

            Guid clsid;
            if (!GetCodecClsid(picname, out clsid))
            {
                MessageBox.Show("不知道的图片后缀 " + Path.GetExtension(picname),
                                "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            IntPtr img = IntPtr.Zero;
            int st = GdipCreateBitmapFromGdiDib(bminfo, pixdat, ref img);
            if ((st != 0) || (img == IntPtr.Zero))
                return false;

            st = GdipSaveImageToFile(img, picname, ref clsid, IntPtr.Zero);
            GdipDisposeImage(img);
            return st == 0;
        }



        [DllImport("gdiplus.dll", ExactSpelling = true)]
        internal static extern int GdipCreateBitmapFromGdiDib(IntPtr bminfo, IntPtr pixdat, ref IntPtr image);

        [DllImport("gdiplus.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern int GdipSaveImageToFile(IntPtr image, string filename, [In] ref Guid clsid, IntPtr encparams);

        [DllImport("gdiplus.dll", ExactSpelling = true)]
        internal static extern int GdipDisposeImage(IntPtr image);

    }
}
