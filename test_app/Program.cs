using System;
using AgroTech.BLL;

try {
    var service = new CultivoService();
    service.ActualizarCultivo(3, new DateTime(2025, 6, 10), new DateTime(2026, 9, 23), "Activo");
    Console.WriteLine("Exito");
} catch(Exception ex) {
    Console.WriteLine("Error: " + ex.Message);
    if(ex.InnerException != null) Console.WriteLine("Inner: " + ex.InnerException.Message);
}
