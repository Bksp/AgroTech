using System;
using System.Collections.Generic;
using System.Linq;
using AgroTech.DAL;
using AgroTech.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace AgroTech.BLL
{
    public class CultivoService
    {
        private static readonly string[] EstadosValidos = { "Activo", "Cosechado", "Perdido por Plaga" };
        private readonly Func<AgroTechDbContext> _contextFactory;

        public CultivoService() : this(() => new AgroTechDbContext()) { }

        public CultivoService(Func<AgroTechDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public Cultivo RegistrarNuevoCultivo(int idParcela, string tipoCultivo, DateTime fechaSiembra, DateTime fechaEstimadaCosecha, int idUsuarioRegistrador)
        {
            if (idParcela <= 0)
                throw new ArgumentException("Debe seleccionar una parcela válida para asociar el cultivo.");

            if (string.IsNullOrWhiteSpace(tipoCultivo))
                throw new ArgumentException("Debe indicar el tipo de cultivo.");

            if (fechaEstimadaCosecha <= fechaSiembra)
                throw new ArgumentException("La fecha estimada de cosecha debe ser posterior a la fecha de siembra.");

            using var db = _contextFactory();

            var existeParcela = db.Parcelas.Any(p => p.IdParcela == idParcela);
            if (!existeParcela)
                throw new ArgumentException("La parcela seleccionada no existe.");

            var cultivo = new Cultivo
            {
                IdParcela = idParcela,
                TipoCultivo = tipoCultivo.Trim(),
                FechaSiembra = fechaSiembra.Date,
                FechaEstimadaCosecha = fechaEstimadaCosecha.Date,
                Estado = "Activo",
                RegistradoPor = idUsuarioRegistrador,
                FechaRegistro = DateTime.Now
            };

            db.Cultivos.Add(cultivo);
            db.SaveChanges();
            return cultivo;
        }

        public void ActualizarEstado(int idCultivo, string nuevoEstado)
        {
            if (!EstadosValidos.Contains(nuevoEstado))
                throw new ArgumentException("Estado de cultivo inválido.");

            using var db = _contextFactory();
            var cultivo = db.Cultivos.Find(idCultivo)
                ?? throw new ArgumentException("El cultivo indicado no existe.");

            cultivo.Estado = nuevoEstado;
            db.SaveChanges();
        }

        public List<Cultivo> ObtenerTodos()
        {
            using var db = _contextFactory();
            return db.Cultivos
                .Include(c => c.Parcela)
                .AsNoTracking()
                .OrderByDescending(c => c.FechaRegistro)
                .ToList();
        }

        public void ActualizarCultivo(int idCultivo, DateTime fechaSiembra, DateTime fechaEstimadaCosecha, string estado)
        {
            if (fechaEstimadaCosecha <= fechaSiembra)
                throw new ArgumentException("La fecha estimada de cosecha debe ser posterior a la fecha de siembra.");

            if (!EstadosValidos.Contains(estado))
                throw new ArgumentException("Estado de cultivo inválido.");

            using var db = _contextFactory();
            var cultivo = db.Cultivos.Find(idCultivo)
                ?? throw new ArgumentException("El cultivo indicado no existe.");

            cultivo.FechaSiembra = fechaSiembra.Date;
            cultivo.FechaEstimadaCosecha = fechaEstimadaCosecha.Date;
            cultivo.Estado = estado;
            
            db.SaveChanges();
        }

        public void EliminarCultivo(int idCultivo)
        {
            using var db = _contextFactory();
            var cultivo = db.Cultivos.Find(idCultivo)
                ?? throw new ArgumentException("El cultivo indicado no existe.");

            db.Cultivos.Remove(cultivo);
            db.SaveChanges();
        }
    }
}
