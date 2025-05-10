using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionTransporte.Models.ViewModels
{
    public class TipoIdentificacionViewModel
    {
        [DisplayName("Tipo de Identificacion")]
         public string NombreTipoIdentificacion { get; set; }
    }
}
