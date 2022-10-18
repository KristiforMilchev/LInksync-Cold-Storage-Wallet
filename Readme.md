![](https://linksync.tech/wallet/1.png)

Public taskboard:
https://concoctcloud.com/Boards/PublicBoard?projectId=9026&iteration=7037&person=0

### **What is a SYNC Wallet ?**

SYNC Wallet is a hot and cold stored wallet depending on your chosen configuratio, which is compatible with all EVM blockchains. It's easy to integrate and provides security and it’s built for privacy.  Using the SYNC wallet, users can move and trade coins/tokens in a safe environment, retaining access to their private key which are stored on either the computer or an external device see compatible devices for more information (Cold storage is currently only supported under Windows).   



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

 
Compiling: 

*   Windows => electronize build /target win
*   Mac => electronize build /target osx
*   Linux => electronize build /target linux



### How to install from binary ?

You can download latest version from the repository LincSync latest release: 

https://github.com/KristiforMilchev/LInksync-Cold-Storage-Wallet/releases/tag/1.0.0.2
````
Installing on Windows
````
Download the win-x64 from the link above, extract the archive to a folder of your chosing and install the app from LInksync-Cold-Storage-Wallet.exe
````
Installation on Linux:
````
Extract the Linux_1.0.0.2 contents into a folder

Optional you can create a desktop link:

sudo nano /usr/share/applications/SYNCWallet.desktop
````
[Desktop Entry]
Version=1.0.2
Name=SYNCWallet
Comment=
Exec=/yourfolderpath/l-inksync--cold--storage--wallet
Icon=/yourfolderpath/icon-linux.png
Terminal=false
Type=Application
Categories=Utility;Application;
````

Click save and you should have an icon for the app in your desktop manager.

````
Mac OS is only supported by building from source OSX. No one on the team has Mac os to produce a build, thanks apple!
 ````


If you find my software beneficial, please consider buying me a coffee. Thanks! 

0x09b26ff91DfB5b959908fd7f6cEe73e19FA75817 – Crypto
