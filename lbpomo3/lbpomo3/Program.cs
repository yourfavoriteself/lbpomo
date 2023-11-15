
namespace lb3
{
    class ChangeMethod
    {
        private ChangeMethod()
        {
        }

        public static string RunChangeMethod(double[][] A, int[] weight, double[] minimalValue, string[] alternative)
        {
            double[] normalizedWeight = NormalizeWeight(weight);
            Console.WriteLine("Нормализованный вектор весов: " + string.Join(", ", normalizedWeight));
            int columns = A[0].Length;

            // Поиск индекса главного критерия
            int index = Array.IndexOf(minimalValue, 1);

            // Поиск максимума и минимума столбцов
            double[] maxFound = new double[columns];
            double[] minFound = new double[columns];
            for (int j = 0; j < columns; j++)
            {
                maxFound[j] = FoundMax(A, j);
                minFound[j] = FoundMin(A, j);
            }

            Console.WriteLine($"Максимальные и минимальные элементы столбцов:\nМаксимумы: {string.Join(", ", maxFound)}\nМинимумы: {string.Join(", ", minFound)}");

            // Нормировка матрицы
            Console.WriteLine("Нормированная матрица A:");
            for (int i = 0; i < A.Length; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (j != index)
                    {
                        A[i][j] = (A[i][j] - minFound[j]) / (maxFound[j] - minFound[j]);
                    }
                }
            }

            foreach (var row in A)
            {
                Console.WriteLine(string.Join(", ", row));
            }

            // Повторно ищем максимумы и минимумы столбцов, чтобы провести проверку на удовлетворение условий минимальности
            for (int j = 0; j < columns; j++)
            {
                maxFound[j] = FoundMax(A, j);
            }
            for (int j = 0; j < columns; j++)
            {
                minFound[j] = FoundMin(A, j);
            }

            List<int> indexes = new List<int>();
            // Проверим минимальное значение для условий
            for (int i = 0; i < A.Length; i++)
            {
                for (int j = 0; j < A[i].Length; j++)
                {
                    if (A[i][j] == maxFound[j] && A[i][j] >= maxFound[j] * minimalValue[j])
                    {
                        indexes.Add(i);
                        break;
                    }
                }
            }

            // Теперь в indexes есть индексы строк, которые удовлетворяют минимальным критериям
            // Дальше нужно посчитать значение на нормализованный вес и отдать максимум
            Dictionary<int, double> values = new Dictionary<int, double>();
            foreach (int i in indexes)
            {
                double val = 0;
                bool hasZero = false; // Флаг для проверки наличия нулевых элементов
                for (int j = 0; j < columns; j++)
                {
                    if (j != index)
                    {
                        if (A[i][j] == 0)
                        {
                            hasZero = true; // Если есть нулевой элемент, устанавливаем флаг
                            break;
                        }
                        val += A[i][j] * normalizedWeight[j];
                    }
                }

                if (!hasZero)
                {
                    values.Add(i, val); // Добавляем в словарь только если нет нулевых элементов
                }
            }

            // Найти максимальное значение и соответствующий индекс
            double maxVal = double.NegativeInfinity;
            int maxIndex = -1;
            foreach (var entry in values)
            {
                if (entry.Value > maxVal)
                {
                    maxVal = entry.Value;
                    maxIndex = entry.Key;
                }
            }

            return alternative[maxIndex];
        }

        public static double FoundMax(double[][] A, int j)
        {
            double max = double.NegativeInfinity;
            foreach (var row in A)
            {
                if (row[j] > max)
                {
                    max = row[j];
                }
            }
            return max;
        }

        public static double FoundMin(double[][] A, int j)
        {
            double min = double.PositiveInfinity;
            foreach (var row in A)
            {
                if (row[j] < min)
                {
                    min = row[j];
                }
            }
            return min;
        }

        // Функция нормализации вектора весов
        public static double[] NormalizeWeight(int[] weight)
        {
            double sum = weight.Sum();
            // Создать массив для нормализованных весов
            double[] normalizedWeights = new double[weight.Length];
            // Нормализовать веса
            for (int i = 0; i < weight.Length; i++)
            {
                normalizedWeights[i] = (double)weight[i] / sum;
            }
            return normalizedWeights;
        }

        static void Main(string[] args)
        {
            // Матрица оценок для альтернатив
            double[][] A = {
                new double[] { 1, 7, 1, 8 },
                new double[] { 3, 4, 2, 10 },
                new double[] { 6, 3, 7, 4 },
                new double[] { 9, 1, 9, 1 },
            };
            // Вектор весов
            int[] w = { 8, 7, 9, 6 };
            // Допустимые уровни для критериев
            double[] a = { 1.0, 0.1, 0.8, 0.1 };
            string[] alternative = { "Автострада", "Шоссе", "Грунтовка", "Проселок" };
            string result = RunChangeMethod(A, w, a, alternative);
            string GREEN = "\u001B[32m";
            string RESET = "\u001B[0m";
            Console.WriteLine(GREEN + "Результат работы программы." + RESET);
            Console.WriteLine("Лучший выбор: " + result);
            Console.ReadLine();
        }
    }
}
