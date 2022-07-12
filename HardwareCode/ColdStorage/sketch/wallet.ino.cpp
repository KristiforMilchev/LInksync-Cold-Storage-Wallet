#line 1 "C:\\Users\\krisk\\source\\repos\\SYNCWallet\\SYNCWallet\\HardwareCode\\wallet\\wallet.ino"
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

#line 19 "C:\\Users\\krisk\\source\\repos\\SYNCWallet\\SYNCWallet\\HardwareCode\\wallet\\wallet.ino"
void setup();
#line 26 "C:\\Users\\krisk\\source\\repos\\SYNCWallet\\SYNCWallet\\HardwareCode\\wallet\\wallet.ino"
void loop();
#line 55 "C:\\Users\\krisk\\source\\repos\\SYNCWallet\\SYNCWallet\\HardwareCode\\wallet\\wallet.ino"
void writeStringToEEPROM(int addrOffset, const String &strToWrite);
#line 65 "C:\\Users\\krisk\\source\\repos\\SYNCWallet\\SYNCWallet\\HardwareCode\\wallet\\wallet.ino"
String readStringFromEEPROM(int addrOffset);
#line 19 "C:\\Users\\krisk\\source\\repos\\SYNCWallet\\SYNCWallet\\HardwareCode\\wallet\\wallet.ino"
void setup() {
  Serial.begin(9600);
 
 
}

// the loop function runs over and over again forever
void loop() {

   char receiveVal;     
     
    if(Serial.available() > 0)  
    {          
        receiveVal = Serial.read();  
        if(receiveVal == "#CF")
        {
           String res = readStringFromEEPROM(0);
           Serial.println(res);
        }
       
    }     
    
    uint8_t key[] = {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31};
    char data[] = "0123456789012345";
    aes256_enc_single(key, data);
    Serial.print("encrypted:");
    Serial.println(data);
    aes256_dec_single(key, data);
    Serial.print("decrypted:");
    Serial.println(data);
      
    String retrievedString = readStringFromEEPROM(0);
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


 

