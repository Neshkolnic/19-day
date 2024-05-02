using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string className = textBox1.Text;
            if (!string.IsNullOrWhiteSpace(className))
            {
                Reflector.WriteClassContentToFile(className);
                MessageBox.Show($"Содержимое класса {className} было записано в файл.");
            }
            else
            {
                MessageBox.Show("Введите имя класса.");
            }
        }

        public static void ExtractPublicMethods(string className, TextBox textBox)
        {
            Type type = Type.GetType(className);
            if (type == null)
            {
                textBox.AppendText($"Класс {className} не найден.\r\n");
                return;
            }

            MethodInfo[] publicMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            if (publicMethods.Length == 0)
            {
                textBox.AppendText($"В классе {className} нет общедоступных публичных методов.\r\n");
                return;
            }

            StringBuilder methodsText = new StringBuilder();
            methodsText.AppendLine($"Общедоступные публичные методы класса {className}:");
            foreach (MethodInfo method in publicMethods)
            {
                methodsText.AppendLine(method.Name);
            }

            textBox.AppendText(methodsText.ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string className = textBox1.Text;
            if (!string.IsNullOrWhiteSpace(className))
            {
                ExtractPublicMethods(className, textBox2);
                MessageBox.Show($"Извлечены общедоступные методы класса {className}.");
            }
            else
            {
                MessageBox.Show("Введите имя класса.");
            }
        }


        public static string GetImplementedInterfaces(string className)
        {
            Type type = Type.GetType(className);
            if (type == null)
            {
                return $"Класс {className} не найден.";
            }

            Type[] interfaces = type.GetInterfaces();
            if (interfaces.Length == 0)
            {
                return $"Класс {className} не реализует ни одного интерфейса.";
            }

            StringBuilder resultText = new StringBuilder();
            resultText.AppendLine($"Интерфейсы, реализованные классом {className}:");
            foreach (Type iface in interfaces)
            {
                resultText.AppendLine(iface.Name);
            }

            return resultText.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string className = textBox1.Text;
            if (!string.IsNullOrWhiteSpace(className))
            {
                string result = GetImplementedInterfaces(className);
                textBox2.AppendText(result + Environment.NewLine);
            }
            else
            {
                MessageBox.Show("Введите имя класса." + Environment.NewLine);
            }
        }

        public static string GetMethodsByParameterType(string className, string parameterTypeName)
        {
            Type type = Type.GetType(className);
            if (type == null)
            {
                return $"Класс {className} не найден.";
            }

            MethodInfo[] methods = type.GetMethods();
            var methodsWithParameter = methods.Where(m => m.GetParameters().Any(p => p.ParameterType.Name == parameterTypeName));
            if (methodsWithParameter.Count() == 0)
            {
                return $"В классе {className} нет методов, содержащих параметр типа {parameterTypeName}.";
            }

            StringBuilder resultText = new StringBuilder();
            resultText.AppendLine($"Методы класса {className}, содержащие параметр типа {parameterTypeName}:");
            foreach (MethodInfo method in methodsWithParameter)
            {
                resultText.AppendLine(method.Name);
            }

            return resultText.ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string className = textBox1.Text;
            string parameterTypeName = "Type"; // Замените "Type" на тип параметра, который вы хотите искать
            if (!string.IsNullOrWhiteSpace(className))
            {
                string result = GetMethodsByParameterType(className, parameterTypeName);
                textBox2.AppendText(result + Environment.NewLine);
            }
            else
            {
                MessageBox.Show("Введите имя класса." + Environment.NewLine);
            }
        }

        public static string CallMethodFromFile(string className, string methodName)
        {
            Type type = Type.GetType(className);
            if (type == null)
            {
                return $"Класс {className} не найден.";
            }

            MethodInfo method = type.GetMethod(methodName);
            if (method == null)
            {
                return $"Метод {methodName} не найден в классе {className}.";
            }

            // Путь к файлу с параметрами
            string filePath = "Parameters.txt";
            if (!File.Exists(filePath))
            {
                return $"Файл с параметрами метода {methodName} не найден.";
            }

            // Чтение параметров из файла
            string[] lines = File.ReadAllLines(filePath);
            object[] parameters = new object[method.GetParameters().Length];
            for (int i = 0; i < parameters.Length && i < lines.Length; i++)
            {
                parameters[i] = Convert.ChangeType(lines[i], method.GetParameters()[i].ParameterType);
            }

            // Создание экземпляра класса и вызов метода
            object instance = Activator.CreateInstance(type);
            object result = method.Invoke(instance, parameters);

            return result.ToString();
        }


        private void button6_Click(object sender, EventArgs e)
        {
            string className = "Databaвse";
            string methodName = "AddUser"; // Замените "MethodName" на имя метода, который вы хотите вызвать
            if (!string.IsNullOrWhiteSpace(className))
            {
                string result = CallMethodFromFile(className, methodName);
                textBox2.AppendText(result + Environment.NewLine);
            }
            else
            {
                MessageBox.Show("Введите имя класса." + Environment.NewLine);
            }
        }

        public static string GetFieldsAndProperties(string className)
        {
            Type type = Type.GetType(className);
            if (type == null)
            {
                return $"Класс {className} не найден.";
            }

            StringBuilder resultText = new StringBuilder();
            resultText.AppendLine($"Поля и свойства класса {className}:");
            foreach (FieldInfo field in type.GetFields())
            {
                resultText.AppendLine($"Поле: {field.Name}");
            }
            foreach (PropertyInfo property in type.GetProperties())
            {
                resultText.AppendLine($"Свойство: {property.Name}");
            }

            return resultText.ToString();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            string className = textBox1.Text;
            if (!string.IsNullOrWhiteSpace(className))
            {
                string result = GetFieldsAndProperties(className);
                textBox2.AppendText(result);
            }
            else
            {
                MessageBox.Show("Введите имя класса.\r\n");
            }
        }
    }
}
