from sensor import Sensor


def main():
    sensor = Sensor()
    
    while True:
        try:
            print(*sensor.read())
        except KeyboardInterrupt:
            try:
                sensor.calibrate()
            except KeyboardInterrupt:
                break

              
if __name__ == "__main__":
    main()