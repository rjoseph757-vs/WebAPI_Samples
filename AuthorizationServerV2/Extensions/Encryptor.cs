//--https://www.devtrends.co.uk/blog/hashing-encryption-and-random-in-asp.net-core
using Microsoft.AspNetCore.DataProtection;
using System.Security.Principal;

#nullable disable

namespace AuthorizationServer.Extensions
{
    public class Encryptor
    {
        private readonly IDataProtector _protector;

        // For Application-wide use
        private const string MachineKeyPurpose = "TMG:Username:{0}";
        private const string Anonymous = "<anonymous>";

        public Encryptor(IDataProtectionProvider provider)
        {
            //_protector = provider.CreateProtector(GetType().FullName);
            string purpose = GetMachineKeyPurpose(Thread.CurrentPrincipal);
            _protector = provider.CreateProtector(purpose);
        }

        public Encryptor()
        {
        }

        public static string GetMachineKeyPurpose(IPrincipal user)
        {
            return user.Identity.IsAuthenticated
                ? string.Format(MachineKeyPurpose,
                user.Identity.Name)
                : string.Format(MachineKeyPurpose,
                Anonymous);
        }

        public string Encrypt(string plaintext)
        {
            return _protector.Protect(plaintext);
        }

        public string Decrypt(string encryptedText)
        {
            return _protector.Unprotect(encryptedText);
        }
    }
}
