using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

// Клас для читання даних із JSON-файлу
public class JsonFileReader : IFileLoader
{
    public async Task<List<T>?> LoadAsync<T>(string filePath)
    {
        // Перевіряємо, чи існує файл
        if (!File.Exists(filePath))
        {
            return null;
        }

        // Асинхронно читаємо весь текст із файлу
        var json = await File.ReadAllTextAsync(filePath);

        // Десеріалізуємо JSON-рядок у колекцію
        return JsonSerializer.Deserialize<List<T>>(json);
    }
}
