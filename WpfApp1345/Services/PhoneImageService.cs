
#nullable enable

using System;
using System.IO;
using System.Linq;

namespace WpfApp1345.Services
{
    public static class PhoneImageService
    {
        private static readonly string imageDirectory =
            FindImageDirectory();

        // Полный путь к папке с изображениями.
        public static string ImageDirectory => imageDirectory;

        // Находим папку проекта, чтобы изображения хранились
        // внутри проекта, а не терялись при обычной сборке.
        private static string FindImageDirectory()
        {
            DirectoryInfo? current =
                new DirectoryInfo(AppContext.BaseDirectory);

            while (current != null)
            {
                string projectFile = Path.Combine(
                    current.FullName,
                    "WpfApp1345.csproj");

                if (File.Exists(projectFile))
                {
                    return Path.Combine(
                        current.FullName,
                        "Images",
                        "PhonePictures");
                }

                current = current.Parent;
            }

            // Запасной путь, если приложение запущено
            // без файлов проекта.
            return Path.Combine(
                AppContext.BaseDirectory,
                "Images",
                "PhonePictures");
        }

        // Копирование выбранного изображения.
        public static string CopyImage(string sourcePath)
        {
            if (string.IsNullOrWhiteSpace(sourcePath) ||
                !File.Exists(sourcePath))
            {
                throw new FileNotFoundException(
                    "Выбранное изображение не найдено.");
            }

            string extension =
                Path.GetExtension(sourcePath).ToLowerInvariant();

            string[] allowedExtensions =
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".bmp",
                ".gif"
            };

            if (!allowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException(
                    "Неподдерживаемый формат изображения.");
            }

            Directory.CreateDirectory(imageDirectory);

            // Уникальное имя предотвращает перезапись
            // одинаково названных файлов.
            string fileName =
                Guid.NewGuid().ToString("N") + extension;

            string destinationPath =
                Path.Combine(imageDirectory, fileName);

            File.Copy(sourcePath, destinationPath, false);

            return Path.GetFullPath(destinationPath);
        }

        // Удаление только тех изображений,
        // которые находятся в нашей папке приложения.
        public static void DeleteIfManaged(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath) ||
                imagePath == "No image")
            {
                return;
            }

            string fullPath = Path.GetFullPath(imagePath);
            string rootPath = Path.GetFullPath(imageDirectory);

            string rootPrefix = rootPath.EndsWith(
                Path.DirectorySeparatorChar.ToString(),
                StringComparison.Ordinal)
                ? rootPath
                : rootPath + Path.DirectorySeparatorChar;

            // Не удаляем произвольные файлы пользователя.
            if (!fullPath.StartsWith(
                rootPrefix,
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
