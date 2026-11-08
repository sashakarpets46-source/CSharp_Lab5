using System;
public class Car : IComparable<Car>
{
    private string _brand = string.Empty;
    private string _ownerSurname = string.Empty;
    private int _purchaseYear;
    private double _mileage;

    public string Brand
    {
        get { return _brand; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Марка автомобіля не може бути пустою.", nameof(Brand));
            }
            _brand = value;
        }
    }

    public string OwnerSurname
    {
        get { return _ownerSurname; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Прізвище власника не може бути пустим.", nameof(OwnerSurname));
            }
            _ownerSurname = value;
        }
    }

    public int PurchaseYear
    {
        get { return _purchaseYear; }
        set
        {
            if (value < 1886)
            {
                throw new ArgumentException("Рік придбання не може бути меншим за 1886.", nameof(PurchaseYear));
            }

            if (value > DateTime.Now.Year)
            {
                throw new ArgumentException("Рік придбання не може бути в майбутньому.", nameof(PurchaseYear));
            }

            _purchaseYear = value;
        }
    }

    public double Mileage
    {
        get { return _mileage; }
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("Пробіг не може бути від'ємним.", nameof(Mileage));
            }

            _mileage = value;
        }
    }

    public Car(string brand, string ownerSurname, int purchaseYear, double mileage)
    {
        // Конструктор використовує властивості, які містять валідацію
        Brand = brand;
        OwnerSurname = ownerSurname;
        PurchaseYear = purchaseYear;
        Mileage = mileage;
    }

    // Реалізація інтерфейсу IComparable
    public int CompareTo(Car? other)
    {
        if (other == null)
        {
            return 1;
        }

        return Mileage.CompareTo(other.Mileage);
    }

    public override string ToString()
    {
        return $"Марка: {Brand}, Власник: {OwnerSurname}, Рік придбання: {PurchaseYear}, Пробіг: {Mileage} км";
    }
}