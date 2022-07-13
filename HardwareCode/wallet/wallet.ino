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
            String attemts = readStringFromEEPROM(0);
            String readPw = readStringFromEEPROM(1); //Get PWD from EEPROM
            String readPK = readStringFromEEPROM(40); //Get PK from EEPROM 

            if(Pass.equals(readPw))
            {
                char Buf[readPK.length()+ 1];
                readPK.toCharArray(Buf, readPK.length()+ 1);
                String test =  String(Buf);
                Serial.write("#SD:" ); 
                Serial.write(Buf); 

            }
            else
            {
               int at = attemts.toInt();
               at = at - 1;
               writeStringToEEPROM(0, String(at)); //PK Part 1 

               if(at == 0)
               {
                  ClearMemmory();
               }
               Serial.write("#ERL"); 
            }
        }
       
  
        
        
       delay(10);
    }     
    

}

void EncryptInitial(String password , String privateKey)
{
    delay(100);
 
   //Write values to EEPROM
   writeStringToEEPROM(0, "3"); //PK Part 1 
   writeStringToEEPROM(1, password); //PK Part 1 
   writeStringToEEPROM(41, privateKey); //PK Part 2 
 
 
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


void ClearMemmory()
{
  for (int i = 0; i < 1000; i++)
  {
    EEPROM.write(i, "0");
  }
}
 
 


 