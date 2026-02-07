using System.ComponentModel.DataAnnotations;

namespace examenDepEmpAdoCrudMVC.Models
{
    public class Empleado
    {
        public int IdEmpleado { get; set; }
        [Required(ErrorMessage = "El nombre del empleado es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string NombreEmpleado { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public Departamento ReferenciaDepartamento { get; set; } = new();
    }
}
