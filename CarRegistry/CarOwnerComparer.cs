
using System.Collections.Generic;

// Клас для порівняння автомобілів за прізвищами власників
public class CarOwnerComparer : IComparer<Car>
{
    public int Compare(Car? x, Car? y)
    {
        // Перевіряємо на null, щоб уникнути винятків
        if (x == null || y == null)
        {
            return 0;
        }

        // Порівнюємо прізвища власників без урахування регістру
        return string.Compare(
            x.OwnerSurname,
            y.OwnerSurname,
            System.StringComparison.OrdinalIgnoreCase);
    }
}
