using Dapper;
using MedicalAppointment.Api.Interfaces;

namespace MedicalAppointment.Api.Services;

public class AdminValidation : IAdminValidation
{
    private readonly IConnectionService _connectionService;

    public AdminValidation(IConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    public async Task<bool> UserExist(string email)
    {
        const string validateIfUserExist = """
                                           select count(*) from users where email = @Email
                                           """;

        var counts = await _connectionService.connection.QueryFirstOrDefaultAsync<int>(validateIfUserExist, new
        {
            Email = email
        });
        
        return counts > 0;
    }

    public async Task<bool> DocumentExist(string document)
    {
        const string validateIfUserExist = """
                                           select count(*) from users where document = @Document;
                                           """;

        var counts = await _connectionService.connection.QueryFirstOrDefaultAsync<int>(validateIfUserExist, new
        {
            Document = document
        });
        
        return counts > 0;
    }
}