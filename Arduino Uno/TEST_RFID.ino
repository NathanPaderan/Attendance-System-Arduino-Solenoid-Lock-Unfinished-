#include <SPI.h>
#include <MFRC522.h>
#include <EEPROM.h>
#define SS_PIN 10
#define RST_PIN 9
int access = 6;
int denied = 7;
int sound = 5;
int relay = 8;
boolean buzzer = false;
boolean solenoid = false;

MFRC522 mfrc522(SS_PIN, RST_PIN);// Create MFRC522 instance.
char ch;
void setup() 
{
  Serial.begin(9600);   // Initiate a serial communication
  SPI.begin();      // Initiate  SPI bus
  mfrc522.PCD_Init();   // Initiate MFRC522
  Serial.println();
  pinMode(access, OUTPUT);
  pinMode(denied, OUTPUT);
  pinMode(sound, OUTPUT);
  digitalWrite(sound,LOW);
  pinMode(relay,OUTPUT);
}
void loop() 
{
  
  // Look for new cards
  if ( ! mfrc522.PICC_IsNewCardPresent()) 
  {
    return;
  }
  // Select one of the cards
  if ( ! mfrc522.PICC_ReadCardSerial()) 
  {
    return;
  }
  //Show UID on serial monitor
  String content= "";
  byte letter;
  buzzer = true;
  for (byte i = 0; i < mfrc522.uid.size; i++) 
  {
     Serial.print(mfrc522.uid.uidByte[i] < 0x10 ? " 0" : " ");
     Serial.print(mfrc522.uid.uidByte[i], HEX);
     content.concat(String(mfrc522.uid.uidByte[i] < 0x10 ? " 0" : " "));
     content.concat(String(mfrc522.uid.uidByte[i], HEX));
  }
  Serial.println();
  content.toUpperCase();
 
  if(buzzer == true){
    Buzz(); 
  }
  delay(500); 
  Serial.available() == 0;
  ch = Serial.read();
  if (ch == 'A')
{
  digitalWrite(relay,HIGH);
  digitalWrite(access,HIGH);
  delay(5000);
  digitalWrite(relay,LOW);
  digitalWrite(access,LOW);

  }
  else
  {digitalWrite(relay,LOW);
  digitalWrite(denied,HIGH); 
  delay(800);
  digitalWrite(relay,LOW);
  digitalWrite(denied,LOW);
  }
}
void Buzz(){
  digitalWrite(sound, HIGH);
  delay(500);
  digitalWrite(sound,LOW);
  buzzer=false;
}
//void Solenoid()
