using AgroTech.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace AgroTech.DAL
{
    public class AgroTechDbContext : DbContext
    {
        public DbSet<Rol> Roles { get; set; } = null!;
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Parcela> Parcelas { get; set; } = null!;
        public DbSet<Cultivo> Cultivos { get; set; } = null!;

        public AgroTechDbContext()
        {
            // Create database if not exists (offline MVP)
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Usando SQLite local como dicta la rúbrica para el MVP / Modo Offline
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "agrotech.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuracion de Tablas y PK
            modelBuilder.Entity<Rol>().HasKey(r => r.IdRol);
            modelBuilder.Entity<Usuario>().HasKey(u => u.IdUsuario);
            modelBuilder.Entity<Parcela>().HasKey(p => p.IdParcela);
            modelBuilder.Entity<Cultivo>().HasKey(c => c.IdCultivo);

            // Seed Data (Simulando la BBDD Real)
            modelBuilder.Entity<Rol>().HasData(
                new Rol { IdRol = 1, NombreRol = "Administrador", Descripcion = "Control total" },
                new Rol { IdRol = 2, NombreRol = "Supervisor", Descripcion = "Gestiona parcelas" },
                new Rol { IdRol = 3, NombreRol = "Trabajador", Descripcion = "Acceso operativo" }
            );

            modelBuilder.Entity<Usuario>().HasData(
                new Usuario { IdUsuario = 1, IdRol = 1, NombreCompleto = "Admin Sistema", CorreoElectronico = "admin@agrotech.cl", PasswordHash = "password", FechaCreacion = DateTime.Now },
                new Usuario { IdUsuario = 2, IdRol = 2, NombreCompleto = "Carlos Producción", CorreoElectronico = "carlos.supervisor@agrotech.cl", PasswordHash = "password", FechaCreacion = DateTime.Now },
                new Usuario { IdUsuario = 3, IdRol = 3, NombreCompleto = "Juan Terreno", CorreoElectronico = "juan.trabajador@agrotech.cl", PasswordHash = "password", FechaCreacion = DateTime.Now }
            );

            modelBuilder.Entity<Parcela>().HasData(
                new Parcela { IdParcela = 1, Nombre = "Sector Norte A1", Ubicacion = "Maule", DimensionesM2 = 255000, RegistradoPor = 1, FechaRegistro = DateTime.Now },
                new Parcela { IdParcela = 2, Nombre = "Sector Sur B2", Ubicacion = "Ñuble", DimensionesM2 = 400000, RegistradoPor = 2, FechaRegistro = DateTime.Now }
            );
        }
    }
}
