
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Тестування класу Car
namespace CarRegistry.Tests;

[TestClass]
public sealed class CarTests
{
    // Тестування конструктора та валідації властивостей

    [TestMethod]
    public void Constructor_WithValidArguments_CreatesCarObject()
    {
        // Arrange
        string brand = "BMW";
        string ownerSurname = "Shevchenko";
        int year = 2020;
        double mileage = 50000;

        // Act
        var car = new Car(brand, ownerSurname, year, mileage);

        // Assert
        Assert.AreEqual(brand, car.Brand);
        Assert.AreEqual(ownerSurname, car.OwnerSurname);
        Assert.AreEqual(year, car.PurchaseYear);
        Assert.AreEqual(mileage, car.Mileage);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    public void Constructor_WithInvalidBrand_ThrowsArgumentException(string invalidBrand)
    {
        Assert.Throws<ArgumentException>(
            () => new Car(invalidBrand, "Shevchenko", 2020, 50000));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    public void Constructor_WithInvalidOwnerSurname_ThrowsArgumentException(string invalidSurname)
    {
        Assert.Throws<ArgumentException>(
            () => new Car("BMW", invalidSurname, 2020, 50000));
    }

    [TestMethod]
    public void Constructor_WithFutureYear_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => new Car("BMW", "Shevchenko", DateTime.Now.Year + 1, 50000));
    }

    [TestMethod]
    public void Constructor_WithEarlyYear_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => new Car("BMW", "Shevchenko", 1885, 50000));
    }

    [TestMethod]
    public void Constructor_WithNegativeMileage_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => new Car("BMW", "Shevchenko", 2020, -100));
    }

    // Тестування реалізації IComparable (порівняння за пробігом)

    [TestMethod]
    public void CompareTo_WithOtherCar_ReturnsCorrectValue()
    {
        var car1 = new Car("BMW", "Ivanenko", 2020, 30000);
        var car2 = new Car("Audi", "Petrenko", 2021, 60000);
        var car3 = new Car("Mercedes", "Kovalenko", 2019, 30000);

        Assert.IsTrue(car1.CompareTo(car2) < 0,
            "Car1 повинен мати менший пробіг за Car2.");

        Assert.IsTrue(car2.CompareTo(car1) > 0,
            "Car2 повинен мати більший пробіг за Car1.");

        Assert.AreEqual(0, car1.CompareTo(car3),
            "Автомобілі повинні мати однаковий результат порівняння.");
    }

    [TestMethod]
    public void CompareTo_WithNull_ReturnsPositiveValue()
    {
        var car = new Car("BMW", "Shevchenko", 2020, 50000);

        Assert.IsTrue(car.CompareTo(null) > 0);
    }

    // Тестування методу ToString()

    [TestMethod]
    public void ToString_ReturnsCorrectlyFormattedString()
    {
        var car = new Car("BMW", "Shevchenko", 2020, 50000);

        string expectedString =
            "Марка: BMW, Власник: Shevchenko, Рік придбання: 2020, Пробіг: 50000 км";

        string actualString = car.ToString();

        Assert.AreEqual(expectedString, actualString);
    }

    // Тестування IComparer (сортування за пробігом)

    [TestMethod]
    public void CarMileageComparer_ReturnsCorrectValue()
    {
        var car1 = new Car("BMW", "Ivanenko", 2020, 30000);
        var car2 = new Car("Audi", "Petrenko", 2021, 60000);

        var comparer = new CarMileageComparer();

        Assert.IsTrue(comparer.Compare(car1, car2) < 0);
        Assert.IsTrue(comparer.Compare(car2, car1) > 0);
        Assert.AreEqual(0, comparer.Compare(car1, car1));
    }

    // Тестування порівняння автомобілів за прізвищами власників

    [TestMethod]
    public void Compare_WhenOwnersAreDifferent_ReturnsCorrectOrder()
    {
        var car1 = new Car("BMW", "Anderson", 2020, 30000);
        var car2 = new Car("Audi", "Brown", 2021, 50000);

        var comparer = new CarOwnerComparer();

        Assert.IsTrue(comparer.Compare(car1, car2) < 0,
            "Anderson повинен бути перед Brown.");

        Assert.IsTrue(comparer.Compare(car2, car1) > 0,
            "Brown повинен бути після Anderson.");
    }

    [TestMethod]
    public void Compare_WhenOwnersAreIdentical_ReturnsZero()
    {
        var car1 = new Car("BMW", "Shevchenko", 2020, 30000);
        var car2 = new Car("Audi", "Shevchenko", 2021, 50000);

        var comparer = new CarOwnerComparer();

        Assert.AreEqual(0, comparer.Compare(car1, car2),
            "Автомобілі з однаковими прізвищами власників мають бути рівними.");
    }

    [TestMethod]
    public void Compare_WhenOwnersHaveDifferentCase_ReturnsZero()
    {
        var car1 = new Car("BMW", "ivanenko", 2020, 30000);
        var car2 = new Car("Audi", "Ivanenko", 2021, 50000);

        var comparer = new CarOwnerComparer();

        Assert.AreEqual(0, comparer.Compare(car1, car2),
            "Порівняння має бути нечутливим до регістру.");
    }

    // Тестування обробки null-значень

    [TestMethod]
    public void Compare_WhenFirstCarIsNull_ReturnsZero()
    {
        var car2 = new Car("Audi", "Brown", 2021, 50000);
        var comparer = new CarOwnerComparer();

        Assert.AreEqual(0, comparer.Compare(null, car2));
    }

    [TestMethod]
    public void Compare_WhenSecondCarIsNull_ReturnsZero()
    {
        var car1 = new Car("BMW", "Anderson", 2020, 30000);
        var comparer = new CarOwnerComparer();

        Assert.AreEqual(0, comparer.Compare(car1, null));
    }

    [TestMethod]
    public void Compare_WhenBothCarsAreNull_ReturnsZero()
    {
        var comparer = new CarOwnerComparer();

        Assert.AreEqual(0, comparer.Compare(null, null));
    }
}
