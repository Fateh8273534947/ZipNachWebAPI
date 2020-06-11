using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Globalization;
using ImageUploadWCF;
using System.IO;
using QRCoder;
using System.Drawing;

namespace ZipNachWebAPI.Models
{
    public static class CheckMandateInfo
    {
        public static void QRGenerator(string MandateID, string EntityId, string AppId, string Ref1)
        {


            //   CommonManger CommonManger = new CommonManger();
            string code = MandateID + "_" + Ref1;
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.H);
            System.Web.UI.WebControls.Image imgBarCode = new System.Web.UI.WebControls.Image();
            imgBarCode.Height = 250;
            imgBarCode.Width = 250;
            using (Bitmap bitMap = qrCode.GetGraphic(20))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] byteImage = ms.ToArray();
                    imgBarCode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                    string result = Convert.ToBase64String(byteImage, 0, byteImage.Length);
                    string check = CreateImage(result.ToString(), MandateID, AppId);
                    if (check != "")
                    {
                        //    DataSet ds = CommonManger.FillDatasetWithParam("Sp_Mandate", "@QueryType", "@EntityId", "@MandateId", "@QRCodeImagepath", "QRCodeImagepath", EntityId, MandateID, check);
                        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(AppId)].ConnectionString);
                        string query = "Sp_Mandate";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@QueryType", "QRCodeImagepath");
                        cmd.Parameters.AddWithValue("@EntityId", EntityId);
                        cmd.Parameters.AddWithValue("@MandateId", MandateID);
                        cmd.Parameters.AddWithValue("@QRCodeImagepath", check);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                    }

                }
                //plQRCode.Controls.Add(imgBarCode);
            }
        }
        public static string CreateImage(string Byt, string MandateID, string AppId)
        {
            try
            {
                byte[] data = Convert.FromBase64String(Byt);
                if (!Directory.Exists(ConfigurationManager.AppSettings["FileUploadPath" + AppId] + @"MandateQrcode\"))
                    Directory.CreateDirectory(ConfigurationManager.AppSettings["FileUploadPath" + AppId] + @"MandateQrcode\");
                var filename = MandateID + ".png";
                var path = "/MandateQrcode/";
                var file = ConfigurationManager.AppSettings["FileUploadPath" + AppId] + @"MandateQrcode\" + filename;//HttpContext.Current.Server.MapPath(path + filename);
                //FileUpload1.PostedFile.SaveAs(Server.MapPath("~\\Uploadform\\" + filename.Trim()));
                //fileuploadimages.SaveAs(Server.MapPath("~/" + filename));
                System.IO.File.WriteAllBytes(file, data);
                path = path + filename;
                return path;
            }

            catch (Exception e)
            {
                return "Error";

            }
        }
        public static bool CheckCategory(string CategoryCode, string AppId)
        {
            bool status = false;
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(AppId)].ConnectionString);
                string query = "Sp_WebAPI";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "CheckCategory");
                cmd.Parameters.AddWithValue("@CategoryCode", CategoryCode);
                cmd.Parameters.AddWithValue("@appId", AppId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    status = true;
                }
                return status;

            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
                return status;
            }
        }
        public static byte[] GetRupeeIcon(string AppId)
        {
            DataSet dsData = new DataSet();
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(AppId)].ConnectionString);
            string query = "Sp_LogoImageData";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@QueryType", "GetRupeeIcon");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dsData);
            return (byte[])dsData.Tables[0].Rows[0][0];

        }
        public static string CheckQrLogo(string EntityId, string AppId)
        {

            DataSet ds = new DataSet();

            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(AppId)].ConnectionString);
                string query = "Sp_Mandate";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "ChecKLogoOrImage");
                cmd.Parameters.AddWithValue("@EntityId", EntityId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                bool QR = Convert.ToBoolean(ds.Tables[0].Rows[0]["PRINTQR"]);
                bool Logo = Convert.ToBoolean(ds.Tables[0].Rows[0]["PRINTLOGO"]);
                if (QR == true)
                {
                    return "1";
                }
                else
                {
                    return "0";
                }

            }
            catch
            {
                return "0";
            }



        }
        public static byte[] GetcutterImage(string AppId)
        {
            DataSet dsData = new DataSet();

            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(AppId)].ConnectionString);
                string query = "Sp_LogoImageData";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "Getcutternew");
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dsData);
                return (byte[])dsData.Tables[0].Rows[0][0];

            }
            catch (Exception ex)
            {
                // RaiseError(ex.Message);
                return null;
            }
        }
        public static DataSet GetShowForPDF(string ID, string AppId)
        {
            DataSet dsData = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(AppId)].ConnectionString);
                string query = "Sp_GetEmailData";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "ShowData");
                cmd.Parameters.AddWithValue("@Id", ID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dsData);
                return dsData;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        //  CommonManger.FillDatasetWithParam("", "@QueryType", "@MandateId", "@EmailCount", "@SmsCount", "SendMail", Convert.ToString(dt.Rows[l]["MandateId"]), "0", "1");
        public static string SendMailCount(string MandateID, string AppId, string EmailCount, string SmsCount,int SMSLength,string  MessageRequestId)
        {
            string status = "";
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(AppId)].ConnectionString);
                string query = "Sp_SendEmail";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "SendMail");
                cmd.Parameters.AddWithValue("@MandateID", MandateID);
                cmd.Parameters.AddWithValue("@EmailCount", EmailCount);
                cmd.Parameters.AddWithValue("@SmsCount", SmsCount);
                 cmd.Parameters.AddWithValue("@SMSLength", SMSLength);
                cmd.Parameters.AddWithValue("@MessageRequestId", MessageRequestId); 
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    status = "";
                }
               

            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
                return ex.Message;
            }
            return status;
        }
        public static bool CheckSMSAndEMail(string EntityId, string AppId, string Type)
        {
            bool status = false;
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(AppId)].ConnectionString);
                string query = "Sp_WebAPI";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "CheckSMSAndEMail");
                cmd.Parameters.AddWithValue("@EntityId", EntityId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    if (Type == "E" && (Convert.ToString(dt.Rows[0]["NetValidateMail"]).ToUpper() == "TRUE" || Convert.ToString(dt.Rows[0]["DebitValidateMail"]).ToUpper() == "TRUE"))
                    {
                        status = true;
                    }
                    if (Type == "S" && (Convert.ToString(dt.Rows[0]["DebitSMS"]).ToUpper() == "TRUE" || Convert.ToString(dt.Rows[0]["NetSMS"]).ToUpper() == "TRUE"))
                    {
                        status = true;
                    }
                }
                return status;

            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
                return status;
            }
        }
        public static bool CheckMandateMode(string MandateID, string AppId)
        {
            bool status = false;
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(AppId)].ConnectionString);
                string query = "Sp_WebAPI";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "CheckMandateMode");
                cmd.Parameters.AddWithValue("@MandateID", MandateID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    status = true;
                }
                return status;

            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
                return status;
            }
        }
        public static DataTable GetMandateData(string MandateID, string AppId)
        {

            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(AppId)].ConnectionString);
                string query = "Sp_WebAPI";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "GetMandateData");
                cmd.Parameters.AddWithValue("@MandateID", MandateID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;

            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
                return null;
            }
        }
        public static bool CheckManadateID(string MandateID, string AppId)
        {
            bool status = false;
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(AppId)].ConnectionString);
                string query = "Sp_WebAPI";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "CheckMandateInfo");
                cmd.Parameters.AddWithValue("@MandateID", MandateID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    status = true;
                }
                return status;

            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
                return status;
            }
        }
        public static bool CheckAmt(string MandateID, string AppId)
        {
            bool status = true;
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(AppId)].ConnectionString);
                string query = "Sp_WebAPI";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "CheckMandateInfo");
                cmd.Parameters.AddWithValue("@MandateID", MandateID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToDecimal(dt.Rows[0]["AmountRupees"]) > Convert.ToDecimal(ConfigurationManager.AppSettings["MaxEmandateAmt"]))
                    {
                        status = false;
                    }
                }
                return status;

            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
                return status;
            }
        }
        public static string GetRefNO(string MandateID, string AppId)
        {
            string status = "";
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(AppId)].ConnectionString);
                string query = "Sp_WebAPI";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "CheckMandateInfo");
                cmd.Parameters.AddWithValue("@MandateID", MandateID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    status = Convert.ToString(dt.Rows[0]["Refrence1"]);
                }
                return status;

            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
                return status;
            }
        }

        public static bool PrintQRcode(string entityid, string AppId)
        {
            bool status = false;

            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(AppId)].ConnectionString);
                string query = "Sp_WebAPI";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "GetPrintQRCode");
                cmd.Parameters.AddWithValue("@entityid", entityid);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["PrintQR"].ToString() == "True")
                    {
                        status = true;
                    }

                    else

                    {
                        status = false;
                    }

                }

                return status;

            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
                return status;
            }
        }

        public static bool CheckAccountValidation(string MandateID, string AppId)
        {
            bool status = false;
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(AppId)].ConnectionString);
                string query = "Sp_WebAPI";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "CheckAccountValidation");
                cmd.Parameters.AddWithValue("@MandateID", MandateID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    status = true;
                }
                return status;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
                return status;
            }

        }

        public static bool CheckPHENachValidation(string MandateID, string AppId)
        {
            bool status = false;
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(AppId)].ConnectionString);
                string query = "Sp_WebAPI";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "CheckPHENachValidation");
                cmd.Parameters.AddWithValue("@MandateID", MandateID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    status = true;
                }
                return status;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
                return status;
            }
        }
        public static bool CheckImageValidation(string MandateID, string AppId)
        {
            bool status = false;
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(AppId)].ConnectionString);
                string query = "Sp_WebAPI";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "CheckImageValidation");
                cmd.Parameters.AddWithValue("@MandateID", MandateID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToString(dt.Rows[0]["JPGPath"]).Trim() != "" && Convert.ToString(dt.Rows[0]["TIPPath"]).Trim() != "")
                    {
                        if (File.Exists(ConfigurationManager.AppSettings["FileUploadPath" + AppId].ToString() + Convert.ToString(dt.Rows[0]["JPGPath"]).Trim().Substring(3, Convert.ToString(dt.Rows[0]["JPGPath"]).Trim().Length - 3)) && File.Exists(ConfigurationManager.AppSettings["FileUploadPath" + AppId].ToString() + Convert.ToString(dt.Rows[0]["TIPPath"]).Trim().Substring(3, Convert.ToString(dt.Rows[0]["TIPPath"]).Trim().Length - 3)))
                        {
                            status = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
            }
            return status;

        }
        public static bool CheckENachValidation(string MandateID, string AppId)
        {
            bool status = false;
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(AppId)].ConnectionString);
                string query = "Sp_WebAPI";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "CheckENachValidation");
                cmd.Parameters.AddWithValue("@MandateID", MandateID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    status = true;
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
            }
            return status;

        }
        public static bool ValidateDateDateOnMandate(string DateOnMandate)
        {
            bool status = false;
            try
            {
                string s = DateOnMandate; // or 2015-11-19
                DateTime dt;
                string[] formats = { "yyyy/MM/dd", "yyyy/MM/dd" };
                if (DateTime.TryParseExact(s, formats, CultureInfo.InvariantCulture,
                                          DateTimeStyles.None, out dt))

                    //    DateTime dateValue= Convert.ToDateTime(DateOnMandate);
                    //if (DateTime.TryParseExact(DateOnMandate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
                    //{

                    if (Convert.ToDateTime(s).Date >= DateTime.Now.Date)
                    {
                        status = true;
                    }
                // }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
            }
            return status;
        }
        public static bool ValidateDate(string DateOnMandate)
        {
            bool status = false;
            try
            {
                string s = DateOnMandate; // or 2015-11-19
                DateTime dt;
                string[] formats = { "yyyy/MM/dd", "yyyy/MM/dd" };
                if (DateTime.TryParseExact(s, formats, CultureInfo.InvariantCulture,
                                          DateTimeStyles.None, out dt))

                    //    DateTime dateValue= Convert.ToDateTime(DateOnMandate);
                    //if (DateTime.TryParseExact(DateOnMandate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
                    //{
                    if (Convert.ToDateTime(s).Date > DateTime.Now.Date)
                    {
                        status = true;
                    }
                // }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
            }
            return status;
        }
        public static bool ValidateDate(string DateOnMandate, string FromDate)
        {
            bool status = true;
            try
            {
                DateTime DateOnMandate1 = Convert.ToDateTime(DateOnMandate);
                DateTime FromDate1 = Convert.ToDateTime(FromDate);
                if (DateOnMandate1.Date <= FromDate1.Date)
                {
                    status = false;
                }
                //string s = DateOnMandate; // or 2015-11-19
                //DateTime dt;
                //string[] formats = { "yyyy/MM/dd", "yyyy/MM/dd" };
                //if (DateTime.TryParseExact(s, formats, CultureInfo.InvariantCulture,  DateTimeStyles.None, out dt))

                //    //    DateTime dateValue= Convert.ToDateTime(DateOnMandate);
                //    //if (DateTime.TryParseExact(DateOnMandate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
                //    //{

                //// }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
            }
            return status;
        }
        public static bool ValidateEmail(string EmailID)
        {
            bool status = false;
            try
            {
                string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
                Regex re = new Regex(strRegex);
                if (re.IsMatch(EmailID))
                    status = true;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
            }
            return status;
        }
        public static bool ValidateMobileNo(string MobileNo)
        {
            bool status = false;
            try
            {
                char[] arr = MobileNo.ToCharArray();
                //if (arr.Length == 10)                    
                //    status = true;
                Regex regex = new Regex(@"^[-+]?[0-9]*\.?[0-9]+$");
                if (regex.IsMatch(MobileNo) && arr.Length == 10)
                    status = true;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
            }
            return status;
        }

        public static bool ValidateMandateType(string MandateType)
        {
            bool status = false;
            try
            {
                if (MandateType == "1" || // Create Request at NPCI
                MandateType == "2" ||  //  Modify Request at NPCI,
                MandateType == "3"     //  Mandate Cancel Request at NPC            
            )
                    status = true;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
            }
            return status;
        }

        public static bool ValidateToDebit(string ToDebit)
        {
            bool status = false;
            try
            {
                if (ToDebit == "SB" || ToDebit == "CA" || ToDebit == "CC" || ToDebit == "SB-NRE" || ToDebit == "SB-NRO" || ToDebit == "Other")
                    status = true;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
            }
            return status;
        }

        public static bool ValidateFrequency(string Frequency)
        {
            bool status = false;
            try
            {
                if (Frequency == "M" || //‘Monthly’
                Frequency == "Q" || //  ’Quarterly’
                Frequency == "H" || // ‘Half-Yearly’
                Frequency == "Y" || // ’Yearly’ 
                Frequency == "A"  // ’As & When Presented’               
            )
                    status = true;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
            }
            return status;
        }

        public static bool ValidateToDebitType(string DebitType)
        {
            bool status = false;
            try
            {
                if (DebitType == "F" ||
                DebitType == "M"
            )
                    status = true;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
            }
            return status;
        }

        public static bool CheckSponserCode(string Sponsercode, string Enitity, string AppId)
        {
            bool status = false;
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(AppId)].ConnectionString);
                string query = "Sp_WebAPI";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "GetSponserResponse");
                cmd.Parameters.AddWithValue("@entityid", Enitity);
                cmd.Parameters.AddWithValue("@SponsorCode", Sponsercode);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    status = true;
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
            }
            return status;
        }

        public static bool CheckUtilityCode(string UtilityCode, string SponserCode, string Enitity, string AppId)
        {
            bool status = false;
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(AppId)].ConnectionString);
                string query = "Sp_WebAPI";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "GetUtilityResponse");
                cmd.Parameters.AddWithValue("@entityid", Enitity);
                cmd.Parameters.AddWithValue("@UtilityCode", UtilityCode);
                cmd.Parameters.AddWithValue("@SponsorCode", SponserCode);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    status = true;
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
            }
            return status;
        }
        public static bool ValidatePrintPr(string PrintPr)
        {
            if (PrintPr == "1" || PrintPr == "0")
            {
                return true;
            }
            else { return false; }
        }
        public static bool ValidateAppID(string AppID)
        {
            bool status = false;
            try
            {
                foreach (ConnectionStringSettings css in ConfigurationManager.ConnectionStrings)
                {
                    string name = css.Name;
                    //string connString = css.ConnectionString;
                    //string provider = css.ProviderName;
                    if (AppID == name)
                    {
                        status = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
            }
            return status;
        }

        public static bool ValidateEntityMerchantKey(string EnitityMarchantKey, string AppID)
        {
            bool status = false;
            try
            {
                //string temp = ConfigurationManager.AppSettings["EnitityMarchantKey" + AppID];
                //if (temp.Trim() == DBsecurity.Decrypt(EnitityMarchantKey))
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(AppID)].ConnectionString);
                string query = "Sp_PresentMentWebApi";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "CheckEnitityMarchantKey");
                cmd.Parameters.AddWithValue("@AppId", AppID);
                cmd.Parameters.AddWithValue("@EnitityMarchantKey", EnitityMarchantKey);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToString(dt.Rows[0]["EnitityMarchantKey"]) == EnitityMarchantKey)
                    {
                        status = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
            }
            return status;
        }

        public static bool ValidateAmount(string Amount)
        {
            bool status = false;
            try
            {
                if (Regex.IsMatch(Amount, @"^\$?(\d{1,3},(\d{3},)*\d{3}|\d+)(.\d{1,4})?$"))
                    status = true;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
            }
            return status;
        }
        public static bool ValidateUpdateType(string Type)
        {
            bool status = false;
            try
            {
                if (Type.ToUpper() == "B" || // Create Request at NPCI
                Type.ToUpper() == "M"
            )
                    status = true;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
            }
            return status;
        }
        public static bool ValidateEmandateType(string EMandateType)
        {
            bool status = false;
            try
            {
                if (EMandateType == "A" || EMandateType == "D" || EMandateType == "N")
                    status = true;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
            }
            return status;
        }
        public static bool ValidateEmandateTypeLive(string EMandateType, string MandateId, string AppID)
        {
            bool status = false;
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(AppID)].ConnectionString);
                string query = "Sp_WebAPI";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "ValidateEmandateType");
                cmd.Parameters.AddWithValue("@appId", AppID);
                cmd.Parameters.AddWithValue("@MandateId", MandateId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    //if ( Convert.ToString(dt.Rows[0]["IsLiveInNACH"]).ToUpper() == "TRUE")
                    //{
                    //    status = true;
                    //}
                    if (Convert.ToString(dt.Rows[0]["DebitCard"]).ToUpper() == "1")
                    {
                        status = true;
                    }
                    if (Convert.ToString(dt.Rows[0]["Netbanking"]).ToUpper() == "1")
                    {
                        status = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
            }
            return status;
        }
        public static DataSet GetData_MandateID(string MandateId, string AppID, string WebAppUrl, string Activity)
        {
            DataSet dtset = new DataSet();
            try
            { 

                string TempId = AppID + MandateId;
                TempId = Global.ReverseString(TempId);
                TempId = Global.CreateRandomCode(6) + TempId;
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(AppID)].ConnectionString);
                string query = "Sp_SendEmail";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "GetData_MandateID");
                cmd.Parameters.AddWithValue("@MandateId", MandateId);
                cmd.Parameters.AddWithValue("@Activity", Activity);
                cmd.Parameters.AddWithValue("@WebAppUrl", WebAppUrl);
                cmd.Parameters.AddWithValue("@EncodedMandateID", TempId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dtset);


            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
            }
            return dtset;
        }
        public static string CheckMandateId(string AppID, string MandeteId)
        {
            string Temp = "";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(AppID)].ConnectionString);
            string query = "Sp_Mandate";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@QueryType", "CheckMandateId");
            cmd.Parameters.AddWithValue("@MandateId", MandeteId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                Temp = dt.Rows[0]["value"].ToString();
            }
            return Temp;
        }
    }
}