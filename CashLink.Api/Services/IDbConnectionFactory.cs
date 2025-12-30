using System.Data;

namespace CashLink.Api.Services;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
