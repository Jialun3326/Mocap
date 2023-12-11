from math import sqrt


class Vector:
    x: float
    y: float
    z: float
    
    def __init__(self, x: float = 0, y: float = 0, z: float = 0) -> None:
        self.x, self.y, self.z = x, y, z
        
    def __add__(self, other: "Vector") -> "Vector":
        return Vector(self.x + other.x, self.y + other.y, self.z + other.z)
    
    def __sub__(self, other: "Vector") -> "Vector":
        return Vector(self.x - other.x, self.y - other.y, self.z - other.z)
    
    def __mul__(self, other: float) -> "Vector":
        return Vector(self.x * other, self.y * other, self.z * other)
    
    def __truediv__(self, other: float) -> "Vector":
        return Vector(self.x / other, self.y / other, self.z / other)
    
    def __str__(self, format: str = " 3.3f", brackets: tuple[str, str] = ("(", ")")) -> str:
        return f"{brackets[0]}{self.x:{format}}, {self.y:{format}}, {self.z:{format}}{brackets[1]}"
    
    def __repr__(self) -> str:
        return f"Vector{self.__str__()}"
    
    def __iter__(self):
        return iter([self.x, self.y, self.z])
    
    def __getitem__(self, index: int) -> float:
        return [self.x, self.y, self.z][index]
    
    def __setitem__(self, index: int, value: float) -> None:
        if index == 0:
            self.x = value
        elif index == 1:
            self.y = value
        elif index == 2:
            self.z = value
        else:
            raise IndexError("Index out of range")
        
    def __len__(self) -> int:
        return 3
    
    def __neg__(self) -> "Vector":
        return Vector(-self.x, -self.y, -self.z)
    
    def normal(self):
        return Vector(self.y, -self.x, 0)
    
    def magnitude(self) -> float:
        return sqrt(dot(self, self))
    
    def normalised(self) -> "Vector":
        return self / self.magnitude()
        
    def normalise(self) -> None:
        self = self.normalised()


def __eq__(a: Vector, b: Vector) -> bool:
    return a.x == b.x and a.y == b.y and a.z == b.z

def __ne__(a: Vector, b: Vector) -> bool:
    return not a == b


def dot(a: Vector, b: Vector) -> float:
    return a.x * b.x + a.y * b.y + a.z * b.z

def cross(a: Vector, b: Vector) -> Vector:
    return Vector(
        a.y * b.z - a.z * b.y,
        a.z * b.x - a.x * b.z,
        a.x * b.y - a.y * b.x,
    )