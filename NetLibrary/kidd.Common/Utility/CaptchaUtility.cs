using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;

namespace kidd.Common.Utility
{
    /// <summary>
    /// 驗證碼產生器
    /// </summary>
    public static class CaptchaUtility
    {
        /// <summary>
        /// 傳入指定字串，產生驗證碼圖片檔案位元組
        /// </summary>
        /// <param name="captcha"></param>
        /// <returns></returns>
        public static byte[] CreateCaptchaImage(string captcha)
        {
            Random rando = new Random();
            Bitmap image = new Bitmap(captcha.Length * 15, 24);
            Graphics graphics = Graphics.FromImage(image);
   
            //背景顏色
            graphics.Clear(Color.Cornsilk);

            //開啟反鋸齒
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;


            //產生擾亂線條
            for (int i = 0; i < 15; i++)
            {
                int x1 = rando.Next(image.Width);
                int x2 = rando.Next(image.Width);
                int y1 = rando.Next(image.Height);
                int y2 = rando.Next(image.Height);
                graphics.DrawLine(new Pen(Color.Gray), x1, y1, x2, y2);
            }

            //文字
            Font font = new Font("Courier New Bold", 14f);
            PointF point = new PointF(4f, 1f);
            SolidBrush solidBrush = new SolidBrush(Color.Black);
            graphics.DrawString(captcha, font, solidBrush, point);

            //產生雜點
            for (int i = 0; i < 100; i++)
            {
                int x = rando.Next(image.Width);
                int y = rando.Next(image.Height);
                image.SetPixel(x, y, Color.FromArgb(rando.Next()));

            }

            //將圖片轉換成位元組
            byte[] result = ImageUtility.ConvertToByte(image);

            //釋放資源
            graphics.Dispose();
            image.Dispose();

            return result;
        }


        /// <summary>
        /// 隨機產生一個花紋樣式
        /// </summary>
        /// <returns></returns>
        public static HatchStyle CreateRandomHatchStyle()
        {
            IEnumerator enumerator;
            ArrayList list = new ArrayList();

            enumerator = System.Enum.GetValues(typeof(HatchStyle)).GetEnumerator();
            while (enumerator.MoveNext())
            {
                HatchStyle style2 = (HatchStyle)Convert.ToInt16(enumerator.Current);
                list.Add(style2);
            }


            int num = new Random().Next(list.Count - 1);
            return (HatchStyle)Convert.ToInt16(list[num]);
        }
    }
}
