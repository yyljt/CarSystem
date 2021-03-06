﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Commons
{
    public class ImageServer
    {
        /// <summary> 
        /// 生成缩略图 (正式使用这个函数)
        /// </summary> 
        /// <param name="SourceFile">文件在服务器上的物理地址</param> 
        /// <param name="strSavePathFile">保存在服务器上的路径（物理地址）</param> 
        /// <param name="ThumbWidth">宽度</param> 
        /// <param name="ThumbHeight">高度</param> 
        /// <param name="BgColor">背景</param> 
        public void myGetThumbnailImage(string SourceFile, string strSavePathFile, int ThumbWidth, int ThumbHeight, string BgColor)
        {
            var oImg = System.Drawing.Image.FromFile(SourceFile);

      


            //小图 
            int intwidth, intheight;
            //当图片的宽度大于高度
            if (oImg.Width > oImg.Height)
            {
                if (ThumbWidth > 0)  //限制了宽度
                {
                    if (oImg.Width > ThumbWidth)
                    {
                        intwidth = ThumbWidth;
                        intheight = (oImg.Height * ThumbWidth) / oImg.Width;
                    }
                    else
                    {
                        intwidth = oImg.Width;
                        intheight = oImg.Height;
                    }
                }
                else//不限制宽度，那就意味着限制高度
                {
                    if (oImg.Height > ThumbHeight)
                    {
                        intwidth = (oImg.Width * ThumbHeight) / oImg.Height;
                        intheight = ThumbHeight;
                    }
                    else
                    {
                        intwidth = oImg.Width;
                        intheight = oImg.Height;
                    }
                }
            }
            else
            {
                if (ThumbHeight > 0)
                {
                    if (oImg.Height > ThumbHeight)
                    {
                        intwidth = (oImg.Width * ThumbHeight) / oImg.Height;
                        intheight = ThumbHeight;
                    }
                    else
                    {
                        intwidth = oImg.Width;
                        intheight = oImg.Height;
                    }
                }
                else
                {
                    if (oImg.Width > ThumbWidth)
                    {
                        intwidth = ThumbWidth;
                        intheight = (oImg.Height * ThumbWidth) / oImg.Width;
                    }
                    else
                    {
                        intwidth = oImg.Width;
                        intheight = oImg.Height;
                    }
                }
            }
            //构造一个指定宽高的Bitmap 
            Bitmap bitmay = new Bitmap(intwidth, intheight);
            Graphics g = Graphics.FromImage(bitmay);
            Color myColor;
            if (BgColor == null)
                myColor = Color.FromName("white");
            else
                myColor = Color.FromName(BgColor);
            //用指定的颜色填充Bitmap 
            g.Clear(myColor);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;// HighQualityBicubic;//最高质量的缩略
            //开始画图 
            g.DrawImage(oImg, new Rectangle(0, 0, intwidth, intheight), new Rectangle(0, 0, oImg.Width, oImg.Height), GraphicsUnit.Pixel);
            bitmay.Save(strSavePathFile, System.Drawing.Imaging.ImageFormat.Jpeg);
            g.Dispose();
            bitmay.Dispose();
            oImg.Dispose();
        }

        /// <summary> 
        /// 生成缩略图 
        /// </summary> 
        /// <param name="originalImagePath">源图路径（物理路径）</param> 
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param> 
        /// <param name="width">缩略图宽度</param> 
        /// <param name="height">缩略图高度</param> 
        /// <param name="mode">生成缩略图的方式</param>   
        public  void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            Image originalImage = Image.FromFile(originalImagePath);
            string sourpath = thumbnailPath.Substring(0, thumbnailPath.LastIndexOf("\\"));

            if (!Directory.Exists(sourpath))
            {
                Directory.CreateDirectory(sourpath);
            }


            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case "HW"://指定高宽缩放（可能变形）                 
                    break;
                case "W"://指定宽，高按比例                     
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H"://指定高，宽按比例 
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）                 
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp图片 
            Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            //新建一个画板 
            Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //设置高质量插值法 
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度 
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充 
            g.Clear(Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分 
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
                new Rectangle(x, y, ow, oh),
                GraphicsUnit.Pixel);

            try
            {
                //以jpg格式保存缩略图 
                bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }


        public string DoloadImg(string surl, string picurlDir)
        {

            String wlDir = picurlDir;
            String wlDir2 = picurlDir + "/Thumb";

            if (!Directory.Exists(wlDir))
            {
                Directory.CreateDirectory(wlDir);
            }
            if (!Directory.Exists(wlDir2))
            {
                Directory.CreateDirectory(wlDir2);
            }

            string filePath = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            filePath += new Random().Next(99).ToString("00");


            try
            {
                String sfile = filePath + ".jpg";
                String wlfile = wlDir + "\\" + sfile;
                WebClient myWebClient = new WebClient();
                myWebClient.DownloadFile(surl, wlfile);

                int[] list = { 32, 60, 80, 100, 160, 220, 360 };
                foreach (var type in list)
                {
                    String wlfile2 = wlDir2 + "\\" + type + sfile;
                    myGetThumbnailImage(wlfile, wlfile2, type, type, null);
                }

                return sfile;
            }
            catch
            {
                return "";
            }


        }

        public static string DoloadImg(byte[] data, string picurlDir)
        {

            String wlDir = picurlDir;//HttpContext.Current.Server.MapPath(picurlDir);
            String wlDir2 = picurlDir + "/Thumb";

            if (!Directory.Exists(wlDir))
            {
                Directory.CreateDirectory(wlDir);
            }
            if (!Directory.Exists(wlDir2))
            {
                Directory.CreateDirectory(wlDir2);
            }

            string filePath = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            filePath += new Random().Next(99).ToString("00");


            try
            {
                MemoryStream ms = new MemoryStream(data);
                Image image = System.Drawing.Image.FromStream(ms);

                String wlfile = wlDir + "\\" + filePath + ".jpg";

                image.Save(wlfile);
                ms.Close();
                return wlfile;
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex);
                return "";
            }


        }

        public byte[] GetPictureData(string imagepath)
        {
            try
            {
                string imgPath = imagepath; //HttpContext.Current.Server.MapPath(imagepath);
                if (!File.Exists(imgPath))
                    return null;
                /**/
                ////根据图片文件的路径使用文件流打开，并保存为byte[] 
                FileStream fs = new FileStream(imgPath, FileMode.Open); //可以是其他重载方法 
                byte[] byData = new byte[fs.Length];
                fs.Read(byData, 0, byData.Length);
                fs.Close();
                return byData;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static Random randNum = new Random();
        //将文件名转化为时间方式
        private string CreateFilePath(string fext)
        {
            string filePath = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            filePath += new Random().Next(99).ToString("00");
            filePath += "." + fext;
            return filePath;
        }

        public static string CreateImageFromBytes(string fileName, byte[] buffer)
        {
            string file = fileName;
            Image image = BytesToImage(buffer);
            ImageFormat format = image.RawFormat;
            if (format.Equals(ImageFormat.Jpeg))
            {
                file += ".jpeg";
            }
            else if (format.Equals(ImageFormat.Png))
            {
                file += ".png";
            }
            else if (format.Equals(ImageFormat.Bmp))
            {
                file += ".bmp";
            }
            else if (format.Equals(ImageFormat.Gif))
            {
                file += ".gif";
            }
            else if (format.Equals(ImageFormat.Icon))
            {
                file += ".icon";
            }
            System.IO.FileInfo info = new System.IO.FileInfo(file);
            System.IO.Directory.CreateDirectory(info.Directory.FullName);
            File.WriteAllBytes(file, buffer);
            return file;
        }

        /// <summary>
        /// Convert Byte[] to Image
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Image BytesToImage(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            Image image = System.Drawing.Image.FromStream(ms);

            return image;
        }
    }
}