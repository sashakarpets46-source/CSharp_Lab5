
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

    // Сортування автомобілів за пробігом
    public void SortByMileage()
    {
        var sortedCars = new List<Car>(_allCars);

        sortedCars.Sort(new CarMileageComparer());

        UpdateVisibleCollection(sortedCars);
    }

    // Фільтрація автомобілів за роком придбання
    public void FilterByYear(int year)
    {
        var filtered = _allCars
            .Where(car => car.PurchaseYear < year)
            .ToList();

        UpdateVisibleCollection(filtered);
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
