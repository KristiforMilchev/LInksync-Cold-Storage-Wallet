 // Unpkg imports
const Web3Modal = window.Web3Modal.default;
const WalletConnectProvider = window.WalletConnectProvider.default;
const EvmChains = window.evmChains;
 
// Web3modal instance
let web3Modal

// Chosen wallet provider given by the dialog window
let provider;
 

// Address of the selected account
let selectedAccount;
var web3;

var contractAddress = '0x9988f8D0346D15d511aF8dFa65ddFeceA6157b92'; //'0xf6a22b0593df74f218027a2d8b7953c9b4542aa1'; // Production Contract  ---- //testnet Contract
var reciever = '0x94bA08739e5FD3f99E531383f31Ab9B22e961D21'; //'0x8b0dCf72f667313a0750602d01a37b74218D3Bce'; //Testnet wallet
var currentChain = "0x4";

var activeChain = 97;


function UpdateNetwork(cuurentNetwork, contract) {
    activeChain = cuurentNetwork;
    currentChain = "0x" + cuurentNetwork.toString(16);
    contractAddress = contract;
}
 


function GetChain() {
    return activeChain;
}

function SwitchProveiderBUSD() {

    switch (activeChain) {

        case 80001:
            
            contractAddress = '0x03BD7398BA6c266900c94BCbe231D27c1e67A919'; //Testnet BUSD Matic
            reciever = '0x94bA08739e5FD3f99E531383f31Ab9B22e961D21'
            break;
        case 97:
            contractAddress = '0x78867BbEeF44f2326bF8DDd1941a4439382EF2A7'; //Testnet BUSD Bsc
            reciever = '0x94bA08739e5FD3f99E531383f31Ab9B22e961D21'
            break;
        case 4:
            contractAddress = '0x502b77Aa21d480f5B6fAFe7954985245606a5812'; //Testnet BUSD Ribkey
            reciever = '0x94bA08739e5FD3f99E531383f31Ab9B22e961D21'
            break;
    }   
 }
 
function SwitchProveiderUSDT() {

    switch (activeChain) {

        case 80001:

            contractAddress = '0xeaBa1eF13899AcE527c6a18F5b67B653133a8cBE'; //Testnet USDT Matic
            reciever = '0x94bA08739e5FD3f99E531383f31Ab9B22e961D21'
            break;
        case 97:
            contractAddress = '0x7ef95a0FEE0Dd31b22626fA2e10Ee6A223F8a684'; //Testnet USDT Bsc
            reciever = '0x94bA08739e5FD3f99E531383f31Ab9B22e961D21'
            break;
        case 4:
            contractAddress = '0x9988f8D0346D15d511aF8dFa65ddFeceA6157b92'; //Testnet USDT Ribkey
            reciever = '0x94bA08739e5FD3f99E531383f31Ab9B22e961D21'
            break;
    }
 }

function SwitchProveiderUSDC() {

    switch (activeChain) {

        case 80001:

            contractAddress = '0x502b77Aa21d480f5B6fAFe7954985245606a5812'; //Testnet USDC Matic
            reciever = '0x94bA08739e5FD3f99E531383f31Ab9B22e961D21'
            break;
        case 97:
            contractAddress = '0x590084142d51f410dbdB87C1Eb2f2dA24EF883eD'; //Testnet USDC Bsc
            reciever = '0x94bA08739e5FD3f99E531383f31Ab9B22e961D21'
            break;
        case 4:
            contractAddress = '0xD58871Adcb2b93b16DBd1C9305545a055c0F5Fb1'; //Testnet USDC Ribkey
            reciever = '0x94bA08739e5FD3f99E531383f31Ab9B22e961D21'
            break;
    }
}


 

/**
 * Get the current active payment contract
 */
function GetContract() {
    return contractAddress;
}

/**
 * Get the current active user account
 */
function GetAccount() {
    return selectedAccount;
}


/**
 * Returns the payable address defined in the system config files
 */
function GetReceiver() {
    return reciever;
}


/**
 * Forces the user to change network in case its on a different network then the system defined
 */
function ChangeNetwork() {
 
        ethereum.request({ method: 'wallet_switchEthereumChain', params: [{ chainId: currentChain }] }).catch(e => {
            (e.message.includes("Unrecognized chain ID"))
            {
                 const swalWithBootstrapButtons = Swal.mixin({
                    customClass: {
                        confirmButton: 'btn btn-danger',
                        cancelButton: 'mr-2 btn btn-error'
                    },
                    buttonsStyling: false,
                })
                swalWithBootstrapButtons.fire(
                    'Error!',
                    "Please setup the network in your wallet first",
                    'Failed'
                ).then((result) => {
                    if (result.value) {
                        window.location.reload();
                    }
                    else if (result.dismiss === Swal.DismissReason.cancel) {

                    }
                });
            }
        });

   

} 


/**
 * Returns back the network session provider for external ETH requests.
 */
function GetProvider() {
    return provider;
}

/**
 * Setup the orchestra
 */
function init() {

    console.log("Initializing example");
    console.log("WalletConnectProvider is", WalletConnectProvider);
 
    // Tell Web3modal what providers we have available.
    // Built-in web browser provider (only one can exist as a time)
    // like MetaMask, Brave or Opera is added automatically by Web3modal
    const providerOptions = {
        walletconnect: {
            package: WalletConnectProvider,
            options: {
                // Mikko's test key - don't copy as your mileage may vary
                infuraId: "24e067d0dc7847f78b5a99a82f1cc38e",
            }
        },

    };

    web3Modal = new Web3Modal({

        cacheProvider: false, // optional
        providerOptions, // required
    });



}


/**
 * Kick in the UI action after Web3modal dialog has chosen a provider
 */
async function fetchAccountData() {

    // Get a Web3 instance for the wallet
    web3 = new Web3(provider);

    console.log("Web3 instance is", web3);

    // Get connected chain id from Ethereum node
    const chainId = activeChain;
    // Load chain information over an HTTP API
    const chainData = await EvmChains.getChain(chainId);

    // Get list of accounts of the connected wallet
    const accounts = await web3.eth.getAccounts();

    // MetaMask does not give you all accounts, only the selected account
    console.log("Got accounts", accounts);
    selectedAccount = accounts[0];
     
    localStorage.setItem('account', selectedAccount);
     

}



/**
 * Fetch account data for UI when
 * - User switches accounts in wallet
 * - User switches networks in wallet
 * - User connects wallet initially
 */
async function refreshAccountData() {


    // Disable button while UI is loading.
    // fetchAccountData() will take a while as it communicates
    // with Ethereum node via JSON-RPC and loads chain data
    // over an API call.
    await fetchAccountData(provider);
 
}


/**
 * Connect wallet button pressed.
 */
async function onConnect() {

    console.log("Opening a dialog", web3Modal);
    try {
        provider = await web3Modal.connect();
    } catch (e) {
        console.log("Could not get a wallet connection", e);
        return;
    }
    VerifySupported(provider.chainId);
    ChangeNetwork();
        
    // Subscribe to accounts change
    provider.on("accountsChanged", (accounts) => {
        IgnoreConnection();
        fetchAccountData();
    });

    // Subscribe to chainId change
    provider.on("chainChanged", (chainId) => {
        IgnoreConnection();
        fetchAccountData();
    });

    // Subscribe to networkId change
    provider.on("networkChanged", (networkId) => {
        IgnoreConnection();
        var result = VerifySupported(networkId);
        if (result) {
            ChangeNetwork();
            fetchAccountData();
        }
        else
            ChangeNetwork();
    });
    
    await refreshAccountData();
}

function VerifySupported(netowrkId) {
    var id = parseInt(netowrkId);
    var res = false;
    switch (id) {
        case 80001:
            currentChain = "0x13881";
            activeChain = 80001;
            res = true;
            document.getElementById("imgNetwork").setAttribute("src", "/images/tokenLogos/polygon.jpg");
            document.getElementById("network").innerHTML = "POLYGON";
            break;
        case 97:
            currentChain = "0x61";
            activeChain = 97;
            res = true;
            document.getElementById("imgNetwork").setAttribute("src", "/images/tokenLogos/bsc.png");
            document.getElementById("network").innerHTML = "BSC";
            break;

        case 4:
            currentChain = "0x4";
            activeChain = 4;
            res = true;
            document.getElementById("imgNetwork").setAttribute("src", "/images/tokenLogos/eth.jpg");
            document.getElementById("network").innerHTML = "ETH";
            break;
    }
    return res;
}

/**
 * Disconnect wallet button pressed.
 */
async function onDisconnect() {

    if (provider.close) {
        await provider.close();

        // If the cached provider is not cleared,
        // WalletConnect will default to the existing session
        // and does not allow to re-scan the QR code with a new wallet.
        // Depending on your use case you may want or want not his behavir.
        provider = null;

    }
    window.location.reload();
     
}


const deleteAllCookies = () => {
    const cookies = document.cookie.split(";");

    for (const cookie of cookies) {
        const eqPos = cookie.indexOf("=");
        const name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
        document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 GMT";
    }
}



$(document).ready(function () {
    init();

    localStorage.removeItem("walletconnect");


});


async function signMessage(message) {
    try {
        const from = selectedAccount;
        console.log('from : ' + from);
        const msg = `0x${bops.from(message, 'utf8').toString('hex')}`;
        console.log('msg : ' + msg);
        //const sign = await ethereum.request({
        //    method: 'personal_sign',
        //    params: [msg, from, 'Random text'],
        //});
        var sign = await web3.currentProvider
            .request({
                method: 'personal_sign',
                params: [msg, selectedAccount, 'Random text'],

            });
         console.log('sign : ' + sign);
        return {
            signature: sign,
            message: msg
        };
    } catch (err) {
        console.error(err);
        return null;
    }
}

async function Disconnect() {
    tx = "";
    await web3Modal.clearCachedProvider();
    window.location.reload();
 

}



var ABI = [{ "inputs": [], "stateMutability": "nonpayable", "type": "constructor" }, { "anonymous": false, "inputs": [{ "indexed": true, "internalType": "address", "name": "owner", "type": "address" }, { "indexed": true, "internalType": "address", "name": "spender", "type": "address" }, { "indexed": false, "internalType": "uint256", "name": "value", "type": "uint256" }], "name": "Approval", "type": "event" }, { "anonymous": false, "inputs": [{ "indexed": false, "internalType": "bool", "name": "enabled", "type": "bool" }], "name": "BuyBackEnabledUpdated", "type": "event" }, { "anonymous": false, "inputs": [{ "indexed": true, "internalType": "address", "name": "previousOwner", "type": "address" }, { "indexed": true, "internalType": "address", "name": "newOwner", "type": "address" }], "name": "OwnershipTransferred", "type": "event" }, { "anonymous": false, "inputs": [{ "indexed": false, "internalType": "uint256", "name": "tokenAmount", "type": "uint256" }], "name": "RewardLiquidityProviders", "type": "event" }, { "anonymous": false, "inputs": [{ "indexed": false, "internalType": "uint256", "name": "tokensSwapped", "type": "uint256" }, { "indexed": false, "internalType": "uint256", "name": "ethReceived", "type": "uint256" }, { "indexed": false, "internalType": "uint256", "name": "tokensIntoLiqudity", "type": "uint256" }], "name": "SwapAndLiquify", "type": "event" }, { "anonymous": false, "inputs": [{ "indexed": false, "internalType": "bool", "name": "enabled", "type": "bool" }], "name": "SwapAndLiquifyEnabledUpdated", "type": "event" }, { "anonymous": false, "inputs": [{ "indexed": false, "internalType": "uint256", "name": "amountIn", "type": "uint256" }, { "indexed": false, "internalType": "address[]", "name": "path", "type": "address[]" }], "name": "SwapETHForTokens", "type": "event" }, { "anonymous": false, "inputs": [{ "indexed": false, "internalType": "uint256", "name": "amountIn", "type": "uint256" }, { "indexed": false, "internalType": "address[]", "name": "path", "type": "address[]" }], "name": "SwapTokensForETH", "type": "event" }, { "anonymous": false, "inputs": [{ "indexed": true, "internalType": "address", "name": "from", "type": "address" }, { "indexed": true, "internalType": "address", "name": "to", "type": "address" }, { "indexed": false, "internalType": "uint256", "name": "value", "type": "uint256" }], "name": "Transfer", "type": "event" }, { "inputs": [], "name": "_liquidityFee", "outputs": [{ "internalType": "uint256", "name": "", "type": "uint256" }], "stateMutability": "view", "type": "function" }, { "inputs": [], "name": "_maxTxAmount", "outputs": [{ "internalType": "uint256", "name": "", "type": "uint256" }], "stateMutability": "view", "type": "function" }, { "inputs": [], "name": "_taxFee", "outputs": [{ "internalType": "uint256", "name": "", "type": "uint256" }], "stateMutability": "view", "type": "function" }, { "inputs": [{ "internalType": "address", "name": "owner", "type": "address" }, { "internalType": "address", "name": "spender", "type": "address" }], "name": "allowance", "outputs": [{ "internalType": "uint256", "name": "", "type": "uint256" }], "stateMutability": "view", "type": "function" }, { "inputs": [{ "internalType": "address", "name": "spender", "type": "address" }, { "internalType": "uint256", "name": "amount", "type": "uint256" }], "name": "approve", "outputs": [{ "internalType": "bool", "name": "", "type": "bool" }], "stateMutability": "nonpayable", "type": "function" }, { "inputs": [{ "internalType": "address", "name": "account", "type": "address" }], "name": "balanceOf", "outputs": [{ "internalType": "uint256", "name": "", "type": "uint256" }], "stateMutability": "view", "type": "function" }, { "inputs": [], "name": "buyBackEnabled", "outputs": [{ "internalType": "bool", "name": "", "type": "bool" }], "stateMutability": "view", "type": "function" }, { "inputs": [], "name": "buyBackUpperLimitAmount", "outputs": [{ "internalType": "uint256", "name": "", "type": "uint256" }], "stateMutability": "view", "type": "function" }, { "inputs": [], "name": "deadAddress", "outputs": [{ "internalType": "address", "name": "", "type": "address" }], "stateMutability": "view", "type": "function" }, { "inputs": [], "name": "decimals", "outputs": [{ "internalType": "uint8", "name": "", "type": "uint8" }], "stateMutability": "view", "type": "function" }, { "inputs": [{ "internalType": "address", "name": "spender", "type": "address" }, { "internalType": "uint256", "name": "subtractedValue", "type": "uint256" }], "name": "decreaseAllowance", "outputs": [{ "internalType": "bool", "name": "", "type": "bool" }], "stateMutability": "nonpayable", "type": "function" }, { "inputs": [{ "internalType": "uint256", "name": "tAmount", "type": "uint256" }], "name": "deliver", "outputs": [], "stateMutability": "nonpayable", "type": "function" }, { "inputs": [{ "internalType": "address", "name": "account", "type": "address" }], "name": "excludeFromFee", "outputs": [], "stateMutability": "nonpayable", "type": "function" }, { "inputs": [{ "internalType": "address", "name": "account", "type": "address" }], "name": "excludeFromReward", "outputs": [], "stateMutability": "nonpayable", "type": "function" }, { "inputs": [], "name": "getTime", "outputs": [{ "internalType": "uint256", "name": "", "type": "uint256" }], "stateMutability": "view", "type": "function" }, { "inputs": [], "name": "getUnlockTime", "outputs": [{ "internalType": "uint256", "name": "", "type": "uint256" }], "stateMutability": "view", "type": "function" }, { "inputs": [{ "internalType": "address", "name": "account", "type": "address" }], "name": "includeInFee", "outputs": [], "stateMutability": "nonpayable", "type": "function" }, { "inputs": [{ "internalType": "address", "name": "account", "type": "address" }], "name": "includeInReward", "outputs": [], "stateMutability": "nonpayable", "type": "function" }, { "inputs": [{ "internalType": "address", "name": "spender", "type": "address" }, { "internalType": "uint256", "name": "addedValue", "type": "uint256" }], "name": "increaseAllowance", "outputs": [{ "internalType": "bool", "name": "", "type": "bool" }], "stateMutability": "nonpayable", "type": "function" }, { "inputs": [{ "internalType": "address", "name": "account", "type": "address" }], "name": "isExcludedFromFee", "outputs": [{ "internalType": "bool", "name": "", "type": "bool" }], "stateMutability": "view", "type": "function" }, { "inputs": [{ "internalType": "address", "name": "account", "type": "address" }], "name": "isExcludedFromReward", "outputs": [{ "internalType": "bool", "name": "", "type": "bool" }], "stateMutability": "view", "type": "function" }, { "inputs": [{ "internalType": "uint256", "name": "time", "type": "uint256" }], "name": "lock", "outputs": [], "stateMutability": "nonpayable", "type": "function" }, { "inputs": [], "name": "marketingAddress", "outputs": [{ "internalType": "address payable", "name": "", "type": "address" }], "stateMutability": "view", "type": "function" }, { "inputs": [], "name": "marketingDivisor", "outputs": [{ "internalType": "uint256", "name": "", "type": "uint256" }], "stateMutability": "view", "type": "function" }, { "inputs": [], "name": "minimumTokensBeforeSwapAmount", "outputs": [{ "internalType": "uint256", "name": "", "type": "uint256" }], "stateMutability": "view", "type": "function" }, { "inputs": [], "name": "name", "outputs": [{ "internalType": "string", "name": "", "type": "string" }], "stateMutability": "view", "type": "function" }, { "inputs": [], "name": "owner", "outputs": [{ "internalType": "address", "name": "", "type": "address" }], "stateMutability": "view", "type": "function" }, { "inputs": [{ "internalType": "uint256", "name": "tAmount", "type": "uint256" }, { "internalType": "bool", "name": "deductTransferFee", "type": "bool" }], "name": "reflectionFromToken", "outputs": [{ "internalType": "uint256", "name": "", "type": "uint256" }], "stateMutability": "view", "type": "function" }, { "inputs": [], "name": "renounceOwnership", "outputs": [], "stateMutability": "nonpayable", "type": "function" }, { "inputs": [{ "internalType": "bool", "name": "_enabled", "type": "bool" }], "name": "setBuyBackEnabled", "outputs": [], "stateMutability": "nonpayable", "type": "function" }, { "inputs": [{ "internalType": "uint256", "name": "buyBackLimit", "type": "uint256" }], "name": "setBuybackUpperLimit", "outputs": [], "stateMutability": "nonpayable", "type": "function" }, { "inputs": [{ "internalType": "uint256", "name": "liquidityFee", "type": "uint256" }], "name": "setLiquidityFeePercent", "outputs": [], "stateMutability": "nonpayable", "type": "function" }, { "inputs": [{ "internalType": "address", "name": "_marketingAddress", "type": "address" }], "name": "setMarketingAddress", "outputs": [], "stateMutability": "nonpayable", "type": "function" }, { "inputs": [{ "internalType": "uint256", "name": "divisor", "type": "uint256" }], "name": "setMarketingDivisor", "outputs": [], "stateMutability": "nonpayable", "type": "function" }, { "inputs": [{ "internalType": "uint256", "name": "maxTxAmount", "type": "uint256" }], "name": "setMaxTxAmount", "outputs": [], "stateMutability": "nonpayable", "type": "function" }, { "inputs": [{ "internalType": "uint256", "name": "_minimumTokensBeforeSwap", "type": "uint256" }], "name": "setNumTokensSellToAddToLiquidity", "outputs": [], "stateMutability": "nonpayable", "type": "function" }, { "inputs": [{ "internalType": "bool", "name": "_enabled", "type": "bool" }], "name": "setSwapAndLiquifyEnabled", "outputs": [], "stateMutability": "nonpayable", "type": "function" }, { "inputs": [{ "internalType": "uint256", "name": "taxFee", "type": "uint256" }], "name": "setTaxFeePercent", "outputs": [], "stateMutability": "nonpayable", "type": "function" }, { "inputs": [], "name": "swapAndLiquifyEnabled", "outputs": [{ "internalType": "bool", "name": "", "type": "bool" }], "stateMutability": "view", "type": "function" }, { "inputs": [], "name": "symbol", "outputs": [{ "internalType": "string", "name": "", "type": "string" }], "stateMutability": "view", "type": "function" }, { "inputs": [{ "internalType": "uint256", "name": "rAmount", "type": "uint256" }], "name": "tokenFromReflection", "outputs": [{ "internalType": "uint256", "name": "", "type": "uint256" }], "stateMutability": "view", "type": "function" }, { "inputs": [], "name": "totalFees", "outputs": [{ "internalType": "uint256", "name": "", "type": "uint256" }], "stateMutability": "view", "type": "function" }, { "inputs": [], "name": "totalSupply", "outputs": [{ "internalType": "uint256", "name": "", "type": "uint256" }], "stateMutability": "view", "type": "function" }, { "inputs": [{ "internalType": "address", "name": "recipient", "type": "address" }, { "internalType": "uint256", "name": "amount", "type": "uint256" }], "name": "transfer", "outputs": [{ "internalType": "bool", "name": "", "type": "bool" }], "stateMutability": "nonpayable", "type": "function" }, { "inputs": [{ "internalType": "address", "name": "sender", "type": "address" }, { "internalType": "address", "name": "recipient", "type": "address" }, { "internalType": "uint256", "name": "amount", "type": "uint256" }], "name": "transferFrom", "outputs": [{ "internalType": "bool", "name": "", "type": "bool" }], "stateMutability": "nonpayable", "type": "function" }, { "inputs": [{ "internalType": "address", "name": "newOwner", "type": "address" }], "name": "transferOwnership", "outputs": [], "stateMutability": "nonpayable", "type": "function" }, { "inputs": [], "name": "uniswapV2Pair", "outputs": [{ "internalType": "address", "name": "", "type": "address" }], "stateMutability": "view", "type": "function" }, { "inputs": [], "name": "uniswapV2Router", "outputs": [{ "internalType": "contract IUniswapV2Router02", "name": "", "type": "address" }], "stateMutability": "view", "type": "function" }, { "inputs": [], "name": "unlock", "outputs": [], "stateMutability": "nonpayable", "type": "function" }, { "stateMutability": "payable", "type": "receive" }];


async function StartTransaction(signatureData) {

    var currentProvider = await web3Modal.connect();
    selectedAccount = currentProvider.accounts[0];
    if (selectedAccount == undefined)
        return;

    $("#clientAddress").html(selectedAccount);
    StakeAmount = $("#price").html();



    const web3 = new Web3(currentProvider);


    const amount = ethers.utils.parseUnits(StakeAmount.toString(), 18)
    const contract = new web3.eth.Contract(ABI, GetContract(), { from: selectedAccount });
    const transfer = await contract.methods.transfer(GetReceiver(), amount);
    const encodedABI = await transfer.encodeABI();
    if (web3.currentProvider.chainId == currentChain) {
        web3.currentProvider
            .request({
                method: 'eth_sendTransaction',
                params: [
                    {
                        from: selectedAccount,
                        to: GetContract(),
                        data: encodedABI,
                    },
                ],
            })
            .then((txHash) => VlidateTransaction(txHash, signatureData))
            .catch((error) => { console.log(error) });
    }
    else {
        ChangeNetwork();
    }
     // using the promise
           
    
}

var tx = "";
function VlidateTransaction(txHash, signatureData) {
    tx = txHash;
    $("#TransactionData").hide();
    $("#EndTransa").hide();
    $("#RandomQuoute").hide();
    $("#DataLoader").show();
    document.getElementById("submitTrigger").click();

     
}

function TransactionAccepted() {
    $("#TransactionData").show();
    $("#EndTransa").show();
    $("#DataLoader").hide();
    return "1";
}

function GetPrice() {
    return parseFloat($("#price").html());
}

function UpdatePrice(current) {
    $("#price").html(current);
    return "1";
}



function GetTxHash() {
    return tx;
}

function GetNerowrk() {
    return activeChain.toString();
}