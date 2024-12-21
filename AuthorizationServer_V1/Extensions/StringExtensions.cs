using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AuthorizationServer.Extensions
{
    public static class StringExtensions
    {
        public static string PropertyDecryptString(this string value)
        {
            string restoredUnsafeString = Base64UrlDecode(value); //restore back unsafe codes (= + /)
            var encryptor = new Encryptor();
            return encryptor.Decrypt(restoredUnsafeString);
        }

        public static string PropertyEncryptString(this string value)
        {
            var encryptor = new Encryptor();
            return encryptor.Encrypt(value);
        }

        public static string Base64UrlEncode(this string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            var s = Convert.ToBase64String(bytes); // Regular base64 encoder
            s = s.Split('=')[0]; // Remove any trailing '='s
            s = s.Replace('+', '-'); // 62nd char of encoding
            s = s.Replace('/', '_'); // 63rd char of encoding
            return s;
        }

        public static string Base64UrlDecode(this string value)
        {
            var s = value;
            s = s.Replace('-', '+'); // 62nd char of encoding
            s = s.Replace('_', '/'); // 63rd char of encoding
            switch (s.Length % 4) // Pad with trailing '='s
            {
                case 0:
                    break; // No pad chars in this case
                case 2:
                    s += "==";
                    break; // Two pad chars
                case 3:
                    s += "=";
                    break; // One pad char
                default:
                    throw new Exception("Illegal Base64 URL string!");
            }

            var bytes = Convert.FromBase64String(s); // Standard Base64 decoder
            return Encoding.UTF8.GetString(bytes);
        }

        public static string ToBase64(this string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }

        public static string FromBase64(this string value)
        {
            byte[] bytes = Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string ConvertGuidToHex(Guid id)
        {
            // Convert Id to encoded Base64 Url and then to HEX
            string guidBase64 = Convert.ToBase64String(id.ToByteArray());
            var strBase64 = guidBase64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
            return ConvertStringToHex(strBase64).ToLower();
        }

        public static Guid ConvertHexToGuid(string hex)
        {
            var strGuid = ConvertHexToString(hex ?? "");
            var output = strGuid;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding
            switch (output.Length % 4) // Pad with trailing '='s
            {
                case 0:
                    break; // No pad chars in this case
                case 2:
                    output += "==";
                    break; // Two pad chars
                case 3:
                    output += "=";
                    break; // One pad char
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(strGuid), "Illegal base64url string!");
            }
            byte[] guidBase64UrlDecoded = Convert.FromBase64String(output);
            return new Guid(guidBase64UrlDecoded);
        }


        public static string ConvertStringToHex(this string asciiString)
        {
            string hex = "";
            foreach (char c in asciiString)
            {
                int tmp = c;
                hex += string.Format("{0:x2}", Convert.ToUInt32(tmp.ToString()));
            }
            return hex;
        }

        public static string ConvertHexToString(this string hexValue)
        {
            string strValue = "";
            while (hexValue.Length > 0)
            {
                strValue += Convert.ToChar(Convert.ToUInt32(hexValue[..2], 16)).ToString();
                //hexValue = hexValue.Substring(2, hexValue.Length - 2);
                hexValue = hexValue[2..];
            }
            return strValue;
        }

        public static string GetRandomNumericString(int length)
        {
            var random = new Random(DateTime.Now.Second);
            //return new string((char)0, length) + "000000000000" + random.Next(1000, 999999).ToString().Right(length);
            return ("000000000000" + random.Next(1000, 999999).ToString()).Right(length);
        }

        //--https://docs.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(500));

                // Examines the domain part of the email and normalizes it.
                static string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(500));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
