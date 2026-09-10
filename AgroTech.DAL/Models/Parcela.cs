using System;
using System.Collections.Generic;

namespace AgroTech.DAL.Models
{
    public class Parcela
    {
        public int IdParcela { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Ubicacion { get; set; } = string.Empty;
        public decimal DimensionesM2 { get; set; }
        public int RegistradoPor { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public Usuario? RegistradoPorUsuario { get; set; }
        public List<Cultivo> Cultivos { get; set; } = new();
    }
}
