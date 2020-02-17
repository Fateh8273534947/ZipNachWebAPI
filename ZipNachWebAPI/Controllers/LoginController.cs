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

namespace ZipNachWebAPI.Controllers
{
    public class LoginController : ApiController
    {
        // [Route("api/Login/Logindata")]
        [Route("api/UploadMandate/Logindata")]
        [HttpPost]
        public LoginResponsee Logindata(Login ul)
        {
            LoginResponsee res = new LoginResponsee();
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(ul.AppId)].ConnectionString);
                string Message = "";
                string userId = "";
                string Username = "";

                con.Open();
                string query = "Sp_WebSevice";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "GetUser");
                cmd.Parameters.AddWithValue("@UserName", ul.emailId);


                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet dt = new DataSet();
                da.Fill(dt);

                // bool isFound = false;

                int i = 0;
                if (dt != null)
                {
                    foreach (DataRow row in dt.Tables[0].Rows)
                    {
                        string pin = dt.Tables[0].Rows[i]["UserName"].ToString();
                        string Passw = DBsecurity.Decrypt(dt.Tables[0].Rows[i]["Password"].ToString(), dt.Tables[0].Rows[i]["PasswordKey"].ToString());
                        if (Passw == ul.password.Trim())
                        {
                            Username = Convert.ToString(dt.Tables[0].Rows[i]["UserName"]);
                            res.userName = Username;
                            userId = Convert.ToString(dt.Tables[0].Rows[i]["UserId"]);
                            res.userId = userId;
                            Message = "Login successfully";
                            res.message = Message;
                            res.status = "success";
                            break;

                        }
                        else
                        {
                            res.status = "failure";
                            Message = "Invalid Credentials";
                        }
                        i++;
                    }


                }
                else
                {
                    res.status = "failure";
                    res.message = "Invalid Credentials";
                    res.userName = "";
                    res.userId = "";
                }


                con.Close();


            }

            catch (Exception ex)
            {
                res.status = "server error";
                res.message = "Invalid data";
            }
            return res;
        }
    }
}
