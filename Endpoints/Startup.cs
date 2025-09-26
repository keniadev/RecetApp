using RecetApp.Endpoints;

namespace Biblioteca.Endpoints
{
    public static class Startup
    {

        public static void UsarEnpoints(this WebApplication app)
        {
            UsuarioEndpoint.AddUsuarioEnpoint(app);
            RolEndpoint.AddRolEnpoint(app);
           
        }
    }
}
