using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

// Клас для запису даних у JSON-файл
public class JsonFileWriter : IFileSaver
{
    public async Task SaveAsync<T>(string filePath, List<T> data)
    {
        // Перевіряємо, чи існує директорія, і якщо ні — створюємо її
        string? directoryPath = Path.GetDirectoryName(filePath);

        if (directoryPath != null && !Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Серіалізуємо колекцію в JSON
        var json = JsonSerializer.Serialize(
            data,
            new JsonSerializerOptions { WriteIndented = true });

        // Асинхронно записуємо JSON-рядок у файл
        await File.WriteAllTextAsync(filePath, json);
    }
}
