using System;

namespace AgroTech.DAL.Models
{
    public class Cultivo
    {
        public int IdCultivo { get; set; }
        public int IdParcela { get; set; }
        public string TipoCultivo { get; set; } = string.Empty;
        public DateTime FechaSiembra { get; set; }
        public DateTime FechaEstimadaCosecha { get; set; }
        public string Estado { get; set; } = "Activo";
        public int RegistradoPor { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        
        public Parcela? Parcela { get; set; }
    }
}
