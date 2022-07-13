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
#include "ArduinoJson.h"



char confirmationPwd[10]= "iV1z@$H88"; //Parity check used to check decryption was successfull 

//EPPROM ebcryption storage structure 
 
void setup() {
  Serial.begin(9600);
 
 }



String cmd = "";
String readConcurrent = "";
bool ShouldReadPassword = false;
bool ShouldReadPK = false;

uint8_t signPassword;
String Pass = "";
 bool tpm = false;
// the loop function runs over and over again forever
void loop() {

   String receiveVal;     

    if(Serial.available() > 0)  
    {          
        receiveVal = Serial.readString();
        int str_len = receiveVal.length() + 1; 
        char char_array[str_len];
        receiveVal.toCharArray(char_array, str_len); 
        StaticJsonDocument<512> doc;
                  Serial.println(receiveVal);

        DeserializationError error = deserializeJson(doc, char_array);
        
        if (error) {
          Serial.print(F("deserializeJson() failed: "));
          Serial.println(error.f_str());
          return;
        }
        
        const char* Cmd = doc["Cmd"]; // "CF"
        const char* PrivateKey = doc["PrivateKey"]; // nullptr
        const char* Password = doc["Password"]; // nullptr
        String currentCMD = Cmd;
        String PK = PrivateKey;
        String Pass = Password;
        
          Serial.println(Cmd);
          Serial.println(PrivateKey);
          Serial.println(Password);


        
        // Test if parsing succeeds.
        if (error) {
          Serial.print(F("deserializeJson() failed: "));
          Serial.println(error.f_str());
          return;
        }

        if(currentCMD.equals("CF"))
        {
           cmd = "";
           receiveVal= "";
           Serial.flush();

          Serial.println("Parity Hash: ");
          //  Serial.println(encrypted.Password);

           String decryptParity = readStringFromEEPROM(0);
           if(decryptParity != "")
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
      

        if(currentCMD.equals("NEW"))
        {
          Serial.println("Stopping to read private key");
          // Serial.println(readConcurrent);
           ShouldReadPK = false;

           Serial.flush();
           Serial.println(Pass);
           Serial.println(PK);
           EncryptInitial(Pass,PK);
           readConcurrent = "";
           cmd = "";
      
        }
       
        if(currentCMD.equals("Login"))
        {
          Serial.println("Trying to logjn");
          String decryptParity = readStringFromEEPROM(0);
          decryptParity.trim();
          Serial.println(decryptParity);
          //Password Encoding
          int str_len_pwd = Pass.length() + 1; 
          char char_array_pwd[str_len_pwd];
          Pass.toCharArray(char_array_pwd, str_len_pwd);
          uint8_t keys[4*sizeof(str_len_pwd)];
          
          //Encoding the original password for ascii
          for(byte i = 0; i < sizeof(char_array_pwd) - 1; i++){
            if((int) char_array_pwd[i] != '\0')
            {
                keys[i] = (int) char_array_pwd[i];  
            }
          }
          
         uint8_t key[] = {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15};


          String before = decryptParity;
           
          int str_len_before = before.length() + 1; 
          char char_array_before[str_len_before];
          before.toCharArray(char_array_before, str_len_before);
 
 
          aes256_dec_single(key, char_array_before);
          String after = char_array_before;
          Serial.println("Decrypted Password "+ after);

          String pk1 = readStringFromEEPROM(10); //PK Part 1 
          String pk2 = readStringFromEEPROM(27); //PK Part 2 
          String pk3 = readStringFromEEPROM(44); //PK Part 3 
          String pk4 = readStringFromEEPROM(61); //PK Part 3 
          Serial.println(pk1);
          Serial.println(pk2);
          Serial.println(pk3);
          Serial.println(pk4);
          char strA1[17];
          char strA2[17]; 
          char strA3[17];
          char strA4[17];
          //Converting the splitted pk to char* PKn []
          pk1.toCharArray(strA1, 17); 
          pk2.toCharArray(strA2, 17); 
          pk3.toCharArray(strA3, 17); 
          pk4.toCharArray(strA4, 17); 
          
          aes256_dec_single(key, strA1);
          aes256_dec_single(key, strA2);
          aes256_dec_single(key, strA3);
          aes256_dec_single(key, strA4);
      
          //Debug purposes only to delete later
          Serial.println(strA1);
          Serial.println(strA2);
          Serial.println(strA3);
          Serial.println(strA4);
    
        }
       
  
        
        
       delay(10);
    }     
    

}

void EncryptInitial(String password , String privateKey)
{
    delay(100);
 

    
    Serial.println("Initial Key: " + privateKey);

    int z = 0;

    //Splitting PK to 4 eaqual peaces each 16
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
    //Converting the splitted pk to char* PKn []
    pk1.toCharArray(strA1, 17); 
    pk2.toCharArray(strA2, 17); 
    pk3.toCharArray(strA3, 17); 
    pk4.toCharArray(strA4, 17); 

    //Password Encoding
    int str_len_pwd = password.length() + 1; 
    char char_array_pwd[str_len_pwd];
    password.toCharArray(char_array_pwd, str_len_pwd);
    uint8_t keys[4*sizeof(str_len_pwd)];
    
    //Encoding the original password for ascii
    for(byte i = 0; i < sizeof(char_array_pwd) - 1; i++){
      if((int) char_array_pwd[i] != '\0')
      {
          keys[i] = (int) char_array_pwd[i];  
      }
    }
 
    uint8_t key[] = {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15};

    aes256_enc_single(key,confirmationPwd);

    aes256_enc_single(key, strA1);
    aes256_enc_single(key, strA2);
    aes256_enc_single(key, strA3);
    aes256_enc_single(key, strA4);
    
    //Debug purposes only to delete later
    Serial.println(strA1);
    Serial.println(strA2);
    Serial.println(strA3);
    Serial.println(strA4);
    Serial.println("Enc Password");
    Serial.println(confirmationPwd);

  
    //aes256_dec_single(keys, strA1);
   // aes256_dec_single(keys, strA2);
    //aes256_dec_single(keys, strA3);
   // aes256_dec_single(keys, strA4);

    //Debug purposes only to delete later
   // Serial.println(strA1);
   // Serial.println(strA2);
   // Serial.println(strA3);
   // Serial.println(strA4);
    


   //Write values to EEPROM
   writeStringToEEPROM(0, confirmationPwd);  //Save Password Parity
   writeStringToEEPROM(10, strA1); //PK Part 1 
   writeStringToEEPROM(27, strA2); //PK Part 2 
   writeStringToEEPROM(44, strA3); //PK Part 3 
   writeStringToEEPROM(61, strA4); //PK Part 3 
 
}


void writeStringToEEPROM(int addrOffset, const String &strToWrite)
{
  byte len = strToWrite.length();
  EEPROM.write(addrOffset, len);
  for (int i = 0; i < len; i++)
  {
    EEPROM.write(addrOffset + 1 + i, strToWrite[i]);
  }
}

String readStringFromEEPROM(int addrOffset)
{
  int newStrLen = EEPROM.read(addrOffset);
  char data[newStrLen + 1];
  for (int i = 0; i < newStrLen; i++)
  {
    data[i] = EEPROM.read(addrOffset + 1 + i);
  }
  data[newStrLen] = '\0'; // !!! NOTE !!! Remove the space between the slash "/" and "0" (I've added a space because otherwise there is a display bug)
  return String(data);
}

 
 


 
