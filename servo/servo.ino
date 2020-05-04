#include <Servo.h>

Servo myservo;

char op;

void setup() {
  Serial.begin(9600);
  myservo.attach(9);
  myservo.write(0);
}

void loop() {
  if (Serial.available() > 0) {
    op = Serial.read();
    if(op == 'P') {
      myservo.write(30);
    } else if (op == 'O') {
      myservo.write(0);
    }
  }
}
