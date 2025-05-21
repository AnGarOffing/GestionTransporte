using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionTransporte.Models.ViewModels
{
    public class TipoIdentificacionViewModel
    {
        public int IdTipoIdentificacion { get; set; }

        [DisplayName("Tipo de Identificacion")]
        [Required(ErrorMessage = "El nombre del tipo de identificación es requerido")]
        public string NombreTipoIdentificacion { get; set; } = string.Empty;
    }
}
