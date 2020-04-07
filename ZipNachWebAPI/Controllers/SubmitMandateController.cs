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
    [EnableCors(origins: "http://192.168.1.246:808/api/Clients", headers: "AllowAllHeaders", methods: "*")]
    public class SubmitMandateController : ApiController
    {
        //submitmandate
        [Route("api/UploadMandate/submitmandate")]
        [HttpPost]
        public GetSubmitMandateResponse GetSubmitMandateInfo(GetSubmitMandateReq Data)
        {
            GetSubmitMandateResponse response = new GetSubmitMandateResponse();
            try
            {

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
                //else if (ValidatePresement.CheckAccess(Data.AppId.Trim(), "A") != true)
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
                //else if (!CheckMandateInfo.CheckAccountValidation(Data.MdtID, Data.AppID))
                //{
                //    response.Message = "Account should be validated";
                //    response.Status = "Failure";
                //    response.ResCode = "ERR0004";
                //    return response;
                //}
                else if (CheckMandateInfo.CheckENachValidation(Data.MdtID, Data.AppID))
                {
                    response.Message = "Mandate type already selected as eMandate";
                    response.Status = "Failure";
                    response.ResCode = "ykR20030";
                    return response;
                }
                else if (!CheckMandateInfo.CheckImageValidation(Data.MdtID, Data.AppID))
                {
                    response.Message = "Mandate Image not found";
                    response.Status = "Failure";
                    response.ResCode = "ykR20048";
                    return response;
                }
                else if (Data.MerchantKey == "")
                {
                    response.Message = "Incomplete data";
                    response.Status = "Failure";
                    response.ResCode = "ykR20020";
                    return response;
                }
                //else if (Data.MerchantKey == "")
                //{
                //    response.Message = "Incomplete data";
                //    response.Status = "Failure";
                //    response.ResCode = "ERR000";
                //    return response;
                //}

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
                    // string temp = ConfigurationManager.AppSettings["EnitityMarchantKey" + Data.AppID];
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
                    // }

                    if (Flag)
                    {
                        try
                        {
                            con.Open();
                            query = "Sp_WebAPI";
                            cmd = new SqlCommand(query, con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@QueryType", "GetSubmitMandateResponse");
                            cmd.Parameters.AddWithValue("@UserId", UserId);
                            cmd.Parameters.AddWithValue("@MandateId", Data.MdtID);
                            da = new SqlDataAdapter(cmd);
                            dt = new DataTable();
                            da.Fill(dt);
                            con.Close();
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                response.Message = "Mandate submitted to Bank successfully";
                                response.ResCode = "ykR20034";
                                response.Status = "Success";
                                response.MandateId = Data.MdtID;

                            }
                        }
                        catch (Exception ex)
                        {
                            //Console.Out.WriteLine("-----------------");
                            //Console.Out.WriteLine(ex.Message);
                        }
                    }
                    else
                    {
                        response.Status = "Failure";
                        response.ResCode = "ykR20020";
                        response.Message = "Incomplete data";
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = "Failure";
                response.ResCode = "ykR20020";
                response.Message = "Incomplete data";
            }
            return response;
        }

        [Route("api/UploadMandate/Cancelmandate")]
        [HttpPost]
        public GetSubmitMandateResponse CancelMandate(GetSubmitMandateReq Data)
        {
            GetSubmitMandateResponse response = new GetSubmitMandateResponse();
            try
            {

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
                    // string temp = ConfigurationManager.AppSettings["EnitityMarchantKey" + Data.AppID];
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
                    // }

                    if (Flag)
                    {
                        try
                        {
                            con.Open();
                            query = "Sp_Mandate";
                            cmd = new SqlCommand(query, con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@QueryType", "UpdateIsCancel");
                            cmd.Parameters.AddWithValue("@UserId", UserId);
                            cmd.Parameters.AddWithValue("@MandateId", Data.MdtID);
                            da = new SqlDataAdapter(cmd);
                            dt = new DataTable();
                            da.Fill(dt);
                            con.Close();
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                response.Message = "Mandate cancelled successfully";
                                response.ResCode = "ykR20052";
                                response.Status = "Success";
                                response.MandateId = Data.MdtID;

                            }
                        }
                        catch (Exception ex)
                        {
                            //Console.Out.WriteLine("-----------------");
                            //Console.Out.WriteLine(ex.Message);
                        }
                    }
                    else
                    {
                        response.Status = "Failure";
                        response.ResCode = "ykR20020";
                        response.Message = "Incomplete data";
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = "Failure";
                response.ResCode = "ykR20020";
                response.Message = "Incomplete data";
            }
            return response;
        }

        public class GetSubmitMandateResponse
        {
            public string Message { get; set; }
            public string Status { get; set; }
            public string MandateId { get; set; }
            public string ResCode { get; set; }


        }

    }
}
