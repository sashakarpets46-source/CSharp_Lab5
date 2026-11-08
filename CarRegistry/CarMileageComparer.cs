using System.Collections.Generic;

// Клас для порівняння автомобілів за пробігом
public class CarMileageComparer : IComparer<Car>
{
    public int Compare(Car? x, Car? y)
    {
        if (x == null && y == null)
            return 0;

        if (x == null)
            return -1;

        if (y == null)
            return 1;

        return x.Mileage.CompareTo(y.Mileage);
    }
}