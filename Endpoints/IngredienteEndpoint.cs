using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecetApp.Data;
using RecetApp.Dto.Ingrediente;
using RecetApp.Models;

namespace RecetApp.Endpoints
{
    public static class IngredienteEndpoint
    {
        public static void AddIngredienteEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/ingredientes").WithTags("ingredientes");

            // Crear ingrediente
            group.MapPost("/", async (RecetAppDb db, CrearIngredienteDTO dto) =>
            {
                var errores = new Dictionary<string, string[]>();

                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    errores["Nombre"] = new[] { "El nombre es requerido" };

                if (string.IsNullOrWhiteSpace(dto.UnidadMedida))
                    errores["UnidadMedida"] = new[] { "La unidad de medida es requerida" };

                if (dto.Nombre?.Length > 100)
                    errores["Nombre"] = new[] { "El nombre no puede exceder 100 caracteres" };

                if (errores.Count > 0)
                    return Results.ValidationProblem(errores);

                var ingrediente = new Ingrediente
                {
                    Nombre = dto.Nombre,
                    UnidadMedida = dto.UnidadMedida
                };

                db.Ingredientes.Add(ingrediente);
                await db.SaveChangesAsync();

                var dtoSalida = new IngredienteDTO(
                    ingrediente.Id,
                    ingrediente.Nombre,
                    ingrediente.UnidadMedida
                );

                return Results.Created($"/api/ingredientes/{ingrediente.Id}", dtoSalida);
            });

            // Listar ingredientes
            group.MapGet("/", async (RecetAppDb db) =>
            {
                var ingredientes = await db.Ingredientes
                    .Select(i => new IngredienteDTO(
                        i.Id,
                        i.Nombre,
                        i.UnidadMedida
                    ))
                    .ToListAsync();

                return Results.Ok(ingredientes);
            });

            // Obtener ingrediente por Id
            group.MapGet("/{id:int}", async (RecetAppDb db, int id) =>
            {
                var ingrediente = await db.Ingredientes
                    .Where(i => i.Id == id)
                    .Select(i => new IngredienteDTO(
                        i.Id,
                        i.Nombre,
                        i.UnidadMedida
                    ))
                    .FirstOrDefaultAsync();

                return ingrediente != null ? Results.Ok(ingrediente) : Results.NotFound();
            });

            // Modificar ingrediente
            group.MapPut("/{id:int}", async (RecetAppDb db, int id, ModificarIngredienteDTO dto) =>
            {
                var ingrediente = await db.Ingredientes.FindAsync(id);
                if (ingrediente == null) return Results.NotFound();

                if (!string.IsNullOrWhiteSpace(dto.Nombre))
                    ingrediente.Nombre = dto.Nombre;

                if (!string.IsNullOrWhiteSpace(dto.UnidadMedida))
                    ingrediente.UnidadMedida = dto.UnidadMedida;

                await db.SaveChangesAsync();

                var dtoSalida = new IngredienteDTO(
                    ingrediente.Id,
                    ingrediente.Nombre,
                    ingrediente.UnidadMedida
                );

                return Results.Ok(dtoSalida);
            });
            // Eliminar Ingredientes
            group.MapDelete("/{id:int}", async (RecetAppDb db, int id) =>
            {
                var Ingrediente = await db.Ingredientes.FindAsync(id);
                if (Ingrediente == null) return Results.NotFound();
                db.Ingredientes.Remove(Ingrediente);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}