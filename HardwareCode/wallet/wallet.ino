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
    //    Serial.println(receiveVal);
        cmd = cmd + receiveVal;
      //  Serial.println(cmd);

        if(cmd == "#CF")
        {
           cmd = "";
           String res = readStringFromEEPROM(0);
       //    Serial.println(res);
           if(res != "")
           {
         //     Serial.println("Device Configured");
              Serial.write("#CFS1"); 
           }
           else
           {
       //       Serial.println("Configuration is missing!");
              Serial.write("#CFS2"); 
           }
           
        }
     
 
        if(ShouldRead && receiveVal == '#')
        {
         //  Serial.println("Stopping to read password");
           ShouldRead = false;
           Pass = readConcurrent;
           readConcurrent = "";
           cmd = "";
           //Serial.println(converted);
           //Serial.println(ascii);
        }


        if(ShouldRead && receiveVal == '!')
        {
        //   Serial.println("Stopping to read private key");
          // Serial.println(readConcurrent);

           ShouldRead = false;
          // Serial.println(Pass);
          // Serial.println(readConcurrent);
           EncryptInitial(Pass,readConcurrent);
           readConcurrent = "";
           cmd = "";
      
        }
        if(ShouldRead == true)
        {
          
           readConcurrent = readConcurrent + receiveVal;
           
        }
             if(cmd == "#CI")
        {
           cmd = "";
       //    Serial.println("Starting to read password");
           ShouldRead = true;
        }

        if(cmd == "#WI")
        {
           cmd = "";
         //  Serial.println("Starting to read private key");
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
  privateKey.toCharArray(char_array, str_len); 
  Serial.println("Initial Key: " + privateKey);

  int z = 0;
  String pk1 = privateKey.substring(0,16);
  String pk2 = privateKey.substring(16, 32);
  String pk3 = privateKey.substring(32, 48);
  String pk4 = privateKey.substring(48, 64);

    //Serial.println(char_array);

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
 char msg[44] = "Some test dawdawdwadwadwadawdwadawdawddawdw";
  aes256_enc_single(keys,msg);
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
  
  aes256_dec_single(keys, strA1);
  aes256_dec_single(keys, strA2);
  aes256_dec_single(keys, strA3);
  aes256_dec_single(keys, strA4);
  Serial.println(strA1);
  Serial.println(strA2);
  Serial.println(strA3);
  Serial.println(strA4);
 // Serial.println("decrypted:");

 
 // String retrievedString = readStringFromEEPROM(0);
  //Serial.print("The String we read from EEPROM: ");
  //Serial.println(retrievedString);
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

 
 

 
 


 