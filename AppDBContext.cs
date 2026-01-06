using back_bd.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
namespace back_bd
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions options) : base(options)
        {
        }
        
        public DbSet<Genero> Generos { get; set; }
        public DbSet<Estudios> Estudios { get; set; }
        public DbSet<Anime> Animes { get; set; }
        public DbSet<Episodios> Episodios { get; set; }
        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Planes> Planes { get; set; }
        public DbSet<Favoritos> Favoritos { get; set; }
        public DbSet<HistorialVisualizaciones> HistorialVisualizaciones { get; set; }
        public DbSet<AnimeGeneros> AnimeGeneros { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnimeGeneros>()
                .HasKey(ag => new { ag.AnimeId, ag.GeneroId });

            modelBuilder.Entity<AnimeGeneros>()
                .HasOne(ag => ag.Anime)
                .WithMany(a => a.AnimeGeneros)
                .HasForeignKey(ag => ag.AnimeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AnimeGeneros>()
                .HasOne(ag => ag.Genero)
                .WithMany(g => g.AnimeGeneros)
                .HasForeignKey(ag => ag.GeneroId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Anime>()
                .HasOne(a => a.Estudio)
                .WithMany(e => e.Animes)
                .HasForeignKey(a => a.EstudioId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Episodios>()
                .HasOne(e => e.Anime)
                .WithMany(a => a.Episodios)
                .HasForeignKey(e => e.AnimeId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Usuarios>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Usuarios>()
                .HasOne(u => u.Plan)
                .WithMany(p => p.Usuarios)
                .HasForeignKey(u => u.PlanId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Usuarios>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Favoritos>()
                .HasOne(f => f.Usuario)
                .WithMany(u => u.Favoritos)
                .HasForeignKey(f => f.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Favoritos>()
                .HasOne(f => f.Anime)
                .WithMany(a => a.Favoritos)
                .HasForeignKey(f => f.AnimeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Favoritos>()
                .HasIndex(f => new { f.UsuarioId, f.AnimeId })
                .IsUnique();

            modelBuilder.Entity<HistorialVisualizaciones>()
                .HasOne(h => h.Usuario)
                .WithMany(u => u.HistorialVisualizaciones)
                .HasForeignKey(h => h.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HistorialVisualizaciones>()
                .HasOne(h => h.Anime)
                .WithMany(a => a.HistorialVisualizaciones)
                .HasForeignKey(h => h.AnimeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HistorialVisualizaciones>()
                .HasOne(h => h.Episodio)
                .WithMany(e => e.HistorialVisualizaciones)
                .HasForeignKey(h => h.EpisodioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HistorialVisualizaciones>()
                .HasIndex(h => new { h.UsuarioId, h.AnimeId })
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity.GetType().GetProperty("FechaCreacion") != null)
                    {
                        entry.Property("FechaCreacion").CurrentValue = DateTime.UtcNow;
                    }

                    if (entry.Entity.GetType().GetProperty("FechaRegistro") != null)
                    {
                        entry.Property("FechaRegistro").CurrentValue = DateTime.UtcNow;
                    }

                    if (entry.Entity.GetType().GetProperty("FechaAgregado") != null)
                    {
                        entry.Property("FechaAgregado").CurrentValue = DateTime.UtcNow;
                    }

                    if (entry.Entity.GetType().GetProperty("FechaVisualizacion") != null)
                    {
                        entry.Property("FechaVisualizacion").CurrentValue = DateTime.UtcNow;
                    }
                    
                    if (entry.Entity.GetType().GetProperty("IsActive") != null)
                    {
                        entry.Property("IsActive").CurrentValue = true;
                    }
                }

                if (entry.State == EntityState.Modified)
                {
                    if (entry.Entity.GetType().GetProperty("FechaCreacion") != null)
                    {
                        entry.Property("FechaCreacion").IsModified = false;
                    }
                    if (entry.Entity.GetType().GetProperty("FechaRegistro") != null)
                    {
                        entry.Property("FechaRegistro").IsModified = false;
                    }

                    if (entry.Entity.GetType().GetProperty("FechaAgregado") != null)
                    {
                        entry.Property("FechaAgregado").IsModified = false;
                    }

                    if (entry.Entity.GetType().GetProperty("FechaVisualizacion") != null)
                    {
                        entry.Property("FechaVisualizacion").IsModified = false;
                    }
                    
                    if (entry.Entity.GetType().GetProperty("FechaModificacion") != null)
                    {
                        entry.Property("FechaModificacion").CurrentValue = DateTime.UtcNow;
                    }
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
