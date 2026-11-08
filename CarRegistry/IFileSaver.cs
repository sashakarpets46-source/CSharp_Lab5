using System.Collections.Generic;
using System.Threading.Tasks;

// Інтерфейс для збереження списку об'єктів певного типу
public interface IFileSaver
{
    // Асинхронне збереження списку у файл
    Task SaveAsync<T>(string filePath, List<T> data);
}
