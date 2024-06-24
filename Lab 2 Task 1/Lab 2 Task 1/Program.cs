#include <iostream>
#include <vector>
#include <thread>
#include <mutex>
#include <chrono>
#include <cstdlib>

const int Width = 100;
const int Height = 25;
const int NumberOfCars = 12;
const int NumberOfMoto = 8;
const int Interval = 200; // in milliseconds
const int Speed = 1;

std::mutex consoleLock;

void setCursorPosition(int x, int y)
{
    std::cout << "\033[" << y + 1 << ";" << x + 1 << "H";
}

void drawBorder()
{
    std::cout << "\033[34m"; // Set console color to blue

    for (int i = 0; i <= Width; ++i)
    {
        setCursorPosition(i, Height);
        std::cout << "*";
    }
    for (int i = 0; i <= Height; ++i)
    {
        setCursorPosition(Width, i);
        std::cout << "*";
    }

    std::cout << "\033[0m"; // Reset console color
}

class Vehicle
{
    public:
    int x, y;
    Vehicle(int x, int y) : x(x), y(y) { }
    virtual void move() = 0;
    virtual char getSymbol() const = 0;
};

class Car : public Vehicle
{
public:
    Car(int x, int y) : Vehicle(x, y) { }
void move() override
{
    std::this_thread::sleep_for(std::chrono::milliseconds(Interval));
    int direction = rand() % 2; // Cars move only along X axis

    consoleLock.lock () ;
    setCursorPosition(x, y);
    std::cout << " ";

    if (direction == 0)
    {
        x = std::min(x + Speed, Width - 1); // Move right
    }
    else
    {
        x = std::max(x - Speed, 0); // Move left
    }

    setCursorPosition(x, y);
    std::cout << getSymbol();
    consoleLock.unlock();
}
char getSymbol() const override {
        return 'C';
    }
};

class Motorcycle : public Vehicle
{
public:
    Motorcycle(int x, int y) : Vehicle(x, y) { }
void move() override
{
    std::this_thread::sleep_for(std::chrono::milliseconds(100)); // Motorcycles move more frequently

    consoleLock.lock () ;
    setCursorPosition(x, y);
    std::cout << " ";

    int direction = rand() % 2; // Motorcycles move only along Y axis
    if (direction == 0)
    {
        y = std::min(y + Speed, Height - 1); // Move down
    }
    else
    {
        y = std::max(y - Speed, 0); // Move up
    }

    setCursorPosition(x, y);
    std::cout << getSymbol();
    consoleLock.unlock();
}
char getSymbol() const override {
        return 'M';
    }
};

int main()
{
    srand(time(0));
    std::cout << "\033[?25l"; // Hide cursor
    drawBorder();

    std::vector<Car> cars;
    std::vector<Motorcycle> motorcycles;
    std::vector<std::thread> threads;

    // Initialize cars at random positions
    for (int i = 0; i < NumberOfCars; ++i)
    {
        cars.emplace_back(rand() % Width, rand() % Height);
        setCursorPosition(cars.back().x, cars.back().y);
        std::cout << cars.back().getSymbol();
    }

    // Initialize motorcycles at random positions
    for (int i = 0; i < NumberOfMoto; ++i)
    {
        motorcycles.emplace_back(rand() % Width, rand() % Height);
        setCursorPosition(motorcycles.back().x, motorcycles.back().y);
        std::cout << motorcycles.back().getSymbol();
    }

    // Start threads for cars
    for (auto & car : cars) {
    threads.emplace_back([&car]() {
        while (true)
        {
            car.move();
        }
    });
}

// Start threads for motorcycles
for (auto & motorcycle : motorcycles)
{
    threads.emplace_back([&motorcycle]() {
        while (true)
        {
            motorcycle.move();
        }
    });
    }

    for (auto & thread : threads)
{
    thread.join();
}

std::cout << "\033[?25h"; // Show cursor

return 0;
}
