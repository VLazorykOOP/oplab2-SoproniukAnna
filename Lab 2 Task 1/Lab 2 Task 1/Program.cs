using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VehicleSimulation
{
    class Program
    {
        const int AREA_WIDTH = 100;
        const int AREA_HEIGHT = 25;
        const int NUM_CARS = 12;
        const int NUM_MOTORCYCLES = 8;
        const int CHANGE_DIRECTION_INTERVAL = 200; // in milliseconds
        const int VELOCITY = 1;

        static object consoleLock = new object();
        static Random rand = new Random();

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            DrawBorder();
            List<Car> cars = new List<Car>();
            List<Motorcycle> motorcycles = new List<Motorcycle>();

            // Initialize cars at random positions
            for (int i = 0; i < NUM_CARS; ++i)
            {
                cars.Add(new Car { x = rand.Next(AREA_WIDTH), y = rand.Next(AREA_HEIGHT) });
                SetCursorPosition(cars[i].x, cars[i].y);
                Console.Write("C");
            }

            // Initialize motorcycles at random positions
            for (int i = 0; i < NUM_MOTORCYCLES; ++i)
            {
                motorcycles.Add(new Motorcycle { x = rand.Next(AREA_WIDTH), y = rand.Next(AREA_HEIGHT) });
                SetCursorPosition(motorcycles[i].x, motorcycles[i].y);
                Console.Write("M");
            }

            List<Task> tasks = new List<Task>();

            // Start tasks for cars
            foreach (var car in cars)
            {
                tasks.Add(Task.Run(() => MoveCar(car)));
            }

            // Start tasks for motorcycles
            foreach (var motorcycle in motorcycles)
            {
                tasks.Add(Task.Run(() => MoveMotorcycle(motorcycle)));
            }

            Task.WaitAll(tasks.ToArray());
        }

        static void MoveCar(Car car)
        {
            while (true)
            {
                Thread.Sleep(CHANGE_DIRECTION_INTERVAL);
                int direction = rand.Next(2); // Cars move only along X axis

                lock (consoleLock)
                {
                    SetCursorPosition(car.x, car.y);
                    Console.Write(" ");

                    car.x = direction switch
                    {
                        0 => Math.Min(car.x + VELOCITY, AREA_WIDTH - 1), // Move right
                        1 => Math.Max(car.x - VELOCITY, 0), // Move left
                        _ => car.x,
                    };

                    SetCursorPosition(car.x, car.y);
                    Console.Write("C");
                }
            }
        }

        static void MoveMotorcycle(Motorcycle motorcycle)
        {
            while (true)
            {
                Thread.Sleep(100); // Motorcycles move more frequently

                lock (consoleLock)
                {
                    SetCursorPosition(motorcycle.x, motorcycle.y);
                    Console.Write(" ");

                    motorcycle.y = rand.Next(2) switch
                    {
                        0 => Math.Min(motorcycle.y + VELOCITY, AREA_HEIGHT - 1), // Move down
                        1 => Math.Max(motorcycle.y - VELOCITY, 0), // Move up
                        _ => motorcycle.y,
                    };

                    SetCursorPosition(motorcycle.x, motorcycle.y);
                    Console.Write("M");
                }
            }
        }

        static void DrawBorder()
        {
            Console.ForegroundColor = ConsoleColor.Red;

            for (int i = 0; i <= AREA_WIDTH; ++i)
            {
                SetCursorPosition(i, AREA_HEIGHT);
                Console.Write("*");
            }
            for (int i = 0; i <= AREA_HEIGHT; ++i)
            {
                SetCursorPosition(AREA_WIDTH, i);
                Console.Write("*");
            }

            Console.ResetColor();
        }

        static void SetCursorPosition(int x, int y)
        {
            Console.SetCursorPosition(x, y);
        }
    }

    class Car
    {
        public int x, y;
    }

    class Motorcycle
    {
        public int x, y;
    }
}
