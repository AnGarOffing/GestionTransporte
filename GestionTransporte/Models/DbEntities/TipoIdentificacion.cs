using System;
using System.Collections.Generic;

namespace GestionTransporte.Models.DbEntities;

public partial class TipoIdentificacion
{
    public int IdTipoIdentificacion { get; set; }

    public string NombreTipoIdentificacion { get; set; } = null!;
} 