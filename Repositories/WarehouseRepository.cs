using Microsoft.Data.SqlClient;
using Tutorial9.Exceptions;
using Tutorial9.Model;

namespace Tutorial9.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly string _connectionString;
    
    public WarehouseRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    
    public async Task<int> addProductToWarehouse(AddProductToDB addProductToDb, CancellationToken cancellationToken)
    {
        
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        try
        {
            //--------- updateing fulfill date
            var query1 = @"UPDATE [dbo].[Order]
                             SET [FulfilledAt] = @date";
            await using var checkCommand = new SqlCommand(query1, connection, (SqlTransaction)transaction);
            checkCommand.Parameters.AddWithValue("@date", addProductToDb.DateAdded);

            var updated = await checkCommand.ExecuteNonQueryAsync(cancellationToken);
            
            if(updated <= 0)
                throw new Exception("Update failed");
            
            var insertQuery = @"INSERT INTO [dbo].[Product_Warehouse] (IdWarehouse,IdProduct, IdOrder,Amount,Price, CreatedAt)
                            VALUES (@id_warehouse, @id_product, @id_order, @amount, @price, @date);
                            SELECT SCOPE_IDENTITY();";

            await using var insertCommand = new SqlCommand(insertQuery, connection, (SqlTransaction)transaction);
            insertCommand.Parameters.AddWithValue("@id_product", addProductToDb.IdProduct);
            insertCommand.Parameters.AddWithValue("@id_warehouse", addProductToDb.IdWarehouse);
            insertCommand.Parameters.AddWithValue("@id_order", addProductToDb.IdOrder);
            insertCommand.Parameters.AddWithValue("@amount", addProductToDb.Amount);
            insertCommand.Parameters.AddWithValue("@price", addProductToDb.Price*addProductToDb.Amount);
            insertCommand.Parameters.AddWithValue("@date", addProductToDb.DateAdded);

            var insertedIdObj = await insertCommand.ExecuteScalarAsync(cancellationToken);
            int insertedId = Convert.ToInt32(insertedIdObj);
            
            
            await transaction.CommitAsync(cancellationToken);

            return insertedId;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new Exception($"Database operation failed: {ex.Message}", ex);
        }
  
    }


    public async Task<bool> DoesOrderExistAsync(int id, CancellationToken cancellationToken)
    {
        await using (var connection = new SqlConnection(_connectionString)){
            
            await connection.OpenAsync(cancellationToken);

            var query = @"SELECT COUNT(1) FROM [dbo].[Product] WHERE IdProduct = @id";
            
            await using (var command = new SqlCommand(query, connection)){
                command.Parameters.AddWithValue("@id", id);
                
                var result = await command.ExecuteScalarAsync(cancellationToken);
                
                return Convert.ToInt32(result) > 0;
            }
        }
        
    }

    public async Task<float> GetPriceAsync(int id, CancellationToken cancellationToken)
    {
        await using (var connection = new SqlConnection(_connectionString)){
            
            await connection.OpenAsync(cancellationToken);

            var query = @"SELECT [dbo].[Product].[Price] FROM [dbo].[Product] WHERE IdProduct = @id";
            
            await using (var command = new SqlCommand(query, connection)){
                command.Parameters.AddWithValue("@id", id);
                
                var result = await command.ExecuteScalarAsync(cancellationToken);

                return Convert.ToSingle(result);
            }
        }
    }

    public Task<int> AddProductViaProcedureAsync(AddProductToDB product, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }


    public async Task<bool> DoesWareHouseExistAsync(int id, CancellationToken cancellationToken)
    {
        return true;
    }

    public async Task<int> GetOrderIdAsync(int id,int amount,DateTime date, CancellationToken cancellationToken)
    {
        await using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);

            var query = @"SELECT 
                        [dbo].[Order].[IdOrder],
                        [dbo].[Order].[IdProduct],
                        [dbo].[Order].[Amount],
                        [dbo].[Order].[CreatedAt],
                        [dbo].[Order].[FulfilledAt]
                        FROM [dbo].[Order]       
                        WHERE [dbo].[Order].[IdProduct] = @id";

            await using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    if(! reader.HasRows)
                        throw new NotFoundException("Order not found");

                    while (await reader.ReadAsync(cancellationToken))
                    {
                        var _amount = reader.GetInt32(reader.GetOrdinal("Amount"));
                        if(_amount != amount)
                            throw new BadRequestException("order with this id does not match amount");
                        
                        var _createdAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"));
                        if(date<_createdAt)
                            throw new ConflictException("current date is older than product date");
                        
                        var _fulfilledAt = reader.IsDBNull(reader.GetOrdinal("FulfilledAt"));
                        if(!_fulfilledAt)
                            throw new ConflictException("order is fulfilled");
                        
                        var _orderId = reader.GetInt32(reader.GetOrdinal("IdOrder"));
                        return _orderId;
                    }
                    
                }
                
            }
            
        }
        return -1;
        
    }
     
    public async Task<bool> DoesProductExistAsync(int id, CancellationToken cancellationToken)
    {
        await using (var connection = new SqlConnection(_connectionString)){
            
            await connection.OpenAsync(cancellationToken);

            var query = @"SELECT COUNT(1) FROM [dbo].[Product] WHERE IdProduct = @id";
            
            await using (var command = new SqlCommand(query, connection)){
                command.Parameters.AddWithValue("@id", id);
                
                var result = await command.ExecuteScalarAsync(cancellationToken);
                
                return Convert.ToInt32(result) > 0;
            }
        }
        
    }
    
}