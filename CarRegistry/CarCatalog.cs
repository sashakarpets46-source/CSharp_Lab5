using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

// Клас-каталог автомобілів
public class CarCatalog
{
    private readonly IFileSaver _fileSaver;
    private readonly IFileLoader _fileLoader;
    private readonly string _filePath;

    // Публічна колекція для відображення автомобілів
    public ObservableCollection<Car> Cars { get; }

    // Приватна колекція для зберігання всіх автомобілів
    private List<Car> _allCars;

    public CarCatalog(
        IFileSaver fileSaver,
        IFileLoader fileLoader,
        string filePath)
    {
        _fileSaver = fileSaver;
        _fileLoader = fileLoader;
        _filePath = filePath;

        Cars = new ObservableCollection<Car>();
        _allCars = new List<Car>();
    }

    // Додавання автомобіля
    public void AddCar(Car car)
    {
        if (car == null)
        {
            throw new ArgumentNullException(nameof(car));
        }

        Cars.Add(car);
        _allCars.Add(car);
    }

    // Видалення автомобіля
    public void DeleteCar(Car car)
    {
        if (car == null)
        {
            throw new ArgumentNullException(nameof(car));
        }

        Cars.Remove(car);
        _allCars.Remove(car);
    }

    // Сортування автомобілів за пробігом через LINQ
    public void SortByMileage()
    {
        var sortedCars = _allCars
            .OrderBy(car => car.Mileage)
            .ToList();

        UpdateVisibleCollection(sortedCars);
    }

    // Сортування автомобілів за прізвищем власника через LINQ
    public void SortByOwner()
    {
        var sortedCars = _allCars
            .OrderBy(car => car.OwnerSurname)
            .ToList();

        UpdateVisibleCollection(sortedCars);
    }

    // Фільтрація автомобілів за роком придбання через LINQ
    public void FilterByYear(int year)
    {
        var filtered = _allCars
            .Where(car => car.PurchaseYear < year)
            .ToList();

        UpdateVisibleCollection(filtered);
    }

    // Пошук автомобілів за частиною марки або прізвища власника
    public void SearchCars(string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            ResetFilter();
            return;
        }

        string lowerTerm = searchTerm.ToLowerInvariant();

        var searchResult = _allCars
            .Where(car =>
                car.Brand.ToLowerInvariant().Contains(lowerTerm) ||
                car.OwnerSurname.ToLowerInvariant().Contains(lowerTerm))
            .ToList();

        UpdateVisibleCollection(searchResult);
    }

    // Отримання списку всіх унікальних власників через LINQ
    public List<string> GetUniqueOwners()
    {
        var uniqueOwners = _allCars
            .Select(car => car.OwnerSurname)
            .Distinct()
            .OrderBy(owner => owner)
            .ToList();

        return uniqueOwners;
    }

    // Знаходження трьох найновіших автомобілів
    public void FindThreeNewestCars()
    {
        var newestCars = _allCars
            .OrderByDescending(car => car.PurchaseYear)
            .Take(3)
            .ToList();

        UpdateVisibleCollection(newestCars);
    }

    // Розрахунок середнього року придбання автомобілів
    public double CalculateAveragePurchaseYear()
    {
        if (_allCars.Count == 0)
        {
            return 0;
        }

        return _allCars.Average(car => car.PurchaseYear);
    }

    // Групування автомобілів за роком придбання
    public void GroupCarsByYear()
    {
        var groupedCars = _allCars
            .GroupBy(car => car.PurchaseYear)
            .OrderByDescending(group => group.Key)
            .SelectMany(group => group)
            .ToList();

        UpdateVisibleCollection(groupedCars);
    }

    // Скидання фільтра
    public void ResetFilter()
    {
        UpdateVisibleCollection(_allCars);
    }

    // Оновлення видимої колекції
    private void UpdateVisibleCollection(List<Car> newList)
    {
        Cars.Clear();

        foreach (var car in newList)
        {
            Cars.Add(car);
        }
    }

    // Асинхронне збереження автомобілів у файл
    public async Task SaveAsync()
    {
        await _fileSaver.SaveAsync(_filePath, _allCars);
    }

    // Асинхронне завантаження автомобілів із файлу
    public async Task LoadAsync()
    {
        var loadedCars = await _fileLoader.LoadAsync<Car>(_filePath);

        if (loadedCars != null)
        {
            _allCars = loadedCars;
            UpdateVisibleCollection(_allCars);
        }
    }
}
