using MedicalAppointment.Api.Interfaces;
using MedicalAppointment.Api.Models;
using Dapper;
using Npgsql;

namespace MedicalAppointment.Api.Services;

public class AdminService : IAdminService
{
    private readonly IConnectionService _connectionService;
    private readonly IAdminValidation _adminValidation;

    public AdminService(IConnectionService connectionService, IAdminValidation adminValidation)
    {
        _connectionService = connectionService;
        _adminValidation = adminValidation;
    }
    
    public async Task<Results<string, int>> CreateUser(User user)
    {
        try
        {
            bool ifUserExist = await _adminValidation.UserExist(user.Email);
            if(ifUserExist) return Results<string, int>.Failure($"{user.Email} already is registered", 404);

            bool ifDocumentExist = await _adminValidation.DocumentExist(user.Document);
            if(ifDocumentExist) return Results<string, int>.Failure($"{user.Document} already is registered", 404);
            
            const string sql = $"""
                                INSERT INTO users
                                (
                                    name,
                                    last_name,
                                    document,
                                    gender,
                                    date_birth,
                                    email,
                                    password,
                                    status,
                                    role_id
                                )
                                VALUES
                                (
                                    @Name,
                                    @LastName,
                                    @Document,
                                    @Gender,
                                    @DateBirth,
                                    @Email,
                                    @Password,
                                    @Status,
                                    @RoleId
                                );
                                """;

            var result = await _connectionService.connection.ExecuteAsync(sql, new
            {
                Name = user.Name,
                LastName = user.LastName,
                Document = user.Document,
                Gender = user.Gender,
                DateBirth = user.DateBirth,
                Email = user.Email,
                Password = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password),
                Status = true,
                RoleId = user.RoleId
            });

            return result > 0
                ? Results<string, int>.Success("Welcome :)", 201)
                : Results<string, int>.Failure("Oh NO!, we can't insert you, try again", 404);
        }
        catch (PostgresException pe) when(pe.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            return Results<string, int>.Failure("The email or document is already registered.", 404);
        }
    }
    
    public async Task<Results<User, int>> GetUserById(Guid id)
    {
        const string sql = """
                           SELECT
                               id,
                               name,
                               last_name AS LastName,
                               document,
                               gender,
                               date_birth::timestamp AS DateBirth,
                               email,
                               password,
                               status,
                               role_id AS RoleId
                           FROM users
                           WHERE id = @Id;
                           """;

        var user = await _connectionService.connection.QueryFirstOrDefaultAsync<User>(sql, new
        {
            Id = id
        });

        return user is null
            ? Results<User, int>.Failure("User not found.", 404)
            : Results<User, int>.Success(user, 200);
    }
    
    public async Task<Results<string, int>> UpdateUser(User user)
    {
        const string sql = """
                           UPDATE users
                           SET
                               name = COALESCE(@Name, name),
                               last_name = COALESCE(@LastName, last_name),
                               document = COALESCE(@Document, document),
                               gender = COALESCE(@Gender, gender),
                               date_birth = COALESCE(@DateBirth, date_birth),
                               email = COALESCE(@Email, email),
                               role_id = COALESCE(@RoleId, role_id)
                           WHERE id = @Id;
                           """;

        var result = await _connectionService.connection.ExecuteAsync(sql, user);

        return result > 0
            ? Results<string, int>.Success("User updated successfully.", 200)
            : Results<string, int>.Failure("User not found.", 404);
    }
    
    public async Task<Results<string, int>> DeactivateUser(Guid id)
    {
        const string sql = """
                           UPDATE users
                           SET status = FALSE
                           WHERE id = @Id;
                           """;

        var result = await _connectionService.connection.ExecuteAsync(sql, new
        {
            Id = id
        });

        return result > 0
            ? Results<string, int>.Success("User deactivated successfully.", 200)
            : Results<string, int>.Failure("User not found.", 404);
    }


    public async Task<Results<(ICollection<User>, int), int>> GetAllUsers(Guid roleId)
    {
        const string sqlUsers = """
                                SELECT
                                    u.id,
                                    u.name,
                                    u.last_name AS "LastName",
                                    u.document,
                                    u.gender,
                                    u.date_birth::timestamp AS "DateBirth",
                                    u.email,
                                    u.status,
                                    u.role_id as RoleId,
                                    r.role AS "Role"
                                FROM users u
                                INNER JOIN role r
                                    ON u.role_id = r.id
                                WHERE u.role_id = 'a56ef8e7-1db7-42e8-ab76-285e7c498cec';
                                """;

        const string sqlCount = """
                                SELECT COUNT(*)
                                FROM users
                                WHERE role_id = @RoleId;
                                """;

        var users = (await _connectionService.connection.QueryAsync<User>(
                sqlUsers,
                new { RoleId = roleId }))
            .ToList();

        var total = await _connectionService.connection.ExecuteScalarAsync<int>(
            sqlCount,
            new { RoleId = roleId });

        return Results<(ICollection<User>, int), int>.Success((users, total), 200);
    }
    
    public async Task<Results<string, int>> ActivateUser(Guid id)
    {
        const string sql = """
                           UPDATE users
                           SET status = true
                           WHERE id = @Id;
                           """;

        var result = await _connectionService.connection.ExecuteAsync(sql, new
        {
            Id = id
        });

        return result > 0
            ? Results<string, int>.Success("User activated successfully.", 200)
            : Results<string, int>.Failure("User not found.", 404);
    }
} 