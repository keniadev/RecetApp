using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecetApp.Data;
using RecetApp.Models;
using RecetApp.Dto.RecetaIngrediente;

namespace RecetApp.Endpoints
{
    public static class RecetaIngredienteEndpoint
    {
        public static void AddRecetaIngredienteEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/recetas-ingredientes").WithTags("recetas-ingredientes");

            // Crear asociación (Receta-Ingrediente con Cantidad)
            group.MapPost("/", async (RecetAppDb db, CrearRecetaIngredienteDTO dto) =>
            {
                var errores = new Dictionary<string, string[]>();

                if (dto.RecetaId <= 0)
                    errores["RecetaId"] = new[] { "RecetaId es requerido" };

                if (dto.IngredienteId <= 0)
                    errores["IngredienteId"] = new[] { "IngredienteId es requerido" };

                if (dto.Cantidad <= 0)
                    errores["Cantidad"] = new[] { "Cantidad debe ser mayor a 0" };

                if (errores.Count > 0)
                    return Results.ValidationProblem(errores);

                var recIng = new RecetaIngrediente
                {
                    RecetaId = dto.RecetaId,
                    IngredienteId = dto.IngredienteId,
                    Cantidad = dto.Cantidad
                };

                db.RecetaIngredientes.Add(recIng);
                await db.SaveChangesAsync();

                var dtoSalida = new RecetaIngredienteDTO(
                    recIng.Id,
                    recIng.RecetaId,
                    recIng.IngredienteId,
                    recIng.Cantidad
                );

                return Results.Created($"/api/recetas-ingredientes/{recIng.Id}", dtoSalida);
            });

            // Listar todas las asociaciones
            group.MapGet("/", async (RecetAppDb db) =>
            {
                var lista = await db.RecetaIngredientes
                    .Select(ri => new RecetaIngredienteDTO(
                        ri.Id,
                        ri.RecetaId,
                        ri.IngredienteId,
                        ri.Cantidad
                    ))
                    .ToListAsync();

                return Results.Ok(lista);
            });

            // Obtener asociación por Id
            group.MapGet("/{id:int}", async (RecetAppDb db, int id) =>
            {
                var ri = await db.RecetaIngredientes
                    .Where(r => r.Id == id)
                    .Select(r => new RecetaIngredienteDTO(
                        r.Id,
                        r.RecetaId,
                        r.IngredienteId,
                        r.Cantidad
                    ))
                    .FirstOrDefaultAsync();

                return ri != null ? Results.Ok(ri) : Results.NotFound();
            });

            // Actualizar Cantidad (solo ejemplo)
            group.MapPut("/{id:int}", async (RecetAppDb db, int id, ModificarRecetaIngredienteDTO dto) =>
            {
                var ri = await db.RecetaIngredientes.FindAsync(id);
                if (ri == null) return Results.NotFound();

                if (dto.Cantidad.HasValue)
                    ri.Cantidad = dto.Cantidad.Value;

                await db.SaveChangesAsync();

                var dtoSalida = new RecetaIngredienteDTO(
                    ri.Id,
                    ri.RecetaId,
                    ri.IngredienteId,
                    ri.Cantidad
                );

                return Results.Ok(dtoSalida);
            });

            // Eliminar asociación
            group.MapDelete("/{id:int}", async (RecetAppDb db, int id) =>
            {
                var ri = await db.RecetaIngredientes.FindAsync(id);
                if (ri == null) return Results.NotFound();

                db.RecetaIngredientes.Remove(ri);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}