using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZipNachWebAPI.Models
{
    public class DataChkESign
    {
        public string AppID { get; set; }
        public string MdtID { get; set; }
        public string MerchantKey { get; set; }
    }
    public class QRImage
    {
        public string MandateID { get; set; }
        public string base64Image { get; set; }

    }
    public class GetMandateReq
    {
        public string AppID { get; set; }
        public string MdtID { get; set; }
        public string MerchantKey { get; set; }
        
    }
    public class CustomerModel
    {
        public string MdtID { get; set; }
        public string AppID { get; set; }
        public string MerchantKey { get; set; }
        public string ScannedImage { get; set; }
         public string RefrenceNo { get; set; }
    }
    public class ValidateAccount
    {
       // public string Password { get; set; }
        public string AppID { get; set; }
        public string MandateMode { get; set; }
        public string SpBankCode { get; set; }
        public string UTLSCode { get; set; }
        public string MDate { get; set; }
        public string Cust1 { get; set; }
        public string Cust2 { get; set; }
        public string Cust3 { get; set; }
        public string MdtID { get; set; }
        public string DType { get; set; }
        public string Frequency { get; set; }
        
         public string MerchantKey { get; set; }
        public string TDebit { get; set; }
        public string BankAc { get; set; }
        public string BankName { get; set; }
        public string IFSC { get; set; }
        public string MICR { get; set; }
        public string Amt { get; set; }
        public string Ref1 { get; set; }
        public string Ref2 { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string PFrom { get; set; }
        public string PTo { get; set; }
        public string MType { get; set; }
        public string UntlCancel { get; set; }
        public string UMRN { get; set; }
        public string IsAggregator { get; set; }
        public string SubMerchantId { get; set; }
        public string CategoryCode { get; set; }
    }
    public class MandateUpdateAsper
    {
        public string MdtID { get; set; }
        public string Type { get; set; }
        public string AppID { get; set; }
        public string MerchantKey { get; set; }

    }
    public class UpdateIsPhysical
    {
        public string AppID { get; set; }
        public string eMandate { get; set; }
        public string eMandateType { get; set; }
        public string MdtID { get; set; }
        public string MerchantKey { get; set; }
    }
  
    public class GetSubmitMandateReq
    {
        public string AppID { get; set; }
        public string MdtID { get; set; }
        public string MerchantKey { get; set; }
    }

    public class ScanQRImage
    {
        public string MandateID { get; set; }
        public string AppId { get; set; }
        public string EnitityMarchantKey { get; set; }
        public string ScanImage { get; set; }
      

    }

}
