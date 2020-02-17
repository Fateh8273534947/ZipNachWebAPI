using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Web.Http;
using ImageUploadWCF;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using ZipNachWebAPI.Models;
using System.Web.Http.Cors;
using System.Web;
using ZXing;
using ZXing.Common;

namespace ZipNachWebAPI.Controllers
{
    [EnableCors(origins: "http://192.168.1.246:808/api/Clients", headers: "AllowAllHeaders", methods: "*")]
    public class UploadMandateBase64Controller : ApiController
    {
        Boolean Greater;
        public string hash1 = string.Empty;
     
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern int DeleteDC(IntPtr hdc);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern int BitBlt(IntPtr hdcDst, int xDst, int yDst, int w, int h, IntPtr hdcSrc, int xSrc, int ySrc, int rop);
        static int SRCCOPY = 0x00CC0020;
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFObase64 bmi, uint Usage, out IntPtr bits, IntPtr hSection, uint dwOffset);
        static uint BI_RGB = 0;
        static uint DIB_RGB_COLORS = 0;

        string finalresult = "";



        // [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]


        [Route("api/UploadMandate/UploadMandateSizeScanBase64")]
        [HttpPost]
        public ResponseDatabase64 UploadMandateCropped(CustomerModel context)
        {
            string TempMandateId = "";
         
            ResponseDatabase64 pathInfo = new ResponseDatabase64();
            // if (context.PrintQr == "1")
            //{
            //    TempMandateId = ReadQrCode(context.ScannedImage, context.MdtID);

            //}
            if (context.AppID == "")
            {
                pathInfo.Message = "Incomplete data";
                pathInfo.Status = "Failure";
                pathInfo.ResCode = "ykR20020";
               
                return pathInfo;
            }
            else if (context.AppID != "" && CheckMandateInfo.ValidateAppID(context.AppID) != true)
            {
                pathInfo.Message = "Invalid AppId";
                pathInfo.Status = "Failure";
                pathInfo.ResCode = "ykR20023";
             
                return pathInfo;
            }
            //else if (ValidatePresement.CheckAccess(context.AppID.Trim(), "A") != true)
            //{
            //    pathInfo.Message = "Unauthorized user";
            //    pathInfo.Status = "Failure";
            //    pathInfo.ResCode = "ykR20038";
            //    return pathInfo;
            //}
            else if (context.MerchantKey == "")
            {
                pathInfo.Message = "Incomplete data";
                pathInfo.Status = "Failure";
                pathInfo.ResCode = "ykR20020";
              
                return pathInfo;
            }
            else if (context.MerchantKey != "" && CheckMandateInfo.ValidateEntityMerchantKey(context.MerchantKey, context.AppID) != true)
            {
                pathInfo.Message = "Invalid MerchantKey";
                pathInfo.Status = "Failure";
                pathInfo.ResCode = "ykR20021";
              
                return pathInfo;
            }
            else if (context.MdtID == "")
            {
                pathInfo.Message = "Incomplete data";
                pathInfo.Status = "Failure";
                pathInfo.ResCode = "ykR20020";
               
                return pathInfo;
            }
            else if (context.RefrenceNo == "" || context.RefrenceNo == null)
            {
                pathInfo.Message = "Incomplete data";
                pathInfo.Status = "Failure";
                pathInfo.ResCode = "ykR20020";
              
                return pathInfo;
            }

            else if (context.ScannedImage == "")
            {
                pathInfo.Message = "Incomplete data";
                pathInfo.Status = "Failure";
                pathInfo.ResCode = "ykR20020";
               
                return pathInfo;
            }
            //else if (context.PrintQr == "")
            //{
            //    pathInfo.Message = "Incomplete data";
            //    pathInfo.Status = "Failure";
            //    pathInfo.ResCode = "ERR000";
            //    retList.Add(pathInfo);
            //    return pathInfo;
            //}

            else if (!CheckMandateInfo.CheckManadateID(context.MdtID, context.AppID))
            {
                pathInfo.Message = "Invalid MdtID";
                pathInfo.Status = "Failure";
                pathInfo.ResCode = "ykR20022";
              
                return pathInfo;
            }
            //else if (!CheckMandateInfo.CheckAccountValidation(context.MdtID, context.AppID))
            //{
            //    pathInfo.Message = "Account should be validated";
            //    pathInfo.Status = "Failure";
            //    pathInfo.ResCode = "ERR0004";
            //    retList.Add(pathInfo);
            //    return pathInfo;
            //}



            else if (!CheckMandateInfo.CheckPHENachValidation(context.MdtID, context.AppID))
            {
                pathInfo.Message = "Mandate type already selected as eMandate";
                pathInfo.Status = "Failure";
                pathInfo.ResCode = "ykR20030";
                return pathInfo;
            }



            //else if (TempMandateId.Trim() != context.MdtID.Trim()&&context.PrintQr=="1")
            // {
            //     pathInfo.Message = "Scan MdtID is not equal to the passed MdtID";
            //     pathInfo.Status = "Failure";
            //     pathInfo.ResCode = "ERR0003";
            //     retList.Add(pathInfo);
            //     return pathInfo;
            // }
            else
            {
                context.RefrenceNo = CheckMandateInfo.GetRefNO(context.MdtID, context.AppID);
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(context.AppID)].ConnectionString);
                bool Flag = false;
                //   string temp = ConfigurationManager.AppSettings["EnitityMarchantKey" + context.AppID];
                string UserId = "";
                string query = "Sp_WebAPI";
                //if (temp.Trim() == DBsecurity.Decrypt(context.MerchantKey))
                //{
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "GetEntityUser");
                cmd.Parameters.AddWithValue("@appId", context.AppID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    UserId = Convert.ToString(dt.Rows[0][0]);
                    Flag = true;
                }
                // }

                if (Flag)
                {
                    Boolean Greater;
                    try
                    {

                        string path = "";
                        string dirnm = "";

                        byte[] imagedata = System.Convert.FromBase64String(context.ScannedImage.ToString());
                        float mb = (imagedata.Length / 1024f) / 1024f;
                        if (mb <= 3)
                        {
                            try
                            {
                                using (System.Drawing.Image image = new Bitmap(new MemoryStream(imagedata)))
                                {

                                    path = ConfigurationManager.AppSettings["FileUploadPath" + context.AppID].ToString() + "Mandate/";
                                    if (!Directory.Exists(path))
                                    {
                                        Directory.CreateDirectory(path);
                                    }
                                    string FilePath = ConfigurationManager.AppSettings["FileUploadPath" + context.AppID].ToString() + "MandateFile/" + context.MdtID;


                                    bool Flag1 = Directory.Exists(FilePath);
                                    if (!Flag1)
                                    {
                                        Directory.CreateDirectory(FilePath);
                                    }
                                    else
                                    {
                                        System.IO.DirectoryInfo di = new DirectoryInfo(FilePath);

                                        foreach (FileInfo file in di.GetFiles())
                                        {
                                            file.Delete();
                                        }

                                    }

                                    dirnm = Global.CreateRandomCode(6);
                                    string FileName = context.RefrenceNo + ".jpg";
                                    string PathWithImg = "Mandate/" + FileName;
                                    image.Save(path + FileName, ImageFormat.Png);
                                    string croppedFileName = string.Empty;
                                    string croppedFilePath = string.Empty;
                                    string TIFFilepath = string.Empty;
                                    string JPGFilepath = string.Empty;
                                    string TempJpgpath = string.Empty;
                                    string TempTifpath = string.Empty;
                                    string fileName = context.RefrenceNo + ".jpg";

                                    string filePath = path + FileName;
                                    if (File.Exists(filePath))
                                    {

                                        System.Drawing.Image orgImg = System.Drawing.Image.FromFile(filePath);
                                        if (orgImg.Width > orgImg.Height)
                                        {
                                            Greater = true;
                                        }
                                        else
                                        {
                                            Greater = false;
                                        }
                                        System.Drawing.Rectangle areaToCrop = new System.Drawing.Rectangle(Convert.ToInt32(0),
                                            Convert.ToInt32(0),
                                            Convert.ToInt32(orgImg.Width),
                                            Convert.ToInt32(orgImg.Height));

                                        Bitmap bitMap = new Bitmap(orgImg.Width, orgImg.Height, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);

                                        //Create graphics object for alteration
                                        using (Graphics g = Graphics.FromImage(bitMap))
                                        {
                                            //Draw image to screen
                                            g.DrawImage(orgImg, new System.Drawing.Rectangle(0, 0, bitMap.Width, bitMap.Height), areaToCrop, GraphicsUnit.Pixel);
                                            g.CompositingQuality = CompositingQuality.HighQuality;
                                            g.SmoothingMode = SmoothingMode.HighQuality;
                                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                                        }


                                        //name the cropped image
                                        //fileName = No  +".jpg";
                                        croppedFileName = context.RefrenceNo + ".jpg";
                                        //Create path to store the cropped image
                                        if (!Directory.Exists(ConfigurationManager.AppSettings["FileUploadPath" + context.AppID].ToString() + "CropImage/"))
                                        {
                                            Directory.CreateDirectory(ConfigurationManager.AppSettings["FileUploadPath" + context.AppID].ToString() + "CropImage/");
                                        }
                                        croppedFilePath = ConfigurationManager.AppSettings["FileUploadPath" + context.AppID].ToString() + "CropImage/" + croppedFileName;
                                        bitMap.Save(croppedFilePath);
                                        var CropImagePath = ConfigurationManager.AppSettings["FileUploadPath" + context.AppID].ToString() + "CropImage/" + context.RefrenceNo + ".jpg";
                                        System.Drawing.Image CropImage = System.Drawing.Image.FromFile(CropImagePath);

                                        using (var image1 = CropImage)
                                        {
                                            //int newWidth = 0;
                                            //int newHeight = 0;
                                            //if (Greater == true)
                                            //{
                                            //    newWidth = 2800; // New Width of Image in Pixel  
                                            //    newHeight = 1200;
                                            //}
                                            //else
                                            //{
                                            //    newWidth = 1700;
                                            //    newHeight = 4000;
                                            //}
                                            int newWidth = 827; // New Width of Image in Pixel  
                                            int newHeight = 356; // New Height of Image in Pixel          
                                            var thumbImg = new Bitmap(newWidth, newHeight, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);

                                            var thumbGraph = Graphics.FromImage(thumbImg);
                                            var imgRectangle = new System.Drawing.Rectangle(0, 0, newWidth, newHeight);

                                            thumbGraph.DrawImage(image1, imgRectangle);


                                            Bitmap b0 = new Bitmap(thumbImg);
                                            //if (Greater == false)
                                            //{
                                            //    b0 = RotateImg(b0, 270);
                                            //}
                                            b0 = CopyToBpp(b0, 1);
                                            b0.SetResolution(100, 100);
                                            croppedFileName = ConfigurationManager.AppSettings["DownloadFileName" + context.AppID].ToString() + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + context.RefrenceNo + ".jpg";

                                            croppedFilePath = ConfigurationManager.AppSettings["FileUploadPath" + context.AppID].ToString() + "MandateFile/" + context.MdtID + "/" + croppedFileName;
                                            JPGFilepath = "../MandateFile/" + context.MdtID + "/" + croppedFileName;
                                            TempJpgpath = croppedFilePath;
                                            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                                            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                                            EncoderParameters myEncoderParameters = new EncoderParameters(1);
                                            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
                                            myEncoderParameters.Param[0] = myEncoderParameter;
                                            b0.Save(croppedFilePath, jpgEncoder, myEncoderParameters);
                                           // b0.Save(croppedFilePath, image1.RawFormat);

                                        }
                                        var CropImagePath1 = ConfigurationManager.AppSettings["FileUploadPath" + context.AppID].ToString() + "CropImage/" + context.RefrenceNo + ".jpg";
                                        System.Drawing.Image CropImage1 = System.Drawing.Image.FromFile(CropImagePath1);

                                        using (var image1 = CropImage1)
                                        {
                                            //int newWidth = 0;
                                            //int newHeight = 0;
                                            //if (Greater == true)
                                            //{
                                            //    //newWidth = 2100;
                                            //    //newHeight = 900;
                                            //    newWidth = CropImage1.Width;
                                            //    newHeight = CropImage1.Height;
                                            //}
                                            //else
                                            //{
                                            //    //newWidth = 900;
                                            //    //newHeight = 2100;
                                            //    newWidth = CropImage1.Width;
                                            //    newHeight = CropImage1.Height;
                                            //}
                                            int newWidth = 827 * 2;
                                            int newHeight = 356 * 2;
                                            var thumbImg1 = new Bitmap(newWidth, newHeight);

                                            var thumbGraph1 = Graphics.FromImage(thumbImg1);

                                            var imgRectangle1 = new System.Drawing.Rectangle(0, 0, newWidth, newHeight);

                                            thumbGraph1.DrawImage(image1, imgRectangle1);


                                            Bitmap b1 = new Bitmap(thumbImg1);
                                            if (Greater == false)
                                            {
                                                b1 = RotateImg(b1, 270);
                                            }
                                            b1 = CopyToBpp(b1, 1);

                                            b1.SetResolution(200, 200);
                                            croppedFileName = ConfigurationManager.AppSettings["DownloadFileName" + context.AppID].ToString() + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + context.RefrenceNo + ".tif";

                                            croppedFilePath = ConfigurationManager.AppSettings["FileUploadPath" + context.AppID].ToString() + "MandateFile/" + context.MdtID + "/" + croppedFileName;
                                            TIFFilepath = "../MandateFile/" + context.MdtID + "/" + croppedFileName;
                                            TempTifpath = croppedFilePath;
                                            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Tiff);
                                            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Compression;
                                            EncoderParameters myEncoderParameters = new EncoderParameters(1);
                                            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder,
                                                    (long)EncoderValue.CompressionCCITT4);
                                            myEncoderParameters.Param[0] = myEncoderParameter;
                                            b1.Save(croppedFilePath, jpgEncoder, myEncoderParameters);
                                          //  b1.Save(croppedFilePath, image1.RawFormat);

                                        }


                                        try
                                        {
                                            CropImage.Dispose();
                                            CropImage1.Dispose();
                                            orgImg.Dispose();

                                            if (File.Exists(ConfigurationManager.AppSettings["FileUploadPath" + context.AppID].ToString() + "CropImage/" + context.RefrenceNo + ".jpg"))
                                            {
                                                File.Delete(ConfigurationManager.AppSettings["FileUploadPath" + context.AppID].ToString() + "CropImage/" + context.RefrenceNo + ".jpg");
                                            }
                                            if (File.Exists(ConfigurationManager.AppSettings["FileUploadPath" + context.AppID].ToString() + "Mandate/" + FileName))
                                            {
                                                File.Delete(ConfigurationManager.AppSettings["FileUploadPath" + context.AppID].ToString() + "Mandate/" + FileName);
                                            }
                                        }
                                        catch
                                        {
                                        }

                                        bitMap = null;
                                        query = "Sp_WebAPI";

                                        cmd = new SqlCommand(query, con);
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        cmd.Parameters.AddWithValue("@QueryType", "UpdateImage");
                                        cmd.Parameters.AddWithValue("@TIFPath", TIFFilepath);
                                        cmd.Parameters.AddWithValue("@JPGPath", JPGFilepath);
                                        cmd.Parameters.AddWithValue("@MandateId", context.MdtID);
                                        da = new SqlDataAdapter(cmd);
                                        DataSet ds = new DataSet();
                                        da.Fill(ds);
                                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                                        {
                                            pathInfo.Message = "Image uploaded successfully";
                                            pathInfo.Status = "Success";
                                            pathInfo.ResCode = "ykR20033";
                                            //pathInfo.JpgImage = ConfigurationManager.AppSettings["FilePathURL" + context.AppID].ToString() + JPGFilepath.Substring(3, JPGFilepath.Length - 3); 
                                            //pathInfo.TifImage = ConfigurationManager.AppSettings["FilePathURL" + context.AppID].ToString()  + TIFFilepath.Substring(3, TIFFilepath.Length - 3); ;
                                            pathInfo.JpgImage = ConvertImageBase64(TempJpgpath);
                                            pathInfo.TifImage = ConvertImageBase64(TempTifpath);
                                            pathInfo.MdtID = context.MdtID;
                                        }
                                        else
                                        {
                                            pathInfo.ResCode = "ykR20020";
                                            pathInfo.Status = "Failure";
                                            pathInfo.Message = "Invalid data";
                                            pathInfo.JpgImage = "";
                                            pathInfo.TifImage = "";
                                        }

                                    }
                                }

                              
                                return pathInfo;
                            }
                            catch (Exception e)
                            {
                                pathInfo.ResCode = "ykR20020";
                                pathInfo.Status = "Failure";
                                pathInfo.Message = "Invalid data";
                                pathInfo.JpgImage = "";
                                pathInfo.TifImage = "";
                            }
                        }
                        else
                        {
                            pathInfo.ResCode = "ykR20026";
                            pathInfo.Status = "Failure";
                            pathInfo.Message = "image size should be not greater than 3 MB";
                            pathInfo.JpgImage = "";
                            pathInfo.TifImage = "";
                        }
                       
                        return pathInfo;
                    }
                    catch (Exception e)
                    {
                        pathInfo.ResCode = "ykR20020";
                        pathInfo.Status = "Failure";
                        pathInfo.Message = "Invalid data";
                        pathInfo.JpgImage = "";
                        pathInfo.TifImage = "";
                    }
                   
                    return pathInfo;
                }
                else
                {
                    pathInfo.Status = "Failure";
                    pathInfo.Message = "Invalid data";
                    pathInfo.ResCode = "ykR20020";
                  
                    return pathInfo;

                }

            }
            return pathInfo;

        }
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        public string ConvertImageBase64(string path)
        {
            //byte[] imageArray = System.IO.File.ReadAllBytes(@"E:\Projects\Fateh\MandatePic.jpg");
            byte[] imageArray = System.IO.File.ReadAllBytes(path);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);
            string[] temp = base64ImageRepresentation.Split(',');
            var img = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(base64ImageRepresentation)));
            return base64ImageRepresentation;
        }



        private Bitmap RotateImg(Bitmap bmp, float angle)
        {
            int w = bmp.Width;
            int h = bmp.Height;


            Bitmap tempImg = new Bitmap(w, h);
            Graphics g = Graphics.FromImage(tempImg);

            g.DrawImageUnscaled(bmp, 1, 1);
            g.Dispose();

            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new RectangleF(0f, 0f, w, h));
            Matrix mtrx = new Matrix();
            //Using System.Drawing.Drawing2D.Matrix class 
            mtrx.Rotate(angle);
            RectangleF rct = path.GetBounds(mtrx);
            Bitmap newImg = new Bitmap(Convert.ToInt32(rct.Width), Convert.ToInt32(rct.Height));
            g = Graphics.FromImage(newImg);

            g.TranslateTransform(-rct.X, -rct.Y);
            g.RotateTransform(angle);
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            g.DrawImageUnscaled(tempImg, 0, 0);
            g.Dispose();
            tempImg.Dispose();


            return newImg;
        }
        public void GenerateGrid(string Id, string UserIdd, string AppID)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(AppID)].ConnectionString);
            try
            {
                string query = "Sp_WebAPI";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "ShowDataMobile");
                cmd.Parameters.AddWithValue("@Id", Id);
                cmd.Parameters.AddWithValue("@UserId", UserIdd);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);


                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {

                        //var FontColour = new BaseColor(35, 31, 32);
                        iTextSharp.text.Font fontAb11 = FontFactory.GetFont("Verdana", 8, iTextSharp.text.Font.BOLD);
                        iTextSharp.text.Font font9 = FontFactory.GetFont("Verdana", 8, iTextSharp.text.Font.BOLD);
                        iTextSharp.text.Font fontAb11BU = FontFactory.GetFont("Verdana", 7, iTextSharp.text.Font.UNDERLINE, iTextSharp.text.Color.GRAY);
                        iTextSharp.text.Font fontAb6 = FontFactory.GetFont("Verdana", 8, iTextSharp.text.Font.BOLD);
                        iTextSharp.text.Font fontAb9 = FontFactory.GetFont("Verdana", 8, iTextSharp.text.Font.BOLD);
                        iTextSharp.text.Font fontAb9B = FontFactory.GetFont("Verdana", 8, iTextSharp.text.Font.BOLD);
                        iTextSharp.text.Font fontAb11B = FontFactory.GetFont("Verdana", 8, iTextSharp.text.Font.BOLD);
                        iTextSharp.text.Font fontA11B = FontFactory.GetFont("Verdana", 8, iTextSharp.text.Font.BOLD);
                        iTextSharp.text.Font fontA119B = FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.BOLD);
                        iTextSharp.text.Font fontText = FontFactory.GetFont("Verdana", 7, iTextSharp.text.Font.BOLD);
                        iTextSharp.text.Font fontText6 = FontFactory.GetFont("Verdana", 6, iTextSharp.text.Font.BOLD);
                        iTextSharp.text.Font fontText5 = FontFactory.GetFont("Verdana", 7, iTextSharp.text.Font.BOLD);
                        iTextSharp.text.Font fontText4 = FontFactory.GetFont("Verdana", 1);


                        //Font fontText = FontFactory.GetFont("Verdana", 7);
                        string query1 = "Sp_LogoImageData";

                        SqlCommand cmd1 = new SqlCommand(query1, con);
                        cmd1.CommandType = CommandType.StoredProcedure;
                        cmd1.Parameters.AddWithValue("@QueryType", "Getcutternew");



                        SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                        DataSet ds1 = new DataSet();
                        da1.Fill(ds1);

                        iTextSharp.text.Image CutterImage = iTextSharp.text.Image.GetInstance((Byte[])(ds1.Tables[0].Rows[0][0]));
                        CutterImage.ScaleToFit(550f, 200f);

                        //iTextSharp.text.Image checkBox = iTextSharp.text.Image.GetInstance(BusinessLibrary1.GetCheckBox());
                        //checkBox.Border = iTextSharp.text.Rectangle.BOX;
                        //checkBox.BorderColor = iTextSharp.text.Color.BLACK;
                        //checkBox.BorderWidth = 12f;
                        //checkBox.ScaleToFit(7f, 7f);



                        //string path = ds.Tables[1].Rows[0]["ImagePath"].ToString();
                        string filePath = ConfigurationManager.AppSettings["FileUploadPath" + AppID].ToString();
                        //string filename = Path.GetFileName(filePath + path);

                        FileStream fs = new FileStream(filePath + "/PdfLogoImage/logo.png", FileMode.Open, FileAccess.Read);
                        BinaryReader br = new BinaryReader(fs);
                        Byte[] bytes = br.ReadBytes((Int32)fs.Length);

                        //br.Close();
                        //fs.Close();

                        //string strQuery = "insert into tblLogoImages(ImageData) values (@ImageData)";
                        //SqlCommand cmd = new SqlCommand(strQuery);
                        //cmd.Parameters.Add("@ImageData", SqlDbType.Binary).Value = bytes;
                        //InsertUpdateData(cmd);C:\Projects\NEWPROJECTS\ZipNACH\Saisho\Banking System\PdfLogoImage\logo.png

                        iTextSharp.text.Image LogoImage = iTextSharp.text.Image.GetInstance(bytes);
                        LogoImage.ScaleAbsolute(50f, 20f);


                        string query11 = "Sp_LogoImageData";

                        SqlCommand cmd11 = new SqlCommand(query11, con);
                        cmd11.CommandType = CommandType.StoredProcedure;
                        cmd11.Parameters.AddWithValue("@QueryType", "GetRupeeIcon");



                        SqlDataAdapter da11 = new SqlDataAdapter(cmd11);
                        DataSet ds11 = new DataSet();
                        da11.Fill(ds11);
                        iTextSharp.text.Image Rupee = iTextSharp.text.Image.GetInstance((Byte[])(ds1.Tables[0].Rows[0][0]));
                        //Rupee.Border = iTextSharp.text.Rectangle.BOX;
                        Rupee.BorderColor = iTextSharp.text.Color.BLACK;
                        Rupee.BorderWidth = 12f;
                        Rupee.ScaleToFit(7f, 7f);


                        FileStream fs111 = new FileStream(filePath + "/images/checkbox.jpg", FileMode.Open, FileAccess.Read);
                        BinaryReader br111 = new BinaryReader(fs111);
                        Byte[] bytes111 = br111.ReadBytes((Int32)fs111.Length);

                        iTextSharp.text.Image SmallcheckBox = iTextSharp.text.Image.GetInstance(bytes111);

                        SmallcheckBox.BorderColor = iTextSharp.text.Color.BLACK;
                        SmallcheckBox.BorderWidth = 12f;
                        SmallcheckBox.ScaleToFit(5f, 5f);


                        FileStream fs11 = new FileStream(filePath + "/images/tick-iconmandate.png", FileMode.Open, FileAccess.Read);
                        BinaryReader br11 = new BinaryReader(fs11);
                        Byte[] bytes11 = br11.ReadBytes((Int32)fs11.Length);

                        iTextSharp.text.Image checkBox = iTextSharp.text.Image.GetInstance(bytes11);

                        checkBox.BorderColor = iTextSharp.text.Color.BLACK;
                        checkBox.BorderWidth = 12f;
                        checkBox.ScaleToFit(8f, 8f);






                        FileStream fs1 = new FileStream(filePath + "/images/checkbox.jpg", FileMode.Open, FileAccess.Read);
                        BinaryReader br1 = new BinaryReader(fs1);
                        Byte[] bytes1 = br1.ReadBytes((Int32)fs1.Length);

                        //br.Close();
                        //fs.Close();

                        //string strQuery = "insert into tblLogoImages(ImageData) values (@ImageData)";
                        //SqlCommand cmd = new SqlCommand(strQuery);
                        //cmd.Parameters.Add("@ImageData", SqlDbType.Binary).Value = bytes;
                        //InsertUpdateData(cmd);

                        iTextSharp.text.Image Box = iTextSharp.text.Image.GetInstance(bytes1);

                        Box.BorderColor = iTextSharp.text.Color.BLACK;
                        Box.BorderWidth = 12f;
                        Box.ScaleToFit(8f, 8f);

                        int i = 1;

                        PdfPTable PdfHeaderTable = new PdfPTable(31);
                        PdfHeaderTable.DefaultCell.NoWrap = false;
                        PdfHeaderTable.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderTable.DefaultCell.Border = PdfCell.NO_BORDER;
                        PdfHeaderTable.WidthPercentage = 100;
                        float[] Headerwidths = new float[] { 4f, 3f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                        PdfHeaderTable.SetWidths(Headerwidths);
                        PdfPCell PdfHeaderCell = null;
                        Document document = new Document();
                        document.Open();
                        Paragraph p = new Paragraph();
                        p.Add(new Chunk(LogoImage, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));

                        PdfHeaderCell = new PdfPCell(p);
                        PdfHeaderCell.FixedHeight = 25f;
                        PdfHeaderCell.Rowspan = 2;
                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderTable.AddCell(PdfHeaderCell);

                        PdfHeaderCell = new PdfPCell(new Phrase("UMRN", fontAb11B));
                        PdfHeaderCell.NoWrap = false;
                        PdfHeaderCell.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderTable.AddCell(PdfHeaderCell);

                        PdfHeaderCell = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell.NoWrap = false;

                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderTable.AddCell(PdfHeaderCell);
                        PdfHeaderCell = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell.NoWrap = false;

                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderTable.AddCell(PdfHeaderCell);
                        PdfHeaderCell = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell.NoWrap = false;

                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderTable.AddCell(PdfHeaderCell);
                        PdfHeaderCell = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell.NoWrap = false;

                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderTable.AddCell(PdfHeaderCell);
                        PdfHeaderCell = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell.NoWrap = false;

                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderTable.AddCell(PdfHeaderCell);
                        PdfHeaderCell = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell.NoWrap = false;

                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderTable.AddCell(PdfHeaderCell);
                        PdfHeaderCell = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell.NoWrap = false;

                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderTable.AddCell(PdfHeaderCell);
                        PdfHeaderCell = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell.NoWrap = false;

                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderTable.AddCell(PdfHeaderCell);
                        PdfHeaderCell = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell.NoWrap = false;

                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderTable.AddCell(PdfHeaderCell);
                        PdfHeaderCell = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell.NoWrap = false;

                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderTable.AddCell(PdfHeaderCell);
                        PdfHeaderCell = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell.NoWrap = false;

                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderTable.AddCell(PdfHeaderCell);
                        PdfHeaderCell = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell.NoWrap = false;

                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderTable.AddCell(PdfHeaderCell);
                        PdfHeaderCell = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell.NoWrap = false;

                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderTable.AddCell(PdfHeaderCell);
                        PdfHeaderCell = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell.NoWrap = false;

                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderTable.AddCell(PdfHeaderCell);
                        PdfHeaderCell = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell.NoWrap = false;

                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderTable.AddCell(PdfHeaderCell);
                        PdfHeaderCell = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell.NoWrap = false;

                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderTable.AddCell(PdfHeaderCell);
                        PdfHeaderCell = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell.NoWrap = false;

                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderTable.AddCell(PdfHeaderCell);
                        PdfHeaderCell = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell.NoWrap = false;

                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderTable.AddCell(PdfHeaderCell);
                        PdfHeaderCell = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell.NoWrap = false;

                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderTable.AddCell(PdfHeaderCell);
                        PdfHeaderCell = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell.NoWrap = false;

                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderTable.AddCell(PdfHeaderCell);


                        //-------------------------------Add Date-------------------------------


                        PdfHeaderCell = new PdfPCell(new Phrase("Date", fontAb11B));
                        PdfHeaderCell.NoWrap = false;
                        PdfHeaderCell.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderTable.AddCell(PdfHeaderCell);

                        string Date = dr["SlipDate"].ToString();
                        char[] chars = new char[8];
                        chars = Date.ToCharArray();
                        if (Convert.ToInt32(chars.Length) > 0)
                        {
                            for (int j = 0; j < Convert.ToInt32(chars.Length); j++)
                            {
                                PdfHeaderCell = new PdfPCell(new Phrase(chars[j].ToString(), fontA119B));
                                PdfHeaderCell.NoWrap = false;
                                PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfHeaderCell.HorizontalAlignment = 1;
                                PdfHeaderTable.AddCell(PdfHeaderCell);
                            }
                        }
                        else
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                PdfHeaderCell = new PdfPCell(new Phrase(" ", fontA119B));
                                PdfHeaderCell.NoWrap = false;
                                PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfHeaderCell.HorizontalAlignment = 1;
                                PdfHeaderTable.AddCell(PdfHeaderCell);
                            }
                        }

                        //----------------------------------------Add Sponsor bankcode---------------------
                        string SpBankCode = dr["SponserBankCode"].ToString();

                        PdfHeaderCell = new PdfPCell(new Phrase("Sponsor bankcode", fontAb11B));
                        PdfHeaderCell.NoWrap = false;
                        PdfHeaderCell.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfHeaderCell.Colspan = 4;
                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfHeaderTable.AddCell(PdfHeaderCell);


                        PdfHeaderCell = new PdfPCell(new Phrase(dr["SponserBankCode"].ToString(), fontAb11B));
                        PdfHeaderCell.NoWrap = false;
                        PdfHeaderCell.Colspan = 12;
                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfHeaderTable.AddCell(PdfHeaderCell);


                        PdfHeaderCell = new PdfPCell(new Phrase("Utility Code", fontAb11B));
                        PdfHeaderCell.NoWrap = false;
                        PdfHeaderCell.Colspan = 6;
                        PdfHeaderCell.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfHeaderTable.AddCell(PdfHeaderCell);

                        PdfHeaderCell = new PdfPCell(new Phrase(dr["UtilityCode"].ToString(), fontAb11B));
                        PdfHeaderCell.NoWrap = false;
                        PdfHeaderCell.Colspan = 8;
                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfHeaderTable.AddCell(PdfHeaderCell);

                        Document documentCheckBox = new Document();
                        documentCheckBox.Open();
                        Paragraph pCheckBox = new Paragraph();
                        //------------------------------- add Created Status-------------------------------------
                        string Status = dr["CreatedStatus"].ToString();
                        if (Status == "C")
                        {
                            pCheckBox.Add(new Phrase("CREATE ", fontText));
                            pCheckBox.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBox.Add(new Phrase(" MODIFY ", fontText));
                            pCheckBox.Add(new Phrase(" CANCEL ", fontText));

                        }
                        else if (Status == "M")
                        {
                            pCheckBox.Add(new Phrase("CREATE ", fontText));

                            pCheckBox.Add(new Phrase(" MODIFY ", fontText));
                            pCheckBox.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBox.Add(new Phrase(" CANCEL ", fontText));

                        }
                        else if (Status == "L")
                        {
                            pCheckBox.Add(new Phrase("CREATE ", fontText));

                            pCheckBox.Add(new Phrase(" MODIFY ", fontText));

                            pCheckBox.Add(new Phrase(" CANCEL ", fontText));
                            pCheckBox.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                        }
                        PdfHeaderCell = new PdfPCell(pCheckBox);
                        PdfHeaderCell.NoWrap = false;
                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderTable.AddCell(PdfHeaderCell);

                        //------------------------------------add BenificiaryName----------------------------------
                        PdfHeaderCell = new PdfPCell(new Phrase("I/We hereby Authorize", fontText));
                        PdfHeaderCell.NoWrap = false;
                        PdfHeaderCell.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                        PdfHeaderCell.Colspan = 4;
                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfHeaderTable.AddCell(PdfHeaderCell);


                        PdfHeaderCell = new PdfPCell(new Phrase(dr["CompanyName"].ToString(), fontAb11B));
                        PdfHeaderCell.NoWrap = false;
                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell.Colspan = 10;
                        PdfHeaderCell.FixedHeight = 20f;
                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfHeaderTable.AddCell(PdfHeaderCell);

                        PdfHeaderCell = new PdfPCell(new Phrase("To Debit", fontAb11B));
                        PdfHeaderCell.NoWrap = false;
                        PdfHeaderCell.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell.Colspan = 5;
                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfHeaderTable.AddCell(PdfHeaderCell);
                        Document documentCheckBoxSB = new Document();
                        documentCheckBoxSB.Open();
                        Paragraph pCheckBoxSB = new Paragraph();
                        //----------------------------------add To Debit---------------------------
                        string chDebit = dr["DebitTo"].ToString();
                        if (chDebit == "SB")
                        {
                            pCheckBoxSB.Add(new Phrase(" ", fontText));
                            pCheckBoxSB.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" SB/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" CA/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" CC/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" SB-NRE/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" SB-NRO/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" OTHER ", fontText));
                        }
                        else if (chDebit == "CA")
                        {
                            pCheckBoxSB.Add(new Phrase(" ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" SB/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" CA/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" CC/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" SB-NRE/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" SB-NRO/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" OTHER ", fontText));
                        }

                        else if (chDebit == "CC")
                        {
                            pCheckBoxSB.Add(new Phrase(" ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" SB/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" CA/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" CC/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" SB-NRE/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" SB-NRO/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" OTHER ", fontText));
                        }
                        else if (chDebit == "RE")
                        {
                            pCheckBoxSB.Add(new Phrase(" ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" SB/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" CA/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" CC/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" SB-NRE/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" SB-NRO/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" OTHER ", fontText));
                        }
                        else if (chDebit == "RD")
                        {
                            pCheckBoxSB.Add(new Phrase(" ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" SB/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" CA/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" CC/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" SB-NRE/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" SB-NRO/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" OTHER ", fontText));
                        }
                        else if (chDebit == "OT")
                        {
                            pCheckBoxSB.Add(new Phrase(" ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" SB/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" CA/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" CC/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" SB-NRE/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" SB-NRO/ ", fontText));
                            pCheckBoxSB.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB.Add(new Phrase(" OTHER ", fontText));
                        }
                        PdfHeaderCell = new PdfPCell(pCheckBoxSB);
                        PdfHeaderCell.NoWrap = false;

                        PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell.Colspan = 11;
                        PdfHeaderCell.HorizontalAlignment = 1;
                        PdfHeaderCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfHeaderTable.AddCell(PdfHeaderCell);
                        PdfPTable PdfMidTable = new PdfPTable(32);
                        PdfMidTable.DefaultCell.NoWrap = false;
                        PdfMidTable.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfMidTable.DefaultCell.Border = PdfCell.NO_BORDER;
                        PdfMidTable.WidthPercentage = 100;
                        float[] PdfMidTableHeaderwidths = new float[] { 4f, 3f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                        PdfMidTable.SetWidths(PdfMidTableHeaderwidths);
                        PdfPCell PdfMidCell = null;
                        //----------------------------------Add AccountNo.-------------------------------------------------

                        PdfMidCell = new PdfPCell(new Phrase("Bank Account Number", fontAb11B));
                        PdfMidCell.NoWrap = false;
                        PdfMidCell.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell.Colspan = 6;
                        PdfMidCell.HorizontalAlignment = 1;
                        PdfHeaderCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfMidTable.AddCell(PdfMidCell);
                        string AccountNo = dr["AccountNo"].ToString();
                        char[] chrAcountNo = new char[Convert.ToInt32(AccountNo.Length)];
                        chrAcountNo = AccountNo.ToCharArray();
                        if (Convert.ToInt32(AccountNo.Length) <= 26)
                        {


                            for (int j = 0; j < Convert.ToInt32(chrAcountNo.Length); j++)
                            {
                                PdfMidCell = new PdfPCell(new Phrase(chrAcountNo[j].ToString(), fontA119B));
                                PdfMidCell.NoWrap = false;
                                PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfMidCell.HorizontalAlignment = 1;
                                PdfHeaderCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                PdfMidTable.AddCell(PdfMidCell);
                            }

                            int len = 26 - Convert.ToInt32(AccountNo.Length);
                            for (int k = 0; k < len; k++)
                            {
                                PdfMidCell = new PdfPCell(new Phrase(" ", fontA119B));
                                PdfMidCell.NoWrap = false;
                                PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfMidCell.HorizontalAlignment = 1;
                                PdfHeaderCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                PdfMidTable.AddCell(PdfMidCell);
                            }
                        }

                        PdfMidCell = new PdfPCell(new Phrase("With Bank", fontAb11B));
                        PdfMidCell.NoWrap = false;
                        PdfMidCell.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell.HorizontalAlignment = 1;
                        PdfMidCell.FixedHeight = 20f;
                        PdfMidCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfMidTable.AddCell(PdfMidCell);


                        //PdfMidCell = new PdfPCell(new Phrase(dr["BankName"].ToString(), fontAb11));
                        //PdfMidCell.NoWrap = false;

                        //PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        //PdfMidCell.Colspan = 6;
                        //PdfMidCell.HorizontalAlignment = 1;
                        //PdfHeaderCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        //PdfMidTable.AddCell(PdfMidCell);
                        //char[] array1 = (Convert.ToString(dr["BankName"]).Trim()).ToCharArray();
                        //if (array1.Length < 22)
                        //{
                        PdfMidCell = new PdfPCell(new Phrase(dr["BankName"].ToString(), fontAb11));
                        PdfMidCell.NoWrap = false;
                        PdfMidCell.FixedHeight = 30f;
                        PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell.Colspan = 6;
                        PdfMidCell.HorizontalAlignment = 1;
                        PdfMidCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfMidTable.AddCell(PdfMidCell);
                        //}
                        //else
                        //{
                        //    PdfMidCell = new PdfPCell(new Phrase(dr["BankName"].ToString(), fontText));
                        //    PdfMidCell.NoWrap = false;

                        //    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        //    PdfMidCell.Colspan = 6;
                        //    PdfMidCell.HorizontalAlignment = 1;
                        //    PdfMidTable.AddCell(PdfMidCell);
                        //}



                        PdfMidCell = new PdfPCell(new Phrase("IFSC", fontAb11B));
                        PdfMidCell.NoWrap = false;
                        PdfMidCell.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell.Colspan = 2;
                        PdfMidCell.HorizontalAlignment = 1;
                        PdfMidCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfMidTable.AddCell(PdfMidCell);

                        //-------------------------Add IFSC code--------------------------------
                        string IFSCcode = dr["IFSCcode"].ToString();
                        char[] chrIFSCcode = new char[Convert.ToInt32(IFSCcode.Length)];
                        chrIFSCcode = IFSCcode.ToCharArray();
                        if (Convert.ToInt32(chrIFSCcode.Length) == 11)
                        {
                            for (int j = 0; j < Convert.ToInt32(chrIFSCcode.Length); j++)
                            {
                                PdfMidCell = new PdfPCell(new Phrase(chrIFSCcode[j].ToString(), fontA119B));
                                PdfMidCell.NoWrap = false;
                                PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfMidCell.HorizontalAlignment = 1;
                                PdfMidCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;

                                PdfMidTable.AddCell(PdfMidCell);
                            }
                        }
                        else
                        {
                            for (int j = 0; j < 11; j++)
                            {
                                PdfMidCell = new PdfPCell(new Phrase(" ", fontA119B));
                                PdfMidCell.NoWrap = false;
                                PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfMidCell.HorizontalAlignment = 1;
                                PdfMidCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                PdfMidTable.AddCell(PdfMidCell);
                            }
                        }
                        PdfMidCell = new PdfPCell(new Phrase("or MICR", fontAb11B));
                        PdfMidCell.NoWrap = false;
                        PdfMidCell.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell.Colspan = 3;
                        PdfMidCell.HorizontalAlignment = 1;
                        PdfMidCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfMidTable.AddCell(PdfMidCell);

                        //-------------------------Add MICRcode--------------------------------
                        string MICRcode = dr["MICRcode"].ToString();
                        char[] chrMICRcode = new char[9];
                        chrMICRcode = MICRcode.ToCharArray();

                        if (Convert.ToInt32(chrMICRcode.Length) == 9)
                        {
                            for (int j = 0; j < Convert.ToInt32(chrMICRcode.Length); j++)
                            {
                                PdfMidCell = new PdfPCell(new Phrase(chrMICRcode[j].ToString(), fontA119B));
                                PdfMidCell.NoWrap = false;
                                PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfMidCell.HorizontalAlignment = 1;
                                PdfMidCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                PdfMidTable.AddCell(PdfMidCell);
                            }
                        }
                        else
                        {
                            for (int j = 0; j < 9; j++)
                            {
                                PdfMidCell = new PdfPCell(new Phrase(" ", fontA119B));
                                PdfMidCell.NoWrap = false;
                                PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfMidCell.HorizontalAlignment = 1;
                                PdfMidCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                PdfMidTable.AddCell(PdfMidCell);
                            }
                        }
                        //-----------------------------------Add amount of Rupees---------------------------
                        PdfMidCell = new PdfPCell(new Phrase("an amount of Rupees", fontAb11B));
                        PdfMidCell.NoWrap = false;
                        PdfMidCell.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell.Colspan = 3;
                        PdfMidCell.HorizontalAlignment = 1;
                        PdfMidTable.AddCell(PdfMidCell);

                        PdfMidCell = new PdfPCell(new Phrase(dr["AmountInWord"].ToString(), fontA11B));
                        PdfMidCell.NoWrap = false;
                        PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell.Colspan = 22;
                        PdfMidCell.FixedHeight = 10f;
                        PdfMidCell.HorizontalAlignment = 1;
                        PdfMidTable.AddCell(PdfMidCell);

                        Document documentAmountInDigit = new Document();
                        documentAmountInDigit.Open();
                        Paragraph pAmountInDigit = new Paragraph();
                        pAmountInDigit.Add(new Chunk(Rupee, PdfPCell.ALIGN_CENTER, PdfPCell.ALIGN_CENTER));
                        pAmountInDigit.Add(new Phrase(" " + dr["AmountInDigit"].ToString(), fontA119B));
                        PdfMidCell = new PdfPCell(pAmountInDigit);

                        PdfMidCell.NoWrap = false;

                        PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell.Colspan = 18;
                        PdfMidCell.HorizontalAlignment = 1;
                        PdfMidTable.AddCell(PdfMidCell);




                        //Document documentAmountInDigit = new Document();
                        //documentAmountInDigit.Open();
                        //Paragraph pAmountInDigit = new Paragraph();
                        //pAmountInDigit.Add(new Chunk(Rupee, PdfPCell.ALIGN_CENTER, PdfPCell.ALIGN_CENTER));
                        //pAmountInDigit.Add(new Phrase(" " + dr["AmountInDigit"].ToString(), fontAb11));
                        //PdfMidCell = new PdfPCell(pAmountInDigit);

                        //PdfMidCell.NoWrap = false;

                        //PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        //PdfMidCell.Colspan = 13;

                        //PdfMidTable.AddCell(PdfMidCell);

                        //---------------------------------Add Frequency--------------------------------------
                        string Freq = dr["Frequency"].ToString();

                        PdfMidCell = new PdfPCell(new Phrase("Frequency", fontAb11B));
                        PdfMidCell.NoWrap = false;
                        PdfMidCell.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell.HorizontalAlignment = 1;
                        PdfMidTable.AddCell(PdfMidCell);


                        Document documentMonthly = new Document();
                        documentMonthly.Open();
                        Paragraph pMonthly = new Paragraph();
                        //------------------------------- add Monthly-------------------------------------
                        if (Freq == "M")
                        {
                            pMonthly.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pMonthly.Add(new Phrase("  Monthly ", fontText));
                        }
                        else
                        {
                            pMonthly.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pMonthly.Add(new Phrase("  Monthly ", fontText));
                        }

                        PdfMidCell = new PdfPCell(pMonthly);
                        PdfMidCell.NoWrap = false;
                        PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell.Colspan = 2;
                        PdfMidCell.HorizontalAlignment = 1;
                        PdfMidTable.AddCell(PdfMidCell);

                        Document documentQtly = new Document();
                        documentQtly.Open();
                        Paragraph pQtly = new Paragraph();
                        //------------------------------- add Qtly-------------------------------------
                        if (Freq == "Q")
                        {
                            pQtly.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pQtly.Add(new Phrase("  Qtly ", fontText));
                        }
                        else
                        {
                            pQtly.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pQtly.Add(new Phrase("  Qtly ", fontText));
                        }

                        PdfMidCell = new PdfPCell(pQtly);
                        PdfMidCell.NoWrap = false;
                        PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell.Colspan = 2;
                        PdfMidCell.HorizontalAlignment = 1;
                        PdfMidTable.AddCell(PdfMidCell);

                        Document documentHYrly = new Document();
                        documentHYrly.Open();
                        Paragraph pHYrly = new Paragraph();
                        //------------------------------- add H-Yrly-------------------------------------
                        if (Freq == "H")
                        {
                            pHYrly.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pHYrly.Add(new Phrase("  H-Yrly ", fontText));
                        }
                        else
                        {
                            pHYrly.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pHYrly.Add(new Phrase("  H-Yrly ", fontText));
                        }

                        PdfMidCell = new PdfPCell(pHYrly);
                        PdfMidCell.NoWrap = false;
                        PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell.Colspan = 3;
                        PdfMidCell.HorizontalAlignment = 1;
                        PdfMidTable.AddCell(PdfMidCell);

                        Document documentYearly = new Document();
                        documentYearly.Open();
                        Paragraph pYearly = new Paragraph();
                        //------------------------------- add Yearly-------------------------------------
                        if (Freq == "Y")
                        {
                            pYearly.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pYearly.Add(new Phrase("  Yearly ", fontText));
                        }
                        else
                        {
                            pYearly.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pYearly.Add(new Phrase("  Yearly ", fontText));
                        }

                        PdfMidCell = new PdfPCell(pYearly);
                        PdfMidCell.NoWrap = false;
                        PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell.Colspan = 3;
                        PdfMidCell.HorizontalAlignment = 1;
                        PdfMidTable.AddCell(PdfMidCell);

                        Document prensentedprensented = new Document();
                        prensentedprensented.Open();
                        Paragraph prensented = new Paragraph();
                        if (Freq == "A")
                        {
                            prensented.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            prensented.Add(new Phrase("  As & when prensented ", fontText));
                        }
                        else
                        {
                            prensented.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            prensented.Add(new Phrase("  As & when prensented ", fontText));
                        }

                        PdfMidCell = new PdfPCell(prensented);
                        PdfMidCell.NoWrap = false;
                        PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell.Colspan = 7;
                        PdfMidCell.HorizontalAlignment = 1;
                        PdfMidTable.AddCell(PdfMidCell);

                        string DebitType = dr["DebitType"].ToString();

                        PdfMidCell = new PdfPCell(new Phrase("Debit Type", fontAb11B));
                        PdfMidCell.NoWrap = false;
                        PdfMidCell.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell.Colspan = 3;
                        PdfMidCell.HorizontalAlignment = 1;
                        PdfMidTable.AddCell(PdfMidCell);

                        Document documentFixed = new Document();
                        documentFixed.Open();
                        Paragraph pFixed = new Paragraph();
                        //------------------------------- add H-Yrly-------------------------------------
                        if (DebitType == "F")
                        {
                            pFixed.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pFixed.Add(new Phrase("  Fixed Amount ", fontText));
                        }
                        else
                        {
                            pFixed.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pFixed.Add(new Phrase("  Fixed Amount ", fontText));
                        }
                        PdfMidCell = new PdfPCell(pFixed);
                        PdfMidCell.NoWrap = false;
                        PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell.Colspan = 5;
                        PdfMidCell.HorizontalAlignment = 1;
                        PdfMidTable.AddCell(PdfMidCell);

                        Document documentMaximum = new Document();
                        documentMaximum.Open();
                        Paragraph pMaximum = new Paragraph();
                        //------------------------------- add H-Yrly-------------------------------------
                        if (DebitType == "M")
                        {
                            pMaximum.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pMaximum.Add(new Phrase("  Maximum Amount ", fontText));
                        }
                        else
                        {
                            pMaximum.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pMaximum.Add(new Phrase("  Maximum Amount ", fontText));
                        }

                        PdfMidCell = new PdfPCell(pMaximum);
                        PdfMidCell.NoWrap = false;
                        PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell.Colspan = 6;
                        PdfMidCell.HorizontalAlignment = 1;
                        PdfMidTable.AddCell(PdfMidCell);

                        PdfMidCell = new PdfPCell(new Phrase("Reference 1", fontAb11B));
                        PdfMidCell.NoWrap = false;
                        PdfMidCell.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell.HorizontalAlignment = 1;
                        PdfMidTable.AddCell(PdfMidCell);
                        PdfMidCell = new PdfPCell(new Phrase(dr["Reference1"].ToString(), fontAb11B));
                        PdfMidCell.NoWrap = false;
                        PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell.Colspan = 15;
                        PdfMidCell.HorizontalAlignment = 1;
                        PdfMidTable.AddCell(PdfMidCell);


                        PdfMidCell = new PdfPCell(new Phrase("Phone Number ", fontAb11B));
                        PdfMidCell.NoWrap = false;
                        PdfMidCell.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell.Colspan = 6;
                        PdfMidCell.HorizontalAlignment = 1;
                        PdfMidTable.AddCell(PdfMidCell);

                        PdfMidCell = new PdfPCell(new Phrase(dr["PhoneNo"].ToString(), fontAb11B));
                        PdfMidCell.NoWrap = false;

                        PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell.Colspan = 10;
                        PdfMidCell.HorizontalAlignment = 1;
                        PdfMidTable.AddCell(PdfMidCell);
                        PdfMidCell = new PdfPCell(new Phrase("Reference 2", fontAb11B));
                        PdfMidCell.NoWrap = false;
                        PdfMidCell.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell.HorizontalAlignment = 1;
                        PdfMidTable.AddCell(PdfMidCell);
                        PdfMidCell = new PdfPCell(new Phrase(dr["Reference2"].ToString(), fontAb11B));
                        PdfMidCell.NoWrap = false;
                        PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell.Colspan = 15;
                        PdfMidCell.HorizontalAlignment = 1;
                        PdfMidTable.AddCell(PdfMidCell);


                        PdfMidCell = new PdfPCell(new Phrase("EMail ID", fontAb11B));
                        PdfMidCell.NoWrap = false;
                        PdfMidCell.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell.Colspan = 6;
                        PdfMidCell.HorizontalAlignment = 1;
                        PdfMidTable.AddCell(PdfMidCell);
                        PdfMidCell = new PdfPCell(new Phrase(dr["EmailId"].ToString(), fontAb11B));
                        PdfMidCell.NoWrap = false;
                        PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell.Colspan = 10;
                        PdfMidCell.HorizontalAlignment = 1;
                        PdfMidTable.AddCell(PdfMidCell);
                        PdfMidCell = new PdfPCell(new Phrase("I agree for the debit of mandate processing charges by the bank whom I am authorizing to debit my account as per latest schedule of charges of bank ", fontText));
                        PdfMidCell.NoWrap = false;
                        PdfMidCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell.Colspan = 32;
                        PdfMidCell.HorizontalAlignment = 1;
                        PdfMidTable.AddCell(PdfMidCell);
                        PdfMidCell = new PdfPCell(new Phrase("PERIOD", fontAb11B));
                        PdfMidCell.NoWrap = false;
                        PdfMidCell.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell.HorizontalAlignment = 1;
                        PdfMidTable.AddCell(PdfMidCell);
                        PdfMidCell = new PdfPCell(new Phrase("", fontAb11B));
                        PdfMidCell.NoWrap = false;
                        PdfMidCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell.Colspan = 31;
                        PdfMidCell.HorizontalAlignment = 1;
                        PdfMidTable.AddCell(PdfMidCell);
                        PdfPTable PdfDetailTable = new PdfPTable(34);
                        PdfDetailTable.DefaultCell.NoWrap = false;
                        PdfDetailTable.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailTable.DefaultCell.Border = PdfCell.NO_BORDER;
                        PdfDetailTable.WidthPercentage = 100;
                        float[] Headerwidths1 = new float[] { 4f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                        PdfDetailTable.SetWidths(Headerwidths1);
                        PdfPCell PdfDetailCell = null;
                        PdfDetailCell = new PdfPCell(new Phrase("From", fontAb11B));
                        PdfDetailCell.NoWrap = false;
                        PdfDetailCell.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell.HorizontalAlignment = 1;
                        PdfDetailTable.AddCell(PdfDetailCell);

                        string PeriodFrom = dr["PeriodFrom"].ToString();
                        char[] chrPeriodFrom = new char[8];
                        chrPeriodFrom = PeriodFrom.ToCharArray();
                        if (Convert.ToInt32(chrPeriodFrom.Length) > 0)
                        {
                            for (int j = 0; j < Convert.ToInt32(chrPeriodFrom.Length); j++)
                            {
                                PdfDetailCell = new PdfPCell(new Phrase(chrPeriodFrom[j].ToString(), fontA119B));
                                PdfDetailCell.NoWrap = false;
                                PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell.HorizontalAlignment = 1;
                                PdfDetailTable.AddCell(PdfDetailCell);
                            }
                        }
                        else
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                PdfDetailCell = new PdfPCell(new Phrase(" ", fontA119B));
                                PdfDetailCell.NoWrap = false;
                                PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell.HorizontalAlignment = 1;
                                PdfDetailTable.AddCell(PdfDetailCell);
                            }

                        }
                        PdfDetailCell = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfDetailCell.NoWrap = false;
                        PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell.HorizontalAlignment = 1;
                        PdfDetailCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        PdfDetailCell.Colspan = 25;
                        PdfDetailTable.AddCell(PdfDetailCell);
                        PdfDetailCell = new PdfPCell(new Phrase("To*", fontAb11B));
                        PdfDetailCell.NoWrap = false;
                        PdfDetailCell.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell.HorizontalAlignment = 1;
                        PdfDetailTable.AddCell(PdfDetailCell);
                        string PeriodTo = dr["PeriodTo"].ToString();
                        char[] chrPeriodTo = new char[8];
                        chrPeriodTo = PeriodTo.ToCharArray();
                        if (Convert.ToInt32(chrPeriodTo.Length) > 0)
                        {
                            if (dr["PeriodTo"].ToString() != "01011900")
                            {
                                for (int j = 0; j < Convert.ToInt32(chrPeriodTo.Length); j++)
                                {
                                    PdfDetailCell = new PdfPCell(new Phrase(chrPeriodTo[j].ToString(), fontA119B));
                                    PdfDetailCell.NoWrap = false;
                                    PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell.HorizontalAlignment = 1;
                                    PdfDetailTable.AddCell(PdfDetailCell);
                                }
                            }
                            else
                            {
                                for (int j = 0; j < 8; j++)
                                {
                                    PdfDetailCell = new PdfPCell(new Phrase(" ", fontA119B));
                                    PdfDetailCell.NoWrap = false;
                                    PdfDetailCell.BackgroundColor = new iTextSharp.text.Color(0, 0, 0);
                                    PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell.HorizontalAlignment = 1;
                                    PdfDetailTable.AddCell(PdfDetailCell);
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                PdfDetailCell = new PdfPCell(new Phrase(" ", fontA119B));
                                PdfDetailCell.NoWrap = false;
                                PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell.HorizontalAlignment = 1;
                                PdfDetailCell.BackgroundColor = new iTextSharp.text.Color(0, 0, 0);
                                PdfDetailTable.AddCell(PdfDetailCell);
                            }
                        }
                        PdfDetailCell = new PdfPCell(new Phrase(" ", fontAb11B));
                        PdfDetailCell.NoWrap = false;
                        PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell.HorizontalAlignment = 1;
                        PdfDetailCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        PdfDetailCell.Colspan = 25;
                        PdfDetailTable.AddCell(PdfDetailCell);
                        PdfDetailCell = new PdfPCell(new Phrase("Or", fontAb11B));
                        PdfDetailCell.NoWrap = false;
                        PdfDetailCell.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell.HorizontalAlignment = 1;
                        PdfDetailTable.AddCell(PdfDetailCell);


                        Document documentCheckBox123 = new Document();
                        documentCheckBox.Open();
                        Paragraph pCheckBox123 = new Paragraph();
                        if (dr["PeriodTo"].ToString() == "01011900")
                        {
                            pCheckBox123.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                        }
                        else
                        {
                            pCheckBox123.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                        }
                        pCheckBox123.Add(new Phrase(" Until Cancelled ", fontAb11));


                        PdfDetailCell = new PdfPCell(pCheckBox123);

                        PdfDetailCell.NoWrap = false;
                        PdfDetailCell.Colspan = 8;
                        PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell.HorizontalAlignment = 1;
                        PdfDetailTable.AddCell(PdfDetailCell);



                        PdfDetailCell = new PdfPCell(new Phrase("Sign. Primary Acc. Holder", fontAb11BU));
                        PdfDetailCell.NoWrap = false;
                        PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell.HorizontalAlignment = 1;
                        PdfDetailCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        PdfDetailCell.Colspan = 8;
                        PdfDetailTable.AddCell(PdfDetailCell);
                        PdfDetailCell = new PdfPCell(new Phrase("Sign Acc. Holder", fontAb11BU));
                        PdfDetailCell.NoWrap = false;
                        PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell.HorizontalAlignment = 1;
                        PdfDetailCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        PdfDetailCell.Colspan = 8;
                        PdfDetailTable.AddCell(PdfDetailCell);
                        PdfDetailCell = new PdfPCell(new Phrase("Sign Acc. Holder", fontAb11BU));
                        PdfDetailCell.NoWrap = false;
                        PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell.HorizontalAlignment = 1;
                        PdfDetailCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        PdfDetailCell.Colspan = 9;
                        PdfDetailTable.AddCell(PdfDetailCell);
                        //PdfDetailCell = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfDetailCell.NoWrap = false;
                        //PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        //PdfDetailCell.HorizontalAlignment = 1;
                        //PdfDetailCell.Border = Rectangle.NO_BORDER;
                        //PdfDetailCell.Colspan = 34;
                        //PdfDetailTable.AddCell(PdfDetailCell);


                        PdfDetailCell = new PdfPCell(new Phrase(" ", fontAb11B));
                        PdfDetailCell.NoWrap = false;
                        PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell.HorizontalAlignment = 1;
                        PdfDetailCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        PdfDetailCell.Colspan = 9;
                        PdfDetailTable.AddCell(PdfDetailCell);

                        PdfDetailCell = new PdfPCell(new Phrase(dr["BenificiaryName"].ToString(), fontAb11));
                        PdfDetailCell.NoWrap = false;
                        PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell.HorizontalAlignment = 1;
                        PdfDetailCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        PdfDetailCell.Colspan = 9;
                        PdfDetailTable.AddCell(PdfDetailCell);

                        PdfDetailCell = new PdfPCell(new Phrase("", fontAb11));
                        PdfDetailCell.NoWrap = false;
                        PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell.HorizontalAlignment = 1;
                        PdfDetailCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        PdfDetailCell.Colspan = 8;
                        PdfDetailTable.AddCell(PdfDetailCell);

                        PdfDetailCell = new PdfPCell(new Phrase("", fontAb11));
                        PdfDetailCell.NoWrap = false;
                        PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell.HorizontalAlignment = 1;
                        PdfDetailCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        PdfDetailCell.Colspan = 8;
                        PdfDetailTable.AddCell(PdfDetailCell);



                        PdfDetailCell = new PdfPCell(new Phrase(" ", fontAb11B));
                        PdfDetailCell.NoWrap = false;
                        PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell.HorizontalAlignment = 1;
                        PdfDetailCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        PdfDetailCell.Colspan = 9;
                        PdfDetailTable.AddCell(PdfDetailCell);
                        PdfDetailCell = new PdfPCell(new Phrase("1.Name as in Bank Records", fontAb11BU));
                        PdfDetailCell.NoWrap = false;
                        PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell.HorizontalAlignment = 1;
                        PdfDetailCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        PdfDetailCell.Colspan = 8;
                        PdfDetailTable.AddCell(PdfDetailCell);
                        PdfDetailCell = new PdfPCell(new Phrase("2.Name as in Bank Records", fontAb11BU));
                        PdfDetailCell.NoWrap = false;
                        PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell.HorizontalAlignment = 1;
                        PdfDetailCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        PdfDetailCell.Colspan = 8;
                        PdfDetailTable.AddCell(PdfDetailCell);
                        PdfDetailCell = new PdfPCell(new Phrase("3.Name as in Bank Records", fontAb11BU));
                        PdfDetailCell.NoWrap = false;
                        PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell.HorizontalAlignment = 1;
                        PdfDetailCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        PdfDetailCell.Colspan = 9;
                        PdfDetailTable.AddCell(PdfDetailCell);

                        PdfDetailCell = new PdfPCell(new Phrase(" ", fontText4));
                        PdfDetailCell.NoWrap = false;
                        PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell.HorizontalAlignment = 1;
                        PdfDetailCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        PdfDetailCell.Colspan = 34;
                        PdfDetailTable.AddCell(PdfDetailCell);

                        PdfDetailCell = new PdfPCell(new Phrase("This is to confirm that declaration has been carefully read, understood & made by me/us. I'm authorizing the user entity/Corporate to debit my account, based on the instruction as agreed and signed by me. I've understood that I'm authorized to cancel/amend this mandate by appropriately communicating the cancellation/amendment request to the user/entity/corporate or the bank where I've authorized the debit.", fontText5));
                        PdfDetailCell.NoWrap = false;
                        PdfDetailCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell.Colspan = 34;
                        PdfDetailTable.AddCell(PdfDetailCell);

                        PdfPTable PdfHeaderTable1 = new PdfPTable(31);
                        PdfHeaderTable1.DefaultCell.NoWrap = false;
                        PdfHeaderTable1.SpacingBefore = 0;
                        PdfHeaderTable1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderTable1.DefaultCell.Border = PdfCell.NO_BORDER;
                        PdfHeaderTable1.WidthPercentage = 100;
                        float[] PdfHeaderTable1Headerwidths = new float[] { 4f, 3f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                        PdfHeaderTable1.SetWidths(PdfHeaderTable1Headerwidths);
                        PdfPCell PdfHeaderCell1 = null;
                        PdfHeaderCell1 = new PdfPCell(new Phrase("Status:Success/Failure/Response Awaited", fontAb9));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.Colspan = 34;
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                        Document document1 = new Document();
                        document1.Open();
                        Paragraph p1 = new Paragraph();
                        p1.Add(new Chunk(LogoImage, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));

                        PdfHeaderCell1 = new PdfPCell(p1);
                        PdfHeaderCell1.FixedHeight = 25f;
                        PdfHeaderCell1.Rowspan = 2;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfHeaderCell1 = new PdfPCell(new Phrase("UMRN", fontAb11B));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfHeaderCell1 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfHeaderCell1 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfHeaderCell1 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfHeaderCell1 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfHeaderCell1 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfHeaderCell1 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfHeaderCell1 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfHeaderCell1 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfHeaderCell1 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfHeaderCell1 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfHeaderCell1 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfHeaderCell1 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfHeaderCell1 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfHeaderCell1 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfHeaderCell1 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfHeaderCell1 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfHeaderCell1 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfHeaderCell1 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfHeaderCell1 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfHeaderCell1 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfHeaderCell1 = new PdfPCell(new Phrase("Date", fontAb11B));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        string Date1 = dr["SlipDate"].ToString();
                        char[] chars1 = new char[8];
                        chars1 = Date1.ToCharArray();
                        if (Convert.ToInt32(chars1.Length) > 0)
                        {
                            for (int j = 0; j < Convert.ToInt32(chars1.Length); j++)
                            {
                                PdfHeaderCell1 = new PdfPCell(new Phrase(chars1[j].ToString(), fontA119B));
                                PdfHeaderCell1.NoWrap = false;
                                PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfHeaderCell1.HorizontalAlignment = 1;
                                PdfHeaderTable1.AddCell(PdfHeaderCell1);
                            }
                        }
                        else
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                PdfHeaderCell1 = new PdfPCell(new Phrase(" ", fontA119B));
                                PdfHeaderCell1.NoWrap = false;
                                PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfHeaderCell1.HorizontalAlignment = 1;
                                PdfHeaderTable1.AddCell(PdfHeaderCell1);
                            }
                        }
                        PdfHeaderCell1 = new PdfPCell(new Phrase("Sponsor bankcode", fontAb11B));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.Colspan = 4;
                        PdfHeaderCell1.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfHeaderCell1 = new PdfPCell(new Phrase(dr["SponserBankCode"].ToString(), fontAb11B));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.Colspan = 12;
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfHeaderCell1 = new PdfPCell(new Phrase("Utility Code", fontAb11B));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.Colspan = 6;
                        PdfHeaderCell1.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfHeaderCell1 = new PdfPCell(new Phrase(dr["UtilityCode"].ToString(), fontAb11B));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.Colspan = 8;
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        Document documentCheckBox1 = new Document();
                        documentCheckBox1.Open();
                        Paragraph pCheckBox1 = new Paragraph();
                        //------------------------------- add Created Status-------------------------------------
                        string status1 = dr["CreatedStatus"].ToString();
                        if (status1 == "C")
                        {
                            pCheckBox1.Add(new Phrase("CREATE ", fontText));
                            pCheckBox1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBox1.Add(new Phrase(" MODIFY ", fontText));
                            pCheckBox1.Add(new Phrase(" CANCEL ", fontText));

                        }
                        else if (status1 == "M")
                        {
                            pCheckBox1.Add(new Phrase("CREATE ", fontText));

                            pCheckBox1.Add(new Phrase(" MODIFY ", fontText));
                            pCheckBox1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBox1.Add(new Phrase(" CANCEL ", fontText));

                        }
                        else if (status1 == "L")
                        {
                            pCheckBox1.Add(new Phrase("CREATE ", fontText));

                            pCheckBox1.Add(new Phrase(" MODIFY ", fontText));

                            pCheckBox1.Add(new Phrase(" CANCEL ", fontText));
                            pCheckBox1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                        }
                        PdfHeaderCell1 = new PdfPCell(pCheckBox1);
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfHeaderCell1 = new PdfPCell(new Phrase("I/We hereby Authorize", fontText));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                        PdfHeaderCell1.Colspan = 4;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfHeaderCell1 = new PdfPCell(new Phrase(dr["CompanyName"].ToString(), fontAb11B));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.FixedHeight = 20f;
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell1.Colspan = 10;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfHeaderCell1 = new PdfPCell(new Phrase("To Debit", fontAb11B));
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell1.Colspan = 5;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        Document documentCheckBoxSB1 = new Document();
                        documentCheckBoxSB1.Open();
                        Paragraph pCheckBoxSB1 = new Paragraph();

                        //----------------------------------add To Debit---------------------------
                        string chDebit1 = dr["DebitTo"].ToString();
                        if (chDebit1 == "SB")
                        {
                            pCheckBoxSB1.Add(new Phrase(" ", fontText));
                            pCheckBoxSB1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" SB/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" CA/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" CC/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" SB-NRE/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" SB-NRO/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" OTHER ", fontText));
                        }
                        else if (chDebit1 == "CA")
                        {
                            pCheckBoxSB1.Add(new Phrase(" ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" SB/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" CA/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" CC/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" SB-NRE/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" SB-NRO/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" OTHER ", fontText));
                        }

                        else if (chDebit1 == "CC")
                        {
                            pCheckBoxSB1.Add(new Phrase(" ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" SB/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" CA/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" CC/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" SB-NRE/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" SB-NRO/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" OTHER ", fontText));
                        }
                        else if (chDebit1 == "RE")
                        {
                            pCheckBoxSB1.Add(new Phrase(" ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" SB/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" CA/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" CC/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" SB-NRE/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" SB-NRO/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" OTHER ", fontText));
                        }
                        else if (chDebit1 == "RD")
                        {
                            pCheckBoxSB1.Add(new Phrase(" ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" SB/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" CA/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" CC/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" SB-NRE/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" SB-NRO/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" OTHER ", fontText));
                        }
                        else if (chDebit1 == "OT")
                        {
                            pCheckBoxSB1.Add(new Phrase(" ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" SB/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" CA/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" CC/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" SB-NRE/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" SB-NRO/ ", fontText));
                            pCheckBoxSB1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB1.Add(new Phrase(" OTHER ", fontText));
                        }
                        PdfHeaderCell1 = new PdfPCell(pCheckBoxSB1);
                        PdfHeaderCell1.NoWrap = false;
                        PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell1.Colspan = 11;
                        PdfHeaderCell1.HorizontalAlignment = 1;
                        PdfHeaderCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfHeaderTable1.AddCell(PdfHeaderCell1);
                        PdfPTable PdfMidTable1 = new PdfPTable(32);
                        PdfMidTable1.DefaultCell.NoWrap = false;
                        PdfMidTable1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidTable1.DefaultCell.Border = PdfCell.NO_BORDER;
                        PdfMidTable1.WidthPercentage = 100;
                        float[] PdfMidTable1Headerwidths = new float[] { 4f, 3f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                        PdfMidTable1.SetWidths(PdfMidTable1Headerwidths);
                        PdfPCell PdfMidCell1 = null;
                        PdfMidCell1 = new PdfPCell(new Phrase("Bank Account Number", fontAb11B));
                        PdfMidCell1.NoWrap = false;
                        PdfMidCell1.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell1.Colspan = 6;
                        PdfMidCell1.HorizontalAlignment = 1;
                        PdfMidTable1.AddCell(PdfMidCell1);
                        //---------------------------Add AccountNo-------------------------------------
                        string AccountNo1 = dr["AccountNo"].ToString();
                        char[] chrAcountNo1 = new char[Convert.ToInt32(AccountNo1.Length)];
                        chrAcountNo1 = AccountNo1.ToCharArray();

                        if (Convert.ToInt32(chrAcountNo1.Length) <= 26)
                        {
                            for (int j = 0; j < Convert.ToInt32(chrAcountNo1.Length); j++)
                            {
                                PdfMidCell1 = new PdfPCell(new Phrase(chrAcountNo1[j].ToString(), fontA119B));
                                PdfMidCell1.NoWrap = false;
                                PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfMidCell1.HorizontalAlignment = 1;
                                PdfMidTable1.AddCell(PdfMidCell1);
                            }
                            int len1 = 26 - Convert.ToInt32(AccountNo1.Length);
                            for (int k = 0; k < len1; k++)
                            {
                                PdfMidCell1 = new PdfPCell(new Phrase(" ", fontA119B));
                                PdfMidCell1.NoWrap = false;
                                PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfMidCell1.HorizontalAlignment = 1;
                                PdfMidTable1.AddCell(PdfMidCell1);
                            }

                        }
                        PdfMidCell1 = new PdfPCell(new Phrase("With Bank", fontAb11B));
                        PdfMidCell1.NoWrap = false;
                        PdfMidCell1.FixedHeight = 20f;
                        PdfMidCell1.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell1.HorizontalAlignment = 1;
                        PdfMidCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfMidTable1.AddCell(PdfMidCell1);

                        //PdfMidCell1 = new PdfPCell(new Phrase(dr["BankName"].ToString(), fontAb11));
                        //PdfMidCell1.NoWrap = false;
                        //PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        //PdfMidCell1.Colspan = 6;
                        //PdfMidCell1.HorizontalAlignment = 1;
                        //PdfMidTable1.AddCell(PdfMidCell1);

                        //char[] array12 = (Convert.ToString(dr["BankName"]).Trim()).ToCharArray();
                        //if (array12.Length < 22)
                        //{
                        PdfMidCell1 = new PdfPCell(new Phrase(dr["BankName"].ToString(), fontAb11));
                        PdfMidCell1.NoWrap = false;
                        PdfMidCell1.FixedHeight = 30f;

                        PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell1.Colspan = 6;
                        PdfMidCell1.HorizontalAlignment = 1;
                        PdfMidCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfMidTable1.AddCell(PdfMidCell1);
                        //}
                        //else
                        //{
                        //    PdfMidCell1 = new PdfPCell(new Phrase(dr["BankName"].ToString(), fontText));
                        //    PdfMidCell1.NoWrap = false;

                        //    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        //    PdfMidCell1.Colspan = 6;
                        //    PdfMidCell1.HorizontalAlignment = 1;
                        //    PdfMidTable1.AddCell(PdfMidCell1);
                        //}

                        PdfMidCell1 = new PdfPCell(new Phrase("IFSC", fontAb11B));
                        PdfMidCell1.NoWrap = false;
                        PdfMidCell1.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell1.Colspan = 2;
                        PdfMidCell1.HorizontalAlignment = 1;
                        PdfMidCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfMidTable1.AddCell(PdfMidCell1);
                        //-------------------------Add IFSC code--------------------------------
                        string IFSCcode1 = dr["IFSCcode"].ToString();
                        char[] chrIFSCcode1 = new char[Convert.ToInt32(IFSCcode.Length)];
                        chrIFSCcode1 = IFSCcode1.ToCharArray();
                        if (Convert.ToInt32(chrIFSCcode1.Length) > 0)
                        {
                            if (Convert.ToInt32(chrIFSCcode1.Length) == 11)
                            {
                                for (int j = 0; j < Convert.ToInt32(chrIFSCcode1.Length); j++)
                                {
                                    PdfMidCell1 = new PdfPCell(new Phrase(chrIFSCcode1[j].ToString(), fontA119B));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfMidTable1.AddCell(PdfMidCell1);
                                }
                            }
                            else
                            {
                                for (int j = 0; j < 11; j++)
                                {
                                    PdfMidCell1 = new PdfPCell(new Phrase(" ", fontA119B));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfMidTable1.AddCell(PdfMidCell1);
                                }
                            }

                        }
                        else
                        {
                            for (int j = 0; j < 11; j++)
                            {
                                PdfMidCell1 = new PdfPCell(new Phrase(" ", fontA119B));
                                PdfMidCell1.NoWrap = false;
                                PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfMidCell1.HorizontalAlignment = 1;
                                PdfMidCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                PdfMidTable1.AddCell(PdfMidCell1);
                            }
                        }
                        PdfMidCell1 = new PdfPCell(new Phrase("or MICR", fontAb11B));
                        PdfMidCell1.NoWrap = false;
                        PdfMidCell1.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell1.Colspan = 3;
                        PdfMidCell1.HorizontalAlignment = 1;
                        PdfMidCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfMidTable1.AddCell(PdfMidCell1);
                        //-------------------------Add MICRcode--------------------------------
                        string MICRcode1 = dr["MICRcode"].ToString();
                        char[] chrMICRcode1 = new char[9];
                        chrMICRcode1 = MICRcode1.ToCharArray();
                        if (true)
                        {
                            if (Convert.ToInt32(chrMICRcode.Length) == 9)
                            {
                                for (int j = 0; j < Convert.ToInt32(chrMICRcode1.Length); j++)
                                {
                                    PdfMidCell1 = new PdfPCell(new Phrase(chrMICRcode1[j].ToString(), fontA119B));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfMidTable1.AddCell(PdfMidCell1);
                                }
                            }
                            else
                            {
                                for (int j = 0; j < 9; j++)
                                {
                                    PdfMidCell1 = new PdfPCell(new Phrase(" ", fontA119B));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfMidTable1.AddCell(PdfMidCell1);
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < 9; j++)
                            {
                                PdfMidCell1 = new PdfPCell(new Phrase(" ", fontA119B));
                                PdfMidCell1.NoWrap = false;
                                PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfMidCell1.HorizontalAlignment = 1;
                                PdfMidCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                PdfMidTable1.AddCell(PdfMidCell1);
                            }
                        }
                        PdfMidCell1 = new PdfPCell(new Phrase("an amount of Rupees", fontAb11B));
                        PdfMidCell1.NoWrap = false;
                        PdfMidCell1.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell1.Colspan = 3;
                        PdfMidCell1.HorizontalAlignment = 1;
                        PdfMidTable1.AddCell(PdfMidCell1);
                        PdfMidCell1 = new PdfPCell(new Phrase(dr["AmountInWord"].ToString(), fontA11B));
                        PdfMidCell1.NoWrap = false;
                        PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell1.Colspan = 22;
                        PdfMidCell1.FixedHeight = 10f;
                        PdfMidCell1.HorizontalAlignment = 1;
                        PdfMidTable1.AddCell(PdfMidCell1);

                        Document documentAmountInDigit1 = new Document();
                        documentAmountInDigit1.Open();
                        Paragraph pAmountInDigit1 = new Paragraph();
                        pAmountInDigit1.Add(new Chunk(Rupee, PdfPCell.ALIGN_CENTER, PdfPCell.ALIGN_CENTER));
                        pAmountInDigit1.Add(new Phrase(" " + dr["AmountInDigit"].ToString(), fontA119B));
                        PdfMidCell1 = new PdfPCell(pAmountInDigit1);

                        PdfMidCell1.NoWrap = false;

                        PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell1.Colspan = 18;
                        PdfMidCell1.HorizontalAlignment = 1;
                        PdfMidTable1.AddCell(PdfMidCell1);


                        //Document documentAmountInDigit1 = new Document();
                        //documentAmountInDigit1.Open();
                        //Paragraph pAmountInDigit1 = new Paragraph();
                        //pAmountInDigit1.Add(new Chunk(Rupee, PdfPCell.ALIGN_CENTER, PdfPCell.ALIGN_CENTER));
                        //pAmountInDigit1.Add(new Phrase(" " + dr["AmountInDigit"].ToString(), fontAb11));
                        //PdfMidCell1 = new PdfPCell(pAmountInDigit1);

                        //PdfMidCell1.NoWrap = false;

                        //PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        //PdfMidCell1.Colspan = 13;

                        //PdfMidTable1.AddCell(PdfMidCell1);
                        string Freq1 = dr["Frequency"].ToString();
                        PdfMidCell1 = new PdfPCell(new Phrase("Frequency", fontAb11B));
                        PdfMidCell1.NoWrap = false;
                        PdfMidCell1.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell1.HorizontalAlignment = 1;
                        PdfMidTable1.AddCell(PdfMidCell1);
                        Document documentMonthly1 = new Document();
                        documentMonthly1.Open();
                        Paragraph pMonthly1 = new Paragraph();
                        //------------------------------- add Monthly-------------------------------------
                        if (Freq1 == "M")
                        {
                            pMonthly1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pMonthly1.Add(new Phrase("  Monthly ", fontText));
                        }
                        else
                        {
                            pMonthly1.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pMonthly1.Add(new Phrase("  Monthly ", fontText));
                        }

                        PdfMidCell1 = new PdfPCell(pMonthly1);
                        PdfMidCell1.NoWrap = false;
                        PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell1.Colspan = 2;
                        PdfMidCell1.HorizontalAlignment = 1;
                        PdfMidTable1.AddCell(PdfMidCell1);
                        Document documentQtly1 = new Document();
                        documentQtly1.Open();
                        Paragraph pQtly1 = new Paragraph();
                        //------------------------------- add Qtly-------------------------------------
                        if (Freq1 == "Q")
                        {
                            pQtly1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pQtly1.Add(new Phrase("  Qtly ", fontText));
                        }
                        else
                        {
                            pQtly1.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pQtly1.Add(new Phrase("  Qtly ", fontText));
                        }
                        PdfMidCell1 = new PdfPCell(pQtly1);
                        PdfMidCell1.NoWrap = false;
                        PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell1.Colspan = 2;
                        PdfMidCell1.HorizontalAlignment = 1;
                        PdfMidTable1.AddCell(PdfMidCell1);
                        Document documentHYrly1 = new Document();
                        documentHYrly1.Open();
                        Paragraph pHYrly1 = new Paragraph();
                        //------------------------------- add H-Yrly-------------------------------------
                        if (Freq1 == "H")
                        {
                            pHYrly1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pHYrly1.Add(new Phrase("  H-Yrly ", fontText));
                        }
                        else
                        {
                            pHYrly1.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pHYrly1.Add(new Phrase("  H-Yrly ", fontText));
                        }
                        PdfMidCell1 = new PdfPCell(pHYrly1);
                        PdfMidCell1.NoWrap = false;
                        PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell1.Colspan = 3;
                        PdfMidCell1.HorizontalAlignment = 1;
                        PdfMidTable1.AddCell(PdfMidCell1);
                        Document documentYearly1 = new Document();
                        documentYearly1.Open();
                        Paragraph pYearly1 = new Paragraph();
                        //------------------------------- add Yearly-------------------------------------
                        if (Freq == "Y")
                        {
                            pYearly1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pYearly1.Add(new Phrase("  Yearly ", fontText));
                        }
                        else
                        {
                            pYearly1.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pYearly1.Add(new Phrase("  Yearly ", fontText));
                        }

                        PdfMidCell1 = new PdfPCell(pYearly1);
                        PdfMidCell1.NoWrap = false;
                        PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell1.Colspan = 3;
                        PdfMidCell1.HorizontalAlignment = 1;
                        PdfMidTable1.AddCell(PdfMidCell1);


                        Document prensentedprensented11 = new Document();
                        prensentedprensented11.Open();
                        Paragraph prensented11 = new Paragraph();
                        if (Freq == "A")
                        {
                            prensented11.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            prensented11.Add(new Phrase("  As & when prensented ", fontText));
                        }
                        else
                        {
                            prensented11.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            prensented11.Add(new Phrase("  As & when prensented ", fontText));
                        }

                        PdfMidCell1 = new PdfPCell(prensented11);
                        PdfMidCell1.NoWrap = false;
                        PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell1.Colspan = 7;
                        PdfMidCell1.HorizontalAlignment = 1;
                        PdfMidTable1.AddCell(PdfMidCell1);


                        string DebitType1 = dr["DebitType"].ToString();
                        PdfMidCell1 = new PdfPCell(new Phrase("Debit Type", fontAb11B));
                        PdfMidCell1.NoWrap = false;
                        PdfMidCell1.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell1.Colspan = 3;
                        PdfMidCell1.HorizontalAlignment = 1;
                        PdfMidTable1.AddCell(PdfMidCell1);
                        Document documentFixed1 = new Document();
                        documentFixed1.Open();
                        Paragraph pFixed1 = new Paragraph();
                        //------------------------------- add H-Yrly-------------------------------------
                        if (DebitType1 == "F")
                        {
                            pFixed1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pFixed1.Add(new Phrase("  Fixed Amount ", fontText));
                        }
                        else
                        {
                            pFixed1.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pFixed1.Add(new Phrase("  Fixed Amount ", fontText));
                        }
                        PdfMidCell1 = new PdfPCell(pFixed1);
                        PdfMidCell1.NoWrap = false;
                        PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell1.Colspan = 5;
                        PdfMidCell1.HorizontalAlignment = 1;
                        PdfMidTable1.AddCell(PdfMidCell1);
                        Document documentMaximum1 = new Document();
                        documentMaximum1.Open();
                        Paragraph pMaximum1 = new Paragraph();
                        //------------------------------- add H-Yrly-------------------------------------
                        if (DebitType1 == "M")
                        {
                            pMaximum1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pMaximum1.Add(new Phrase("  Maximum Amount ", fontText));
                        }
                        else
                        {
                            pMaximum1.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pMaximum1.Add(new Phrase("  Maximum Amount ", fontText));
                        }

                        PdfMidCell1 = new PdfPCell(pMaximum1);
                        PdfMidCell1.NoWrap = false;
                        PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell1.Colspan = 6;
                        PdfMidCell1.HorizontalAlignment = 1;
                        PdfMidTable1.AddCell(PdfMidCell1);
                        PdfMidCell1 = new PdfPCell(new Phrase("Reference 1", fontAb11B));
                        PdfMidCell1.NoWrap = false;
                        PdfMidCell1.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell1.HorizontalAlignment = 1;
                        PdfMidTable1.AddCell(PdfMidCell1);
                        PdfMidCell1 = new PdfPCell(new Phrase(dr["Reference1"].ToString(), fontAb11B));
                        PdfMidCell1.NoWrap = false;
                        PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell1.Colspan = 15;
                        PdfMidCell1.HorizontalAlignment = 1;
                        PdfMidTable1.AddCell(PdfMidCell1);
                        PdfMidCell1 = new PdfPCell(new Phrase("Phone Number ", fontAb11B));
                        PdfMidCell1.NoWrap = false;
                        PdfMidCell1.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell1.Colspan = 6;
                        PdfMidCell1.HorizontalAlignment = 1;
                        PdfMidTable1.AddCell(PdfMidCell1);
                        PdfMidCell1 = new PdfPCell(new Phrase(dr["PhoneNo"].ToString(), fontAb11B));
                        PdfMidCell1.NoWrap = false;
                        PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell1.Colspan = 10;
                        PdfMidCell1.HorizontalAlignment = 1;
                        PdfMidTable1.AddCell(PdfMidCell1);
                        PdfMidCell1 = new PdfPCell(new Phrase("Reference 2", fontAb11B));
                        PdfMidCell1.NoWrap = false;
                        PdfMidCell1.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell1.HorizontalAlignment = 1;
                        PdfMidTable1.AddCell(PdfMidCell1);
                        PdfMidCell1 = new PdfPCell(new Phrase(dr["Reference2"].ToString(), fontAb11B));
                        PdfMidCell1.NoWrap = false;
                        PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell1.Colspan = 15;
                        PdfMidCell1.HorizontalAlignment = 1;
                        PdfMidTable1.AddCell(PdfMidCell1);
                        PdfMidCell1 = new PdfPCell(new Phrase("EMail ID", fontAb11B));
                        PdfMidCell1.NoWrap = false;
                        PdfMidCell1.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell1.Colspan = 6;
                        PdfMidCell1.HorizontalAlignment = 1;
                        PdfMidTable1.AddCell(PdfMidCell1);
                        PdfMidCell1 = new PdfPCell(new Phrase(dr["EmailId"].ToString(), fontAb11B));
                        PdfMidCell1.NoWrap = false;
                        PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell1.Colspan = 10;
                        PdfMidCell1.HorizontalAlignment = 1;
                        PdfMidTable1.AddCell(PdfMidCell1);
                        PdfMidCell1 = new PdfPCell(new Phrase("I agree for the debit of mandate processing charges by the bank whom I am authorizing to debit my account as per latest schedule of charges of bank ", fontText));
                        PdfMidCell1.NoWrap = false;
                        PdfMidCell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell1.Colspan = 32;
                        PdfMidCell1.HorizontalAlignment = 1;
                        PdfMidTable1.AddCell(PdfMidCell1);
                        PdfMidCell1 = new PdfPCell(new Phrase("PERIOD", fontAb11B));
                        PdfMidCell1.NoWrap = false;
                        PdfMidCell1.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell1.HorizontalAlignment = 1;
                        PdfMidTable1.AddCell(PdfMidCell1);
                        PdfMidCell1 = new PdfPCell(new Phrase("", fontAb11B));
                        PdfMidCell1.NoWrap = false;
                        PdfMidCell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell1.Colspan = 31;
                        PdfMidCell1.HorizontalAlignment = 1;
                        PdfMidTable1.AddCell(PdfMidCell1);
                        PdfPTable PdfDetailTable1 = new PdfPTable(34);
                        PdfDetailTable1.DefaultCell.NoWrap = false;
                        PdfDetailTable1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailTable1.DefaultCell.Border = PdfCell.NO_BORDER;
                        PdfDetailTable1.WidthPercentage = 100;
                        float[] PdfDetailTable1Headerwidths1 = new float[] { 4f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                        PdfDetailTable1.SetWidths(PdfDetailTable1Headerwidths1);
                        PdfPCell PdfDetailCell1 = null;
                        PdfDetailCell1 = new PdfPCell(new Phrase("From", fontAb11B));
                        PdfDetailCell1.NoWrap = false;
                        PdfDetailCell1.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell1.HorizontalAlignment = 1;
                        PdfDetailTable1.AddCell(PdfDetailCell1);
                        string PeriodFrom1 = dr["PeriodFrom"].ToString();
                        char[] chrPeriodFrom1 = new char[8];
                        chrPeriodFrom1 = PeriodFrom1.ToCharArray();
                        if (Convert.ToInt32(chrPeriodFrom1.Length) > 0)
                        {
                            for (int j = 0; j < Convert.ToInt32(chrPeriodFrom1.Length); j++)
                            {
                                PdfDetailCell1 = new PdfPCell(new Phrase(chrPeriodFrom1[j].ToString(), fontA119B));
                                PdfDetailCell1.NoWrap = false;
                                PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell1.HorizontalAlignment = 1;
                                PdfDetailTable1.AddCell(PdfDetailCell1);
                            }
                        }
                        else
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                PdfDetailCell1 = new PdfPCell(new Phrase(" ", fontA119B));
                                PdfDetailCell1.NoWrap = false;
                                PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell1.HorizontalAlignment = 1;
                                PdfDetailTable1.AddCell(PdfDetailCell1);
                            }
                        }
                        PdfDetailCell1 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfDetailCell1.NoWrap = false;
                        PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell1.HorizontalAlignment = 1;
                        PdfDetailCell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        PdfDetailCell1.Colspan = 25;
                        PdfDetailTable1.AddCell(PdfDetailCell1);
                        PdfDetailCell1 = new PdfPCell(new Phrase("To*", fontAb11B));
                        PdfDetailCell1.NoWrap = false;
                        PdfDetailCell1.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell1.HorizontalAlignment = 1;
                        PdfDetailTable1.AddCell(PdfDetailCell1);
                        string PeriodTo1 = dr["PeriodTo"].ToString();
                        char[] chrPeriodTo1 = new char[8];
                        chrPeriodTo1 = PeriodTo1.ToCharArray();
                        if (Convert.ToInt32(chrPeriodTo1.Length) > 0)
                        {
                            if (dr["PeriodTo"].ToString() != "01011900")
                            {
                                for (int j = 0; j < Convert.ToInt32(chrPeriodTo.Length); j++)
                                {
                                    PdfDetailCell1 = new PdfPCell(new Phrase(chrPeriodTo[j].ToString(), fontA119B));
                                    PdfDetailCell1.NoWrap = false;
                                    PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell1.HorizontalAlignment = 1;
                                    PdfDetailTable1.AddCell(PdfDetailCell1);
                                }
                            }
                            else
                            {
                                for (int j = 0; j < 8; j++)
                                {
                                    PdfDetailCell1 = new PdfPCell(new Phrase(" ", fontA119B));
                                    PdfDetailCell1.NoWrap = false;
                                    PdfDetailCell1.BackgroundColor = new iTextSharp.text.Color(0, 0, 0);
                                    PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell1.HorizontalAlignment = 1;
                                    PdfDetailTable1.AddCell(PdfDetailCell1);
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                PdfDetailCell1 = new PdfPCell(new Phrase(" ", fontA119B));
                                PdfDetailCell1.NoWrap = false;
                                PdfDetailCell1.BackgroundColor = new iTextSharp.text.Color(0, 0, 0);
                                PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell1.HorizontalAlignment = 1;
                                PdfDetailTable1.AddCell(PdfDetailCell1);
                            }
                        }
                        PdfDetailCell1 = new PdfPCell(new Phrase(" ", fontAb11B));
                        PdfDetailCell1.NoWrap = false;
                        PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell1.HorizontalAlignment = 1;
                        PdfDetailCell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        PdfDetailCell1.Colspan = 25;
                        PdfDetailTable1.AddCell(PdfDetailCell1);
                        PdfDetailCell1 = new PdfPCell(new Phrase("Or", fontAb11B));
                        PdfDetailCell1.NoWrap = false;
                        PdfDetailCell1.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell1.HorizontalAlignment = 1;
                        PdfDetailTable1.AddCell(PdfDetailCell1);

                        Document documentCheckBox1234 = new Document();
                        documentCheckBox1234.Open();
                        Paragraph pCheckBox1234 = new Paragraph();
                        if (dr["PeriodTo"].ToString() == "01011900")
                        {
                            pCheckBox1234.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                        }
                        else
                        {
                            pCheckBox1234.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                        }

                        pCheckBox1234.Add(new Phrase(" Until Cancelled ", fontAb11));


                        PdfDetailCell1 = new PdfPCell(pCheckBox1234);

                        PdfDetailCell1.NoWrap = false;
                        PdfDetailCell1.Colspan = 8;
                        PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell1.HorizontalAlignment = 1;
                        PdfDetailTable1.AddCell(PdfDetailCell1);


                        PdfDetailCell1 = new PdfPCell(new Phrase("Sign. Primary Acc. Holder", fontAb11BU));
                        PdfDetailCell1.NoWrap = false;
                        PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell1.HorizontalAlignment = 1;
                        PdfDetailCell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        PdfDetailCell1.Colspan = 8;
                        PdfDetailTable1.AddCell(PdfDetailCell1);
                        PdfDetailCell1 = new PdfPCell(new Phrase("Sign Acc. Holder", fontAb11BU));
                        PdfDetailCell1.NoWrap = false;
                        PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell1.HorizontalAlignment = 1;
                        PdfDetailCell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        PdfDetailCell1.Colspan = 8;
                        PdfDetailTable1.AddCell(PdfDetailCell1);
                        PdfDetailCell1 = new PdfPCell(new Phrase("Sign Acc. Holder", fontAb11BU));
                        PdfDetailCell1.NoWrap = false;
                        PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell1.HorizontalAlignment = 1;
                        PdfDetailCell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        PdfDetailCell1.Colspan = 9;
                        PdfDetailTable1.AddCell(PdfDetailCell1);

                        //PdfDetailCell1 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfDetailCell1.NoWrap = false;
                        //PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        //PdfDetailCell1.HorizontalAlignment = 1;
                        //PdfDetailCell1.Border = Rectangle.NO_BORDER;

                        //PdfDetailCell1.Colspan = 34;
                        //PdfDetailTable1.AddCell(PdfDetailCell1);



                        PdfDetailCell1 = new PdfPCell(new Phrase(" ", fontAb11B));
                        PdfDetailCell1.NoWrap = false;
                        PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell1.HorizontalAlignment = 1;
                        PdfDetailCell1.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        PdfDetailCell1.Colspan = 9;
                        PdfDetailTable1.AddCell(PdfDetailCell1);

                        PdfDetailCell1 = new PdfPCell(new Phrase(dr["BenificiaryName"].ToString(), fontAb11));
                        PdfDetailCell1.NoWrap = false;
                        PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell1.HorizontalAlignment = 1;
                        PdfDetailCell1.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        PdfDetailCell1.Colspan = 9;
                        PdfDetailTable1.AddCell(PdfDetailCell1);

                        PdfDetailCell1 = new PdfPCell(new Phrase("", fontAb11));
                        PdfDetailCell1.NoWrap = false;
                        PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell1.HorizontalAlignment = 1;
                        PdfDetailCell1.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        PdfDetailCell1.Colspan = 8;
                        PdfDetailTable1.AddCell(PdfDetailCell1);

                        PdfDetailCell1 = new PdfPCell(new Phrase("", fontAb11));
                        PdfDetailCell1.NoWrap = false;
                        PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell1.HorizontalAlignment = 1;
                        PdfDetailCell1.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        PdfDetailCell1.Colspan = 8;
                        PdfDetailTable1.AddCell(PdfDetailCell1);


                        PdfDetailCell1 = new PdfPCell(new Phrase(" ", fontAb11B));
                        PdfDetailCell1.NoWrap = false;
                        PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell1.HorizontalAlignment = 1;
                        PdfDetailCell1.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        PdfDetailCell1.Colspan = 9;
                        PdfDetailTable1.AddCell(PdfDetailCell1);

                        PdfDetailCell1 = new PdfPCell(new Phrase("1.Name as in Bank Records", fontAb11BU));
                        PdfDetailCell1.NoWrap = false;
                        PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell1.HorizontalAlignment = 1;
                        PdfDetailCell1.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        PdfDetailCell1.Colspan = 8;
                        PdfDetailTable1.AddCell(PdfDetailCell1);

                        PdfDetailCell1 = new PdfPCell(new Phrase("2.Name as in Bank Records", fontAb11BU));
                        PdfDetailCell1.NoWrap = false;
                        PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell1.HorizontalAlignment = 1;
                        PdfDetailCell1.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        PdfDetailCell1.Colspan = 8;
                        PdfDetailTable1.AddCell(PdfDetailCell1);

                        PdfDetailCell1 = new PdfPCell(new Phrase("3.Name as in Bank Records", fontAb11BU));
                        PdfDetailCell1.NoWrap = false;
                        PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell1.HorizontalAlignment = 1;
                        PdfDetailCell1.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        PdfDetailCell1.Colspan = 9;
                        PdfDetailTable1.AddCell(PdfDetailCell1);


                        PdfDetailCell1 = new PdfPCell(new Phrase(" ", fontText4));
                        PdfDetailCell1.NoWrap = false;
                        PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell1.HorizontalAlignment = 1;
                        PdfDetailCell1.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        PdfDetailCell1.Colspan = 34;
                        PdfDetailTable1.AddCell(PdfDetailCell1);


                        PdfDetailCell1 = new PdfPCell(new Phrase("This is to confirm that declaration has been carefully read, understood & made by me/us. I'm authorizing the user entity/Corporate to debit my account, based on the instruction as agreed and signed by me. I've understood that I'm authorized to cancel/amend this mandate by appropriately communicating the cancellation/amendment request to the user/entity/corporate or the bank where I've authorized the debit.", fontText5));
                        PdfDetailCell1.NoWrap = false;
                        PdfDetailCell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell1.Colspan = 34;

                        PdfDetailTable1.AddCell(PdfDetailCell1);

                        PdfPTable PdfHeaderTable2 = new PdfPTable(31);
                        PdfHeaderTable2.DefaultCell.NoWrap = false;
                        PdfHeaderTable2.SpacingBefore = 0;
                        PdfHeaderTable2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderTable2.DefaultCell.Border = PdfCell.NO_BORDER;
                        PdfHeaderTable2.WidthPercentage = 100;
                        float[] PdfHeaderTable2Headerwidths = new float[] { 4f, 3f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                        PdfHeaderTable2.SetWidths(PdfHeaderTable2Headerwidths);




                        PdfPCell PdfHeaderCell2 = null;

                        PdfHeaderCell2 = new PdfPCell(new Phrase("Status:Success/Failure/Response Awaited", fontAb9));
                        PdfHeaderCell2.NoWrap = false;
                        PdfHeaderCell2.Colspan = 34;
                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_RTL;



                        Document document2 = new Document();
                        document2.Open();
                        Paragraph p2 = new Paragraph();
                        p2.Add(new Chunk(LogoImage, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                        PdfHeaderCell2 = new PdfPCell(p2);
                        PdfHeaderCell2.FixedHeight = 25f;
                        PdfHeaderCell2.Rowspan = 2;


                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);

                        PdfHeaderCell2 = new PdfPCell(new Phrase("UMRN", fontAb11B));
                        PdfHeaderCell2.NoWrap = false;
                        PdfHeaderCell2.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;




                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);

                        PdfHeaderCell2 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell2.NoWrap = false;

                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);
                        PdfHeaderCell2 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell2.NoWrap = false;

                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);
                        PdfHeaderCell2 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell2.NoWrap = false;

                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);
                        PdfHeaderCell2 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell2.NoWrap = false;

                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);
                        PdfHeaderCell2 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell2.NoWrap = false;

                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);
                        PdfHeaderCell2 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell2.NoWrap = false;

                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);
                        PdfHeaderCell2 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell2.NoWrap = false;

                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);
                        PdfHeaderCell2 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell2.NoWrap = false;

                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);
                        PdfHeaderCell2 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell2.NoWrap = false;

                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);
                        PdfHeaderCell2 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell2.NoWrap = false;

                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);
                        PdfHeaderCell2 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell2.NoWrap = false;

                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);
                        PdfHeaderCell2 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell2.NoWrap = false;

                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);
                        PdfHeaderCell2 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell2.NoWrap = false;

                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);
                        PdfHeaderCell2 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell2.NoWrap = false;

                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);
                        PdfHeaderCell2 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell2.NoWrap = false;

                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);
                        PdfHeaderCell2 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell2.NoWrap = false;

                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);
                        PdfHeaderCell2 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell2.NoWrap = false;

                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);
                        PdfHeaderCell2 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell2.NoWrap = false;

                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);
                        PdfHeaderCell2 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell2.NoWrap = false;

                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);
                        PdfHeaderCell2 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfHeaderCell2.NoWrap = false;

                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);



                        PdfHeaderCell2 = new PdfPCell(new Phrase("Date", fontAb11B));
                        PdfHeaderCell2.NoWrap = false;

                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell2.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);

                        string Date2 = dr["SlipDate"].ToString();
                        char[] chars2 = new char[8];
                        chars2 = Date2.ToCharArray();
                        if (Convert.ToInt32(chars2.Length) > 0)
                        {
                            for (int j = 0; j < Convert.ToInt32(chars2.Length); j++)
                            {
                                PdfHeaderCell2 = new PdfPCell(new Phrase(chars2[j].ToString(), fontA119B));
                                PdfHeaderCell2.NoWrap = false;
                                PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfHeaderCell2.HorizontalAlignment = 1;
                                PdfHeaderTable2.AddCell(PdfHeaderCell2);
                            }
                        }
                        else
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                PdfHeaderCell2 = new PdfPCell(new Phrase(" ", fontA119B));
                                PdfHeaderCell2.NoWrap = false;
                                PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfHeaderCell2.HorizontalAlignment = 1;
                                PdfHeaderTable2.AddCell(PdfHeaderCell2);
                            }
                        }


                        //PdfHeaderCell2 = new PdfPCell(new Phrase("d", fontAb11));
                        //PdfHeaderCell2.NoWrap = false;

                        //PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfHeaderCell2.HorizontalAlignment = 1;
                        //PdfHeaderTable2.AddCell(PdfHeaderCell2);

                        //PdfHeaderCell2 = new PdfPCell(new Phrase("d", fontAb11));
                        //PdfHeaderCell2.NoWrap = false;

                        //PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfHeaderCell2.HorizontalAlignment = 1;
                        //PdfHeaderTable2.AddCell(PdfHeaderCell2);

                        //PdfHeaderCell2 = new PdfPCell(new Phrase("m", fontAb11));
                        //PdfHeaderCell2.NoWrap = false;

                        //PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfHeaderCell2.HorizontalAlignment = 1;
                        //PdfHeaderTable2.AddCell(PdfHeaderCell2);
                        //PdfHeaderCell2 = new PdfPCell(new Phrase("m", fontAb11));
                        //PdfHeaderCell2.NoWrap = false;

                        //PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfHeaderCell2.HorizontalAlignment = 1;
                        //PdfHeaderTable2.AddCell(PdfHeaderCell2);
                        //PdfHeaderCell2 = new PdfPCell(new Phrase("y", fontAb11));
                        //PdfHeaderCell2.NoWrap = false;

                        //PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfHeaderCell2.HorizontalAlignment = 1;
                        //PdfHeaderTable2.AddCell(PdfHeaderCell2);
                        //PdfHeaderCell2 = new PdfPCell(new Phrase("y", fontAb11));
                        //PdfHeaderCell2.NoWrap = false;

                        //PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfHeaderCell2.HorizontalAlignment = 1;
                        //PdfHeaderTable2.AddCell(PdfHeaderCell2);

                        //PdfHeaderCell2 = new PdfPCell(new Phrase("y", fontAb11));
                        //PdfHeaderCell2.NoWrap = false;

                        //PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfHeaderCell2.HorizontalAlignment = 1;
                        //PdfHeaderTable2.AddCell(PdfHeaderCell2);
                        //PdfHeaderCell2 = new PdfPCell(new Phrase("y", fontAb11));
                        //PdfHeaderCell2.NoWrap = false;

                        //PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfHeaderCell2.HorizontalAlignment = 1;
                        //PdfHeaderTable2.AddCell(PdfHeaderCell2);


                        PdfHeaderCell2 = new PdfPCell(new Phrase("Sponsor bankcode", fontAb11B));
                        PdfHeaderCell2.NoWrap = false;
                        PdfHeaderCell2.Colspan = 4;
                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell2.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);


                        PdfHeaderCell2 = new PdfPCell(new Phrase(dr["SponserBankCode"].ToString(), fontAb11B));
                        PdfHeaderCell2.NoWrap = false;
                        PdfHeaderCell2.Colspan = 12;
                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);


                        PdfHeaderCell2 = new PdfPCell(new Phrase("Utility Code", fontAb11B));
                        PdfHeaderCell2.NoWrap = false;
                        PdfHeaderCell2.Colspan = 6;
                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell2.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);


                        PdfHeaderCell2 = new PdfPCell(new Phrase(dr["UtilityCode"].ToString(), fontAb11B));
                        PdfHeaderCell2.NoWrap = false;
                        PdfHeaderCell2.Colspan = 8;
                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_RTL;

                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);


                        //PdfHeaderCell2 = new PdfPCell(new Phrase("CREATE MODIFY CANCEL", fontAb11B));
                        //PdfHeaderCell2.NoWrap = false;

                        //PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfHeaderCell2.HorizontalAlignment = 1;
                        //PdfHeaderTable2.AddCell(PdfHeaderCell2);

                        Document documentCheckBox2 = new Document();
                        documentCheckBox2.Open();
                        Paragraph pCheckBox2 = new Paragraph();
                        //------------------------------- add Created Status-------------------------------------
                        string status2 = dr["CreatedStatus"].ToString();
                        if (status2 == "C")
                        {
                            pCheckBox2.Add(new Phrase("CREATE ", fontText));
                            pCheckBox2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBox2.Add(new Phrase(" MODIFY ", fontText));
                            pCheckBox2.Add(new Phrase(" CANCEL ", fontText));

                        }
                        else if (status2 == "M")
                        {
                            pCheckBox2.Add(new Phrase("CREATE ", fontText));

                            pCheckBox2.Add(new Phrase(" MODIFY ", fontText));
                            pCheckBox2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBox2.Add(new Phrase(" CANCEL ", fontText));

                        }
                        else if (status2 == "L")
                        {
                            pCheckBox2.Add(new Phrase("CREATE ", fontText));

                            pCheckBox2.Add(new Phrase(" MODIFY ", fontText));

                            pCheckBox2.Add(new Phrase(" CANCEL ", fontText));
                            pCheckBox2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                        }

                        //pCheckBox.Add(new Phrase("CREATE ", fontAb11B));
                        //pCheckBox.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                        //pCheckBox.Add(new Phrase(" MODIFY ", fontAb11B));
                        //pCheckBox.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                        //pCheckBox.Add(new Phrase(" CANCEL ", fontAb11B));ss
                        //pCheckBox.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                        PdfHeaderCell2 = new PdfPCell(pCheckBox2);
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);

                        PdfHeaderCell2 = new PdfPCell(new Phrase("I/We hereby Authorize", fontText));
                        PdfHeaderCell2.NoWrap = false;
                        PdfHeaderCell2.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                        PdfHeaderCell2.Colspan = 4;
                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);


                        PdfHeaderCell2 = new PdfPCell(new Phrase(dr["CompanyName"].ToString(), fontAb11B));
                        PdfHeaderCell2.NoWrap = false;
                        PdfHeaderCell2.FixedHeight = 20f;
                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell2.Colspan = 10;
                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);


                        PdfHeaderCell2 = new PdfPCell(new Phrase("To Debit", fontAb11B));
                        PdfHeaderCell2.NoWrap = false;
                        PdfHeaderCell2.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell2.Colspan = 5;
                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);

                        Document documentCheckBoxSB2 = new Document();
                        documentCheckBoxSB2.Open();
                        Paragraph pCheckBoxSB2 = new Paragraph();

                        //----------------------------------add To Debit---------------------------
                        string chDebit2 = dr["DebitTo"].ToString();
                        if (chDebit1 == "SB")
                        {
                            pCheckBoxSB2.Add(new Phrase(" ", fontText));
                            pCheckBoxSB2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" SB/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" CA/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" CC/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" SB-NRE/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" SB-NRO/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" OTHER ", fontText));
                        }
                        else if (chDebit1 == "CA")
                        {
                            pCheckBoxSB2.Add(new Phrase(" ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" SB/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" CA/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" CC/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" SB-NRE/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" SB-NRO/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" OTHER ", fontText));
                        }

                        else if (chDebit1 == "CC")
                        {
                            pCheckBoxSB2.Add(new Phrase(" ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" SB/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" CA/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" CC/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" SB-NRE/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" SB-NRO/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" OTHER ", fontText));
                        }
                        else if (chDebit1 == "RE")
                        {
                            pCheckBoxSB2.Add(new Phrase(" ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" SB/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" CA/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" CC/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" SB-NRE/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" SB-NRO/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" OTHER ", fontText));
                        }
                        else if (chDebit1 == "RD")
                        {
                            pCheckBoxSB2.Add(new Phrase(" ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" SB/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" CA/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" CC/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" SB-NRE/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" SB-NRO/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" OTHER ", fontText));
                        }
                        else if (chDebit1 == "OT")
                        {
                            pCheckBoxSB2.Add(new Phrase(" ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" SB/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" CA/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" CC/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" SB-NRE/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" SB-NRO/ ", fontText));
                            pCheckBoxSB2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pCheckBoxSB2.Add(new Phrase(" OTHER ", fontText));
                        }
                        //pCheckBoxSB.Add(new Phrase("SB ", fontAb11B));
                        //pCheckBoxSB.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));

                        // pCheckBoxSB.Add(new Phrase(" /CA/CC/SB-NRE/SB-NRO/OTHER", fontAb11B));
                        PdfHeaderCell2 = new PdfPCell(pCheckBoxSB2);



                        //  PdfHeaderCell2 = new PdfPCell(new Phrase("SB/CA/CC/SB-NRE/SB-NRO/OTHER", fontAb11B));
                        PdfHeaderCell2.NoWrap = false;

                        PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfHeaderCell2.Colspan = 11;
                        PdfHeaderCell2.HorizontalAlignment = 1;
                        PdfHeaderCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfHeaderTable2.AddCell(PdfHeaderCell2);


                        PdfPTable PdfMidTable2 = new PdfPTable(32);
                        PdfMidTable2.DefaultCell.NoWrap = false;

                        PdfMidTable2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfMidTable2.DefaultCell.Border = PdfCell.NO_BORDER;
                        PdfMidTable2.WidthPercentage = 100;
                        float[] PdfMidTable2Headerwidths = new float[] { 4f, 3f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                        PdfMidTable2.SetWidths(PdfMidTable2Headerwidths);




                        PdfPCell PdfMidCell2 = null;






                        PdfMidCell2 = new PdfPCell(new Phrase("Bank Account Number", fontAb11B));
                        PdfMidCell2.NoWrap = false;
                        PdfMidCell2.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell2.Colspan = 6;
                        PdfMidCell2.HorizontalAlignment = 1;
                        PdfMidTable2.AddCell(PdfMidCell2);

                        string AccountNo2 = dr["AccountNo"].ToString();
                        char[] chrAcountNo2 = new char[Convert.ToInt32(AccountNo2.Length)];
                        chrAcountNo2 = AccountNo2.ToCharArray();
                        if (Convert.ToInt32(AccountNo2.Length) <= 26)
                        {
                            for (int j = 0; j < Convert.ToInt32(chrAcountNo2.Length); j++)
                            {
                                PdfMidCell2 = new PdfPCell(new Phrase(chrAcountNo2[j].ToString(), fontA119B));
                                PdfMidCell2.NoWrap = false;
                                PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfMidCell2.HorizontalAlignment = 1;
                                PdfMidTable2.AddCell(PdfMidCell2);
                            }
                            int len2 = 26 - Convert.ToInt32(AccountNo2.Length);
                            for (int k = 0; k < len2; k++)
                            {
                                PdfMidCell2 = new PdfPCell(new Phrase(" ", fontA119B));
                                PdfMidCell2.NoWrap = false;
                                PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfMidCell2.HorizontalAlignment = 1;
                                PdfMidTable2.AddCell(PdfMidCell2);
                            }
                        }





                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);



                        PdfMidCell2 = new PdfPCell(new Phrase("With Bank", fontAb11B));
                        PdfMidCell2.NoWrap = false;
                        PdfMidCell2.FixedHeight = 20f;
                        PdfMidCell2.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfMidCell2.HorizontalAlignment = 1;
                        PdfMidCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfMidTable2.AddCell(PdfMidCell2);


                        char[] array = (Convert.ToString(dr["BankName"]).Trim()).ToCharArray();
                        //if (array.Length < 22)
                        //{
                        PdfMidCell2 = new PdfPCell(new Phrase(dr["BankName"].ToString(), fontAb11));
                        PdfMidCell2.NoWrap = false;
                        PdfMidCell2.FixedHeight = 30f;
                        PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell2.Colspan = 6;
                        PdfMidCell2.HorizontalAlignment = 1;
                        PdfMidCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfMidTable2.AddCell(PdfMidCell2);
                        //}
                        //else
                        //{
                        //    PdfMidCell2 = new PdfPCell(new Phrase(dr["BankName"].ToString(), fontText));
                        //    PdfMidCell2.NoWrap = false;

                        //    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        //    PdfMidCell2.Colspan = 6;
                        //    PdfMidCell2.HorizontalAlignment = 1;
                        //    PdfMidTable2.AddCell(PdfMidCell2);
                        //}


                        PdfMidCell2 = new PdfPCell(new Phrase("IFSC", fontAb11B));
                        PdfMidCell2.NoWrap = false;
                        PdfMidCell2.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell2.Colspan = 2;
                        PdfMidCell2.HorizontalAlignment = 1;
                        PdfMidCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfMidTable2.AddCell(PdfMidCell2);


                        //-------------------------Add IFSC code--------------------------------
                        string IFSCcode2 = dr["IFSCcode"].ToString();
                        char[] chrIFSCcode2 = new char[Convert.ToInt32(IFSCcode.Length)];
                        chrIFSCcode2 = IFSCcode2.ToCharArray();
                        if (Convert.ToInt32(chrIFSCcode2.Length) > 0)
                        {
                            if (Convert.ToInt32(chrIFSCcode2.Length) == 11)
                            {
                                for (int j = 0; j < Convert.ToInt32(chrIFSCcode2.Length); j++)
                                {
                                    PdfMidCell2 = new PdfPCell(new Phrase(chrIFSCcode2[j].ToString(), fontA119B));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfMidTable2.AddCell(PdfMidCell2);
                                }
                            }
                            else
                            {
                                for (int j = 0; j < 11; j++)
                                {
                                    PdfMidCell2 = new PdfPCell(new Phrase(" ", fontA119B));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfMidTable2.AddCell(PdfMidCell2);
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < 11; j++)
                            {
                                PdfMidCell2 = new PdfPCell(new Phrase(" ", fontA119B));
                                PdfMidCell2.NoWrap = false;
                                PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfMidCell2.HorizontalAlignment = 1;
                                PdfMidCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                PdfMidTable2.AddCell(PdfMidCell2);
                            }
                        }



                        //PdfMidCell2 = new PdfPCell(new Phrase("x", fontAb11));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase("x", fontAb11));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase("x", fontAb11));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase("x", fontAb11));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase("x", fontAb11));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase("x", fontAb11));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase("x", fontAb11));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase("x", fontAb11));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase("x", fontAb11));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);



                        PdfMidCell2 = new PdfPCell(new Phrase("or MICR", fontAb11B));
                        PdfMidCell2.NoWrap = false;
                        PdfMidCell2.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell2.Colspan = 3;
                        PdfMidCell2.HorizontalAlignment = 1;
                        PdfMidCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        PdfMidTable2.AddCell(PdfMidCell2);


                        //-------------------------Add MICRcode--------------------------------
                        string MICRcode2 = dr["MICRcode"].ToString();
                        char[] chrMICRcode2 = new char[9];
                        chrMICRcode2 = MICRcode2.ToCharArray();
                        if (Convert.ToInt32(chrMICRcode2.Length) > 0)
                        {
                            if (Convert.ToInt32(chrMICRcode.Length) == 9)
                            {
                                for (int j = 0; j < Convert.ToInt32(chrMICRcode2.Length); j++)
                                {
                                    PdfMidCell2 = new PdfPCell(new Phrase(chrMICRcode2[j].ToString(), fontA119B));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfMidTable2.AddCell(PdfMidCell2);
                                }
                            }
                            else
                            {
                                for (int j = 0; j < 9; j++)
                                {
                                    PdfMidCell2 = new PdfPCell(new Phrase(" ", fontA119B));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfMidTable2.AddCell(PdfMidCell2);
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < 9; j++)
                            {
                                PdfMidCell2 = new PdfPCell(new Phrase(" ", fontA119B));
                                PdfMidCell2.NoWrap = false;
                                PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfMidCell2.HorizontalAlignment = 1;
                                PdfMidCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                PdfMidTable2.AddCell(PdfMidCell2);
                            }
                        }



                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);
                        //PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);



                        PdfMidCell2 = new PdfPCell(new Phrase("an amount of Rupees", fontAb11B));
                        PdfMidCell2.NoWrap = false;
                        PdfMidCell2.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell2.Colspan = 3;
                        PdfMidCell2.HorizontalAlignment = 1;

                        PdfMidTable2.AddCell(PdfMidCell2);


                        PdfMidCell2 = new PdfPCell(new Phrase(dr["AmountInWord"].ToString(), fontA11B));
                        PdfMidCell2.NoWrap = false;
                        PdfMidCell2.FixedHeight = 10f;
                        PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell2.Colspan = 22;
                        PdfMidCell2.HorizontalAlignment = 1;
                        PdfMidTable2.AddCell(PdfMidCell2);

                        Document documentAmountInDigit2 = new Document();
                        documentAmountInDigit2.Open();
                        Paragraph pAmountInDigit2 = new Paragraph();
                        pAmountInDigit2.Add(new Chunk(Rupee, PdfPCell.ALIGN_CENTER, PdfPCell.ALIGN_CENTER));
                        pAmountInDigit2.Add(new Phrase(" " + dr["AmountInDigit"].ToString(), fontA119B));
                        PdfMidCell2 = new PdfPCell(pAmountInDigit2);

                        PdfMidCell2.NoWrap = false;

                        PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell2.Colspan = 18;
                        PdfMidCell2.HorizontalAlignment = 1;
                        PdfMidTable2.AddCell(PdfMidCell2);


                        string Freq2 = dr["Frequency"].ToString();

                        PdfMidCell2 = new PdfPCell(new Phrase("Frequency", fontAb11B));
                        PdfMidCell2.NoWrap = false;
                        PdfMidCell2.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfMidCell2.HorizontalAlignment = 1;
                        PdfMidTable2.AddCell(PdfMidCell2);


                        Document documentMonthly2 = new Document();
                        documentMonthly2.Open();
                        Paragraph pMonthly2 = new Paragraph();
                        //------------------------------- add Monthly-------------------------------------
                        if (Freq2 == "M")
                        {
                            pMonthly2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pMonthly2.Add(new Phrase("  Monthly ", fontText));
                        }
                        else
                        {
                            pMonthly2.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pMonthly2.Add(new Phrase("  Monthly ", fontText));
                        }

                        PdfMidCell2 = new PdfPCell(pMonthly2);

                        // PdfMidCell1 = new PdfPCell(new Phrase("Monthly", fontAb11));
                        PdfMidCell2.NoWrap = false;

                        PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell2.Colspan = 2;
                        PdfMidCell2.HorizontalAlignment = 1;
                        PdfMidTable2.AddCell(PdfMidCell2);

                        Document documentQtly2 = new Document();
                        documentQtly2.Open();
                        Paragraph pQtly2 = new Paragraph();
                        //------------------------------- add Qtly-------------------------------------
                        if (Freq2 == "Q")
                        {
                            pQtly2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pQtly2.Add(new Phrase("  Qtly ", fontText));
                        }
                        else
                        {
                            pQtly2.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pQtly2.Add(new Phrase("  Qtly ", fontText));
                        }

                        PdfMidCell2 = new PdfPCell(pQtly2);

                        // PdfMidCell1 = new PdfPCell(new Phrase("Qtly", fontAb11B));

                        PdfMidCell2.NoWrap = false;

                        PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell2.Colspan = 2;
                        PdfMidCell2.HorizontalAlignment = 1;
                        PdfMidTable2.AddCell(PdfMidCell2);

                        Document documentHYrly2 = new Document();
                        documentHYrly2.Open();
                        Paragraph pHYrly2 = new Paragraph();
                        //------------------------------- add H-Yrly-------------------------------------
                        if (Freq2 == "H")
                        {
                            pHYrly2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pHYrly2.Add(new Phrase("  H-Yrly ", fontText));
                        }
                        else
                        {
                            pHYrly2.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pHYrly2.Add(new Phrase("  H-Yrly ", fontText));
                        }

                        PdfMidCell2 = new PdfPCell(pHYrly2);

                        // PdfMidCell1 = new PdfPCell(new Phrase("H-Yrly", fontAb11B));
                        PdfMidCell2.NoWrap = false;

                        PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell2.Colspan = 3;
                        PdfMidCell2.HorizontalAlignment = 1;
                        PdfMidTable2.AddCell(PdfMidCell2);

                        Document documentYearly2 = new Document();
                        documentYearly2.Open();
                        Paragraph pYearly2 = new Paragraph();
                        //------------------------------- add Yearly-------------------------------------
                        if (Freq2 == "Y")
                        {
                            pYearly2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pYearly2.Add(new Phrase("  Yearly ", fontText));
                        }
                        else
                        {
                            pYearly2.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pYearly2.Add(new Phrase("  Yearly ", fontText));
                        }

                        PdfMidCell2 = new PdfPCell(pYearly2);

                        PdfMidCell2.NoWrap = false;
                        PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell2.Colspan = 3;
                        PdfMidCell2.HorizontalAlignment = 1;
                        PdfMidTable2.AddCell(PdfMidCell2);


                        Document prensentedprensented22 = new Document();
                        prensentedprensented22.Open();
                        Paragraph prensented22 = new Paragraph();
                        if (Freq == "A")
                        {
                            prensented22.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            prensented22.Add(new Phrase("  As & when prensented ", fontText));
                        }
                        else
                        {
                            prensented22.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            prensented22.Add(new Phrase("  As & when prensented ", fontText));
                        }

                        PdfMidCell2 = new PdfPCell(prensented22);
                        PdfMidCell2.NoWrap = false;
                        PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell2.Colspan = 7;
                        PdfMidCell2.HorizontalAlignment = 1;
                        PdfMidTable2.AddCell(PdfMidCell2);


                        //PdfMidCell2 = new PdfPCell(new Phrase("As & when prensented", fontAb11B));
                        //PdfMidCell2.NoWrap = false;

                        //PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        //PdfMidCell2.Colspan = 6;
                        //PdfMidCell2.HorizontalAlignment = 1;
                        //PdfMidTable2.AddCell(PdfMidCell2);

                        string DebitType2 = dr["DebitType"].ToString();

                        PdfMidCell2 = new PdfPCell(new Phrase("Debit Type", fontAb11B));
                        PdfMidCell2.NoWrap = false;
                        PdfMidCell2.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell2.Colspan = 3;
                        PdfMidCell2.HorizontalAlignment = 1;
                        PdfMidTable2.AddCell(PdfMidCell2);

                        Document documentFixed2 = new Document();
                        documentFixed2.Open();
                        Paragraph pFixed2 = new Paragraph();
                        //------------------------------- add H-Yrly-------------------------------------
                        if (DebitType2 == "F")
                        {
                            pFixed2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pFixed2.Add(new Phrase("  Fixed Amount ", fontText));
                        }
                        else
                        {
                            pFixed2.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pFixed2.Add(new Phrase("  Fixed Amount ", fontText));
                        }

                        PdfMidCell2 = new PdfPCell(pFixed2);
                        //   PdfMidCell1 = new PdfPCell(new Phrase("Fixed Amount", fontAb11B));
                        PdfMidCell2.NoWrap = false;
                        PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell2.Colspan = 5;
                        PdfMidCell2.HorizontalAlignment = 1;
                        PdfMidTable2.AddCell(PdfMidCell2);


                        Document documentMaximum2 = new Document();
                        documentMaximum2.Open();
                        Paragraph pMaximum2 = new Paragraph();
                        //------------------------------- add H-Yrly-------------------------------------
                        if (DebitType2 == "M")
                        {
                            pMaximum2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pMaximum2.Add(new Phrase("  Maximum Amount ", fontText));
                        }
                        else
                        {
                            pMaximum2.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                            pMaximum2.Add(new Phrase("  Maximum Amount ", fontText));
                        }

                        PdfMidCell2 = new PdfPCell(pMaximum2);
                        // PdfMidCell2 = new PdfPCell(new Phrase("Maximum Amount", fontAb11));
                        PdfMidCell2.NoWrap = false;

                        PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell2.Colspan = 6;
                        PdfMidCell2.HorizontalAlignment = 1;
                        PdfMidTable2.AddCell(PdfMidCell2);

                        PdfMidCell2 = new PdfPCell(new Phrase("Reference 1", fontAb11B));
                        PdfMidCell2.NoWrap = false;
                        PdfMidCell2.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfMidCell2.HorizontalAlignment = 1;
                        PdfMidTable2.AddCell(PdfMidCell2);


                        PdfMidCell2 = new PdfPCell(new Phrase(dr["Reference1"].ToString(), fontAb11B));
                        PdfMidCell2.NoWrap = false;

                        PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell2.Colspan = 15;
                        PdfMidCell2.HorizontalAlignment = 1;
                        PdfMidTable2.AddCell(PdfMidCell2);


                        PdfMidCell2 = new PdfPCell(new Phrase("Phone Number ", fontAb11B));
                        PdfMidCell2.NoWrap = false;
                        PdfMidCell2.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell2.Colspan = 6;
                        PdfMidCell2.HorizontalAlignment = 1;
                        PdfMidTable2.AddCell(PdfMidCell2);

                        PdfMidCell2 = new PdfPCell(new Phrase(dr["PhoneNo"].ToString(), fontAb11B));
                        PdfMidCell2.NoWrap = false;

                        PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell2.Colspan = 10;
                        PdfMidCell2.HorizontalAlignment = 1;
                        PdfMidTable2.AddCell(PdfMidCell2);


                        PdfMidCell2 = new PdfPCell(new Phrase("Reference 2", fontAb11B));
                        PdfMidCell2.NoWrap = false;
                        PdfMidCell2.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfMidCell2.HorizontalAlignment = 1;
                        PdfMidTable2.AddCell(PdfMidCell2);


                        PdfMidCell2 = new PdfPCell(new Phrase(dr["Reference2"].ToString(), fontAb11B));
                        PdfMidCell2.NoWrap = false;

                        PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell2.Colspan = 15;
                        PdfMidCell2.HorizontalAlignment = 1;
                        PdfMidTable2.AddCell(PdfMidCell2);


                        PdfMidCell2 = new PdfPCell(new Phrase("EMail ID", fontAb11B));
                        PdfMidCell2.NoWrap = false;
                        PdfMidCell2.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell2.Colspan = 6;
                        PdfMidCell2.HorizontalAlignment = 1;
                        PdfMidTable2.AddCell(PdfMidCell2);

                        PdfMidCell2 = new PdfPCell(new Phrase(dr["EmailId"].ToString(), fontAb11B));
                        PdfMidCell2.NoWrap = false;

                        PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell2.Colspan = 10;
                        PdfMidCell2.HorizontalAlignment = 1;
                        PdfMidTable2.AddCell(PdfMidCell2);


                        PdfMidCell2 = new PdfPCell(new Phrase("I agree for the debit of mandate processing charges by the bank whom I am authorizing to debit my account as per latest schedule of charges of bank ", fontText));
                        PdfMidCell2.NoWrap = false;
                        PdfMidCell2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell2.Colspan = 32;
                        PdfMidCell2.HorizontalAlignment = 1;
                        PdfMidTable2.AddCell(PdfMidCell2);


                        PdfMidCell2 = new PdfPCell(new Phrase("PERIOD", fontAb11B));
                        PdfMidCell2.NoWrap = false;
                        PdfMidCell2.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfMidCell2.HorizontalAlignment = 1;
                        PdfMidTable2.AddCell(PdfMidCell2);

                        PdfMidCell2 = new PdfPCell(new Phrase("", fontAb11B));
                        PdfMidCell2.NoWrap = false;
                        PdfMidCell2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfMidCell2.Colspan = 31;
                        PdfMidCell2.HorizontalAlignment = 1;
                        PdfMidTable2.AddCell(PdfMidCell2);



                        PdfPTable PdfDetailTable2 = new PdfPTable(34);
                        PdfDetailTable2.DefaultCell.NoWrap = false;

                        PdfDetailTable2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                        PdfDetailTable2.DefaultCell.Border = PdfCell.NO_BORDER;
                        PdfDetailTable2.WidthPercentage = 100;
                        float[] PdfDetailTable2Headerwidths1 = new float[] { 4f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                        PdfDetailTable2.SetWidths(PdfDetailTable2Headerwidths1);




                        PdfPCell PdfDetailCell2 = null;

                        PdfDetailCell2 = new PdfPCell(new Phrase("From", fontAb11B));
                        PdfDetailCell2.NoWrap = false;
                        PdfDetailCell2.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell2.HorizontalAlignment = 1;
                        PdfDetailTable2.AddCell(PdfDetailCell2);

                        string PeriodFrom2 = dr["PeriodFrom"].ToString();
                        char[] chrPeriodFrom2 = new char[8];
                        chrPeriodFrom2 = PeriodFrom2.ToCharArray();
                        if (Convert.ToInt32(chrPeriodFrom2.Length) > 0)
                        {
                            for (int j = 0; j < Convert.ToInt32(chrPeriodFrom2.Length); j++)
                            {
                                PdfDetailCell2 = new PdfPCell(new Phrase(chrPeriodFrom2[j].ToString(), fontA119B));
                                PdfDetailCell2.NoWrap = false;
                                PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell2.HorizontalAlignment = 1;
                                PdfDetailTable2.AddCell(PdfDetailCell2);
                            }
                        }
                        else
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                PdfDetailCell2 = new PdfPCell(new Phrase(" ", fontA119B));
                                PdfDetailCell2.NoWrap = false;
                                PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell2.HorizontalAlignment = 1;
                                PdfDetailTable2.AddCell(PdfDetailCell2);
                            }
                        }


                        //PdfDetailCell2 = new PdfPCell(new Phrase("x", fontAb11));
                        //PdfDetailCell2.NoWrap = false;
                        //PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        //PdfDetailCell2.HorizontalAlignment = 1;
                        //PdfDetailTable2.AddCell(PdfDetailCell2);
                        //PdfDetailCell2 = new PdfPCell(new Phrase("x", fontAb11));
                        //PdfDetailCell2.NoWrap = false;
                        //PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        //PdfDetailCell2.HorizontalAlignment = 1;
                        //PdfDetailTable2.AddCell(PdfDetailCell2);
                        //PdfDetailCell2 = new PdfPCell(new Phrase("x", fontAb11));
                        //PdfDetailCell2.NoWrap = false;
                        //PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        //PdfDetailCell2.HorizontalAlignment = 1;
                        //PdfDetailTable2.AddCell(PdfDetailCell2);
                        //PdfDetailCell2 = new PdfPCell(new Phrase("x", fontAb11));
                        //PdfDetailCell2.NoWrap = false;
                        //PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        //PdfDetailCell2.HorizontalAlignment = 1;
                        //PdfDetailTable2.AddCell(PdfDetailCell2);
                        //PdfDetailCell2 = new PdfPCell(new Phrase("x", fontAb11));
                        //PdfDetailCell2.NoWrap = false;
                        //PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        //PdfDetailCell2.HorizontalAlignment = 1;
                        //PdfDetailTable2.AddCell(PdfDetailCell2);
                        //PdfDetailCell2 = new PdfPCell(new Phrase("x", fontAb11));
                        //PdfDetailCell2.NoWrap = false;
                        //PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        //PdfDetailCell2.HorizontalAlignment = 1;
                        //PdfDetailTable2.AddCell(PdfDetailCell2);
                        //PdfDetailCell2 = new PdfPCell(new Phrase("x", fontAb11));
                        //PdfDetailCell2.NoWrap = false;
                        //PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        //PdfDetailCell2.HorizontalAlignment = 1;
                        //PdfDetailTable2.AddCell(PdfDetailCell2);

                        //PdfDetailCell2 = new PdfPCell(new Phrase("x", fontAb11));
                        //PdfDetailCell2.NoWrap = false;
                        //PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        //PdfDetailCell2.HorizontalAlignment = 1;
                        //PdfDetailTable2.AddCell(PdfDetailCell2);

                        PdfDetailCell2 = new PdfPCell(new Phrase(" ", fontAb11));
                        PdfDetailCell2.NoWrap = false;
                        PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell2.HorizontalAlignment = 1;
                        PdfDetailCell2.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        PdfDetailCell2.Colspan = 25;
                        PdfDetailTable2.AddCell(PdfDetailCell2);




                        PdfDetailCell2 = new PdfPCell(new Phrase("To*", fontAb11B));
                        PdfDetailCell2.NoWrap = false;
                        PdfDetailCell2.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell2.HorizontalAlignment = 1;
                        PdfDetailTable2.AddCell(PdfDetailCell2);

                        string PeriodTo2 = dr["PeriodTo"].ToString();

                        char[] chrPeriodTo2 = new char[8];
                        chrPeriodTo2 = PeriodTo2.ToCharArray();
                        if (Convert.ToInt32(chrPeriodTo2.Length) > 0)
                        {
                            if (dr["PeriodTo"].ToString() != "01011900")
                            {
                                for (int j = 0; j < Convert.ToInt32(chrPeriodTo.Length); j++)
                                {
                                    PdfDetailCell2 = new PdfPCell(new Phrase(chrPeriodTo[j].ToString(), fontA119B));
                                    PdfDetailCell2.NoWrap = false;
                                    PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell2.HorizontalAlignment = 1;
                                    PdfDetailTable2.AddCell(PdfDetailCell2);
                                }
                            }
                            else
                            {
                                for (int j = 0; j < 8; j++)
                                {
                                    PdfDetailCell2 = new PdfPCell(new Phrase(" ", fontA119B));
                                    PdfDetailCell2.NoWrap = false;
                                    PdfDetailCell2.BackgroundColor = new iTextSharp.text.Color(0, 0, 0);
                                    PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell2.HorizontalAlignment = 1;
                                    PdfDetailTable2.AddCell(PdfDetailCell2);
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                PdfDetailCell2 = new PdfPCell(new Phrase(" ", fontA119B));
                                PdfDetailCell2.NoWrap = false;
                                //PdfDetailCell2.Colspan = 8;
                                PdfDetailCell2.BackgroundColor = new iTextSharp.text.Color(0, 0, 0);
                                PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell2.HorizontalAlignment = 1;
                                PdfDetailTable2.AddCell(PdfDetailCell2);
                            }
                        }


                        //PdfDetailCell2 = new PdfPCell(new Phrase("", fontAb11B));
                        //PdfDetailCell2.NoWrap = false;
                        //PdfDetailCell2.Colspan = 8;
                        //PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        //PdfDetailCell2.HorizontalAlignment = 1;
                        //PdfDetailTable2.AddCell(PdfDetailCell2);

                        PdfDetailCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        PdfDetailCell2.NoWrap = false;
                        PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell2.HorizontalAlignment = 1;
                        PdfDetailCell2.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        PdfDetailCell2.Colspan = 25;
                        PdfDetailTable2.AddCell(PdfDetailCell2);


                        PdfDetailCell2 = new PdfPCell(new Phrase("Or", fontAb11B));
                        PdfDetailCell2.NoWrap = false;
                        PdfDetailCell2.BackgroundColor = new iTextSharp.text.Color(252, 252, 252);
                        PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell2.HorizontalAlignment = 1;
                        PdfDetailTable2.AddCell(PdfDetailCell2);

                        Document documentCheckBox1235 = new Document();
                        documentCheckBox1235.Open();
                        Paragraph pCheckBox1235 = new Paragraph();
                        if (dr["PeriodTo"].ToString() == "01011900")
                        {
                            pCheckBox1235.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                        }
                        else
                        {
                            pCheckBox1235.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                        }

                        pCheckBox1235.Add(new Phrase(" Until Cancelled ", fontAb11));


                        PdfDetailCell2 = new PdfPCell(pCheckBox1235);


                        PdfDetailCell2.NoWrap = false;
                        PdfDetailCell2.Colspan = 8;
                        PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell2.HorizontalAlignment = 1;
                        PdfDetailTable2.AddCell(PdfDetailCell2);


                        PdfDetailCell2 = new PdfPCell(new Phrase("Sign. Primary Acc. Holder", fontAb11BU));
                        PdfDetailCell2.NoWrap = false;
                        PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell2.HorizontalAlignment = 1;
                        PdfDetailCell2.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        PdfDetailCell2.Colspan = 8;
                        PdfDetailTable2.AddCell(PdfDetailCell2);

                        PdfDetailCell2 = new PdfPCell(new Phrase("Sign Acc. Holder", fontAb11BU));
                        PdfDetailCell2.NoWrap = false;
                        PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell2.HorizontalAlignment = 1;
                        PdfDetailCell2.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        PdfDetailCell2.Colspan = 8;
                        PdfDetailTable2.AddCell(PdfDetailCell2);

                        PdfDetailCell2 = new PdfPCell(new Phrase("Sign Acc. Holder", fontAb11BU));
                        PdfDetailCell2.NoWrap = false;
                        PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell2.HorizontalAlignment = 1;
                        PdfDetailCell2.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        PdfDetailCell2.Colspan = 9;
                        PdfDetailTable2.AddCell(PdfDetailCell2);

                        //PdfDetailCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        //PdfDetailCell2.NoWrap = false;
                        //PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        //PdfDetailCell2.HorizontalAlignment = 1;
                        //PdfDetailCell2.Border = Rectangle.NO_BORDER;

                        //PdfDetailCell2.Colspan = 34;
                        //PdfDetailTable2.AddCell(PdfDetailCell2);

                        PdfDetailCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        PdfDetailCell2.NoWrap = false;
                        PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell2.HorizontalAlignment = 1;
                        PdfDetailCell2.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        PdfDetailCell2.Colspan = 9;
                        PdfDetailTable2.AddCell(PdfDetailCell2);

                        PdfDetailCell2 = new PdfPCell(new Phrase(dr["BenificiaryName"].ToString(), fontAb11));
                        PdfDetailCell2.NoWrap = false;
                        PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell2.HorizontalAlignment = 1;
                        PdfDetailCell2.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        PdfDetailCell2.Colspan = 9;
                        PdfDetailTable2.AddCell(PdfDetailCell2);

                        PdfDetailCell2 = new PdfPCell(new Phrase("", fontAb11));
                        PdfDetailCell2.NoWrap = false;
                        PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell2.HorizontalAlignment = 1;
                        PdfDetailCell2.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        PdfDetailCell2.Colspan = 8;
                        PdfDetailTable2.AddCell(PdfDetailCell2);

                        PdfDetailCell2 = new PdfPCell(new Phrase("", fontAb11));
                        PdfDetailCell2.NoWrap = false;
                        PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell2.HorizontalAlignment = 1;
                        PdfDetailCell2.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        PdfDetailCell2.Colspan = 8;
                        PdfDetailTable2.AddCell(PdfDetailCell2);


                        PdfDetailCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                        PdfDetailCell2.NoWrap = false;
                        PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell2.HorizontalAlignment = 1;
                        PdfDetailCell2.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        PdfDetailCell2.Colspan = 9;
                        PdfDetailTable2.AddCell(PdfDetailCell2);

                        PdfDetailCell2 = new PdfPCell(new Phrase("1.Name as in Bank Records", fontAb11BU));
                        PdfDetailCell2.NoWrap = false;
                        PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell2.HorizontalAlignment = 1;
                        PdfDetailCell2.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        PdfDetailCell2.Colspan = 8;
                        PdfDetailTable2.AddCell(PdfDetailCell2);

                        PdfDetailCell2 = new PdfPCell(new Phrase("2.Name as in Bank Records", fontAb11BU));
                        PdfDetailCell2.NoWrap = false;
                        PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell2.HorizontalAlignment = 1;
                        PdfDetailCell2.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        PdfDetailCell2.Colspan = 8;
                        PdfDetailTable2.AddCell(PdfDetailCell2);

                        PdfDetailCell2 = new PdfPCell(new Phrase("3.Name as in Bank Records", fontAb11BU));
                        PdfDetailCell2.NoWrap = false;
                        PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell2.HorizontalAlignment = 1;
                        PdfDetailCell2.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        PdfDetailCell2.Colspan = 9;
                        PdfDetailTable2.AddCell(PdfDetailCell2);


                        PdfDetailCell2 = new PdfPCell(new Phrase(" ", fontText4));
                        PdfDetailCell2.NoWrap = false;
                        PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell2.HorizontalAlignment = 1;
                        PdfDetailCell2.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        PdfDetailCell2.Colspan = 34;
                        PdfDetailTable2.AddCell(PdfDetailCell2);


                        PdfDetailCell2 = new PdfPCell(new Phrase("This is to confirm that declaration has been carefully read, understood & made by me/us. I'm authorizing the user entity/Corporate to debit my account, based on the instruction as agreed and signed by me. I've understood that I'm authorized to cancel/amend this mandate by appropriately communicating the cancellation/amendment request to the user/entity/corporate or the bank where I've authorized the debit.", fontText5));
                        PdfDetailCell2.NoWrap = false;
                        PdfDetailCell2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                        PdfDetailCell2.Colspan = 34;

                        PdfDetailTable2.AddCell(PdfDetailCell2);



                        Document pdfDoc = new Document(PageSize.A4, 20f, 20f, 10f, 10f);
                        MemoryStream memoryStream = new MemoryStream();
                        HTMLWorker htmlparser = new HTMLWorker(pdfDoc);

                        PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                        writer.CloseStream = false;
                        pdfDoc.Open();
                        //Response.ContentType = "Application/pdf";
                        //Response.AppendHeader("content-disposition", "attachment;filename=" + Convert.ToString(dr["Reference1"]) + "");
                        pdfDoc.Add(PdfHeaderTable);
                        pdfDoc.Add(PdfMidTable);
                        pdfDoc.Add(PdfDetailTable);
                        pdfDoc.Add(CutterImage);
                        pdfDoc.Add(PdfHeaderTable1);
                        pdfDoc.Add(PdfMidTable1);
                        pdfDoc.Add(PdfDetailTable1);
                        pdfDoc.Add(CutterImage);
                        pdfDoc.Add(PdfHeaderTable2);
                        pdfDoc.Add(PdfMidTable2);
                        pdfDoc.Add(PdfDetailTable2);
                        pdfDoc.Close();
                        memoryStream.Position = 0;
                        StringBuilder sb = new StringBuilder();
                        string WebAppUrl = ConfigurationManager.AppSettings["WebAppUrl" + AppID].ToString();
                        string SMTPHost = ConfigurationManager.AppSettings["SMTPHost"].ToString();
                        string UserId = ConfigurationManager.AppSettings["UserId"].ToString();
                        string MailPassword = ConfigurationManager.AppSettings["MailPassword"].ToString();
                        string SMTPPort = ConfigurationManager.AppSettings["SMTPPort"].ToString();
                        string SMTPEnableSsl = ConfigurationManager.AppSettings["SMTPEnableSsl"].ToString();
                        sb.Append("Please find the attachment. <br> <br>");
                        sb.Append("Reference Number : " + Convert.ToString(ds.Tables[0].Rows[0]["Reference1"]) + "  <br> <br>");
                        sb.Append("Account Holder : " + ds.Tables[0].Rows[0]["BenificiaryName"].ToString() + "  <br> <br>");
                        sb.Append("Bank Account Number : " + Convert.ToString(ds.Tables[0].Rows[0]["AccountNo"]) + "  <br> <br><br> <br>");
                        sb.Append("This is auto generated mail. Please don't reply to this mail.   <br> <br>");
                        SmtpClient smtpClient = new SmtpClient();

                        MailMessage mailmsg = new MailMessage();
                        MailAddress mailaddress = new MailAddress(UserId);

                        mailmsg.To.Add(Convert.ToString(ds.Tables[0].Rows[0]["mailSendEmail"]));

                        mailmsg.Attachments.Add(new Attachment(memoryStream, "Mandate.pdf"));

                        mailmsg.Body = Convert.ToString(sb);


                        mailmsg.From = mailaddress;

                        mailmsg.Subject = "Mandate For Reference No: " + Convert.ToString(ds.Tables[0].Rows[0]["Reference1"]);
                        mailmsg.IsBodyHtml = true;




                        smtpClient.Host = SMTPHost;
                        smtpClient.Port = Convert.ToInt32(SMTPPort);
                        smtpClient.EnableSsl = Convert.ToBoolean(SMTPEnableSsl);
                        smtpClient.UseDefaultCredentials = true;
                        smtpClient.Credentials = new System.Net.NetworkCredential(UserId, MailPassword);
                        smtpClient.Send(mailmsg);

                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                con.Close();
            }
        }
        static uint MAKERGB(int r, int g, int b)
        {

            return ((uint)(b & 255)) | ((uint)((r & 255) << 8)) | ((uint)((g & 255) << 16));

        }
        static System.Drawing.Bitmap CopyToBpp(System.Drawing.Bitmap b, int bpp)
        {

            if (bpp != 1 && bpp != 8) throw new System.ArgumentException("1 or 8", "bpp");



            int w = b.Width, h = b.Height;

            IntPtr hbm = b.GetHbitmap();



            BITMAPINFObase64 bmi = new BITMAPINFObase64();

            bmi.biSize = 40;

            bmi.biWidth = w;

            bmi.biHeight = h;

            bmi.biPlanes = 1;

            bmi.biBitCount = (short)bpp;

            bmi.biCompression = BI_RGB;

            bmi.biSizeImage = (uint)(((w + 7) & 0xFFFFFFF8) * h / 8);

            bmi.biXPelsPerMeter = 1000000;

            bmi.biYPelsPerMeter = 1000000;

            // Now for the colour table.

            uint ncols = (uint)1 << bpp;

            bmi.biClrUsed = ncols;

            bmi.biClrImportant = ncols;

            bmi.cols = new uint[256];

            if (bpp == 1) { bmi.cols[0] = MAKERGB(0, 0, 0); bmi.cols[1] = MAKERGB(255, 255, 255); }

            else { for (int i = 0; i < ncols; i++) bmi.cols[i] = MAKERGB(i, i, i); }

            IntPtr bits0; IntPtr hbm0 = CreateDIBSection(IntPtr.Zero, ref bmi, DIB_RGB_COLORS, out bits0, IntPtr.Zero, 0);

            IntPtr sdc = GetDC(IntPtr.Zero);

            IntPtr hdc = CreateCompatibleDC(sdc); SelectObject(hdc, hbm);

            IntPtr hdc0 = CreateCompatibleDC(sdc); SelectObject(hdc0, hbm0);

            BitBlt(hdc0, 0, 0, w, h, hdc, 0, 0, SRCCOPY);

            System.Drawing.Bitmap b0 = System.Drawing.Bitmap.FromHbitmap(hbm0);



            DeleteDC(hdc);

            DeleteDC(hdc0);

            ReleaseDC(IntPtr.Zero, sdc);

            DeleteObject(hbm);

            DeleteObject(hbm0);

            //

            return b0;

        }




    }
   
    public struct BITMAPINFObase64
    {

        public uint biSize;

        public int biWidth, biHeight;

        public short biPlanes, biBitCount;

        public uint biCompression, biSizeImage;

        public int biXPelsPerMeter, biYPelsPerMeter;

        public uint biClrUsed, biClrImportant;

        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 256)]

        public uint[] cols;

    }
    public class ResponseDatabase64
    {
        //public string filePath { get; set; }
        //public string folderPath { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public string JpgImage { get; set; }
        public string TifImage { get; set; }
        public string MdtID { get; set; }
        public string ResCode { get; set; }
    }
 

}
