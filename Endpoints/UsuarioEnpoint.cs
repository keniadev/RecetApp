using Microsoft.EntityFrameworkCore;
using RecetApp.Data;
using RecetApp.Dto;
using RecetApp.Dto.Usuario;
using RecetApp.Models;

namespace RecetApp.Endpoints
{
    public static class UsuarioEndpoint
    {
        public static void AddUsuarioEnpoint(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/usuarios").WithTags("usuarios");

            // Crear usuario
            group.MapPost("/", async (RecetAppDb db, CrearUsuarioDTO dto) =>
            {
                var errores = new Dictionary<string, string[]>();

                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    errores["Nombre"] = new[] { "El nombre es requerido" };

                if (string.IsNullOrWhiteSpace(dto.Email))
                    errores["Email"] = new[] { "El email es requerido" };

                if (string.IsNullOrWhiteSpace(dto.Clave))
                    errores["Clave"] = new[] { "La contraseña es requerida" };

                // Validar email único
                if (await db.Usuarios.AnyAsync(u => u.Email == dto.Email))
                    errores["Email"] = new[] { "El email ya está registrado" };

                if (errores.Count > 0)
                    return Results.ValidationProblem(errores);

                // Crear usuario con rol por defecto (Usuario)
                var usuario = new Usuario
                {
                    Nombre = dto.Nombre,
                    Email = dto.Email,
                    Clave = dto.Clave, 
                    RolId = 2 // Rol "Usuario" por defecto
                };

                db.Usuarios.Add(usuario);
                await db.SaveChangesAsync();

                // DTO de salida
                var dtoSalida = new UsuarioDTO(
                    usuario.Id,
                    usuario.Nombre,
                    usuario.Email,
                    usuario.FotoPerfilUrl
                    
                );

                return Results.Created($"/api/usuarios/{usuario.Id}", dtoSalida);
            });

            // Listar usuarios
            group.MapGet("/", async (RecetAppDb db) =>
            {
                var usuarios = await db.Usuarios
                    .Include(u => u.Rol)
                    .Select(u => new UsuarioDTO(
                        u.Id,
                        u.Nombre,
                        u.Email,
                        u.FotoPerfilUrl
                       
                    ))
                    .ToListAsync();

                return Results.Ok(usuarios);
            });

            // Obtener usuario por Id
            group.MapGet("/{id:int}", async (RecetAppDb db, int id) =>
            {
                var usuario = await db.Usuarios
                    .Include(u => u.Rol)
                    .Where(u => u.Id == id)
                    .Select(u => new UsuarioDTO(
                        u.Id,
                        u.Nombre,
                        u.Email,
                        u.FotoPerfilUrl
                        
                    ))
                    .FirstOrDefaultAsync();

                return usuario != null ? Results.Ok(usuario) : Results.NotFound();
            });

            // Modificar usuario
            group.MapPut("/{id:int}", async (RecetAppDb db, int id, ModificarUsuarioDTO dto) =>
            {
                var usuario = await db.Usuarios.FindAsync(id);
                if (usuario == null) return Results.NotFound();

                if (!string.IsNullOrWhiteSpace(dto.Nombre))
                    usuario.Nombre = dto.Nombre;

                if (!string.IsNullOrWhiteSpace(dto.Email))
                    usuario.Email = dto.Email;

                if (!string.IsNullOrWhiteSpace(dto.Clave))
                    usuario.Clave = dto.Clave; 

                await db.SaveChangesAsync();

                var dtoSalida = new UsuarioDTO(
                    usuario.Id,
                    usuario.Nombre,
                    usuario.Email,
                    usuario.FotoPerfilUrl
                 
                );

                return Results.Ok(dtoSalida);
            });


            // Eliminar usuario
            group.MapDelete("/{id:int}", async (RecetAppDb db, int id) =>
            {
                var usuario = await db.Usuarios.FindAsync(id);

                if (usuario == null)
                    return Results.NotFound(new { mensaje = "Usuario no encontrado" });

                db.Usuarios.Remove(usuario);
                await db.SaveChangesAsync();

                return Results.Ok(new { mensaje = "Usuario eliminado correctamente" });
            });

        }
    }
}
