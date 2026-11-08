using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Тестування LINQ-методів каталогу автомобілів
namespace CarRegistry.Tests;

[TestClass]
public class CarCatalogTests
{
    private CarCatalog _catalog = null!;

    private Car _car1 = null!;
    private Car _car2 = null!;
    private Car _car3 = null!;

    // Заглушка для збереження файлів
    private class MockFileSaver : IFileSaver
    {
        public Task SaveAsync<T>(string filePath, List<T> data)
        {
            return Task.CompletedTask;
        }
    }

    // Заглушка для завантаження файлів
    private class MockFileLoader : IFileLoader
    {
        public Task<List<T>?> LoadAsync<T>(string filePath)
        {
            return Task.FromResult<List<T>?>(null);
        }
    }

    [TestInitialize]
    public void TestInitialize()
    {
        // Створюємо тестові автомобілі
        _car1 = new Car("BMW", "Shevchenko", 2020, 50000);
        _car2 = new Car("Audi", "Petrenko", 2021, 30000);
        _car3 = new Car("Mercedes", "Ivanenko", 2019, 70000);

        // Створюємо каталог із заглушками
        _catalog = new CarCatalog(
            new MockFileSaver(),
            new MockFileLoader(),
            "test.json");

        // Наповнюємо каталог тестовими даними
        _catalog.AddCar(_car1);
        _catalog.AddCar(_car2);
        _catalog.AddCar(_car3);
    }

    // ==============================
    // ТЕСТУВАННЯ ПОШУКУ АВТОМОБІЛІВ
    // ==============================

    // Пошук із null
    [TestMethod]
    public void SearchCars_WithNullTerm()
    {
        _catalog.SearchCars(null);

        Assert.AreEqual(3, _catalog.Cars.Count);
    }

    // Пошук із пробілами
    [TestMethod]
    public void SearchCars_WithWhiteSpaceTerm()
    {
        _catalog.SearchCars("   ");

        Assert.AreEqual(3, _catalog.Cars.Count);
    }

    // Пошук за повною маркою автомобіля
    [TestMethod]
    public void SearchCars_WithFullBrand()
    {
        _catalog.SearchCars("BMW");

        Assert.AreEqual(1, _catalog.Cars.Count);
        Assert.AreSame(_car1, _catalog.Cars[0]);
    }

    // Пошук за частиною марки автомобіля
    [TestMethod]
    public void SearchCars_WithFragmentBrand()
    {
        _catalog.SearchCars("Mer");

        Assert.AreEqual(1, _catalog.Cars.Count);
        Assert.AreSame(_car3, _catalog.Cars[0]);
    }

    // Пошук за повним прізвищем власника
    [TestMethod]
    public void SearchCars_WithFullOwnerSurname()
    {
        _catalog.SearchCars("Petrenko");

        Assert.AreEqual(1, _catalog.Cars.Count);
        Assert.AreSame(_car2, _catalog.Cars[0]);
    }

    // Пошук за частиною прізвища власника
    [TestMethod]
    public void SearchCars_WithFragmentOwnerSurname()
    {
        _catalog.SearchCars("Shev");

        Assert.AreEqual(1, _catalog.Cars.Count);
        Assert.AreSame(_car1, _catalog.Cars[0]);
    }

    // Перевірка пошуку без урахування регістру
    [TestMethod]
    public void SearchCars_WithDifferentCase()
    {
        _catalog.SearchCars("bmw");

        Assert.AreEqual(1, _catalog.Cars.Count);
        Assert.AreSame(_car1, _catalog.Cars[0]);
    }

    // ==============================
    // ТЕСТУВАННЯ УНІКАЛЬНИХ ВЛАСНИКІВ
    // ==============================

    [TestMethod]
    public void GetUniqueOwners_ReturnsUniqueOwners()
    {
        // Arrange
        _catalog.AddCar(
            new Car("Toyota", "Shevchenko", 2022, 15000));

        // Act
        var owners = _catalog.GetUniqueOwners();

        // Assert
        Assert.AreEqual(3, owners.Count);
        Assert.AreEqual("Ivanenko", owners[0]);
        Assert.AreEqual("Petrenko", owners[1]);
        Assert.AreEqual("Shevchenko", owners[2]);
    }

    // ==============================
    // ТЕСТУВАННЯ ТРЬОХ НАЙНОВІШИХ АВТОМОБІЛІВ
    // ==============================

    [TestMethod]
    public void FindThreeNewestCars_ReturnsCarsInDescendingYearOrder()
    {
        // Act
        _catalog.FindThreeNewestCars();

        // Assert
        Assert.AreEqual(3, _catalog.Cars.Count);
        Assert.AreSame(_car2, _catalog.Cars[0]);
        Assert.AreSame(_car1, _catalog.Cars[1]);
        Assert.AreSame(_car3, _catalog.Cars[2]);
    }

    [TestMethod]
    public void FindThreeNewestCars_WithMoreThanThreeCars_ReturnsOnlyThree()
    {
        // Arrange
        var car4 = new Car("Toyota", "Brown", 2023, 10000);
        var car5 = new Car("Honda", "Green", 2024, 20000);

        _catalog.AddCar(car4);
        _catalog.AddCar(car5);

        // Act
        _catalog.FindThreeNewestCars();

        // Assert
        Assert.AreEqual(3, _catalog.Cars.Count);
        Assert.AreSame(car5, _catalog.Cars[0]);
        Assert.AreSame(car4, _catalog.Cars[1]);
        Assert.AreSame(_car2, _catalog.Cars[2]);
    }

    // ==============================
    // ТЕСТУВАННЯ СЕРЕДНЬОГО РОКУ
    // ==============================

    [TestMethod]
    public void CalculateAveragePurchaseYear_ReturnsCorrectAverage()
    {
        // Act
        double average = _catalog.CalculateAveragePurchaseYear();

        // Assert
        Assert.AreEqual(2020.0, average);
    }

    [TestMethod]
    public void CalculateAveragePurchaseYear_WithEmptyCatalog_ReturnsZero()
    {
        // Arrange
        var emptyCatalog = new CarCatalog(
            new MockFileSaver(),
            new MockFileLoader(),
            "empty.json");

        // Act
        double average = emptyCatalog.CalculateAveragePurchaseYear();

        // Assert
        Assert.AreEqual(0.0, average);
    }

    // ==============================
    // ТЕСТУВАННЯ ГРУПУВАННЯ ЗА РОКОМ
    // ==============================

    [TestMethod]
    public void GroupCarsByYear_ReturnsCarsInDescendingYearOrder()
    {
        // Act
        _catalog.GroupCarsByYear();

        // Assert
        Assert.AreEqual(3, _catalog.Cars.Count);
        Assert.AreSame(_car2, _catalog.Cars[0]);
        Assert.AreSame(_car1, _catalog.Cars[1]);
        Assert.AreSame(_car3, _catalog.Cars[2]);
    }

    [TestMethod]
    public void GroupCarsByYear_WithSameYear_KeepsCarsTogether()
    {
        // Arrange
        var car4 = new Car("Toyota", "Brown", 2020, 15000);
        _catalog.AddCar(car4);

        // Act
        _catalog.GroupCarsByYear();

        // Assert
        Assert.AreEqual(4, _catalog.Cars.Count);

        Assert.AreSame(_car2, _catalog.Cars[0]);
        Assert.AreSame(_car1, _catalog.Cars[1]);
        Assert.AreSame(car4, _catalog.Cars[2]);
        Assert.AreSame(_car3, _catalog.Cars[3]);
    }
}
