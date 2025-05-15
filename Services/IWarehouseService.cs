using Tutorial9.Model;

namespace Tutorial9.Services;

public interface IWarehouseService
{
    Task<int> AddProductToWarehouseAsync(AddProductRequest request, CancellationToken cancellationToken);
}