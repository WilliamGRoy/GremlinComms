using ThomTwo.Domain.Entities;

namespace ThomTwo.Domain.Repository;

public interface IOfficerRepository
{
    Task<Officer> GetByIdAsync(string id);
    Task<IEnumerable<Officer>> GetAllAsync();
    Task AddAsync(Officer person);
    Task UpdateAsync(Officer person);
    Task DeleteAsync(string id);
}
