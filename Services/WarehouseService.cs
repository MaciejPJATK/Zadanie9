using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices.JavaScript;
using Microsoft.Data.SqlClient;
using Tutorial9.Exceptions;
using Tutorial9.Model;
using Tutorial9.Repositories;

namespace Tutorial9.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository;
    public WarehouseService(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }
    
    
    public async Task<int> AddProductToWarehouseAsync(AddProductRequest request, CancellationToken cancellationToken)
    {
        if(! await _warehouseRepository.DoesProductExistAsync(request.IdProduct,cancellationToken))
            throw new NotFoundException("Product doesnt exist");
        
        if(! await _warehouseRepository.DoesWareHouseExistAsync(request.IdWarehouse,cancellationToken))
            throw new NotFoundException("Warehouse doesnt exist");
        
        var orderid = await _warehouseRepository.GetOrderIdAsync(request.IdProduct,request.Amount,DateTime.Now,cancellationToken);
        
        if(orderid<=0)
            throw new NotFoundException("Order doesnt exist");
        
        if(!await _warehouseRepository.DoesOrderExistAsync(orderid, cancellationToken))
            throw new ConflictException("Order is already being proceeded with");

        var price = await _warehouseRepository.GetPriceAsync(request.IdProduct, cancellationToken);
        if(price <= 0)
            throw new ConflictException("Price is lesser than 0");
        
        
        var dto = new AddProductToDB()
        {
            Amount = request.Amount,
            DateAdded = DateTime.Now,
            IdWarehouse = request.IdWarehouse,
            IdProduct = request.IdProduct,
            Price = price,
            IdOrder = orderid
        };
        var result = await _warehouseRepository.addProductToWarehouse(dto, cancellationToken);
        
        return result;
    }
}