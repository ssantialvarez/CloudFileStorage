using System.Security.Cryptography;

namespace CloudFileStorage.Helpers
{
    public class PasswordHasher
    {
        private const int SaltSize = 16; // Size of the salt in bytes
        private const int HashSize = 32; // Size of the hash in bytes
        private const int Iterations = 10000; // Number of iterations of the PBKDF2 algorithm

        private static readonly HashAlgorithmName algorithm = HashAlgorithmName.SHA512;

        public string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                algorithm,
                HashSize
            );
            return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
        }

        public bool Verify(string password, string hashedPassword)
        {
            string[] parts = hashedPassword.Split('-');
            if (parts.Length != 2)
            {
                throw new FormatException("Invalid hash format");
            }
            byte[] hash = Convert.FromHexString(parts[0]);
            byte[] salt = Convert.FromHexString(parts[1]);
            byte[] computedHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                algorithm,
                HashSize
            );
            return CryptographicOperations.FixedTimeEquals(hash, computedHash);
        }
    }
}
