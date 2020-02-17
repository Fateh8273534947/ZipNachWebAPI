using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ImageUploadWCF
{
    public sealed class Global
    {
        //use to display ApplicationName
        public const string ApplicationName = "Digital University";

        // Used to Generate and Get Random Number based on Code Length
        public static string ReverseString(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
        public static string CreateRandomCode(int CodeLength)
        {
            string allChar = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
            string[] allCharArray = allChar.Split(',');
            string randomCode = "";
            int temp = -1;

            Random rand = new Random();
            // Loop Starts to Generate Random Number 
            for (int i = 0; i < CodeLength; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks)); // Used here System DateTime to give more diversification.
                }
                int t = rand.Next(62);
                if (temp != -1 && temp == t)
                {
                    // Recursive Calling of parent function 
                    return CreateRandomCode(CodeLength);
                }
                temp = t;
                randomCode += allCharArray[t];
            }
            return randomCode;
        }

        // Used to Get Auto Password with the Help of First Name
        public static string GetAutoPassword(string firstname)
        {
            string password = string.Empty;
            password = firstname.ToLower().Trim() + "_12345";
            return password;
        }



        // Used to Check string has only alphabates or not
        public static Boolean IsCheckCharacter(string str)
        {
            bool ValidateChar = true;
            for (int Counter = 0; Counter < str.Length; Counter++)
            {
                string passchar = str.Substring(Counter, 1);
                char c1 = passchar[0];
                int code = c1;


                if ((code > 64 && code < 91) || (code > 96 && code < 123) || (code == 32))
                {
                    ValidateChar = true;


                }
                else
                {
                    ValidateChar = false;
                    break;
                }

            }
            return ValidateChar;
        }



        /// <summary>
        /// Find string between two symbols
        /// </summary>
        /// <param name="strSource">strSource</param>
        /// <param name="strBegin">strBegin</param>
        /// <param name="strEnd">strEnd</param>
        /// <param name="includeBegin">includeBegin</param>
        /// <param name="includeEnd">includeEnd</param>
        /// <returns>string array</returns>
        public static string[] GetStringInBetween(string strSource, string strBegin,
            string strEnd, bool includeBegin, bool includeEnd)
        {
            string[] result = { "", "" };
            int iIndexOfBegin = strSource.IndexOf(strBegin);
            if (iIndexOfBegin != -1)
            {
                // include the Begin string if desired

                if (includeBegin)
                {
                    iIndexOfBegin -= strBegin.Length;
                }
                strSource = strSource.Substring(iIndexOfBegin + strBegin.Length);
                int iEnd = strSource.IndexOf(strEnd);
                if (iEnd != -1)
                {  // include the End string if desired
                    if (includeEnd)
                    {
                        iEnd += strEnd.Length;
                    }
                    result[0] = strSource.Substring(0, iEnd);
                    // advance beyond this segment
                    if (iEnd + strEnd.Length < strSource.Length)
                    {
                        result[1] = strSource.Substring(iEnd + strEnd.Length);
                    }
                }
            }
            else
            {
                result[1] = strSource;
            }
            return result;
        }

        /// <summary>
        /// Removes special characters from input string.
        /// </summary>
        /// <param name="inputString">inputString</param>
        /// <returns>string</returns>
        public static string RemoveSpecialCharacters(string inputString)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in inputString)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == '@' || c == ' ' || c == '/' || c == '-')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Function Used to Resize the Image 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="Okey"></param>
        /// <param name="key"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="newimagename"></param>
        //public static Boolean ResizeImage(string ImageFromPath, string ImageToPath, float width, float height, ref byte[] ImageContent)
        //{
        //    //System.Drawing.Image oImg = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings[Okey] + image));
        //    System.Drawing.Image oImg = System.Drawing.Image.FromFile(ImageFromPath);

        //    float OriginalWidth = oImg.Width;
        //    float OriginalHeight = oImg.Height;

        //    float multiplecationFactor = OriginalWidth / OriginalHeight;
        //    width = multiplecationFactor * width;


        //    System.Drawing.Image oThumbNail = new System.Drawing.Bitmap(Convert.ToInt32(width), Convert.ToInt32(height));//, System.Drawing.Imaging.PixelFormat.Format24bppRgb

        //    System.Drawing.Graphics oGraphic = System.Drawing.Graphics.FromImage(oThumbNail);

        //    oGraphic.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

        //    //set smoothing mode to high quality
        //    oGraphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        //    //set the interpolation mode
        //    oGraphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        //    //set the offset mode
        //    oGraphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

        //    System.Drawing.Rectangle oRectangle = new System.Drawing.Rectangle(0, 0, Convert.ToInt32(width), Convert.ToInt32(height));

        //    oGraphic.DrawImage(oImg, oRectangle);

        //    if (ImageToPath.Substring(ImageToPath.LastIndexOf(".")) != ".png")
        //        oThumbNail.Save(ImageToPath, System.Drawing.Imaging.ImageFormat.Jpeg);
        //    else
        //        oThumbNail.Save(ImageToPath, System.Drawing.Imaging.ImageFormat.Png);



        //    System.Drawing.Image oImg2 = System.Drawing.Image.FromFile(ImageToPath);
        //    MemoryStream ms = new MemoryStream();
        //    oImg2.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
        //    ImageContent = ms.ToArray();


        //    //if (newimagename == "")
        //    //{
        //    //    if (ImageFromPath.Substring(ImageFromPath.LastIndexOf(".")) != ".png")
        //    //        //oThumbNail.Save(HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings[Okey] + image), System.Drawing.Imaging.ImageFormat.Jpeg);
        //    //        oThumbNail.Save(HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings[Okey] + image), System.Drawing.Imaging.ImageFormat.Jpeg);
        //    //    else
        //    //        //oThumbNail.Save(HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings[Okey] + image), System.Drawing.Imaging.ImageFormat.Png);
        //    //        oThumbNail.Save(HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings[Okey] + image), System.Drawing.Imaging.ImageFormat.Png);
        //    //}
        //    //else
        //    //{
        //    //    if (newimagename.Substring(newimagename.LastIndexOf(".")) != ".png")
        //    //        //oThumbNail.Save(HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings[Okey] + newimagename), System.Drawing.Imaging.ImageFormat.Jpeg);
        //    //        oThumbNail.Save(HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings[Okey] + newimagename), System.Drawing.Imaging.ImageFormat.Jpeg);
        //    //    else
        //    //        //oThumbNail.Save(HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings[Okey] + newimagename), System.Drawing.Imaging.ImageFormat.Png);
        //    //        oThumbNail.Save(HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings[Okey] + newimagename), System.Drawing.Imaging.ImageFormat.Png);
        //    //}
        //    oImg.Dispose();
        //    return true;
        //}



        public static long FindSize(object obj)
        {
            long lSize = 0;

            try
            {
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                BinaryFormatter objFormatter = new BinaryFormatter();
                objFormatter.Serialize(stream, obj);
                lSize = stream.Length;
                //Console.WriteLine("Size of the Object: " + lSize);

            }
            catch (Exception excp)
            {
                Console.WriteLine("Error: " + excp.Message);
            }
            return lSize;
        }
    }
}