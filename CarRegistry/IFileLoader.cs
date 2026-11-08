using System.Collections.Generic;
using System.Threading.Tasks;

// Інтерфейс для завантаження списку об'єктів певного типу
public interface IFileLoader
{
    // Асинхронне завантаження списку з файлу
    Task<List<T>?> LoadAsync<T>(string filePath);
}
