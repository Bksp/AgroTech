using System;
using System.Linq;
using AgroTech.BLL;
using AgroTech.DAL;
using AgroTech.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AgroTech.Tests
{
    public class CultivoServiceTests
    {
        private AgroTechDbContext GetInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AgroTechDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            
            var context = new AgroTechDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public void RegistrarNuevoCultivo_Valido_GuardaCultivoEnBaseDeDatos()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetInMemoryContext(dbName))
            {
                var usuario = new Usuario { NombreCompleto = "Test", CorreoElectronico = "test@agrotech.cl", PasswordHash = "hash" };
                context.Usuarios.Add(usuario);
                context.SaveChanges();

                var parcela = new Parcela { Nombre = "Parcela Test", Ubicacion = "Maule", DimensionesM2 = 1000, RegistradoPor = usuario.IdUsuario };
                context.Parcelas.Add(parcela);
                context.SaveChanges();
            }

            // Act
            using (var context = GetInMemoryContext(dbName))
            {
                var service = new CultivoService(() => GetInMemoryContext(dbName));
                
                var fechaSiembra = DateTime.Now;
                var fechaCosecha = DateTime.Now.AddMonths(3);
                
                var cultivo = service.RegistrarNuevoCultivo(1, "Tomate", fechaSiembra, fechaCosecha, 1);
                
                Assert.NotNull(cultivo);
                Assert.Equal(1, cultivo.IdCultivo);
                Assert.Equal("Tomate", cultivo.TipoCultivo);
                Assert.Equal("Activo", cultivo.Estado);
            }

            // Assert
            using (var context = GetInMemoryContext(dbName))
            {
                var cultivosDb = context.Cultivos.ToList();
                Assert.Single(cultivosDb);
                Assert.Equal("Tomate", cultivosDb.First().TipoCultivo);
            }
        }

        [Fact]
        public void ActualizarCultivo_Valido_CambiaDatos()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetInMemoryContext(dbName))
            {
                var usuario = new Usuario { NombreCompleto = "Test", CorreoElectronico = "test@agrotech.cl", PasswordHash = "hash" };
                context.Usuarios.Add(usuario);
                var parcela = new Parcela { Nombre = "Parcela Test", Ubicacion = "Maule", DimensionesM2 = 1000, RegistradoPor = 1 };
                context.Parcelas.Add(parcela);
                var cultivo = new Cultivo { IdParcela = 1, TipoCultivo = "Tomate", Estado = "Activo", RegistradoPor = 1, FechaSiembra = DateTime.Now, FechaEstimadaCosecha = DateTime.Now.AddMonths(2) };
                context.Cultivos.Add(cultivo);
                context.SaveChanges();
            }

            // Act
            var service = new CultivoService(() => GetInMemoryContext(dbName));
            var nuevaSiembra = DateTime.Now.AddDays(1);
            var nuevaCosecha = DateTime.Now.AddMonths(4);
            service.ActualizarCultivo(1, nuevaSiembra, nuevaCosecha, "Cosechado");

            // Assert
            using (var context = GetInMemoryContext(dbName))
            {
                var cultivoActualizado = context.Cultivos.Find(1);
                Assert.NotNull(cultivoActualizado);
                Assert.Equal("Cosechado", cultivoActualizado.Estado);
                Assert.Equal(nuevaSiembra.Date, cultivoActualizado.FechaSiembra);
                Assert.Equal(nuevaCosecha.Date, cultivoActualizado.FechaEstimadaCosecha);
            }
        }

        [Fact]
        public void EliminarCultivo_Existente_BorraRegistro()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetInMemoryContext(dbName))
            {
                var cultivo = new Cultivo { IdParcela = 1, TipoCultivo = "Tomate", Estado = "Activo", RegistradoPor = 1, FechaSiembra = DateTime.Now, FechaEstimadaCosecha = DateTime.Now.AddMonths(2) };
                context.Cultivos.Add(cultivo);
                context.SaveChanges();
            }

            // Act
            var service = new CultivoService(() => GetInMemoryContext(dbName));
            service.EliminarCultivo(1);

            // Assert
            using (var context = GetInMemoryContext(dbName))
            {
                var count = context.Cultivos.Count();
                Assert.Equal(0, count);
            }
        }
    }
}
