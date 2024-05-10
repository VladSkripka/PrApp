namespace WebApplication2.Infrastructure.Services;

public class FileService
{
    public async Task<string> SaveFileAsync(IFormFile? schema)
    {
        if (schema == null || schema.Length == 0)
        {
            throw new ArgumentNullException("Schema can't be null");
        }

        var fileName = Guid.NewGuid() + Path.GetExtension(schema.FileName);
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await schema.CopyToAsync(fileStream);
        return fileName;
    }
}