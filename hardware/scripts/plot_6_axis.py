import matplotlib.pyplot as plt
from sensor import Sensor
from vector import Vector


class Data:
    accelerometer: list[Vector]
    gyroscope: list[Vector]
    max_data_points: int
    
    def __init__(self) -> None:
        self.accelerometer, self.gyroscope = [], []
        self.max_data_points = 20
        
    def append(self, accelerometer: Vector, gyroscope: Vector) -> None:
        self.accelerometer.append(accelerometer)
        self.gyroscope.append(gyroscope)
        while len(self.accelerometer) > self.max_data_points:
            self.accelerometer.pop(0)
        while len(self.gyroscope) > self.max_data_points:
            self.gyroscope.pop(0)


def setup_graphs():
    plt.ion()
    _, graphs = plt.subplots(2, 3, figsize=(11, 6))
    graphs = graphs.ravel()
    return graphs


def plot_data(graphs, data: Data) -> None:
    for i in range(6):
        graphs[i].cla()
        if i < 3:
            graphs[i].set_ylim(-1.5, 1.5)
        else:
            graphs[i].set_ylim(-4, 4)
    graphs[1].set_title("Accelerometer")
    graphs[4].set_title("Gyroscope")
    for i, direction in zip(range(3), "xyz"):
        graphs[i].set_ylabel(f"{direction} (g)")
    for i, direction in zip(range(3, 6), "xyz"):
        graphs[i].set_ylabel(f"{direction} (deg/s)")
    for i in range(6):
        graphs[i].set_xlabel("Time (s)")
    graphs[0].plot([a.x for a in data.accelerometer])
    graphs[1].plot([a.y for a in data.accelerometer])
    graphs[2].plot([a.z for a in data.accelerometer])
    graphs[3].plot([g.x for g in data.gyroscope])
    graphs[4].plot([g.y for g in data.gyroscope])
    graphs[5].plot([g.z for g in data.gyroscope])
    plt.pause(0.001)
    

def print_data(data: Data) -> None:
    print(f"({data.accelerometer[-1]}), ({data.gyroscope[-1]})")
        

def main():
    sensor = Sensor()
    data = Data()
    graphs = setup_graphs()
    
    while True:
        data.append(*sensor.read())
        plot_data(graphs, data)
        # print_data(data)

              

if __name__ == "__main__":
    main()