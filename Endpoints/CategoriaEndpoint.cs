using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecetApp.Data;
using RecetApp.Dto.Categoria;
using RecetApp.Models;

namespace RecetApp.Endpoints
{
    public static class CategoriaEndpoint
    {
        // Método de extensión para mapear endpoints de categorías
        public static void AddCategoriaEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/categorias").WithTags("categorias");

            // Crear categoría
            group.MapPost("/", async (RecetAppDb db, CrearCategoriaDTO dto) =>
            {
                var errores = new Dictionary<string, string[]>();

                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    errores["Nombre"] = new[] { "El nombre es requerido" };

                if (dto.Nombre?.Length > 100)
                    errores["Nombre"] = new[] { "El nombre no puede exceder 100 caracteres" };

                if (errores.Count > 0)
                    return Results.ValidationProblem(errores);

                var categoria = new Categoria
                {
                    Nombre = dto.Nombre
                };

                db.Categorias.Add(categoria);
                await db.SaveChangesAsync();

                var dtoSalida = new CategoriaDTO(
                    categoria.Id,
                    categoria.Nombre
                );

                return Results.Created($"/api/categorias/{categoria.Id}", dtoSalida);
            });

            // Listar categorías
            group.MapGet("/", async (RecetAppDb db) =>
            {
                var categorias = await db.Categorias
                    .Select(c => new CategoriaDTO(
                        c.Id,
                        c.Nombre
                    ))
                    .ToListAsync();

                return Results.Ok(categorias);
            });

            // Obtener categoría por Id
            group.MapGet("/{id:int}", async (RecetAppDb db, int id) =>
            {
                var categoria = await db.Categorias
                    .Where(c => c.Id == id)
                    .Select(c => new CategoriaDTO(
                        c.Id,
                        c.Nombre
                    ))
                    .FirstOrDefaultAsync();

                return categoria != null ? Results.Ok(categoria) : Results.NotFound();
            });

            // Modificar categoría
            group.MapPut("/{id:int}", async (RecetAppDb db, int id, ModificarCategoriaDTO dto) =>
            {
                var categoria = await db.Categorias.FindAsync(id);
                if (categoria == null) return Results.NotFound();

                if (!string.IsNullOrWhiteSpace(dto.Nombre))
                    categoria.Nombre = dto.Nombre;

                await db.SaveChangesAsync();

                var dtoSalida = new CategoriaDTO(
                    categoria.Id,
                    categoria.Nombre
                );

                return Results.Ok(dtoSalida);
            });
        }
    }
}