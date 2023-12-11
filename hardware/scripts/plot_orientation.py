import matplotlib.pyplot as plt
from time import time
from sensor import Sensor
from vector import Vector
from mahony_filter import MahonyFilter


def setup_graphs():
    plt.ion()
    _, graphs = plt.subplots(3)
    graphs = graphs.ravel()
    return graphs


def plot_data(graphs, angles: list[Vector]) -> None:
    for i, _ in enumerate(graphs):
        graphs[i].cla()
        graphs[i].set_ylim(-180, 180)
        
    graphs[0].set_title("Euler Angles")
    graphs[2].set_xlabel("Time (s)")
    
    graphs[0].set_ylabel("x (deg)")
    graphs[1].set_ylabel("y (deg)")
    graphs[2].set_ylabel("z (deg)")
    
    graphs[0].plot([a.x for a in angles])
    graphs[1].plot([a.y for a in angles])
    graphs[2].plot([a.z for a in angles])
    
    plt.pause(0.001)
     

def main():
    sensor = Sensor()
    filter = MahonyFilter()
    graphs = setup_graphs()
    angles = []
    delta_time = 0
    
    while True:
        start_time = time()
        filter.update(*sensor.read(), delta_time)
        angles.append(filter.quaternion.to_euler_angles())
        while len(angles) > 50:
            angles.pop(0)
        plot_data(graphs, angles)
        print(filter.quaternion.to_euler_angles())
        delta_time = time() - start_time


if __name__ == "__main__":
    main()