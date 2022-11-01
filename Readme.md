![](https://linksync.tech/wallet/1.png)

Public taskboard:
https://concoctcloud.com/Boards/PublicBoard?projectId=9026&iteration=7037&person=0

##Link to the documentation

https://linksync.developerhub.io/sync-wallet/getting-started

### **What is a SYNC Wallet ?**

SYNC Wallet is a local and hardware wallet depending on your chosen configuratio, which is compatible with all EVM blockchains. It's easy to integrate and provides security and it’s built for privacy.  Using the SYNC wallet, users can move and trade coins/tokens in a safe environment, retaining access to their private key which are stored on either the computer or an external device see compatible devices for more information.   



### Compatible devices ?

SYNC Wallet can be installed on any chipset and is compatible with atmega328, which can be found on Amazon or your local provider with an easy to install using steps in the application under hardware wallet. By choosing to purchase own chipset from a vendor of your chosing, you know for certain that the device is not compromised by any way. 

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
 
*   Create or import a wallet.

### **How to use ?**

SYNC Wallet is easy to navigate, you can send and receive transactions and you have the option to add custom tokens to a cold storage wallet.

![](https://linksync.tech/wallet/3.png)

### How to send transactions ?

To send a transaction you just need to select a token from the list of tokens in the wallet. Add the receiver address, choose the amount you want to send, approve the transaction with your Personal PIN code.   

![](https://linksync.tech/wallet/6.png)

How to receive a token ?  

To receive a transaction, you need to click on your wallet address on the top, and you need to send the address to the person that’s sending the tokens. (If you are receiving a custom token that is not on our official list, please make sure you import it to view the value using the import token functionality)    

### How to add a custom token?

To add a custom token, go to the import token, set the contract address, if the symbol and decimals do not load automatically. You will need to add them manually. 

![](https://linksync.tech/wallet/4.png)

How to add EVM compatible network ?  

Click on to add a new network set the RPC endpoint, Network Name, Chain ID, Symbol and block explorer.

![](https://linksync.tech/wallet/5.png)

### How to compile from source ?

Prerequisites:  

Electron.Net https://github.com/ElectronNET/Electron.NET You will have to compile it under .net core 6 both the CLI and the API, it is advised to install the tool globally follow the instructions in their reposiitory to set it up. Update the paths in the csproj with the paths in case they aren't ../ above this repository file structure.

My version of ArduinoSketchUploader i have re writen the library to work under Mac and Linux, the original maintainer does't answer to any PR requests for a merge so for now use my fork and the mac/lunux branch found here again compile using .net core 6
https://github.com/KristiforMilchev/ArduinoSketchUploader/tree/Netcore6-Linux-Mac-Windows

 
Compiling: 

*   Windows => electronize build /target win
*   Mac => electronize build /target osx
*   Linux => electronize build /target linux

# Debug

If you want to debug the application make sure to comment out
//Electron.ReadAuth(); in Program.cs inside the Presentation folder, it will force the application to run in the browser rather then the electron wrapper. Or just attach to the process of electron if you want to do it in the app itself, just remember not to open PR with this line commented.


### How to install from binary ?

You can download latest version from the repository LincSync latest release: 

https://github.com/KristiforMilchev/LInksync-Cold-Storage-Wallet/releases/tag/1.0.0.3
````
Installing on Windows
````
Download the win-x64 from the link above, extract the archive to a folder of your chosing and install the app from LInksync-Cold-Storage-Wallet.exe
````
Installation on Linux:
````
Extract the Linux_1.0.0.3 contents into a folder

Optional you can create a desktop link:

sudo nano /usr/share/applications/SYNCWallet.desktop
````
[Desktop Entry]
Version=1.0.3
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
Extract the contents of the mac.zip file.

Either install it from the provided DMG file or open the mac folder and start the application from there.
````


If you find my software beneficial, please consider buying me a coffee. Thanks! 

0x09b26ff91DfB5b959908fd7f6cEe73e19FA75817 – Crypto

SYNC Wallet © 2022 by Kristifor Milchev is licensed under CC BY-NC-SA 4.0 
https://creativecommons.org/licenses/by-nc-sa/4.0/?ref=chooser-v1
