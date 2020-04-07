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
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Script.Serialization;
using ZipNachWebAPI.Models;

namespace ZipNachWebAPI.Controllers
{
    public class SubmitMandateImageDataController : ApiController
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
        static uint MAKERGB(int r, int g, int b)
        {

            return ((uint)(b & 255)) | ((uint)((r & 255) << 8)) | ((uint)((g & 255) << 16));

        }
        [Route("api/UploadMandate/SubmitMandateImage")]
        [HttpPost]
        public  ResponseGetMobileNO uploadA4scan(SubmitMandateImageInput context)
        {

            string fileName = string.Empty;
            string filePath = string.Empty;
            string extension = string.Empty;
            string filePath1 = string.Empty;
            string filePath2 = string.Empty;
            string RefrenceNo = string.Empty;


            string croppedFileName = string.Empty;
            string croppedFilePath = string.Empty;
            // string Path = "";
            //string dirnm = "";
            ResponseGetMobileNO pathInfo = new ResponseGetMobileNO();
            // byte[] imagedata = System.Convert.FromBase64String(context.base64.ToString());
            try
            {

                if (context.AppID == "" || context.AppID == null)
                {
                    pathInfo.message = "Incomplete data";
                    pathInfo.status = "Failure";
                    return pathInfo;
                }



                if (context.MandeteId == "" || context.MandeteId == null)
                {
                    pathInfo.message = "Incomplete data";
                    pathInfo.status = "Failure";
                    return pathInfo;
                }

                if (context.UserId == "" || context.UserId == null)
                {
                    pathInfo.message = "Incomplete data";
                    pathInfo.status = "Failure";
                    return pathInfo;
                }
                if (context.ImageBytes == "" || context.ImageBytes == null)
                {
                    pathInfo.message = "Incomplete data";
                    pathInfo.status = "Failure";
                    return pathInfo;
                }
                if (context.ImageBytes.Length < 0 || context.ImageBytes == null)
                {
                    pathInfo.message = "Incomplete data";
                    pathInfo.status = "Failure";
                    return pathInfo;
                }
                
                string TempJpgpath = string.Empty;
                string TempTifpath = string.Empty;
                Boolean Greater =true;
                string Path = "";
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(context.AppID)].ConnectionString);
                string query = "Sp_Mandate";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "CheckMandateId");
                cmd.Parameters.AddWithValue("@MandateId", context.MandeteId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows[0]["value"].ToString() == "1")
                {
                    RefrenceNo = (dt.Rows[0]["Refrence1"].ToString());
                    
                    System.IO.MemoryStream mm = new System.IO.MemoryStream();
                    byte[] bytes = System.Convert.FromBase64String(context.ImageBytes);
                    mm.Write(bytes, 0, bytes.Length);
                    System.Drawing.Image img = System.Drawing.Image.FromStream(mm);
                    System.Drawing.Image imgtif = System.Drawing.Image.FromStream(mm);


                    if (Convert.ToInt32(img.HorizontalResolution) <= 300)
                    {

                        //bool Flag = System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/MandateFile/" + context.MandeteId));
                        Path = ConfigurationManager.AppSettings["FileUploadPath" + context.AppID].ToString() + "MandateFile/" + context.MandeteId + "/";
                        string FilePath = ConfigurationManager.AppSettings["FileUploadPath" + context.AppID].ToString() + "MandateFile/" + context.MandeteId + "/";


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

                        fileName = ConfigurationManager.AppSettings["DownloadFileName" + context.AppID].ToString() + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + RefrenceNo + ".jpg";

                        //filePath = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/MandateFile/" + context.MandeteId + "/" + fileName.Trim()));

                        filePath = ConfigurationManager.AppSettings["FileUploadPath" + context.AppID].ToString() + "MandateFile/" + context.MandeteId + "/" + fileName.Trim();


                        filePath1 = "../MandateFile/" + context.MandeteId + "/" + fileName;
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);



                            img = System.Drawing.Image.FromFile(filePath);
                            if (img.Width > img.Height)
                            {
                                Greater = true;
                            }
                            else
                            {
                                Greater = false;
                            }
                            //System.Drawing.Rectangle areaToCrop = new System.Drawing.Rectangle(Convert.ToInt32(0),
                            //    Convert.ToInt32(0),
                            //    Convert.ToInt32(img.Width),
                            //    Convert.ToInt32(img.Height));




                        }

                        Bitmap bitMap = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);

                        using (Graphics g = Graphics.FromImage(bitMap))
                        {


                            g.CompositingQuality = CompositingQuality.HighQuality;
                            g.SmoothingMode = SmoothingMode.HighQuality;
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        }
                        bitMap.SetResolution(96, 96);
                        bitMap.Save(filePath);
                        using (var image = img)
                        {
                            int newWidth = 0;
                            int newHeight = 0;
                            //if (Greater == true)
                            //{
                            newWidth = 827; // New Width of Image in Pixel  
                            newHeight = 356;

                            //}
                            //else
                            //{
                            //    newWidth = 356;
                            //    newHeight = 827;

                            //}


                            var thumbImg = new Bitmap(newWidth, newHeight, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
                            var thumbGraph = Graphics.FromImage(thumbImg);
                            var imgRectangle = new System.Drawing.Rectangle(0, 0, newWidth, newHeight);

                            thumbGraph.DrawImage(image, imgRectangle);
                            thumbImg.SetResolution(100, 100);

                            System.Drawing.Bitmap b0 = CopyToBpp(thumbImg, 8);
                            b0.SetResolution(100, 100);
                            fileName = ConfigurationManager.AppSettings["DownloadFileName" + context.AppID].ToString() + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + RefrenceNo + ".jpg";

                            filePath1 = "../MandateFile/" + context.MandeteId + "/" + fileName;
                            croppedFilePath = ConfigurationManager.AppSettings["FileUploadPath" + context.AppID].ToString() + "MandateFile/" + context.MandeteId + "/" + fileName.Trim();

                            TempJpgpath = croppedFilePath;
                            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                            EncoderParameters myEncoderParameters = new EncoderParameters(1);
                            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
                            myEncoderParameters.Param[0] = myEncoderParameter;
                            b0.Save(croppedFilePath, jpgEncoder, myEncoderParameters);
                           
                        }

                        // img.Save(filePath);


                        query = "Sp_Mandate";
                        cmd = new SqlCommand(query, con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@QueryType", "CheckLogo");
                        cmd.Parameters.AddWithValue("@MandateId", context.MandeteId);
                        da = new SqlDataAdapter(cmd);
                        dt = new DataTable();
                        da.Fill(dt);

                        string Logo = dt.Rows[0]["printlogo"].ToString();

                        string result = "";
                        if (Logo == "True")
                        {
                            result = context.MandeteId;
                        }
                        else
                        {
                            // result = GetMandatefromQR(filePath, context.MandeteId);
                            result = context.MandeteId;
                        }
                        if (result == context.MandeteId)
                        {

                            fileName = ConfigurationManager.AppSettings["DownloadFileName" + context.AppID].ToString() + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + RefrenceNo + ".tif";

                            filePath = ConfigurationManager.AppSettings["FileUploadPath" + context.AppID].ToString() + "MandateFile/" + context.MandeteId + "/" + fileName.Trim();
                            filePath2 = "../MandateFile/" + context.MandeteId + "/" + fileName;

                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);



                                imgtif = System.Drawing.Image.FromFile(filePath);

                                //System.Drawing.Rectangle areaToCrop = new System.Drawing.Rectangle(Convert.ToInt32(0),
                                //    Convert.ToInt32(0),
                                //    Convert.ToInt32(img.Width),
                                //    Convert.ToInt32(img.Height));




                            }
                            //imgtif = System.Drawing.Image.FromFile(filePath);
                            Bitmap bitMap1 = new Bitmap(imgtif.Width, imgtif.Height, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);

                            using (Graphics g = Graphics.FromImage(bitMap1))
                            {


                                g.CompositingQuality = CompositingQuality.HighQuality;
                                g.SmoothingMode = SmoothingMode.HighQuality;
                                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                            }
                            bitMap1.SetResolution(96, 96);
                            bitMap1.Save(filePath);


                            using (var image1 = imgtif)
                            {
                                int newWidth = 0;
                                int newHeight = 0;
                                //if (Greater == true)
                                //{
                                //    newWidth = imgtif.Width;
                                //    newHeight = imgtif.Height;
                                newWidth = 827 * 2; // New Width of Image in Pixel  
                                newHeight = 356 * 2;
                                //}
                                //else
                                //{
                                //    newWidth = imgtif.Width;
                                //    newHeight = imgtif.Height;
                                //    newWidth = 827 * 2; // New Width of Image in Pixel  
                                //    newHeight = 356 * 2;
                                //}
                                var thumbImg1 = new Bitmap(newWidth, newHeight);

                                var thumbGraph1 = Graphics.FromImage(thumbImg1);

                                var imgRectangle1 = new System.Drawing.Rectangle(0, 0, newWidth, newHeight);

                                thumbGraph1.DrawImage(image1, imgRectangle1);


                                System.Drawing.Bitmap b1 = CopyToBpp(thumbImg1, 1);
                                b1.SetResolution(200, 200);






                                fileName = ConfigurationManager.AppSettings["DownloadFileName" + context.AppID].ToString() + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + RefrenceNo + ".tif";


                                filePath2 = "../MandateFile/" + context.MandeteId + "/" + fileName;
                                croppedFilePath = ConfigurationManager.AppSettings["FileUploadPath" + context.AppID].ToString() + "MandateFile/" + context.MandeteId + "/" + fileName.Trim();

                                TempTifpath = croppedFilePath;
                                ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Tiff);
                                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Compression;
                                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder,
                                        (long)EncoderValue.CompressionCCITT4);
                                myEncoderParameters.Param[0] = myEncoderParameter;
                                b1.Save(croppedFilePath, jpgEncoder, myEncoderParameters);
                              
                                //b1.Save(croppedFilePath, image1.RawFormat);

                            }

                        }
                        else
                        {


                            pathInfo.message = "Invalid Mandate";
                            pathInfo.status = "Failure";
                        }



                    }
                    else
                    {
                        pathInfo.message = "Image Size Must Be less Than 300Dpi";
                        pathInfo.status = "Failure";
                    }

                    filePath2 = "../MandateFile/" + context.MandeteId + "/" + fileName;
                    query = "Sp_Mandate";
                    cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@QueryType", "UpdatePNGTIP");
                    cmd.Parameters.AddWithValue("@MandateId", context.MandeteId);
                    cmd.Parameters.AddWithValue("@PNGPath", filePath1);
                    cmd.Parameters.AddWithValue("@TIPPath", filePath2);
                    da = new SqlDataAdapter(cmd);
                    dt = new DataTable();
                    da.Fill(dt);


                    pathInfo.message = "Image uploaded successfully";
                    pathInfo.status = "Success";
                    try
                    {
                        if (Convert.ToString(ConfigurationManager.AppSettings["SFDC" + context.AppID]) != null && Convert.ToString(ConfigurationManager.AppSettings["SFDC" + context.AppID]) != "")
                        {
                            string apiUrl = Convert.ToString(ConfigurationManager.AppSettings["SFDC" + context.AppID]);
                            var input = new
                            {
                                MdtId = context.MandeteId,
                                ResStatus = "Success",
                                ResCode = "ykR20033",
                                Message = "Image uploaded successfully",
                                MandateStatus = dt.Rows[0]["status"],
                                TiffBase64 = ConvertImageBase64(TempTifpath),
                                JpgBase64 = ConvertImageBase64(TempJpgpath),
                                IsAggregatorValue = dt.Rows[0]["IsAggregatorValue"],
                                
                            };
                            string inputJson = (new JavaScriptSerializer()).Serialize(input);
                            WebClient client = new WebClient();
                            client.Headers["Content-type"] = "application/json";
                            client.Encoding = Encoding.UTF8;

                            string json = client.UploadString(apiUrl, inputJson);
                            string temppath = System.Web.Hosting.HostingEnvironment.MapPath("~/SFDCResponse\\" + context.AppID);
                            if (!Directory.Exists(temppath))
                                Directory.CreateDirectory(temppath);
                            temppath = temppath + "\\" + context.MandeteId + ".txt";
                            File.WriteAllText(temppath, json);
                        }
                    }
                    catch(Exception ex)
                    { }
                }

                else
                {
                    pathInfo.message = "Invalid MandateId";
                    pathInfo.status = "Failure";

                }

            }
            catch (Exception e)
            {
                pathInfo.message = e.Message;
                pathInfo.status = "failure";

            }

            //imag.Dispose();
            //Write code here to Save byte code to database..  
            return pathInfo;


        }
        public async Task PostData(string AppID, string MandeteId,string TempTifpath,string TempJpgpath,string status)
        {
            string json1 = "";
            if (Convert.ToString(ConfigurationManager.AppSettings["SFDC" + AppID]) != null && Convert.ToString(ConfigurationManager.AppSettings["SFDC" + AppID]) != "")
            {
                string apiUrl = Convert.ToString(ConfigurationManager.AppSettings["SFDC" + AppID]);
                var input = new
                {
                    MdtId = MandeteId,
                    ResStatus = "Success",
                    ResCode = "ykR20033",
                    Message = "Image uploaded successfully",
                    MandateStatus = status,
                    TiffBase64 = ConvertImageBase64(TempTifpath),
                    JpgBase64 = ConvertImageBase64(TempJpgpath),

                };
                string inputJson = (new JavaScriptSerializer()).Serialize(input);

                var webReq = (HttpWebRequest)WebRequest.Create(apiUrl);
                using (var streamWriter = new StreamWriter(webReq.GetRequestStream()))
                {
                    streamWriter.Write(inputJson);
                }
                using (WebResponse response = await webReq.GetResponseAsync())
                {
                    var responseStream = response.GetResponseStream();
                    var myStreamReader = new StreamReader(responseStream, Encoding.Default);
                     json1 = myStreamReader.ReadToEnd();
                }
                //return json1;
                //    WebClient client = new WebClient();
                //client.Headers["Content-type"] = "application/json";
                //client.Encoding = Encoding.UTF8;

                //string json = client.UploadString(apiUrl, inputJson);
                //string temppath = System.Web.Hosting.HostingEnvironment.MapPath("~/SFDCResponse\\" + AppID);
                //if (!Directory.Exists(temppath))
                //    Directory.CreateDirectory(temppath);
                //temppath = temppath + "\\" + MandeteId + ".txt";
                //File.WriteAllText(temppath, json);
                //return  json;
            }
          //  return json1;
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

            //

            return b0;

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
       
    }
    public class ResponseGetMobileNO
    {
        public string message { get; set; }
        public string status { get; set; }

    }
}
