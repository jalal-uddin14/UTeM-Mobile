using UTeM_Mobile.Core.Models;

namespace UTeM_Mobile.Core.IServices
{
    public interface IGenericService<T> where T : class
    {
        Task<ListResponse<T>> GetAllAsync(string url, AuthToken? token = null);
        Task<PaginatedResponse<T>> GetPagedListAsync(string url, AuthToken? token = null);
        Task<ObjectResponse<T>> GetDetailsAsync(string url, AuthToken? token = null);
        Task<ObjectResponse<T>> PostAsync(string url, object content, AuthToken? token = null);
        Task<ObjectResponse<T>> PutAsync(string url, object content, AuthToken? token = null);
        Task<ObjectResponse<T>> DeleteAsync(string url, AuthToken? token = null);
        Task<ObjectResponse<T>> PostFile(string url, MultipartFormDataContent content, AuthToken? token = null);
    }
}
