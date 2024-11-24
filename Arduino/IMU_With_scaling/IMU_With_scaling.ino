#include <Wire.h>
#include <Adafruit_TinyUSB.h> // for Serial
#include <Adafruit_LIS3MDL.h>
#include <Deneyap_6EksenAtaletselOlcumBirimi.h>       // Deneyap_IvmeOlcerVeDonuOlcer.h kütüphanesi eklendi
#include <bluefruit.h>
#include <Adafruit_LittleFS.h>
#include <InternalFileSystem.h>
#include <avr/dtostrf.h>
#include <SimpleKalmanFilter.h> // Include SimpleKalmanFilter library

// BLE Service
BLEDfu  bledfu;  // OTA DFU service
BLEDis  bledis;  // device information
BLEUart bleuart; // uart over ble
BLEBas  blebas;  // battery

// L3GD20 I2C address is 0x6A(106)
#define Adrs_gyr 0x6B
#define Adrs_mag 0x1C

Adafruit_LIS3MDL lis3mdl;
LSM6DSM lsm6dsm;                                      // lsm6dsm icin Class tanimlamasi

// Kalman filters for roll, pitch, yaw
SimpleKalmanFilter rollKalmanFilter(1, 1, 0.01); // Q, R, P initial
SimpleKalmanFilter pitchKalmanFilter(1, 1, 0.01); // Q, R, P initial
SimpleKalmanFilter yawKalmanFilter(1, 1, 0.01); // Q, R, P initial

void setup()
{
  Serial.begin(115200);// Initialise serial communication, set baud rate = 9600
  //while (!Serial) delay(10); //waits for serial terminal to be open, necessary in newer arduino boards.

  pinMode(LED_BUILTIN, OUTPUT);
  Wire.begin();  // Initialise I2C communication as MASTER
  
  // BLE

  // Setup the BLE LED to be enabled on CONNECT
  // Note: This is actually the default behavior, but provided
  // here in case you want to control this LED manually via PIN 19
  Bluefruit.autoConnLed(true);

  // Config the peripheral connection with maximum bandwidth 
  // more SRAM required by SoftDevice
  // Note: All config***() function must be called before begin()
  Bluefruit.configPrphBandwidth(BANDWIDTH_MAX);

  Bluefruit.begin();
  Bluefruit.setTxPower(4);    // Check bluefruit.h for supported values
  //Bluefruit.setName(getMcuUniqueID()); // useful testing with multiple central connections
  Bluefruit.setName("Feather 8"); // useful testing with multiple central connections
  Bluefruit.Periph.setConnectCallback(connect_callback);
  Bluefruit.Periph.setDisconnectCallback(disconnect_callback);

  // To be consistent OTA DFU should be added first if it exists
  bledfu.begin();

  // Configure and Start Device Information Service
  bledis.setManufacturer("Adafruit Industries");
  bledis.setModel("Bluefruit Feather52");
  bledis.begin();

  // Configure and Start BLE Uart Service
  bleuart.begin();

  // Start BLE Battery Service
  blebas.begin();
  blebas.write(100);

  // Set up and start advertising
  startAdv();

  Serial.println("Please use Adafruit's Bluefruit LE app to connect in UART mode");
  Serial.println("Once connected, enter character(s) that you wish to send");

  // Gyro
  if (lsm6dsm.begin() != IMU_SUCCESS) {          // hardware I2C mode, can pass in address & alt Wire
    Serial.println("Failed to find LSM6DSM");
    while (1) { delay(10); }
  }
  
  // Magnetometer
  if (! lis3mdl.begin_I2C(Adrs_mag)) {          // hardware I2C mode, can pass in address & alt Wire
    Serial.println("Failed to find LIS3MDL");
    while (1) { delay(10); }
  }

  lis3mdl.setPerformanceMode(LIS3MDL_MEDIUMMODE);
  lis3mdl.setOperationMode(LIS3MDL_CONTINUOUSMODE);
  lis3mdl.setDataRate(LIS3MDL_DATARATE_155_HZ);
  lis3mdl.setRange(LIS3MDL_RANGE_4_GAUSS);
  lis3mdl.setIntThreshold(500);
  lis3mdl.configInterrupt(false, false, true, // enable z axis
                          true, // polarity
                          false, // don't latch
                          true); // enabled!

  delay(100);
}


void startAdv(void)
{
  // Advertising packet
  Bluefruit.Advertising.addFlags(BLE_GAP_ADV_FLAGS_LE_ONLY_GENERAL_DISC_MODE);
  Bluefruit.Advertising.addTxPower();

  // Include bleuart 128-bit uuid
  Bluefruit.Advertising.addService(bleuart);

  // Secondary Scan Response packet (optional)
  // Since there is no room for 'Name' in Advertising packet
  Bluefruit.ScanResponse.addName();
  
  /* Start Advertising
   * - Enable auto advertising if disconnected
   * - Interval:  fast mode = 20 ms, slow mode = 152.5 ms
   * - Timeout for fast mode is 30 seconds
   * - Start(timeout) with timeout = 0 will advertise forever (until connected)
   * 
   * For recommended advertising interval
   * https://developer.apple.com/library/content/qa/qa1931/_index.html   
   */
  Bluefruit.Advertising.restartOnDisconnect(true);
  Bluefruit.Advertising.setInterval(32, 244);    // in unit of 0.625 ms
  Bluefruit.Advertising.setFastTimeout(30);      // number of seconds in fast mode
  Bluefruit.Advertising.start(0);                // 0 = Don't stop advertising after n seconds  
}


void loop()
{
  // Sensor
  float xACC = lsm6dsm.readFloatAccelX();
  float yACC = lsm6dsm.readFloatAccelY();
  float zACC = lsm6dsm.readFloatAccelZ();

  float xGYR = lsm6dsm.readFloatGyroX();
  float yGYR = lsm6dsm.readFloatGyroY();
  float zGYR = lsm6dsm.readFloatGyroZ();

  //float temp = lsm6dsm.readTempC();

  lis3mdl.read();      // get X Y and Z data at once
  float xMAG = lis3mdl.x;
  float yMAG = lis3mdl.y;
  float zMAG = lis3mdl.z;

  // Kalman filter update
  float roll = rollKalmanFilter.updateEstimate(atan2(yACC, zACC) * RAD_TO_DEG);
  float pitch = pitchKalmanFilter.updateEstimate(atan2(-xACC, sqrt(yACC * yACC + zACC * zACC)) * RAD_TO_DEG);
  // ***************************SCALING************************************************************************************************************************************************************

  pitch = ((pitch + 80) / 160.0) * 180.0;

  if (roll >= 0 && roll <= 180) {
  roll = 180- roll; // 0 to 180 remains the same
} else{
  roll = -roll-180 ; // Convert 180 to 360 to -180 to 0
}
 // *****************************************************************************************************************************************************************
  // where ay and az are the raw accelerometer readings along the Y and Z axes, respectively,
  // and beta is the correction angle obtained from the magnetometer
  float beta = atan2(yMAG, xMAG);
  float yaw = yawKalmanFilter.updateEstimate((atan2(yACC, sqrt(yACC * yACC + zACC * zACC)) + beta )* RAD_TO_DEG);

  Serial.println(
                 String(roll)+"\t\t"+String(pitch)+"\t\t"+String(yaw)+"\t\t");
  
  // BLE
  uint8_t floatArray[sizeof(float) * 3];
  memcpy(floatArray, &roll, sizeof(float));
  memcpy(floatArray + sizeof(float) * 1, &pitch, sizeof(float));
  memcpy(floatArray + sizeof(float) * 2, &yaw, sizeof(float));
  bleuart.write(floatArray, sizeof(floatArray));

  // Forward from BLEUART to HW Serial
  while ( bleuart.available() )
  {
    uint8_t ch;
    ch = (uint8_t) bleuart.read();
    Serial.write(ch);
  }
  
  delay(100);
}



// callback invoked when central connects
void connect_callback(uint16_t conn_handle)
{
  // Get the reference to current connection
  BLEConnection* connection = Bluefruit.Connection(conn_handle);

  char central_name[32] = { 0 };
  connection->getPeerName(central_name, sizeof(central_name));

  Serial.print("Connected to ");
  Serial.println(central_name);
}

/**
 * Callback invoked when a connection is dropped
 * @param conn_handle connection where this event happens
 * @param reason is a BLE_HCI_STATUS_CODE which can be found in ble_hci.h
 */
void disconnect_callback(uint16_t conn_handle, uint8_t reason)
{
  (void) conn_handle;
  (void) reason;

  Serial.println();
  Serial.print("Disconnected, reason = 0x"); Serial.println(reason, HEX);
}
