using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ImageUploadWCF;
using ZipNachWebAPI.Models;
using System.Web.Http.Cors;
using System.Web;
using System.Drawing;
using ZXing;
using ZXing.Common;
using System.IO;

namespace ZipNachWebAPI.Controllers
{
    [EnableCors(origins: "http://192.168.1.246:808/api/Clients", headers: "AllowAllHeaders", methods: "*")]
    public class ScanQRCodeController : ApiController
    {

        [Route("api/ScanQRCodeController/ScanQRCode")]
        [HttpPost]

        public GetMandateResponseforQRcode ScanQRCode(ScanQRImage Data)
        {
            GetMandateResponseforQRcode response = new GetMandateResponseforQRcode();
            try
            {
                if (Data.AppId == "")
                {
                    response.message = "Incomplete data";
                    response.status = "Failure";
                    response.ResCode = "ERR000";
                    return response;
                }
                else if (Data.AppId != "" && CheckMandateInfo.ValidateAppID(Data.AppId) != true)
                {
                    response.message = "Invalid ApplicationId";
                    response.status = "Failure";
                    response.ResCode = "ykR20023";

                    return response;
                }
                else if (ValidatePresement.CheckAccess(Data.AppId.Trim(), "A") != true)
                {
                    response.message = "Unauthorized user";
                    response.status = "Failure";
                    response.ResCode = "ykR20038";
                    return response;
                }
                else if (Data.EnitityMarchantKey == "")
                {
                    response.message = "Incomplete data";
                    response.status = "Failure";
                    response.ResCode = "ERR000";
                    return response;
                }

                else if (Data.EnitityMarchantKey != "" && CheckMandateInfo.ValidateEntityMerchantKey(Data.EnitityMarchantKey, Data.AppId) != true)
                {
                    response.message = "Invalid EnitityMarchantKey";
                    response.status = "Failure";
                    response.ResCode = "ykR20021";
                    return response;
                }
               

               
                else
                {
                    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(Data.AppId)].ConnectionString);
                    bool Flag = false;
                  //  string temp = ConfigurationManager.AppSettings["EnitityMarchantKey" + Data.AppId];
                    string UserId = "";
                    string query = "Sp_WebAPI";
                    //if (temp.Trim() == DBsecurity.Decrypt(Data.EnitityMarchantKey))
                    //{
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@QueryType", "GetEntityUser");
                    cmd.Parameters.AddWithValue("@appId", Data.AppId);
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
                          con.Open();
                            query = "Sp_WebAPI";
                             cmd = new SqlCommand(query, con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@QueryType", "GetMandateStatus");
                            cmd.Parameters.AddWithValue("@MandateId", Data.MandateID);
                             da = new SqlDataAdapter(cmd);
                             dt = new DataTable();
                            da.Fill(dt);
                            con.Close();
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                response.message = "Successfully received";
                                response.ResCode = "KPY000";
                                response.status = "Success";
                                response.MandateId = Data.MandateID;
                                response.Mandatestatus = Convert.ToString(dt.Rows[0]["status"]);
                            }
                        }
                        
                    }

                    if (Data.ScanImage == "")
                {
                    response.message = "Incomplete data";
                    response.status = "Failure";
                    response.ResCode = "ERR000";
                    return response;
                }
                if (Data.ScanImage == null)
                {
                    response.message = "Incomplete data";
                    response.status = "Failure";
                    response.ResCode = "ERR000";
                    return response;
                }
                if (Data.MandateID == "")
                {
                    response.message = "Incomplete data";
                    response.status = "Failure";
                    response.ResCode = "ERR000";
                    return response;
                }
                if (Data.MandateID == null)
                {
                    response.message = "Incomplete data";
                    response.status = "Failure";
                    response.ResCode = "ERR000";
                    return response;
                }

                else if (!CheckMandateInfo.CheckManadateID(Data.MandateID, Data.AppId))
                {
                    response.message = "MandateId is not exist";
                    response.status = "Failure";
                    response.ResCode = "ERR0003";
                    return response;
                }

                else
                {

                    byte[] data = Convert.FromBase64String(Data.ScanImage);
                    var filename = Data.MandateID + ".jpg";
                    var path = "/QRCodeImage/";
                    bool exists = System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(path));

                    if (!exists)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(path));
                    }

                    var file = HttpContext.Current.Server.MapPath(path + filename);
                    System.IO.File.WriteAllBytes(file, data);
                    path = path + filename;

                    Bitmap bitmap1 = new Bitmap(file);
                    int x = 0, y = 0, width = 1531, height = 486;
                    Bitmap CroppedImage = bitmap1.Clone(new System.Drawing.Rectangle(x, y, width, height), bitmap1.PixelFormat);
                    LuminanceSource source;
                    source = new BitmapLuminanceSource(CroppedImage);
                    BinaryBitmap bitmap = new BinaryBitmap(new HybridBinarizer(source));
                    Result result = new MultiFormatReader().decode(bitmap);

                    string finalresult = result.ToString();
                    response.message = finalresult;
                    bitmap1.Dispose();
                    CroppedImage.Dispose();
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    File.Delete(file);
                    return response;
                }
            }
            catch (Exception Ex)
            {
                response.message = "Incomplete data";
                return response;
            }
        }
    }




    public class GetMandateResponseforQRcode
    {
        public string message { get; set; }
        public string status { get; set; }
        public string MandateId { get; set; }
        public string Mandatestatus { get; set; }
        public string ResCode { get; set; }
    }
}


