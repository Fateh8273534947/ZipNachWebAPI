using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using ImageUploadWCF;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

namespace ZipNachWebAPI.Models
{
    public class ValidatePresement
    {
        public static bool CheckAccess(string AppId, string Type)
        {
            bool presentstatus = false;
            bool Accountstatus = false;
            bool Emandatestatus = false;
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(AppId)].ConnectionString);
                string query = "Sp_PresentMentWebApi";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "CheckAccess");
                cmd.Parameters.AddWithValue("@AppId", AppId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToString(dt.Rows[0]["IsPresentment"]).ToUpper() == "TRUE")
                    {
                        presentstatus = true;
                    }
                    if (Convert.ToString(dt.Rows[0]["IsAccountvalidation"]).ToUpper() == "TRUE")
                    {
                        Accountstatus = true;
                    }
                    if (Convert.ToString(dt.Rows[0]["IsEmandate"]).ToUpper() == "TRUE")
                    {
                        Emandatestatus = true;
                    }
                }
                if (Type == "P")
                {
                    return presentstatus;
                }
                else if (Type == "E")
                {
                    return Emandatestatus;
                }
                else
                {
                    return Accountstatus;
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
                if (Type == "P")
                {
                    return presentstatus;
                }
                else if (Type == "E")
                {
                    return Emandatestatus;
                }
                else
                {
                    return Accountstatus;
                }
            }
        }
        public static bool ValidateAppID(string AppID)
        {
            bool status = false;
            try
            {
                foreach (ConnectionStringSettings css in ConfigurationManager.ConnectionStrings)
                {
                    string name = css.Name;
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
        public static bool ValidateDate(string PresentmentDate)
        {
            bool status = false;
            try
            {

                string dateValue = PresentmentDate; // or 2015-11-19
                DateTime date = Convert.ToDateTime(dateValue);
                string tempdate = date.ToString("yyyy-MM-dd");
                if (tempdate != dateValue)
                {
                    status = true;
                }


            }
            catch (Exception ex)
            {
                status = true;
            }
            return status;
        }
        public static string ValidateEntityDate(string PresentmentDate, string AppID, string date, string BankCode)
        {
            string status = "";
            try
            {
                //string temp = ConfigurationManager.AppSettings["EnitityMarchantKey" + AppID];
                //if (temp.Trim() == DBsecurity.Decrypt(EnitityMarchantKey))
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(AppID)].ConnectionString);
                string query = "Sp_PresentMentWebApi";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "CheckIsAcceptable");
                cmd.Parameters.AddWithValue("@AppId", AppID);
                cmd.Parameters.AddWithValue("@PresentmentDate", date);
                cmd.Parameters.AddWithValue("@Bankcode", BankCode);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    status = Convert.ToString(dt.Rows[0]["IsCheck"]);
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
            }
            return status;
        }
        public static bool ValidateDate(string PresentmentDate, string AppID, string date, string BankCode)
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
                cmd.Parameters.AddWithValue("@QueryType", "CheckIsAcceptable");
                cmd.Parameters.AddWithValue("@AppId", AppID);
                cmd.Parameters.AddWithValue("@PresentmentDate", date);
                cmd.Parameters.AddWithValue("@Bankcode", BankCode);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToString(dt.Rows[0]["IsCheck"]) == "1")
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
        public static bool ValidateInteger(string value)
        {
            bool status = false;
            try
            {
                Int64 check;
                if (!Int64.TryParse(value, out check))
                    status = true;
            }
            catch (Exception ex)
            {

            }
            return status;
        }
        public static int CheckXMLDUPLICATE(string xml)
        {
            int duplicatecount = 0;
            try
            {

                ArrayList UMRN = new ArrayList();
                StringReader theReader = new StringReader(xml);
                DataSet dsxml = new DataSet("DS");
                dsxml.ReadXml(theReader);
                for (int i = 0; i < dsxml.Tables[0].Rows.Count; i++)
                {
                    UMRN.Add(dsxml.Tables[0].Rows[i]["UMRN"].ToString());
                    if (dsxml.Tables[0].Rows[i]["UMRN"].ToString() == "")
                    {
                        return duplicatecount + 1;
                    }
                    //bool checkinteger = ValidatePresement.ValidateInteger(dsxml.Tables[0].Rows[i]["UMRN"].ToString());
                    //if (checkinteger == true)
                    //{
                    //    return duplicatecount + 1;
                    //}
                    for (int x = i + 1; x < dsxml.Tables[0].Rows.Count; x++)
                    {
                        if (UMRN.Contains(dsxml.Tables[0].Rows[x]["UMRN"].ToString()))
                        {
                            return duplicatecount + 1;
                        }


                    }


                }
                return duplicatecount;
            }
            catch (Exception ex)
            {

                return duplicatecount + 1;

            }


        }
        public static string XMLCREATION(string UMRN, string AMOUNT, string UpdatedOn, string Status)
        {
            string Singlexml = "";
            Singlexml += "<UMRNDTLS>";
            Singlexml += "<UMRN>"; Singlexml += UMRN; Singlexml += "</UMRN>"; Singlexml += "<AMOUNT>"; Singlexml += AMOUNT; Singlexml += "</AMOUNT>"; Singlexml += "<Status>"; Singlexml += Status; Singlexml += "</Status>"; Singlexml += "<UpdatedOn>"; Singlexml += UpdatedOn; Singlexml += "</UpdatedOn>"; Singlexml += "</UMRNDTLS>";
            return Singlexml;

        }
        public static string CreateXML(string xml)
        {
            string Singlexml = "<dtxml>";
            StringReader theReader = new StringReader(xml);
            DataSet dsxml = new DataSet("DS");
            dsxml.ReadXml(theReader);
            for (int i = 0; i < dsxml.Tables[0].Rows.Count; i++)
            {
                Singlexml += "<dtxml";
                Singlexml += " " + "UMRN=" + @"""" + dsxml.Tables[0].Rows[i]["UMRN"].ToString() + @"""" + " ";

                for (int j = i; j < dsxml.Tables[0].Rows.Count; j++)
                {
                    Singlexml += "AMOUNT=" + @"""" + dsxml.Tables[0].Rows[i]["AMOUNT"].ToString() + @"""" + "/>";
                    break;
                }
            }
            Singlexml += "</dtxml >";
            return Singlexml;

        }
        public static string CreateUMRNXML(string xml)
        {
            string Singlexml = "<dtxml>";
            StringReader theReader = new StringReader(xml);
            DataSet dsxml = new DataSet("DS");
            dsxml.ReadXml(theReader);
            for (int i = 0; i < dsxml.Tables[0].Rows.Count; i++)
            {
                Singlexml += "<dtxml";
                Singlexml += " " + "UMRN=" + @"""" + dsxml.Tables[0].Rows[i]["UMRN"].ToString() + @"""" + " />";

            }
            Singlexml += "</dtxml >";
            return Singlexml;

        }
        public static bool CheckXMLDUPLICATEAMOUNT(string xml)
        {
            bool duplicatecount = false;
            StringReader theReader = new StringReader(xml);
            DataSet dsxml = new DataSet("DS");
            dsxml.ReadXml(theReader);
            for (int i = 0; i < dsxml.Tables[0].Rows.Count; i++)
            {
                bool UMRNAMT = ValidatePresement.ValidateAmount(dsxml.Tables[0].Rows[i]["AMOUNT"].ToString());
                if (UMRNAMT == false) { return true; }

            }
            return duplicatecount;

        }
        public static string XMLCREATIONWITHFILE(string UMRN, string AMOUNT, string CreatedOn, string UpdatedOn, string Status, string fileno)
        {
            string Singlexml = "";

            Singlexml += "<UMRNDTLS>";
            Singlexml += "<FileNo>"; Singlexml += fileno; Singlexml += "</FileNo>"; Singlexml += "<AMOUNT>"; Singlexml += AMOUNT; Singlexml += "</AMOUNT>"; Singlexml += "<Status>"; Singlexml += Status; Singlexml += "</Status>"; Singlexml += "<CreatedOn>"; Singlexml += CreatedOn; Singlexml += "</CreatedOn>"; Singlexml += "<UpdatedOn>"; Singlexml += UpdatedOn; Singlexml += "</UpdatedOn>"; Singlexml += "</UMRNDTLS>";
            return Singlexml;

        }


    }
}