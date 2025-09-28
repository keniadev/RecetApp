using RecetApp.Endpoints;

namespace Biblioteca.Endpoints
{
    public static class Startup
    {

        public static void UsarEnpoints(this WebApplication app)
        {
            UsuarioEndpoint.AddUsuarioEnpoint(app);
            RolEndpoint.AddRolEnpoint(app);
            CategoriaEndpoint.AddCategoriaEndpoints(app);
            CategoriaRecetaEndpoint.AddCategoriaRecetaEndpoints(app);
            RecetaEndpoint.AddRecetaEndpoint(app);
            RecetaFavoritaEndpoint.AddRecetaFavoritaEndpoints(app);
            ImagenEndpoint.AddImagenEndpoint(app);
            RecetaIngredienteEndpoint.AddRecetaIngredienteEndpoints(app);
            IngredienteEndpoint.AddIngredienteEndpoints(app);

        }
    }
}
