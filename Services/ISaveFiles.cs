namespace back_bd.Services
{
    public interface ISaveFiles
    {
        Task<string> SaveFile(string container, IFormFile file);
        Task DeleteFile(string container, string? fileRoute);
        async Task<string> EditFile(string container, IFormFile file, string? fileRoute) { 
            await DeleteFile(container, fileRoute);
            return await SaveFile(container, file);
        }

    }
}
