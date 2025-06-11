using Geometryclass;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab14_Geometry
{
    class Program
    {
        static void Main(string[] args)
        {
            // Часть 1: Работа с коллекцией "Тетрадь" (Dictionary и Stack)
            Console.WriteLine(" Часть 1: Работа с коллекцией 'Тетрадь'");
            Part1_NotebookExample();

            // Часть 2: Работа с коллекцией из лабораторной работы 12
            Console.WriteLine("\n Часть 2: Работа с коллекцией из лаб. работы 12");
            Part2_HashTableExample();
        }

        static void Part1_NotebookExample()
        {
            // 1. Создаем коллекцию "Тетрадь" (Dictionary<int, Stack<Geometryfigure1>>)
            var notebook = new Dictionary<int, Stack<Geometryfigure1>>();
            var random = new Random();

            // Заполняем тетрадь 5 страницами
            for (int pageNum = 1; pageNum <= 5; pageNum++)
            {
                var stack = new Stack<Geometryfigure1>();
                int figuresCount = random.Next(3, 6); // 3-5 фигур на странице

                for (int i = 0; i < figuresCount; i++)
                {
                    Geometryfigure1 figure;
                    switch (random.Next(3))
                    {
                        case 0:
                            figure = new Circle1(random.Next(1, 10));
                            break;
                        case 1:
                            figure = new Rectangle1(random.Next(1, 10), random.Next(1, 10));
                            break;
                        case 2:
                            figure = new Parallelepiped1(random.Next(1, 5), random.Next(1, 5), random.Next(1, 5));
                            break;
                        default:
                            figure = new Circle1(1);
                            break;
                    }
                    stack.Push(figure);
                }
                notebook.Add(pageNum, stack);
            }

            // 2. Выполняем запросы LINQ

            // a) Выборка данных (фигуры с площадью > 30)
            Console.WriteLine("\n1. Фигуры с площадью > 30:");

            // Способ 1: LINQ запрос
            Console.WriteLine("\nLINQ запрос:");
            var largeFiguresLinq = from page in notebook
                                   from figure in page.Value
                                   where (figure as IGeometricFigure1)?.Area() > 30
                                   select figure;
            PrintFigures(largeFiguresLinq);

            // Способ 2: Методы расширения
            Console.WriteLine("\nМетоды расширения:");
            var largeFiguresExt = notebook.SelectMany(p => p.Value)
                                       .Where(f => (f as IGeometricFigure1)?.Area() > 30);
            PrintFigures(largeFiguresExt);

            // Способ 3: Без методов расширения (for)
            Console.WriteLine("\nБез методов расширения (for):");
            var largeFiguresManual = new List<Geometryfigure1>();
            foreach (var page in notebook)
            {
                foreach (var figure in page.Value)
                {
                    if ((figure as IGeometricFigure1)?.Area() > 30)
                    {
                        largeFiguresManual.Add(figure);
                    }
                }
            }
            PrintFigures(largeFiguresManual);

            // b) Операции над множествами (разность между страницами 1 и 2)
            Console.WriteLine("\n2. Разность множеств (страница 1 \\ страница 2):");

            // LINQ запрос
            Console.WriteLine("\nLINQ запрос:");
            var diffLinq = from figure in notebook[1]
                           where !notebook[2].Contains(figure, new FigureComparer())
                           select figure;
            PrintFigures(diffLinq);

            // Методы расширения
            Console.WriteLine("\nМетоды расширения:");
            var diffExt = notebook[1].Except(notebook[2], new FigureComparer());
            PrintFigures(diffExt);

            // c) Агрегирование данных
            Console.WriteLine("\n3. Агрегирование данных:");

            // LINQ запрос
            Console.WriteLine("\nLINQ запрос:");
            var aggLinq = from page in notebook
                          from figure in page.Value
                          let geoFig = figure as IGeometricFigure1
                          where geoFig != null
                          select geoFig.Area();

            Console.WriteLine($"Всего фигур: {aggLinq.Count()}");
            Console.WriteLine($"Макс. площадь: {aggLinq.Max():F2}");
            Console.WriteLine($"Мин. площадь: {aggLinq.Min():F2}");
            Console.WriteLine($"Сред. площадь: {aggLinq.Average():F2}");
            Console.WriteLine($"Сумма площадей: {aggLinq.Sum():F2}");

            // Методы расширения
            Console.WriteLine("\nМетоды расширения:");
            var aggExt = notebook.SelectMany(p => p.Value)
                                .OfType<IGeometricFigure1>()
                                .Select(f => f.Area());

            Console.WriteLine($"Всего фигур: {aggExt.Count()}");
            Console.WriteLine($"Макс. площадь: {aggExt.Max():F2}");
            Console.WriteLine($"Мин. площадь: {aggExt.Min():F2}");
            Console.WriteLine($"Сред. площадь: {aggExt.Average():F2}");
            Console.WriteLine($"Сумма площадей: {aggExt.Sum():F2}");

            // d) Группировка данных по типу фигуры
            Console.WriteLine("\n4. Группировка по типу фигуры:");

            // LINQ запрос
            Console.WriteLine("\nLINQ запрос:");
            var groupLinq = from figure in notebook.SelectMany(p => p.Value)
                            group figure by figure.GetType().Name into g
                            orderby g.Count() descending
                            select new { Type = g.Key, Count = g.Count(), Figures = g };

            foreach (var group in groupLinq)
            {
                Console.WriteLine($"{group.Type}: {group.Count} шт.");
                PrintFigures(group.Figures.Take(2));
                Console.WriteLine();
            }

            // Методы расширения
            Console.WriteLine("\nМетоды расширения:");
            var groupExt = notebook.SelectMany(p => p.Value)
                                 .GroupBy(f => f.GetType().Name)
                                 .OrderByDescending(g => g.Count())
                                 .Select(g => new { Type = g.Key, Count = g.Count(), Figures = g });

            foreach (var group in groupExt)
            {
                Console.WriteLine($"{group.Type}: {group.Count} шт.");
                PrintFigures(group.Figures.Take(2));
                Console.WriteLine();
            }

            // e) Получение нового типа (проекция)
            Console.WriteLine("\n5. Проекция (информация о страницах):");

            // LINQ запрос
            Console.WriteLine("\nLINQ запрос:");
            var projectionLinq = from page in notebook
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

            foreach (var info in projectionLinq)
            {
                Console.WriteLine($"Страница {info.PageNumber}: " +
                                $"{info.FiguresCount} фигур ({info.CirclesCount} кругов), " +
                                $"Общая площадь: {info.TotalArea:F2}");
            }

            // Методы расширения
            Console.WriteLine("\nМетоды расширения:");
            var projectionExt = notebook.Select(page => new
            {
                PageNumber = page.Key,
                FiguresCount = page.Value.Count,
                CirclesCount = page.Value.OfType<Circle1>().Count(),
                TotalArea = page.Value.OfType<IGeometricFigure1>().Sum(f => f.Area())
            });

            foreach (var info in projectionExt)
            {
                Console.WriteLine($"Страница {info.PageNumber}: " +
                                $"{info.FiguresCount} фигур ({info.CirclesCount} кругов), " +
                                $"Общая площадь: {info.TotalArea:F2}");
            }

            // f) Соединение (Join с дополнительными данными)
            Console.WriteLine("\n6. Соединение с дополнительными данными:");
            var pageColors = new Dictionary<int, string>
            {
                {1, "Красная"}, {2, "Синяя"}, {3, "Зеленая"}, {4, "Желтая"}, {5, "Фиолетовая"}
            };

            // LINQ запрос
            Console.WriteLine("\nLINQ запрос:");
            var joinLinq = from page in notebook
                           join color in pageColors on page.Key equals color.Key
                           select new
                           {
                               Page = page.Key,
                               Color = color.Value,
                               FiguresCount = page.Value.Count
                           };

            foreach (var item in joinLinq)
            {
                Console.WriteLine($"Страница {item.Page} ({item.Color}): {item.FiguresCount} фигур");
            }

            // Методы расширения
            Console.WriteLine("\nМетоды расширения:");
            var joinExt = notebook.Join(pageColors,
                                      page => page.Key,
                                      color => color.Key,
                                      (page, color) => new
                                      {
                                          Page = page.Key,
                                          Color = color.Value,
                                          FiguresCount = page.Value.Count
                                      });

            foreach (var item in joinExt)
            {
                Console.WriteLine($"Страница {item.Page} ({item.Color}): {item.FiguresCount} фигур");
            }
        }

        static void Part2_HashTableExample()
        {
            // Создаем и заполняем хеш-таблицу 
            var hashTable = new Dictionary<int, Geometryfigure1>();
            var random = new Random();

            for (int i = 1; i <= 20; i++)
            {
                Geometryfigure1 figure;
                switch (random.Next(3))
                {
                    case 0:
                        figure = new Circle1(random.Next(1, 10));
                        break;
                    case 1:
                        figure = new Rectangle1(random.Next(1, 10), random.Next(1, 10));
                        break;
                    case 2:
                        figure = new Parallelepiped1(random.Next(1, 5), random.Next(1, 5), random.Next(1, 5));
                        break;
                    default:
                        figure = new Circle1(1);
                        break;
                }
                hashTable.Add(i, figure);
            }

            // Выполняем запросы к хеш-таблице

            // a) Выборка данных (фигуры с площадью > 20)
            Console.WriteLine("\n1. Фигуры с площадью > 20:");

            // LINQ запрос
            Console.WriteLine("\nLINQ запрос:");
            var largeFiguresLinq = from kv in hashTable
                                   where (kv.Value as IGeometricFigure1)?.Area() > 20
                                   select kv.Value;
            PrintFigures(largeFiguresLinq);

            // Методы расширения
            Console.WriteLine("\nМетоды расширения:");
            var largeFiguresExt = hashTable.Where(kv => (kv.Value as IGeometricFigure1)?.Area() > 20)
                                         .Select(kv => kv.Value);
            PrintFigures(largeFiguresExt);

            // b) Получение счетчика (количество прямоугольников)
            Console.WriteLine("\n2. Количество прямоугольников:");

            // LINQ запрос
            Console.WriteLine("\nLINQ запрос:");
            var rectCountLinq = (from kv in hashTable
                                 where kv.Value is Rectangle1
                                 select kv).Count();
            Console.WriteLine(rectCountLinq);

            // Методы расширения
            Console.WriteLine("\nМетоды расширения:");
            var rectCountExt = hashTable.Count(kv => kv.Value is Rectangle1);
            Console.WriteLine(rectCountExt);

            // c) Агрегирование данных
            Console.WriteLine("\n3. Агрегирование данных:");

            // LINQ запрос
            Console.WriteLine("\nLINQ запрос:");
            var aggLinq = from kv in hashTable
                          let geoFig = kv.Value as IGeometricFigure1
                          where geoFig != null
                          select geoFig.Area();

            Console.WriteLine($"Макс. площадь: {aggLinq.Max():F2}");
            Console.WriteLine($"Мин. площадь: {aggLinq.Min():F2}");
            Console.WriteLine($"Сред. площадь: {aggLinq.Average():F2}");
            Console.WriteLine($"Сумма площадей: {aggLinq.Sum():F2}");

            // Методы расширения
            Console.WriteLine("\nМетоды расширения:");
            var aggExt = hashTable.Values.OfType<IGeometricFigure1>().Select(f => f.Area());

            Console.WriteLine($"Макс. площадь: {aggExt.Max():F2}");
            Console.WriteLine($"Мин. площадь: {aggExt.Min():F2}");
            Console.WriteLine($"Сред. площадь: {aggExt.Average():F2}");
            Console.WriteLine($"Сумма площадей: {aggExt.Sum():F2}");

            // d) Группировка данных по типу фигуры
            Console.WriteLine("\n4. Группировка по типу фигуры:");

            // LINQ запрос
            Console.WriteLine("\nLINQ запрос:");
            var groupLinq = from kv in hashTable
                            group kv by kv.Value.GetType().Name into g
                            orderby g.Count() descending
                            select new { Type = g.Key, Count = g.Count(), Figures = g.Select(kv => kv.Value) };

            foreach (var group in groupLinq)
            {
                Console.WriteLine($"{group.Type}: {group.Count} шт.");
                PrintFigures(group.Figures.Take(2));
                Console.WriteLine();
            }

            // Методы расширения
            Console.WriteLine("\nМетоды расширения:");
            var groupExt = hashTable.GroupBy(kv => kv.Value.GetType().Name)
                                  .OrderByDescending(g => g.Count())
                                  .Select(g => new { Type = g.Key, Count = g.Count(), Figures = g.Select(kv => kv.Value) });

            foreach (var group in groupExt)
            {
                Console.WriteLine($"{group.Type}: {group.Count} шт.");
                PrintFigures(group.Figures.Take(2));
                Console.WriteLine();
            }
        }

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
            if (obj is Circle1 c)
                return c.Radius.GetHashCode();

            if (obj is Rectangle1 r)
                return (r.Length, r.Width).GetHashCode();

            if (obj is Parallelepiped1 p)
                return (p.Length, p.Width, p.Height).GetHashCode();

            return obj.GetHashCode();
        }
    }
}