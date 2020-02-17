using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ImageUploadWCF;
using ZipNachWebAPI.Models;
using System.Web.Http.Cors;
using System.Web;
using ZXing;
using ZXing.Common;

namespace ZipNachWebAPI.Controllers
{
    [EnableCors(origins: "http://192.168.1.246:808/api/Clients", headers: "AllowAllHeaders", methods: "*")]
    public class UploadMandateOrgbase64Controller : ApiController
    {

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
        static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO bmi, uint Usage, out IntPtr bits, IntPtr hSection, uint dwOffset);
        static uint BI_RGB = 0;
        static uint DIB_RGB_COLORS = 0;
        string finalresult = "";
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct BITMAPINFO
        {
            public uint biSize;
            public int biWidth, biHeight;
            public short biPlanes, biBitCount;
            public uint biCompression, biSizeImage;
            public int biXPelsPerMeter, biYPelsPerMeter;
            public uint biClrUsed, biClrImportant;
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 256)]
            public uint[] cols;
            //public uint cols;
        }

        static uint MAKERGB(int r, int g, int b)
        {
            return ((uint)(b & 255)) | ((uint)((r & 255) << 8)) | ((uint)((g & 255) << 16));
        }

        [Route("api/UploadMandateOrg/uploadA4scanbase64")]
        [HttpPost]
        public ResponseDataorgbase64 uploadA4scan(CustomerModel context)
        {
            string TempMandateId = "";
            // var retList = new List<ResponseDataorgbase64>();
            //ResponseDataorgbase64 retList = new ResponseDataorgbase64();

            ResponseDataorgbase64 pathInfo = new ResponseDataorgbase64();
            //if (context.PrintQr == "1")
            //{
            //    TempMandateId = ReadQrCode(context.base64, context.MandateId);

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
            else if (context.MdtID == "")
            {
                pathInfo.Message = "Incomplete data";
                pathInfo.Status = "Failure";
                pathInfo.ResCode = "ykR20020";
               
                return pathInfo;
            }
            else if (!CheckMandateInfo.CheckManadateID(context.MdtID, context.AppID))
            {
                pathInfo.Message = "Invalid MandateId";
                pathInfo.Status = "Failure";
                pathInfo.ResCode = "ykR200203";
               
                return pathInfo;
            }
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
            else if (context.ScannedImage == "")
            {
                pathInfo.Message = "Incomplete data";
                pathInfo.Status = "Failure";
                pathInfo.ResCode = "ykR20020";
              
                return pathInfo;
            }


            //else if (!CheckMandateInfo.CheckAccountValidation(context.MdtID, context.AppID))
            //{
            //    pathInfo.Message = "Account should be validated";
            //    pathInfo.Status = "Failure";
            //    pathInfo.ResCode = "ykR200204";
            //    retList.Add(pathInfo);
            //    return pathInfo;
            //}
            else if (CheckMandateInfo.CheckENachValidation(context.MdtID, context.AppID))
            {
                pathInfo.Message = "Mandate type already selected as eMandate";
                pathInfo.Status = "Failure";
                pathInfo.ResCode = "ykR20030";
               
                return pathInfo;
            }

            //else if (TempMandateId.Trim() != context.MdtID.Trim() && context.PrintQr == "1")
            //{
            //    pathInfo.Message = "Scan MandateId is not equal to the passed mandateId";
            //    pathInfo.Status = "Failure";
            //    pathInfo.ResCode = "ykR200203";
            //    retList.Add(pathInfo);
            //    return pathInfo;
            //}
            else
            {

                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(context.AppID)].ConnectionString);
                bool Flag = false;
                // string temp = ConfigurationManager.AppSettings["EnitityMarchantKey" + context.AppID];
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
                //}
                if (Flag)
                {
                    Boolean Greater;
                    try
                    {
                        string ID = Convert.ToString(context.MdtID);
                        string No = CheckMandateInfo.GetRefNO(context.MdtID, context.AppID);// Convert.ToString(context.RefrenceNo);
                        string fileName = string.Empty;
                        string filePath = string.Empty;
                        string extension = string.Empty;
                        string targetPath = string.Empty;
                        string TIFFilepath = string.Empty;
                        string JPGFilepath = string.Empty;
                        string OrgFilepath = string.Empty;
                        byte[] bytes = System.Convert.FromBase64String(context.ScannedImage);
                        float mb = (bytes.Length / 1024f) / 1024f;
                        System.IO.MemoryStream mm = new System.IO.MemoryStream();
                        mm.Write(bytes, 0, bytes.Length);
                        System.Drawing.Image img = System.Drawing.Image.FromStream(mm);
                        if (Convert.ToInt32(img.HorizontalResolution) >= 300 && mb <= 3)
                        {
                            try
                            {

                                if (!Directory.Exists(ConfigurationManager.AppSettings["FileUploadPath" + context.AppID] + @"FullMandate\" + ID))
                                    Directory.CreateDirectory(ConfigurationManager.AppSettings["FileUploadPath" + context.AppID] + @"FullMandate\" + ID);
                                System.IO.DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["FileUploadPath" + context.AppID] + @"FullMandate\" + ID);
                                foreach (FileInfo file in di.GetFiles())
                                {
                                    file.Delete();
                                }
                                fileName = ConfigurationManager.AppSettings["DownloadFileName" + context.AppID].ToString() + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + No + ".jpg";
                                filePath = Path.Combine(ConfigurationManager.AppSettings["FileUploadPath" + context.AppID] + @"\ColouredImage\", fileName);
                                if (!Directory.Exists(ConfigurationManager.AppSettings["FileUploadPath" + context.AppID] + @"\ColouredImage\"))
                                {
                                    Directory.CreateDirectory(ConfigurationManager.AppSettings["FileUploadPath" + context.AppID] + @"\ColouredImage\");
                                }
                                using (System.Drawing.Image image = new Bitmap(new MemoryStream(bytes)))
                                {
                                    image.Save(filePath, ImageFormat.Png);
                                    int newWidth = 4960; // New Width of Image in Pixel  
                                    int newHeight = 7015; // New Height of Image in Pixel  
                                    var thumbImg = new Bitmap(newWidth, newHeight);
                                    var thumbGraph = Graphics.FromImage(thumbImg);
                                    thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                                    thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                                    thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                    var imgRectangle = new Rectangle(0, 0, newWidth, newHeight);
                                    thumbGraph.DrawImage(image, imgRectangle);
                                    // Save the file  d
                                    targetPath = ConfigurationManager.AppSettings["FileUploadPath" + context.AppID] + @"\FullMandate\" + ID + @"\" + fileName;
                                    thumbImg.Save(targetPath, image.RawFormat);
                                    OrgFilepath = @"\FullMandate\" + ID + @"\" + fileName;
                                }
                                fileName = ConfigurationManager.AppSettings["DownloadFileName" + context.AppID].ToString() + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + No + ".tif";
                                if (File.Exists(filePath))
                                {
                                    File.Delete(filePath);
                                }
                            }

                            catch (Exception ex)
                            {
                                // lblMsg.Text = "Oops!! error occured : " + ex.Message.ToString();
                            }
                            finally
                            {
                                extension = string.Empty;
                                fileName = string.Empty;
                                filePath = string.Empty;
                            }
                            if (!Directory.Exists(ConfigurationManager.AppSettings["FileUploadPath" + context.AppID] + @"\MandateFile\" + ID))
                            {
                                Directory.CreateDirectory(ConfigurationManager.AppSettings["FileUploadPath" + context.AppID] + @"\MandateFile\" + ID);
                            }
                            else
                            {
                                System.IO.DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["FileUploadPath" + context.AppID] + @"\MandateFile\" + ID);

                                foreach (FileInfo file in di.GetFiles())
                                {
                                    file.Delete();
                                }

                            }
                            string XCoordinate = "";
                            string YCoordinate = "";
                            string Width = "";
                            string Height = "";
                            string croppedFileName = string.Empty;
                            string croppedFilePath = string.Empty;
                            string TempJpgpath = string.Empty;
                            string TempTifpath = string.Empty;
                            filePath = Path.Combine(ConfigurationManager.AppSettings["FileUploadPath" + context.AppID] + @"\FullMandate\" + ID + "", ConfigurationManager.AppSettings["DownloadFileName" + context.AppID].ToString() + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + No + ".jpg");
                            //Check if file exists on the path i.e. in the UploadedImages folder.
                            if (File.Exists(filePath))
                            {
                                //Get the image from UploadedImages folder.
                                System.Drawing.Image orgImg = System.Drawing.Image.FromFile(filePath);
                                XCoordinate = "0";
                                YCoordinate = "0";
                                Width = Convert.ToString(orgImg.Width);
                                Height = Convert.ToString((Convert.ToInt32(orgImg.Height) / 3));
                                Rectangle areaToCrop = new Rectangle(Convert.ToInt32(XCoordinate),
                                    Convert.ToInt32(YCoordinate),
                                    Convert.ToInt32(Width),
                                    Convert.ToInt32(Height));
                                try
                                {
                                    Bitmap bitMap = new Bitmap(areaToCrop.Width, areaToCrop.Height, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
                                    //Create graphics object for alteration
                                    using (Graphics g = Graphics.FromImage(bitMap))
                                    {
                                        //Draw image to screen
                                        g.DrawImage(orgImg, new Rectangle(0, 0, bitMap.Width, bitMap.Height), areaToCrop, GraphicsUnit.Pixel);
                                        g.CompositingQuality = CompositingQuality.HighQuality;
                                        g.SmoothingMode = SmoothingMode.HighQuality;
                                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                    }
                                    bitMap.SetResolution(96, 96);
                                    //name the cropped image                           
                                    croppedFileName = ConfigurationManager.AppSettings["DownloadFileName" + context.AppID].ToString() + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + No + ".jpg";
                                    //Create path to store the cropped image
                                    if (!Directory.Exists(ConfigurationManager.AppSettings["FileUploadPath" + context.AppID] + @"\CropImage\"))
                                    {
                                        Directory.CreateDirectory(ConfigurationManager.AppSettings["FileUploadPath" + context.AppID] + @"\CropImage\");
                                    }
                                    croppedFilePath = Path.Combine(ConfigurationManager.AppSettings["FileUploadPath" + context.AppID] + @"\CropImage\", croppedFileName);
                                    bitMap.Save(croppedFilePath);
                                    var CropImagePath = Path.Combine(ConfigurationManager.AppSettings["FileUploadPath" + context.AppID] + @"\CropImage\", ConfigurationManager.AppSettings["DownloadFileName" + context.AppID].ToString() + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + No + ".jpg");
                                    System.Drawing.Image CropImage = System.Drawing.Image.FromFile(CropImagePath);
                                    using (var image = CropImage)
                                    {
                                        //int newWidth = 4200; // New Width of Image in Pixel  
                                        //int newHeight = 1750; // New Height of Image in Pixel 

                                        int newWidth = 827; // New Width of Image in Pixel  
                                        int newHeight = 356; // New Height of Image in Pixel                                                         
                                        var thumbImg = new Bitmap(newWidth, newHeight, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
                                        var thumbGraph = Graphics.FromImage(thumbImg);
                                        var imgRectangle = new Rectangle(0, 0, newWidth, newHeight);
                                        thumbGraph.DrawImage(image, imgRectangle);
                                        thumbImg.SetResolution(100, 100);
                                        System.Drawing.Bitmap b0 = CopyToBpp(thumbImg, 8);
                                        b0.SetResolution(100, 100);
                                        croppedFileName = ConfigurationManager.AppSettings["DownloadFileName" + context.AppID].ToString() + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + No + ".jpg";
                                        croppedFilePath = Path.Combine(ConfigurationManager.AppSettings["FileUploadPath" + context.AppID] + @"\MandateFile\" + ID + @"\", croppedFileName);
                                          JPGFilepath = "../MandateFile/" + ID + "/" + croppedFileName;
                                        TempJpgpath = croppedFilePath;
                                        ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                                        System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                                        EncoderParameters myEncoderParameters = new EncoderParameters(1);
                                        EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
                                        myEncoderParameters.Param[0] = myEncoderParameter;

                                        b0.Save(croppedFilePath, jpgEncoder, myEncoderParameters);

                                       // b0.Save(croppedFilePath, image.RawFormat);
                                    }
                                    var CropImagePath1 = Path.Combine(ConfigurationManager.AppSettings["FileUploadPath" + context.AppID] + @"\CropImage\", ConfigurationManager.AppSettings["DownloadFileName" + context.AppID].ToString() + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + No + ".jpg");
                                    System.Drawing.Image CropImage1 = System.Drawing.Image.FromFile(CropImagePath1);
                                    using (var image1 = CropImage1)
                                    {
                                        int newWidth = 827 * 2;
                                        int newHeight = 356 * 2;
                                        var thumbImg1 = new Bitmap(newWidth, newHeight);
                                        var thumbGraph1 = Graphics.FromImage(thumbImg1);
                                        var imgRectangle1 = new Rectangle(0, 0, newWidth, newHeight);
                                        thumbGraph1.DrawImage(image1, imgRectangle1);
                                        thumbImg1.SetResolution(200, 200);
                                        System.Drawing.Bitmap b1 = CopyToBpp(thumbImg1, 1);
                                        b1.SetResolution(200, 200);
                                        croppedFileName = ConfigurationManager.AppSettings["DownloadFileName" + context.AppID].ToString() + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + No + ".tif";
                                        croppedFilePath = Path.Combine(ConfigurationManager.AppSettings["FileUploadPath" + context.AppID] + @"\MandateFile\" + ID + @"\", croppedFileName);
                                         TIFFilepath = "../MandateFile/" + ID + "/" + croppedFileName;
                                        TempTifpath = croppedFilePath;
                                        ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Tiff);
                                        System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Compression;
                                        EncoderParameters myEncoderParameters = new EncoderParameters(1);
                                        EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder,
                                                (long)EncoderValue.CompressionCCITT4);
                                        myEncoderParameters.Param[0] = myEncoderParameter;
                                        b1.Save(croppedFilePath, jpgEncoder, myEncoderParameters);
                                        // b1.Save(croppedFilePath, image1.RawFormat);
                                    }
                                    orgImg.Dispose();
                                    con.Open();
                                    cmd = new SqlCommand("Sp_Mandate", con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@QueryType", "UpdatePNGTIP");
                                    cmd.Parameters.AddWithValue("@TIPPath", TIFFilepath);
                                    cmd.Parameters.AddWithValue("@PNGPath", JPGFilepath);
                                    cmd.Parameters.AddWithValue("@MandateId", context.MdtID);
                                    cmd.Parameters.AddWithValue("@UserId", UserId);
                                    try
                                    {
                                        cmd.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        pathInfo.Message = ex.Message;
                                    }
                                    finally { con.Close(); }
                                    TIFFilepath = croppedFilePath;
                                    if (File.Exists(CropImagePath1))
                                    {
                                        File.Delete(CropImagePath1);
                                    }
                                    if (File.Exists(ConfigurationManager.AppSettings["FileUploadPath" + context.AppID] + @"\CropImage\" + ConfigurationManager.AppSettings["DownloadFileName" + context.AppID].ToString() + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + No + ".jpg"))
                                    {
                                        File.Delete(ConfigurationManager.AppSettings["FileUploadPath" + context.AppID] + @"\CropImage\" + ConfigurationManager.AppSettings["DownloadFileName" + context.AppID].ToString() + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + No + ".jpg");
                                    }
                                    bitMap = null;

                                    //Show cropped image
                                    croppedFileName = ConfigurationManager.AppSettings["DownloadFileName" + context.AppID].ToString() + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + No + ".jpg";
                                    pathInfo.Status = "Success";
                                    pathInfo.ResCode = "ykR20033";
                                    pathInfo.Message = "Image uploaded successfully";
                                    pathInfo.MdtID = context.MdtID;
                                    //   pathInfo.FullImagePath = ConfigurationManager.AppSettings["FilePath" + context.AppID].ToString() + "/" + OrgFilepath;
                                    //pathInfo.JpgImage = ConfigurationManager.AppSettings["FilePathURL" + context.AppID].ToString() + "/" + JPGFilepath.Substring(3, JPGFilepath.Length - 3);
                                    //pathInfo.TifImage = ConfigurationManager.AppSettings["FilePathURL" + context.AppID].ToString() + "/" + TIFFilepath.Substring(3, TIFFilepath.Length - 3);
                                    pathInfo.JpgImage = ConvertImageBase64(TempJpgpath);
                                    pathInfo.TifImage = ConvertImageBase64(TempTifpath);
                                    //System.Web.Hosting.HostingEnvironment.MapPath("~/SomePath");

                                }
                                catch (Exception ex)
                                {
                                    // lblMsg.Text = "Oops!! error occured : " + ex.Message.ToString();
                                }
                                finally
                                {
                                    fileName = string.Empty;
                                    filePath = string.Empty;
                                    croppedFileName = string.Empty;
                                    croppedFilePath = string.Empty;
                                }
                            }

                            return pathInfo;
                        }
                        else
                        {
                            pathInfo.ResCode = "ykR20027";
                            pathInfo.Status = "Failure";
                            pathInfo.Message = "Image resolution should be greater than or equal to 300 DPI-This is for only uploading A4 Scan";
                            pathInfo.JpgImage = "";
                            pathInfo.TifImage = "";
                        }
                    }
                    catch (Exception e)
                    {
                        pathInfo.ResCode = "ykR20020";
                        pathInfo.Status = "Failure";
                        pathInfo.Message = "Incomplete data";
                        pathInfo.JpgImage = "";
                        pathInfo.TifImage = "";
                    }
                
                    return pathInfo;
                }
                else
                {
                    pathInfo.ResCode = "ykR20020";
                    pathInfo.Status = "Failure";
                    pathInfo.Message = "Incomplete data";
                 
                    return pathInfo;
                }
            }
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

        static System.Drawing.Bitmap CopyToBpp(System.Drawing.Bitmap b, int bpp)
        {
            if (bpp != 1 && bpp != 8) throw new System.ArgumentException("1 or 8", "bpp");
            int w = b.Width, h = b.Height;
            IntPtr hbm = b.GetHbitmap();
            BITMAPINFO bmi = new BITMAPINFO();
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
            return b0;
        }

    }
    public class ResponseDataorgbase64
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public string JpgImage { get; set; }
        public string TifImage { get; set; }
        public string MdtID { get; set; }
        public string ResCode { get; set; }
       
    }


}