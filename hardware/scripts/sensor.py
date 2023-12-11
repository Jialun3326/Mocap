import serial
from time import time
from vector import Vector


class Sensor:
    uart: serial.Serial
    accelerometer_offset: float
    gyroscope_offsets: Vector
    
    def __init__(self) -> None:
        self.uart = serial.Serial(
            port="COM10",
            baudrate=115200,
            bytesize=serial.EIGHTBITS,
            stopbits=serial.STOPBITS_ONE,
            parity=serial.PARITY_NONE,
        )
        self.calibrate()
        
    def calibrate(self) -> None:
        self.accelerometer_offset = 1
        self.gyroscope_offsets = Vector()

        print("Please place the IMU sensor upright on a flat surface")
        input("Press enter when ready to continue...")

        print("Calibrating...")
        start_time = time()
        num_iterations = 0
        accelerometer_average = 0
        gyroscope_averages = Vector(0, 0, 0)
        while time() - start_time < 3:
            accelerometer, gyroscope = self.read()
            accelerometer_average += accelerometer.z
            gyroscope_averages += gyroscope
            num_iterations += 1

        try:
            self.accelerometer_offset = num_iterations / accelerometer_average
        except ZeroDivisionError:
            print("[WARNING]: Accelerometer average is zero")
            self.accelerometer_offset = 1
        self.gyroscope_offsets = Vector(
            gyroscope_averages.x / num_iterations,
            gyroscope_averages.y / num_iterations,
            gyroscope_averages.z / num_iterations
        )

        print("Calibration complete")
        print(f"Accelerometer offset: {self.accelerometer_offset:.2f}")
        print(f"Gyroscope offsets:    ({self.gyroscope_offsets})")

        input("Press enter when ready to continue...")
    
    def read(self) -> tuple[Vector, Vector]:
        line = read_latest_line(self.uart)
    
        if len(line) != (6 * 4):
            print("[WARNING]: Invalid data received")
            return Vector(), Vector()
        
        data = [hex_to_float(line[i*4:(i+1)*4]) for i in range(6)]
        
        acclerometer_data = Vector(
            data[0] * self.accelerometer_offset,
            data[1] * self.accelerometer_offset,
            data[2] * self.accelerometer_offset,
        )
        gyroscope_data = Vector(
            data[3] - self.gyroscope_offsets.x,
            data[4] - self.gyroscope_offsets.y,
            data[5] - self.gyroscope_offsets.z,
        )
        
        return acclerometer_data, gyroscope_data

    def __del__(self) -> None:
        self.uart.close()

    
def read_latest_line(uart: serial.Serial) -> str:
    line = uart.readline().decode().strip()
    while uart.in_waiting > 0:
        line = uart.readline().decode().strip()
    return line
    
    
def hex_to_float(hex: str) -> float:
    return float(-(int(hex, 16) & 0x8000) | (int(hex, 16) & 0x7fff)) * 0.0001
