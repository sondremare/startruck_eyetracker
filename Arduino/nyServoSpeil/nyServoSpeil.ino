// Sweep
// by BARRAGAN <http://barraganstudio.com> 
// This example code is in the public domain.


#include <Servo.h> 
 
Servo myservoX;  // create servo object to control a servo 
                // a maximum of eight servo objects can be created 
Servo myservoY;
Servo myservoA;
Servo myservoB;

int pos = 0;    // variable to store the servo position 
 
void setup() 
{ 
  Serial.begin(9600);
  myservoX.attach(7);  // attaches the servo on pin 9 to the servo object 
  myservoY.attach(9);
  myservoA.attach(3);
  myservoB.attach(5);
  int myTimeout = 250;
  Serial.setTimeout(myTimeout);
  myservoX.write(90);
  myservoY.write(90);
  myservoA.write(90);
  myservoB.write(90);
} 
 
 
void loop() 
{ 
  //myservo.write(0);            // tell servo to go to position in variable 'pos' 
  //delay(1000);    // waits 15ms for the servo to reach the position 
  //myservo.write(180);
  //delay(1000);
  //String data = "";
  if (Serial.available() > 0) {
    String data = Serial.readString();

    if(data.substring(0,1) == "X") {
      data.remove(0,1);
      myservoX.write(data.toInt());      
    } 
    if(data.substring(0,1) == "Y"){
      data.remove(0,1);
      myservoY.write(data.toInt());
    }
    if(data.substring(0,1) == "A"){
      data.remove(0,1);
      myservoA.write(data.toInt());
    }
    if(data.substring(0,1) == "B"){
      data.remove(0,1);
      myservoB.write(data.toInt());
    }
    Serial.write(1);
  }
}

