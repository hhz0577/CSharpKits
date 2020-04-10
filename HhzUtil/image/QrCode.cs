using System;
using System.Collections.Generic;
using System.Text;

namespace HhzUtil.image
{
    using ThoughtWorks.QRCode.Codec;
    using ThoughtWorks.QRCode.Codec.Data;
    using System.Drawing;
    //二维码
    public class QrCode
    {
        public Bitmap getDimensionalBmp(string qrcode)
        {
            Bitmap bmp = null;
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 3;
            //int version = Convert.ToInt16(cboVersion.Text);
            qrCodeEncoder.QRCodeVersion = 8;
            //qrCodeEncoder.QRCodeScale = 4;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            bmp = qrCodeEncoder.Encode(qrcode);
            return bmp;
        }
        public string getDimensionalCode(Bitmap bmp)
        {
            if (bmp == null) return "";
            QRCodeDecoder decoder = new QRCodeDecoder();
            string decodedString = decoder.decode(new QRCodeBitmapImage(bmp));
            return decodedString;
        }
    }
}
