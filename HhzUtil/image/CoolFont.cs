using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HhzUtil.image
{
    using System.Drawing;
    public class CoolFont
    {
        Font font;
        string text = string.Empty;

        #region [ 构造函数 ]
        public CoolFont()
        {
            font = new Font("Arial", 8.25F);
        }

        public CoolFont(string fontname, Single size)
        {
            font = new Font(fontname, size);
        }

        public CoolFont(Font f)
        {
            font = new Font(f, f.Style);
        }

        public CoolFont(string fontname, Single size, FontStyle style)
        {
            font = new Font(fontname, size, style);
        }

        public CoolFont(Font f, FontStyle style)
        {
            font = new Font(f, style);
        }

        public CoolFont(Font f, Single size)
        {
            Font tf = new Font(f.Name, size);
            font = new Font(tf, f.Style);
            tf.Dispose();
        }
        #endregion

        #region [ 属性 ]
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public Font Font
        {
            get { return font; }
            set { font = value; }
        }

        public string FontName
        {
            get { return font.Name; }
            set
            {
                Font tf = new Font(value, font.Size);
                font = new Font(tf, font.Style);
                tf.Dispose();
            }
        }

        public Single FontSize
        {
            get { return font.Size; }
            set
            {
                Font tf = new Font(font.Name, value);
                font = new Font(tf, font.Style);
                tf.Dispose();
            }
        }

        public FontStyle Style
        {
            get { return font.Style; }
            set { font = new Font(font, value); }
        }

        public int Height
        {
            get { return font.Height; }
        }

        public float SizeInPoints
        {
            get { return font.SizeInPoints; }
        }
        #endregion

        #region [ 公有函数 ]
        public void FitWidth(Graphics g, float width)
        {
            this.FontSize = FitStringWidth(this.Text, this.Font, g, width);
        }

        public void FitHeight(Graphics g, float height)
        {
            this.FontSize = FitStringHeight(this.Text, this.Font, g, height);
        }

        public void FitRect(Graphics g, Rectangle rt)
        {
            this.FontSize = FitStringRect(this.Text, this.Font, g, rt);
        }

        public void DrawStringFit(Graphics g, Brush brush, Rectangle rt)
        {
            DrawStringFit(this, g, brush, rt);
        }

        public void DrawStringFit(Graphics g, Brush brush, Rectangle rt, StringAlignment alignment, StringAlignment alignmentV)
        {
            DrawStringFit(this, g, brush, rt, alignment, alignmentV);
        }
        #endregion

        #region [ 静态函数 ]
        public static SizeF GetStringSize(string s, Font f, Graphics g)
        {
            return g.MeasureString(s, f);
        }

        public static SizeF GetStringSize(CoolFont cf, Graphics g)
        {
            return g.MeasureString(cf.Text, cf.Font);
        }

        public static float FitStringWidth(string s, Font f, Graphics g, float width)
        {
            CoolFont cf = new CoolFont(f, f.Style);
            cf.Text = s;

            SizeF sf = GetStringSize(cf, g);
            cf.FontSize = Math.Abs(sf.Width - width) / cf.Text.Length;
            sf = GetStringSize(cf, g);

            if (sf.Width > width)
            {
                while (sf.Width > width)
                {
                    cf.FontSize -= 0.1F;

                    sf = GetStringSize(cf, g);
                }
            }
            else if (sf.Width < width)
            {
                while (sf.Width < width)
                {
                    cf.FontSize += 0.1F;
                    sf = GetStringSize(cf, g);
                }
                cf.FontSize -= 0.1F;
            }

            return cf.FontSize;
        }

        public static float FitStringWidth(CoolFont cf, Graphics g, float width)
        {
            return FitStringWidth(cf.Text, cf.Font, g, width);
        }

        public static float FitStringHeight(string s, Font f, Graphics g, float height)
        {
            CoolFont cf = new CoolFont(f, f.Style);
            cf.Text = s;

            SizeF sf = GetStringSize(cf, g);
            cf.FontSize = Math.Abs(sf.Height - height) / cf.Text.Length;
            sf = GetStringSize(cf, g);

            if (sf.Height > height)
            {
                while (sf.Height > height)
                {
                    cf.FontSize -= 0.1F;
                    sf = GetStringSize(cf, g);
                }
            }
            else if (sf.Height < height)
            {
                while (sf.Height < height)
                {
                    cf.FontSize += 0.1F;
                    sf = GetStringSize(cf, g);
                }
                cf.FontSize -= 0.1F;
            }
            return cf.FontSize;
        }

        public static float FitStringHeight(CoolFont cf, Graphics g, float height)
        {
            return FitStringHeight(cf.Text, cf.Font, g, height);
        }

        public static float FitStringRect(string s, Font f, Graphics g, Rectangle rt)
        {
            CoolFont cf = new CoolFont(f, f.Style);
            cf.Text = s;

            SizeF sf = GetStringSize(cf, g);
            cf.FontSize = (sf.Width - rt.Width) / cf.Text.Length;
            sf = GetStringSize(cf, g);

            if (sf.Width > rt.Width)
            {
                while (sf.Width > rt.Width)
                {
                    cf.FontSize -= 0.1F;

                    sf = GetStringSize(cf, g);
                }
            }
            else if (sf.Width < rt.Width)
            {
                while (sf.Width < rt.Width)
                {
                    cf.FontSize += 0.1F;
                    sf = GetStringSize(cf, g);
                }
                cf.FontSize -= 0.1F;
            }


            if (sf.Height > rt.Height)
            {
                while (sf.Height > rt.Height)
                {
                    cf.FontSize -= 0.1F;
                    sf = GetStringSize(cf, g);
                }
            }

            return cf.FontSize;
        }

        public static float FitStringRect(CoolFont cf, Graphics g, Rectangle rt)
        {
            return FitStringRect(cf.Text, cf.Font, g, rt);
        }

        public static void DrawStringFit(string s, Font f, Graphics g, Brush brush, Rectangle rt)
        {
            CoolFont cf = new CoolFont(f, f.Style);
            cf.Text = s;

            SizeF sf = GetStringSize(cf, g);
            cf.FontSize = Math.Abs(sf.Width - rt.Width) / cf.Text.Length;
            sf = GetStringSize(cf, g);

            if (sf.Width > rt.Width)
            {
                while (sf.Width > rt.Width)
                {
                    cf.FontSize -= 0.1F;

                    sf = GetStringSize(cf, g);
                }
            }
            else if (sf.Width < rt.Width)
            {
                while (sf.Width < rt.Width)
                {
                    cf.FontSize += 0.1F;
                    sf = GetStringSize(cf, g);
                }
                cf.FontSize -= 0.1F;
            }

            if (sf.Height > rt.Height)
            {
                while (sf.Height > rt.Height)
                {
                    cf.FontSize -= 0.1F;
                    sf = GetStringSize(cf, g);
                }
            }


            StringFormat format = new StringFormat();
            format.FormatFlags = StringFormatFlags.NoClip;
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            g.DrawString(cf.Text, cf.Font, brush, rt, format);

        }

        public static void DrawStringFit(CoolFont cf, Graphics g, Brush brush, Rectangle rt)
        {
            DrawStringFit(cf.Text, cf.Font, g, brush, rt);
        }

        public static void DrawStringFit(string s, Font f, Graphics g, Brush brush, Rectangle rt, StringAlignment alignment, StringAlignment alignmentV)
        {
            CoolFont cf = new CoolFont(f, f.Style);
            cf.Text = s;

            SizeF sf = GetStringSize(cf, g);
            cf.FontSize = Math.Abs(sf.Width - rt.Width) / cf.Text.Length;
            sf = GetStringSize(cf, g);

            if (sf.Width > rt.Width)
            {
                while (sf.Width > rt.Width)
                {
                    cf.FontSize -= 0.1F;

                    sf = GetStringSize(cf, g);
                }
            }
            else if (sf.Width < rt.Width)
            {
                while (sf.Width < rt.Width)
                {
                    cf.FontSize += 0.1F;
                    sf = GetStringSize(cf, g);
                }
                cf.FontSize -= 0.1F;
            }

            if (sf.Height > rt.Height)
            {
                while (sf.Height > rt.Height)
                {
                    cf.FontSize -= 0.1F;
                    sf = GetStringSize(cf, g);
                }
            }


            StringFormat format = new StringFormat();
            format.FormatFlags = StringFormatFlags.NoClip;
            format.Alignment = alignment;
            format.LineAlignment = alignmentV;

            g.DrawString(cf.Text, cf.Font, brush, rt, format);
        }

        public static void DrawStringFit(CoolFont cf, Graphics g, Brush brush, Rectangle rt, StringAlignment alignment, StringAlignment alignmentV)
        {
            DrawStringFit(cf.Text, cf.Font, g, brush, rt, alignment, alignmentV);
        }
        #endregion
    }
}
