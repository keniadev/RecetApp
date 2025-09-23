using Microsoft.EntityFrameworkCore;
using RecetApp.Data;
using RecetApp.Dto;
using RecetApp.Dto.Rol;
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


        }
    }
}
