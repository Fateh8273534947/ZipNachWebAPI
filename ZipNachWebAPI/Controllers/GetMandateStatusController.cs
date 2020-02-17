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
    //Latest
    [EnableCors(origins: "http://192.168.1.246:808/api/Clients", headers: "AllowAllHeaders", methods: "*")]
    public class GetMandateStatusController : ApiController
    {

        [Route("api/UploadMandate/GetMandateStatus")]
        //[Route("api/GetMandateStatus/GetMandateStatus")]
        [HttpPost]
        public GetMandateResponse GetMandateInfo(GetMandateReq Data)
        {
            GetMandateResponse response = new GetMandateResponse();
            if (Data.AppID == "")
            {
                response.Message = "Incomplete data";
                response.Status = "Failure";
                response.ResCode = "ykR20020";
                return response;
            }
            else if (Data.AppID != "" && CheckMandateInfo.ValidateAppID(Data.AppID) != true)
            {
                response.Message = "Invalid AppId";
                response.Status = "Failure";
                response.ResCode = "ykR20023";

                return response;
            }
            //else if (ValidatePresement.CheckAccess(Data.AppID.Trim(), "A") != true)
            //{
            //    response.Message = "Unauthorized user";
            //    response.Status = "Failure";
            //    response.ResCode = "ykR20038";
            //    return response;
            //}
            else if (Data.MdtID == "")
            {
                response.Message = "Incomplete data";
                response.Status = "Failure";
                response.ResCode = "ykR20020";
                return response;
            }

            else if (!CheckMandateInfo.CheckManadateID(Data.MdtID, Data.AppID))
            {
                response.Message = "Invalid MandateId";
                response.Status = "Failure";
                response.ResCode = "ykR20022";
                return response;
            }
            else if (Data.MerchantKey == "")
            {
                response.Message = "Incomplete data";
                response.Status = "Failure";
                response.ResCode = "ykR20020";
                return response;
            }
            else if (Data.MerchantKey != "" && CheckMandateInfo.ValidateEntityMerchantKey(Data.MerchantKey, Data.AppID) != true)
            {
                response.Message = "Invalid MerchantKey";
                response.Status = "Failure";
                response.ResCode = "ykR20021";
                return response;
            }
            else
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(Data.AppID)].ConnectionString);
                bool Flag = false;
                //string temp = ConfigurationManager.AppSettings["EnitityMarchantKey" + Data.AppID];
                string UserId = "";
                string query = "Sp_WebAPI";
                //if (temp.Trim() == DBsecurity.Decrypt(Data.MerchantKey))
                //{
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "GetEntityUser");
                cmd.Parameters.AddWithValue("@appId", Data.AppID);
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
                    try
                    {
                        con.Open();
                        query = "Sp_WebAPI";
                        cmd = new SqlCommand(query, con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@QueryType", "GetMandateStatus");
                        cmd.Parameters.AddWithValue("@MandateId", Data.MdtID);
                        da = new SqlDataAdapter(cmd);
                        dt = new DataTable();
                        da.Fill(dt);
                        con.Close();
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            string Nach = Convert.ToString(dt.Rows[0]["Nach"]) == "True" ? "Yes" : "No";
                            string Aadhar = Convert.ToString(dt.Rows[0]["IsLiveInNACH"]) == "True" ? "Yes" : "No";
                            string Netbanking = Convert.ToString(dt.Rows[0]["Netbanking"]) == "1" ? "Yes" : "No";
                            string Debitcard = Convert.ToString(dt.Rows[0]["DebitCard"]) == "1" ? "Yes" : "No";
                            string UPI = Convert.ToString(dt.Rows[0]["Is_UPI"]) == "True" ? "Yes" : "No";
                            string Lateststatus = Convert.ToString(dt.Rows[0]["status"]);
                            string StatusDescription = Convert.ToString(dt.Rows[0]["StatusDescription"]);
                            string isAggr = "";
                            if (Convert.ToString(dt.Rows[0]["IsAggregator"]).ToLower() == "true")
                            {
                                isAggr = "1";
                            }
                            else
                            {
                                isAggr = "0";
                            }
                            response.Message = "Mandate Status received";
                            response.ResCode = "ykR20035";
                            response.Status = "Success";
                            response.MandateId = Data.MdtID;
                            response.Mandatestatus = Convert.ToString(dt.Rows[0]["status"]);
                            //response.MandateData = "<MandateData><AppID>" + Data.AppID + "</AppID><MerchantKey>" + Data.MerchantKey + "</MerchantKey><MandateId>" + Data.MdtID + "</MandateId><MandateMode> " + Convert.ToString(dt.Rows[0]["MandateType"]) + " </MandateMode><DateOnMandate> " + Convert.ToString(dt.Rows[0]["DateOnMandate"]) + "</DateOnMandate><SponsorCode>" + Convert.ToString(dt.Rows[0]["SponsorbankCode"]) + "</SponsorCode><UtilityCode> " + Convert.ToString(dt.Rows[0]["UtilityCode"]) + " </UtilityCode ><ToDebit>" + Convert.ToString(dt.Rows[0]["ToDebit"]) + "</ToDebit ><BankName>" + Convert.ToString(dt.Rows[0]["BankName"]) + "</BankName><AcNo>" + Convert.ToString(dt.Rows[0]["AcNo"]) + "</AcNo><IFSC> " + Convert.ToString(dt.Rows[0]["IFSC"]) + " </IFSC><MICR>" + Convert.ToString(dt.Rows[0]["MICR"]) + "</MICR><AmountRupees>" + Convert.ToString(dt.Rows[0]["AmountRupees"]) + "</AmountRupees><Frequency>" + Convert.ToString(dt.Rows[0]["Frequency"]) + "</Frequency><DebitType>" + Convert.ToString(dt.Rows[0]["DebitType"]) + "</DebitType><Refrence1> " + Convert.ToString(dt.Rows[0]["Refrence1"]) + " </Refrence1><Refrence2> " + Convert.ToString(dt.Rows[0]["Refrence2"]) + " </Refrence2>< PhNumber> " + Convert.ToString(dt.Rows[0]["PhoneNumber"]) + " </PhNumber><EmailId> " + Convert.ToString(dt.Rows[0]["EmailId"]) + " </EmailId><From> " + Convert.ToString(dt.Rows[0]["FromDate"]) + "</From><To> " + Convert.ToString(dt.Rows[0]["Todate"]) + " </To>< Customer1> " + Convert.ToString(dt.Rows[0]["Customer1"]) + " </Customer1><Customer2> " + Convert.ToString(dt.Rows[0]["Customer2"]) + "</Customer2><Customer3> " + Convert.ToString(dt.Rows[0]["Customer3"]) + "</Customer3><Nach>" + Nach + "</Nach><EMandateAadhar>" + Aadhar + "</EMandateAadhar><EMandateNetBanking>" + Netbanking + "</EMandateNetBanking><EMandateDebitCard>" + Debitcard + "</EMandateDebitCard><EMandateUPI>" + UPI + "</EMandateUPI><MandateStatus>" + Lateststatus + "</MandateStatus><IsAggregator>" + isAggr + "</IsAggregator><SubMerchantId>" + Convert.ToString(dt.Rows[0]["IsAggregatorValue"]) + "</SubMerchantId><CategoryCode>" + Convert.ToString(dt.Rows[0]["CategoryCode"]) + "</CategoryCode></MandateData>";
                            response.MandateData = "<MandateData><AppID>" + Data.AppID + "</AppID><MerchantKey>" + Data.MerchantKey + "</MerchantKey><MdtID>" + Data.MdtID + "</MdtID><MType>" + Convert.ToString(dt.Rows[0]["MandateType"]) + "</MType><MDate>" + Convert.ToString(dt.Rows[0]["DateOnMandate"]) + "</MDate><SpBankCode>" + Convert.ToString(dt.Rows[0]["SponsorbankCode"]) + "</SpBankCode><UTLSCode>" + Convert.ToString(dt.Rows[0]["UtilityCode"]) + "</UTLSCode><TDebit>" + Convert.ToString(dt.Rows[0]["ToDebit"]) + "</TDebit><BankName>" + Convert.ToString(dt.Rows[0]["BankName"]) + "</BankName><BankAc>" + Convert.ToString(dt.Rows[0]["AcNo"]) + "</BankAc><IFSC>" + Convert.ToString(dt.Rows[0]["IFSC"]) + "</IFSC><MICR>" + Convert.ToString(dt.Rows[0]["MICR"]) + "</MICR><Amt>" + Convert.ToString(dt.Rows[0]["AmountRupees"]) + "</Amt><Frequency>" + Convert.ToString(dt.Rows[0]["Frequency"]) + "</Frequency><DType>" + Convert.ToString(dt.Rows[0]["DebitType"]) + "</DType><Ref1>" + Convert.ToString(dt.Rows[0]["Refrence1"]) + "</Ref1><Ref2>" + Convert.ToString(dt.Rows[0]["Refrence2"]) + "</Ref2><Phone>" + Convert.ToString(dt.Rows[0]["PhoneNumber"]) + "</Phone><Email>" + Convert.ToString(dt.Rows[0]["EmailId"]) + "</Email><PFrom>" + Convert.ToString(dt.Rows[0]["FromDate"]) + "</PFrom><PTo>" + Convert.ToString(dt.Rows[0]["Todate"]) + "</PTo><Cust1>" + Convert.ToString(dt.Rows[0]["Customer1"]) + "</Cust1><Cust2>" + Convert.ToString(dt.Rows[0]["Customer2"]) + "</Cust2><Cust3>" + Convert.ToString(dt.Rows[0]["Customer3"]) + "</Cust3><Nach>" + Nach + "</Nach><EMandateAadhar>" + Aadhar + "</EMandateAadhar><EMandateNetBanking>" + Netbanking + "</EMandateNetBanking><EMandateDebitCard>" + Debitcard + "</EMandateDebitCard><EMandateUPI>" + UPI + "</EMandateUPI><MandateStatus>" + Lateststatus + "</MandateStatus><IsAggregator>" + isAggr + "</IsAggregator><SubMerchantId>" + Convert.ToString(dt.Rows[0]["IsAggregatorValue"]) + "</SubMerchantId><CategoryCode>" + Convert.ToString(dt.Rows[0]["CategoryCode"]) + "</CategoryCode><AcceptRefNo>" + Convert.ToString(dt.Rows[0]["AcceptRefNo"]) + "</AcceptRefNo><MndtReqId>" + Convert.ToString(dt.Rows[0]["MSGId"]) + "</MndtReqId><NPCIRefMsgId>" + Convert.ToString(dt.Rows[0]["NPCIMsgId"]) + "</NPCIRefMsgId><StatusDescription>" + StatusDescription + "</StatusDescription><UMRN>" + Convert.ToString(dt.Rows[0]["UMRN"]) + "</UMRN></MandateData>";
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Out.WriteLine("-----------------");
                        Console.Out.WriteLine(ex.Message);
                    }
                }
                else
                {
                    response.Status = "Failure";
                    response.ResCode = "ykR20020";
                    response.Message = "Invalid data";
                    return response;
                }
            }
            return response;
        }
        [Route("api/GetMandateStatus/ReadQrCode")]
        [HttpPost]
        public GetMandateResponse ReadQrCode(QRImage Data)
        {
            GetMandateResponse response = new GetMandateResponse();
            try
            {
                if (Data.base64Image == "")
                {
                    response.Message = "Incomplete data";
                    response.Status = "Failure";
                    response.ResCode = "ykR20020";
                    return response;
                }
                if (Data.base64Image == null)
                {
                    response.Message = "Incomplete data";
                    response.Status = "Failure";
                    response.ResCode = "ykR20020";
                    return response;
                }
                if (Data.MandateID == "")
                {
                    response.Message = "Incomplete data";
                    response.Status = "Failure";
                    response.ResCode = "ykR20020";
                    return response;
                }
                if (Data.MandateID == null)
                {
                    response.Message = "Incomplete data";
                    response.Status = "Failure";
                    response.ResCode = "ykR20020";
                    return response;
                }
                else
                {

                    byte[] data = Convert.FromBase64String(Data.base64Image);
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
                  //  int x = 0, y = 0, width = 1531, height = 486;
                    int x = 0, y = 0, width = 260, height = 190;
                    Bitmap CroppedImage = bitmap1.Clone(new System.Drawing.Rectangle(x+124, y+198, width , height), bitmap1.PixelFormat);
                    LuminanceSource source;
                    source = new BitmapLuminanceSource(CroppedImage);
                   // CroppedImage.Save("E:\\abc.jpg");
                    BinaryBitmap bitmap = new BinaryBitmap(new HybridBinarizer(source));
                    Result result = new MultiFormatReader().decode(bitmap);

                    string finalresult = result.ToString();
                    response.Message = finalresult;
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
                response.Message = "Incomplete data";
                return response;
            }
        }
        public class GetMandateResponse
        {
            public string Message { get; set; }
            public string Status { get; set; }
            public string MandateId { get; set; }
            public string Mandatestatus { get; set; }
            public string ResCode { get; set; }
            public string MandateData { get; set; }
        }
        public class GetSendLinkResponse
        {
            public string Message { get; set; }
            public string Status { get; set; }
            public string ResCode { get; set; }
        }
    }
}
