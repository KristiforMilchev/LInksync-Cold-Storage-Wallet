using SYNCWallet.Models;

namespace SYNCWallet.Services.Definitions
{
    public interface IUtilities
    {
        public Task<List<NetworkSettings>> SetupNetworks();
        public Task<T> GetRequest<T>(string url);
        public IEnumerable<string> Split(string str, int chunkSize);
        public string BinaryToString(string data);
        public string StringToBinary(string data);
        public decimal SetDecimalPoint(decimal value, int delimeter);
        public decimal ConvertToDex(decimal value, int delimeter);
        public decimal ConvertToBigIntDex(System.Numerics.BigInteger value, int delimeter);

        public decimal ConvertToDexDecimal(decimal number, int decimals);
        public int GetSystemOs();
        public string GetOsSavePath();
        public void OpenErrorView(string msg, int attempts);

        /// <summary>
        /// Truncates a price to the 14th decimal to make it more user friendly
        /// Example 0.00000000275465465654 ->  0.0000000027
        /// Parameters:
        /// <param name="price">The input amount to be truncated, returns the same value if it's less then 14 characters.</param>
        /// </summary>
        public string TruncateDecimals(decimal price);
    }
}
