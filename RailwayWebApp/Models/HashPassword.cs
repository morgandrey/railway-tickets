using System.Security.Cryptography;

namespace RailwayWebApp.Models {
    public static class HashPassword {
        public static byte[] CreateSalt() {
            var rng = new RNGCryptoServiceProvider();
            var salt = new byte[16];
            rng.GetBytes(salt);
            return salt;
        }
        public static byte[] GenerateSaltedHash(byte[] password, byte[] salt) {
            var algorithm = new SHA256Managed();

            var plainTextWithSaltBytes = new byte[password.Length + salt.Length];

            for (int i = 0; i < password.Length; i++) {
                plainTextWithSaltBytes[i] = password[i];
            }
            for (int i = 0; i < salt.Length; i++) {
                plainTextWithSaltBytes[password.Length + i] = salt[i];
            }
            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }
        public static bool CompareByteArrays(byte[] array1, byte[] array2) {
            if (array1.Length != array2.Length) {
                return false;
            }

            for (int i = 0; i < array1.Length; i++) {
                if (array1[i] != array2[i]) {
                    return false;
                }
            }
            return true;
        }
    }
}