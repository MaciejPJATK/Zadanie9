using Tutorial9.Model;

namespace Tutorial9.Repositories;

public interface IWarehouseRepository
{
    public Task<int> addProductToWarehouse(AddProductToDB addProductToDb, CancellationToken cancellationToken);
    Task<bool> DoesProductExistAsync(int requestIdProduct, CancellationToken cancellationToken);
    
    
    Task<bool> DoesWareHouseExistAsync(int id, CancellationToken cancellationToken);
    
    Task<int> GetOrderIdAsync(int id,int amount,DateTime date, CancellationToken cancellationToken);
    
    Task<bool> DoesOrderExistAsync(int id, CancellationToken cancellationToken);
    
    Task<float> GetPriceAsync(int id, CancellationToken cancellationToken);
    
    // Task<int> AddProductViaProcedureAsync(AddProductToDB product, CancellationToken cancellationToken);
}