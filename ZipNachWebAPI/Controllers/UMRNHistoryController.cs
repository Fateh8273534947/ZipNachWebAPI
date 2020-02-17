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
    public class UMRNHistoryController : ApiController
    {

        [Route("api/UMRNHistory/FileUMRNHistory")]
        [HttpPost]
        public HistoryloginResponse FileUMRNHistory(HistorySavePresentmentDetails ul)
        {

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[ul.AppID].ConnectionString);
            string query = ""; SqlCommand dbcommand;
            HistoryloginResponse res = new HistoryloginResponse();
            if (ul.AppID == null || ul.AppID.Trim() == "")
            {
                res.UMRN = ul.UMRN;
                res.TotalRecords = "";
                res.TotalAmt = "";
                res.Status = "Failure";
                res.ResCode = "ykR30008";
                res.ResDescription = "Invalid AppId";
                res.UMRNData = ul.UMRNData;
                return res;
            }
            else if (ul.AppID.Trim() != "" && ValidatePresement.ValidateAppID(ul.AppID.Trim()) != true)
            {
                res.UMRN = ul.UMRN;
                res.TotalRecords = "";
                res.TotalAmt = "";
                res.Status = "Failure";
                res.ResCode = "ykR30008";
                res.ResDescription = "Invalid AppId";
                res.UMRNData = ul.UMRNData;
                return res;
            }
            else if (ValidatePresement.CheckAccess(ul.AppID.Trim(), "P") != true)
            {
                res.ResDescription = "Unauthorized user";
                res.Status = "Failure";
                res.ResCode = "ykR30017";
                res.TotalRecords = "";
                res.TotalAmt = "";
                res.UMRNData = ul.UMRNData;
                return res;
            }
            else if (ul.MerchantKey == null || ul.MerchantKey.Trim() == "")
            {
                res.UMRN = ul.UMRN;
                res.TotalRecords = "";
                res.TotalAmt = "";
                res.Status = "Failure";
                res.ResCode = "ykR30009";
                res.ResDescription = "Invalid MerchantKey";
                res.UMRNData = ul.UMRNData;
                return res;
            }
            else if (ul.MerchantKey.Trim() != "" && ValidatePresement.ValidateEntityMerchantKey(ul.MerchantKey.Trim(),ul.AppID) != true)
            {
                res.UMRN = ul.UMRN;
                res.TotalRecords = "";
                res.TotalAmt = "";
                res.Status = "Failure";
                res.ResCode = "ykR30009";
                res.ResDescription = "Invalid MerchantKey";
                res.UMRNData = ul.UMRNData;
                return res;
            }
            else if (ul.ActivityID == null || ul.ActivityID.Trim() == "")
            {
                res.UMRN = ul.UMRN;
                res.TotalRecords = "";
                res.TotalAmt = "";
                res.Status = "Failure";
                res.ResCode = "ykR30010";
                res.ResDescription = "Invalid ActivityId";
                res.UMRNData = ul.UMRNData;
                return res;
            }
            else if (ul.UMRN == null || ul.UMRN.Trim() == "")
            {
                res.UMRN = ul.UMRN;
                res.TotalRecords = "";
                res.TotalAmt = "";
                res.Status = "Failure";
                res.ResCode = "ykR30012";
                res.ResDescription = "Invalid Data";
                res.UMRNData = ul.UMRNData;
                return res;
            }
            //else if (ul.ActivityID.Trim() != "")
            //{
            //    int checklength = ul.ActivityID.Length;
            //    if (checklength != 18)
            //    {
            //        res.UMRN = ul.UMRN;
            //        res.TotalRecords = "";
            //        res.TotalAmt = "";
            //        res.Status = "Failure";
            //        res.ResCode = "ykR30010";
            //        res.ResDescription = "Invalid ActivityId";
            //        res.UMRNData = ul.UMRNData;
            //        return res;
            //    }

            //}
            if (ul.RequestType == null || ul.RequestType.Trim() == "")
            {
                res.UMRN = ul.UMRN;
                res.TotalRecords = "";
                res.TotalAmt = "";
                res.Status = "Failure";
                res.ResCode = "ykR30012";
                res.ResDescription = "Invalid Data";
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
                            res.UMRNData = ul.UMRN;
                            res.TotalRecords = "";
                            res.TotalAmt = "";
                            res.Status = "Failure";
                            res.ResCode = "ykR30010";
                            res.ResDescription = "Invalid ActivityId";
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
                #region Check integer compare
                if (ul.RequestType.Trim() != "")
                {
                    if (ul.RequestType == "7")
                    {
                        bool checkrequesttype = ValidatePresement.ValidateInteger(ul.RequestType);
                        if (checkrequesttype == true)
                        {
                            res.UMRNData = ul.UMRN;
                            res.TotalRecords = "";
                            res.TotalAmt = "";
                            res.Status = "Failure";
                            res.ResCode = "ykR30012";
                            res.ResDescription = "Invalid Data";
                            res.UMRNData = ul.UMRNData;
                            Flag = false;
                            return res;
                        }
                    }
                    else
                    {
                        res.UMRN = ul.UMRN;
                        res.TotalRecords = "";
                        res.TotalAmt = "";
                        res.Status = "Failure";
                        res.ResCode = "ykR30012";
                        res.ResDescription = "Invalid Data";
                        res.UMRNData = ul.UMRNData;
                        Flag = false;
                        return res;
                    }
                }
                #endregion
                #region Check integer compare
                //if (ul.UMRN.Trim() != "")
                //{
                //    bool checkumrninteger = ValidatePresement.ValidateInteger(ul.UMRN);
                //    if (checkumrninteger == true)
                //    {
                //        res.UMRN = ul.UMRN;
                //        res.TotalRecords = "";
                //        res.TotalAmt = "";
                //        res.Status = "Failure";
                //        res.ResCode = "ykR30012";
                //        res.ResDescription = "Invalid Data";
                //        res.UMRNData = ul.UMRNData;
                //        Flag = false;
                //        return res;
                //    }

                //}
                #endregion
                if (Flag)
                {
                    #region Insert Presentment Header
                    try
                    {
                        conn.Close();
                        query = "Sp_PresentMentWebApi";
                        dbcommand = new SqlCommand(query, conn);
                        dbcommand.CommandType = CommandType.StoredProcedure;
                        dbcommand.Parameters.AddWithValue("@QueryType", "EXISTINGUMRNHISTORY");
                        dbcommand.Parameters.AddWithValue("@AppId", Convert.ToInt64(ul.AppID.Trim()));
                        dbcommand.Parameters.AddWithValue("@ActivityId", ul.ActivityID.Trim());
                        dbcommand.Parameters.AddWithValue("@RequestType", Convert.ToInt64(ul.RequestType.Trim()));
                        dbcommand.Parameters.AddWithValue("@SINGLEUMRN", ul.UMRN.Trim());
                        SqlDataAdapter da = new SqlDataAdapter(dbcommand);
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        if (ds.Tables[1].Rows.Count > 0 && ds.Tables[1].Rows.Count > 0)
                        {
                            res.UMRN = ds.Tables[0].Rows[0]["UMRN"].ToString();
                            res.TotalRecords = ds.Tables[0].Rows[0]["totalrecord"].ToString();
                            res.TotalAmt = ds.Tables[0].Rows[0]["totalamount"].ToString();
                            res.Status = "Success";
                            res.ResCode = "ykR30007";
                            res.ResDescription = "UMRN Details Received Successfully";
                            string GenerateXML = ""; string UMRN = ""; string AMOUNT = ""; string fileno = "";
                            string finalxml = "<UMDATA>";
                            
                            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                            {
                                UMRN = ds.Tables[1].Rows[i]["UMRN"].ToString();
                                AMOUNT = ds.Tables[1].Rows[i]["AMOUNT"].ToString();
                                fileno= ds.Tables[1].Rows[i]["FileNo"].ToString();
                                GenerateXML = ValidatePresement.XMLCREATIONWITHFILE(UMRN, AMOUNT, Convert.ToDateTime(ds.Tables[1].Rows[0]["CreatedOn"].ToString()).ToString("yyyy-MM-dd hh:mm"), Convert.ToDateTime(ds.Tables[1].Rows[0]["UpdatedOn"].ToString()).ToString("yyyy-MM-dd hh:mm"), ds.Tables[1].Rows[0]["UMRNStatus"].ToString(),fileno);
                                finalxml += GenerateXML;
                            }
                            
                            finalxml += "</UMDATA>";
                            res.UMRNData = finalxml;
                            return res;
                        }
                        else
                        {
                            res.UMRN = ul.UMRN;
                            res.TotalRecords = "";
                            res.TotalAmt = "";
                            res.Status = "Failure";
                            res.ResCode = "ykR30012";
                            res.ResDescription = "Invalid Data";
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
                    res.UMRN = ul.UMRN;
                    res.TotalRecords = "";
                    res.TotalAmt = "";
                    res.Status = "Failure";
                    res.ResCode = "ykR30012";
                    res.ResDescription = "Invalid Data";
                    res.UMRNData = ul.UMRNData;
                    return res;
                }

            }
            return res;



        }

    }
}
