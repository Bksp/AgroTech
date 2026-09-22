using AgroTech.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace AgroTech.DAL
{
    public class AgroTechDbContext : DbContext
    {
        public AgroTechDbContext()
        {
        }

        public AgroTechDbContext(DbContextOptions<AgroTechDbContext> options) : base(options)
        {
        }

        public DbSet<Rol> Roles { get; set; } = null!;
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Parcela> Parcelas { get; set; } = null!;
        public DbSet<Cultivo> Cultivos { get; set; } = null!;


        private const string Host = "aws-0-us-east-1.pooler.supabase.com";
        private const string Port = "5432";
        private const string DbName = "postgres";
        private const string UserId = "postgres.eonkkmzfswxhgljrplmt";
        private const string Password = "agrotech1010.";

        public static string ConnectionString =>
            $"Host={Host};Port={Port};Database={DbName};Username={UserId};Password={Password};SSL Mode=Require;Trust Server Certificate=true;";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //---------------- Roles ----------------
            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("roles");
                entity.HasKey(r => r.IdRol);
                entity.Property(r => r.IdRol).HasColumnName("id_rol");
                entity.Property(r => r.NombreRol).HasColumnName("nombre_rol");
                entity.Property(r => r.Descripcion).HasColumnName("descripcion");
            });

            //---------------- Usuarios ----------------
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("usuarios");
                entity.HasKey(u => u.IdUsuario);
                entity.Property(u => u.IdUsuario).HasColumnName("id_usuario");
                entity.Property(u => u.IdRol).HasColumnName("id_rol");
                entity.Property(u => u.NombreCompleto).HasColumnName("nombre_completo");
                entity.Property(u => u.CorreoElectronico).HasColumnName("correo_electronico");
                entity.Property(u => u.PasswordHash).HasColumnName("password_hash");
                entity.Property(u => u.Estado).HasColumnName("estado");
                entity.Property(u => u.FechaCreacion).HasColumnName("fecha_creacion");

                entity.HasOne(u => u.Rol)
                      .WithMany()
                      .HasForeignKey(u => u.IdRol)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            //---------------- Parcelas ----------------
            modelBuilder.Entity<Parcela>(entity =>
            {
                entity.ToTable("parcelas");
                entity.HasKey(p => p.IdParcela);
                entity.Property(p => p.IdParcela).HasColumnName("id_parcela");
                entity.Property(p => p.Nombre).HasColumnName("nombre");
                entity.Property(p => p.Ubicacion).HasColumnName("ubicacion");
                entity.Property(p => p.DimensionesM2).HasColumnName("dimensiones_m2").HasColumnType("decimal(12,2)");
                entity.Property(p => p.RegistradoPor).HasColumnName("registrado_por");
                entity.Property(p => p.FechaRegistro).HasColumnName("fecha_registro");

                entity.HasOne(p => p.RegistradoPorUsuario)
                      .WithMany()
                      .HasForeignKey(p => p.RegistradoPor)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            //---------------- Cultivos ----------------
            modelBuilder.Entity<Cultivo>(entity =>
            {
                entity.ToTable("cultivos");
                entity.HasKey(c => c.IdCultivo);
                entity.Property(c => c.IdCultivo).HasColumnName("id_cultivo");
                entity.Property(c => c.IdParcela).HasColumnName("id_parcela");
                entity.Property(c => c.TipoCultivo).HasColumnName("tipo_cultivo");
                entity.Property(c => c.FechaSiembra).HasColumnName("fecha_siembra");
                entity.Property(c => c.FechaEstimadaCosecha).HasColumnName("fecha_estimada_cosecha");
                entity.Property(c => c.Estado).HasColumnName("estado");
                entity.Property(c => c.RegistradoPor).HasColumnName("registrado_por");
                entity.Property(c => c.FechaRegistro).HasColumnName("fecha_registro");

                entity.HasOne(c => c.Parcela)
                      .WithMany(p => p.Cultivos)
                      .HasForeignKey(c => c.IdParcela)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.RegistradoPorUsuario)
                      .WithMany()
                      .HasForeignKey(c => c.RegistradoPor)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
