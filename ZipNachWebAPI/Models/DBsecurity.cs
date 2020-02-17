using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Encryption;
using System.Data;
using ImageUploadWCF;
using System.Security.Cryptography;
using System.Text;

namespace ZipNachWebAPI
{
    public class DBsecurity
    {
        private const string mysecurityKey = "Yoeki123";
        public static string EncryptMD5(string TextToEncrypt)
        {
            byte[] MyEncryptedArray = UTF8Encoding.UTF8.GetBytes(TextToEncrypt);
            MD5CryptoServiceProvider MyMD5CryptoService = new   MD5CryptoServiceProvider();
            byte[] MysecurityKeyArray = MyMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(mysecurityKey));
            MyMD5CryptoService.Clear();
            var MyTripleDESCryptoService = new TripleDESCryptoServiceProvider();
            MyTripleDESCryptoService.Key = MysecurityKeyArray;
            MyTripleDESCryptoService.Mode = CipherMode.ECB;
            MyTripleDESCryptoService.Padding = PaddingMode.PKCS7;
            var MyCrytpoTransform = MyTripleDESCryptoService.CreateEncryptor();
            byte[] MyresultArray = MyCrytpoTransform.TransformFinalBlock(MyEncryptedArray, 0,MyEncryptedArray.Length);
            MyTripleDESCryptoService.Clear();
            return Convert.ToBase64String(MyresultArray, 0,MyresultArray.Length);
        }
        public static string DecryptMD5(string TextToDecrypt)
        {
            byte[] MyDecryptArray = Convert.FromBase64String(TextToDecrypt);
            MD5CryptoServiceProvider MyMD5CryptoService = new MD5CryptoServiceProvider();
            byte[] MysecurityKeyArray = MyMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(mysecurityKey));
            MyMD5CryptoService.Clear();
            var MyTripleDESCryptoService = new  TripleDESCryptoServiceProvider();
            MyTripleDESCryptoService.Key = MysecurityKeyArray;
            MyTripleDESCryptoService.Mode = CipherMode.ECB;
            MyTripleDESCryptoService.Padding = PaddingMode.PKCS7;
            var MyCrytpoTransform = MyTripleDESCryptoService.CreateDecryptor();
            byte[] MyresultArray = MyCrytpoTransform.TransformFinalBlock(MyDecryptArray, 0,MyDecryptArray.Length);
            MyTripleDESCryptoService.Clear();
            return UTF8Encoding.UTF8.GetString(MyresultArray);
        }
        // Used to Decrypt the Passed Value on the Basis of a Fixed Key.
        public static string Decrypt(string Value)
        {
            string strValue1;
            string strValue2;
            string[] sp = Value.Split('|');
            strValue1 = sp[0];
            strValue2 = sp[1];
            string[] arrValue1 = strValue1.Split(' ');
            string[] arrValue2 = strValue2.Split(' ');
            if (arrValue1.Length > 0)
            {
                strValue1 = "";
                for (int i = 1; i <= arrValue1.Length; i++)
                {
                    strValue1 = strValue1 + arrValue1[i - 1];
                    if (i < arrValue1.Length)
                    {
                        strValue1 = strValue1 + "+";
                    }
                }
            }
            if (arrValue2.Length > 0)
            {
                strValue2 = "";
                for (int i = 1; i <= arrValue2.Length; i++)
                {
                    strValue2 = strValue2 + arrValue2[i - 1];
                    if (i < arrValue2.Length)
                    {
                        strValue2 = strValue2 + "+";
                    }
                }
            }
            EncryptionAlgorithm alg = EncryptionAlgorithm.Rc2;
            byte[] IV = Convert.FromBase64String(strValue2);
            Decryptor dec = new Decryptor(alg, IV);
            string strValue = dec.Decrypt(strValue1, "abcd12345");
            return strValue;
        }

        // Used to Decrypt the Passed Value on the Basis of a dynamically Supplied Key.
        public static string Decrypt(string Value, string key)
        {
            string strValue1;
            string strValue2;
            string[] sp = Value.Split('|');
            strValue1 = sp[0];
            strValue2 = sp[1];
            string[] arrValue1 = strValue1.Split(' ');
            string[] arrValue2 = strValue2.Split(' ');
            if (arrValue1.Length > 0)
            {
                strValue1 = "";
                for (int i = 1; i <= arrValue1.Length; i++)
                {
                    strValue1 = strValue1 + arrValue1[i - 1];
                    if (i < arrValue1.Length)
                    {
                        strValue1 = strValue1 + "+";
                    }
                }
            }
            if (arrValue2.Length > 0)
            {
                strValue2 = "";
                for (int i = 1; i <= arrValue2.Length; i++)
                {
                    strValue2 = strValue2 + arrValue2[i - 1];
                    if (i < arrValue2.Length)
                    {
                        strValue2 = strValue2 + "+";
                    }
                }
            }
            EncryptionAlgorithm alg = EncryptionAlgorithm.Rc2;
            byte[] IV = Convert.FromBase64String(strValue2);
            Decryptor dec = new Decryptor(alg, IV);
            string strValue = dec.Decrypt(strValue1, key);
            return strValue;
        }

        // Used to Encrypt the Passed Value on the Basis of a Fixed Key.
        public static string Encrypt(string Value)
        {
            EncryptionAlgorithm alg = EncryptionAlgorithm.Rc2;
            Encryptor en = new Encryptor(alg, "abcd12345"); // If You Lost the Key you would not be able to Decrypt the encrypted Value.
            string EncValue = en.Encrypt(Value);
            string IVserverName = Convert.ToBase64String(en.IV);
            string strEncValue = EncValue + "|" + IVserverName;
            return strEncValue;
        }

        // Used to Encrypt the Passed Value on the Basis of a dynamically Supplied Key.
        public static string Encrypt(string value, ref string key)
        {
            EncryptionAlgorithm alg = EncryptionAlgorithm.Rc2;
            key = Global.CreateRandomCode(9);
            Encryptor en = new Encryptor(alg, key); // This key need to be moved to database
            string EncValue = en.Encrypt(value);            // If You Lost the Key you would not be able to Decrypt the encrypted Value.
            string IVserverName = Convert.ToBase64String(en.IV);
            string strEncValue = EncValue + "|" + IVserverName;
            return strEncValue;
        }

        // Used to Authenticate User and Load the same in to the Current Session.
        public static Boolean IsAutheticatedUser(DataSet dsUser, string strGivenPassword)
        {
            string strDbPassword = dsUser.Tables[0].Rows[0]["UserPassword"].ToString();
            string strDbPasswordKey = dsUser.Tables[0].Rows[0]["PasswordKey"].ToString();

            strDbPassword = Decrypt(strDbPassword, strDbPasswordKey);
            if (strDbPassword.CompareTo(strGivenPassword) != 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// The method create a Base64 encoded string from a normal string.
        /// </summary>
        /// <param name="toEncode">toEncode</param>
        /// <returns>The Base64 encoded string.</returns>
        public static string Base64Encode(string toEncode)
        {
            byte[] encData_byte = new byte[toEncode.Length];
            encData_byte = System.Text.Encoding.UTF8.GetBytes(toEncode);
            string encodedData = Convert.ToBase64String(encData_byte);
            return encodedData;
        }

        /// <summary>
        /// The method to decode your Base64 string.
        /// </summary>
        /// <param name="encodedData">encodedData</param>
        /// <returns>A String containing the results of decoding the specified sequence</returns>
        public static string Base64Decode(string encodedData)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();

            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }

    }
}