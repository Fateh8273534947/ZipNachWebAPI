using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZipNachWebAPI.Models
{
    public class SavePresentmentDetails
    {
        public string AppID { get; set; }
        public string MerchantKey { get; set; }
        public string ActivityID { get; set; }
        public string FileNo { get; set; }
        public string BankCode { get; set; }
        public string PresentmentDate { get; set; }
        public string RequestType { get; set; }
        public string UMRNData { get; set; }

    }
    public class HistorySavePresentmentDetails
    {
        public string AppID { get; set; }
        public string MerchantKey { get; set; }
        public string ActivityID { get; set; }
        public string FileNo { get; set; }
        public string BankCode { get; set; }
        public string PresentmentDate { get; set; }
        public string RequestType { get; set; }
        public string UMRNData { get; set; }
        public string UMRN { get; set; }

    }
    public class loginResponse
    {
        public string ResDescription { get; set; }
        public string Status { get; set; }
        public string ActivityID { get; set; }
        public string ResCode { get; set; }
        public string FileNo { get; set; }
        public string BankCode { get; set; }
        public string CreatedDateTime { get; set; }
        public string LastEditDateTime { get; set; }
        public string TotalRecords { get; set; }
        public string TotalAmt { get; set; }
        public string FileStatus { get; set; }
        public string UMRNData { get; set; }
        
    }
    public class HistoryloginResponse
    {
        public string UMRN { get; set; }
        public string TotalRecords { get; set; }
        public string TotalAmt { get; set; }
        public string Status { get; set; }
        public string ResCode { get; set; }
        public string ResDescription { get; set; }
        public string UMRNData { get; set; }
        
    }
}