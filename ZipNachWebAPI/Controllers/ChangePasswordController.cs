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
using System.IO;
using System.Text;
using System.Web.UI;
using System.Net.Mail;

namespace ZipNachWebAPI.Controllers
{
    public class ChangePasswordController : ApiController
    {
        [Route("api/UploadMandate/ChangePassword")]
        [HttpPost]
        public ChangePasswordResponse ChangePassword(ChangePassword ul)
        {
            ChangePasswordResponse res = new ChangePasswordResponse();
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(ul.AppId)].ConnectionString);
                string Password = string.Empty;
                string PasswordKey = string.Empty;
                string ChangePassword = string.Empty;
                string changePasswordKey = string.Empty;
                ChangePassword = DBsecurity.Encrypt(ul.Password, ref changePasswordKey);
                con.Open();
                string query = "Sp_WebSevice";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "ChangePassword");
                cmd.Parameters.AddWithValue("@UserId", ul.UserId);
                cmd.Parameters.AddWithValue("@Password", ChangePassword);
                cmd.Parameters.AddWithValue("@PasswordKey", changePasswordKey);




                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable ds = new DataTable();
                da.Fill(ds);


                if (ds != null && ds.Rows.Count > 0)
                {
                    res.status = "Success";
                    res.message = "Password changed successfully";
                    res.userId = ul.UserId;

                }
                else
                {
                    res.status = "failure";
                    res.message = "Password not changed";
                    res.userId = "";
                }


                con.Close();


            }
            catch (Exception ex)
            {
                res.status = "failure";
                res.message = "Invalid data";
            }
            return res;
        }
        [Route("api/UploadMandate/ForgetPassword")]
        [HttpPost]
        public SendEmailResponse ForgetPassword(SendEmail Data)
        {
            SendEmailResponse pf = new SendEmailResponse();
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(Data.AppId)].ConnectionString);
                con.Open();
                string query = "Sp_WebSevice";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "ChkEmail");
                cmd.Parameters.AddWithValue("@EmailId", Data.emailId);


                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0 && ds.Tables[0] != null && Convert.ToInt32(ds.Tables[0].Rows[0]["value"]) == 1)
                    {
                        if (ds.Tables[1].Rows.Count > 0 && ds.Tables[1] != null)
                        {
                            using (StringWriter sw = new StringWriter())
                            {
                                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                                {
                                    StringBuilder sb = new StringBuilder();
                                    string WebAppUrl = ConfigurationManager.AppSettings["WebAppUrl"+Data.AppId].ToString();
                                    //string SMTPHost = ConfigurationManager.AppSettings["SMTPHost"].ToString();
                                    //string UserId = ConfigurationManager.AppSettings["UserId"].ToString();
                                    //string MailPassword = ConfigurationManager.AppSettings["MailPassword"].ToString();
                                    //string SMTPPort = ConfigurationManager.AppSettings["SMTPPort"].ToString();
                                    //string SMTPEnableSsl = ConfigurationManager.AppSettings["SMTPEnableSsl"].ToString();

                                    string SMTPHost = ConfigurationManager.AppSettings["Amazon_SMTPHost"].ToString();
                                    string UserId = ConfigurationManager.AppSettings["Amazon_UserId"].ToString();
                                    string MailPassword = ConfigurationManager.AppSettings["Amazon_MailPassword"].ToString();
                                    string SMTPPort = ConfigurationManager.AppSettings["Amazon_SMTPPort"].ToString();
                                    string SMTPEnableSsl = ConfigurationManager.AppSettings["Amazon_SMTPEnableSsl"].ToString();
                                    string FromMailId = ConfigurationManager.AppSettings["Amazon_FromMailId"+Data.AppId].ToString();

                                    sb.Append("Dear " + ds.Tables[1].Rows[0]["UserName"].ToString() + ",<br> <br>");
                                    sb.Append("Please click on the below button to set a new Password . <br> <br>");


                                    string User = DBsecurity.Encrypt(ds.Tables[1].Rows[0]["UserId"].ToString());
                                    sb.Append("<a href='" + WebAppUrl + "ChangePassword.aspx?Id=" + User + "' target='_blank'>");
                                    sb.Append("<input style='background-color: #3965a9;color: #fff;padding: 3px 10px 3px 10px;' type='button' value='Change Password' /></a> </div>");
                                    
                                    SmtpClient smtpClient = new SmtpClient();

                                    MailMessage mailmsg = new MailMessage();
                                    MailAddress mailaddress = new MailAddress(FromMailId);
                                    
                                    mailmsg.To.Add(ds.Tables[1].Rows[0]["EmailId"].ToString());
                                    
                                    mailmsg.From = mailaddress;

                                    mailmsg.Subject = "Recover Password";
                                    mailmsg.IsBodyHtml = true;
                                    mailmsg.Body = sb.ToString();
                                    
                                    smtpClient.Host = SMTPHost;
                                    smtpClient.Port = Convert.ToInt32(SMTPPort);
                                    smtpClient.EnableSsl = Convert.ToBoolean(SMTPEnableSsl);
                                    smtpClient.UseDefaultCredentials = true;
                                    smtpClient.Credentials = new System.Net.NetworkCredential(UserId, MailPassword);
                                    smtpClient.Send(mailmsg);


                                }
                            }
                        }
                        pf.message = "Successfully Send";
                        pf.status = "Success";

                    }
                    else if (ds.Tables[0].Rows.Count > 0 && ds.Tables[0] != null && Convert.ToInt32(ds.Tables[0].Rows[0]["value"]) == 2)
                    {
                        if (ds.Tables[1].Rows.Count > 0 && ds.Tables[1] != null)
                        {
                            using (StringWriter sw = new StringWriter())
                            {
                                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                                {
                                    string WebAppUrl = ConfigurationManager.AppSettings["WebAppUrl"].ToString();
                                    string SMTPHost = ConfigurationManager.AppSettings["SMTPHost"].ToString();
                                    string UserId = ConfigurationManager.AppSettings["UserId"].ToString();
                                    string MailPassword = ConfigurationManager.AppSettings["MailPassword"].ToString();
                                    string SMTPPort = ConfigurationManager.AppSettings["SMTPPort"].ToString();
                                    string SMTPEnableSsl = ConfigurationManager.AppSettings["SMTPEnableSsl"].ToString();

                                    StringBuilder sb = new StringBuilder();


                                    sb.Append("Dear Sir/Mam ,<br> <br>");
                                    sb.Append(" " + ds.Tables[1].Rows[0]["Name"].ToString() + " has requested to set a new password. Please click on the below button to set a new Password. <br> <br>");

                                    string User = DBsecurity.Encrypt(ds.Tables[1].Rows[0]["UserId"].ToString());
                                    sb.Append("<a href='" + WebAppUrl + "ChangePassword.aspx?Id=" + User + "' target='_blank'>");
                                    sb.Append("<input style='background-color: #3965a9;color: #fff;padding: 3px 10px 3px 10px;' type='button' value='Change Password' /></a> </div>");





                                    SmtpClient smtpClient = new SmtpClient();

                                    MailMessage mailmsg = new MailMessage();
                                    MailAddress mailaddress = new MailAddress(UserId);



                                    mailmsg.To.Add(ds.Tables[2].Rows[0]["EmailId"].ToString());



                                    mailmsg.From = mailaddress;

                                    mailmsg.Subject = "Recover Password";
                                    mailmsg.IsBodyHtml = true;
                                    mailmsg.Body = sb.ToString();



                                    smtpClient.Host = SMTPHost;
                                    smtpClient.Port = Convert.ToInt32(SMTPPort);
                                    smtpClient.EnableSsl = Convert.ToBoolean(SMTPEnableSsl);
                                    smtpClient.UseDefaultCredentials = true;
                                    smtpClient.Credentials = new System.Net.NetworkCredential(UserId, MailPassword);
                                    smtpClient.Send(mailmsg);


                                }
                            }
                        }
                        pf.message = "Successfully Send";
                        pf.status = "Success";
                    }
                    else if (ds.Tables[0].Rows.Count > 0 && ds.Tables[0] != null && Convert.ToInt32(ds.Tables[0].Rows[0]["value"]) == -1)
                    {
                        pf.status = "failure";
                        pf.message = "Not exist";
                    }
                }
                else
                {
                    pf.status = "failure";
                    pf.message = "Not exist";
                }

            }
            catch (Exception ex)
            {
                pf.message = "Invalid data";
                pf.status = "failure";
            }
            return pf;
        }
    }
}
