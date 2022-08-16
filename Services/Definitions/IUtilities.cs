using SYNCWallet.Models;

namespace SYNCWallet.Services.Definitions
{
    public interface IUtilities
    {
        /// <summary>
        /// Returns a list of NetworkSettings, executes a query against the main github repository 
        /// retruves the officially supported networks and then checks if there are any local networks returns the combined result
        /// </summary>
        public Task<List<NetworkSettings>> SetupNetworks();

        /// <summary>
        /// Returns the result of a Generic GET request as a model of T
        /// Parameters:
        /// <paramref name="url"/>
        /// <param name="url">The weboage url to get, must be a JSON that matches the T properties.</param>
        /// </summary>
        public Task<T> GetRequest<T>(string url);

        /// <summary>
        /// Splits a string to equal chunks
        /// Parameters:
        /// <paramref name="str"/>
        /// <param name="str">The original string that has to be split to equal chunks</param>
        /// <paramref name="chunkSize"/>
        /// <param name="chunkSize">The number of piaces that the string has to be broken to</param>
        /// </summary>
        public IEnumerable<string> Split(string str, int chunkSize);

        /// <summary>
        /// Converts a binary to string
        /// Example: 101-> 1
        /// Parameters:
        /// <paramref name="data"/>
        /// <param name="data">Any text UTF 8 encoded</param>
        /// </summary>
        public string BinaryToString(string data);

        /// <summary>
        /// Converts a string to binary
        /// Example: 1-> 101
        /// Parameters:
        /// <param name="data">Any text UTF 8 encoded</param>
        /// </summary>
        public string StringToBinary(string data);


        /// <summary>
        /// Multiplies the value to the 10 of the power of the delimiter, 
        /// used for interaction with solidity smart contracts.
        /// Parameters:
        /// <paramref name="value"/>
        /// <param name="value">The value that has to be multiplied</param>
        /// <paramref name="delimeter"/>
        /// <param name="delimeter">the number to rise 10 to the power over</param>
        /// </summary>
        public decimal SetDecimalPoint(decimal value, int delimeter);

        /// <summary>
        /// Divides the value to the 10 of the power of the delimiter, 
        /// used for interaction with solidity smart contracts.
        /// Parameters:
        /// <paramref name="value"/>
        /// <param name="value">The value that has to be multiplied</param>
        /// <paramref name="delimeter"/>
        /// <param name="delimeter">the number to rise 10 to the power over</param>
        /// </summary>
        public decimal ConvertToDex(decimal value, int delimeter);


        /// <summary>
        /// Multiplies the value to the 10 of the power of the delimiter, 
        /// used for interaction with solidity smart contracts.
        /// Parameters:
        /// <paramref name="value"/>
        /// <param name="value">The value that has to be multiplied BigInteger</param>
        /// <paramref name="delimeter"/>
        /// <param name="delimeter">the number to rise 10 to the power over</param>
        /// </summary>
        public decimal ConvertToBigIntDex(System.Numerics.BigInteger value, int delimeter);


        /// <summary>
        /// Divides the value to the 10 of the power of the delimiter, 
        /// used for interaction with solidity smart contracts.
        /// Parameters:
        /// <paramref name="value"/>
        /// <param name="value">The value that has to be multiplied</param>
        /// <paramref name="delimeter"/>
        /// <param name="delimeter">the number to rise 10 to the power over</param>
        /// </summary>
        public decimal ConvertToDexDecimal(decimal number, int decimals);

        /// <summary>
        /// Returns the active device operating system
        /// </summary>
        public int GetSystemOs();


        /// <summary>
        /// Returns the device files safe path, not onwed by system OS for downloads and local settings such as imported tokens, imported networks.
        /// </summary>
        public string GetOsSavePath();

        /// <summary>
        /// Opens a popup window that shows an alert
        /// Parameters:
        /// <paramref name="msg"/>
        /// <param name="msg">The message that has to be displayed on the screen</param>
        /// <paramref name="attempts"/>
        /// <param name="attempts">Obsolete not being utilized, part of an old API, to be removed next V</param>
        /// </summary>
        public void OpenErrorView(string msg, int attempts);

        /// <summary>
        /// Truncates a price to the 14th decimal to make it more user friendly
        /// Example 0.00000000275465465654 ->  0.0000000027
        /// Parameters:
        /// <paramref name="price"/>
        /// <param name="price">The input amount to be truncated, returns the same value if it's less then 14 characters.</param>
        /// </summary>
        public string TruncateDecimals(decimal price);
    }
}
