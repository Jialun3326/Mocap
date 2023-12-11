from time import time
from quaternion import Quaternion
from mahony_filter import MahonyFilter
from sensor import Sensor


def print_data(quaternion: Quaternion, display_mode: str) -> None:
    if display_mode == 'q':
        print(quaternion)
    elif display_mode == 'e':
        print(quaternion.to_euler_angles())
    elif display_mode == 'b':
        print(f"Quaternion:   {quaternion}")
        print(f"Euler angles: {quaternion.to_euler_angles()}")
    elif display_mode == 'n':
        pass
    else:
        raise ValueError("Invalid display_mode")


def main():
    sensor = Sensor()
    filter = MahonyFilter()
    
    # display_mode = input("Display mode (q/e/b/n): ")
    display_mode = 'q'
    
    filename = input("Filename: ")
    
    delta_time = 0
    
    with open(f"data/{filename}.csv", "w") as file:
        while True:
            try:
                start_time = time()
                filter.update(*sensor.read(), delta_time)
                # print_data(filter.quaternion, display_mode)
                file.write(str(filter.quaternion) + "\n")
                delta_time = time() - start_time
                print(1/delta_time)
            except KeyboardInterrupt:
                try:
                    file.flush()
                    sensor.calibrate()
                    delta_time = 0
                except KeyboardInterrupt:
                    break
            

if __name__ == "__main__":
    main()