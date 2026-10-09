using System.Globalization;
using Microsoft.EntityFrameworkCore;
using WarehouseApp.Data;
using WarehouseApp.Models;
using WarehouseApp.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

await using (var db = new WarehouseContext())
{
    await db.Database.EnsureCreatedAsync();
}

var service = new WarehouseService();

while (true)
{
    PrintMenu();
    Console.Write("Выберите действие: ");
    string? choice = Console.ReadLine();
    Console.WriteLine();

    try
    {
        switch (choice)
        {
            case "1":
                await ShowAllAsync(service);
                break;
            case "2":
                await AddAsync(service);
                break;
            case "3":
                await EditAsync(service);
                break;
            case "4":
                await DeleteAsync(service);
                break;
            case "5":
                await ShowOneAsync(service);
                break;
            case "0":
                Console.WriteLine("Работа программы завершена.");
                return;
            default:
                Console.WriteLine("Неизвестная команда. Выберите пункт от 0 до 5.");
                break;
        }
    }
    catch (DbUpdateException exception)
    {
        Console.WriteLine($"Не удалось сохранить данные: {exception.GetBaseException().Message}");
    }

    Console.WriteLine("\nНажмите Enter, чтобы продолжить...");
    Console.ReadLine();
    if (!Console.IsInputRedirected && !Console.IsOutputRedirected)
    {
        Console.Clear();
    }
}

static void PrintMenu()
{
    Console.WriteLine("============================");
    Console.WriteLine("       СКЛАД ТОВАРОВ");
    Console.WriteLine("============================");
    Console.WriteLine("1. Показать все товары");
    Console.WriteLine("2. Добавить товар");
    Console.WriteLine("3. Изменить товар");
    Console.WriteLine("4. Удалить товар");
    Console.WriteLine("5. Найти товар по ID");
    Console.WriteLine("0. Выход");
    Console.WriteLine("============================");
}

static async Task ShowAllAsync(WarehouseService service)
{
    List<Product> products = await service.GetAllAsync();

    if (products.Count == 0)
    {
        Console.WriteLine("Склад пуст.");
        return;
    }

    PrintTableHeader();
    foreach (Product product in products)
    {
        PrintProduct(product);
    }
}

static async Task ShowOneAsync(WarehouseService service)
{
    int id = ReadPositiveInt("Введите ID товара: ");
    Product? product = await service.GetByIdAsync(id);

    if (product is null)
    {
        Console.WriteLine("Товар с таким ID не найден.");
        return;
    }

    PrintTableHeader();
    PrintProduct(product);
}

static async Task AddAsync(WarehouseService service)
{
    Console.WriteLine("Добавление товара");
    string name = ReadRequiredString("Название: ", 100);
    string category = ReadRequiredString("Категория: ", 50);
    int quantity = ReadNonNegativeInt("Количество: ");
    decimal price = ReadNonNegativeDecimal("Цена: ");

    Product product = await service.CreateAsync(name, category, quantity, price);
    Console.WriteLine($"Товар добавлен. Его ID: {product.Id}.");
}

static async Task EditAsync(WarehouseService service)
{
    int id = ReadPositiveInt("Введите ID изменяемого товара: ");
    Product? current = await service.GetByIdAsync(id);

    if (current is null)
    {
        Console.WriteLine("Товар с таким ID не найден.");
        return;
    }

    Console.WriteLine("Введите новые данные товара:");
    string name = ReadRequiredString("Название: ", 100);
    string category = ReadRequiredString("Категория: ", 50);
    int quantity = ReadNonNegativeInt("Количество: ");
    decimal price = ReadNonNegativeDecimal("Цена: ");

    await service.UpdateAsync(id, name, category, quantity, price);
    Console.WriteLine("Товар успешно изменён.");
}

static async Task DeleteAsync(WarehouseService service)
{
    int id = ReadPositiveInt("Введите ID удаляемого товара: ");
    Product? product = await service.GetByIdAsync(id);

    if (product is null)
    {
        Console.WriteLine("Товар с таким ID не найден.");
        return;
    }

    Console.Write($"Удалить товар «{product.Name}»? (д/н): ");
    string? confirmation = Console.ReadLine()?.Trim().ToLowerInvariant();

    if (confirmation is not ("д" or "да" or "y" or "yes"))
    {
        Console.WriteLine("Удаление отменено.");
        return;
    }

    await service.DeleteAsync(id);
    Console.WriteLine("Товар удалён.");
}

static string ReadRequiredString(string prompt, int maxLength)
{
    while (true)
    {
        Console.Write(prompt);
        string value = Console.ReadLine()?.Trim() ?? string.Empty;

        if (value.Length == 0)
        {
            Console.WriteLine("Поле не может быть пустым.");
        }
        else if (value.Length > maxLength)
        {
            Console.WriteLine($"Допустимо не более {maxLength} символов.");
        }
        else
        {
            return value;
        }
    }
}

static int ReadPositiveInt(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        if (int.TryParse(Console.ReadLine(), out int value) && value > 0)
        {
            return value;
        }

        Console.WriteLine("Введите целое число больше нуля.");
    }
}

static int ReadNonNegativeInt(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        if (int.TryParse(Console.ReadLine(), out int value) && value >= 0)
        {
            return value;
        }

        Console.WriteLine("Введите целое неотрицательное число.");
    }
}

static decimal ReadNonNegativeDecimal(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string input = (Console.ReadLine() ?? string.Empty).Replace(',', '.');

        if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value)
            && value >= 0
            && value <= 999999999.99m)
        {
            return decimal.Round(value, 2);
        }

        Console.WriteLine("Введите корректную неотрицательную цену (до 999999999,99).");
    }
}

static void PrintTableHeader()
{
    Console.WriteLine($"{"ID",-5} {"Название",-24} {"Категория",-18} {"Кол-во",8} {"Цена",12}");
    Console.WriteLine(new string('-', 72));
}

static void PrintProduct(Product product)
{
    Console.WriteLine($"{product.Id,-5} {TrimForTable(product.Name, 24),-24} " +
                      $"{TrimForTable(product.Category, 18),-18} {product.Quantity,8} {product.Price,12:F2}");
}

static string TrimForTable(string value, int maxLength)
{
    return value.Length <= maxLength ? value : value[..(maxLength - 1)] + "…";
}
