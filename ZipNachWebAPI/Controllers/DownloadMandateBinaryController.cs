using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Script.Serialization;
using ImageUploadWCF;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using ZipNachWebAPI.Models;

namespace ZipNachWebAPI.Controllers
{
    //[EnableCors(origins: "http://localhost:49979/", headers: "AllowAllHeaders", methods: "*")]
    [EnableCors(origins: "http://192.168.1.246:808/api/Clients", headers: "AllowAllHeaders", methods: "*")]
    //[EnableCors("AllowAllHeaders")]
    public class DownloadMandateBinaryController : ApiController
    {
        [Route("api/UploadMandate/physicalmandateBase64")]
        [HttpPost]
        public BinaryDownloadResponse physicalmandate(BinaryDownloadmandatePdf context)
        {
           // HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            BinaryDownloadResponse res = new BinaryDownloadResponse();
            string fileName = "Reference" + DateTime.Now.ToString("yyyyMMddmmssff") + ".pdf";
            var pdfPath = HttpContext.Current.Server.MapPath(@"~/Files/");
            if (!Directory.Exists(pdfPath))
            {
                Directory.CreateDirectory(pdfPath);
            }
            pdfPath = pdfPath + fileName;
            Document pdfDoc = new Document(PageSize.A4, 20f, 20f, 10f, 10f);
            //System.IO.FileStream _fs = new FileStream(pdfPath, FileMode.Create);
            //iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(pdfDoc, _fs);
            //writer.CloseStream = false;
            MemoryStream memoryStream = new MemoryStream();
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);

            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
            writer.CloseStream = false;
            pdfDoc.Open();
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            //var json = request.Content.ReadAsStringAsync().Result;
            //var JSONObj = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(json);

            //BinaryDownloadmandatePdf context = new BinaryDownloadmandatePdf();

            //context.AppID = JSONObj["AppID"];
            //context.MandateId = JSONObj["MdtID"];
            //context.MerchantKey = JSONObj["MerchantKey"];
            //context.PhysicalNach = JSONObj["PhysicalNach"];
           // context.PrintQr = JSONObj["PrintQr"];
            if (context.AppID == "")
            {
                res.Message = "Incomplete data";
                res.Status = "Failure";
                res.ResCode = "ykR20020";
                //  HttpResponseMessage responseMessage = Request.CreateResponse(HttpStatusCode.Created, res);
                //  HttpResponseMessage responseMessage = Request.CreateResponse(HttpStatusCode.Created, "Failure OR ERR000 OR Incomplete data");
                //   return responseMessage;
                return res;
            }
           
            else if (context.AppID != "" && CheckMandateInfo.ValidateAppID(context.AppID) != true)
            {
                res.Message = "Invalid AppID";
                res.Status = "Failure";
                res.ResCode = "ykR20023";
            //    HttpResponseMessage responseMessage = Request.CreateResponse(HttpStatusCode.Created, res);
               // HttpResponseMessage responseMessage = Request.CreateResponse(HttpStatusCode.Created, "Failure OR ykR20023 OR Invalid Application Id");
                return res;
            }
            //else if (ValidatePresement.CheckAccess(context.AppID.Trim(), "A") != true)
            //{
            //    HttpResponseMessage responseMessage = Request.CreateResponse(HttpStatusCode.Created, "Failure OR ykR20038 ORUnauthorized user");
            //    return responseMessage;
            //}
            //else if (context.PrintQr == "")
            //{
            //    HttpResponseMessage responseMessage = Request.CreateResponse(HttpStatusCode.Created, "Failure OR ERR000 OR Incomplete data");
            //    return responseMessage;
            //}
            //else if (context.PrintQr != ""&&CheckMandateInfo.ValidatePrintPr(context.PrintQr)!=true)
            //{
            //    HttpResponseMessage responseMessage = Request.CreateResponse(HttpStatusCode.Created, "Failure OR ykR20023 OR Invalid PrintQr");
            //    return responseMessage;
            //}
            else if (context.PhysicalNach == "")
            {
                res.Message = "Incomplete data";
                res.Status = "Failure";
                res.ResCode = "ykR20020";
                HttpResponseMessage responseMessage = Request.CreateResponse(HttpStatusCode.Created, res);
               // HttpResponseMessage responseMessage = Request.CreateResponse(HttpStatusCode.Created, "Failure OR ERR000 OR Incomplete data");
                return res;

            }
            else if (context.PhysicalNach != "1")
            {
                res.Message = "Invalid data";
                res.Status = "Failure";
                res.ResCode = "ykR20020";
                HttpResponseMessage responseMessage = Request.CreateResponse(HttpStatusCode.Created, res);
             //   HttpResponseMessage responseMessage = Request.CreateResponse(HttpStatusCode.Created, "Failure OR ERR000 OR Invalid PhysicalNach");
                return res;
            }
            else if (context.MerchantKey == "")
            {
                res.Message = "Incomplete data";
                res.Status = "Failure";
                res.ResCode = "ykR20020";
                HttpResponseMessage responseMessage = Request.CreateResponse(HttpStatusCode.Created, res);
              //  HttpResponseMessage responseMessage = Request.CreateResponse(HttpStatusCode.Created, "Failure OR ERR000 OR Incomplete data");
                return res;
            }
            else if (context.MerchantKey != "" && CheckMandateInfo.ValidateEntityMerchantKey(context.MerchantKey, context.AppID) != true)
            {
                res.Message = "Invalid MerchantKey";
                res.Status = "Failure";
                res.ResCode = "ykR20021";
                HttpResponseMessage responseMessage = Request.CreateResponse(HttpStatusCode.Created, res);
                //HttpResponseMessage responseMessage = Request.CreateResponse(HttpStatusCode.Created, "Failure OR ykR20021 OR Invalid MerchantKey");
                return res;
            }
            else if (context.MdtID == "")
            {
                res.Message = "Incomplete data";
                res.Status = "Failure";
                res.ResCode = "ykR20020";
                HttpResponseMessage responseMessage = Request.CreateResponse(HttpStatusCode.Created, res);
               // HttpResponseMessage responseMessage = Request.CreateResponse(HttpStatusCode.Created, "Failure OR ERR000 OR Incomplete data");
                return res;
            }
            else if (!CheckMandateInfo.CheckManadateID(context.MdtID, context.AppID))
            {
                res.Message = "Invalid MandateId";
                res.Status = "Failure";
                res.ResCode = "ykR20022";
                HttpResponseMessage responseMessage = Request.CreateResponse(HttpStatusCode.Created, res);
              //  HttpResponseMessage responseMessage = Request.CreateResponse(HttpStatusCode.Created, "Failure OR ERR000 OR MandateId is not exist");
                return res;
            }
            //else if (!CheckMandateInfo.CheckAccountValidation(context.MandateId, context.AppID))
            //{
            //    res.Message = "MandateId is not exist";
            //    res.Status = "Failure";
            //    res.ResCode = "ERR000";
            //    HttpResponseMessage responseMessage = Request.CreateResponse(HttpStatusCode.Created, res);
            //   // HttpResponseMessage responseMessage = Request.CreateResponse(HttpStatusCode.Created, "Failure OR ERR000 OR Account should be validated");
            //    return responseMessage;
            //}
            //else if (!CheckMandateInfo.CheckPHENachValidation(context.MandateId, context.AppID))
            //{
            //    HttpResponseMessage responseMessage = Request.CreateResponse(HttpStatusCode.Created, "Mandate already physical");
            //    return responseMessage;
            //}
            else if (CheckMandateInfo.CheckENachValidation(context.MdtID, context.AppID))
            {
                res.Message = "Mandate type already selected as eMandate";
                res.Status = "Failure";
                res.ResCode = "ykR20030";
                HttpResponseMessage responseMessage = Request.CreateResponse(HttpStatusCode.Created, res);
                //HttpResponseMessage responseMessage = Request.CreateResponse(HttpStatusCode.Created, "Failure OR ERR000 OR Mandate already EMandate");
                return res;
            }
            else
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(context.AppID)].ConnectionString);
                bool Flag = false;
                //string temp = ConfigurationManager.AppSettings["MerchantKey" + context.AppID];
                string UserId = "";
                string entityid = "";
                string query = "Sp_WebAPI";
                //if (temp.Trim() == DBsecurity.Decrypt(context.MerchantKey))
                //{
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@QueryType", "GetEntityUser");
                cmd.Parameters.AddWithValue("@AppID", context.AppID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        UserId = Convert.ToString(dt.Rows[0][0]);
                    entityid = Convert.ToString(dt.Rows[0]["entityid"]);
                    Flag = true;
                    }
                //}
                if (Flag)
                {
                    if (CheckMandateInfo.CheckQrLogo(entityid, context.AppID) == "1")
                    {

                        try
                        {
                            query = "Sp_WebAPI";
                            cmd = new SqlCommand(query, con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@QueryType", "UpdateIsPHysical");
                            cmd.Parameters.AddWithValue("@MandateId", context.MdtID);
                            cmd.Parameters.AddWithValue("@UserId", UserId);
                            try
                            {
                                con.Open();
                                int value = cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex) { }
                            finally { con.Close(); }
                            //  string ID = DbSecurity.Decrypt(Request.QueryString["id"]);
                            DataSet ds = new DataSet();

                            //var result = CreatePdf.CheckQrLogo(DbSecurity.Encrypt(Iace.User.CurrentUser.User.ReferenceId.ToString()));
                            //string filePath = Server.MapPath("~");
                            //Byte[] bytes;
                            //if (result == "1")
                            //{
                            //    FileStream fs = new FileStream(filePath + "/MandateQrcode/" + ID + ".png", FileMode.Open, FileAccess.Read);
                            //    BinaryReader br = new BinaryReader(fs);
                            //    bytes = br.ReadBytes((Int32)fs.Length);
                            //    ds = BusinessLibrary1.GetShowForPDF(ID);
                            //}
                            //else
                            //{
                            //    FileStream fs = new FileStream(filePath + "/PdfLogoImage/logo.png", FileMode.Open, FileAccess.Read);
                            //    BinaryReader br = new BinaryReader(fs);
                            //    bytes = br.ReadBytes((Int32)fs.Length);
                            //}

                            //ds = BusinessLibrary1.GetShowForPDF(ID);
                            ds = GetShowForPDF(context.MdtID, context.AppID);
                            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow dr in ds.Tables[0].Rows)
                                {
                                    int count_IherebyAuthorize = (dr["CompanyName"].ToString()).Length;
                                    int count_BankName = (dr["BankName"].ToString()).Length;
                                    int count = (dr["EmailId"].ToString()).Length;
                                    int count_Ref1 = (dr["Reference1"].ToString()).Length;
                                    int count_Ref2 = (dr["Reference2"].ToString()).Length;
                                    int count_Customer = (dr["BenificiaryName"].ToString()).Length;
                                    //var FontColour = new BaseColor(35, 31, 32);

                                    Font fontAb11 = FontFactory.GetFont("Verdana", 8, Font.BOLD);
                                    Font fontAbCutomer = FontFactory.GetFont("Verdana", 7, Font.BOLD);
                                    Font font9 = FontFactory.GetFont("Verdana", 8, Font.BOLD);
                                    Font fontAb11BU = FontFactory.GetFont("Verdana", 7, Font.UNDERLINE, Color.LIGHT_GRAY);
                                    Font fontAb6 = FontFactory.GetFont("Verdana", 8, Font.BOLD);
                                    Font fontAb9 = FontFactory.GetFont("Verdana", 8, Font.BOLD);
                                    Font fontAb9B = FontFactory.GetFont("Verdana", 8, Font.BOLD);
                                    Font fontAb11B = FontFactory.GetFont("Verdana", 8, Font.BOLD);
                                    Font fontA11B = FontFactory.GetFont("Verdana", 8, Font.BOLD);
                                    Font fontA119B = FontFactory.GetFont("Verdana", 9, Font.BOLD);
                                    Font fontText = FontFactory.GetFont("Verdana", 7, Font.BOLD);
                                    Font fontText6 = FontFactory.GetFont("Verdana", 6, Font.BOLD);
                                    Font fontText5 = FontFactory.GetFont("Verdana", 5, Font.BOLD);
                                    Font fontText4 = FontFactory.GetFont("Verdana", 1);
                                    var spacerParagraph = new Paragraph();
                                    spacerParagraph.SpacingBefore = 5f;
                                    spacerParagraph.SpacingAfter = 5f;
                                    string filePath = ConfigurationManager.AppSettings["FilePath" + context.AppID]; //AppDomain.CurrentDomain.BaseDirectory;
                                    iTextSharp.text.Image CutterImage = iTextSharp.text.Image.GetInstance(GetcutterImage(context.AppID));
                                    //System.IO.File.WriteAllBytes("E:/YoekiProjects/cutternew.png", BusinessLibrary1.GetcutterImage());  

                                    CutterImage.ScaleToFit(550f, 200f);
                                    WebClient myWebClient = new WebClient();
                                    byte[] bytes = myWebClient.DownloadData(filePath + "MandateQrcode/" + context.MdtID + ".png");
                                    iTextSharp.text.Image LogoImage = iTextSharp.text.Image.GetInstance(bytes);
                                    //LogoImage.ScaleAbsolute(23f, 23f);
                                    LogoImage.ScaleAbsolute(50f, 50f);
                                    iTextSharp.text.Image Rupee = iTextSharp.text.Image.GetInstance(GetRupeeIcon(context.AppID));
                                    //Rupee.Border = iTextSharp.text.Rectangle.BOX;
                                    Rupee.BorderColor = iTextSharp.text.Color.BLACK;
                                    Rupee.BorderWidth = 12f;
                                    Rupee.ScaleToFit(7f, 7f);

                                    ////FileStream fs111 = new FileStream(filePath + "/images/checkbox.jpg", FileMode.Open, FileAccess.Read);
                                    //FileStream fs111 = new FileStream(filePath + "/images/checkbox-background.jpg", FileMode.Open, FileAccess.Read);
                                    //BinaryReader br111 = new BinaryReader(fs111);
                                    //Byte[] bytes111 = br111.ReadBytes((Int32)fs111.Length);
                                    byte[] bytes111 = myWebClient.DownloadData(filePath + "images/checkbox-background.jpg");
                                    iTextSharp.text.Image SmallcheckBox = iTextSharp.text.Image.GetInstance(bytes111);

                                    SmallcheckBox.BorderColor = iTextSharp.text.Color.BLACK;
                                    SmallcheckBox.BorderWidth = 12f;
                                    SmallcheckBox.ScaleToFit(5f, 5f);


                                    //FileStream fs11 = new FileStream(filePath + "/images/tick-iconmandate.png", FileMode.Open, FileAccess.Read);
                                    //BinaryReader br11 = new BinaryReader(fs11);
                                    //Byte[] bytes11 = br11.ReadBytes((Int32)fs11.Length);
                                    byte[] bytes11 = myWebClient.DownloadData(filePath + "images/tick-iconmandate.png");
                                    iTextSharp.text.Image checkBox = iTextSharp.text.Image.GetInstance(bytes11);

                                    checkBox.BorderColor = iTextSharp.text.Color.BLACK;
                                    checkBox.BorderWidth = 12f;
                                    checkBox.ScaleToFit(8f, 8f);


                                    //FileStream fs1 = new FileStream(filePath + "/images/checkbox-background.jpg", FileMode.Open, FileAccess.Read);
                                    //BinaryReader br1 = new BinaryReader(fs1);
                                    //Byte[] bytes1 = br1.ReadBytes((Int32)fs1.Length);
                                    byte[] bytes1 = myWebClient.DownloadData(filePath + "images/checkbox-background.jpg");

                                    iTextSharp.text.Image Box = iTextSharp.text.Image.GetInstance(bytes1);
                                    Box.BorderColor = iTextSharp.text.Color.BLACK;
                                    Box.BorderWidth = 12f;
                                    Box.ScaleToFit(8f, 8f);

                                    int i = 1;

                                    //PdfPTable PdfMain = new PdfPTable(1);
                                    //PdfMain.DefaultCell.NoWrap = false;
                                    //PdfMain.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    ////PdfHeaderTable.DefaultCell.Border = 2;
                                    ////PdfHeaderTable.DefaultCell.BorderColor = Color.BLACK;
                                    //PdfMain.WidthPercentage = 100;
                                    //float[] HeaderwidthsMain = new float[] { 15f};
                                    //PdfMain.SetWidths(HeaderwidthsMain);
                                    //PdfPCell PdfMainCell = null;
                                    //Document documentMain = new Document();
                                    //documentMain.Open();

                                    #region Table
                                    PdfPTable PdfHeaderTable = new PdfPTable(33);
                                    //PdfPTable PdfHeaderTable = new PdfPTable(31);
                                    PdfHeaderTable.DefaultCell.NoWrap = false;
                                    PdfHeaderTable.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    //PdfHeaderTable.DefaultCell.Border = 2;
                                    //PdfHeaderTable.DefaultCell.BorderColor = Color.BLACK;
                                    PdfHeaderTable.WidthPercentage = 100;
                                    float[] Headerwidths = new float[] { 1f, 4f, 3f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                                    //float[] Headerwidths = new float[] { 4f, 3f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                                    PdfHeaderTable.SetWidths(Headerwidths);
                                    PdfPCell PdfHeaderCell = null;
                                    Document document = new Document();
                                    document.Open();
                                    //Paragraph p = new Paragraph();
                                    //p.Add(new Chunk(LogoImage, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));


                                    PdfHeaderCell = new PdfPCell(new Phrase(""));
                                    PdfHeaderCell.Colspan = 33;
                                    PdfHeaderCell.Border = Rectangle.NO_BORDER;
                                    PdfHeaderCell.BorderWidthTop = 2f;
                                    PdfHeaderCell.BorderWidthLeft = 2f;
                                    PdfHeaderCell.BorderWidthRight = 2f;
                                    PdfHeaderTable.AddCell(PdfHeaderCell);


                                    PdfHeaderCell = new PdfPCell(new Phrase(" "));
                                    PdfHeaderCell.FixedHeight = 50f;
                                    PdfHeaderCell.Rowspan = 3;
                                    PdfHeaderCell.Border = Rectangle.NO_BORDER;//Avinash                        
                                    PdfHeaderCell.BorderWidthLeft = 2f;
                                    PdfHeaderTable.AddCell(PdfHeaderCell);


                                    PdfHeaderCell = new PdfPCell(LogoImage);
                                    PdfHeaderCell.FixedHeight = 50f;
                                    //PdfHeaderCell = new PdfPCell(p);
                                    //PdfHeaderCell.FixedHeight = 30f;

                                    PdfHeaderCell.Rowspan = 3;
                                    PdfHeaderCell.Border = Rectangle.NO_BORDER;//Avinash
                                                                               //PdfHeaderCell.Border = Rectangle.LEFT_BORDER;
                                                                               //PdfHeaderCell.Border = Rectangle.TOP_BORDER;
                                                                               // PdfHeaderCell.HorizontalAlignment = 1;
                                    PdfHeaderTable.AddCell(PdfHeaderCell);

                                    PdfHeaderCell = new PdfPCell(new Phrase("UMRN", fontAb11B));
                                    PdfHeaderCell.NoWrap = false;
                                    PdfHeaderCell.BackgroundColor = new Color(252, 252, 252);
                                    PdfHeaderCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    //PdfHeaderCell.FixedHeight = 20f;
                                    PdfHeaderCell.HorizontalAlignment = 1;
                                    //PdfHeaderCell.Border = Rectangle.TOP_BORDER;
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
                                    PdfHeaderCell.BackgroundColor = new Color(252, 252, 252);
                                    PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderCell.HorizontalAlignment = 1;
                                    PdfHeaderCell.Border = Rectangle.NO_BORDER;//Avinash
                                                                               //PdfHeaderCell.Border = Rectangle.TOP_BORDER;
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

                                    PdfHeaderCell = new PdfPCell(new Phrase(" "));
                                    PdfHeaderCell.NoWrap = false;
                                    PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderCell.HorizontalAlignment = 1;
                                    PdfHeaderCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfHeaderCell.BorderWidthRight = 2f;
                                    PdfHeaderTable.AddCell(PdfHeaderCell);

                                    //----------------------------------------Add Sponsor bankcode---------------------
                                    string SpBankCode = dr["SponserBankCode"].ToString();
                                    PdfHeaderCell = new PdfPCell(new Phrase("Sponsor bankcode", fontAb11B));
                                    PdfHeaderCell.NoWrap = false;
                                    PdfHeaderCell.BackgroundColor = new Color(252, 252, 252);
                                    PdfHeaderCell.Colspan = 4;
                                    PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderCell.HorizontalAlignment = 1;
                                    PdfHeaderCell.FixedHeight = 15f;
                                    PdfHeaderCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfHeaderCell.Border = Rectangle.NO_BORDER;//Avinash
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
                                    PdfHeaderCell.BackgroundColor = new Color(252, 252, 252);
                                    PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderCell.HorizontalAlignment = 1;
                                    PdfHeaderCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfHeaderCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfHeaderTable.AddCell(PdfHeaderCell);

                                    PdfHeaderCell = new PdfPCell(new Phrase(dr["UtilityCode"].ToString(), fontAb11B));
                                    PdfHeaderCell.NoWrap = false;
                                    PdfHeaderCell.Colspan = 8;
                                    PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                                    PdfHeaderCell.HorizontalAlignment = 1;
                                    //PdfHeaderCell.FixedHeight = 20f;
                                    PdfHeaderCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfHeaderTable.AddCell(PdfHeaderCell);


                                    PdfHeaderCell = new PdfPCell(new Phrase(" "));
                                    PdfHeaderCell.NoWrap = false;
                                    PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderCell.HorizontalAlignment = 1;
                                    PdfHeaderCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfHeaderCell.BorderWidthRight = 2f;
                                    PdfHeaderTable.AddCell(PdfHeaderCell);


                                    ////------------------------------- add Created Status-------------------------------------
                                    Document documentCheckBox = new Document();
                                    //documentCheckBox.Open();
                                    //Paragraph pCheckBox = new Paragraph();
                                    //string status = dr["CreatedStatus"].ToString();
                                    //if (status == "C")
                                    //{
                                    //    pCheckBox.Add(new Phrase("CREATE ", fontText));
                                    //    pCheckBox.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                    //    pCheckBox.Add(new Phrase(" MODIFY ", fontText));
                                    //    pCheckBox.Add(new Phrase(" CANCEL ", fontText));

                                    //}
                                    //else if (status == "M")
                                    //{
                                    //    pCheckBox.Add(new Phrase("CREATE ", fontText));
                                    //    pCheckBox.Add(new Phrase(" MODIFY ", fontText));
                                    //    pCheckBox.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                    //    pCheckBox.Add(new Phrase(" CANCEL ", fontText));

                                    //}
                                    //else if (status == "L")
                                    //{
                                    //    pCheckBox.Add(new Phrase("CREATE ", fontText));

                                    //    pCheckBox.Add(new Phrase(" MODIFY ", fontText));

                                    //    pCheckBox.Add(new Phrase(" CANCEL ", fontText));
                                    //    pCheckBox.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                    //}
                                    //PdfHeaderCell = new PdfPCell(pCheckBox);
                                    //PdfHeaderCell.NoWrap = false;
                                    //PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    //PdfHeaderCell.Border = Rectangle.NO_BORDER;//Avinash
                                    //PdfHeaderTable.AddCell(PdfHeaderCell);

                                    //PdfHeaderCell = new PdfPCell(new Phrase(" "));
                                    //PdfHeaderCell.NoWrap = false;
                                    //PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    //PdfHeaderCell.HorizontalAlignment = 1;
                                    //PdfHeaderCell.Border = Rectangle.NO_BORDER;//Avinash
                                    //PdfHeaderCell.BorderWidthLeft = 2f;
                                    //PdfHeaderTable.AddCell(PdfHeaderCell);




                                    PdfHeaderCell = new PdfPCell(new Phrase("I/We hereby Authorize", fontAb11B));
                                    PdfHeaderCell.NoWrap = false;
                                    PdfHeaderCell.BackgroundColor = new Color(252, 252, 252);
                                    PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderCell.Colspan = 5;
                                    PdfHeaderCell.FixedHeight = 15f;
                                    PdfHeaderCell.HorizontalAlignment = 1;
                                    PdfHeaderCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfHeaderCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfHeaderTable.AddCell(PdfHeaderCell);


                                    //PdfHeaderCell = new PdfPCell(new Phrase(dr["CompanyName"].ToString(), fontAb11B));
                                    if (count_IherebyAuthorize < 34)
                                    {
                                        PdfHeaderCell = new PdfPCell(new Phrase(dr["CompanyName"].ToString(), fontAb11B));
                                    }
                                    else if (count_IherebyAuthorize < 40)
                                    {
                                        PdfHeaderCell = new PdfPCell(new Phrase(dr["CompanyName"].ToString(), fontText));
                                    }
                                    else if (count_IherebyAuthorize < 48)
                                    {
                                        PdfHeaderCell = new PdfPCell(new Phrase(dr["CompanyName"].ToString(), fontText6));
                                    }
                                    else
                                    {
                                        PdfHeaderCell = new PdfPCell(new Phrase(dr["CompanyName"].ToString(), fontText5));
                                    }

                                    PdfHeaderCell.NoWrap = false;
                                    PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    //PdfHeaderCell.Colspan = 10;
                                    PdfHeaderCell.Colspan = 10;
                                    PdfHeaderCell.HorizontalAlignment = 1;
                                    PdfHeaderCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfHeaderTable.AddCell(PdfHeaderCell);

                                    PdfHeaderCell = new PdfPCell(new Phrase("To Debit", fontAb11B));
                                    PdfHeaderCell.NoWrap = false;
                                    PdfHeaderCell.BackgroundColor = new Color(252, 252, 252);
                                    PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderCell.Colspan = 3;
                                    PdfHeaderCell.HorizontalAlignment = 1;
                                    PdfHeaderCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfHeaderCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfHeaderTable.AddCell(PdfHeaderCell);

                                    Document documentCheckBoxSB = new Document();
                                    documentCheckBoxSB.Open();
                                    Paragraph pCheckBoxSB = new Paragraph();
                                    //----------------------------------add To Debit---------------------------
                                    string chDebit = dr["DebitTo"].ToString();
                                    if (chDebit == "SB")
                                    {
                                        // pCheckBoxSB.Add(new Phrase(" ", fontText));
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
                                        //pCheckBoxSB.Add(new Phrase(" ", fontText));
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
                                        //pCheckBoxSB.Add(new Phrase(" ", fontText));
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
                                        // pCheckBoxSB.Add(new Phrase(" ", fontText));
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
                                        // pCheckBoxSB.Add(new Phrase(" ", fontText));
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
                                        //pCheckBoxSB.Add(new Phrase(" ", fontText));
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
                                    //PdfHeaderCell.Colspan = 11;
                                    PdfHeaderCell.Colspan = 12;
                                    PdfHeaderCell.HorizontalAlignment = 1;
                                    PdfHeaderCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfHeaderCell.Border = Rectangle.NO_BORDER;//Avinash                        
                                    PdfHeaderTable.AddCell(PdfHeaderCell);

                                    PdfHeaderCell = new PdfPCell(new Phrase(" "));
                                    PdfHeaderCell.NoWrap = false;
                                    PdfHeaderCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderCell.HorizontalAlignment = 1;
                                    PdfHeaderCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfHeaderCell.BorderWidthRight = 2f;
                                    PdfHeaderTable.AddCell(PdfHeaderCell);

                                    //PdfPTable PdfMidTable = new PdfPTable(32);
                                    PdfPTable PdfMidTable = new PdfPTable(34);
                                    PdfMidTable.DefaultCell.NoWrap = false;
                                    PdfMidTable.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidTable.DefaultCell.Border = PdfCell.NO_BORDER;
                                    PdfMidTable.WidthPercentage = 100;
                                    float[] PdfMidTableHeaderwidths = new float[] { 1f, 4f, 3f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                                    //float[] PdfMidTableHeaderwidths = new float[] { 4f, 3f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                                    PdfMidTable.SetWidths(PdfMidTableHeaderwidths);
                                    PdfPCell PdfMidCell = null;


                                    PdfMidCell = new PdfPCell(new Phrase(" "));
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell.BorderWidthLeft = 2f;
                                    PdfMidTable.AddCell(PdfMidCell);


                                    documentCheckBox.Open();
                                    Paragraph pCheckBox = new Paragraph();
                                    string status = dr["CreatedStatus"].ToString();
                                    if (status == "C")
                                    {
                                        pCheckBox.Add(new Phrase("CREATE ", fontText));
                                        pCheckBox.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pCheckBox.Add(new Phrase(" MODIFY ", fontText));
                                        pCheckBox.Add(new Phrase(" CANCEL ", fontText));

                                    }
                                    else if (status == "M")
                                    {
                                        pCheckBox.Add(new Phrase("CREATE ", fontText));
                                        pCheckBox.Add(new Phrase(" MODIFY ", fontText));
                                        pCheckBox.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pCheckBox.Add(new Phrase(" CANCEL ", fontText));

                                    }
                                    else if (status == "L")
                                    {
                                        pCheckBox.Add(new Phrase("CREATE ", fontText));

                                        pCheckBox.Add(new Phrase(" MODIFY ", fontText));

                                        pCheckBox.Add(new Phrase(" CANCEL ", fontText));
                                        pCheckBox.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                    }
                                    PdfMidCell = new PdfPCell(pCheckBox);
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.Border = Rectangle.NO_BORDER;//Avinash
                                                                            //PdfMidCell.Border = Rectangle.LEFT_BORDER;//Avinash
                                                                            //PdfMidCell.BorderWidth = 2f;
                                                                            //PdfMidCell.Rowspan = 2;
                                    PdfMidTable.AddCell(PdfMidCell);


                                    //PdfMidCell = new PdfPCell(new Phrase("", fontAb11B));
                                    //PdfMidCell.Colspan = 31;
                                    //PdfMidCell.FixedHeight = 0f;
                                    //PdfMidCell.Border = Rectangle.NO_BORDER;//Avinash
                                    //PdfMidCell.Border = Rectangle.RIGHT_BORDER;//Avinash
                                    ////PdfMidCell.Border = Rectangle.LEFT_BORDER;//Avinash
                                    //PdfMidTable.AddCell(PdfMidCell);



                                    //----------------------------------Add AccountNo.-------------------------------------------------
                                    PdfMidCell = new PdfPCell(new Phrase("Bank a/c Number", fontAb11B));
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    //PdfMidCell.Colspan = 6;
                                    PdfMidCell.Colspan = 5;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfMidCell.Border = Rectangle.NO_BORDER;//Avinash
                                                                            //PdfMidCell.Border = Rectangle.LEFT_BORDER;
                                                                            //PdfMidCell.Border = Rectangle.TOP_BORDER;
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
                                            PdfMidCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                            PdfMidTable.AddCell(PdfMidCell);
                                        }

                                        int len = 26 - Convert.ToInt32(AccountNo.Length);
                                        for (int k = 0; k < len; k++)
                                        {
                                            PdfMidCell = new PdfPCell(new Phrase(" ", fontA119B));
                                            PdfMidCell.NoWrap = false;
                                            PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                            PdfMidCell.HorizontalAlignment = 1;
                                            PdfMidCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                            PdfMidTable.AddCell(PdfMidCell);
                                        }
                                    }

                                    PdfMidCell = new PdfPCell(new Phrase(" "));
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell.BorderWidthRight = 2f;
                                    PdfMidTable.AddCell(PdfMidCell);

                                    PdfMidCell = new PdfPCell(new Phrase(" "));
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell.BorderWidthLeft = 2f;
                                    PdfMidTable.AddCell(PdfMidCell);


                                    PdfMidCell = new PdfPCell(new Phrase("With Bank", fontAb11B));
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidCell.FixedHeight = 25f;
                                    PdfMidCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfMidCell.Border = Rectangle.NO_BORDER;//Avinash
                                                                            //PdfMidCell.Border = Rectangle.LEFT_BORDER;
                                                                            //PdfMidCell.BorderWidth = 2f;
                                    PdfMidTable.AddCell(PdfMidCell);

                                    //PdfMidCell = new PdfPCell(new Phrase(dr["BankName"].ToString(), fontAb11));                        
                                    if (count_BankName < 34)
                                    {
                                        PdfMidCell = new PdfPCell(new Phrase(dr["BankName"].ToString(), fontAb11B));
                                    }
                                    else if (count_BankName < 40)
                                    {
                                        PdfMidCell = new PdfPCell(new Phrase(dr["BankName"].ToString(), fontText));
                                    }
                                    else if (count_BankName < 48)
                                    {
                                        PdfMidCell = new PdfPCell(new Phrase(dr["BankName"].ToString(), fontText6));
                                    }
                                    else
                                    {
                                        PdfMidCell = new PdfPCell(new Phrase(dr["BankName"].ToString(), fontText5));
                                    }

                                    PdfMidCell.NoWrap = false;
                                    //PdfMidCell.FixedHeight = 30f;
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.Colspan = 6;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfMidTable.AddCell(PdfMidCell);


                                    PdfMidCell = new PdfPCell(new Phrase("IFSC", fontAb11B));
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.Colspan = 2;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfMidCell.Border = Rectangle.NO_BORDER;//Avinash
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
                                    PdfMidCell.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.Colspan = 3;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfMidCell.Border = Rectangle.NO_BORDER;//Avinash
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

                                    PdfMidCell = new PdfPCell(new Phrase(" "));
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell.BorderWidthRight = 2f;
                                    PdfMidTable.AddCell(PdfMidCell);
                                    //-----------------------------------Add amount of Rupees---------------------------

                                    PdfMidCell = new PdfPCell(new Phrase(" "));
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell.BorderWidthLeft = 2f;
                                    PdfMidTable.AddCell(PdfMidCell);

                                    PdfMidCell = new PdfPCell(new Phrase("an amount of Rupees", fontAb11B));
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.Colspan = 2;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidCell.Border = Rectangle.NO_BORDER;//Avinash                        
                                                                            //PdfMidCell.Border = Rectangle.LEFT_BORDER;
                                                                            //PdfMidCell.BorderWidth = 2f;
                                    PdfMidCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_LEFT;
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
                                    //PdfMidCell.Colspan = 10;
                                    PdfMidCell.Colspan = 8;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidTable.AddCell(PdfMidCell);

                                    PdfMidCell = new PdfPCell(new Phrase(" "));
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell.BorderWidthRight = 2f;
                                    PdfMidTable.AddCell(PdfMidCell);

                                    //---------------------------------Add Frequency--------------------------------------
                                    PdfMidCell = new PdfPCell(new Phrase(" "));
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell.BorderWidthLeft = 2f;
                                    PdfMidTable.AddCell(PdfMidCell);

                                    string Freq = dr["Frequency"].ToString();
                                    //PdfMidCell = new PdfPCell(new Phrase("Frequency", fontAb11B));
                                    PdfMidCell = new PdfPCell(new Phrase("Frequency", fontText));
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidCell.Border = Rectangle.NO_BORDER;//Avinash
                                                                            //PdfMidCell.Border = Rectangle.LEFT_BORDER;
                                                                            //PdfMidCell.BorderWidth = 2f;
                                    PdfMidCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_LEFT;
                                    PdfMidTable.AddCell(PdfMidCell);


                                    Document documentMonthly = new Document();
                                    documentMonthly.Open();
                                    Paragraph pMonthly = new Paragraph();
                                    //------------------------------- add Monthly-------------------------------------
                                    if (Freq == "M")
                                    {
                                        pMonthly.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pMonthly.Add(new Phrase("  Monthly", fontText));
                                    }
                                    else
                                    {
                                        pMonthly.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pMonthly.Add(new Phrase("  Monthly", fontText));
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
                                        pQtly.Add(new Phrase(" Qtly", fontText));
                                    }
                                    else
                                    {
                                        pQtly.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pQtly.Add(new Phrase(" Qtly", fontText));
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
                                        pHYrly.Add(new Phrase("  H-Yrly", fontText));
                                    }
                                    else
                                    {
                                        pHYrly.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pHYrly.Add(new Phrase("  H-Yrly", fontText));
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
                                        pYearly.Add(new Phrase("  Yearly", fontText));
                                    }
                                    else
                                    {
                                        pYearly.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pYearly.Add(new Phrase("  Yearly", fontText));
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
                                        prensented.Add(new Phrase("  As & when prensented", fontText));
                                    }
                                    else
                                    {
                                        prensented.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        prensented.Add(new Phrase("  As & when prensented", fontText));
                                    }

                                    PdfMidCell = new PdfPCell(prensented);
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.Colspan = 7;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidTable.AddCell(PdfMidCell);

                                    string DebitType = dr["DebitType"].ToString();

                                    //PdfMidCell = new PdfPCell(new Phrase("Debit Type", fontAb11B));
                                    PdfMidCell = new PdfPCell(new Phrase("Debit Type", fontText));
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.Colspan = 3;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_LEFT;
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


                                    PdfMidCell = new PdfPCell(new Phrase(" "));
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell.BorderWidthRight = 2f;
                                    PdfMidTable.AddCell(PdfMidCell);

                                    PdfMidCell = new PdfPCell(new Phrase(" "));
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell.BorderWidthLeft = 2f;
                                    PdfMidTable.AddCell(PdfMidCell);

                                    PdfMidCell = new PdfPCell(new Phrase("Reference 1", fontAb11B));
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidCell.Border = Rectangle.NO_BORDER;//Avinash
                                                                            //PdfMidCell.Border = Rectangle.LEFT_BORDER;
                                                                            //PdfMidCell.BorderWidth = 2f;
                                    PdfMidCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_LEFT;
                                    PdfMidTable.AddCell(PdfMidCell);

                                    //PdfMidCell = new PdfPCell(new Phrase(dr["Reference1"].ToString(), fontAb11B));
                                    if (count_Ref1 < 34)
                                    {
                                        PdfMidCell = new PdfPCell(new Phrase(dr["Reference1"].ToString(), fontAb11B));
                                    }
                                    else if (count_Ref1 < 40)
                                    {
                                        PdfMidCell = new PdfPCell(new Phrase(dr["Reference1"].ToString(), fontText));
                                    }
                                    else if (count_Ref1 < 48)
                                    {
                                        PdfMidCell = new PdfPCell(new Phrase(dr["Reference1"].ToString(), fontText6));
                                    }
                                    else
                                    {
                                        PdfMidCell = new PdfPCell(new Phrase(dr["Reference1"].ToString(), fontText5));
                                    }

                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.Colspan = 15;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidTable.AddCell(PdfMidCell);


                                    PdfMidCell = new PdfPCell(new Phrase("Phone Number ", fontAb11B));
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.Colspan = 6;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidTable.AddCell(PdfMidCell);

                                    PdfMidCell = new PdfPCell(new Phrase(dr["PhoneNo"].ToString(), fontAb11B));
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.Colspan = 10;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidTable.AddCell(PdfMidCell);

                                    PdfMidCell = new PdfPCell(new Phrase(" "));
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell.BorderWidthRight = 2f;
                                    PdfMidTable.AddCell(PdfMidCell);

                                    PdfMidCell = new PdfPCell(new Phrase(" "));
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell.BorderWidthLeft = 2f;
                                    PdfMidTable.AddCell(PdfMidCell);

                                    PdfMidCell = new PdfPCell(new Phrase("Reference 2", fontAb11B));
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidCell.Border = Rectangle.NO_BORDER;//Avinash
                                                                            //PdfMidCell.Border = Rectangle.LEFT_BORDER;
                                                                            //PdfMidCell.BorderWidth = 2f;
                                    PdfMidCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_LEFT;
                                    PdfMidTable.AddCell(PdfMidCell);

                                    //PdfMidCell = new PdfPCell(new Phrase(dr["Reference2"].ToString(), fontAb11B));
                                    if (count_Ref2 < 34)
                                    {
                                        PdfMidCell = new PdfPCell(new Phrase(dr["Reference2"].ToString(), fontAb11B));
                                    }
                                    else if (count_Ref2 < 40)
                                    {
                                        PdfMidCell = new PdfPCell(new Phrase(dr["Reference2"].ToString(), fontText));
                                    }
                                    else if (count_Ref2 < 48)
                                    {
                                        PdfMidCell = new PdfPCell(new Phrase(dr["Reference2"].ToString(), fontText6));
                                    }
                                    else
                                    {
                                        PdfMidCell = new PdfPCell(new Phrase(dr["Reference2"].ToString(), fontText5));
                                    }

                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.Colspan = 15;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidTable.AddCell(PdfMidCell);


                                    PdfMidCell = new PdfPCell(new Phrase("EMail ID", fontAb11B));
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.Colspan = 6;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidTable.AddCell(PdfMidCell);

                                    //PdfMidCell = new PdfPCell(new Phrase(dr["EmailId"].ToString(), fontAb11B));
                                    if (count < 34)
                                    {
                                        PdfMidCell = new PdfPCell(new Phrase(dr["EmailId"].ToString(), fontAb11B));
                                    }
                                    //else if (count < 40)
                                    //{
                                    //    PdfMidCell = new PdfPCell(new Phrase(dr["EmailId"].ToString(), fontText));
                                    //}
                                    //else if (count < 48)
                                    //{
                                    //    PdfMidCell = new PdfPCell(new Phrase(dr["EmailId"].ToString(), fontText6));
                                    //}
                                    else
                                    {
                                        PdfMidCell = new PdfPCell(new Phrase(dr["EmailId"].ToString(), fontText5));
                                    }

                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.Colspan = 10;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidTable.AddCell(PdfMidCell);

                                    PdfMidCell = new PdfPCell(new Phrase(" "));
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell.BorderWidthRight = 2f;
                                    PdfMidTable.AddCell(PdfMidCell);

                                    PdfMidCell = new PdfPCell(new Phrase(" "));
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell.BorderWidthLeft = 2f;
                                    PdfMidTable.AddCell(PdfMidCell);

                                    //PdfMidCell = new PdfPCell(new Phrase("PERIOD", fontAb11B));
                                    PdfMidCell = new PdfPCell(new Phrase("PERIOD", fontText));
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidCell.FixedHeight = 8f;
                                    //PdfMidCell.BorderWidthLeft = 2f;
                                    //PdfMidCell.Border = Rectangle.NO_BORDER;//Avinash
                                    //PdfMidCell.Border = Rectangle.LEFT_BORDER;
                                    PdfMidTable.AddCell(PdfMidCell);//

                                    //PdfMidCell = new PdfPCell(new Phrase("I agree for the debit of mandate processing charges by the bank whom I am authorizing to debit my account as per latest schedule of charges of bank ", fontText));
                                    PdfMidCell = new PdfPCell(new Phrase("I agree for the debit of mandate processing charges by the bank whom I am authorizing to debit my account as per latest schedule of charges of bank ", fontText6));
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.Border = Rectangle.NO_BORDER;
                                    //PdfMidCell.Border = Rectangle.RIGHT_BORDER;
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    //PdfMidCell.Colspan = 32;
                                    PdfMidCell.Colspan = 31;//Avinash
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidTable.AddCell(PdfMidCell);


                                    PdfMidCell = new PdfPCell(new Phrase(" "));
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell.BorderWidthRight = 2f;
                                    PdfMidTable.AddCell(PdfMidCell);

                                    //PdfMidCell = new PdfPCell(new Phrase("PERIOD", fontAb11B));
                                    //PdfMidCell.NoWrap = false;
                                    ////PdfMidCell.BackgroundColor = new Color(252, 252, 252);
                                    //PdfHeaderCell.Border = Rectangle.NO_BORDER;//Avinash
                                    //PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    //PdfMidCell.HorizontalAlignment = 1;
                                    //PdfMidTable.AddCell(PdfMidCell);

                                    PdfMidCell = new PdfPCell(new Phrase("", fontAb11B));
                                    PdfMidCell.NoWrap = false;
                                    PdfMidCell.Border = Rectangle.NO_BORDER;
                                    PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell.Colspan = 31;
                                    PdfMidCell.HorizontalAlignment = 1;
                                    PdfMidTable.AddCell(PdfMidCell);

                                    //PdfPTable PdfDetailTable = new PdfPTable(34);
                                    PdfPTable PdfDetailTable = new PdfPTable(36);
                                    PdfDetailTable.DefaultCell.NoWrap = false;
                                    PdfDetailTable.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailTable.DefaultCell.Border = PdfCell.NO_BORDER;
                                    PdfDetailTable.WidthPercentage = 100;
                                    //float[] Headerwidths1 = new float[] { 4f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                                    float[] Headerwidths1 = new float[] { 1f, 4f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                                    PdfDetailTable.SetWidths(Headerwidths1);
                                    PdfPCell PdfDetailCell = null;


                                    PdfDetailCell = new PdfPCell(new Phrase(""));
                                    PdfDetailCell.NoWrap = false;
                                    PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell.HorizontalAlignment = 1;
                                    PdfDetailCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfDetailCell.BorderWidthLeft = 2f;
                                    PdfDetailTable.AddCell(PdfDetailCell);


                                    PdfDetailCell = new PdfPCell(new Phrase("From", fontAb11B));
                                    PdfDetailCell.NoWrap = false;
                                    //PdfDetailCell.BackgroundColor = new Color(252, 252, 252);
                                    PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell.HorizontalAlignment = 1;
                                    //PdfDetailCell.BorderWidthLeft = 2f;
                                    //PdfDetailCell.Border = Rectangle.NO_BORDER;//Avinash
                                    //PdfDetailCell.Border = Rectangle.LEFT_BORDER;
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
                                    PdfDetailCell.Border = Rectangle.NO_BORDER;
                                    //PdfDetailCell.Border = Rectangle.RIGHT_BORDER;
                                    PdfDetailCell.Colspan = 25;
                                    PdfDetailTable.AddCell(PdfDetailCell);

                                    PdfDetailCell = new PdfPCell(new Phrase(""));
                                    PdfDetailCell.NoWrap = false;
                                    PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell.HorizontalAlignment = 1;
                                    PdfDetailCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfDetailCell.BorderWidthRight = 2f;
                                    PdfDetailTable.AddCell(PdfDetailCell);


                                    PdfDetailCell = new PdfPCell(new Phrase(""));
                                    PdfDetailCell.NoWrap = false;
                                    PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell.HorizontalAlignment = 1;
                                    PdfDetailCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfDetailCell.BorderWidthLeft = 2f;
                                    PdfDetailTable.AddCell(PdfDetailCell);

                                    PdfDetailCell = new PdfPCell(new Phrase("To*", fontAb11B));
                                    PdfDetailCell.NoWrap = false;
                                    PdfDetailCell.BackgroundColor = new Color(252, 252, 252);
                                    PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell.HorizontalAlignment = 1;
                                    //PdfDetailCell.BorderWidthLeft = 2f;
                                    //PdfDetailCell.Border = Rectangle.NO_BORDER;//Avinash
                                    //PdfDetailCell.Border = Rectangle.LEFT_BORDER;
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
                                                //PdfDetailCell.BackgroundColor = new Color(0,0,0);
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
                                            //PdfDetailCell.BackgroundColor = new Color(0, 0, 0);
                                            PdfDetailTable.AddCell(PdfDetailCell);
                                        }
                                    }
                                    //PdfDetailCell = new PdfPCell(new Phrase(" ", fontAb11B));
                                    //PdfDetailCell.NoWrap = false;
                                    //PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    //PdfDetailCell.HorizontalAlignment = 1;
                                    //PdfDetailCell.Border = Rectangle.NO_BORDER;
                                    //PdfDetailCell.Colspan = 25;
                                    //PdfDetailTable.AddCell(PdfDetailCell);

                                    /*Avinash*/
                                    PdfDetailCell = new PdfPCell(new Phrase("Sign. Primary Acc. Holder", fontAb11BU));
                                    PdfDetailCell.NoWrap = false;
                                    PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell.HorizontalAlignment = 1;
                                    PdfDetailCell.Border = Rectangle.NO_BORDER;
                                    PdfDetailCell.Colspan = 8;
                                    PdfDetailTable.AddCell(PdfDetailCell);

                                    PdfDetailCell = new PdfPCell(new Phrase("Sign Acc. Holder", fontAb11BU));
                                    PdfDetailCell.NoWrap = false;
                                    PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell.HorizontalAlignment = 1;
                                    PdfDetailCell.Border = Rectangle.NO_BORDER;
                                    PdfDetailCell.Colspan = 8;
                                    PdfDetailTable.AddCell(PdfDetailCell);

                                    PdfDetailCell = new PdfPCell(new Phrase("Sign Acc. Holder", fontAb11BU));
                                    PdfDetailCell.NoWrap = false;
                                    PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell.HorizontalAlignment = 1;
                                    PdfDetailCell.Border = Rectangle.NO_BORDER;
                                    //PdfDetailCell.Border = Rectangle.RIGHT_BORDER;
                                    PdfDetailCell.Colspan = 9;
                                    PdfDetailTable.AddCell(PdfDetailCell);
                                    /*Avinash*/

                                    PdfDetailCell = new PdfPCell(new Phrase(""));
                                    PdfDetailCell.NoWrap = false;
                                    PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell.HorizontalAlignment = 1;
                                    PdfDetailCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfDetailCell.BorderWidthRight = 2f;
                                    PdfDetailTable.AddCell(PdfDetailCell);

                                    PdfDetailCell = new PdfPCell(new Phrase(""));
                                    PdfDetailCell.NoWrap = false;
                                    PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell.HorizontalAlignment = 1;
                                    PdfDetailCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfDetailCell.BorderWidthLeft = 2f;
                                    PdfDetailTable.AddCell(PdfDetailCell);


                                    PdfDetailCell = new PdfPCell(new Phrase("Or", fontAb11B));
                                    PdfDetailCell.NoWrap = false;
                                    PdfDetailCell.BackgroundColor = new Color(252, 252, 252);
                                    PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell.HorizontalAlignment = 1;
                                    //PdfDetailCell.BorderWidthLeft = 2f;
                                    //PdfDetailCell.Border = Rectangle.NO_BORDER;//Avinash
                                    //PdfDetailCell.Border = Rectangle.LEFT_BORDER;
                                    PdfDetailTable.AddCell(PdfDetailCell);


                                    Document documentCheckBox123 = new Document();
                                    documentCheckBox123.Open();
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



                                    //PdfDetailCell = new PdfPCell(new Phrase("Sign. Primary Acc. Holder", fontAb11BU));
                                    //PdfDetailCell.NoWrap = false;
                                    //PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    //PdfDetailCell.HorizontalAlignment = 1;
                                    //PdfDetailCell.Border = Rectangle.NO_BORDER;
                                    //PdfDetailCell.Colspan = 8;
                                    //PdfDetailTable.AddCell(PdfDetailCell);

                                    //PdfDetailCell = new PdfPCell(new Phrase("Sign Acc. Holder", fontAb11BU));
                                    //PdfDetailCell.NoWrap = false;
                                    //PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    //PdfDetailCell.HorizontalAlignment = 1;
                                    //PdfDetailCell.Border = Rectangle.NO_BORDER;
                                    //PdfDetailCell.Colspan = 8;
                                    //PdfDetailTable.AddCell(PdfDetailCell);

                                    //PdfDetailCell = new PdfPCell(new Phrase("Sign Acc. Holder", fontAb11BU));
                                    //PdfDetailCell.NoWrap = false;
                                    //PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    //PdfDetailCell.HorizontalAlignment = 1;
                                    //PdfDetailCell.Border = Rectangle.NO_BORDER;
                                    //PdfDetailCell.Colspan = 9;
                                    //PdfDetailTable.AddCell(PdfDetailCell);



                                    //PdfDetailCell = new PdfPCell(new Phrase(" ", fontAb11B));
                                    //PdfDetailCell.NoWrap = false;
                                    //PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    //PdfDetailCell.HorizontalAlignment = 1;
                                    //PdfDetailCell.Border = Rectangle.NO_BORDER;

                                    //PdfDetailCell.Colspan = 5;
                                    //PdfDetailTable.AddCell(PdfDetailCell);

                                    //PdfDetailCell = new PdfPCell(new Phrase(dr["BenificiaryName"].ToString(), fontAbCutomer));
                                    if (count_Customer < 10)
                                    {
                                        PdfDetailCell = new PdfPCell(new Phrase(dr["BenificiaryName"].ToString(), fontAb11B));
                                    }
                                    else if (count_Customer < 20)
                                    {
                                        PdfDetailCell = new PdfPCell(new Phrase(dr["BenificiaryName"].ToString(), fontText));
                                    }
                                    else if (count_Customer < 30)
                                    {
                                        PdfDetailCell = new PdfPCell(new Phrase(dr["BenificiaryName"].ToString(), fontText6));
                                    }
                                    else
                                    {
                                        PdfDetailCell = new PdfPCell(new Phrase(dr["BenificiaryName"].ToString(), fontText5));
                                    }

                                    PdfDetailCell.NoWrap = false;
                                    PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell.HorizontalAlignment = 1;
                                    PdfDetailCell.Border = Rectangle.NO_BORDER;
                                    //PdfDetailCell.Colspan = 13;
                                    PdfDetailCell.Colspan = 7;
                                    PdfDetailTable.AddCell(PdfDetailCell);

                                    if (count_Customer < 10)
                                    {
                                        PdfDetailCell = new PdfPCell(new Phrase(dr["Customer2"].ToString(), fontAb11B));
                                    }
                                    else if (count_Customer < 20)
                                    {
                                        PdfDetailCell = new PdfPCell(new Phrase(dr["Customer2"].ToString(), fontText));
                                    }
                                    else if (count_Customer < 30)
                                    {
                                        PdfDetailCell = new PdfPCell(new Phrase(dr["Customer2"].ToString(), fontText6));
                                    }
                                    else
                                    {
                                        PdfDetailCell = new PdfPCell(new Phrase(dr["Customer2"].ToString(), fontText5));
                                    }
                                    //  PdfDetailCell = new PdfPCell(new Phrase(dr["Customer2"].ToString(), fontAbCutomer));
                                    PdfDetailCell.NoWrap = false;
                                    PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell.HorizontalAlignment = 1;
                                    PdfDetailCell.Border = Rectangle.NO_BORDER;
                                    PdfDetailCell.Colspan = 10;
                                    PdfDetailTable.AddCell(PdfDetailCell);

                                    if (count_Customer < 10)
                                    {
                                        PdfDetailCell = new PdfPCell(new Phrase(dr["Customer3"].ToString(), fontAb11B));
                                    }
                                    else if (count_Customer < 20)
                                    {
                                        PdfDetailCell = new PdfPCell(new Phrase(dr["Customer3"].ToString(), fontText));
                                    }
                                    else if (count_Customer < 30)
                                    {
                                        PdfDetailCell = new PdfPCell(new Phrase(dr["Customer3"].ToString(), fontText6));
                                    }
                                    else
                                    {
                                        PdfDetailCell = new PdfPCell(new Phrase(dr["Customer3"].ToString(), fontText5));
                                    }
                                    //PdfDetailCell = new PdfPCell(new Phrase(dr["Customer3"].ToString(), fontAbCutomer));
                                    PdfDetailCell.NoWrap = false;
                                    PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell.HorizontalAlignment = 1;
                                    PdfDetailCell.Border = Rectangle.NO_BORDER;
                                    PdfDetailCell.Colspan = 6;
                                    PdfDetailTable.AddCell(PdfDetailCell);

                                    PdfDetailCell = new PdfPCell(new Phrase(" ", fontAb11B));
                                    PdfDetailCell.NoWrap = false;
                                    PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell.HorizontalAlignment = 1;
                                    PdfDetailCell.Border = Rectangle.NO_BORDER;
                                    //PdfDetailCell.Border = Rectangle.RIGHT_BORDER;//Avinash
                                    PdfDetailCell.BorderWidthRight = 2f;
                                    PdfDetailCell.Colspan = 9;
                                    PdfDetailTable.AddCell(PdfDetailCell);


                                    /*Avinash[14/01/2020]*/
                                    PdfDetailCell = new PdfPCell(new Phrase(""));
                                    PdfDetailCell.NoWrap = false;
                                    PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell.HorizontalAlignment = 1;
                                    PdfDetailCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfDetailCell.BorderWidthLeft = 2f;
                                    PdfDetailCell.BorderWidthRight = 2f;
                                    PdfDetailCell.Colspan = 38;
                                    PdfDetailTable.AddCell(PdfDetailCell);

                                    PdfDetailCell = new PdfPCell(new Phrase(""));
                                    PdfDetailCell.NoWrap = false;
                                    PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell.HorizontalAlignment = 1;
                                    PdfDetailCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfDetailCell.BorderWidthLeft = 2f;
                                    PdfDetailCell.BorderWidthRight = 2f;
                                    PdfDetailCell.Colspan = 38;
                                    PdfDetailTable.AddCell(PdfDetailCell);

                                    PdfDetailCell = new PdfPCell(new Phrase(""));
                                    PdfDetailCell.NoWrap = false;
                                    PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell.HorizontalAlignment = 1;
                                    PdfDetailCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfDetailCell.BorderWidthLeft = 2f;
                                    PdfDetailCell.BorderWidthRight = 2f;
                                    PdfDetailCell.Colspan = 38;
                                    PdfDetailTable.AddCell(PdfDetailCell);

                                    PdfDetailCell = new PdfPCell(new Phrase(""));
                                    PdfDetailCell.NoWrap = false;
                                    PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell.HorizontalAlignment = 1;
                                    PdfDetailCell.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfDetailCell.BorderWidthLeft = 2f;
                                    PdfDetailTable.AddCell(PdfDetailCell);

                                    PdfDetailCell = new PdfPCell(new Phrase("This is to confirm that declaration has been carefully read, understood & made by me/us. I'm authorizing the user entity/Corporate to debit my account, based on the instruction as agreed and signed by me. I've understood that I'm authorized to cancel/amend this mandate by appropriately communicating the cancellation/amendment request to the user/entity/corporate or the bank where I've authorized the debit.", fontText5));
                                    //PdfDetailCell = new PdfPCell(new Phrase("This is to confirm that declaration has been carefully read, understood & made by me/us. I'm authorizing the user entity/Corporate to debit my account, based on the instruction as agreed and signed by me. I've understood that I'm authorized to cancel/amend this mandate by appropriately communicating the cancellation/amendment request to the user/entity/corporate or the bank where I've authorized the debit.", fontText5));
                                    PdfDetailCell.NoWrap = false;
                                    PdfDetailCell.Colspan = 34;
                                    PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_LEFT;
                                    //PdfDetailCell.HorizontalAlignment = 1;
                                    PdfDetailCell.Border = Rectangle.NO_BORDER;
                                    PdfDetailTable.AddCell(PdfDetailCell);


                                    PdfDetailCell = new PdfPCell(new Phrase(""));
                                    PdfDetailCell.Border = Rectangle.NO_BORDER;
                                    //PdfDetailCell.Border = Rectangle.RIGHT_BORDER;
                                    PdfDetailCell.BorderWidthRight = 2f;
                                    PdfDetailTable.AddCell(PdfDetailCell);


                                    //PdfDetailCell = new PdfPCell(new Phrase(""));
                                    //PdfDetailCell.NoWrap = false;
                                    //PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    //PdfDetailCell.HorizontalAlignment = 1;
                                    //PdfDetailCell.Border = Rectangle.NO_BORDER;//Avinash
                                    //PdfDetailCell.BorderWidthRight = 2f;
                                    //PdfDetailTable.AddCell(PdfDetailCell);

                                    //PdfDetailCell = new PdfPCell(new Phrase(" "));
                                    //PdfDetailCell.Colspan = 36;
                                    //PdfDetailCell.Border = Rectangle.NO_BORDER;
                                    ////PdfDetailCell.Border = Rectangle.LEFT_BORDER;
                                    //PdfDetailCell.BorderWidthLeft = 2f;
                                    //PdfDetailCell.BorderWidthRight = 2f;
                                    //PdfDetailTable.AddCell(PdfDetailCell);


                                    //PdfDetailCell = new PdfPCell(new Phrase(" "));
                                    //PdfDetailCell.Border = Rectangle.NO_BORDER;
                                    //PdfDetailCell.Border = Rectangle.RIGHT_BORDER;
                                    //PdfDetailCell.BorderWidthRight = 2f;
                                    //PdfDetailTable.AddCell(PdfDetailCell);

                                    PdfDetailCell = new PdfPCell(new Phrase(" "));
                                    PdfDetailCell.Colspan = 36;
                                    PdfDetailCell.Border = Rectangle.NO_BORDER;
                                    PdfDetailCell.BorderWidthRight = 2f;
                                    PdfDetailCell.FixedHeight = 5f;
                                    PdfDetailCell.BorderWidthLeft = 2f;
                                    PdfDetailCell.BorderWidthBottom = 2f;
                                    PdfDetailTable.AddCell(PdfDetailCell);


                                    //PdfDetailCell = new PdfPCell(new Phrase(" "));
                                    //PdfDetailCell.Border = Rectangle.NO_BORDER;
                                    //PdfDetailCell.Border = Rectangle.RIGHT_BORDER;
                                    //PdfDetailCell.BorderWidthRight = 2f;
                                    //PdfDetailTable.AddCell(PdfDetailCell);

                                    #endregion

                                    #region Table1
                                    PdfPTable PdfHeaderTable1 = new PdfPTable(33);
                                    PdfHeaderTable1.DefaultCell.NoWrap = false;
                                    PdfHeaderTable1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderTable1.WidthPercentage = 100;
                                    float[] Headerwidths11 = new float[] { 1f, 4f, 3f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                                    //float[] Headerwidths = new float[] { 4f, 3f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                                    PdfHeaderTable1.SetWidths(Headerwidths11);
                                    PdfPCell PdfHeaderCell1 = null;
                                    Document document1 = new Document();
                                    document1.Open();

                                    PdfHeaderCell1 = new PdfPCell(new Phrase(""));
                                    PdfHeaderCell1.Colspan = 33;
                                    PdfHeaderCell1.Border = Rectangle.NO_BORDER;
                                    PdfHeaderCell1.BorderWidthTop = 2f;
                                    PdfHeaderCell1.BorderWidthLeft = 2f;
                                    PdfHeaderCell1.BorderWidthRight = 2f;
                                    PdfHeaderTable1.AddCell(PdfHeaderCell1);

                                    //PdfHeaderCell1 = new PdfPCell(new Phrase(""));
                                    //PdfHeaderCell1.Colspan = 33;
                                    //PdfHeaderCell1.Border = Rectangle.NO_BORDER;
                                    //PdfHeaderCell1.BorderWidthLeft = 2f;
                                    //PdfHeaderCell1.BorderWidthRight = 2f;
                                    //PdfHeaderTable1.AddCell(PdfHeaderCell1);

                                    PdfHeaderCell1 = new PdfPCell(new Phrase(" "));
                                    PdfHeaderCell1.FixedHeight = 35f;
                                    PdfHeaderCell1.Rowspan = 3;
                                    PdfHeaderCell1.Border = Rectangle.NO_BORDER;//Avinash                        
                                    PdfHeaderCell1.BorderWidthLeft = 2f;
                                    PdfHeaderTable1.AddCell(PdfHeaderCell1);


                                    PdfHeaderCell1 = new PdfPCell(LogoImage);
                                    PdfHeaderCell1.FixedHeight = 50f;
                                    PdfHeaderCell1.Rowspan = 3;
                                    PdfHeaderCell1.Border = Rectangle.NO_BORDER;
                                    PdfHeaderTable1.AddCell(PdfHeaderCell1);

                                    PdfHeaderCell1 = new PdfPCell(new Phrase("UMRN", fontAb11B));
                                    PdfHeaderCell1.NoWrap = false;
                                    PdfHeaderCell1.BackgroundColor = new Color(252, 252, 252);
                                    PdfHeaderCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    //PdfHeaderCell1.FixedHeight = 20f;
                                    PdfHeaderCell1.HorizontalAlignment = 1;
                                    //PdfHeaderCell1.Border = Rectangle.TOP_BORDER;
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

                                    //-------------------------------Add Date-------------------------------

                                    PdfHeaderCell1 = new PdfPCell(new Phrase("Date", fontAb11B));
                                    PdfHeaderCell1.NoWrap = false;
                                    PdfHeaderCell1.BackgroundColor = new Color(252, 252, 252);
                                    PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderCell1.HorizontalAlignment = 1;
                                    PdfHeaderCell1.Border = Rectangle.NO_BORDER;//Avinash
                                                                                //PdfHeaderCell1.Border = Rectangle.TOP_BORDER;
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

                                    PdfHeaderCell1 = new PdfPCell(new Phrase(" "));
                                    PdfHeaderCell1.NoWrap = false;
                                    PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderCell1.HorizontalAlignment = 1;
                                    PdfHeaderCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfHeaderCell1.BorderWidthRight = 2f;
                                    PdfHeaderTable1.AddCell(PdfHeaderCell1);

                                    //----------------------------------------Add Sponsor bankcode---------------------
                                    string SpBankCode1 = dr["SponserBankCode"].ToString();
                                    PdfHeaderCell1 = new PdfPCell(new Phrase("Sponsor bankcode", fontAb11B));
                                    PdfHeaderCell1.NoWrap = false;
                                    PdfHeaderCell1.BackgroundColor = new Color(252, 252, 252);
                                    PdfHeaderCell1.Colspan = 4;
                                    PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderCell1.HorizontalAlignment = 1;
                                    PdfHeaderCell1.FixedHeight = 15f;
                                    PdfHeaderCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfHeaderCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfHeaderTable1.AddCell(PdfHeaderCell1);

                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 1)
                                    {
                                        PdfHeaderCell1 = new PdfPCell(new Phrase(dr["SponserBankCode"].ToString(), fontAb11B));
                                    }
                                    else
                                    {
                                        PdfHeaderCell1 = new PdfPCell(new Phrase(" ", fontAb11B));
                                    }
                                    PdfHeaderCell1.NoWrap = false;
                                    PdfHeaderCell1.Colspan = 12;
                                    PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderCell1.HorizontalAlignment = 1;
                                    PdfHeaderCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfHeaderTable1.AddCell(PdfHeaderCell1);


                                    PdfHeaderCell1 = new PdfPCell(new Phrase("Utility Code", fontAb11B));
                                    PdfHeaderCell1.NoWrap = false;
                                    PdfHeaderCell1.Colspan = 6;
                                    PdfHeaderCell1.BackgroundColor = new Color(252, 252, 252);
                                    PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderCell1.HorizontalAlignment = 1;
                                    PdfHeaderCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfHeaderCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfHeaderTable1.AddCell(PdfHeaderCell1);

                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 1)
                                    {
                                        PdfHeaderCell1 = new PdfPCell(new Phrase(dr["UtilityCode"].ToString(), fontAb11B));
                                    }
                                    else
                                    {
                                        PdfHeaderCell1 = new PdfPCell(new Phrase(" ", fontAb11B));
                                    }
                                    PdfHeaderCell1.NoWrap = false;
                                    PdfHeaderCell1.Colspan = 8;
                                    PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                                    PdfHeaderCell1.HorizontalAlignment = 1;
                                    //PdfHeaderCell1.FixedHeight = 20f;
                                    PdfHeaderCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfHeaderTable1.AddCell(PdfHeaderCell1);


                                    PdfHeaderCell1 = new PdfPCell(new Phrase(" "));
                                    PdfHeaderCell1.NoWrap = false;
                                    PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderCell1.HorizontalAlignment = 1;
                                    PdfHeaderCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfHeaderCell1.BorderWidthRight = 2f;
                                    PdfHeaderTable1.AddCell(PdfHeaderCell1);


                                    ////------------------------------- add Created Status-------------------------------------
                                    Document documentCheckBox1 = new Document();
                                    PdfHeaderCell1 = new PdfPCell(new Phrase("I/We hereby Authorize", fontAb11B));

                                    PdfHeaderCell1.NoWrap = false;
                                    PdfHeaderCell1.BackgroundColor = new Color(252, 252, 252);
                                    PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderCell1.Colspan = 5;
                                    PdfHeaderCell1.FixedHeight = 15f;
                                    PdfHeaderCell1.HorizontalAlignment = 1;
                                    PdfHeaderCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfHeaderCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfHeaderTable1.AddCell(PdfHeaderCell1);


                                    //PdfHeaderCell1 = new PdfPCell(new Phrase(dr["CompanyName"].ToString(), fontAb11B));
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 1)
                                    {
                                        if (count_IherebyAuthorize < 34)
                                        {
                                            PdfHeaderCell1 = new PdfPCell(new Phrase(dr["CompanyName"].ToString(), fontAb11B));
                                        }
                                        else if (count_IherebyAuthorize < 40)
                                        {
                                            PdfHeaderCell1 = new PdfPCell(new Phrase(dr["CompanyName"].ToString(), fontText));
                                        }
                                        else if (count_IherebyAuthorize < 48)
                                        {
                                            PdfHeaderCell1 = new PdfPCell(new Phrase(dr["CompanyName"].ToString(), fontText6));
                                        }
                                        else
                                        {
                                            PdfHeaderCell1 = new PdfPCell(new Phrase(dr["CompanyName"].ToString(), fontText5));
                                        }
                                    }
                                    else
                                    {
                                        PdfHeaderCell1 = new PdfPCell(new Phrase(" ", fontText6));
                                    }

                                    PdfHeaderCell1.NoWrap = false;
                                    PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderCell1.Colspan = 10;
                                    PdfHeaderCell1.HorizontalAlignment = 1;
                                    PdfHeaderCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfHeaderTable1.AddCell(PdfHeaderCell1);

                                    PdfHeaderCell1 = new PdfPCell(new Phrase("To Debit", fontAb11B));
                                    PdfHeaderCell1.NoWrap = false;
                                    PdfHeaderCell1.BackgroundColor = new Color(252, 252, 252);
                                    PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderCell1.Colspan = 3;
                                    PdfHeaderCell1.HorizontalAlignment = 1;
                                    PdfHeaderCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfHeaderCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfHeaderTable1.AddCell(PdfHeaderCell1);

                                    Document documentCheckBox1SB1 = new Document();
                                    documentCheckBox1SB1.Open();
                                    Paragraph pCheckBox1SB1 = new Paragraph();
                                    //----------------------------------add To Debit---------------------------
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 1)
                                    {
                                        string chDebit1 = dr["DebitTo"].ToString();
                                        if (chDebit1 == "SB")
                                        {
                                            //   pCheckBox1SB1.Add(new Phrase(" ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" SB/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" CA/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" CC/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" SB-NRE/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" SB-NRO/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" OTHER ", fontText));
                                        }
                                        else if (chDebit1 == "CA")
                                        {
                                            // pCheckBox1SB1.Add(new Phrase(" ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" SB/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" CA/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" CC/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" SB-NRE/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" SB-NRO/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" OTHER ", fontText));
                                        }

                                        else if (chDebit1 == "CC")
                                        {
                                            //pCheckBox1SB1.Add(new Phrase(" ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" SB/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" CA/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" CC/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" SB-NRE/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" SB-NRO/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" OTHER ", fontText));
                                        }
                                        else if (chDebit1 == "RE")
                                        {
                                            // pCheckBox1SB1.Add(new Phrase(" ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" SB/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" CA/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" CC/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" SB-NRE/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" SB-NRO/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" OTHER ", fontText));
                                        }
                                        else if (chDebit1 == "RD")
                                        {
                                            // pCheckBox1SB1.Add(new Phrase(" ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" SB/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" CA/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" CC/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" SB-NRE/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" SB-NRO/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" OTHER ", fontText));
                                        }
                                        else if (chDebit1 == "OT")
                                        {
                                            // pCheckBox1SB1.Add(new Phrase(" ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" SB/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" CA/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" CC/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" SB-NRE/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" SB-NRO/ ", fontText));
                                            pCheckBox1SB1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB1.Add(new Phrase(" OTHER ", fontText));
                                        }
                                    }
                                    else
                                    {
                                        pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pCheckBox1SB1.Add(new Phrase(" SB/ ", fontText));
                                        pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pCheckBox1SB1.Add(new Phrase(" CA/ ", fontText));
                                        pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pCheckBox1SB1.Add(new Phrase(" CC/ ", fontText));
                                        pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pCheckBox1SB1.Add(new Phrase(" SB-NRE/ ", fontText));
                                        pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pCheckBox1SB1.Add(new Phrase(" SB-NRO/ ", fontText));
                                        pCheckBox1SB1.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pCheckBox1SB1.Add(new Phrase(" OTHER ", fontText));
                                    }
                                    PdfHeaderCell1 = new PdfPCell(pCheckBox1SB1);
                                    PdfHeaderCell1.NoWrap = false;
                                    PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    //PdfHeaderCell.Colspan = 11;
                                    PdfHeaderCell1.Colspan = 12;
                                    PdfHeaderCell1.HorizontalAlignment = 1;
                                    PdfHeaderCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfHeaderCell1.Border = Rectangle.NO_BORDER;//Avinash                        
                                    PdfHeaderTable1.AddCell(PdfHeaderCell1);

                                    PdfHeaderCell1 = new PdfPCell(new Phrase(" "));
                                    PdfHeaderCell1.NoWrap = false;
                                    PdfHeaderCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderCell1.HorizontalAlignment = 1;
                                    PdfHeaderCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfHeaderCell1.BorderWidthRight = 2f;
                                    PdfHeaderTable1.AddCell(PdfHeaderCell1);

                                    //PdfPTable PdfMidTable1 = new PdfPTable(32);
                                    PdfPTable PdfMidTable1 = new PdfPTable(34);
                                    PdfMidTable1.DefaultCell.NoWrap = false;
                                    PdfMidTable1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidTable1.DefaultCell.Border = PdfCell.NO_BORDER;
                                    PdfMidTable1.WidthPercentage = 100;
                                    float[] PdfMidTable1Headerwidths11 = new float[] { 1f, 4f, 3f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                                    //float[] PdfMidTable1Headerwidths = new float[] { 4f, 3f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                                    PdfMidTable1.SetWidths(PdfMidTable1Headerwidths11);

                                    PdfPCell PdfMidCell1 = null;
                                    PdfMidCell1 = new PdfPCell(new Phrase(" "));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell1.BorderWidthLeft = 2f;
                                    PdfMidTable1.AddCell(PdfMidCell1);


                                    documentCheckBox1.Open();
                                    Paragraph pCheckBox1 = new Paragraph();
                                    string status_1 = dr["CreatedStatus"].ToString();
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 1)
                                    {
                                        if (status_1 == "C")
                                        {
                                            pCheckBox1.Add(new Phrase("CREATE ", fontText));
                                            pCheckBox1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1.Add(new Phrase(" MODIFY ", fontText));
                                            pCheckBox1.Add(new Phrase(" CANCEL ", fontText));

                                        }
                                        else if (status_1 == "M")
                                        {
                                            pCheckBox1.Add(new Phrase("CREATE ", fontText));
                                            pCheckBox1.Add(new Phrase(" MODIFY ", fontText));
                                            pCheckBox1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1.Add(new Phrase(" CANCEL ", fontText));

                                        }
                                        else if (status_1 == "L")
                                        {
                                            pCheckBox1.Add(new Phrase("CREATE ", fontText));

                                            pCheckBox1.Add(new Phrase(" MODIFY ", fontText));

                                            pCheckBox1.Add(new Phrase(" CANCEL ", fontText));
                                            pCheckBox1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        }
                                    }
                                    else
                                    {
                                        pCheckBox1.Add(new Phrase("CREATE ", fontText));

                                        pCheckBox1.Add(new Phrase(" MODIFY ", fontText));

                                        pCheckBox1.Add(new Phrase(" CANCEL ", fontText));

                                    }
                                    PdfMidCell1 = new PdfPCell(pCheckBox1);
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.Border = Rectangle.NO_BORDER;
                                    PdfMidTable1.AddCell(PdfMidCell1);


                                    //----------------------------------Add AccountNo.-------------------------------------------------
                                    PdfMidCell1 = new PdfPCell(new Phrase("Bank a/c Number", fontAb11B));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.Colspan = 5;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfMidCell1.Border = Rectangle.NO_BORDER;
                                    PdfMidTable1.AddCell(PdfMidCell1);


                                    string AccountNo1 = dr["AccountNo"].ToString();
                                    char[] chrAcountNo1 = new char[Convert.ToInt32(AccountNo1.Length)];
                                    chrAcountNo1 = AccountNo1.ToCharArray();
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 1)
                                    {
                                        if (Convert.ToInt32(AccountNo1.Length) <= 26)
                                        {
                                            for (int j = 0; j < Convert.ToInt32(chrAcountNo1.Length); j++)
                                            {
                                                PdfMidCell1 = new PdfPCell(new Phrase(chrAcountNo1[j].ToString(), fontA119B));
                                                PdfMidCell1.NoWrap = false;
                                                PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                                PdfMidCell1.HorizontalAlignment = 1;
                                                PdfMidCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                                PdfMidTable1.AddCell(PdfMidCell1);
                                            }

                                            int len = 26 - Convert.ToInt32(AccountNo1.Length);
                                            for (int k = 0; k < len; k++)
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
                                        if (Convert.ToInt32(AccountNo1.Length) <= 26)
                                        {
                                            for (int j = 0; j < Convert.ToInt32(chrAcountNo1.Length); j++)
                                            {
                                                PdfMidCell1 = new PdfPCell(new Phrase(" ", fontA119B));
                                                PdfMidCell1.NoWrap = false;
                                                PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                                PdfMidCell1.HorizontalAlignment = 1;
                                                PdfMidCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                                PdfMidTable1.AddCell(PdfMidCell1);
                                            }

                                            int len = 26 - Convert.ToInt32(AccountNo1.Length);
                                            for (int k = 0; k < len; k++)
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

                                    PdfMidCell1 = new PdfPCell(new Phrase(" "));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell1.BorderWidthRight = 2f;
                                    PdfMidTable1.AddCell(PdfMidCell1);

                                    PdfMidCell1 = new PdfPCell(new Phrase(" "));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell1.BorderWidthLeft = 2f;
                                    PdfMidTable1.AddCell(PdfMidCell1);


                                    PdfMidCell1 = new PdfPCell(new Phrase("With Bank", fontAb11B));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.FixedHeight = 25f;
                                    PdfMidCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfMidCell1.Border = Rectangle.NO_BORDER;//Avinash
                                                                             //PdfMidCell1.Border = Rectangle.LEFT_BORDER;
                                                                             //PdfMidCell1.BorderWidth = 2f;
                                    PdfMidTable1.AddCell(PdfMidCell1);

                                    //PdfMidCell1 = new PdfPCell(new Phrase(dr["BankName"].ToString(), fontAb11));
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 1)
                                    {
                                        if (count_BankName < 34)
                                        {
                                            PdfMidCell1 = new PdfPCell(new Phrase(dr["BankName"].ToString(), fontAb11B));
                                        }
                                        else if (count_BankName < 40)
                                        {
                                            PdfMidCell1 = new PdfPCell(new Phrase(dr["BankName"].ToString(), fontText));
                                        }
                                        else if (count_BankName < 48)
                                        {
                                            PdfMidCell1 = new PdfPCell(new Phrase(dr["BankName"].ToString(), fontText6));
                                        }
                                        else
                                        {
                                            PdfMidCell1 = new PdfPCell(new Phrase(dr["BankName"].ToString(), fontText5));
                                        }
                                    }
                                    else
                                    {
                                        PdfMidCell1 = new PdfPCell(new Phrase(" ", fontAb11B));
                                    }
                                    PdfMidCell1.NoWrap = false;
                                    //PdfMidCell1.FixedHeight = 30f;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.Colspan = 6;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfMidTable1.AddCell(PdfMidCell1);


                                    PdfMidCell1 = new PdfPCell(new Phrase("IFSC", fontAb11B));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.Colspan = 2;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfMidCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidTable1.AddCell(PdfMidCell1);

                                    //-------------------------Add IFSC code--------------------------------
                                    string IFSCcode1 = dr["IFSCcode"].ToString();
                                    char[] chrIFSCcode1 = new char[Convert.ToInt32(IFSCcode1.Length)];
                                    chrIFSCcode1 = IFSCcode1.ToCharArray();
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 1)
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
                                        if (Convert.ToInt32(chrIFSCcode1.Length) == 11)
                                        {
                                            for (int j = 0; j < Convert.ToInt32(chrIFSCcode1.Length); j++)
                                            {
                                                PdfMidCell1 = new PdfPCell(new Phrase(" ", fontA119B));
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
                                    PdfMidCell1 = new PdfPCell(new Phrase("or MICR", fontAb11B));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.Colspan = 3;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfMidCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidTable1.AddCell(PdfMidCell1);

                                    //-------------------------Add MICRcode--------------------------------
                                    string MICRcode1 = dr["MICRcode"].ToString();
                                    char[] chrMICRcode1 = new char[9];
                                    chrMICRcode1 = MICRcode1.ToCharArray();

                                    if (Convert.ToInt32(chrMICRcode1.Length) == 9)
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

                                    PdfMidCell1 = new PdfPCell(new Phrase(" "));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell1.BorderWidthRight = 2f;
                                    PdfMidTable1.AddCell(PdfMidCell1);
                                    //-----------------------------------Add amount of Rupees---------------------------

                                    PdfMidCell1 = new PdfPCell(new Phrase(" "));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell1.BorderWidthLeft = 2f;
                                    PdfMidTable1.AddCell(PdfMidCell1);

                                    PdfMidCell1 = new PdfPCell(new Phrase("an amount of Rupees", fontAb11B));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.Colspan = 2;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.Border = Rectangle.NO_BORDER;//Avinash                        
                                                                             //PdfMidCell1.Border = Rectangle.LEFT_BORDER;
                                                                             //PdfMidCell1.BorderWidth = 2f;
                                    PdfMidCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_LEFT;
                                    PdfMidTable1.AddCell(PdfMidCell1);
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 1)
                                    {
                                        PdfMidCell1 = new PdfPCell(new Phrase(dr["AmountInWord"].ToString(), fontA11B));
                                    }
                                    else
                                    {
                                        PdfMidCell1 = new PdfPCell(new Phrase(" ", fontA11B));
                                    }
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
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 1)
                                    {
                                        pAmountInDigit1.Add(new Phrase(" " + dr["AmountInDigit"].ToString(), fontA119B));
                                    }
                                    else
                                    {
                                        pAmountInDigit1.Add(new Phrase(" " + " ", fontA119B));
                                    }

                                    PdfMidCell1 = new PdfPCell(pAmountInDigit1);
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    //PdfMidCell1.Colspan = 10;
                                    PdfMidCell1.Colspan = 8;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidTable1.AddCell(PdfMidCell1);

                                    PdfMidCell1 = new PdfPCell(new Phrase(" "));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell1.BorderWidthRight = 2f;
                                    PdfMidTable1.AddCell(PdfMidCell1);

                                    //---------------------------------Add Frequency--------------------------------------
                                    PdfMidCell1 = new PdfPCell(new Phrase(" "));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell1.BorderWidthLeft = 2f;
                                    PdfMidTable1.AddCell(PdfMidCell1);

                                    string Freq1 = dr["Frequency"].ToString();
                                    PdfMidCell1 = new PdfPCell(new Phrase("Frequency", fontText));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.Border = Rectangle.NO_BORDER;//Avinash
                                                                             //PdfMidCell1.Border = Rectangle.LEFT_BORDER;
                                                                             //PdfMidCell1.BorderWidth = 2f;
                                    PdfMidCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_LEFT;
                                    PdfMidTable1.AddCell(PdfMidCell1);


                                    Document documentMonthly1 = new Document();
                                    documentMonthly1.Open();
                                    Paragraph pMonthly1 = new Paragraph();
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 1)
                                    {
                                        //------------------------------- add Monthly-------------------------------------
                                        if (Freq1 == "M")
                                        {
                                            pMonthly1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pMonthly1.Add(new Phrase("  Monthly", fontText));
                                        }
                                        else
                                        {
                                            pMonthly1.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pMonthly1.Add(new Phrase("  Monthly", fontText));
                                        }

                                    }
                                    else
                                    {
                                        pMonthly1.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pMonthly1.Add(new Phrase("  Monthly", fontText));
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
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 1)
                                    {
                                        if (Freq1 == "Q")
                                        {
                                            pQtly1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pQtly1.Add(new Phrase(" Qtly", fontText));
                                        }
                                        else
                                        {
                                            pQtly1.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pQtly1.Add(new Phrase(" Qtly", fontText));
                                        }
                                    }
                                    else
                                    {
                                        pQtly1.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pQtly1.Add(new Phrase(" Qtly", fontText));
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
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 1)
                                    {
                                        if (Freq1 == "H")
                                        {
                                            pHYrly1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pHYrly1.Add(new Phrase("  H-Yrly", fontText));
                                        }
                                        else
                                        {
                                            pHYrly1.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pHYrly1.Add(new Phrase("  H-Yrly", fontText));
                                        }

                                    }
                                    else
                                    {
                                        pHYrly1.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pHYrly1.Add(new Phrase("  H-Yrly", fontText));
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
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 1)
                                    {
                                        if (Freq1 == "Y")
                                        {
                                            pYearly1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pYearly1.Add(new Phrase("  Yearly", fontText));
                                        }
                                        else
                                        {
                                            pYearly1.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pYearly1.Add(new Phrase("  Yearly", fontText));
                                        }
                                    }
                                    else
                                    {
                                        pYearly1.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pYearly1.Add(new Phrase("  Yearly", fontText));
                                    }

                                    PdfMidCell1 = new PdfPCell(pYearly1);
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.Colspan = 3;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidTable1.AddCell(PdfMidCell1);

                                    Document prensented1prensented11 = new Document();
                                    prensented1prensented11.Open();
                                    Paragraph prensented1 = new Paragraph();
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 1)
                                    {
                                        if (Freq1 == "A")
                                        {
                                            prensented1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            prensented1.Add(new Phrase("  As & when prensented", fontText));
                                        }
                                        else
                                        {
                                            prensented1.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            prensented1.Add(new Phrase("  As & when prensented", fontText));
                                        }
                                    }
                                    else
                                    {
                                        prensented1.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        prensented1.Add(new Phrase("  As & when prensented", fontText));
                                    }

                                    PdfMidCell1 = new PdfPCell(prensented1);
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.Colspan = 7;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidTable1.AddCell(PdfMidCell1);

                                    string DebitType1 = dr["DebitType"].ToString();

                                    PdfMidCell1 = new PdfPCell(new Phrase("Debit Type", fontText));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.Colspan = 3;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_LEFT;
                                    PdfMidTable1.AddCell(PdfMidCell1);

                                    Document documentFixed1 = new Document();
                                    documentFixed1.Open();
                                    Paragraph pFixed1 = new Paragraph();
                                    //------------------------------- add H-Yrly-------------------------------------
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 1)
                                    {
                                        if (DebitType1 == "F")
                                        {
                                            pFixed1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pFixed1.Add(new Phrase("  Fixed Amount", fontText));
                                        }
                                        else
                                        {
                                            pFixed1.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pFixed1.Add(new Phrase("  Fixed Amount", fontText));
                                        }
                                    }
                                    else
                                    {
                                        pFixed1.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pFixed1.Add(new Phrase("  Fixed Amount", fontText));
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
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 1)
                                    {
                                        if (DebitType1 == "M")
                                        {
                                            pMaximum1.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pMaximum1.Add(new Phrase("  Maximum Amount", fontText));
                                        }
                                        else
                                        {
                                            pMaximum1.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pMaximum1.Add(new Phrase("  Maximum Amount", fontText));
                                        }
                                    }
                                    else
                                    {
                                        pMaximum1.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pMaximum1.Add(new Phrase("  Maximum Amount", fontText));
                                    }

                                    PdfMidCell1 = new PdfPCell(pMaximum1);
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.Colspan = 6;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidTable1.AddCell(PdfMidCell1);


                                    PdfMidCell1 = new PdfPCell(new Phrase(" "));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell1.BorderWidthRight = 2f;
                                    PdfMidTable1.AddCell(PdfMidCell1);

                                    PdfMidCell1 = new PdfPCell(new Phrase(" "));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell1.BorderWidthLeft = 2f;
                                    PdfMidTable1.AddCell(PdfMidCell1);

                                    PdfMidCell1 = new PdfPCell(new Phrase("Reference 1", fontAb11B));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.Border = Rectangle.NO_BORDER;//Avinash
                                                                             //PdfMidCell1.Border = Rectangle.LEFT_BORDER;
                                                                             //PdfMidCell1.BorderWidth = 2f;
                                    PdfMidCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_LEFT;
                                    PdfMidTable1.AddCell(PdfMidCell1);

                                    //PdfMidCell1 = new PdfPCell(new Phrase(dr["Reference1"].ToString(), fontAb11B));

                                    if (count_Ref1 < 34)
                                    {
                                        PdfMidCell1 = new PdfPCell(new Phrase(dr["Reference1"].ToString(), fontAb11B));
                                    }
                                    else if (count_Ref1 < 40)
                                    {
                                        PdfMidCell1 = new PdfPCell(new Phrase(dr["Reference1"].ToString(), fontText));
                                    }
                                    else if (count_Ref1 < 48)
                                    {
                                        PdfMidCell1 = new PdfPCell(new Phrase(dr["Reference1"].ToString(), fontText6));
                                    }
                                    else
                                    {
                                        PdfMidCell1 = new PdfPCell(new Phrase(dr["Reference1"].ToString(), fontText5));
                                    }

                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.Colspan = 15;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidTable1.AddCell(PdfMidCell1);


                                    PdfMidCell1 = new PdfPCell(new Phrase("Phone Number ", fontAb11B));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.Colspan = 6;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidTable1.AddCell(PdfMidCell1);

                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 1)
                                    {
                                        PdfMidCell1 = new PdfPCell(new Phrase(dr["PhoneNo"].ToString(), fontAb11B));
                                    }
                                    else
                                    {
                                        PdfMidCell1 = new PdfPCell(new Phrase(" ", fontAb11B));
                                    }
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.Colspan = 10;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidTable1.AddCell(PdfMidCell1);

                                    PdfMidCell1 = new PdfPCell(new Phrase(" "));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell1.BorderWidthRight = 2f;
                                    PdfMidTable1.AddCell(PdfMidCell1);

                                    PdfMidCell1 = new PdfPCell(new Phrase(" "));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell1.BorderWidthLeft = 2f;
                                    PdfMidTable1.AddCell(PdfMidCell1);

                                    PdfMidCell1 = new PdfPCell(new Phrase("Reference 2", fontAb11B));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.Border = Rectangle.NO_BORDER;
                                    PdfMidCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_LEFT;
                                    PdfMidTable1.AddCell(PdfMidCell1);

                                    //PdfMidCell1 = new PdfPCell(new Phrase(dr["Reference2"].ToString(), fontAb11B));
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 1)
                                    {
                                        if (count_Ref2 < 34)
                                        {
                                            PdfMidCell1 = new PdfPCell(new Phrase(dr["Reference2"].ToString(), fontAb11B));
                                        }
                                        else if (count_Ref2 < 40)
                                        {
                                            PdfMidCell1 = new PdfPCell(new Phrase(dr["Reference2"].ToString(), fontText));
                                        }
                                        else if (count_Ref2 < 48)
                                        {
                                            PdfMidCell1 = new PdfPCell(new Phrase(dr["Reference2"].ToString(), fontText6));
                                        }
                                        else
                                        {
                                            PdfMidCell1 = new PdfPCell(new Phrase(dr["Reference2"].ToString(), fontText5));
                                        }
                                    }
                                    else
                                    {
                                        PdfMidCell1 = new PdfPCell(new Phrase(" ", fontText5));
                                    }
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.Colspan = 15;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidTable1.AddCell(PdfMidCell1);


                                    PdfMidCell1 = new PdfPCell(new Phrase("EMail ID", fontAb11B));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.Colspan = 6;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidTable1.AddCell(PdfMidCell1);

                                    //PdfMidCell1 = new PdfPCell(new Phrase(dr["EmailId"].ToString(), fontAb11B));
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 1)
                                    {
                                        if (count < 34)
                                        {
                                            PdfMidCell1 = new PdfPCell(new Phrase(dr["EmailId"].ToString(), fontAb11B));
                                        }
                                        //else if (count < 40)
                                        //{
                                        //    PdfMidCell1 = new PdfPCell(new Phrase(dr["EmailId"].ToString(), fontText));
                                        //}
                                        //else if (count < 48)
                                        //{
                                        //    PdfMidCell1 = new PdfPCell(new Phrase(dr["EmailId"].ToString(), fontText6));
                                        //}
                                        else
                                        {
                                            PdfMidCell1 = new PdfPCell(new Phrase(dr["EmailId"].ToString(), fontText5));
                                        }
                                    }
                                    else
                                    {
                                        PdfMidCell1 = new PdfPCell(new Phrase(" ", fontText5));
                                    }
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.Colspan = 10;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidTable1.AddCell(PdfMidCell1);

                                    PdfMidCell1 = new PdfPCell(new Phrase(" "));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell1.BorderWidthRight = 2f;
                                    PdfMidTable1.AddCell(PdfMidCell1);

                                    PdfMidCell1 = new PdfPCell(new Phrase(" "));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell1.BorderWidthLeft = 2f;
                                    PdfMidTable1.AddCell(PdfMidCell1);

                                    PdfMidCell1 = new PdfPCell(new Phrase("PERIOD", fontText));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell1.FixedHeight = 8f;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidTable1.AddCell(PdfMidCell1);//

                                    PdfMidCell1 = new PdfPCell(new Phrase("I agree for the debit of mandate processing charges by the bank whom I am authorizing to debit my account as per latest schedule of charges of bank ", fontText6));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.Border = Rectangle.NO_BORDER;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.Colspan = 31;//Avinash
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidTable1.AddCell(PdfMidCell1);


                                    PdfMidCell1 = new PdfPCell(new Phrase(" "));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell1.BorderWidthRight = 2f;
                                    PdfMidTable1.AddCell(PdfMidCell1);

                                    PdfMidCell1 = new PdfPCell(new Phrase("", fontAb11B));
                                    PdfMidCell1.NoWrap = false;
                                    PdfMidCell1.Border = Rectangle.NO_BORDER;
                                    PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell1.Colspan = 31;
                                    PdfMidCell1.HorizontalAlignment = 1;
                                    PdfMidTable1.AddCell(PdfMidCell1);


                                    PdfPTable PdfDetailTable1 = new PdfPTable(36);
                                    PdfDetailTable1.DefaultCell.NoWrap = false;
                                    PdfDetailTable1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailTable1.DefaultCell.Border = PdfCell.NO_BORDER;
                                    PdfDetailTable1.WidthPercentage = 100;
                                    //float[] Headerwidths1 = new float[] { 4f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                                    float[] Headerwidths1_1 = new float[] { 1f, 4f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                                    PdfDetailTable1.SetWidths(Headerwidths1_1);
                                    PdfPCell PdfDetailCell1 = null;


                                    PdfDetailCell1 = new PdfPCell(new Phrase(""));
                                    PdfDetailCell1.NoWrap = false;
                                    PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell1.HorizontalAlignment = 1;
                                    PdfDetailCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfDetailCell1.BorderWidthLeft = 2f;
                                    PdfDetailTable1.AddCell(PdfDetailCell1);


                                    PdfDetailCell1 = new PdfPCell(new Phrase("From", fontAb11B));
                                    PdfDetailCell1.NoWrap = false;
                                    //PdfDetailCell1.BackgroundColor = new Color(252, 252, 252);
                                    PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell1.HorizontalAlignment = 1;
                                    //PdfDetailCell1.BorderWidthLeft = 2f;
                                    //PdfDetailCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    //PdfDetailCell1.Border = Rectangle.LEFT_BORDER;
                                    PdfDetailTable1.AddCell(PdfDetailCell1);

                                    string PeriodFrom1 = dr["PeriodFrom"].ToString();
                                    char[] chrPeriodFrom1 = new char[8];
                                    chrPeriodFrom1 = PeriodFrom1.ToCharArray();
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 1)
                                    {
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
                                    }
                                    else
                                    {
                                        if (Convert.ToInt32(chrPeriodFrom1.Length) > 0)
                                        {
                                            for (int j = 0; j < Convert.ToInt32(chrPeriodFrom1.Length); j++)
                                            {
                                                PdfDetailCell1 = new PdfPCell(new Phrase(" ", fontA119B));
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
                                    }
                                    PdfDetailCell1 = new PdfPCell(new Phrase(" ", fontAb11));
                                    PdfDetailCell1.NoWrap = false;
                                    PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell1.HorizontalAlignment = 1;
                                    PdfDetailCell1.Border = Rectangle.NO_BORDER;
                                    //PdfDetailCell1.Border = Rectangle.RIGHT_BORDER;
                                    PdfDetailCell1.Colspan = 25;
                                    PdfDetailTable1.AddCell(PdfDetailCell1);

                                    PdfDetailCell1 = new PdfPCell(new Phrase(""));
                                    PdfDetailCell1.NoWrap = false;
                                    PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell1.HorizontalAlignment = 1;
                                    PdfDetailCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfDetailCell1.BorderWidthRight = 2f;
                                    PdfDetailTable1.AddCell(PdfDetailCell1);


                                    PdfDetailCell1 = new PdfPCell(new Phrase(""));
                                    PdfDetailCell1.NoWrap = false;
                                    PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell1.HorizontalAlignment = 1;
                                    PdfDetailCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfDetailCell1.BorderWidthLeft = 2f;
                                    PdfDetailTable1.AddCell(PdfDetailCell1);

                                    PdfDetailCell1 = new PdfPCell(new Phrase("To*", fontAb11B));
                                    PdfDetailCell1.NoWrap = false;
                                    PdfDetailCell1.BackgroundColor = new Color(252, 252, 252);
                                    PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell1.HorizontalAlignment = 1;
                                    //PdfDetailCell1.BorderWidthLeft = 2f;
                                    //PdfDetailCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    //PdfDetailCell1.Border = Rectangle.LEFT_BORDER;
                                    PdfDetailTable1.AddCell(PdfDetailCell1);

                                    string PeriodTo1 = dr["PeriodTo"].ToString();
                                    char[] chrPeriodTo1 = new char[8];
                                    chrPeriodTo1 = PeriodTo1.ToCharArray();
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 1)
                                    {
                                        if (Convert.ToInt32(chrPeriodTo1.Length) > 0)
                                        {
                                            if (dr["PeriodTo"].ToString() != "01011900")
                                            {
                                                for (int j = 0; j < Convert.ToInt32(chrPeriodTo1.Length); j++)
                                                {
                                                    PdfDetailCell1 = new PdfPCell(new Phrase(chrPeriodTo1[j].ToString(), fontA119B));
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
                                                    //PdfDetailCell1.BackgroundColor = new Color(0,0,0);
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
                                                PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                                PdfDetailCell1.HorizontalAlignment = 1;
                                                //PdfDetailCell1.BackgroundColor = new Color(0, 0, 0);
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
                                            PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                            PdfDetailCell1.HorizontalAlignment = 1;
                                            //PdfDetailCell1.BackgroundColor = new Color(0, 0, 0);
                                            PdfDetailTable1.AddCell(PdfDetailCell1);
                                        }
                                    }

                                    /*Avinash*/
                                    PdfDetailCell1 = new PdfPCell(new Phrase("Sign. Primary Acc. Holder", fontAb11BU));
                                    PdfDetailCell1.NoWrap = false;
                                    PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell1.HorizontalAlignment = 1;
                                    PdfDetailCell1.Border = Rectangle.NO_BORDER;
                                    PdfDetailCell1.Colspan = 8;
                                    PdfDetailTable1.AddCell(PdfDetailCell1);

                                    PdfDetailCell1 = new PdfPCell(new Phrase("Sign Acc. Holder", fontAb11BU));
                                    PdfDetailCell1.NoWrap = false;
                                    PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell1.HorizontalAlignment = 1;
                                    PdfDetailCell1.Border = Rectangle.NO_BORDER;
                                    PdfDetailCell1.Colspan = 8;
                                    PdfDetailTable1.AddCell(PdfDetailCell1);

                                    PdfDetailCell1 = new PdfPCell(new Phrase("Sign Acc. Holder", fontAb11BU));
                                    PdfDetailCell1.NoWrap = false;
                                    PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell1.HorizontalAlignment = 1;
                                    PdfDetailCell1.Border = Rectangle.NO_BORDER;
                                    //PdfDetailCell1.Border = Rectangle.RIGHT_BORDER;
                                    PdfDetailCell1.Colspan = 9;
                                    PdfDetailTable1.AddCell(PdfDetailCell1);
                                    /*Avinash*/

                                    PdfDetailCell1 = new PdfPCell(new Phrase(""));
                                    PdfDetailCell1.NoWrap = false;
                                    PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell1.HorizontalAlignment = 1;
                                    PdfDetailCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfDetailCell1.BorderWidthRight = 2f;
                                    PdfDetailTable1.AddCell(PdfDetailCell1);

                                    PdfDetailCell1 = new PdfPCell(new Phrase(""));
                                    PdfDetailCell1.NoWrap = false;
                                    PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell1.HorizontalAlignment = 1;
                                    PdfDetailCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfDetailCell1.BorderWidthLeft = 2f;
                                    PdfDetailTable1.AddCell(PdfDetailCell1);


                                    PdfDetailCell1 = new PdfPCell(new Phrase("Or", fontAb11B));
                                    PdfDetailCell1.NoWrap = false;
                                    PdfDetailCell1.BackgroundColor = new Color(252, 252, 252);
                                    PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell1.HorizontalAlignment = 1;
                                    //PdfDetailCell1.BorderWidthLeft = 2f;
                                    //PdfDetailCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    //PdfDetailCell1.Border = Rectangle.LEFT_BORDER;
                                    PdfDetailTable1.AddCell(PdfDetailCell1);


                                    Document documentCheckBox11231 = new Document();
                                    documentCheckBox11231.Open();
                                    Paragraph pCheckBox1123 = new Paragraph();
                                    if (dr["PeriodTo"].ToString() == "01011900")
                                    {
                                        pCheckBox1123.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                    }
                                    else
                                    {
                                        pCheckBox1123.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                    }
                                    pCheckBox1123.Add(new Phrase(" Until Cancelled ", fontAb11));
                                    PdfDetailCell1 = new PdfPCell(pCheckBox1123);
                                    PdfDetailCell1.NoWrap = false;
                                    PdfDetailCell1.Colspan = 8;
                                    PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell1.HorizontalAlignment = 1;
                                    PdfDetailTable1.AddCell(PdfDetailCell1);

                                    //PdfDetailCell1 = new PdfPCell(new Phrase(dr["BenificiaryName"].ToString(), fontAbCutomer));
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 1)
                                    {
                                        if (count_Customer < 10)
                                        {
                                            PdfDetailCell1 = new PdfPCell(new Phrase(dr["BenificiaryName"].ToString(), fontAb11B));
                                        }
                                        else if (count_Customer < 20)
                                        {
                                            PdfDetailCell1 = new PdfPCell(new Phrase(dr["BenificiaryName"].ToString(), fontText));
                                        }
                                        else if (count_Customer < 30)
                                        {
                                            PdfDetailCell1 = new PdfPCell(new Phrase(dr["BenificiaryName"].ToString(), fontText6));
                                        }
                                        else
                                        {
                                            PdfDetailCell1 = new PdfPCell(new Phrase(dr["BenificiaryName"].ToString(), fontText5));
                                        }
                                    }
                                    else
                                    {
                                        PdfDetailCell1 = new PdfPCell(new Phrase(" ", fontText5));
                                    }
                                    PdfDetailCell1.NoWrap = false;
                                    PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell1.HorizontalAlignment = 1;
                                    PdfDetailCell1.Border = Rectangle.NO_BORDER;
                                    //PdfDetailCell1.Colspan = 13;
                                    PdfDetailCell1.Colspan = 7;
                                    PdfDetailTable1.AddCell(PdfDetailCell1);

                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 1)
                                    {

                                        if (count_Customer < 10)
                                        {
                                            PdfDetailCell1 = new PdfPCell(new Phrase(dr["Customer2"].ToString(), fontAb11B));
                                        }
                                        else if (count_Customer < 20)
                                        {
                                            PdfDetailCell1 = new PdfPCell(new Phrase(dr["Customer2"].ToString(), fontText));
                                        }
                                        else if (count_Customer < 30)
                                        {
                                            PdfDetailCell1 = new PdfPCell(new Phrase(dr["Customer2"].ToString(), fontText6));
                                        }
                                        else
                                        {
                                            PdfDetailCell1 = new PdfPCell(new Phrase(dr["Customer2"].ToString(), fontText5));
                                        }
                                    }
                                    else
                                    {
                                        PdfDetailCell1 = new PdfPCell(new Phrase(" ", fontText5));
                                    }
                                    //PdfDetailCell1 = new PdfPCell(new Phrase(dr["Customer2"].ToString(), fontAbCutomer));                        
                                    PdfDetailCell1.NoWrap = false;
                                    PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell1.HorizontalAlignment = 1;
                                    PdfDetailCell1.Border = Rectangle.NO_BORDER;
                                    PdfDetailCell1.Colspan = 10;
                                    PdfDetailTable1.AddCell(PdfDetailCell1);

                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 1)
                                    {
                                        if (count_Customer < 10)
                                        {
                                            PdfDetailCell1 = new PdfPCell(new Phrase(dr["Customer3"].ToString(), fontAb11B));
                                        }
                                        else if (count_Customer < 20)
                                        {
                                            PdfDetailCell1 = new PdfPCell(new Phrase(dr["Customer3"].ToString(), fontText));
                                        }
                                        else if (count_Customer < 30)
                                        {
                                            PdfDetailCell1 = new PdfPCell(new Phrase(dr["Customer3"].ToString(), fontText6));
                                        }
                                        else
                                        {
                                            PdfDetailCell1 = new PdfPCell(new Phrase(dr["Customer3"].ToString(), fontText5));
                                        }
                                    }
                                    else
                                    {
                                        PdfDetailCell1 = new PdfPCell(new Phrase(" ", fontText5));
                                    }
                                    //PdfDetailCell1 = new PdfPCell(new Phrase(dr["Customer3"].ToString(), fontAbCutomer));
                                    PdfDetailCell1.NoWrap = false;
                                    PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell1.HorizontalAlignment = 1;
                                    PdfDetailCell1.Border = Rectangle.NO_BORDER;
                                    PdfDetailCell1.Colspan = 6;
                                    PdfDetailTable1.AddCell(PdfDetailCell1);

                                    PdfDetailCell1 = new PdfPCell(new Phrase(" ", fontAb11B));
                                    PdfDetailCell1.NoWrap = false;
                                    PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell1.HorizontalAlignment = 1;
                                    PdfDetailCell1.Border = Rectangle.NO_BORDER;
                                    //PdfDetailCell1.Border = Rectangle.RIGHT_BORDER;//Avinash
                                    PdfDetailCell1.BorderWidthRight = 2f;
                                    PdfDetailCell1.Colspan = 9;
                                    PdfDetailTable1.AddCell(PdfDetailCell1);

                                    /*Avinash[14/01/2020]*/
                                    PdfDetailCell1 = new PdfPCell(new Phrase(""));
                                    PdfDetailCell1.NoWrap = false;
                                    PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell1.HorizontalAlignment = 1;
                                    PdfDetailCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfDetailCell1.BorderWidthLeft = 2f;
                                    PdfDetailCell1.BorderWidthRight = 2f;
                                    PdfDetailCell1.Colspan = 38;
                                    PdfDetailTable1.AddCell(PdfDetailCell1);

                                    PdfDetailCell1 = new PdfPCell(new Phrase(""));
                                    PdfDetailCell1.NoWrap = false;
                                    PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell1.HorizontalAlignment = 1;
                                    PdfDetailCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfDetailCell1.BorderWidthLeft = 2f;
                                    PdfDetailCell1.BorderWidthRight = 2f;
                                    PdfDetailCell1.Colspan = 38;
                                    PdfDetailTable1.AddCell(PdfDetailCell1);


                                    PdfDetailCell1 = new PdfPCell(new Phrase(""));
                                    PdfDetailCell1.NoWrap = false;
                                    PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell1.HorizontalAlignment = 1;
                                    PdfDetailCell1.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfDetailCell1.BorderWidthLeft = 2f;
                                    PdfDetailTable1.AddCell(PdfDetailCell1);

                                    PdfDetailCell1 = new PdfPCell(new Phrase("This is to confirm that declaration has been carefully read, understood & made by me/us. I'm authorizing the user entity/Corporate to debit my account, based on the instruction as agreed and signed by me. I've understood that I'm authorized to cancel/amend this mandate by appropriately communicating the cancellation/amendment request to the user/entity/corporate or the bank where I've authorized the debit.", fontText5));
                                    PdfDetailCell1.NoWrap = false;
                                    PdfDetailCell1.Colspan = 34;
                                    PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_LEFT;
                                    PdfDetailCell1.Border = Rectangle.NO_BORDER;
                                    PdfDetailTable1.AddCell(PdfDetailCell1);


                                    PdfDetailCell1 = new PdfPCell(new Phrase(""));
                                    PdfDetailCell1.Border = Rectangle.NO_BORDER;
                                    PdfDetailCell1.BorderWidthRight = 2f;
                                    PdfDetailTable1.AddCell(PdfDetailCell1);

                                    PdfDetailCell1 = new PdfPCell(new Phrase(" "));
                                    PdfDetailCell1.Colspan = 36;
                                    PdfDetailCell1.Border = Rectangle.NO_BORDER;
                                    PdfDetailCell1.BorderWidthRight = 2f;
                                    PdfDetailCell1.BorderWidthLeft = 2f;
                                    PdfDetailCell1.BorderWidthBottom = 2f;
                                    PdfDetailCell1.FixedHeight = 5f;
                                    PdfDetailTable1.AddCell(PdfDetailCell1);

                                    #endregion

                                    #region Table2
                                    PdfPTable PdfHeaderTable2 = new PdfPTable(33);
                                    PdfHeaderTable2.DefaultCell.NoWrap = false;
                                    PdfHeaderTable2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderTable2.WidthPercentage = 100;
                                    float[] Headerwidths112 = new float[] { 1f, 4f, 3f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                                    //float[] Headerwidths = new float[] { 4f, 3f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                                    PdfHeaderTable2.SetWidths(Headerwidths112);
                                    PdfPCell PdfHeaderCell2 = null;
                                    Document document2 = new Document();
                                    document2.Open();

                                    PdfHeaderCell2 = new PdfPCell(new Phrase(""));
                                    PdfHeaderCell2.Colspan = 33;
                                    PdfHeaderCell2.Border = Rectangle.NO_BORDER;
                                    PdfHeaderCell2.BorderWidthTop = 2f;
                                    PdfHeaderCell2.BorderWidthLeft = 2f;
                                    PdfHeaderCell2.BorderWidthRight = 2f;
                                    PdfHeaderTable2.AddCell(PdfHeaderCell2);

                                    //PdfHeaderCell2 = new PdfPCell(new Phrase(""));
                                    //PdfHeaderCell2.Colspan = 33;
                                    //PdfHeaderCell2.Border = Rectangle.NO_BORDER;
                                    //PdfHeaderCell2.BorderWidthLeft = 2f;
                                    //PdfHeaderCell2.BorderWidthRight = 2f;
                                    //PdfHeaderTable2.AddCell(PdfHeaderCell2);

                                    PdfHeaderCell2 = new PdfPCell(new Phrase(" "));
                                    PdfHeaderCell2.FixedHeight = 35f;
                                    PdfHeaderCell2.Rowspan = 3;
                                    PdfHeaderCell2.Border = Rectangle.NO_BORDER;//Avinash                        
                                    PdfHeaderCell2.BorderWidthLeft = 2f;
                                    PdfHeaderTable2.AddCell(PdfHeaderCell2);


                                    PdfHeaderCell2 = new PdfPCell(LogoImage);
                                    PdfHeaderCell2.FixedHeight = 50f;
                                    PdfHeaderCell2.Rowspan = 3;
                                    PdfHeaderCell2.Border = Rectangle.NO_BORDER;
                                    PdfHeaderTable2.AddCell(PdfHeaderCell2);

                                    PdfHeaderCell2 = new PdfPCell(new Phrase("UMRN", fontAb11B));
                                    PdfHeaderCell2.NoWrap = false;
                                    PdfHeaderCell2.BackgroundColor = new Color(252, 252, 252);
                                    PdfHeaderCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    //PdfHeaderCell2.FixedHeight = 20f;
                                    PdfHeaderCell2.HorizontalAlignment = 1;
                                    //PdfHeaderCell2.Border = Rectangle.TOP_BORDER;
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

                                    //-------------------------------Add Date-------------------------------

                                    PdfHeaderCell2 = new PdfPCell(new Phrase("Date", fontAb11B));
                                    PdfHeaderCell2.NoWrap = false;
                                    PdfHeaderCell2.BackgroundColor = new Color(252, 252, 252);
                                    PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderCell2.HorizontalAlignment = 1;
                                    PdfHeaderCell2.Border = Rectangle.NO_BORDER;//Avinash
                                                                                //PdfHeaderCell2.Border = Rectangle.TOP_BORDER;
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

                                    PdfHeaderCell2 = new PdfPCell(new Phrase(" "));
                                    PdfHeaderCell2.NoWrap = false;
                                    PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderCell2.HorizontalAlignment = 1;
                                    PdfHeaderCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfHeaderCell2.BorderWidthRight = 2f;
                                    PdfHeaderTable2.AddCell(PdfHeaderCell2);

                                    //----------------------------------------Add Sponsor bankcode---------------------
                                    string SpBankCode2 = dr["SponserBankCode"].ToString();
                                    PdfHeaderCell2 = new PdfPCell(new Phrase("Sponsor bankcode", fontAb11B));
                                    PdfHeaderCell2.NoWrap = false;
                                    PdfHeaderCell2.BackgroundColor = new Color(252, 252, 252);
                                    PdfHeaderCell2.Colspan = 4;
                                    PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderCell2.HorizontalAlignment = 1;
                                    PdfHeaderCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfHeaderCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfHeaderCell2.FixedHeight = 15f;
                                    PdfHeaderTable2.AddCell(PdfHeaderCell2);

                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 2)
                                    {
                                        PdfHeaderCell2 = new PdfPCell(new Phrase(dr["SponserBankCode"].ToString(), fontAb11B));
                                    }
                                    else
                                    {
                                        PdfHeaderCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                                    }
                                    PdfHeaderCell2.NoWrap = false;
                                    PdfHeaderCell2.Colspan = 12;
                                    PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderCell2.HorizontalAlignment = 1;
                                    PdfHeaderCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfHeaderTable2.AddCell(PdfHeaderCell2);


                                    PdfHeaderCell2 = new PdfPCell(new Phrase("Utility Code", fontAb11B));
                                    PdfHeaderCell2.NoWrap = false;
                                    PdfHeaderCell2.Colspan = 6;
                                    PdfHeaderCell2.BackgroundColor = new Color(252, 252, 252);
                                    PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderCell2.HorizontalAlignment = 1;
                                    PdfHeaderCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfHeaderCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfHeaderTable2.AddCell(PdfHeaderCell2);

                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 2)
                                    {
                                        PdfHeaderCell2 = new PdfPCell(new Phrase(dr["UtilityCode"].ToString(), fontAb11B));
                                    }
                                    else
                                    {
                                        PdfHeaderCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                                    }
                                    PdfHeaderCell2.NoWrap = false;
                                    PdfHeaderCell2.Colspan = 8;
                                    PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                                    PdfHeaderCell2.HorizontalAlignment = 1;
                                    PdfHeaderCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfHeaderTable2.AddCell(PdfHeaderCell2);


                                    PdfHeaderCell2 = new PdfPCell(new Phrase(" "));
                                    PdfHeaderCell2.NoWrap = false;
                                    PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderCell2.HorizontalAlignment = 1;
                                    PdfHeaderCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfHeaderCell2.BorderWidthRight = 2f;
                                    PdfHeaderTable2.AddCell(PdfHeaderCell2);


                                    ////------------------------------- add Created Status-------------------------------------
                                    Document documentCheckBox2 = new Document();
                                    PdfHeaderCell2 = new PdfPCell(new Phrase("I/We hereby Authorize", fontAb11B));
                                    PdfHeaderCell2.NoWrap = false;
                                    PdfHeaderCell2.BackgroundColor = new Color(252, 252, 252);
                                    PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderCell2.Colspan = 5;
                                    PdfHeaderCell2.FixedHeight = 15f;
                                    PdfHeaderCell2.HorizontalAlignment = 1;
                                    PdfHeaderCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfHeaderCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfHeaderTable2.AddCell(PdfHeaderCell2);


                                    //PdfHeaderCell2 = new PdfPCell(new Phrase(dr["CompanyName"].ToString(), fontAb11B));
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 2)
                                    {
                                        if (count_IherebyAuthorize < 34)
                                        {
                                            PdfHeaderCell2 = new PdfPCell(new Phrase(dr["CompanyName"].ToString(), fontAb11B));
                                        }
                                        else if (count_IherebyAuthorize < 40)
                                        {
                                            PdfHeaderCell2 = new PdfPCell(new Phrase(dr["CompanyName"].ToString(), fontText));
                                        }
                                        else if (count_IherebyAuthorize < 48)
                                        {
                                            PdfHeaderCell2 = new PdfPCell(new Phrase(dr["CompanyName"].ToString(), fontText6));
                                        }
                                        else
                                        {
                                            PdfHeaderCell2 = new PdfPCell(new Phrase(dr["CompanyName"].ToString(), fontText5));
                                        }
                                    }
                                    else
                                    {
                                        PdfHeaderCell2 = new PdfPCell(new Phrase(" ", fontText5));
                                    }

                                    PdfHeaderCell2.NoWrap = false;
                                    PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderCell2.Colspan = 10;
                                    //PdfHeaderCell2.FixedHeight = 20f;
                                    PdfHeaderCell2.HorizontalAlignment = 1;
                                    PdfHeaderCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfHeaderTable2.AddCell(PdfHeaderCell2);

                                    PdfHeaderCell2 = new PdfPCell(new Phrase("To Debit", fontAb11B));
                                    PdfHeaderCell2.NoWrap = false;
                                    PdfHeaderCell2.BackgroundColor = new Color(252, 252, 252);
                                    PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderCell2.Colspan = 3;
                                    PdfHeaderCell2.HorizontalAlignment = 1;
                                    PdfHeaderCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfHeaderCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfHeaderTable2.AddCell(PdfHeaderCell2);

                                    Document documentCheckBox1SB2 = new Document();
                                    documentCheckBox1SB2.Open();
                                    Paragraph pCheckBox1SB2 = new Paragraph();
                                    //----------------------------------add To Debit---------------------------
                                    string chDebit2 = dr["DebitTo"].ToString();
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 2)
                                    {
                                        if (chDebit2 == "SB")
                                        {
                                            //  pCheckBox1SB2.Add(new Phrase(" ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" SB/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" CA/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" CC/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" SB-NRE/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" SB-NRO/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" OTHER ", fontText));
                                        }
                                        else if (chDebit2 == "CA")
                                        {
                                            // pCheckBox1SB2.Add(new Phrase(" ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" SB/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" CA/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" CC/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" SB-NRE/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" SB-NRO/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" OTHER ", fontText));
                                        }

                                        else if (chDebit2 == "CC")
                                        {
                                            //  pCheckBox1SB2.Add(new Phrase(" ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" SB/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" CA/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" CC/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" SB-NRE/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" SB-NRO/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" OTHER ", fontText));
                                        }
                                        else if (chDebit2 == "RE")
                                        {
                                            // pCheckBox1SB2.Add(new Phrase(" ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" SB/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" CA/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" CC/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" SB-NRE/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" SB-NRO/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" OTHER ", fontText));
                                        }
                                        else if (chDebit2 == "RD")
                                        {
                                            // pCheckBox1SB2.Add(new Phrase(" ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" SB/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" CA/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" CC/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" SB-NRE/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" SB-NRO/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" OTHER ", fontText));
                                        }
                                        else if (chDebit2 == "OT")
                                        {
                                            // pCheckBox1SB2.Add(new Phrase(" ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" SB/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" CA/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" CC/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" SB-NRE/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" SB-NRO/ ", fontText));
                                            pCheckBox1SB2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox1SB2.Add(new Phrase(" OTHER ", fontText));
                                        }
                                    }
                                    else
                                    {
                                        pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pCheckBox1SB2.Add(new Phrase(" SB/ ", fontText));
                                        pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pCheckBox1SB2.Add(new Phrase(" CA/ ", fontText));
                                        pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pCheckBox1SB2.Add(new Phrase(" CC/ ", fontText));
                                        pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pCheckBox1SB2.Add(new Phrase(" SB-NRE/ ", fontText));
                                        pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pCheckBox1SB2.Add(new Phrase(" SB-NRO/ ", fontText));
                                        pCheckBox1SB2.Add(new Chunk(SmallcheckBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pCheckBox1SB2.Add(new Phrase(" OTHER ", fontText));
                                    }
                                    PdfHeaderCell2 = new PdfPCell(pCheckBox1SB2);
                                    PdfHeaderCell2.NoWrap = false;
                                    PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    //PdfHeaderCell2.Colspan = 11;
                                    PdfHeaderCell2.Colspan = 12;
                                    PdfHeaderCell2.HorizontalAlignment = 1;
                                    PdfHeaderCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfHeaderCell2.Border = Rectangle.NO_BORDER;//Avinash                        
                                    PdfHeaderTable2.AddCell(PdfHeaderCell2);

                                    PdfHeaderCell2 = new PdfPCell(new Phrase(" "));
                                    PdfHeaderCell2.NoWrap = false;
                                    PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfHeaderCell2.HorizontalAlignment = 1;
                                    PdfHeaderCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfHeaderCell2.BorderWidthRight = 2f;
                                    PdfHeaderTable2.AddCell(PdfHeaderCell2);

                                    PdfPTable PdfMidTable2 = new PdfPTable(34);
                                    PdfMidTable2.DefaultCell.NoWrap = false;
                                    PdfMidTable2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidTable2.DefaultCell.Border = PdfCell.NO_BORDER;
                                    PdfMidTable2.WidthPercentage = 100;
                                    float[] PdfMidTable2Headerwidths1122 = new float[] { 1f, 4f, 3f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                                    //float[] PdfMidTable2Headerwidths = new float[] { 4f, 3f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                                    PdfMidTable2.SetWidths(PdfMidTable2Headerwidths1122);

                                    PdfPCell PdfMidCell2 = null;
                                    PdfMidCell2 = new PdfPCell(new Phrase(" "));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell2.BorderWidthLeft = 2f;
                                    PdfMidTable2.AddCell(PdfMidCell2);


                                    documentCheckBox2.Open();
                                    Paragraph pCheckBox2 = new Paragraph();
                                    string status_2 = dr["CreatedStatus"].ToString();
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 2)
                                    {
                                        if (status_2 == "C")
                                        {
                                            pCheckBox2.Add(new Phrase("CREATE ", fontText));
                                            pCheckBox2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox2.Add(new Phrase(" MODIFY ", fontText));
                                            pCheckBox2.Add(new Phrase(" CANCEL ", fontText));

                                        }
                                        else if (status_2 == "M")
                                        {
                                            pCheckBox2.Add(new Phrase("CREATE ", fontText));
                                            pCheckBox2.Add(new Phrase(" MODIFY ", fontText));
                                            pCheckBox2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pCheckBox2.Add(new Phrase(" CANCEL ", fontText));

                                        }
                                        else if (status_2 == "L")
                                        {
                                            pCheckBox2.Add(new Phrase("CREATE ", fontText));
                                            pCheckBox2.Add(new Phrase(" MODIFY ", fontText));
                                            pCheckBox2.Add(new Phrase(" CANCEL ", fontText));
                                            pCheckBox2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        }
                                    }
                                    else
                                    {
                                        pCheckBox2.Add(new Phrase("CREATE ", fontText));
                                        pCheckBox2.Add(new Phrase(" MODIFY ", fontText));
                                        pCheckBox2.Add(new Phrase(" CANCEL ", fontText));
                                    }
                                    PdfMidCell2 = new PdfPCell(pCheckBox2);
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.Border = Rectangle.NO_BORDER;
                                    PdfMidTable2.AddCell(PdfMidCell2);


                                    //----------------------------------Add AccountNo.-------------------------------------------------
                                    PdfMidCell2 = new PdfPCell(new Phrase("Bank a/c Number", fontAb11B));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.Colspan = 5;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfMidCell2.Border = Rectangle.NO_BORDER;
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    string AccountNo2 = dr["AccountNo"].ToString();
                                    char[] chrAcountNo2 = new char[Convert.ToInt32(AccountNo2.Length)];
                                    chrAcountNo2 = AccountNo2.ToCharArray();
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 2)
                                    {
                                        if (Convert.ToInt32(AccountNo2.Length) <= 26)
                                        {
                                            for (int j = 0; j < Convert.ToInt32(chrAcountNo2.Length); j++)
                                            {
                                                PdfMidCell2 = new PdfPCell(new Phrase(chrAcountNo2[j].ToString(), fontA119B));
                                                PdfMidCell2.NoWrap = false;
                                                PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                                PdfMidCell2.HorizontalAlignment = 1;
                                                PdfMidCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                                PdfMidTable2.AddCell(PdfMidCell2);
                                            }

                                            int len = 26 - Convert.ToInt32(AccountNo2.Length);
                                            for (int k = 0; k < len; k++)
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
                                        if (Convert.ToInt32(AccountNo2.Length) <= 26)
                                        {
                                            for (int j = 0; j < Convert.ToInt32(chrAcountNo2.Length); j++)
                                            {
                                                PdfMidCell2 = new PdfPCell(new Phrase(" ", fontA119B));
                                                PdfMidCell2.NoWrap = false;
                                                PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                                PdfMidCell2.HorizontalAlignment = 1;
                                                PdfMidCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                                PdfMidTable2.AddCell(PdfMidCell2);
                                            }

                                            int len = 26 - Convert.ToInt32(AccountNo2.Length);
                                            for (int k = 0; k < len; k++)
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

                                    PdfMidCell2 = new PdfPCell(new Phrase(" "));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell2.BorderWidthRight = 2f;
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    PdfMidCell2 = new PdfPCell(new Phrase(" "));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell2.BorderWidthLeft = 2f;
                                    PdfMidTable2.AddCell(PdfMidCell2);


                                    PdfMidCell2 = new PdfPCell(new Phrase("With Bank", fontAb11B));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.FixedHeight = 25f;
                                    PdfMidCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfMidCell2.Border = Rectangle.NO_BORDER;//Avinash
                                                                             //PdfMidCell2.Border = Rectangle.LEFT_BORDER;
                                                                             //PdfMidCell2.BorderWidth = 2f;
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    //PdfMidCell2 = new PdfPCell(new Phrase(dr["BankName"].ToString(), fontAb11));
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 2)
                                    {
                                        if (count_BankName < 34)
                                        {
                                            PdfMidCell2 = new PdfPCell(new Phrase(dr["BankName"].ToString(), fontAb11B));
                                        }
                                        else if (count_BankName < 40)
                                        {
                                            PdfMidCell2 = new PdfPCell(new Phrase(dr["BankName"].ToString(), fontText));
                                        }
                                        else if (count_BankName < 48)
                                        {
                                            PdfMidCell2 = new PdfPCell(new Phrase(dr["BankName"].ToString(), fontText6));
                                        }
                                        else
                                        {
                                            PdfMidCell2 = new PdfPCell(new Phrase(dr["BankName"].ToString(), fontText5));
                                        }
                                    }
                                    else
                                    {
                                        PdfMidCell2 = new PdfPCell(new Phrase(" ", fontText5));
                                    }
                                    PdfMidCell2.NoWrap = false;
                                    //PdfMidCell2.FixedHeight = 30f;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.Colspan = 6;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfMidTable2.AddCell(PdfMidCell2);


                                    PdfMidCell2 = new PdfPCell(new Phrase("IFSC", fontAb11B));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.Colspan = 2;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfMidCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    //-------------------------Add IFSC code--------------------------------
                                    string IFSCcode2 = dr["IFSCcode"].ToString();
                                    char[] chrIFSCcode2 = new char[Convert.ToInt32(IFSCcode2.Length)];
                                    chrIFSCcode2 = IFSCcode2.ToCharArray();
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 2)
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
                                        if (Convert.ToInt32(chrIFSCcode2.Length) == 11)
                                        {
                                            for (int j = 0; j < Convert.ToInt32(chrIFSCcode2.Length); j++)
                                            {
                                                PdfMidCell2 = new PdfPCell(new Phrase(" ", fontA119B));
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
                                    PdfMidCell2 = new PdfPCell(new Phrase("or MICR", fontAb11B));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.Colspan = 3;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                    PdfMidCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    //-------------------------Add MICRcode--------------------------------
                                    string MICRcode2 = dr["MICRcode"].ToString();
                                    char[] chrMICRcode2 = new char[9];
                                    chrMICRcode2 = MICRcode2.ToCharArray();

                                    if (Convert.ToInt32(chrMICRcode2.Length) == 9)
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

                                    PdfMidCell2 = new PdfPCell(new Phrase(" "));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell2.BorderWidthRight = 2f;
                                    PdfMidTable2.AddCell(PdfMidCell2);
                                    //-----------------------------------Add amount of Rupees---------------------------

                                    PdfMidCell2 = new PdfPCell(new Phrase(" "));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell2.BorderWidthLeft = 2f;
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    PdfMidCell2 = new PdfPCell(new Phrase("an amount of Rupees", fontAb11B));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.Colspan = 2;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.Border = Rectangle.NO_BORDER;//Avinash                        
                                                                             //PdfMidCell2.Border = Rectangle.LEFT_BORDER;
                                                                             //PdfMidCell2.BorderWidth = 2f;
                                    PdfMidCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_LEFT;
                                    PdfMidTable2.AddCell(PdfMidCell2);
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 2)
                                    {
                                        PdfMidCell2 = new PdfPCell(new Phrase(dr["AmountInWord"].ToString(), fontA11B));
                                    }
                                    else
                                    {
                                        PdfMidCell2 = new PdfPCell(new Phrase(" ", fontA11B));
                                    }
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.Colspan = 22;
                                    PdfMidCell2.FixedHeight = 10f;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    Document documentAmountInDigit2 = new Document();
                                    documentAmountInDigit2.Open();
                                    Paragraph pAmountInDigit2 = new Paragraph();
                                    pAmountInDigit2.Add(new Chunk(Rupee, PdfPCell.ALIGN_CENTER, PdfPCell.ALIGN_CENTER));
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 2)
                                    {
                                        pAmountInDigit2.Add(new Phrase(" " + dr["AmountInDigit"].ToString(), fontA119B));
                                    }
                                    else
                                    {
                                        pAmountInDigit2.Add(new Phrase(" " + " ", fontA119B));
                                    }
                                    PdfMidCell2 = new PdfPCell(pAmountInDigit2);
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    //PdfMidCell2.Colspan = 10;
                                    PdfMidCell2.Colspan = 8;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    PdfMidCell2 = new PdfPCell(new Phrase(" "));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell2.BorderWidthRight = 2f;
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    //---------------------------------Add Frequency--------------------------------------
                                    PdfMidCell2 = new PdfPCell(new Phrase(" "));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell2.BorderWidthLeft = 2f;
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    string Freq2 = dr["Frequency"].ToString();
                                    PdfMidCell2 = new PdfPCell(new Phrase("Frequency", fontText));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.Border = Rectangle.NO_BORDER;
                                    PdfMidCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_LEFT;
                                    PdfMidTable2.AddCell(PdfMidCell2);


                                    Document documentMonthly2 = new Document();
                                    documentMonthly2.Open();
                                    Paragraph pMonthly2 = new Paragraph();
                                    //------------------------------- add Monthly-------------------------------------
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 2)
                                    {
                                        if (Freq2 == "M")
                                        {
                                            pMonthly2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pMonthly2.Add(new Phrase("  Monthly", fontText));
                                        }
                                        else
                                        {
                                            pMonthly2.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pMonthly2.Add(new Phrase("  Monthly", fontText));
                                        }
                                    }
                                    else
                                    {
                                        pMonthly2.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pMonthly2.Add(new Phrase("  Monthly", fontText));
                                    }

                                    PdfMidCell2 = new PdfPCell(pMonthly2);
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.Colspan = 2;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    Document documentQtly2 = new Document();
                                    documentQtly2.Open();
                                    Paragraph pQtly2 = new Paragraph();
                                    //------------------------------- add Qtly-------------------------------------
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 2)
                                    {
                                        if (Freq2 == "Q")
                                        {
                                            pQtly2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pQtly2.Add(new Phrase(" Qtly", fontText));
                                        }
                                        else
                                        {
                                            pQtly2.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pQtly2.Add(new Phrase(" Qtly", fontText));
                                        }
                                    }
                                    else
                                    {
                                        pQtly2.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pQtly2.Add(new Phrase(" Qtly", fontText));
                                    }

                                    PdfMidCell2 = new PdfPCell(pQtly2);
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.Colspan = 2;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    Document documentHYrly2 = new Document();
                                    documentHYrly2.Open();
                                    Paragraph pHYrly2 = new Paragraph();
                                    //------------------------------- add H-Yrly-------------------------------------
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 2)
                                    {
                                        if (Freq2 == "H")
                                        {
                                            pHYrly2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pHYrly2.Add(new Phrase("  H-Yrly", fontText));
                                        }
                                        else
                                        {
                                            pHYrly2.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pHYrly2.Add(new Phrase("  H-Yrly", fontText));
                                        }
                                    }
                                    else
                                    {
                                        pHYrly2.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pHYrly2.Add(new Phrase("  H-Yrly", fontText));
                                    }

                                    PdfMidCell2 = new PdfPCell(pHYrly2);
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.Colspan = 3;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    Document documentYearly2 = new Document();
                                    documentYearly2.Open();
                                    Paragraph pYearly2 = new Paragraph();
                                    //------------------------------- add Yearly-------------------------------------
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 2)
                                    {
                                        if (Freq2 == "Y")
                                        {
                                            pYearly2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pYearly2.Add(new Phrase("  Yearly", fontText));
                                        }
                                        else
                                        {
                                            pYearly2.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pYearly2.Add(new Phrase("  Yearly", fontText));
                                        }

                                    }
                                    else
                                    {
                                        pYearly2.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pYearly2.Add(new Phrase("  Yearly", fontText));
                                    }
                                    PdfMidCell2 = new PdfPCell(pYearly2);
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.Colspan = 3;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    Document prensented1prensented12 = new Document();
                                    prensented1prensented12.Open();
                                    Paragraph prensented2 = new Paragraph();
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 2)
                                    {
                                        if (Freq2 == "A")
                                        {
                                            prensented2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            prensented2.Add(new Phrase("  As & when prensented", fontText));
                                        }
                                        else
                                        {
                                            prensented2.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            prensented2.Add(new Phrase("  As & when prensented", fontText));
                                        }
                                    }
                                    else
                                    {
                                        prensented2.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        prensented2.Add(new Phrase("  As & when prensented", fontText));
                                    }

                                    PdfMidCell2 = new PdfPCell(prensented2);
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.Colspan = 7;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    string DebitType2 = dr["DebitType"].ToString();

                                    PdfMidCell2 = new PdfPCell(new Phrase("Debit Type", fontText));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.Colspan = 3;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_LEFT;
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    Document documentFixed2 = new Document();
                                    documentFixed2.Open();
                                    Paragraph pFixed2 = new Paragraph();
                                    //------------------------------- add H-Yrly-------------------------------------
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 2)
                                    {
                                        if (DebitType2 == "F")
                                        {
                                            pFixed2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pFixed2.Add(new Phrase("  Fixed Amount", fontText));
                                        }
                                        else
                                        {
                                            pFixed2.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pFixed2.Add(new Phrase("  Fixed Amount", fontText));
                                        }
                                    }
                                    else
                                    {
                                        pFixed2.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pFixed2.Add(new Phrase("  Fixed Amount", fontText));
                                    }
                                    PdfMidCell2 = new PdfPCell(pFixed2);
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.Colspan = 5;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    Document documentMaximum2 = new Document();
                                    documentMaximum2.Open();
                                    Paragraph pMaximum2 = new Paragraph();
                                    //------------------------------- add H-Yrly-------------------------------------
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 2)
                                    {
                                        if (DebitType2 == "M")
                                        {
                                            pMaximum2.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pMaximum2.Add(new Phrase("  Maximum Amount", fontText));
                                        }
                                        else
                                        {
                                            pMaximum2.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                            pMaximum2.Add(new Phrase("  Maximum Amount", fontText));
                                        }
                                    }
                                    else
                                    {
                                        pMaximum2.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        pMaximum2.Add(new Phrase("  Maximum Amount", fontText));
                                    }

                                    PdfMidCell2 = new PdfPCell(pMaximum2);
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.Colspan = 6;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidTable2.AddCell(PdfMidCell2);


                                    PdfMidCell2 = new PdfPCell(new Phrase(" "));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell2.BorderWidthRight = 2f;
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    PdfMidCell2 = new PdfPCell(new Phrase(" "));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell2.BorderWidthLeft = 2f;
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    PdfMidCell2 = new PdfPCell(new Phrase("Reference 1", fontAb11B));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.Border = Rectangle.NO_BORDER;//Avinash
                                                                             //PdfMidCell2.Border = Rectangle.LEFT_BORDER;
                                                                             //PdfMidCell2.BorderWidth = 2f;
                                    PdfMidCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_LEFT;
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    //PdfMidCell2 = new PdfPCell(new Phrase(dr["Reference1"].ToString(), fontAb11B));
                                    if (count_Ref1 < 34)
                                    {
                                        PdfMidCell2 = new PdfPCell(new Phrase(dr["Reference1"].ToString(), fontAb11B));
                                    }
                                    else if (count_Ref1 < 40)
                                    {
                                        PdfMidCell2 = new PdfPCell(new Phrase(dr["Reference1"].ToString(), fontText));
                                    }
                                    else if (count_Ref1 < 48)
                                    {
                                        PdfMidCell2 = new PdfPCell(new Phrase(dr["Reference1"].ToString(), fontText6));
                                    }
                                    else
                                    {
                                        PdfMidCell2 = new PdfPCell(new Phrase(dr["Reference1"].ToString(), fontText5));
                                    }
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.Colspan = 15;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidTable2.AddCell(PdfMidCell2);


                                    PdfMidCell2 = new PdfPCell(new Phrase("Phone Number ", fontAb11B));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.Colspan = 6;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 2)
                                    {
                                        PdfMidCell2 = new PdfPCell(new Phrase(dr["PhoneNo"].ToString(), fontAb11B));
                                    }
                                    else
                                    {
                                        PdfMidCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                                    }
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.Colspan = 10;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    PdfMidCell2 = new PdfPCell(new Phrase(" "));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell2.BorderWidthRight = 2f;
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    PdfMidCell2 = new PdfPCell(new Phrase(" "));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell2.BorderWidthLeft = 2f;
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    PdfMidCell2 = new PdfPCell(new Phrase("Reference 2", fontAb11B));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.Border = Rectangle.NO_BORDER;
                                    PdfMidCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_LEFT;
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    //PdfMidCell2 = new PdfPCell(new Phrase(dr["Reference2"].ToString(), fontAb11B));
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 2)
                                    {
                                        if (count_Ref2 < 34)
                                        {
                                            PdfMidCell2 = new PdfPCell(new Phrase(dr["Reference2"].ToString(), fontAb11B));
                                        }
                                        else if (count_Ref2 < 40)
                                        {
                                            PdfMidCell2 = new PdfPCell(new Phrase(dr["Reference2"].ToString(), fontText));
                                        }
                                        else if (count_Ref2 < 48)
                                        {
                                            PdfMidCell2 = new PdfPCell(new Phrase(dr["Reference2"].ToString(), fontText6));
                                        }
                                        else
                                        {
                                            PdfMidCell2 = new PdfPCell(new Phrase(dr["Reference2"].ToString(), fontText5));
                                        }
                                    }
                                    else
                                    {
                                        PdfMidCell2 = new PdfPCell(new Phrase(" ", fontText5));
                                    }
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.Colspan = 15;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidTable2.AddCell(PdfMidCell2);


                                    PdfMidCell2 = new PdfPCell(new Phrase("EMail ID", fontAb11B));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.Colspan = 6;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    //PdfMidCell2 = new PdfPCell(new Phrase(dr["EmailId"].ToString(), fontAb11B));
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 2)
                                    {
                                        if (count < 34)
                                        {
                                            PdfMidCell2 = new PdfPCell(new Phrase(dr["EmailId"].ToString(), fontAb11B));
                                        }
                                        //else if (count < 40)
                                        //{
                                        //    PdfMidCell2 = new PdfPCell(new Phrase(dr["EmailId"].ToString(), fontText));
                                        //}
                                        //else if (count < 48)
                                        //{
                                        //    PdfMidCell2 = new PdfPCell(new Phrase(dr["EmailId"].ToString(), fontText6));
                                        //}
                                        else
                                        {
                                            PdfMidCell2 = new PdfPCell(new Phrase(dr["EmailId"].ToString(), fontText5));
                                        }
                                    }
                                    else
                                    {
                                        PdfMidCell2 = new PdfPCell(new Phrase(" ", fontText5));
                                    }

                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.Colspan = 10;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    PdfMidCell2 = new PdfPCell(new Phrase(" "));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell2.BorderWidthRight = 2f;
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    PdfMidCell2 = new PdfPCell(new Phrase(" "));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell2.BorderWidthLeft = 2f;
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    PdfMidCell2 = new PdfPCell(new Phrase("PERIOD", fontText));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.FixedHeight = 8f;
                                    PdfMidCell2.BackgroundColor = new Color(252, 252, 252);
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidTable2.AddCell(PdfMidCell2);//

                                    PdfMidCell2 = new PdfPCell(new Phrase("I agree for the debit of mandate processing charges by the bank whom I am authorizing to debit my account as per latest schedule of charges of bank ", fontText6));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.Border = Rectangle.NO_BORDER;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.Colspan = 31;//Avinash
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidTable2.AddCell(PdfMidCell2);


                                    PdfMidCell2 = new PdfPCell(new Phrase(" "));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfMidCell2.BorderWidthRight = 2f;
                                    PdfMidTable2.AddCell(PdfMidCell2);

                                    PdfMidCell2 = new PdfPCell(new Phrase("", fontAb11B));
                                    PdfMidCell2.NoWrap = false;
                                    PdfMidCell2.Border = Rectangle.NO_BORDER;
                                    PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfMidCell2.Colspan = 31;
                                    PdfMidCell2.HorizontalAlignment = 1;
                                    PdfMidTable2.AddCell(PdfMidCell2);


                                    PdfPTable PdfDetailTable2 = new PdfPTable(36);
                                    PdfDetailTable2.DefaultCell.NoWrap = false;
                                    PdfDetailTable2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailTable2.DefaultCell.Border = PdfCell.NO_BORDER;
                                    PdfDetailTable2.WidthPercentage = 100;
                                    //float[] Headerwidths1 = new float[] { 4f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                                    float[] Headerwidths1_2 = new float[] { 1f, 4f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
                                    PdfDetailTable2.SetWidths(Headerwidths1_2);
                                    PdfPCell PdfDetailCell2 = null;


                                    PdfDetailCell2 = new PdfPCell(new Phrase(""));
                                    PdfDetailCell2.NoWrap = false;
                                    PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell2.HorizontalAlignment = 1;
                                    PdfDetailCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfDetailCell2.BorderWidthLeft = 2f;
                                    PdfDetailTable2.AddCell(PdfDetailCell2);


                                    PdfDetailCell2 = new PdfPCell(new Phrase("From", fontAb11B));
                                    PdfDetailCell2.NoWrap = false;
                                    //PdfDetailCell2.BackgroundColor = new Color(252, 252, 252);
                                    PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell2.HorizontalAlignment = 1;
                                    //PdfDetailCell2.BorderWidthLeft = 2f;
                                    //PdfDetailCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    //PdfDetailCell2.Border = Rectangle.LEFT_BORDER;
                                    PdfDetailTable2.AddCell(PdfDetailCell2);

                                    string PeriodFrom2 = dr["PeriodFrom"].ToString();
                                    char[] chrPeriodFrom2 = new char[8];
                                    chrPeriodFrom2 = PeriodFrom2.ToCharArray();
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 2)
                                    {
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
                                    }
                                    else
                                    {
                                        if (Convert.ToInt32(chrPeriodFrom2.Length) > 0)
                                        {
                                            for (int j = 0; j < Convert.ToInt32(chrPeriodFrom2.Length); j++)
                                            {
                                                PdfDetailCell2 = new PdfPCell(new Phrase(" ", fontA119B));
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
                                    }
                                    PdfDetailCell2 = new PdfPCell(new Phrase(" ", fontAb11));
                                    PdfDetailCell2.NoWrap = false;
                                    PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell2.HorizontalAlignment = 1;
                                    PdfDetailCell2.Border = Rectangle.NO_BORDER;
                                    //PdfDetailCell2.Border = Rectangle.RIGHT_BORDER;
                                    PdfDetailCell2.Colspan = 25;
                                    PdfDetailTable2.AddCell(PdfDetailCell2);

                                    PdfDetailCell2 = new PdfPCell(new Phrase(""));
                                    PdfDetailCell2.NoWrap = false;
                                    PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell2.HorizontalAlignment = 1;
                                    PdfDetailCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfDetailCell2.BorderWidthRight = 2f;
                                    PdfDetailTable2.AddCell(PdfDetailCell2);


                                    PdfDetailCell2 = new PdfPCell(new Phrase(""));
                                    PdfDetailCell2.NoWrap = false;
                                    PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell2.HorizontalAlignment = 1;
                                    PdfDetailCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfDetailCell2.BorderWidthLeft = 2f;
                                    PdfDetailTable2.AddCell(PdfDetailCell2);

                                    PdfDetailCell2 = new PdfPCell(new Phrase("To*", fontAb11B));
                                    PdfDetailCell2.NoWrap = false;
                                    PdfDetailCell2.BackgroundColor = new Color(252, 252, 252);
                                    PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell2.HorizontalAlignment = 1;
                                    //PdfDetailCell2.BorderWidthLeft = 2f;
                                    //PdfDetailCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    //PdfDetailCell2.Border = Rectangle.LEFT_BORDER;
                                    PdfDetailTable2.AddCell(PdfDetailCell2);

                                    string PeriodTo2 = dr["PeriodTo"].ToString();
                                    char[] chrPeriodTo2 = new char[8];
                                    chrPeriodTo2 = PeriodTo2.ToCharArray();
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 2)
                                    {
                                        if (Convert.ToInt32(chrPeriodTo2.Length) > 0)
                                        {
                                            if (dr["PeriodTo"].ToString() != "01011900")
                                            {
                                                for (int j = 0; j < Convert.ToInt32(chrPeriodTo2.Length); j++)
                                                {
                                                    PdfDetailCell2 = new PdfPCell(new Phrase(chrPeriodTo2[j].ToString(), fontA119B));
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
                                                    //PdfDetailCell2.BackgroundColor = new Color(0,0,0);
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
                                                PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                                PdfDetailCell2.HorizontalAlignment = 1;
                                                //PdfDetailCell2.BackgroundColor = new Color(0, 0, 0);
                                                PdfDetailTable2.AddCell(PdfDetailCell2);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (Convert.ToInt32(chrPeriodTo2.Length) > 0)
                                        {
                                            if (dr["PeriodTo"].ToString() != "01011900")
                                            {
                                                for (int j = 0; j < Convert.ToInt32(chrPeriodTo2.Length); j++)
                                                {
                                                    PdfDetailCell2 = new PdfPCell(new Phrase(" ", fontA119B));
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
                                                    //PdfDetailCell2.BackgroundColor = new Color(0,0,0);
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
                                                PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                                PdfDetailCell2.HorizontalAlignment = 1;
                                                //PdfDetailCell2.BackgroundColor = new Color(0, 0, 0);
                                                PdfDetailTable2.AddCell(PdfDetailCell2);
                                            }
                                        }
                                    }

                                    /*Avinash*/
                                    PdfDetailCell2 = new PdfPCell(new Phrase("Sign. Primary Acc. Holder", fontAb11BU));
                                    PdfDetailCell2.NoWrap = false;
                                    PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell2.HorizontalAlignment = 1;
                                    PdfDetailCell2.Border = Rectangle.NO_BORDER;
                                    PdfDetailCell2.Colspan = 8;
                                    PdfDetailTable2.AddCell(PdfDetailCell2);

                                    PdfDetailCell2 = new PdfPCell(new Phrase("Sign Acc. Holder", fontAb11BU));
                                    PdfDetailCell2.NoWrap = false;
                                    PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell2.HorizontalAlignment = 1;
                                    PdfDetailCell2.Border = Rectangle.NO_BORDER;
                                    PdfDetailCell2.Colspan = 8;
                                    PdfDetailTable2.AddCell(PdfDetailCell2);

                                    PdfDetailCell2 = new PdfPCell(new Phrase("Sign Acc. Holder", fontAb11BU));
                                    PdfDetailCell2.NoWrap = false;
                                    PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell2.HorizontalAlignment = 1;
                                    PdfDetailCell2.Border = Rectangle.NO_BORDER;
                                    //PdfDetailCell2.Border = Rectangle.RIGHT_BORDER;
                                    PdfDetailCell2.Colspan = 9;
                                    PdfDetailTable2.AddCell(PdfDetailCell2);
                                    /*Avinash*/

                                    PdfDetailCell2 = new PdfPCell(new Phrase(""));
                                    PdfDetailCell2.NoWrap = false;
                                    PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell2.HorizontalAlignment = 1;
                                    PdfDetailCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfDetailCell2.BorderWidthRight = 2f;
                                    PdfDetailTable2.AddCell(PdfDetailCell2);

                                    PdfDetailCell2 = new PdfPCell(new Phrase(""));
                                    PdfDetailCell2.NoWrap = false;
                                    PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell2.HorizontalAlignment = 1;
                                    PdfDetailCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfDetailCell2.BorderWidthLeft = 2f;
                                    PdfDetailTable2.AddCell(PdfDetailCell2);


                                    PdfDetailCell2 = new PdfPCell(new Phrase("Or", fontAb11B));
                                    PdfDetailCell2.NoWrap = false;
                                    PdfDetailCell2.BackgroundColor = new Color(252, 252, 252);
                                    PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell2.HorizontalAlignment = 1;
                                    //PdfDetailCell2.BorderWidthLeft = 2f;
                                    //PdfDetailCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    //PdfDetailCell2.Border = Rectangle.LEFT_BORDER;
                                    PdfDetailTable2.AddCell(PdfDetailCell2);


                                    Document documentCheckBox11232 = new Document();
                                    documentCheckBox11232.Open();
                                    Paragraph pCheckBox11232 = new Paragraph();
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 2)
                                    {
                                        if (dr["PeriodTo"].ToString() == "01011900")
                                        {
                                            pCheckBox11232.Add(new Chunk(checkBox, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        }
                                        else
                                        {
                                            pCheckBox11232.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                        }
                                    }
                                    else
                                    {
                                        pCheckBox11232.Add(new Chunk(Box, PdfPCell.ALIGN_LEFT, PdfPCell.ALIGN_LEFT));
                                    }
                                    pCheckBox11232.Add(new Phrase(" Until Cancelled ", fontAb11));
                                    PdfDetailCell2 = new PdfPCell(pCheckBox11232);
                                    PdfDetailCell2.NoWrap = false;
                                    PdfDetailCell2.Colspan = 8;
                                    PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell2.HorizontalAlignment = 1;
                                    PdfDetailTable2.AddCell(PdfDetailCell2);

                                    //PdfDetailCell2 = new PdfPCell(new Phrase(dr["BenificiaryName"].ToString(), fontAbCutomer));
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 2)
                                    {
                                        if (count_Customer < 10)
                                        {
                                            PdfDetailCell2 = new PdfPCell(new Phrase(dr["BenificiaryName"].ToString(), fontAb11B));
                                        }
                                        else if (count_Customer < 20)
                                        {
                                            PdfDetailCell2 = new PdfPCell(new Phrase(dr["BenificiaryName"].ToString(), fontText));
                                        }
                                        else if (count_Customer < 30)
                                        {
                                            PdfDetailCell2 = new PdfPCell(new Phrase(dr["BenificiaryName"].ToString(), fontText6));
                                        }
                                        else
                                        {
                                            PdfDetailCell2 = new PdfPCell(new Phrase(dr["BenificiaryName"].ToString(), fontText5));
                                        }
                                    }
                                    else
                                    {
                                        PdfDetailCell2 = new PdfPCell(new Phrase(" ", fontText5));
                                    }
                                    PdfDetailCell2.NoWrap = false;
                                    PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell2.HorizontalAlignment = 1;
                                    PdfDetailCell2.Border = Rectangle.NO_BORDER;
                                    //PdfDetailCell2.Colspan = 13;
                                    PdfDetailCell2.Colspan = 7;
                                    PdfDetailTable2.AddCell(PdfDetailCell2);
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 2)
                                    {

                                        if (count_Customer < 10)
                                        {
                                            PdfDetailCell2 = new PdfPCell(new Phrase(dr["Customer2"].ToString(), fontAb11B));
                                        }
                                        else if (count_Customer < 20)
                                        {
                                            PdfDetailCell2 = new PdfPCell(new Phrase(dr["Customer2"].ToString(), fontText));
                                        }
                                        else if (count_Customer < 30)
                                        {
                                            PdfDetailCell2 = new PdfPCell(new Phrase(dr["Customer2"].ToString(), fontText6));
                                        }
                                        else
                                        {
                                            PdfDetailCell2 = new PdfPCell(new Phrase(dr["Customer2"].ToString(), fontText5));
                                        }
                                    }
                                    else
                                    {
                                        PdfDetailCell2 = new PdfPCell(new Phrase(" ", fontText5));
                                    }
                                    //PdfDetailCell2 = new PdfPCell(new Phrase(dr["Customer2"].ToString(), fontAbCutomer));
                                    PdfDetailCell2.NoWrap = false;
                                    PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell2.HorizontalAlignment = 1;
                                    PdfDetailCell2.Border = Rectangle.NO_BORDER;
                                    PdfDetailCell2.Colspan = 10;
                                    PdfDetailTable2.AddCell(PdfDetailCell2);
                                    if (Convert.ToInt16(ds.Tables[0].Rows[0]["NoOfQRCopy"]) > 2)
                                    {

                                        if (count_Customer < 10)
                                        {
                                            PdfDetailCell2 = new PdfPCell(new Phrase(dr["Customer3"].ToString(), fontAb11B));
                                        }
                                        else if (count_Customer < 20)
                                        {
                                            PdfDetailCell2 = new PdfPCell(new Phrase(dr["Customer3"].ToString(), fontText));
                                        }
                                        else if (count_Customer < 30)
                                        {
                                            PdfDetailCell2 = new PdfPCell(new Phrase(dr["Customer3"].ToString(), fontText6));
                                        }
                                        else
                                        {
                                            PdfDetailCell2 = new PdfPCell(new Phrase(dr["Customer3"].ToString(), fontText5));
                                        }
                                    }
                                    else
                                    {
                                        PdfDetailCell2 = new PdfPCell(new Phrase(" ", fontText5));
                                    }
                                    //PdfDetailCell2 = new PdfPCell(new Phrase(dr["Customer3"].ToString(), fontAbCutomer));
                                    PdfDetailCell2.NoWrap = false;
                                    PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell2.HorizontalAlignment = 1;
                                    PdfDetailCell2.Border = Rectangle.NO_BORDER;
                                    PdfDetailCell2.Colspan = 6;
                                    PdfDetailTable2.AddCell(PdfDetailCell2);

                                    PdfDetailCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                                    PdfDetailCell2.NoWrap = false;
                                    PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell2.HorizontalAlignment = 1;
                                    PdfDetailCell2.Border = Rectangle.NO_BORDER;
                                    //PdfDetailCell2.Border = Rectangle.RIGHT_BORDER;//Avinash
                                    PdfDetailCell2.BorderWidthRight = 2f;
                                    PdfDetailCell2.Colspan = 9;
                                    PdfDetailTable2.AddCell(PdfDetailCell2);

                                    /*Avinash[14/01/2020]*/
                                    PdfDetailCell2 = new PdfPCell(new Phrase(""));
                                    PdfDetailCell2.NoWrap = false;
                                    PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell2.HorizontalAlignment = 1;
                                    PdfDetailCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfDetailCell2.BorderWidthLeft = 2f;
                                    PdfDetailCell2.BorderWidthRight = 2f;
                                    PdfDetailCell2.Colspan = 38;
                                    PdfDetailTable2.AddCell(PdfDetailCell2);

                                    //PdfDetailCell2 = new PdfPCell(new Phrase(""));
                                    //PdfDetailCell2.NoWrap = false;
                                    //PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    //PdfDetailCell2.HorizontalAlignment = 1;
                                    //PdfDetailCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    //PdfDetailCell2.BorderWidthLeft = 2f;
                                    //PdfDetailCell2.BorderWidthRight = 2f;
                                    //PdfDetailCell2.Colspan = 38;
                                    //PdfDetailTable2.AddCell(PdfDetailCell2);

                                    PdfDetailCell2 = new PdfPCell(new Phrase(""));
                                    PdfDetailCell2.NoWrap = false;
                                    PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell2.HorizontalAlignment = 1;
                                    PdfDetailCell2.Border = Rectangle.NO_BORDER;//Avinash
                                    PdfDetailCell2.BorderWidthLeft = 2f;
                                    PdfDetailTable2.AddCell(PdfDetailCell2);

                                    PdfDetailCell2 = new PdfPCell(new Phrase("This is to confirm that declaration has been carefully read, understood & made by me/us. I'm authorizing the user entity/Corporate to debit my account, based on the instruction as agreed and signed by me. I've understood that I'm authorized to cancel/amend this mandate by appropriately communicating the cancellation/amendment request to the user/entity/corporate or the bank where I've authorized the debit.", fontText5));
                                    PdfDetailCell2.NoWrap = false;
                                    PdfDetailCell2.Colspan = 34;
                                    PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                    PdfDetailCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_LEFT;
                                    PdfDetailCell2.Border = Rectangle.NO_BORDER;
                                    PdfDetailTable2.AddCell(PdfDetailCell2);


                                    PdfDetailCell2 = new PdfPCell(new Phrase(""));
                                    PdfDetailCell2.Border = Rectangle.NO_BORDER;
                                    PdfDetailCell2.BorderWidthRight = 2f;
                                    PdfDetailTable2.AddCell(PdfDetailCell2);

                                    PdfDetailCell2 = new PdfPCell(new Phrase(" "));
                                    PdfDetailCell2.Colspan = 36;
                                    PdfDetailCell2.Border = Rectangle.NO_BORDER;
                                    PdfDetailCell2.FixedHeight = 5f;
                                    PdfDetailCell2.BorderWidthRight = 2f;
                                    PdfDetailCell2.BorderWidthLeft = 2f;
                                    PdfDetailCell2.BorderWidthBottom = 2f;
                                    PdfDetailTable2.AddCell(PdfDetailCell2);

                                    #endregion

                                    //Document pdfDoc = new Document(PageSize.A4, 20f, 20f, 25f, 10f);
                                    // Document pdfDoc = new Document(PageSize.A4, 20f, 20f, 10f, 10f);
                                    //  MemoryStream memoryStream = new MemoryStream();
                                    // HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                                    //  PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                                    //PdfWriter.GetInstance(pdfDoc, new FileStream("" + Convert.ToString(dr["Reference1"]) + ".pdf", FileMode.Create));

                                    pdfDoc.Open();
                                    //Response.ContentType = "Application/pdf";
                                    //Response.AppendHeader("content-disposition", "attachment;filename=" + Convert.ToString(dr["Reference1"]) + ".pdf");
                                    //LineSeparator line = new LineSeparator();
                                    // line = new LineSeparator(2f, 100f, Color.BLACK, Element.ALIGN_TOP, 1); pdfDoc.Add(line);
                                    pdfDoc.Add(new Paragraph(""));
                                    pdfDoc.Add(PdfHeaderTable);
                                    pdfDoc.Add(PdfMidTable);
                                    pdfDoc.Add(PdfDetailTable);
                                    //line = new LineSeparator(2f, 100f, Color.BLACK, Element.ALIGN_BOTTOM, 1); pdfDoc.Add(line);
                                    pdfDoc.Add(CutterImage);
                                    //var spacerParagraph = new Paragraph();
                                    //spacerParagraph.SpacingBefore = 6f;
                                    //spacerParagraph.SpacingAfter = 0f;
                                    //pdfDoc.Add(spacerParagraph);

                                    pdfDoc.Add(PdfHeaderTable1);
                                    pdfDoc.Add(PdfMidTable1);
                                    pdfDoc.Add(PdfDetailTable1);
                                    pdfDoc.Add(CutterImage);
                                    pdfDoc.Add(PdfHeaderTable2);
                                    pdfDoc.Add(PdfMidTable2);
                                    pdfDoc.Add(PdfDetailTable2);
                                    pdfDoc.Close();

                                    writer.Close();
                                    res.PDFBase64 = Convert.ToBase64String(memoryStream.ToArray());
                                    // res.BinaryData1 = memoryStream.ToArray();// bytesPDF;
                                    res.Message = "Mandate downloaded successfully";
                                    res.Status = "Success";
                                    res.ResCode = "ykR20047";
                                    res.MdtID = context.MdtID;
                                    //  _fs.Close();
                                    //  _fs.Dispose();
                                    //Read the File into a Byte Array.
                                    //byte[] bytesPDF = memoryStream.ToArray();//File.ReadAllBytes(pdfPath);
                                    ////Set the Response Content.
                                    //response.Content = new ByteArrayContent(bytesPDF);
                                    ////Set the Response Content Length.
                                    //response.Content.Headers.ContentLength = bytesPDF.LongLength;
                                    ////Set the Content Disposition Header Value and FileName.
                                    //response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                                    //response.Content.Headers.ContentDisposition.FileName = fileName;
                                    ////Set the File Content Type.
                                    //response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(fileName));
                                    ////FileInfo file = new FileInfo(pdfPath);
                                    ////file.Delete();
                                    ////Response.Buffer = true;
                                    ////Response.ContentType = "application/pdf";
                                    ////Response.AddHeader("content-disposition", "attachment;filename=" + Convert.ToString(dr["Reference1"]) + ".pdf");
                                    ////Response.Cache.SetCacheability(HttpCacheability.NoCache);
                                    ////Response.Write(pdfDoc);
                                    ////Response.End();

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // BusinessLibrary1.RaiseError(ex.Message);
                        }

                    }
                    else { 
                    try
                    {

                        query = "Sp_WebAPI";
                        cmd = new SqlCommand(query, con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@QueryType", "UpdateIsPHysical");
                        cmd.Parameters.AddWithValue("@MandateId", context.MdtID);
                        cmd.Parameters.AddWithValue("@UserId", UserId);
                        try
                        {
                            con.Open();
                            int value = cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex) { }
                        finally { con.Close(); }

                        DataSet ds = new DataSet();
                        ds = GetShowForPDF(context.MdtID, context.AppID);
                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                //var FontColour = new BaseColor(35, 31, 32);
                                Font fontAb11 = FontFactory.GetFont("Verdana", 8, Font.BOLD);
                                Font fontAbCutomer = FontFactory.GetFont("Verdana", 7, Font.BOLD);
                                Font font9 = FontFactory.GetFont("Verdana", 8, Font.BOLD);
                                Font fontAb11BU = FontFactory.GetFont("Verdana", 7, Font.UNDERLINE, Color.GRAY);
                                Font fontAb6 = FontFactory.GetFont("Verdana", 8, Font.BOLD);
                                Font fontAb9 = FontFactory.GetFont("Verdana", 8, Font.BOLD);
                                Font fontAb9B = FontFactory.GetFont("Verdana", 8, Font.BOLD);
                                Font fontAb11B = FontFactory.GetFont("Verdana", 8, Font.BOLD);
                                Font fontA11B = FontFactory.GetFont("Verdana", 8, Font.BOLD);
                                Font fontA119B = FontFactory.GetFont("Verdana", 9, Font.BOLD);
                                Font fontText = FontFactory.GetFont("Verdana", 7, Font.BOLD);
                                Font fontText6 = FontFactory.GetFont("Verdana", 6, Font.BOLD);
                                Font fontText5 = FontFactory.GetFont("Verdana", 7, Font.BOLD);
                                Font fontText4 = FontFactory.GetFont("Verdana", 1);
                                iTextSharp.text.Image CutterImage = iTextSharp.text.Image.GetInstance(GetcutterImage(context.AppID));
                                CutterImage.ScaleToFit(550f, 200f);
                                string filePath = ConfigurationManager.AppSettings["FilePath" + context.AppID]; //AppDomain.CurrentDomain.BaseDirectory;
                                var req = System.Net.WebRequest.Create(filePath + "PdfLogoImage/logo.png");
                                // Stream fs = req.GetResponse().GetResponseStream();
                                // FileStream fs = new FileStream(filePath + "PdfLogoImage/logo.png", FileMode.Open, FileAccess.Read);
                                //FileStream fs = new FileStream(req.GetResponse().GetResponseStream(), FileMode.Open, FileAccess.Read);
                                // BinaryReader br = new BinaryReader(fs);
                                WebClient myWebClient = new WebClient();
                                byte[] bytes = myWebClient.DownloadData(filePath + "PdfLogoImage/logo.png");
                                //Byte[] bytes = br.ReadBytes((Int32)fs.Length);
                                iTextSharp.text.Image LogoImage = iTextSharp.text.Image.GetInstance(bytes);
                                LogoImage.ScaleAbsolute(50f, 20f);
                                iTextSharp.text.Image Rupee = iTextSharp.text.Image.GetInstance(GetRupeeIcon(context.AppID));
                                Rupee.BorderColor = iTextSharp.text.Color.BLACK;
                                Rupee.BorderWidth = 12f;
                                Rupee.ScaleToFit(7f, 7f);
                                //FileStream fs111 = new FileStream(filePath + "/images/checkbox-background.jpg", FileMode.Open, FileAccess.Read);
                                // BinaryReader br111 = new BinaryReader(fs111);
                                // Byte[] bytes111 = br111.ReadBytes((Int32)fs111.Length);
                                byte[] bytes111 = myWebClient.DownloadData(filePath + "images/checkbox-background.jpg");
                                iTextSharp.text.Image SmallcheckBox = iTextSharp.text.Image.GetInstance(bytes111);
                                SmallcheckBox.BorderColor = iTextSharp.text.Color.BLACK;
                                SmallcheckBox.BorderWidth = 12f;
                                SmallcheckBox.ScaleToFit(5f, 5f);
                                //  FileStream fs11 = new FileStream(filePath + "/images/tick-iconmandate.png", FileMode.Open, FileAccess.Read);
                                //  BinaryReader br11 = new BinaryReader(fs11);
                                //  Byte[] bytes11 = br11.ReadBytes((Int32)fs11.Length);
                                byte[] bytes11 = myWebClient.DownloadData(filePath + "images/tick-iconmandate.png");
                                iTextSharp.text.Image checkBox = iTextSharp.text.Image.GetInstance(bytes11);
                                checkBox.BorderColor = iTextSharp.text.Color.BLACK;
                                checkBox.BorderWidth = 12f;
                                checkBox.ScaleToFit(8f, 8f);
                                //FileStream fs1 = new FileStream(filePath + "/images/checkbox-background.jpg", FileMode.Open, FileAccess.Read);
                                // BinaryReader br1 = new BinaryReader(fs1);
                                //  Byte[] bytes1 = br1.ReadBytes((Int32)fs1.Length);
                                byte[] bytes1 = myWebClient.DownloadData(filePath + "images/checkbox-background.jpg");
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
                                PdfHeaderCell.BackgroundColor = new Color(252, 252, 252);
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
                                PdfHeaderCell.BackgroundColor = new Color(252, 252, 252);
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
                                PdfHeaderCell.BackgroundColor = new Color(252, 252, 252);
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
                                PdfHeaderCell.BackgroundColor = new Color(252, 252, 252);
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
                                PdfHeaderCell.BackgroundColor = new Color(252, 252, 252);
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
                                PdfHeaderCell.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell = new PdfPCell(new Phrase("Bank a/c Number", fontAb11B));
                                PdfMidCell.NoWrap = false;
                                PdfMidCell.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell.BackgroundColor = new Color(252, 252, 252);
                                PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfMidCell.HorizontalAlignment = 1;
                                PdfMidCell.FixedHeight = 20f;
                                PdfMidCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                PdfMidTable.AddCell(PdfMidCell);
                                PdfMidCell = new PdfPCell(new Phrase(dr["BankName"].ToString(), fontAb11));
                                PdfMidCell.NoWrap = false;
                                PdfMidCell.FixedHeight = 30f;
                                PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfMidCell.Colspan = 6;
                                PdfMidCell.HorizontalAlignment = 1;
                                PdfMidCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                PdfMidTable.AddCell(PdfMidCell);
                                PdfMidCell = new PdfPCell(new Phrase("IFSC", fontAb11B));
                                PdfMidCell.NoWrap = false;
                                PdfMidCell.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell.BackgroundColor = new Color(252, 252, 252);
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
                                //---------------------------------Add Frequency--------------------------------------
                                string Freq = dr["Frequency"].ToString();
                                PdfMidCell = new PdfPCell(new Phrase("Frequency", fontAb11B));
                                PdfMidCell.NoWrap = false;
                                PdfMidCell.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell.Border = Rectangle.NO_BORDER;
                                PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfMidCell.Colspan = 32;
                                PdfMidCell.HorizontalAlignment = 1;
                                PdfMidTable.AddCell(PdfMidCell);
                                PdfMidCell = new PdfPCell(new Phrase("PERIOD", fontAb11B));
                                PdfMidCell.NoWrap = false;
                                PdfMidCell.BackgroundColor = new Color(252, 252, 252);
                                PdfMidCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfMidCell.HorizontalAlignment = 1;
                                PdfMidTable.AddCell(PdfMidCell);
                                PdfMidCell = new PdfPCell(new Phrase("", fontAb11B));
                                PdfMidCell.NoWrap = false;
                                PdfMidCell.Border = Rectangle.NO_BORDER;
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
                                PdfDetailCell.BackgroundColor = new Color(252, 252, 252);
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
                                PdfDetailCell.Border = Rectangle.NO_BORDER;
                                PdfDetailCell.Colspan = 25;
                                PdfDetailTable.AddCell(PdfDetailCell);
                                PdfDetailCell = new PdfPCell(new Phrase("To*", fontAb11B));
                                PdfDetailCell.NoWrap = false;
                                PdfDetailCell.BackgroundColor = new Color(252, 252, 252);
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
                                        PdfDetailTable.AddCell(PdfDetailCell);
                                    }
                                }
                                PdfDetailCell = new PdfPCell(new Phrase(" ", fontAb11B));
                                PdfDetailCell.NoWrap = false;
                                PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell.HorizontalAlignment = 1;
                                PdfDetailCell.Border = Rectangle.NO_BORDER;
                                PdfDetailCell.Colspan = 25;
                                PdfDetailTable.AddCell(PdfDetailCell);
                                PdfDetailCell = new PdfPCell(new Phrase("Or", fontAb11B));
                                PdfDetailCell.NoWrap = false;
                                PdfDetailCell.BackgroundColor = new Color(252, 252, 252);
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
                                PdfDetailCell.Border = Rectangle.NO_BORDER;
                                PdfDetailCell.Colspan = 8;
                                PdfDetailTable.AddCell(PdfDetailCell);
                                PdfDetailCell = new PdfPCell(new Phrase("Sign Acc. Holder", fontAb11BU));
                                PdfDetailCell.NoWrap = false;
                                PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell.HorizontalAlignment = 1;
                                PdfDetailCell.Border = Rectangle.NO_BORDER;
                                PdfDetailCell.Colspan = 8;
                                PdfDetailTable.AddCell(PdfDetailCell);
                                PdfDetailCell = new PdfPCell(new Phrase("Sign Acc. Holder", fontAb11BU));
                                PdfDetailCell.NoWrap = false;
                                PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell.HorizontalAlignment = 1;
                                PdfDetailCell.Border = Rectangle.NO_BORDER;
                                PdfDetailCell.Colspan = 9;
                                PdfDetailTable.AddCell(PdfDetailCell);
                                PdfDetailCell = new PdfPCell(new Phrase(" ", fontAb11B));
                                PdfDetailCell.NoWrap = false;
                                PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell.HorizontalAlignment = 1;
                                PdfDetailCell.Border = Rectangle.NO_BORDER;
                                PdfDetailCell.Colspan = 5;
                                PdfDetailTable.AddCell(PdfDetailCell);
                                PdfDetailCell = new PdfPCell(new Phrase(dr["BenificiaryName"].ToString(), fontAbCutomer));
                                PdfDetailCell.NoWrap = false;
                                PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell.HorizontalAlignment = 1;
                                PdfDetailCell.Border = Rectangle.NO_BORDER;
                                PdfDetailCell.Colspan = 13;
                                PdfDetailTable.AddCell(PdfDetailCell);
                                PdfDetailCell = new PdfPCell(new Phrase(dr["Customer2"].ToString(), fontAbCutomer));
                                PdfDetailCell.NoWrap = false;
                                PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell.HorizontalAlignment = 1;
                                PdfDetailCell.Border = Rectangle.NO_BORDER;
                                PdfDetailCell.Colspan = 10;
                                PdfDetailTable.AddCell(PdfDetailCell);
                                PdfDetailCell = new PdfPCell(new Phrase(dr["Customer3"].ToString(), fontAbCutomer));
                                PdfDetailCell.NoWrap = false;
                                PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell.HorizontalAlignment = 1;
                                PdfDetailCell.Border = Rectangle.NO_BORDER;
                                PdfDetailCell.Colspan = 6;
                                PdfDetailTable.AddCell(PdfDetailCell);
                                PdfDetailCell = new PdfPCell(new Phrase(" ", fontAb11B));
                                PdfDetailCell.NoWrap = false;
                                PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell.HorizontalAlignment = 1;
                                PdfDetailCell.Border = Rectangle.NO_BORDER;
                                PdfDetailCell.Colspan = 9;
                                PdfDetailTable.AddCell(PdfDetailCell);
                                PdfDetailCell = new PdfPCell(new Phrase("1.Name as in Bank Records", fontAb11BU));
                                PdfDetailCell.NoWrap = false;
                                PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell.HorizontalAlignment = 1;
                                PdfDetailCell.Border = Rectangle.NO_BORDER;
                                PdfDetailCell.Colspan = 8;
                                PdfDetailTable.AddCell(PdfDetailCell);
                                PdfDetailCell = new PdfPCell(new Phrase("2.Name as in Bank Records", fontAb11BU));
                                PdfDetailCell.NoWrap = false;
                                PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell.HorizontalAlignment = 1;
                                PdfDetailCell.Border = Rectangle.NO_BORDER;
                                PdfDetailCell.Colspan = 8;
                                PdfDetailTable.AddCell(PdfDetailCell);
                                PdfDetailCell = new PdfPCell(new Phrase("3.Name as in Bank Records", fontAb11BU));
                                PdfDetailCell.NoWrap = false;
                                PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell.HorizontalAlignment = 1;
                                PdfDetailCell.Border = Rectangle.NO_BORDER;
                                PdfDetailCell.Colspan = 9;
                                PdfDetailTable.AddCell(PdfDetailCell);
                                PdfDetailCell = new PdfPCell(new Phrase(" ", fontText4));
                                PdfDetailCell.NoWrap = false;
                                PdfDetailCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell.HorizontalAlignment = 1;
                                PdfDetailCell.Border = Rectangle.NO_BORDER;
                                PdfDetailCell.Colspan = 34;
                                PdfDetailTable.AddCell(PdfDetailCell);
                                PdfDetailCell = new PdfPCell(new Phrase("This is to confirm that declaration has been carefully read, understood & made by me/us. I'm authorizing the user entity/Corporate to debit my account, based on the instruction as agreed and signed by me. I've understood that I'm authorized to cancel/amend this mandate by appropriately communicating the cancellation/amendment request to the user/entity/corporate or the bank where I've authorized the debit.", fontText5));
                                PdfDetailCell.NoWrap = false;
                                PdfDetailCell.Border = Rectangle.NO_BORDER;
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
                                PdfHeaderCell.BackgroundColor = new Color(252, 252, 252);
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
                                PdfHeaderCell1.BackgroundColor = new Color(252, 252, 252);
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
                                PdfHeaderCell1.BackgroundColor = new Color(252, 252, 252);
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
                                PdfHeaderCell1.BackgroundColor = new Color(252, 252, 252);
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
                                PdfHeaderCell1.BackgroundColor = new Color(252, 252, 252);
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
                                PdfHeaderCell1.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell1 = new PdfPCell(new Phrase("Bank a/c Number", fontAb11B));
                                PdfMidCell1.NoWrap = false;
                                PdfMidCell1.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell1.BackgroundColor = new Color(252, 252, 252);
                                PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfMidCell1.HorizontalAlignment = 1;
                                PdfMidCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                PdfMidTable1.AddCell(PdfMidCell1);
                                PdfMidCell1 = new PdfPCell(new Phrase(dr["BankName"].ToString(), fontAb11));
                                PdfMidCell1.NoWrap = false;
                                PdfMidCell1.FixedHeight = 30f;
                                PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfMidCell1.Colspan = 6;
                                PdfMidCell1.HorizontalAlignment = 1;
                                PdfMidCell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                PdfMidTable1.AddCell(PdfMidCell1);
                                PdfMidCell1 = new PdfPCell(new Phrase("IFSC", fontAb11B));
                                PdfMidCell1.NoWrap = false;
                                PdfMidCell1.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell1.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell1.BackgroundColor = new Color(252, 252, 252);
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
                                string Freq1 = dr["Frequency"].ToString();
                                PdfMidCell1 = new PdfPCell(new Phrase("Frequency", fontAb11B));
                                PdfMidCell1.NoWrap = false;
                                PdfMidCell1.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell1.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell1.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell1.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell1.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell1.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell1.Border = Rectangle.NO_BORDER;
                                PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfMidCell1.Colspan = 32;
                                PdfMidCell1.HorizontalAlignment = 1;
                                PdfMidTable1.AddCell(PdfMidCell1);
                                PdfMidCell1 = new PdfPCell(new Phrase("PERIOD", fontAb11B));
                                PdfMidCell1.NoWrap = false;
                                PdfMidCell1.BackgroundColor = new Color(252, 252, 252);
                                PdfMidCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfMidCell1.HorizontalAlignment = 1;
                                PdfMidTable1.AddCell(PdfMidCell1);
                                PdfMidCell1 = new PdfPCell(new Phrase("", fontAb11B));
                                PdfMidCell1.NoWrap = false;
                                PdfMidCell1.Border = Rectangle.NO_BORDER;
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
                                PdfDetailCell1.BackgroundColor = new Color(252, 252, 252);
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
                                PdfDetailCell1.Border = Rectangle.NO_BORDER;
                                PdfDetailCell1.Colspan = 25;
                                PdfDetailTable1.AddCell(PdfDetailCell1);
                                PdfDetailCell1 = new PdfPCell(new Phrase("To*", fontAb11B));
                                PdfDetailCell1.NoWrap = false;
                                PdfDetailCell1.BackgroundColor = new Color(252, 252, 252);
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
                                            //PdfDetailCell1.BackgroundColor = new Color(0, 0, 0);
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
                                        //PdfDetailCell1.BackgroundColor = new Color(0, 0, 0);
                                        PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                        PdfDetailCell1.HorizontalAlignment = 1;
                                        PdfDetailTable1.AddCell(PdfDetailCell1);
                                    }
                                }
                                PdfDetailCell1 = new PdfPCell(new Phrase(" ", fontAb11B));
                                PdfDetailCell1.NoWrap = false;
                                PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell1.HorizontalAlignment = 1;
                                PdfDetailCell1.Border = Rectangle.NO_BORDER;
                                PdfDetailCell1.Colspan = 25;
                                PdfDetailTable1.AddCell(PdfDetailCell1);
                                PdfDetailCell1 = new PdfPCell(new Phrase("Or", fontAb11B));
                                PdfDetailCell1.NoWrap = false;
                                PdfDetailCell1.BackgroundColor = new Color(252, 252, 252);
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
                                PdfDetailCell1.Border = Rectangle.NO_BORDER;
                                PdfDetailCell1.Colspan = 8;
                                PdfDetailTable1.AddCell(PdfDetailCell1);
                                PdfDetailCell1 = new PdfPCell(new Phrase("Sign Acc. Holder", fontAb11BU));
                                PdfDetailCell1.NoWrap = false;
                                PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell1.HorizontalAlignment = 1;
                                PdfDetailCell1.Border = Rectangle.NO_BORDER;
                                PdfDetailCell1.Colspan = 8;
                                PdfDetailTable1.AddCell(PdfDetailCell1);
                                PdfDetailCell1 = new PdfPCell(new Phrase("Sign Acc. Holder", fontAb11BU));
                                PdfDetailCell1.NoWrap = false;
                                PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell1.HorizontalAlignment = 1;
                                PdfDetailCell1.Border = Rectangle.NO_BORDER;
                                PdfDetailCell1.Colspan = 9;
                                PdfDetailTable1.AddCell(PdfDetailCell1);
                                PdfDetailCell1 = new PdfPCell(new Phrase(" ", fontAb11B));
                                PdfDetailCell1.NoWrap = false;
                                PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell1.HorizontalAlignment = 1;
                                PdfDetailCell1.Border = Rectangle.NO_BORDER;
                                PdfDetailCell1.Colspan = 5;
                                PdfDetailTable1.AddCell(PdfDetailCell1);
                                PdfDetailCell1 = new PdfPCell(new Phrase(dr["BenificiaryName"].ToString(), fontAbCutomer));
                                PdfDetailCell1.NoWrap = false;
                                PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell1.HorizontalAlignment = 1;
                                PdfDetailCell1.Border = Rectangle.NO_BORDER;
                                PdfDetailCell1.Colspan = 13;
                                PdfDetailTable1.AddCell(PdfDetailCell1);
                                PdfDetailCell1 = new PdfPCell(new Phrase(dr["Customer2"].ToString(), fontAbCutomer));
                                PdfDetailCell1.NoWrap = false;
                                PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell1.HorizontalAlignment = 1;
                                PdfDetailCell1.Border = Rectangle.NO_BORDER;
                                PdfDetailCell1.Colspan = 10;
                                PdfDetailTable1.AddCell(PdfDetailCell1);
                                PdfDetailCell1 = new PdfPCell(new Phrase(dr["Customer3"].ToString(), fontAbCutomer));
                                PdfDetailCell1.NoWrap = false;
                                PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell1.HorizontalAlignment = 1;
                                PdfDetailCell1.Border = Rectangle.NO_BORDER;
                                PdfDetailCell1.Colspan = 6;
                                PdfDetailTable1.AddCell(PdfDetailCell1);
                                PdfDetailCell1 = new PdfPCell(new Phrase(" ", fontAb11B));
                                PdfDetailCell1.NoWrap = false;
                                PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell1.HorizontalAlignment = 1;
                                PdfDetailCell1.Border = Rectangle.NO_BORDER;
                                PdfDetailCell1.Colspan = 9;
                                PdfDetailTable1.AddCell(PdfDetailCell1);
                                PdfDetailCell1 = new PdfPCell(new Phrase("1.Name as in Bank Records", fontAb11BU));
                                PdfDetailCell1.NoWrap = false;
                                PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell1.HorizontalAlignment = 1;
                                PdfDetailCell1.Border = Rectangle.NO_BORDER;
                                PdfDetailCell1.Colspan = 8;
                                PdfDetailTable1.AddCell(PdfDetailCell1);
                                PdfDetailCell1 = new PdfPCell(new Phrase("2.Name as in Bank Records", fontAb11BU));
                                PdfDetailCell1.NoWrap = false;
                                PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell1.HorizontalAlignment = 1;
                                PdfDetailCell1.Border = Rectangle.NO_BORDER;
                                PdfDetailCell1.Colspan = 8;
                                PdfDetailTable1.AddCell(PdfDetailCell1);
                                PdfDetailCell1 = new PdfPCell(new Phrase("3.Name as in Bank Records", fontAb11BU));
                                PdfDetailCell1.NoWrap = false;
                                PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell1.HorizontalAlignment = 1;
                                PdfDetailCell1.Border = Rectangle.NO_BORDER;
                                PdfDetailCell1.Colspan = 9;
                                PdfDetailTable1.AddCell(PdfDetailCell1);
                                PdfDetailCell1 = new PdfPCell(new Phrase(" ", fontText4));
                                PdfDetailCell1.NoWrap = false;
                                PdfDetailCell1.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell1.HorizontalAlignment = 1;
                                PdfDetailCell1.Border = Rectangle.NO_BORDER;
                                PdfDetailCell1.Colspan = 34;
                                PdfDetailTable1.AddCell(PdfDetailCell1);
                                PdfDetailCell1 = new PdfPCell(new Phrase("This is to confirm that declaration has been carefully read, understood & made by me/us. I'm authorizing the user entity/Corporate to debit my account, based on the instruction as agreed and signed by me. I've understood that I'm authorized to cancel/amend this mandate by appropriately communicating the cancellation/amendment request to the user/entity/corporate or the bank where I've authorized the debit.", fontText5));
                                PdfDetailCell1.NoWrap = false;
                                PdfDetailCell1.Border = Rectangle.NO_BORDER;
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
                                PdfHeaderCell2.BackgroundColor = new Color(252, 252, 252);
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
                                PdfHeaderCell2.BackgroundColor = new Color(252, 252, 252);
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
                                PdfHeaderCell2 = new PdfPCell(new Phrase("Sponsor bankcode", fontAb11B));
                                PdfHeaderCell2.NoWrap = false;
                                PdfHeaderCell2.Colspan = 4;
                                PdfHeaderCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfHeaderCell2.BackgroundColor = new Color(252, 252, 252);
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
                                PdfHeaderCell2.BackgroundColor = new Color(252, 252, 252);
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
                                PdfHeaderCell2 = new PdfPCell(pCheckBox2);
                                PdfHeaderTable2.AddCell(PdfHeaderCell2);
                                PdfHeaderCell2 = new PdfPCell(new Phrase("I/We hereby Authorize", fontText));
                                PdfHeaderCell2.NoWrap = false;
                                PdfHeaderCell2.BackgroundColor = new Color(252, 252, 252);
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
                                PdfHeaderCell2.BackgroundColor = new Color(252, 252, 252);
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
                                PdfHeaderCell2 = new PdfPCell(pCheckBoxSB2);
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
                                PdfMidCell2 = new PdfPCell(new Phrase("Bank a/c Number", fontAb11B));
                                PdfMidCell2.NoWrap = false;
                                PdfMidCell2.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell2 = new PdfPCell(new Phrase("With Bank", fontAb11B));
                                PdfMidCell2.NoWrap = false;
                                PdfMidCell2.FixedHeight = 20f;
                                PdfMidCell2.BackgroundColor = new Color(252, 252, 252);
                                PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfMidCell2.HorizontalAlignment = 1;
                                PdfMidCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                PdfMidTable2.AddCell(PdfMidCell2);
                                char[] array = (Convert.ToString(dr["BankName"]).Trim()).ToCharArray();
                                PdfMidCell2 = new PdfPCell(new Phrase(dr["BankName"].ToString(), fontAb11));
                                PdfMidCell2.NoWrap = false;
                                PdfMidCell2.FixedHeight = 30f;
                                PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfMidCell2.Colspan = 6;
                                PdfMidCell2.HorizontalAlignment = 1;
                                PdfMidCell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                                PdfMidTable2.AddCell(PdfMidCell2);
                                PdfMidCell2 = new PdfPCell(new Phrase("IFSC", fontAb11B));
                                PdfMidCell2.NoWrap = false;
                                PdfMidCell2.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell2 = new PdfPCell(new Phrase("or MICR", fontAb11B));
                                PdfMidCell2.NoWrap = false;
                                PdfMidCell2.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell2 = new PdfPCell(new Phrase("an amount of Rupees", fontAb11B));
                                PdfMidCell2.NoWrap = false;
                                PdfMidCell2.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell2.BackgroundColor = new Color(252, 252, 252);
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
                                string DebitType2 = dr["DebitType"].ToString();
                                PdfMidCell2 = new PdfPCell(new Phrase("Debit Type", fontAb11B));
                                PdfMidCell2.NoWrap = false;
                                PdfMidCell2.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell2.NoWrap = false;
                                PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfMidCell2.Colspan = 6;
                                PdfMidCell2.HorizontalAlignment = 1;
                                PdfMidTable2.AddCell(PdfMidCell2);
                                PdfMidCell2 = new PdfPCell(new Phrase("Reference 1", fontAb11B));
                                PdfMidCell2.NoWrap = false;
                                PdfMidCell2.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell2.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell2.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell2.BackgroundColor = new Color(252, 252, 252);
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
                                PdfMidCell2.Border = Rectangle.NO_BORDER;
                                PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfMidCell2.Colspan = 32;
                                PdfMidCell2.HorizontalAlignment = 1;
                                PdfMidTable2.AddCell(PdfMidCell2);
                                PdfMidCell2 = new PdfPCell(new Phrase("PERIOD", fontAb11B));
                                PdfMidCell2.NoWrap = false;
                                PdfMidCell2.BackgroundColor = new Color(252, 252, 252);
                                PdfMidCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfMidCell2.HorizontalAlignment = 1;
                                PdfMidTable2.AddCell(PdfMidCell2);
                                PdfMidCell2 = new PdfPCell(new Phrase("", fontAb11B));
                                PdfMidCell2.NoWrap = false;
                                PdfMidCell2.Border = Rectangle.NO_BORDER;
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
                                PdfDetailCell2.BackgroundColor = new Color(252, 252, 252);
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
                                PdfDetailCell2 = new PdfPCell(new Phrase(" ", fontAb11));
                                PdfDetailCell2.NoWrap = false;
                                PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell2.HorizontalAlignment = 1;
                                PdfDetailCell2.Border = Rectangle.NO_BORDER;
                                PdfDetailCell2.Colspan = 25;
                                PdfDetailTable2.AddCell(PdfDetailCell2);
                                PdfDetailCell2 = new PdfPCell(new Phrase("To*", fontAb11B));
                                PdfDetailCell2.NoWrap = false;
                                PdfDetailCell2.BackgroundColor = new Color(252, 252, 252);
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
                                            //PdfDetailCell2.BackgroundColor = new Color(0, 0, 0);
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
                                        PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                        PdfDetailCell2.HorizontalAlignment = 1;
                                        PdfDetailTable2.AddCell(PdfDetailCell2);
                                    }
                                }
                                PdfDetailCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                                PdfDetailCell2.NoWrap = false;
                                PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell2.HorizontalAlignment = 1;
                                PdfDetailCell2.Border = Rectangle.NO_BORDER;
                                PdfDetailCell2.Colspan = 25;
                                PdfDetailTable2.AddCell(PdfDetailCell2);
                                PdfDetailCell2 = new PdfPCell(new Phrase("Or", fontAb11B));
                                PdfDetailCell2.NoWrap = false;
                                PdfDetailCell2.BackgroundColor = new Color(252, 252, 252);
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
                                PdfDetailCell2.Border = Rectangle.NO_BORDER;
                                PdfDetailCell2.Colspan = 8;
                                PdfDetailTable2.AddCell(PdfDetailCell2);
                                PdfDetailCell2 = new PdfPCell(new Phrase("Sign Acc. Holder", fontAb11BU));
                                PdfDetailCell2.NoWrap = false;
                                PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell2.HorizontalAlignment = 1;
                                PdfDetailCell2.Border = Rectangle.NO_BORDER;
                                PdfDetailCell2.Colspan = 8;
                                PdfDetailTable2.AddCell(PdfDetailCell2);
                                PdfDetailCell2 = new PdfPCell(new Phrase("Sign Acc. Holder", fontAb11BU));
                                PdfDetailCell2.NoWrap = false;
                                PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell2.HorizontalAlignment = 1;
                                PdfDetailCell2.Border = Rectangle.NO_BORDER;
                                PdfDetailCell2.Colspan = 9;
                                PdfDetailTable2.AddCell(PdfDetailCell2);
                                PdfDetailCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                                PdfDetailCell2.NoWrap = false;
                                PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell2.HorizontalAlignment = 1;
                                PdfDetailCell2.Border = Rectangle.NO_BORDER;
                                PdfDetailCell2.Colspan = 5;
                                PdfDetailTable2.AddCell(PdfDetailCell2);
                                PdfDetailCell2 = new PdfPCell(new Phrase(dr["BenificiaryName"].ToString(), fontAbCutomer));
                                PdfDetailCell2.NoWrap = false;
                                PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell2.HorizontalAlignment = 1;
                                PdfDetailCell2.Border = Rectangle.NO_BORDER;
                                PdfDetailCell2.Colspan = 13;
                                PdfDetailTable2.AddCell(PdfDetailCell2);
                                PdfDetailCell2 = new PdfPCell(new Phrase(dr["Customer2"].ToString(), fontAbCutomer));
                                PdfDetailCell2.NoWrap = false;
                                PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell2.HorizontalAlignment = 1;
                                PdfDetailCell2.Border = Rectangle.NO_BORDER;
                                PdfDetailCell2.Colspan = 10;
                                PdfDetailTable2.AddCell(PdfDetailCell2);
                                PdfDetailCell2 = new PdfPCell(new Phrase(dr["Customer3"].ToString(), fontAbCutomer));
                                PdfDetailCell2.NoWrap = false;
                                PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell2.HorizontalAlignment = 1;
                                PdfDetailCell2.Border = Rectangle.NO_BORDER;
                                PdfDetailCell2.Colspan = 6;
                                PdfDetailTable2.AddCell(PdfDetailCell2);
                                PdfDetailCell2 = new PdfPCell(new Phrase(" ", fontAb11B));
                                PdfDetailCell2.NoWrap = false;
                                PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell2.HorizontalAlignment = 1;
                                PdfDetailCell2.Border = Rectangle.NO_BORDER;
                                PdfDetailCell2.Colspan = 9;
                                PdfDetailTable2.AddCell(PdfDetailCell2);
                                PdfDetailCell2 = new PdfPCell(new Phrase("1.Name as in Bank Records", fontAb11BU));
                                PdfDetailCell2.NoWrap = false;
                                PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell2.HorizontalAlignment = 1;
                                PdfDetailCell2.Border = Rectangle.NO_BORDER;
                                PdfDetailCell2.Colspan = 8;
                                PdfDetailTable2.AddCell(PdfDetailCell2);
                                PdfDetailCell2 = new PdfPCell(new Phrase("2.Name as in Bank Records", fontAb11BU));
                                PdfDetailCell2.NoWrap = false;
                                PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell2.HorizontalAlignment = 1;
                                PdfDetailCell2.Border = Rectangle.NO_BORDER;
                                PdfDetailCell2.Colspan = 8;
                                PdfDetailTable2.AddCell(PdfDetailCell2);
                                PdfDetailCell2 = new PdfPCell(new Phrase("3.Name as in Bank Records", fontAb11BU));
                                PdfDetailCell2.NoWrap = false;
                                PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell2.HorizontalAlignment = 1;
                                PdfDetailCell2.Border = Rectangle.NO_BORDER;
                                PdfDetailCell2.Colspan = 9;
                                PdfDetailTable2.AddCell(PdfDetailCell2);
                                PdfDetailCell2 = new PdfPCell(new Phrase(" ", fontText4));
                                PdfDetailCell2.NoWrap = false;
                                PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell2.HorizontalAlignment = 1;
                                PdfDetailCell2.Border = Rectangle.NO_BORDER;
                                PdfDetailCell2.Colspan = 34;
                                PdfDetailTable2.AddCell(PdfDetailCell2);
                                PdfDetailCell2 = new PdfPCell(new Phrase("This is to confirm that declaration has been carefully read, understood & made by me/us. I'm authorizing the user entity/Corporate to debit my account, based on the instruction as agreed and signed by me. I've understood that I'm authorized to cancel/amend this mandate by appropriately communicating the cancellation/amendment request to the user/entity/corporate or the bank where I've authorized the debit.", fontText5));
                                PdfDetailCell2.NoWrap = false;
                                PdfDetailCell2.Border = Rectangle.NO_BORDER;
                                PdfDetailCell2.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                                PdfDetailCell2.Colspan = 34;
                                PdfDetailTable2.AddCell(PdfDetailCell2);
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
                                //using (var fileStream = new FileStream("E:\\testfatehs.pdf", FileMode.Create, FileAccess.Write))
                                //{
                                //    ms.CopyTo(fileStream);
                                //}



                                // var pdfPath=  @"C:\Users\dev-y02\Desktop\Amit121118\EXAPIForPdf\EXAPIForPdf\Files\" + fileName;

                                //Check whether File blank.
                                if (fileName == "")
                                {
                                    //Throw 404 (Not Found) exception if File not found.
                                    //  response.StatusCode = HttpStatusCode.NotFound;
                                    // response.ReasonPhrase = string.Format("File not found: {0} .", fileName);
                                    //throw new HttpResponseException(response);
                                }
                                // Closing the document
                                pdfDoc.Close();
                                writer.Close();
                                // _fs.Close();
                                //  _fs.Dispose();
                                //Read the File into a Byte Array.
                                // byte[] bytesPDF = File.ReadAllBytes(memoryStream);
                                //  var inputAsString = Convert.ToBase64String(memoryStream.ToArray());
                                res.PDFBase64 = Convert.ToBase64String(memoryStream.ToArray());
                                // res.BinaryData1 = memoryStream.ToArray();// bytesPDF;
                                res.Message = "Mandate downloaded successfully";
                                res.Status = "Success";
                                res.ResCode = "ykR20047";
                                res.MdtID = context.MdtID;
                                //Set the Response Content.
                                // response.Content = new ByteArrayContent(bytesPDF);
                                //Set the Response Content Length.
                                //  response.Content.Headers.ContentLength = bytesPDF.LongLength;
                                //Set the Content Disposition Header Value and FileName.
                                //   response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                                //   response.Content.Headers.ContentDisposition.FileName = fileName;
                                //Set the File Content Type.
                                //   response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(fileName));
                                // FileInfo file = new FileInfo(pdfPath);
                                // file.Delete();
                            }
                        }
                        else
                        {
                            // return Request.CreateErrorResponse(HttpStatusCode.NotFound, "KPY000");
                            HttpResponseMessage responseMessage = Request.CreateResponse(HttpStatusCode.Created, "ERR000 OR Incomplete data");
                            return res;
                        }
                    }
                    catch (Exception ex)
                    {
                        return res;
                    }
                }
                }
                else
                {
                    //context.Status = "Failure";
                    //context.ResCode = "ERR000";
                    //context.Message = "Invalid Credentials";
                    // return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Failure OR ERR000 OR Invalid Credentials");
                    HttpResponseMessage responseMessage = Request.CreateResponse(HttpStatusCode.Created, "KPY000 OR Incomplete data");
                    return res;
                }
            }
            return res;
        }






        public DataSet GetShowForPDF(string MandateId, string AppID)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[AppID].ConnectionString);
                string query = "Sp_GetEmailData";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "ShowData");
                cmd.Parameters.AddWithValue("@Id", MandateId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet dsData = new DataSet();
                da.Fill(dsData);
                return dsData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public byte[] GetRupeeIcon(string AppID)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[AppID].ConnectionString);
                string query = "Sp_LogoImageData";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "GetRupeeIcon");
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet dsData = new DataSet();
                da.Fill(dsData);
                return (byte[])dsData.Tables[0].Rows[0][0];
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public byte[] GetcutterImage(string AppID)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[AppID].ConnectionString);
                string query = "Sp_LogoImageData";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "Getcutternew");
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet dsData = new DataSet();
                da.Fill(dsData);
                return (byte[])dsData.Tables[0].Rows[0][0];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private bool IsResponseValid(HttpResponseMessage response)
        {
            if ((response != null) && (response.StatusCode == HttpStatusCode.OK))
                return true;
            return false;
        }
    }

    public class BinaryDownloadmandatePdf
    {
        public string MdtID { get; set; }
        public string MerchantKey { get; set; }
        public string AppID { get; set; }
        public string PhysicalNach { get; set; }
        public string PrintQr { get; set; }
    }
    //public class BinaryPDFResponse
    //{
    //    public string Message { get; set; }
    //    public string Status { get; set; }
    //    public string MandateId { get; set; }
    //    public string PdfContent { get; set; }
    //}
    //public class BinaryGetMandatePDFReq
    //{
    //    public string AppID { get; set; }
    //    public string MandateId { get; set; }
    //    public string MerchantKey { get; set; }
    //    public string Status { get; set; }
    //    public string Message { get; set; }
    //}

}
