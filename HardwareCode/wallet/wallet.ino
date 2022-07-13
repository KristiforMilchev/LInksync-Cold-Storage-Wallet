/*
  SYNC Hardware cold wallet

  Meant for decentralized storage of crypto asssets on offline, cold storagea, and it's compatable
  with SYNC desktop extension for wallet operations. 

  Which can be downloaded for Windows/Mac/Linux at this address:

  -> SYNC wallet address
  
  modified 8 Sep 2022
  by Kristifor Milchev
*/

#include "Arduino.h"
#include "EEPROM.h"
#include "AESLib.h"

char confirmationPwd[10]= "iV1z@$H88"; //Parity check used to check decryption was successfull 

//EPPROM ebcryption storage structure 
struct EncryptedData{
 public:
 char strA1;
 char strA2;
 char strA3;
 char strA4;
 char Password[];
};
 
    EncryptedData encrypted = {
      0,
      0,
      0,
      0,
      0
    };
void setup() {
  Serial.begin(9600);
 
 
}



String cmd = "";
String readConcurrent = "";
bool ShouldRead = false;
uint8_t signPassword;
String Pass = "";
 
// the loop function runs over and over again forever
void loop() {

   char receiveVal;     

    if(Serial.available() > 0)  
    {          
        receiveVal = Serial.read();  
        cmd = cmd + receiveVal;
         cmd.replace(" ", "");
         cmd.trim();

        Serial.println(cmd);

        if(cmd == "#CF")
        {
           cmd = "";
           receiveVal= "";
          

           loadConfig(encrypted);
           Serial.println("Parity Hash: ");
          //  Serial.println(encrypted.Password);

           
           if(encrypted.Password != "")
           {
              Serial.println("Device Configured");
              Serial.write("#CFS1"); 
           }
           else
           {
              Serial.println("Configuration is missing!");
              Serial.write("#CFS2"); 
           }
           
        }
     
 
        if(ShouldRead && receiveVal == '#')
        {
           Serial.println("Stopping to read password");
           ShouldRead = false;
           Pass = readConcurrent;
           readConcurrent = "";
           cmd = "";
          
        }


        if(ShouldRead && receiveVal == '!')
        {
          Serial.println("Stopping to read private key");
          // Serial.println(readConcurrent);

           ShouldRead = false;
           Serial.println(Pass);
           Serial.println(readConcurrent);
           EncryptInitial(Pass,readConcurrent);
           readConcurrent = "";
           cmd = "";
      
        }
        if(ShouldRead == true)
        {
                   

           readConcurrent = readConcurrent + receiveVal;
             Serial.println(readConcurrent);
           
        }
        
        if(cmd == "#CI")
        {
           cmd = "";
           Serial.println("Starting to read password");
           ShouldRead = true;
        }

        if(cmd == "#WI")
        {
           cmd = "";
           Serial.println("Starting to read private key");
           ShouldRead = true;
        }
        
        
       delay(10);
    }     
    

}

void EncryptInitial(String password , String privateKey)
{
    delay(100);
    // data
    int str_len = privateKey.length() + 1; 
    char char_array[str_len];
    password.toCharArray(char_array, str_len); 
    Serial.println("Initial Key: " + privateKey);

    int z = 0;
    String pk1 = privateKey.substring(0,16);
    String pk2 = privateKey.substring(16, 32);
    String pk3 = privateKey.substring(32, 48);
    String pk4 = privateKey.substring(48, 64);
    Serial.println(pk1);
    Serial.println(pk2);
    Serial.println(pk3);
    Serial.println(pk4);
    char strA1[17];
    char strA2[17]; 
    char strA3[17];
    char strA4[17];
    pk1.toCharArray(strA1, str_len); 
    pk2.toCharArray(strA2, str_len); 
    pk3.toCharArray(strA3, str_len); 
    pk4.toCharArray(strA4, str_len); 

    //Password Encoding
    int str_len_pwd = password.length() + 1; 
    char char_array_pwd[str_len_pwd];
    password.toCharArray(char_array_pwd, str_len_pwd);
    uint8_t keys[4*sizeof(str_len_pwd)];
 
    for(byte i = 0; i < sizeof(char_array_pwd) - 1; i++){
    if((int) char_array_pwd[i] != '\0')
    {
            keys[i] = (int) char_array_pwd[i];  
    }
    }
 
 
    aes256_enc_single(keys,confirmationPwd);
    // Serial.println("encrypted:");
    // Serial.println(msg);
    aes256_enc_single(keys, strA1);
    aes256_enc_single(keys, strA2);
    aes256_enc_single(keys, strA3);
    aes256_enc_single(keys, strA4);
    Serial.println(strA1);
    Serial.println(strA2);
    Serial.println(strA3);
    Serial.println(strA4);
    Serial.println("Enc Password");
    Serial.println(confirmationPwd);

  
    aes256_dec_single(keys, strA1);
    aes256_dec_single(keys, strA2);
    aes256_dec_single(keys, strA3);
    aes256_dec_single(keys, strA4);
    Serial.println(strA1);
    Serial.println(strA2);
    Serial.println(strA3);
    Serial.println(strA4);
    // Serial.println("decrypted:");

    EncryptedData encrypted = {
    strA1,
    strA2,
    strA3,
    strA4,
    confirmationPwd
    };
    saveConfig(encrypted); //This will fail current function only accepts string
    // String retrievedString = readStringFromEEPROM(0);
    //Serial.print("The String we read from EEPROM: ");
    //Serial.println(retrievedString);
}

void loadConfig(EncryptedData& config) {
  EEPROM.get(0, config);
}

void saveConfig(EncryptedData& config) {
  EEPROM.put(0, config);
}

 
 


 
