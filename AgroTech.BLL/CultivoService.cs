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

        public Cultivo RegistrarNuevoCultivo(int idParcela, string tipoCultivo, DateTime fechaSiembra, DateTime fechaEstimadaCosecha, int idUsuarioRegistrador)
        {
            if (idParcela <= 0)
                throw new ArgumentException("Debe seleccionar una parcela válida para asociar el cultivo.");

            if (string.IsNullOrWhiteSpace(tipoCultivo))
                throw new ArgumentException("Debe indicar el tipo de cultivo.");

            if (fechaEstimadaCosecha <= fechaSiembra)
                throw new ArgumentException("La fecha estimada de cosecha debe ser posterior a la fecha de siembra.");

            using var db = new AgroTechDbContext();

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

            using var db = new AgroTechDbContext();
            var cultivo = db.Cultivos.Find(idCultivo)
                ?? throw new ArgumentException("El cultivo indicado no existe.");

            cultivo.Estado = nuevoEstado;
            db.SaveChanges();
        }

        public List<Cultivo> ObtenerTodos()
        {
            using var db = new AgroTechDbContext();
            return db.Cultivos
                .Include(c => c.Parcela)
                .AsNoTracking()
                .OrderByDescending(c => c.FechaRegistro)
                .ToList();
        }
    }
}
