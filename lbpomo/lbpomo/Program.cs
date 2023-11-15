using Gtk;
using Cairo;

namespace ParetoMethodCSharp
{
    public class ParetoMethod
    {
        public class Point
        {
            public double X { get; set; }
            public double Y { get; set; }

            public Point(double x, double y)
            {
                X = x;
                Y = y;
            }

            public override string ToString()
            {
                return $"({X};{Y})";
            }
        }

        private static bool resultDisplayed = false; // Move the variable here

        private static double ManhattanLength(Point p1, Point p2)
        {
            return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
        }

        private static void DisplayParetoGraph(List<Point> setPareto, Point utopiaPoint)
        {
            Application.Init();

            double minX = 0;
            double minY = 0;
            double maxX = setPareto.Max(point => point.X);
            double maxY = setPareto.Max(point => point.Y);

            minX = Math.Min(minX, utopiaPoint.X);
            minY = Math.Min(minY, utopiaPoint.Y);
            maxX = Math.Max(maxX, utopiaPoint.X);
            maxY = Math.Max(maxY, utopiaPoint.Y);

            double scale = 50;
            int padding = 100;
            int windowWidth = (int)((maxX - minX) * scale) + 2 * padding;
            int windowHeight = (int)((maxY - minY) * scale) + 2 * padding;

            Window window = new Window("Множество Парето");
            window.Resize(windowWidth, windowHeight);

            DrawingArea area = new DrawingArea();
            area.Drawn += (o, args) => OnDrawEvent(area, setPareto, utopiaPoint);

            window.Add(area);
            window.ShowAll();

            window.Destroyed += (sender, e) => Application.Quit();

            Application.Run();
        }

        
        private static void OnDrawEvent(DrawingArea area, List<Point> setPareto, Point utopiaPoint)
        {
            Cairo.Context cr = Gdk.CairoHelper.Create(area.GdkWindow);

            double minX = 0;
            double minY = 0;
            double maxX = setPareto.Max(point => point.X);
            double maxY = setPareto.Max(point => point.Y);

            minX = Math.Min(minX, utopiaPoint.X);
            minY = Math.Min(minY, utopiaPoint.Y);
            maxX = Math.Max(maxX, utopiaPoint.X);
            maxY = Math.Max(maxY, utopiaPoint.Y);

            double scale = 50;
            int padding = 100;

            cr.SetSourceRGB(1, 1, 1);
            cr.Rectangle(0, 0, area.Allocation.Width, area.Allocation.Height);
            cr.Fill();

            cr.SetSourceRGB(0, 0, 0);
            cr.LineWidth = 1;

            for (int y = (int)Math.Ceiling(minY); y <= (int)Math.Floor(maxY); y++)
            {
                int yPos = (int)((maxY - y) * scale) + padding;
                cr.MoveTo(padding, yPos);
                cr.LineTo(area.Allocation.Width - padding, yPos);
                cr.Stroke();

                cr.SelectFontFace("Arial", FontSlant.Normal, FontWeight.Normal);
                cr.SetFontSize(12);
                cr.MoveTo(padding - 35, yPos + 5);
                cr.ShowText(y.ToString());
            }

            for (int x = (int)Math.Ceiling(minX); x <= (int)Math.Floor(maxX); x++)
            {
                int xPos = (int)((x - minX) * scale) + padding;
                cr.MoveTo(xPos, padding);
                cr.LineTo(xPos, area.Allocation.Height - padding);
                cr.Stroke();

                cr.SelectFontFace("Arial", FontSlant.Normal, FontWeight.Normal);
                cr.SetFontSize(12);
                cr.MoveTo(xPos - 5, area.Allocation.Height - padding + 15);
                cr.ShowText(x.ToString());
            }

            cr.SetSourceRGB(0, 0, 0);
            cr.LineWidth = 1;

            setPareto = setPareto.OrderBy(p => p.X).ToList();

            for (int i = 0; i < setPareto.Count - 1; i++)
            {
                int x1 = (int)((setPareto[i].X - minX) * scale) + padding;
                int y1 = (int)((maxY - setPareto[i].Y) * scale) + padding;

                int x2 = (int)((setPareto[i + 1].X - minX) * scale) + padding;
                int y2 = (int)((maxY - setPareto[i + 1].Y) * scale) + padding;

                cr.MoveTo(x1, y1);
                cr.LineTo(x2, y2);
                cr.Stroke();
            }

            Point bestPoint = setPareto[0];
            foreach (Point point in setPareto)
            {
                if (ManhattanLength(utopiaPoint, point) < ManhattanLength(utopiaPoint, bestPoint))
                {
                    bestPoint = point;
                }
            }

            int utopiaX = (int)((utopiaPoint.X - minX) * scale) + padding;
            int utopiaY = (int)((maxY - utopiaPoint.Y) * scale) + padding;

            cr.SetSourceRGB(1, 0, 0);
            cr.Arc(utopiaX, utopiaY, 5.5, 0, 2 * Math.PI);
            cr.Fill();

            cr.SetSourceRGB(0, 0, 0);
            cr.SelectFontFace("Arial", FontSlant.Normal, FontWeight.Normal);
            cr.SetFontSize(12);
            cr.MoveTo(utopiaX, utopiaY - 10);
            cr.ShowText("Точка утопии");

            cr.Dispose();

            if (!resultDisplayed)
            {
                string[] alternatives = { "Автострада", "Шоссе", "Грунтовка", "Проселок" };
                Console.WriteLine($"Оптимальный результат: {alternatives[Array.IndexOf(setPareto.ToArray(), bestPoint)]}");
                resultDisplayed = true;
            }
        }




        private static string RunPareto(double[][] A, string[] alternative, int ind1, int ind2)
        {
            Point utopiaPoint = new Point(10.0, 10.0);
            List<Point> setPareto = A.Select(row => new Point(row[ind1], row[ind2])).ToList();

            Console.Write("Множество Парето: ");
            foreach (Point p in setPareto)
            {
                Console.Write(p.ToString() + " ");
            }
            Console.WriteLine();

            DisplayParetoGraph(setPareto, utopiaPoint);

            Point bestPoint = setPareto[0];
            foreach (Point point in setPareto)
            {
                if (ManhattanLength(utopiaPoint, point) < ManhattanLength(utopiaPoint, bestPoint))
                {
                    bestPoint = point;
                }
            }

            int index = -1;
            double minDistance = double.MaxValue;

            for (int i = 0; i < A.Length; i++)
            {
                double distance = ManhattanLength(utopiaPoint, new Point(A[i][ind1], A[i][ind2]));
                if (distance < minDistance)
                {
                    minDistance = distance;
                    index = i;
                }
            }

            if (index != -1)
            {
                Console.WriteLine($"Оптимальный результат: {alternative[index]}");
                return alternative[index];
            }
            else
            {
                Console.WriteLine("Оптимальный результат не найден");
                return null;
            }
        }

        static void Main()
        {
            string[] alternative = { "Автострада", "Шоссе", "Грунтовка", "Проселок" };
            double[][] A =
            {
                new double[] { 1, 1 },
                new double[] { 3, 2 },
                new double[] { 6, 7 },
                new double[] { 9, 9 }
            };

            Console.WriteLine(RunPareto(A, alternative, 0, 1));
        }
    }
}