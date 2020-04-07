using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ZipNachWebAPI.Models;

namespace ZipNachWebAPI.Controllers
{
    public class GetAllNPCILiveBankDataController : ApiController
    {
        [Route("api/UploadMandate/GetBankData")]
        [HttpPost]
       
        public BankResponseData GetBankdata(GetMandateReq Data)
        {
            BankResponseData response = new BankResponseData();
            List<BankResponse> ListView = new List<BankResponse>();
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
                string query = "Sp_WebAPI";
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Convert.ToString(Data.AppID)].ConnectionString);
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "GetLiveBank");
               // cmd.Parameters.AddWithValue("@appId", Data.AppID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {

                    BankResponse bnk = new BankResponse();
                    bnk.BankCode = row["BankCode"].ToString();
                    bnk.BankName = row["BankName"].ToString();
                    bnk.LiveOnDebitCard = row["LiveOnDebitCard"].ToString();
                    bnk.LiveOnNetBanking = row["LiveOnNetBanking"].ToString();
                    ListView.Add(bnk);
                }
                response.BankData = ListView;
                response.Message = "All Live Bank Data On NPCI received successfully";
                response.ResCode = "ykR20035";
                response.Status = "Success";
                return response;
            }
        }
    }
    public class BankResponse
    {
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string LiveOnDebitCard { get; set; }
        public string LiveOnNetBanking { get; set; }
       
    }
    public class BankResponseData
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public string ResCode { get; set; }
        public string LiveOnNetBanking { get; set; }
        public List<BankResponse> BankData     { get; set; }
    }
}
