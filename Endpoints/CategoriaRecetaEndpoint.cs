using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecetApp.Data;
using RecetApp.Dto.CategoriaReceta;
using RecetApp.Dto.Categoria;
using RecetApp.Models;

namespace RecetApp.Endpoints
{
    public static class CategoriaRecetaEndpoint
    {
        public static void AddCategoriaRecetaEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/recetas/{recetaId:int}/categorias").WithTags("CategoriaReceta");

            // Asignar una categoría a una receta (POST)
            group.MapPost("/", async (RecetAppDb db, int recetaId, CrearCategoriaRecetaDTO dto) =>
            {
                // Validar que la receta y la categoría existan
                var recetaExiste = await db.Recetas.AnyAsync(r => r.Id == recetaId);
                var categoriaExiste = await db.Categorias.AnyAsync(c => c.Id == dto.CategoriaId);

                if (!recetaExiste)
                {
                    return Results.NotFound($"La receta con ID {recetaId} no fue encontrada.");
                }
                if (!categoriaExiste)
                {
                    return Results.NotFound($"La categoría con ID {dto.CategoriaId} no fue encontrada.");
                }

                // Validar si la relación ya existe para evitar duplicados
                var relacionExistente = await db.CategoriaRecetas
                    .AnyAsync(cr => cr.RecetaId == recetaId && cr.CategoriaId == dto.CategoriaId);

                if (relacionExistente)
                {
                    return Results.Conflict("Esta categoría ya está asignada a esta receta.");
                }

                var categoriaReceta = new CategoriaReceta
                {
                    RecetaId = recetaId,
                    CategoriaId = dto.CategoriaId
                };

                db.CategoriaRecetas.Add(categoriaReceta);
                await db.SaveChangesAsync();

                // DTO de salida para confirmar la creación
                var dtoSalida = new CategoriaRecetaDTO(categoriaReceta.Id, categoriaReceta.RecetaId, categoriaReceta.CategoriaId);

                return Results.Created($"/api/recetas/{recetaId}/categorias/{categoriaReceta.Id}", dtoSalida);
            });

            // Eliminar una categoría de una receta (DELETE)
            group.MapDelete("/{categoriaId:int}", async (RecetAppDb db, int recetaId, int categoriaId) =>
            {
                var categoriaReceta = await db.CategoriaRecetas
                    .FirstOrDefaultAsync(cr => cr.RecetaId == recetaId && cr.CategoriaId == categoriaId);

                if (categoriaReceta == null)
                {
                    return Results.NotFound("La relación entre la receta y la categoría no fue encontrada.");
                }

                db.CategoriaRecetas.Remove(categoriaReceta);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });

            // Listar las categorías de una receta (GET)
            group.MapGet("/", async (RecetAppDb db, int recetaId) =>
            {
                var categorias = await db.CategoriaRecetas
                    .Where(cr => cr.RecetaId == recetaId)
                    .Select(cr => new CategoriaDTO(
                        cr.Categoria!.Id,
                        cr.Categoria.Nombre
                    ))
                    .ToListAsync();

                if (categorias.Count == 0 && !await db.Recetas.AnyAsync(r => r.Id == recetaId))
                {
                    return Results.NotFound($"La receta con ID {recetaId} no fue encontrada.");
                }

                return Results.Ok(categorias);
            });




            // [NUEVOS ENDPOINTS QUE TE FALTAN - AGREGADOS AL FINAL]

            // 1. GET: Traer TODOS los registros de CategoriaReceta (sin filtrar por receta)
            group.MapGet("/all", async (RecetAppDb db) =>
            {
                var todasLasRelaciones = await db.CategoriaRecetas
                    .Include(cr => cr.Receta)
                    .Include(cr => cr.Categoria)
                    .Select(cr => new
                    {
                        cr.Id,
                        cr.RecetaId,
                        cr.CategoriaId,
                        RecetaTitulo = cr.Receta!.Titulo,
                        CategoriaNombre = cr.Categoria!.Nombre
                    })
                    .ToListAsync();

                return Results.Ok(todasLasRelaciones);
            });

            // 2. PUT: Modificar una relación CategoriaReceta
            group.MapPut("/{id:int}", async (RecetAppDb db, int id, ModificarCategoriaRecetaDTO dto) =>
            {
                var relacionExistente = await db.CategoriaRecetas
                    .FirstOrDefaultAsync(cr => cr.Id == id);

                if (relacionExistente == null)
                {
                    return Results.NotFound("Relación no encontrada.");
                }

                // Validar que la nueva categoría exista
                var categoriaExiste = await db.Categorias.AnyAsync(c => c.Id == dto.CategoriaId);
                if (!categoriaExiste)
                {
                    return Results.NotFound($"La categoría con ID {dto.CategoriaId} no existe.");
                }

                // Validar que no exista otra relación igual
                var duplicado = await db.CategoriaRecetas
                    .AnyAsync(cr => cr.RecetaId == relacionExistente.RecetaId &&
                                   cr.CategoriaId == dto.CategoriaId &&
                                   cr.Id != id);

                if (duplicado)
                {
                    return Results.Conflict("Ya existe esta relación para esta receta.");
                }

                // Actualizar
                relacionExistente.CategoriaId = dto.CategoriaId;
                await db.SaveChangesAsync();

                return Results.Ok(new CategoriaRecetaDTO(
                    relacionExistente.Id,
                    relacionExistente.RecetaId,
                    relacionExistente.CategoriaId
                ));
            });
        }
    }
}

