using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

public class Reflector
{
    // Выводит всё содержимое класса в текстовый файл
    public static void WriteClassContentToFile(string className)
    {
        Type type = Type.GetType(className);
        if (type == null)
        {
            Console.WriteLine($"Класс {className} не найден.");
            return;
        }

        string fileName = $"{className}_Content.txt";
        using (StreamWriter writer = new StreamWriter(fileName))
        {
            writer.WriteLine($"Содержимое класса {className}:");
            foreach (MemberInfo member in type.GetMembers())
            {
                writer.WriteLine(member.ToString());
            }
        }

        Console.WriteLine($"Содержимое класса {className} записано в файл {fileName}.");
    }

    // Извлекает все общедоступные публичные методы класса
    public static string[] ExtractPublicMethods(string className)
    {
        Type type = Type.GetType(className);
        if (type == null)
        {
            return new string[] { $"Класс {className} не найден." };
        }

        MethodInfo[] publicMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
        if (publicMethods.Length == 0)
        {
            return new string[] { $"В классе {className} нет общедоступных публичных методов." };
        }

        List<string> methodNames = new List<string>();
        foreach (MethodInfo method in publicMethods)
        {
            methodNames.Add(method.Name);
        }

        return methodNames.ToArray();
    }


    // Получает информацию о полях и свойствах класса
    // Получает поля и свойства класса и возвращает их в виде списка строк
    public static List<string> GetFieldsAndProperties(string className)
    {
        Type type = Type.GetType(className);
        if (type == null)
        {
            return new List<string> { $"Класс {className} не найден." };
        }

        List<string> fieldsAndProperties = new List<string>();
        fieldsAndProperties.Add($"Поля и свойства класса {className}:");
        foreach (FieldInfo field in type.GetFields())
        {
            fieldsAndProperties.Add($"Поле: {field.Name}");
        }
        foreach (PropertyInfo property in type.GetProperties())
        {
            fieldsAndProperties.Add($"Свойство: {property.Name}");
        }

        return fieldsAndProperties;
    }

    // Получает все реализованные классом интерфейсы и возвращает их в виде списка строк
    public static List<string> GetImplementedInterfaces(string className)
    {
        Type type = Type.GetType(className);
        if (type == null)
        {
            return new List<string> { $"Класс {className} не найден." };
        }

        Type[] interfaces = type.GetInterfaces();
        if (interfaces.Length == 0)
        {
            return new List<string> { $"Класс {className} не реализует ни одного интерфейса." };
        }

        List<string> implementedInterfaces = new List<string>();
        implementedInterfaces.Add($"Интерфейсы, реализованные классом {className}:");
        foreach (Type iface in interfaces)
        {
            implementedInterfaces.Add(iface.Name);
        }

        return implementedInterfaces;
    }

    // Получает методы класса, содержащие указанный тип параметра, и возвращает их имена в виде списка строк
    public static List<string> GetMethodsByParameterType(string className, string parameterTypeName)
    {
        Type type = Type.GetType(className);
        if (type == null)
        {
            return new List<string> { $"Класс {className} не найден." };
        }

        MethodInfo[] methods = type.GetMethods();
        var methodsWithParameter = methods.Where(m => m.GetParameters().Any(p => p.ParameterType.Name == parameterTypeName));
        if (methodsWithParameter.Count() == 0)
        {
            return new List<string> { $"В классе {className} нет методов, содержащих параметр типа {parameterTypeName}." };
        }

        List<string> methodsList = new List<string>();
        methodsList.Add($"Методы класса {className}, содержащие параметр типа {parameterTypeName}:");
        foreach (MethodInfo method in methodsWithParameter)
        {
            methodsList.Add(method.Name);
        }

        return methodsList;
    }

    // Вызывает указанный метод класса, используя значения параметров из текстового файла, и возвращает результат вызова
    public static object CallMethodFromFile(string className, string methodName)
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

        string fileName = $"{methodName}_Parameters.txt";
        if (!File.Exists(fileName))
        {
            return $"Файл с параметрами метода {methodName} не найден.";
        }

        string[] lines = File.ReadAllLines(fileName);
        object[] parameters = new object[method.GetParameters().Length];
        for (int i = 0; i < parameters.Length && i < lines.Length; i++)
        {
            parameters[i] = Convert.ChangeType(lines[i], method.GetParameters()[i].ParameterType);
        }

        object instance = Activator.CreateInstance(type);
        object result = method.Invoke(instance, parameters);
        return result;
    }
}
