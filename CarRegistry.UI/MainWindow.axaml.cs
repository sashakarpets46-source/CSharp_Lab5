
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace CarRegistry.UI;

public partial class MainWindow : Window
{
    private readonly CarCatalog _catalog;

    public MainWindow()
    {
        InitializeComponent();

        string filePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "CarRegistry",
            "cars.json");

        _catalog = new CarCatalog(
            new JsonFileWriter(),
            new JsonFileReader(),
            filePath);

        CarsList.ItemsSource = _catalog.Cars;

        _ = LoadCarsAsync();
    }

    // Додавання автомобіля
    private void AddButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            string brand = BrandTextBox.Text ?? "";
            string owner = OwnerTextBox.Text ?? "";

            if (!int.TryParse(YearTextBox.Text, out int year))
            {
                StatusText.Text = "Введіть правильний рік.";
                return;
            }

            if (!double.TryParse(
                MileageTextBox.Text,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out double mileage))
            {
                StatusText.Text = "Введіть правильний пробіг.";
                return;
            }

            var car = new Car(brand, owner, year, mileage);

            _catalog.AddCar(car);

            BrandTextBox.Text = "";
            OwnerTextBox.Text = "";
            YearTextBox.Text = "";
            MileageTextBox.Text = "";

            StatusText.Text = "Автомобіль успішно додано.";
        }
        catch (ArgumentException ex)
        {
            StatusText.Text = ex.Message;
        }
    }

    // Видалення автомобіля
    private void DeleteButton_Click(object? sender, RoutedEventArgs e)
    {
        if (CarsList.SelectedItem is Car selectedCar)
        {
            _catalog.DeleteCar(selectedCar);
            StatusText.Text = "Автомобіль видалено.";
        }
        else
        {
            StatusText.Text = "Спочатку виберіть автомобіль.";
        }
    }

    // Сортування за пробігом
private void SortButton_Click(object? sender, RoutedEventArgs e)
{
    _catalog.SortByMileage();
    StatusText.Text = "Автомобілі відсортовано за пробігом.";
}

    // Фільтрація за роком
    private void FilterButton_Click(object? sender, RoutedEventArgs e)
    {
        if (!int.TryParse(FilterYearTextBox.Text, out int year))
        {
            StatusText.Text = "Введіть правильний рік.";
            return;
        }

        _catalog.FilterByYear(year);
        StatusText.Text = $"Показано автомобілі до {year} року.";
    }

    // Скидання фільтра
    private void ResetButton_Click(object? sender, RoutedEventArgs e)
    {
        _catalog.ResetFilter();
        StatusText.Text = "Фільтр скинуто.";
    }

    // Збереження
    private async void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            await _catalog.SaveAsync();
            StatusText.Text = "Автомобілі успішно збережено.";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Помилка збереження: {ex.Message}";
        }
    }

    // Завантаження
    private async void LoadButton_Click(object? sender, RoutedEventArgs e)
    {
        await LoadCarsAsync();
    }

    private async Task LoadCarsAsync()
    {
        try
        {
            await _catalog.LoadAsync();
            StatusText.Text = "Дані завантажено.";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Помилка завантаження: {ex.Message}";
        }
    }
}
