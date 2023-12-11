from vector import Vector, cross
from quaternion import Quaternion


class MahonyFilter: # (Filter):
    two_kp: float
    two_ki: float
    quaternion: Quaternion
    integral_feedback: Vector
    inverse_sample_frequency: float

    def __init__(self, proportional_gain: float = 0.5, integral_gain: float = 0, sample_frequency: float = 512) -> None:
        self.two_kp = 2 * proportional_gain
        self.two_ki = 2 * integral_gain
        self.quaternion = Quaternion()
        self.integral_feedback = Vector()
        self.inverse_sample_frequency = 1 / sample_frequency
        
    def update(self, accelerometer: Vector, gyroscope: Vector, delta_time: float = 0) -> None:
        if delta_time == 0:
            delta_time = self.inverse_sample_frequency
        
        if accelerometer != Vector(0, 0, 0):
            accelerometer.normalise()

            # Estimated direction of gravity
            gravity = Vector(
                self.quaternion.b * self.quaternion.d - self.quaternion.a * self.quaternion.c,
                self.quaternion.a * self.quaternion.b + self.quaternion.c * self.quaternion.d,
                self.quaternion.a * self.quaternion.a - 0.5 + self.quaternion.d * self.quaternion.d
            )

            # Error is sum of cross product between estimated and measured direction of gravity
            error = cross(accelerometer, gravity)

            # Compute and apply integral feedback if enabled
            if self.two_ki > 0:
                # Integral error scaled by Ki
                self.integral_feedback += error * self.two_ki * delta_time
                # Apply integral feedback
                gyroscope += self.integral_feedback
            else:
                # Prevent integral windup
                self.integral_feedback = Vector(0, 0, 0)

            # Apply proportional feedback
            gyroscope += error * self.two_kp

        # Integrate rate of change of quaternion
        gyroscope *= 0.5 * delta_time
        self.quaternion += Quaternion(
            -self.quaternion.b * gyroscope.x - self.quaternion.c * gyroscope.y - self.quaternion.d * gyroscope.z,
            self.quaternion.a * gyroscope.x + self.quaternion.c * gyroscope.z - self.quaternion.d * gyroscope.y,
            self.quaternion.a * gyroscope.y - self.quaternion.b * gyroscope.z + self.quaternion.d * gyroscope.x,
            self.quaternion.a * gyroscope.z + self.quaternion.b * gyroscope.y - self.quaternion.c * gyroscope.x
        )

        # Normalise quaternion
        self.quaternion.normalise()