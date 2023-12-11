from math import sqrt, atan2, asin, pi
from vector import Vector


class Quaternion:
    a: float
    b: float
    c: float
    d: float
    
    def __init__(self, a: float = 1, b: float = 0, c: float = 0, d: float = 0) -> None:
        self.a, self.b, self.c, self.d = a, b, c, d
        
    def __add__(self, other: "Quaternion") -> "Quaternion":
        return Quaternion(self.a + other.a, self.b + other.b, self.c + other.c, self.d + other.d)
    
    def __sub__(self, other: "Quaternion") -> "Quaternion":
        return Quaternion(self.a - other.a, self.b - other.b, self.c - other.c, self.d - other.d)
    
    def __mul__(self, other: float) -> "Quaternion":
        return Quaternion(self.a * other, self.b * other, self.c * other, self.d * other)
    
    def __truediv__(self, other: float) -> "Quaternion":
        return Quaternion(self.a / other, self.b / other, self.c / other, self.d / other)
    
    def __str__(self, format: str = " 3.3f", brackets: tuple[str, str] = ("(", ")")) -> str:
        return f"{brackets[0]}{self.a:{format}}, {self.b:{format}}, {self.c:{format}}, {self.d:{format}}{brackets[1]}"
    
    def __repr__(self) -> str:
        return f"Quaternion{self.__str__()}"
    
    def __iter__(self):
        return iter([self.a, self.b, self.c, self.d])
    
    def __getitem__(self, index: int) -> float:
        return [self.a, self.b, self.c, self.d][index]
    
    def __setitem__(self, index: int, value: float) -> None:
        if index == 0:
            self.a = value
        elif index == 1:
            self.b = value
        elif index == 2:
            self.c = value
        elif index == 3:
            self.d = value
        else:
            raise IndexError("Index out of range")
        
    def __len__(self) -> int:
        return 4
    
    def __neg__(self) -> "Quaternion":
        return Quaternion(-self.a, -self.b, -self.c, -self.d)
    
    # def normal(self):
    #     return Quaternion(self.b, -self.a, 0, 0)
    
    def magnitude(self) -> float:
        return sqrt(dot(self, self))
        
    def normalised(self) -> "Quaternion":
        return self / self.magnitude()
    
    def normalise(self) -> None:
        self = self.normalised()
        
    def conjugate(self) -> "Quaternion":
        return Quaternion(self.a, -self.b, -self.c, -self.d)
    
    def to_euler_angles(self, radians: bool = False) -> Vector:
        euler_angles = Vector(
            atan2(2 * (self.a * self.b + self.c * self.d), 1 - 2 * (self.b ** 2 + self.c ** 2)),
            # FIXME: Is this correct?
            asin(max(min(2 * (self.a * self.c - self.d * self.b), 1), -1)),
            atan2(2 * (self.a * self.d + self.b * self.c), 1 - 2 * (self.c ** 2 + self.d ** 2))
        )
        if not radians:
            euler_angles *= 180 / pi
        return euler_angles


def __eq__(q1: Quaternion, q2: Quaternion) -> bool:
    return q1.a == q2.a and q1.b == q2.b and q1.c == q2.c and q1.d == q2.d

def __ne__(q1: Quaternion, q2: Quaternion) -> bool:
    return not q1 == q2


def dot(q1: Quaternion, q2: Quaternion) -> float:
    return q1.a * q2.a + q1.b * q2.b + q1.c * q2.c + q1.d * q2.d

def cross(q1: Quaternion, q2: Quaternion) -> Quaternion:
    return Quaternion(
        q1.b * q2.c - q1.c * q2.b + q1.d * q2.a + q1.a * q2.d,
        q1.c * q2.a - q1.a * q2.c + q1.d * q2.b + q1.b * q2.d,
        q1.a * q2.b - q1.b * q2.a + q1.d * q2.c + q1.c * q2.d,
        q1.a * q2.c - q1.c * q2.a + q1.b * q2.b + q1.d * q2.d,
    )