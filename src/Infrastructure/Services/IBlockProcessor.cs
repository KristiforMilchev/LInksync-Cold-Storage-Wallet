namespace SYNCWallet.Services.Definitions
{
    public interface IBlockProcessor
    {
        public void BeginProcessing();
        public void Dispose();
    }
}