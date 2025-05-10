using Microsoft.AspNetCore.Mvc;

namespace GestionTransporte.Controllers
{
    public class TipoIdentificacionController : Controller
    {
        

        public TipoIdentificacionController()
        {
            
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
