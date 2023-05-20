using System;
using System.Collections.Generic;
using System.Linq;

namespace LW16
{
    public class PNumber
    {
        private List<int> digits; // Список цифр числа
        private int radix; // Основа системи числення

        // Конструктори
        public PNumber(int radix)
        {
            this.radix = radix;
            digits = new List<int>();
        }

        public PNumber(int[] digits, int radix)
        {
            this.radix = radix;
            this.digits = new List<int>(digits);
        }

        // Конструктор копіювання
        public PNumber(PNumber other)
        {
            radix = other.radix;
            digits = new List<int>(other.digits);
        }

        // Метод для додавання цифри в число
        public void AddDigit(int digit)
        {
            if (digit < 0 || digit >= radix)
                throw new ArgumentException("Incorrect digit");
            digits.Add(digit);
        }

        // Метод для зміни окремої цифри числа
        public void ChangeDigit(int index, int digit)
        {
            if (index < 0 || index >= digits.Count)
                throw new IndexOutOfRangeException("Index is out of range");
            if (digit < 0 || digit >= radix)
                throw new ArgumentException("Incorrect digit");
            digits[index] = digit;
        }

        // Метод для видалення цифри з числа
        public void RemoveDigit(int index)
        {
            if (index < 0 || index >= digits.Count)
                throw new IndexOutOfRangeException("Index is out of range");
            digits.RemoveAt(index);
        }

        // Метод для переведення числа в рядок
        public override string ToString()
        {
            return string.Join("", digits.Select(d => d.ToString()).Reverse());
        }

        // Метод для піднесення числа до ступеня
        public static PNumber Pow(PNumber bas, PNumber exp)
        {
            if (bas.radix != exp.radix)
                throw new ArgumentException("Numbers have different radix");

            if (exp == Zero)
            {
                return DecimalToPNumber(1, bas.radix);
            }

            PNumber res = bas;

            for (int i = 0; i < PNumberToDecimal(exp) - 1; i++)
            {
                res = Multiply(res, bas);
            }

            return res;
        }

        // Метод для виконання додавання чисел
        public static PNumber Add(PNumber a, PNumber b)
        {
            if (a.radix != b.radix)
                throw new ArgumentException("Numbers have different radix");

            PNumber result = new PNumber(a.radix);
            int carry = 0;

            int maxLength = Math.Max(a.digits.Count, b.digits.Count);
            for (int i = 0; i < maxLength; i++)
            {
                int sum = carry;
                if (i < a.digits.Count)
                    sum += a.digits[i];
                if (i < b.digits.Count)
                    sum += b.digits[i];

                result.AddDigit(sum % a.radix);
                carry = sum / a.radix;
            }

            if (carry > 0)
                result.AddDigit(carry);

            return result;
        }

        // Метод для пошуку різниці чисел
        public static PNumber Subtract(PNumber a, PNumber b)
        {
            if (a.radix != b.radix)
                throw new ArgumentException("Numbers have different radix");

            if (a < b)
                throw new ArgumentException("The first number must be greater than or equal to the second number");

            PNumber result = new PNumber(a.radix);
            int borrow = 0;

            for (int i = 0; i < a.digits.Count; i++)
            {
                int diff = a.digits[i] - borrow;
                if (i < b.digits.Count)
                    diff -= b.digits[i];

                if (diff < 0)
                {
                    diff += a.radix;
                    borrow = 1;
                }
                else
                {
                    borrow = 0;
                }

                result.AddDigit(diff);
            }

            result.RemoveLeadingZeros();

            return result;
        }

        // Метод для виконання множення чисел
        public static PNumber Multiply(PNumber a, PNumber b)
        {
            if (a.radix != b.radix)
                throw new ArgumentException("Numbers have different radix");

            PNumber result = new PNumber(a.radix);

            for (int i = 0; i < b.digits.Count; i++)
            {
                PNumber partialProduct = MultiplyByDigit(a, b.digits[i]);
                partialProduct.ShiftLeft(i);
                result = Add(result, partialProduct);
            }

            return result;
        }

        // Додатковий метод для множення числа на цифру
        private static PNumber MultiplyByDigit(PNumber number, int digit)
        {
            PNumber result = new PNumber(number.radix);
            int carry = 0;

            for (int i = 0; i < number.digits.Count; i++)
            {
                int product = number.digits[i] * digit + carry;
                result.AddDigit(product % number.radix);
                carry = product / number.radix;
            }

            if (carry > 0)
                result.AddDigit(carry);

            return result;
        }

        // Метод для виконання ділення чисел
        public static PNumber Divide(PNumber dividend, PNumber divisor, out PNumber remainder)
        {
            if (dividend.radix != divisor.radix)
                throw new ArgumentException("Numbers have different radix");

            if (divisor == Zero)
                throw new DivideByZeroException("Dividing by zero is not possible");

            if (dividend < divisor)
            {
                remainder = new PNumber(dividend);
                return Zero;
            }

            int a = PNumberToDecimal(dividend);
            int b = PNumberToDecimal(divisor);

            remainder = DecimalToPNumber(a % b, dividend.radix);

            return DecimalToPNumber(a / b, dividend.radix);
        }

        // Метод для перевірки правильності запису числа в P-ічній системі числення
        public static bool IsValidPNumber(string number, int radix)
        {
            foreach (char digitChar in number)
            {
                int digit = (int)char.GetNumericValue(digitChar);
                if (digit < 0 || digit >= radix)
                    return false;
            }

            return true;
        }

        // Метод для переведення числа з десяткової системи числення в P-ічну
        public static PNumber DecimalToPNumber(int decimalNumber, int radix)
        {
            PNumber result = new PNumber(radix);

            while (decimalNumber > 0)
            {
                int remainder = decimalNumber % radix;
                result.AddDigit(remainder);
                decimalNumber /= radix;
            }

            return result;
        }

        // Метод для переведення числа з P-ічної системи числення в десяткову
        public static int PNumberToDecimal(PNumber number)
        {
            int result = 0;
            int power = 1;

            for (int i = 0; i < number.digits.Count; i++)
            {
                result += number.digits[i] * power;
                power *= number.radix;
            }

            return result;
        }

        // Метод для виконання перевірки чисел на рівність
        public static bool operator ==(PNumber a, PNumber b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (a is null || b is null)
                return false;

            if (a.radix != b.radix || a.digits.Count != b.digits.Count)
                return false;

            for (int i = 0; i < a.digits.Count; i++)
            {
                if (a.digits[i] != b.digits[i])
                    return false;
            }

            return true;
        }

        // Метод для виконання перевірки чисел на нерівність
        public static bool operator !=(PNumber a, PNumber b)
        {
            return !(a == b);
        }

        // Метод для виконання перевірки на більше чи рівно
        public static bool operator >=(PNumber a, PNumber b)
        {
            return a > b || a == b;
        }

        // Метод для виконання перевірки на менше чи рівно
        public static bool operator <=(PNumber a, PNumber b)
        {
            return a < b || a == b;
        }

        // Метод для виконання перевірки на більше
        public static bool operator >(PNumber a, PNumber b)
        {
            if (a is null || b is null)
                throw new ArgumentNullException();

            if (a.radix != b.radix)
                throw new ArgumentException("Numbers have different radix");

            if (a.digits.Count != b.digits.Count)
                return a.digits.Count > b.digits.Count;

            for (int i = a.digits.Count - 1; i >= 0; i--)
            {
                if (a.digits[i] != b.digits[i])
                    return a.digits[i] > b.digits[i];
            }

            return false;
        }

        // Метод для виконнаня перевірки на менше
        public static bool operator <(PNumber a, PNumber b)
        {
            return !(a > b) && a != b;
        }

        // Метод для видалення провідних нулів числа
        private void RemoveLeadingZeros()
        {
            while (digits.Count > 1 && digits[digits.Count - 1] == 0)
                digits.RemoveAt(digits.Count - 1);
        }

        // Метод для виконання інвертування порядку цифр числа
        private void ReverseDigits()
        {
            digits.Reverse();
        }

        // Метод для виконання зсуву числа вліво на вказану кількість розрядів
        private void ShiftLeft(int count)
        {
            for (int i = 0; i < count; i++)
                digits.Insert(0, 0);
        }

        // Статична властивість для подання нуля в P-ічній системі числення
        public static PNumber Zero { get; } = new PNumber(0);

        // Деструктор
        ~PNumber()
        {
            digits.Clear();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            PNumber a = new PNumber(new int[] { 1, 0, 1 }, 2); // Створюємо перше число в двійковій системі числення

            PNumber b = new PNumber(new int[] { 1, 1 }, 2); // Створюємо друге число в двійковій системі числення

            // Підносимо перше число до ступеню з індексом, що дорівнює другому числу
            // Виводимо результат в двійковій системі числення
            PNumber pow = PNumber.Pow(a, b);
            Console.WriteLine($"Піднесення числа до ступеня: {pow}");

            // Переводимо результат в десяткову систему числення і виводимо
            int decimalResultPow = PNumber.PNumberToDecimal(pow);
            Console.WriteLine($"Піднесення числа до ступеня (в десятковій системі числення): {decimalResultPow}");

            // Додаємо числа і виводимо результат в двійковій системі числення
            PNumber c = PNumber.Add(a, b);
            Console.WriteLine($"Сума: {c}");

            // Переводимо результат в десяткову систему числення і виводимо
            int decimalResultAdd = PNumber.PNumberToDecimal(c);
            Console.WriteLine($"Сума (в десятковій системі числення): {decimalResultAdd}");

            // Віднімаємо числа і виводимо результат в двійковій системі числення
            PNumber d = PNumber.Subtract(a, b);
            Console.WriteLine($"Різниця: {d}");

            // Переводимо результат в десяткову систему числення і виводимо
            int decimalResultSubtract = PNumber.PNumberToDecimal(d);
            Console.WriteLine($"Різниця (в десятковій системі числення): {decimalResultSubtract}");

            // Множимо числа і виводимо результат в двійковій системі числення
            PNumber e = PNumber.Multiply(a, b);
            Console.WriteLine($"Добуток: {e}");

            // Переводимо результат в десяткову систему числення і виводимо
            int decimalResultMultiply = PNumber.PNumberToDecimal(e);
            Console.WriteLine($"Добуток (в десятковій системі числення): {decimalResultMultiply}");

            PNumber remainder = PNumber.Zero; // Задаємо остачу

            // Ділимо числа і виводимо результат та остачу в двійковій системі числення
            PNumber f = PNumber.Divide(a, b, out remainder);
            Console.WriteLine($"Частка: {f}");
            Console.WriteLine($"Остача: {remainder}");

            // Переводимо результат та остачу в десяткову систему числення і виводимо
            int decimalResultDivide = PNumber.PNumberToDecimal(f);
            Console.WriteLine($"Частка (в десятковій системі числення): {decimalResultDivide}");
            int decimalRemainder = PNumber.PNumberToDecimal(remainder);
            Console.WriteLine($"Остача (в десятковій системі числення): {decimalRemainder}");

            // Переводимо результат з десяткову системи числення в двійкову і виводимо
            PNumber g = PNumber.DecimalToPNumber(15, 2);
            Console.WriteLine($"Переведення з десяткової системи числення в двійкову: {g}");

            // Перевіряємо правильність запису числа в двійковій системі числення
            bool h = PNumber.IsValidPNumber(a.ToString(), 2);
            Console.WriteLine($"Правильність запису числа в двійковій системі числення: {h}");

            // Проводимо порівняння чисел і виводимо результати
            Console.WriteLine("Порівняння чисел: ");
            Console.WriteLine($"Дорівнює: {a == b}");
            Console.WriteLine($"Не дорівнює: {a != b}");
            Console.WriteLine($"Більше або дорівнює: {a >= b}");
            Console.WriteLine($"Менше або дорівнює: {a <= b}");
            Console.WriteLine($"Більше: {a > b}");
            Console.WriteLine($"Менше: {a < b}");
        }
    }
}