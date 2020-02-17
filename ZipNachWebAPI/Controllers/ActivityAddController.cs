using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ZipNachWebAPI.Models;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

namespace QuickCheckPresentmentAPI.Controllers
{
    public class ActivityAddController : ApiController
    {
        [Route("api/ActivityAdd/RecordAdd")]
        [HttpPost]
        public loginResponse RecordAdd(SavePresentmentDetails ul)
        {

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[ul.AppID].ConnectionString);
            string query = ""; SqlCommand dbcommand;
            loginResponse res = new loginResponse();
            if (ul.AppID ==null || ul.AppID.Trim() == "")
            {
                res.ResDescription = "Invalid AppId";
                res.Status = "Failure";
                res.ActivityID = ul.ActivityID;
                res.ResCode = "ykR30008";
                res.FileNo = "";
                res.BankCode = ul.BankCode;
                res.CreatedDateTime = "";
                res.LastEditDateTime = "";
                res.TotalRecords = "";
                res.TotalAmt = "";
                res.FileStatus = "";
                res.UMRNData = ul.UMRNData;
                return res;
            }
            else if (ul.AppID.Trim() != "" && ValidatePresement.ValidateAppID(ul.AppID.Trim()) != true)
            {
                res.ResDescription = "Invalid AppId";
                res.Status = "Failure";
                res.ActivityID = ul.ActivityID;
                res.ResCode = "ykR30008";
                res.FileNo = "";
                res.BankCode = ul.BankCode;
                res.CreatedDateTime = "";
                res.LastEditDateTime = "";
                res.TotalRecords = "";
                res.TotalAmt = "";
                res.FileStatus = "";
                res.UMRNData = ul.UMRNData;
                return res;
            }
            else if (ValidatePresement.CheckAccess(ul.AppID.Trim(),"P") != true)
            {
                res.ResDescription = "Unauthorized user";
                res.Status = "Failure";
                res.ActivityID = ul.ActivityID;
                res.ResCode = "ykR30017"; 
                res.FileNo = "";
                res.BankCode = ul.BankCode;
                res.CreatedDateTime = "";
                res.LastEditDateTime = "";
                res.TotalRecords = "";
                res.TotalAmt = "";
                res.FileStatus = "";
                res.UMRNData = ul.UMRNData;
                return res;
            }
            else if (ul.MerchantKey == null || ul.MerchantKey.Trim() == "")
            {
                res.ResDescription = "Invalid Merchant Key";
                res.Status = "Failure";
                res.ActivityID = ul.ActivityID;
                res.ResCode = "ykR30009";
                res.FileNo = "";
                res.BankCode = ul.BankCode;
                res.CreatedDateTime = "";
                res.LastEditDateTime = "";
                res.TotalRecords = "";
                res.TotalAmt = "";
                res.FileStatus = "";
                res.UMRNData = ul.UMRNData;
                return res;
            }
            else if (ul.MerchantKey.Trim() != "" && ValidatePresement.ValidateEntityMerchantKey(ul.MerchantKey.Trim(),ul.AppID) != true)
            {
                res.ResDescription = "Invalid Merchant Key";
                res.Status = "Failure";
                res.ActivityID = ul.ActivityID;
                res.ResCode = "ykR30009";
                res.FileNo = "";
                res.BankCode = ul.BankCode;
                res.CreatedDateTime = "";
                res.LastEditDateTime = "";
                res.TotalRecords = "";
                res.TotalAmt = "";
                res.FileStatus = "";
                res.UMRNData = ul.UMRNData;
                return res;
            }
            else if (ul.ActivityID == null || ul.ActivityID.Trim() == "")
            {
                res.ResDescription = "Invalid ActivityId";
                res.Status = "Failure";
                res.ActivityID = ul.ActivityID;
                res.ResCode = "ykR30010";
                res.FileNo = "";
                res.BankCode = ul.BankCode;
                res.CreatedDateTime = "";
                res.LastEditDateTime = "";
                res.TotalRecords = "";
                res.TotalAmt = "";
                res.FileStatus = "";
                res.UMRNData = ul.UMRNData;
                return res;
            }
            //else if (ul.ActivityID.Trim() != "")
            //{
            //    int checklength = ul.ActivityID.Length;
            //    if (checklength != 18)
            //    {
            //        res.ResDescription = "Invalid ActivityId";
            //        res.Status = "Failure";
            //        res.ActivityID = ul.ActivityID;
            //        res.ResCode = "ykR30010";
            //        res.FileNo = "";
            //        res.BankCode = ul.BankCode;
            //        res.CreatedDateTime = "";
            //        res.LastEditDateTime = "";
            //        res.TotalRecords = "";
            //        res.TotalAmt = "";
            //        res.FileStatus = "";
            //        res.UMRNData = ul.UMRNData;
            //        return res;
            //    }

            //}
            if (ul.RequestType == null || ul.RequestType.Trim() == "")
            {
                res.ResDescription = "Invalid Data";
                res.Status = "Failure";
                res.ActivityID = ul.ActivityID;
                res.ResCode = "ykR30012";
                res.FileNo = "";
                res.BankCode = ul.BankCode;
                res.CreatedDateTime = "";
                res.LastEditDateTime = "";
                res.TotalRecords = "";
                res.TotalAmt = "";
                res.FileStatus = "";
                res.UMRNData = ul.UMRNData;
                return res;
            }
            else if (ul.FileNo == null || ul.FileNo.Trim() == "")
            {
                res.ResDescription = "Invalid File Number";
                res.Status = "Failure";
                res.ActivityID = ul.ActivityID;
                res.ResCode = "ykR30011";
                res.FileNo = ul.FileNo;
                res.BankCode = ul.BankCode;
                res.CreatedDateTime = "";
                res.LastEditDateTime = "";
                res.TotalRecords = "";
                res.TotalAmt = "";
                res.FileStatus = "";
                res.UMRNData = ul.UMRNData;
                return res;
            }
            else if (ul.UMRNData == null || ul.UMRNData.Trim() == "")
            {
                res.ResDescription = "Invalid Data";
                res.Status = "Failure";
                res.ActivityID = ul.ActivityID;
                res.ResCode = "ykR30012";
                res.FileNo = "";
                res.BankCode = ul.BankCode;
                res.CreatedDateTime = "";
                res.LastEditDateTime = "";
                res.TotalRecords = "";
                res.TotalAmt = "";
                res.FileStatus = "";
                res.UMRNData = ul.UMRNData;
                return res;
            }
            else
            {
                bool Flag = true;
                #region For Checking Activity
                try
                {
                    if (ul.ActivityID.Trim() != "")
                    {
                        query = "Sp_PresentMentWebApi";
                        dbcommand = new SqlCommand(query, conn);
                        dbcommand.Connection.Open();
                        dbcommand.CommandType = CommandType.StoredProcedure;
                        dbcommand.Parameters.AddWithValue("@QueryType", "ValidateActivity");
                        dbcommand.Parameters.AddWithValue("@ActivityId", ul.ActivityID);
                        SqlDataAdapter da = new SqlDataAdapter(dbcommand);
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        int getactivity = Convert.ToInt32(ds.Tables[0].Rows.Count);
                        if (getactivity == 0)
                        {
                            dbcommand.Connection.Close(); dbcommand.Parameters.Clear();
                            res.ResDescription = "Invalid ActivityId";
                            res.Status = "Failure";
                            res.ActivityID = ul.ActivityID;
                            res.ResCode = "ykR30010";
                            res.FileNo = "";
                            res.BankCode = ul.BankCode;
                            res.CreatedDateTime = "";
                            res.LastEditDateTime = "";
                            res.TotalRecords = "";
                            res.TotalAmt = "";
                            res.FileStatus = "";
                            res.UMRNData = ul.UMRNData;
                            return res;

                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine("-----------------");
                    Console.Out.WriteLine(ex.Message);
                }

                #endregion
                #region For Checking File Number
                try
                {
                    if (ul.FileNo.Trim() != "")
                    {
                        conn.Close();
                        query = "Sp_PresentMentWebApi";
                        dbcommand = new SqlCommand(query, conn);
                        dbcommand.Connection.Open();
                        dbcommand.CommandType = CommandType.StoredProcedure;
                        dbcommand.Parameters.AddWithValue("@QueryType", "ValidateFileno");
                        dbcommand.Parameters.AddWithValue("@FileNumber", ul.FileNo);
                        SqlDataAdapter da = new SqlDataAdapter(dbcommand);
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        int getactivity = Convert.ToInt32(ds.Tables[0].Rows.Count);
                        if (getactivity == 0)
                        {
                            dbcommand.Connection.Close(); dbcommand.Parameters.Clear();
                            res.ResDescription = "Invalid File Number";
                            res.Status = "Failure";
                            res.ActivityID = ul.ActivityID;
                            res.ResCode = "ykR30011";
                            res.FileNo = "";
                            res.BankCode = ul.BankCode;
                            res.CreatedDateTime = "";
                            res.LastEditDateTime = "";
                            res.TotalRecords = "";
                            res.TotalAmt = "";
                            res.FileStatus = "";
                            res.UMRNData = ul.UMRNData;
                            Flag = false;
                            return res;

                        }
                        else
                        {
                            ul.PresentmentDate = Convert.ToString(ds.Tables[0].Rows[0]["Date"]);
                        }
                    }
                    #region Checking Valid Date 
                    if (ValidatePresement.ValidateDate(ul.PresentmentDate, ul.AppID, ul.PresentmentDate, ul.BankCode))
                    {
                        res.ResDescription = "Unauthorized request";
                        res.Status = "Failure";
                        res.ActivityID = ul.ActivityID;
                        res.ResCode = "ykR30017";
                        res.FileNo = "";
                        res.BankCode = ul.BankCode;
                        res.CreatedDateTime = "";
                        res.LastEditDateTime = "";
                        res.TotalRecords = "";
                        res.TotalAmt = "";
                        res.FileStatus = "";
                        res.UMRNData = ul.UMRNData;
                        Flag = false;
                        return res;
                    }
                    #endregion
                    #region Checking Valid Date 
                    if (ValidatePresement.ValidateEntityDate(ul.PresentmentDate, ul.AppID, ul.PresentmentDate, ul.BankCode) == "2")
                    {
                        res.ResDescription = "Unauthorized request";
                        res.Status = "Failure";
                        res.ActivityID = ul.ActivityID;
                        res.ResCode = "ykR30017";
                        res.FileNo = "";
                        res.BankCode = ul.BankCode;
                        res.CreatedDateTime = "";
                        res.LastEditDateTime = "";
                        res.TotalRecords = "";
                        res.TotalAmt = "";
                        res.FileStatus = "";
                        res.UMRNData = ul.UMRNData;
                        Flag = false;
                        return res;
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine("-----------------");
                    Console.Out.WriteLine(ex.Message);
                }

                #endregion
                #region Check integer compare
                if (ul.RequestType.Trim() != "")
                {
                    if (ul.RequestType == "3")
                    {
                        bool checkrequesttype = ValidatePresement.ValidateInteger(ul.RequestType);
                        if (checkrequesttype == true)
                        {
                            res.ResDescription = "Invalid Data";
                            res.Status = "Failure";
                            res.ActivityID = ul.ActivityID;
                            res.ResCode = "ykR30012";
                            res.FileNo = "";
                            res.BankCode = ul.BankCode;
                            res.CreatedDateTime = "";
                            res.LastEditDateTime = "";
                            res.TotalRecords = "";
                            res.TotalAmt = "";
                            res.FileStatus = "";
                            res.UMRNData = ul.UMRNData;
                            Flag = false;
                            return res;
                        }
                    }
                    else
                    {
                        res.ResDescription = "Invalid Data";
                        res.Status = "Failure";
                        res.ActivityID = ul.ActivityID;
                        res.ResCode = "ykR30012";
                        res.FileNo = "";
                        res.BankCode = ul.BankCode;
                        res.CreatedDateTime = "";
                        res.LastEditDateTime = "";
                        res.TotalRecords = "";
                        res.TotalAmt = "";
                        res.FileStatus = "";
                        res.UMRNData = ul.UMRNData;
                        Flag = false;
                        return res;
                    }
                }
                #endregion
                #region Check File integer compare
                if (ul.FileNo.Trim() != "")
                {
                    bool checkfileno = ValidatePresement.ValidateInteger(ul.FileNo);
                    if (checkfileno == true)
                    {
                        res.ResDescription = "Invalid File Number";
                        res.Status = "Failure";
                        res.ActivityID = ul.ActivityID;
                        res.ResCode = "ykR30011";
                        res.FileNo = "";
                        res.BankCode = ul.BankCode;
                        res.CreatedDateTime = "";
                        res.LastEditDateTime = "";
                        res.TotalRecords = "";
                        res.TotalAmt = "";
                        res.FileStatus = "";
                        res.UMRNData = ul.UMRNData;
                        Flag = false;
                        return res;
                    }
                }
                #endregion
                #region Checking Duplicate XML
                int getcount = ValidatePresement.CheckXMLDUPLICATE(ul.UMRNData);
                if (getcount == 1)
                {
                    res.ResDescription = "Invalid Data";
                    res.Status = "Failure";
                    res.ActivityID = ul.ActivityID;
                    res.ResCode = "ykR30012";
                    res.FileNo = "";
                    res.BankCode = ul.BankCode;
                    res.CreatedDateTime = "";
                    res.LastEditDateTime = "";
                    res.TotalRecords = "";
                    res.TotalAmt = "";
                    res.FileStatus = "";
                    res.UMRNData = ul.UMRNData;
                    Flag = false;
                    return res;
                }
                #endregion
                #region Checking Numeric or decimal Amount in XML
                bool checkamt = ValidatePresement.CheckXMLDUPLICATEAMOUNT(ul.UMRNData);
                if (checkamt == true)
                {
                    res.ResDescription = "Invalid Data";
                    res.Status = "Failure";
                    res.ActivityID = ul.ActivityID;
                    res.ResCode = "ykR30012";
                    res.FileNo = "";
                    res.BankCode = ul.BankCode;
                    res.CreatedDateTime = "";
                    res.LastEditDateTime = "";
                    res.TotalRecords = "";
                    res.TotalAmt = "";
                    res.FileStatus = "";
                    res.UMRNData = ul.UMRNData;
                    Flag = false;
                    return res;
                }
                #endregion
                if (Flag)
                {
                    string xml = ul.UMRNData;
                    string createsinglexml = ValidatePresement.CreateXML(xml);
                    #region Insert Presentment Header
                    try
                    {
                        conn.Close();
                        query = "Sp_PresentMentWebApi";
                        dbcommand = new SqlCommand(query, conn);
                        dbcommand.CommandType = CommandType.StoredProcedure;
                        dbcommand.Parameters.AddWithValue("@QueryType", "EXISTINGADDPRESENTMENTFILE");
                        dbcommand.Parameters.AddWithValue("@FileNumber", Convert.ToString(ul.FileNo.Trim()));
                        dbcommand.Parameters.AddWithValue("@AppId", Convert.ToInt64(ul.AppID.Trim()));
                        dbcommand.Parameters.AddWithValue("@ActivityId", ul.ActivityID.Trim());
                        dbcommand.Parameters.AddWithValue("@RequestType", Convert.ToInt64(ul.RequestType.Trim()));
                        dbcommand.Parameters.AddWithValue("@UMRNData", createsinglexml.Trim());
                        SqlDataAdapter da = new SqlDataAdapter(dbcommand);
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        if (ds.Tables[0].Rows.Count > 0 && ds.Tables[1].Rows.Count > 0)
                        {
                            res.ActivityID = ds.Tables[1].Rows[0]["ActivityId"].ToString();
                            res.FileNo = ds.Tables[1].Rows[0]["FileNo"].ToString();
                            res.BankCode = ds.Tables[1].Rows[0]["BankCode"].ToString();
                            res.CreatedDateTime = Convert.ToDateTime(ds.Tables[1].Rows[0]["CreatedDateTime"].ToString()).ToString("yyyy-MM-dd hh:mm");
                            res.LastEditDateTime = Convert.ToDateTime(ds.Tables[1].Rows[0]["LastEditDateTime"].ToString()).ToString("yyyy-MM-dd hh:mm");
                            res.TotalRecords = ds.Tables[0].Rows[0]["Recordcount"].ToString();
                            res.TotalAmt = ds.Tables[0].Rows[0]["TotalAmount"].ToString();
                            res.Status = "Success";
                            res.FileStatus = ds.Tables[1].Rows[0]["FileStatus"].ToString();
                            res.ResCode = "ykR30003";
                            res.ResDescription = "Record Added Successfully";
                            string GenerateXML = ""; string UMRN = ""; string AMOUNT = "";
                            string finalxml = "<UMDATA>";
                            for (int i = 0; i < ds.Tables[2].Rows.Count; i++)
                            {
                                UMRN = ds.Tables[2].Rows[i]["UMRN"].ToString();
                                AMOUNT = ds.Tables[2].Rows[i]["AMOUNT"].ToString();
                                GenerateXML = ValidatePresement.XMLCREATION(UMRN, AMOUNT, Convert.ToDateTime(ds.Tables[1].Rows[0]["LastEditDateTime"].ToString()).ToString("yyyy-MM-dd hh:mm"), ds.Tables[1].Rows[0]["FileStatus"].ToString());
                                finalxml += GenerateXML;
                            }
                            finalxml += "</UMDATA>";
                            res.UMRNData = finalxml;
                            return res;
                        }
                        else
                        {
                            res.ResDescription = "Invalid Data";
                            res.Status = "Failure";
                            res.ActivityID = ul.ActivityID;
                            res.ResCode = "ykR30012";
                            res.FileNo = "";
                            res.BankCode = ul.BankCode;
                            res.CreatedDateTime = "";
                            res.LastEditDateTime = "";
                            res.TotalRecords = "";
                            res.TotalAmt = "";
                            res.FileStatus = "";
                            res.UMRNData = ul.UMRNData;
                            return res;
                        }


                    }
                    catch (Exception ex)
                    {
                        Console.Out.WriteLine("-----------------");
                        Console.Out.WriteLine(ex.Message);
                    }
                    #endregion

                }

                else
                {
                    res.ResDescription = "Invalid Data";
                    res.Status = "Failure";
                    res.ActivityID = ul.ActivityID;
                    res.ResCode = "ykR30012";
                    res.FileNo = "";
                    res.BankCode = ul.BankCode;
                    res.CreatedDateTime = "";
                    res.LastEditDateTime = "";
                    res.TotalRecords = "";
                    res.TotalAmt = "";
                    res.FileStatus = "";
                    res.UMRNData = ul.UMRNData;
                    return res;
                }

            }
            return res;



        }
    }
}
