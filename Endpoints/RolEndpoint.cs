using Microsoft.EntityFrameworkCore;
using RecetApp.Data;
using RecetApp.Dto;
using RecetApp.Dto.Rol;
using RecetApp.Dto.Usuario;
using RecetApp.Models;

namespace RecetApp.Endpoints
{
    public static class RolEndpoint
    {

        public static void AddRolEnpoint(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/roles").WithTags("roles");

            group.MapPost("/", async (RecetAppDb db, CrearRolDTO dto) =>
            {

                var errores = new Dictionary<string, string[]>();

                if (string.IsNullOrWhiteSpace(dto.Nombre))
                {
                    errores["Nombre"] = ["El nombre es requerido"];
                }

                if (errores.Count > 0) return Results.ValidationProblem(errores);

                var entity = new Rol
                {
                    Nombre = dto.Nombre
                }; 


                db.Roles.Add(entity);
                await db.SaveChangesAsync();

                var dtoSalida = new RolDTO
                (
                    entity.Id,
                    entity.Nombre
                );

                return Results.Created($"/roles/ {entity.Id}", dtoSalida);

            });

            group.MapGet("/", async(RecetAppDb db) =>
            {

                var consulta = await db.Roles.ToListAsync();

                var roles = consulta.Select(r => new RolDTO
                (
                    r.Id,
                    r.Nombre
                ))
                .OrderBy( r => r.Nombre )
                .ToList();

                return Results.Ok(roles);
            });


            // Obtener rol por Id
            group.MapGet("/{id:int}", async (RecetAppDb db, int id) =>
            {
                var rol = await db.Roles
                    .Where(r => r.Id == id)
                    .Select(u => new RolDTO(
                        u.Id,
                        u.Nombre
                    ))
                    .FirstOrDefaultAsync();

                return rol != null ? Results.Ok(rol) : Results.NotFound();
            });

            // Modificar rol
            group.MapPut("/{id:int}", async (RecetAppDb db, int id, ModificarRolDTO dto) =>
            {
                var rol = await db.Roles.FindAsync(id);
                if (rol == null) return Results.NotFound();

                if (!string.IsNullOrWhiteSpace(dto.Nombre))
                    rol.Nombre = dto.Nombre;

                await db.SaveChangesAsync();

                var dtoSalida = new RolDTO(
                    rol.Id,
                    rol.Nombre
                );

                return Results.Ok(dtoSalida);
            });

            // Eliminar rol
            group.MapDelete("/{id:int}", async (RecetAppDb db, int id) =>
            {
                var rol = await db.Roles.FindAsync(id);

                if (rol == null)
                    return Results.NotFound(new { mensaje = "Rol no encontrado" });

                bool tieneUsuarios = await db.Usuarios.AnyAsync(u => u.RolId == id);
                if (tieneUsuarios)
                    return Results.BadRequest(new { mensaje = "No se puede eliminar el rol porque tiene usuarios asociados." });

                db.Roles.Remove(rol);
                await db.SaveChangesAsync();

                return Results.Ok(new { mensaje = "Rol eliminado correctamente" });
            });

        }
    }
}
