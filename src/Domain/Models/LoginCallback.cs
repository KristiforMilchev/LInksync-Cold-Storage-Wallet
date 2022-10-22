 
namespace SYNCWallet.Models
{
    public delegate void LoginCallback(bool status);
    public delegate void TriggerLoader(string status);
    public delegate void ErrorCallback(string title, string msg);
    public delegate void WidgetLoadedGeneric<T>(T data, string contract, bool isGlobal);
    public delegate void IncomingBlock();

    

    

}
