#include <Servo.h>

Servo servo1;
Servo servo2;

const int servo1Pin = 5;
const int servo2Pin = 6;

void setup() {
  Serial.begin(115200);
  servo1.attach(servo1Pin);
  servo2.attach(servo2Pin);
  Serial.println("Arduino ready");
}

void loop() {
  if (Serial.available() > 0) {
    String input = Serial.readStringUntil(';');
    //Serial.print("Received: ");
    //Serial.println(input);
    
    if (input.startsWith("<servo>,")) {
      input = input.substring(8); // 移除 "<servo>,"
      int commaIndex = input.indexOf(',');
      if (commaIndex != -1) {
        int angle1 = input.substring(0, commaIndex).toInt();
        int angle2 = input.substring(commaIndex + 1).toInt();
        
        servo1.write(angle1);
        servo2.write(angle2);
        
        /*
        Serial.print("Set angles: ");
        Serial.print(angle1);
        Serial.print(", ");
        Serial.println(angle2);
        */
      }
    }
    
    // 清空串口缓冲区
    while(Serial.available() > 0) {
      Serial.read();
    }
    
    //Serial.println("Ready for next command");
  }
}