using Microsoft.EntityFrameworkCore;
using RecetApp.Models;

namespace RecetApp.Data
{
    public class RecetAppDb : DbContext
    {
        public RecetAppDb(DbContextOptions<RecetAppDb> options) : base(options) { }

        // DbSets 
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<RecetaFavorita> RecetasFavoritas { get; set; }
        public DbSet<Receta> Recetas { get; set; }
        public DbSet<RecetaIngrediente> RecetaIngredientes { get; set; }
        public DbSet<Ingrediente> Ingredientes { get; set; }
        public DbSet<CategoriaReceta> CategoriaRecetas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Usuario
            modelBuilder.Entity<Usuario>(e =>
            {
                e.HasKey(u => u.Id);
                e.Property(u => u.Nombre).IsRequired().HasMaxLength(100);
                e.Property(u => u.Email).IsRequired().HasMaxLength(100);
                e.Property(u => u.Clave).IsRequired().HasMaxLength(100);
            // Relación con Rol
                e.HasOne(u => u.Rol)
                 .WithMany(r => r.Usuarios)
                 .HasForeignKey(u => u.RolId)
                 .OnDelete(DeleteBehavior.Restrict);
                e.HasIndex(u => u.Email).IsUnique();
                e.HasMany(u => u.Recetas)
                 .WithOne(r => r.Usuario)
                 .HasForeignKey(r => r.UsuarioId)
                 .OnDelete(DeleteBehavior.Restrict);
                e.HasMany(u => u.RecetaFavoritas)
                 .WithOne(rf => rf.Usuario)
                 .HasForeignKey(rf => rf.UsuarioId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
            // Rol
            modelBuilder.Entity<Rol>(e =>
            {
                e.HasKey(r => r.Id);
                e.Property(r => r.Nombre).IsRequired().HasMaxLength(50);
            });
            // RecetaFavorita
            modelBuilder.Entity<RecetaFavorita>(e =>
            {
                e.HasKey(rf => rf.Id);
                e.HasOne(rf => rf.Usuario)
                 .WithMany(u => u.RecetaFavoritas)
                 .HasForeignKey(rf => rf.UsuarioId)
                 .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(rf => rf.Receta)
                 .WithMany(r => r.RecetaFavoritas)
                 .HasForeignKey(rf => rf.RecetaId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
            // Receta
            modelBuilder.Entity<Receta>(e =>
            {
                e.HasKey(r => r.Id);
                e.Property(r => r.Titulo).IsRequired().HasMaxLength(200);
                e.Property(r => r.Descripcion).IsRequired();
                e.HasOne(r => r.Usuario)
                 .WithMany(u => u.Recetas)
                 .HasForeignKey(r => r.UsuarioId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
            // RecetaIngrediente
            modelBuilder.Entity<RecetaIngrediente>(e =>
            {
                e.HasKey(ri => ri.Id);
                e.HasOne(ri => ri.Receta)
                 .WithMany(r => r.RecetaIngredientes)
                 .HasForeignKey(ri => ri.RecetaId)
                 .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(ri => ri.Ingrediente)
                 .WithMany(i => i.RecetaIngredientes)
                 .HasForeignKey(ri => ri.IngredienteId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
            // Ingrediente
            modelBuilder.Entity<Ingrediente>(e =>
            {
                e.HasKey(i => i.Id);
                e.Property(i => i.Nombre).IsRequired().HasMaxLength(100);
            });
            // CategoriaReceta
            modelBuilder.Entity<CategoriaReceta>(e =>
            {
                e.HasKey(cr => cr.Id);
                e.HasOne(cr => cr.Receta)
                 .WithMany(r => r.CategoriaRecetas)
                 .HasForeignKey(cr => cr.RecetaId)
                 .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(cr => cr.Categoria)
                 .WithMany(c => c.CategoriaRecetas)
                 .HasForeignKey(cr => cr.CategoriaId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
            // Categoria
            modelBuilder.Entity<Categoria>(e =>
            {
                e.HasKey(c => c.Id);
                e.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
            });
             base.OnModelCreating(modelBuilder);

        }
    }
}