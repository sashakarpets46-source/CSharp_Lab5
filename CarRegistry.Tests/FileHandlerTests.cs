
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

// Тестування асинхронної роботи з файлами
namespace CarRegistry.Tests;

[TestClass]
public class FileHandlerTests
{
    private string _testFilePath = string.Empty;

    // Метод, який виконується перед кожним тестом
    [TestInitialize]
    public void TestInitialize()
    {
        // Створюємо унікальний шлях до файлу для кожного тесту
        _testFilePath = Path.Combine(
            Path.GetTempPath(),
            Path.GetRandomFileName() + ".json");
    }

    // Метод, який виконується після кожного тесту
    [TestCleanup]
    public void TestCleanup()
    {
        // Видаляємо файл після завершення тесту
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }

    // Тестування запису даних у файл
    [TestMethod]
    public async Task SaveAsync_ShouldWriteDataToFile()
    {
        var cars = new List<Car>
        {
            new Car("BMW", "Shevchenko", 2020, 50000),
            new Car("Audi", "Ivanenko", 2021, 30000)
        };

        var fileSaver = new JsonFileWriter();

        // Записуємо автомобілі у файл
        await fileSaver.SaveAsync(_testFilePath, cars);

        // Перевіряємо, чи файл було створено
        Assert.IsTrue(File.Exists(_testFilePath),
            "Файл не було створено.");

        // Читаємо вміст файлу
        string fileContent = await File.ReadAllTextAsync(_testFilePath);

        Assert.IsFalse(string.IsNullOrWhiteSpace(fileContent),
            "Файл порожній.");

        // Додаткова перевірка: читаємо записані дані
        var fileReader = new JsonFileReader();

        var loadedCars = await fileReader.LoadAsync<Car>(_testFilePath);

        Assert.IsNotNull(loadedCars,
            "Десеріалізація не вдалася.");

        Assert.AreEqual(cars.Count, loadedCars.Count,
            "Кількість автомобілів не збігається.");

        Assert.AreEqual(cars[0].Brand, loadedCars[0].Brand);
    }

    // Тестування читання даних із файлу
    [TestMethod]
    public async Task LoadAsync_ShouldReadDataFromFile()
    {
        // Створюємо JSON із тестовими автомобілями
        string jsonContent =
            "[{\"Brand\":\"BMW\",\"OwnerSurname\":\"Shevchenko\",\"PurchaseYear\":2020,\"Mileage\":50000}]";

        // Записуємо JSON у тимчасовий файл
        await File.WriteAllTextAsync(_testFilePath, jsonContent);

        var fileReader = new JsonFileReader();

        // Читаємо автомобілі з файлу
        var loadedCars = await fileReader.LoadAsync<Car>(_testFilePath);

        // Перевіряємо результат
        Assert.IsNotNull(loadedCars,
            "Об'єкти не були завантажені.");

        Assert.AreEqual(1, loadedCars.Count,
            "Завантажена некоректна кількість об'єктів.");

        Assert.AreEqual("BMW", loadedCars[0].Brand);
        Assert.AreEqual("Shevchenko", loadedCars[0].OwnerSurname);
        Assert.AreEqual(2020, loadedCars[0].PurchaseYear);
        Assert.AreEqual(50000, loadedCars[0].Mileage);
    }

    // Тестування поведінки, коли файл не існує
    [TestMethod]
    public async Task LoadAsync_WhenFileDoesNotExist_ReturnsNull()
    {
        var fileReader = new JsonFileReader();

        // Спробуємо прочитати файл, якого не існує
        var loadedCars = await fileReader.LoadAsync<Car>(_testFilePath);

        Assert.IsNull(loadedCars,
            "Повинен повертатися null, якщо файл не існує.");
    }
}
