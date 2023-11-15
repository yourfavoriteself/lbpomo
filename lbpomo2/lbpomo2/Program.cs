

namespace CriteriaCombination
{
    class Program
    {
        static void Main(string[] args)
        {
            // Карта для γ
            Dictionary<string, double> markCriteria = new Dictionary<string, double>
            {
                {"12", 0.5},
                {"13", 1.0},
                {"14", 1.0},
                {"21", 0.5},
                {"23", 1.0},
                {"24", 1.0},
                {"31", 0.0},
                {"32", 0.0},
                {"34", 1.0},
                {"41", 0.0},
                {"42", 0.0},
                {"43", 0.0}
            };

            // Рейтинг альтернатив по критериям
            double[][] A =
            {
                new double[] {1, 7, 1, 8},
                new double[] {3, 4, 2, 10},
                new double[] {6, 3, 7, 4},
                new double[] {9, 1, 9, 1}
            };

            string[] alternative = { "Автострада", "Шоссе", "Грунтовка", "Проселок" };

            string result = RunCriteriaCombination(A, markCriteria, alternative);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Результат работы программы.");
            Console.WriteLine("Лучший выбор: " + result);
            Console.ResetColor();
            Console.ReadLine();
        }

        public static string RunCriteriaCombination(double[][] A, Dictionary<string, double> markCriteria, string[] alternative)
        {
            // Нормализуем матрицу
            double[][] normalizeMatrix = NormalizeMatrix(A);
            Console.WriteLine("Нормализованная матрица А:");
            foreach (var row in normalizeMatrix)
            {
                Console.WriteLine(string.Join(", ", row));
            }

            int columns = A[0].Length;

            // Получаем αᵢ для всего
            List<double> weight = GetWeight(columns, markCriteria);
            Console.WriteLine("Вектор α: " + string.Join(", ", weight));

            // Нормализуем этот вектор
            List<double> normalizeWeight = NormalizeWeight(weight);
            Console.WriteLine("Нормализованный вектор α: " + string.Join(", ", normalizeWeight));

            // Перемножаем полученные нормализованные матрицы
            double[] multiplyResult = MultiplyMatrixAndWeight(normalizeMatrix, normalizeWeight);
            Console.WriteLine("Произведение нормализованных матриц: " + string.Join(", ", multiplyResult));

            // Ищем максимальный индекс
            int indexMax = FindMaxIndex(multiplyResult);
            Console.WriteLine("Индекс максимального элемента: " + indexMax);

            return alternative[indexMax];
        }

        public static int FindMaxIndex(double[] array)
        {
            double max = double.NegativeInfinity;
            int maxIndex = -1;

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] > max)
                {
                    max = array[i];
                    maxIndex = i;
                }
            }

            return maxIndex;
        }

        public static double[] MultiplyMatrixAndWeight(double[][] matrix, List<double> weight)
        {
            int rows = matrix.Length;
            int cols = matrix[0].Length;

            if (cols != weight.Count)
            {
                throw new ArgumentException("Несоответствие размеров матрицы и веса");
            }

            double[] result = new double[rows];

            for (int i = 0; i < rows; i++)
            {
                double sum = 0.0;
                for (int j = 0; j < cols; j++)
                {
                    sum += matrix[i][j] * weight[j];
                }
                result[i] = sum;
            }

            return result;
        }

        public static List<double> NormalizeWeight(List<double> weight)
        {
            List<double> normalize = new List<double>();
            double sum = weight.Sum();

            foreach (double w in weight)
            {
                normalize.Add(w / sum);
            }

            return normalize;
        }

        public static List<double> GetWeight(int columns, Dictionary<string, double> markCriteria)
        {
            List<double> weight = new List<double>();

            for (int i = 0; i < columns; i++)
            {
                double a = 0;
                for (int j = 0; j < columns; j++)
                {
                    if (i != j)
                    {
                        string key = (i + 1) + "" + (j + 1);
                        a += markCriteria[key];
                    }
                }
                weight.Add(a);
            }

            return weight;
        }

        public static double[][] NormalizeMatrix(double[][] A)
        {
            int columns = A[0].Length;

            for (int j = 0; j < columns; j++)
            {
                double sum = SumColumn(A, j);
                for (int i = 0; i < A.Length; i++)
                {
                    A[i][j] /= sum;
                }
            }

            return A;
        }
        

        public static double SumColumn(double[][] A, int j)
        {
            double sum = 0;
            foreach (var row in A)
            {
                sum += row[j];
            }

            return sum;


            
        }
    }
}