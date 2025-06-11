using Geometryclass;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Lab14_Geometry
{
    class Program
    {
        private static Dictionary<int, Stack<Geometryfigure1>> notebook;
        private static Dictionary<int, Geometryfigure1> hashTable;
        private static Random random = new Random();

        static void Main(string[] args)
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nГлавное меню ");
                Console.WriteLine("1. Работа с коллекцией 'Тетрадь'");
                Console.WriteLine("2. Работа с хеш-таблицей из лаб. работы 12");
                Console.WriteLine("3. Выход");
                Console.Write("Выберите пункт меню: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        NotebookMenu();
                        break;
                    case "2":
                        HashTableMenu();
                        break;
                    case "3":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Неверный ввод!");
                        break;
                }
            }
        }

        static void NotebookMenu()
        {
            notebook = CreateNotebook(5); // Создаем тетрадь с 5 страницами

            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n Меню работы с тетрадью ");
                Console.WriteLine("1. Выборка данных (фигуры с площадью > 30)");
                Console.WriteLine("2. Операции над множествами (разность страниц)");
                Console.WriteLine("3. Агрегирование данных");
                Console.WriteLine("4. Группировка данных по типу фигуры");
                Console.WriteLine("5. Проекция (информация о страницах)");
                Console.WriteLine("6. Соединение с дополнительными данными");
                Console.WriteLine("7. Сравнение производительности");
                Console.WriteLine("8. Назад");
                Console.Write("Выберите пункт меню: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        ExecuteSelectionQuery();
                        break;
                    case "2":
                        ExecuteSetOperation();
                        break;
                    case "3":
                        ExecuteAggregation();
                        break;
                    case "4":
                        ExecuteGrouping();
                        break;
                    case "5":
                        ExecuteProjection();
                        break;
                    case "6":
                        ExecuteJoin();
                        break;
                    case "7":
                        ComparePerformance();
                        break;
                    case "8":
                        back = true;
                        break;
                    default:
                        Console.WriteLine("Неверный ввод!");
                        break;
                }
            }
        }

        static void HashTableMenu()
        {
            hashTable = CreateHashTable(20); // Создаем хеш-таблицу с 20 элементами

            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n Меню работы с хеш-таблицей ");
                Console.WriteLine("1. Выборка данных (фигуры с площадью > 20)");
                Console.WriteLine("2. Получение счетчика (количество прямоугольников)");
                Console.WriteLine("3. Агрегирование данных");
                Console.WriteLine("4. Группировка данных по типу фигуры");
                Console.WriteLine("5. Назад");
                Console.Write("Выберите пункт меню: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        ExecuteHashTableSelection();
                        break;
                    case "2":
                        ExecuteHashTableCount();
                        break;
                    case "3":
                        ExecuteHashTableAggregation();
                        break;
                    case "4":
                        ExecuteHashTableGrouping();
                        break;
                    case "5":
                        back = true;
                        break;
                    default:
                        Console.WriteLine("Неверный ввод!");
                        break;
                }
            }
        }

        #region Создание коллекций
        static Dictionary<int, Stack<Geometryfigure1>> CreateNotebook(int pages)
        {
            var notebook = new Dictionary<int, Stack<Geometryfigure1>>();

            for (int pageNum = 1; pageNum <= pages; pageNum++)
            {
                var stack = new Stack<Geometryfigure1>();
                int figuresCount = random.Next(3, 6); // 3-5 фигур на странице

                for (int i = 0; i < figuresCount; i++)
                {
                    Geometryfigure1 figure = CreateRandomFigure();
                    stack.Push(figure);
                }
                notebook.Add(pageNum, stack);
            }

            Console.WriteLine($"Создана тетрадь с {pages} страницами, всего фигур: {notebook.Sum(p => p.Value.Count)}");
            return notebook;
        }

        static Dictionary<int, Geometryfigure1> CreateHashTable(int size)
        {
            var table = new Dictionary<int, Geometryfigure1>();

            for (int i = 1; i <= size; i++)
            {
                Geometryfigure1 figure = CreateRandomFigure();
                table.Add(i, figure);
            }

            Console.WriteLine($"Создана хеш-таблица с {size} элементами");
            return table;
        }

        static Geometryfigure1 CreateRandomFigure()
        {
            switch (random.Next(3))
            {
                case 0:
                    return new Circle1(random.Next(1, 10));
                case 1:
                    return new Rectangle1(random.Next(1, 10), random.Next(1, 10));
                case 2:
                    return new Parallelepiped1(random.Next(1, 5), random.Next(1, 5), random.Next(1, 5));
                default:
                    return new Circle1(1);
            }
        }
        #endregion

        #region Запросы для тетради
        static void ExecuteSelectionQuery()
        {
            Console.WriteLine("\n Выборка данных (фигуры с площадью > 30) ");

            // LINQ запрос
            Console.WriteLine("\nLINQ запрос:");
            var linqResult = from page in notebook
                             from figure in page.Value
                             where (figure as IGeometricFigure1)?.Area() > 30
                             select figure;
            PrintFigures(linqResult);

            // Методы расширения
            Console.WriteLine("\nМетоды расширения:");
            var extResult = notebook.SelectMany(p => p.Value)
                                   .Where(f => (f as IGeometricFigure1)?.Area() > 30);
            PrintFigures(extResult);

            // Без методов расширения
            Console.WriteLine("\nБез методов расширения (for):");
            var manualResult = new List<Geometryfigure1>();
            foreach (var page in notebook)
            {
                foreach (var figure in page.Value)
                {
                    if ((figure as IGeometricFigure1)?.Area() > 30)
                    {
                        manualResult.Add(figure);
                    }
                }
            }
            PrintFigures(manualResult);
        }

        static void ExecuteSetOperation()
        {
            Console.WriteLine("\n Разность множеств (страница 1 \\ страница 2) ");

            if (notebook.Count < 2)
            {
                Console.WriteLine("Недостаточно страниц для операции!");
                return;
            }

            // LINQ запрос
            Console.WriteLine("\nLINQ запрос:");
            var linqResult = from figure in notebook[1]
                             where !notebook[2].Contains(figure, new FigureComparer())
                             select figure;
            PrintFigures(linqResult);

            // Методы расширения
            Console.WriteLine("\nМетоды расширения:");
            var extResult = notebook[1].Except(notebook[2], new FigureComparer());
            PrintFigures(extResult);
        }

        static void ExecuteAggregation()
        {
            Console.WriteLine("\nАгрегирование данных ");

            // LINQ запрос
            Console.WriteLine("\nLINQ запрос:");
            var linqResult = from page in notebook
                             from figure in page.Value
                             let geoFig = figure as IGeometricFigure1
                             where geoFig != null
                             select geoFig.Area();

            Console.WriteLine($"Всего фигур: {linqResult.Count()}");
            Console.WriteLine($"Макс. площадь: {linqResult.Max():F2}");
            Console.WriteLine($"Мин. площадь: {linqResult.Min():F2}");
            Console.WriteLine($"Сред. площадь: {linqResult.Average():F2}");
            Console.WriteLine($"Сумма площадей: {linqResult.Sum():F2}");

            // Методы расширения
            Console.WriteLine("\nМетоды расширения:");
            var extResult = notebook.SelectMany(p => p.Value)
                                  .OfType<IGeometricFigure1>()
                                  .Select(f => f.Area());

            Console.WriteLine($"Всего фигур: {extResult.Count()}");
            Console.WriteLine($"Макс. площадь: {extResult.Max():F2}");
            Console.WriteLine($"Мин. площадь: {extResult.Min():F2}");
            Console.WriteLine($"Сред. площадь: {extResult.Average():F2}");
            Console.WriteLine($"Сумма площадей: {extResult.Sum():F2}");
        }

        static void ExecuteGrouping()
        {
            Console.WriteLine("\nГруппировка по типу фигуры ");

            // LINQ запрос
            Console.WriteLine("\nLINQ запрос:");
            var linqResult = from figure in notebook.SelectMany(p => p.Value)
                             group figure by figure.GetType().Name into g
                             orderby g.Count() descending
                             select new { Type = g.Key, Count = g.Count(), Figures = g };

            foreach (var group in linqResult)
            {
                Console.WriteLine($"{group.Type}: {group.Count} шт.");
                PrintFigures(group.Figures.Take(2));
                Console.WriteLine();
            }

            // Методы расширения
            Console.WriteLine("\nМетоды расширения:");
            var extResult = notebook.SelectMany(p => p.Value)
                                  .GroupBy(f => f.GetType().Name)
                                  .OrderByDescending(g => g.Count())
                                  .Select(g => new { Type = g.Key, Count = g.Count(), Figures = g });

            foreach (var group in extResult)
            {
                Console.WriteLine($"{group.Type}: {group.Count} шт.");
                PrintFigures(group.Figures.Take(2));
                Console.WriteLine();
            }
        }

        static void ExecuteProjection()
        {
            Console.WriteLine("\nПроекция (информация о страницах) ");

            // LINQ запрос
            Console.WriteLine("\nLINQ запрос:");
            var linqResult = from page in notebook
                             select new
                             {
                                 PageNumber = page.Key,
                                 FiguresCount = page.Value.Count,
                                 CirclesCount = (from figure in page.Value
                                                 where figure is Circle1
                                                 select figure).Count(),
                                 TotalArea = (from figure in page.Value
                                              let geoFig = figure as IGeometricFigure1
                                              where geoFig != null
                                              select geoFig.Area()).Sum()
                             };

            foreach (var info in linqResult)
            {
                Console.WriteLine($"Страница {info.PageNumber}: " +
                                $"{info.FiguresCount} фигур ({info.CirclesCount} кругов), " +
                                $"Общая площадь: {info.TotalArea:F2}");
            }

            // Методы расширения
            Console.WriteLine("\nМетоды расширения:");
            var extResult = notebook.Select(page => new
            {
                PageNumber = page.Key,
                FiguresCount = page.Value.Count,
                CirclesCount = page.Value.OfType<Circle1>().Count(),
                TotalArea = page.Value.OfType<IGeometricFigure1>().Sum(f => f.Area())
            });

            foreach (var info in extResult)
            {
                Console.WriteLine($"Страница {info.PageNumber}: " +
                                $"{info.FiguresCount} фигур ({info.CirclesCount} кругов), " +
                                $"Общая площадь: {info.TotalArea:F2}");
            }
        }

        static void ExecuteJoin()
        {
            Console.WriteLine("\n Соединение с дополнительными данными ");

            // Новый тип данных для соединения 
            var pageColors = new Dictionary<int, PageInfo>
            {
                {1, new PageInfo { Color = "Красная", Author = "Иванов" }},
                {2, new PageInfo { Color = "Синяя", Author = "Петров" }},
                {3, new PageInfo { Color = "Зеленая", Author = "Сидоров" }},
                {4, new PageInfo { Color = "Желтая", Author = "Кузнецов" }},
                {5, new PageInfo { Color = "Фиолетовая", Author = "Васильев" }}
            };

            // LINQ запрос
            Console.WriteLine("\nLINQ запрос:");
            var linqResult = from page in notebook
                             join info in pageColors on page.Key equals info.Key
                             select new
                             {
                                 Page = page.Key,
                                 Color = info.Value.Color,
                                 Author = info.Value.Author,
                                 FiguresCount = page.Value.Count
                             };

            foreach (var item in linqResult)
            {
                Console.WriteLine($"Страница {item.Page} ({item.Color}, автор: {item.Author}): {item.FiguresCount} фигур");
            }

            // Методы расширения
            Console.WriteLine("\nМетоды расширения:");
            var extResult = notebook.Join(pageColors,
                                         page => page.Key,
                                         info => info.Key,
                                         (page, info) => new
                                         {
                                             Page = page.Key,
                                             Color = info.Value.Color,
                                             Author = info.Value.Author,
                                             FiguresCount = page.Value.Count
                                         });

            foreach (var item in extResult)
            {
                Console.WriteLine($"Страница {item.Page} ({item.Color}, автор: {item.Author}): {item.FiguresCount} фигур");
            }
        }

        static void ComparePerformance()
        {
            Console.WriteLine("\n Сравнение производительности ");

            // Создаем большую коллекцию для тестов
            var largeNotebook = CreateNotebook(1000);

            // 1. С методом расширения (Where)
            var stopwatch = Stopwatch.StartNew();
            var resultExt = largeNotebook.SelectMany(p => p.Value)
                                        .Where(f => (f as IGeometricFigure1)?.Area() > 30)
                                        .ToList();
            stopwatch.Stop();
            long timeExt = stopwatch.ElapsedTicks;

            // 2. Без методов расширения (for)
            stopwatch.Restart();
            var resultManual = new List<Geometryfigure1>();
            foreach (var page in largeNotebook)
            {
                foreach (var figure in page.Value)
                {
                    if ((figure as IGeometricFigure1)?.Area() > 30)
                    {
                        resultManual.Add(figure);
                    }
                }
            }
            stopwatch.Stop();
            long timeManual = stopwatch.ElapsedTicks;

            // 3. LINQ запрос
            stopwatch.Restart();
            var resultLinq = (from page in largeNotebook
                              from figure in page.Value
                              where (figure as IGeometricFigure1)?.Area() > 30
                              select figure).ToList();
            stopwatch.Stop();
            long timeLinq = stopwatch.ElapsedTicks;

            // Выводим результаты
            Console.WriteLine("\nРезультаты измерения времени:");
            Console.WriteLine($"Методы расширения: {timeExt} тиков, найдено {resultExt.Count} элементов");
            Console.WriteLine($"LINQ запрос:       {timeLinq} тиков, найдено {resultLinq.Count} элементов");
            Console.WriteLine($"Ручная реализация: {timeManual} тиков, найдено {resultManual.Count} элементов");

            // Анализ результатов
            Console.WriteLine("\nАнализ результатов:");
            Console.WriteLine($"Отношение скорости (расширения/ручная): {(double)timeExt / timeManual:F2}");
            Console.WriteLine($"Отношение скорости (LINQ/ручная):       {(double)timeLinq / timeManual:F2}");
        }
        #endregion

        #region Запросы для хеш-таблицы
        static void ExecuteHashTableSelection()
        {
            Console.WriteLine("\n Выборка данных (фигуры с площадью > 20) ");

            // LINQ запрос
            Console.WriteLine("\nLINQ запрос:");
            var linqResult = from kv in hashTable
                             where (kv.Value as IGeometricFigure1)?.Area() > 20
                             select kv.Value;
            PrintFigures(linqResult);

            // Методы расширения
            Console.WriteLine("\nМетоды расширения:");
            var extResult = hashTable.Where(kv => (kv.Value as IGeometricFigure1)?.Area() > 20)
                                   .Select(kv => kv.Value);
            PrintFigures(extResult);
        }

        static void ExecuteHashTableCount()
        {
            Console.WriteLine("\n Количество прямоугольников ");

            // LINQ запрос
            Console.WriteLine("\nLINQ запрос:");
            var linqResult = (from kv in hashTable
                              where kv.Value is Rectangle1
                              select kv).Count();
            Console.WriteLine($"Прямоугольников: {linqResult}");

            // Методы расширения
            Console.WriteLine("\nМетоды расширения:");
            var extResult = hashTable.Count(kv => kv.Value is Rectangle1);
            Console.WriteLine($"Прямоугольников: {extResult}");
        }

        static void ExecuteHashTableAggregation()
        {
            Console.WriteLine("\n Агрегирование данных");

            // LINQ запрос
            Console.WriteLine("\nLINQ запрос:");
            var linqResult = from kv in hashTable
                             let geoFig = kv.Value as IGeometricFigure1
                             where geoFig != null
                             select geoFig.Area();

            Console.WriteLine($"Макс. площадь: {linqResult.Max():F2}");
            Console.WriteLine($"Мин. площадь: {linqResult.Min():F2}");
            Console.WriteLine($"Сред. площадь: {linqResult.Average():F2}");
            Console.WriteLine($"Сумма площадей: {linqResult.Sum():F2}");

            // Методы расширения
            Console.WriteLine("\nМетоды расширения:");
            var extResult = hashTable.Values.OfType<IGeometricFigure1>()
                                   .Select(f => f.Area());

            Console.WriteLine($"Макс. площадь: {extResult.Max():F2}");
            Console.WriteLine($"Мин. площадь: {extResult.Min():F2}");
            Console.WriteLine($"Сред. площадь: {extResult.Average():F2}");
            Console.WriteLine($"Сумма площадей: {extResult.Sum():F2}");
        }

        static void ExecuteHashTableGrouping()
        {
            Console.WriteLine("\n Группировка по типу фигуры");

            // LINQ запрос
            Console.WriteLine("\nLINQ запрос:");
            var linqResult = from kv in hashTable
                             group kv by kv.Value.GetType().Name into g
                             orderby g.Count() descending
                             select new { Type = g.Key, Count = g.Count(), Figures = g.Select(kv => kv.Value) };

            foreach (var group in linqResult)
            {
                Console.WriteLine($"{group.Type}: {group.Count} шт.");
                PrintFigures(group.Figures.Take(2));
                Console.WriteLine();
            }

            // Методы расширения
            Console.WriteLine("\nМетоды расширения:");
            var extResult = hashTable.GroupBy(kv => kv.Value.GetType().Name)
                                   .OrderByDescending(g => g.Count())
                                   .Select(g => new { Type = g.Key, Count = g.Count(), Figures = g.Select(kv => kv.Value) });

            foreach (var group in extResult)
            {
                Console.WriteLine($"{group.Type}: {group.Count} шт.");
                PrintFigures(group.Figures.Take(2));
                Console.WriteLine();
            }
        }
        #endregion

        static void PrintFigures(IEnumerable<Geometryfigure1> figures)
        {
            foreach (var figure in figures)
            {
                Console.WriteLine(figure);
                if (figure is IGeometricFigure1 geoFigure)
                {
                    Console.WriteLine($"  Площадь: {geoFigure.Area():F2}");
                    if (figure is Parallelepiped1 p)
                        Console.WriteLine($"  Объем: {p.Volume():F2}");
                }
                Console.WriteLine();
            }
        }
    }

    // Компаратор для сравнения геометрических фигур
    public class FigureComparer : IEqualityComparer<Geometryfigure1>
    {
        public bool Equals(Geometryfigure1 x, Geometryfigure1 y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            if (x.GetType() != y.GetType()) return false;

            if (x is Circle1 c1 && y is Circle1 c2)
                return c1.Radius == c2.Radius;

            if (x is Rectangle1 r1 && y is Rectangle1 r2)
                return r1.Length == r2.Length && r1.Width == r2.Width;

            if (x is Parallelepiped1 p1 && y is Parallelepiped1 p2)
                return p1.Length == p2.Length && p1.Width == p2.Width && p1.Height == p2.Height;

            return false;
        }

        public int GetHashCode(Geometryfigure1 obj)
        {
            if (obj is Circle1 c) return c.Radius.GetHashCode();
            if (obj is Rectangle1 r) return (r.Length, r.Width).GetHashCode();
            if (obj is Parallelepiped1 p) return (p.Length, p.Width, p.Height).GetHashCode();
            return obj.GetHashCode();
        }
    }
}