using System;
using System.Collections.Generic;
using System.Linq;
using AgroTech.DAL;
using AgroTech.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace AgroTech.BLL
{
    public class ParcelaService
    {
        public Parcela RegistrarNuevaParcela(string nombre, string ubicacion, decimal dimensionesM2, int idUsuarioRegistrador)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(ubicacion))
                throw new ArgumentException("El nombre y la ubicación de la parcela son obligatorios.");

            var ubicacionNormalizada = ubicacion.ToLowerInvariant();
            if (!ubicacionNormalizada.Contains("maule") && !ubicacionNormalizada.Contains("ñuble") && !ubicacionNormalizada.Contains("nuble"))
                throw new ArgumentException("La ubicación geográfica está restringida a Maule o Ñuble.");

            if (dimensionesM2 < 1.00m)
                throw new ArgumentException("Las dimensiones de la parcela deben ser mayores o iguales a 1.00 m².");

            var parcela = new Parcela
            {
                Nombre = nombre.Trim(),
                Ubicacion = ubicacion.Trim(),
                DimensionesM2 = dimensionesM2,
                RegistradoPor = idUsuarioRegistrador,
                FechaRegistro = DateTime.Now
            };

            using var db = new AgroTechDbContext();
            db.Parcelas.Add(parcela);
            db.SaveChanges();
            return parcela;
        }

        public List<Parcela> ObtenerTodas()
        {
            using var db = new AgroTechDbContext();
            return db.Parcelas.AsNoTracking().OrderByDescending(p => p.FechaRegistro).ToList();
        }
    }
}
