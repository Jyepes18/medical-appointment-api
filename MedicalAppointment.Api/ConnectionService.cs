using System.Data;
using Npgsql;

namespace MedicalAppointment.Api;

public class ConnectionService : IConnectionService,  IDisposable
{
    private bool _disposedValue;
    public NpgsqlConnection connection { get; }
    public IConfiguration _Configuration;

    public ConnectionService(IConfiguration configuration)
    {
        _Configuration = configuration;
        
        connection = new NpgsqlConnection(_Configuration["ConnectionStrings:DefaultConnection"]);
        if (connection == null)
        {
            throw new ArgumentNullException("The connection wasn't successfuly");
        }
        connection.Open();
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                connection.Close();
            }

            _disposedValue = true;
        }
    }
    
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
