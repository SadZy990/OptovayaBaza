using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace OptovayaBaza.ParallelDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Сравнение последовательного и параллельного выполнения\n");

            var materials = GenerateMaterials(100000);

            Console.WriteLine("Задача: Подсчёт общей стоимости всех материалов\n");

            //ПОСЛЕДОВАТЕЛЬНО
            var stopwatch = Stopwatch.StartNew();
            decimal sumSequential = 0;
            foreach (var m in materials)
            {
                sumSequential += m.Price * m.Stock;
            }
            stopwatch.Stop();
            long timeSequential = stopwatch.ElapsedMilliseconds;

            Console.WriteLine($"Последовательно: {timeSequential} мс");
            Console.WriteLine($"Результат: {sumSequential:C}\n");

            //ПАРАЛЛЕЛЬНО
            stopwatch.Restart();
            decimal sumParallel = 0;
            object locker = new object();

            Parallel.ForEach(materials, material =>
            {
                decimal total = material.Price * material.Stock;
                lock (locker)
                {
                    sumParallel += total;
                }
            });
            stopwatch.Stop();
            long timeParallel = stopwatch.ElapsedMilliseconds;

            Console.WriteLine($"Параллельно: {timeParallel} мс");
            Console.WriteLine($"Результат: {sumParallel:C}\n");

            Console.WriteLine($"Ускорение: {Math.Round((double)timeSequential / timeParallel, 2)}x");
            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        /// <summary>
        /// Генерация списка материалов
        /// </summary>
        static List<Material> GenerateMaterials(int count)
        {
            var random = new Random();
            var list = new List<Material>();

            for (int i = 1; i <= count; i++)
            {
                list.Add(new Material
                {
                    Id = i,
                    Name = $"Материал_{i}",
                    Unit = "шт",
                    Price = random.Next(100, 5000),
                    Stock = random.Next(1, 100)
                });
            }
            return list;
        }
    }

    //чтобы не тянуть основной проект
    class Material
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}