![](https://linksync.tech/wallet/0.png)

### **What is a SYNC Wallet ?**

SYNC Wallet is a cold stored wallet which is compatible with all EVM blockchains. It's easy to integrate and provides security and it’s built for privacy.  Using the SYNC wallet, users can move and trade coins/tokens in a safe environment, retaining access to their private key which are stored on their devices.   

![](https://linksync.tech/wallet/1.png)

### Compatible devices ?

SYNC Wallet can be installed on any chipset and is compatible with atmega328, which can be found on Amazon or your local provider with an easy to install using guides in the repository. By choosing to use your own chipset, you know for certain that the device is not compromised by any kind. 

**List of devices:**

*   Leonardo        
*   Mega1284        
*   Mega2560       
*   Micro  
*   NanoR2  
*   NanoR3  
*   UnoR3

### **How to install ?**

You can install the device as the following steps:   

*   Download SYNC Wallet  
*   Install the program  
*   Connect your compatible device.  
*   Chose the right model from the dropdown  
*   Create or import a wallet.

### **How to use ?**

SYNC Wallet is easy to navigate, you can send and receive transactions and you have the option to add custom tokens to a cold storage wallet.

![](https://linksync.tech/wallet/3.png)

### How to send transactions ?

To send a transaction you just need to select a token from the list of tokens in the wallet. Add the receiver address, choose the amount you want to send, approve the transaction with your Personal PIN code.   

![](https://linksync.tech/wallet/6.png)

How to receive a token ?  

To receive a transaction, you need to click on your wallet address on the top, and you need to send the address to the person that’s sending the tokens. (If you are receiving a custom token that is not on our official list, please make sure you add it to view the value using the import token functionality)    

### How to add a custom token?

To add a custom token, go to the import token, set the contract address, if the symbol and decimals do not load automatically. You will need to add them manually. 

![](https://linksync.tech/wallet/4.png)

How to add EVM compatible network ?  

Click on to add a new network set the RPC endpoint, Network Name, Chain ID, Symbol and block explorer.

![](https://linksync.tech/wallet/5.png)

### How to compile from source ?

Prerequisites:  

*   The solution is built on the latest stable release of .Net MAUI, as of the last update on the readme file It’s still in preview, so you will either need to download it as a CLI you can follow the guide on this page for reference of installing .Net MAUI. 
*   [.NET MAUI with .NET CLI (Command Line Interface). (mauiman.dev)](https://mauiman.dev/maui_cli_commandlineinterface.html)

If you don’t want to hassle with installing the CLI version yourself, you can install it from Visual Studio installer

Compiling: 

*   Windows => dotnet publish -f net6.0-windows10.0.19041.0 -c Release 
*   Mac => dotnet build -f:net6.0-maccatalyst -c:Release 
*   Android Device => dotnet publish -f:net6.0-android -c:Release


### How to install from binary ?

You can download latest version from the repository LincSync latest release: 

https://github.com/KristiforMilchev/LInksync-Cold-Storage-Wallet/releases/tag/1.0.0.1

### Important, first time install would require the user to install the certificate to prove the authenticity of the binary to ensure it matches with the latest source code and there are no changes made!


To install the app, it must be signed with a certificate that you already trust. If it isn't, Windows won't let you install the app. You'll be presented with a dialog similar to the following, with the Install button disabled:

![](https://docs.microsoft.com/en-us/dotnet/maui/windows/deployment/media/overview/install-untrusted.png)


Notice that in the previous image, the Publisher was "unknown."

To trust the certificate of app package, perform the following steps:

*	Right-click on the .msix file and choose Properties.
*	Select the Digital Signatures tab.
*	Choose the certificate then press Details.

![](https://docs.microsoft.com/en-us/dotnet/maui/windows/deployment/media/overview/properties-digital-signatures.png)

*	Select View Certificate.
*	Select Install Certificate...
*	Choose Local Machine then select Next.
*	If you're prompted by User Account Control to Do you want to allow this app to make changes to your device?, select Yes
*	In the Certificate Import Wizard window, select Place all certificates in the following store.
*	Select Browse... and then choose the Trusted People store. Select OK to close the dialog.
*	Select Next and then Finish. You should see a dialog that says: The import was successful.

Now, try opening the package file again to install the app. You should see a dialog similar to the following, with the Publisher correctly displayed:

![](https://docs.microsoft.com/en-us/dotnet/maui/windows/deployment/media/overview/install-trusted.png)




Installation on Linux:

Extract the Linux_1.0.0.1 contents into a folder

Add all dependencies -> sudo apt install npm  (Or your choice of a package manager)

chmod +x wallet.sh

./wallet.sh

Installation OSX:

Extract the content of osx_x64

chmod +x LInksync-Cold-Storage-Wallet

./LInksync-Cold-Storage-Wallet


If you find my software beneficial, please consider buying me a coffee. Thanks! 

0x7A310161c2f86d34949a4a136d07A23D23028884 – Crypto
