using Microsoft.AspNetCore.Mvc;

using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

using Newtonsoft.Json;

using ASPProject.Models;
namespace ASPProject.Controllers
{
    public class MantenedorController : Controller
    {
        //firebase con variable
        IFirebaseClient client;

        public MantenedorController()
        {
            //firebase configraction
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "eHrMQbvc3fDOf9pc84v1St8X0P3Jv2qIiCRzPWaT",
                BasePath = "https://aspproject-ee37b-default-rtdb.firebaseio.com/"
            };

            client = new FirebaseClient(config);
        }

        //data display method
        public IActionResult Inicio()
        {
            Dictionary<string, Contacto> lista = new Dictionary<string, Contacto>();
            FirebaseResponse response = client.Get("contactos");

            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                lista = JsonConvert.DeserializeObject<Dictionary<string, Contacto>>(response.Body);
            }

            List<Contacto> listaContacto = new List<Contacto>();

            foreach(KeyValuePair<string,Contacto> elemento in lista)
            {
                listaContacto.Add(new Contacto()
                {
                    IdContacto = elemento.Key,
                    Nombre = elemento.Value.Nombre,
                    Correo = elemento.Value.Correo,
                    Telefono = elemento.Value.Telefono
                });
            }


            return View(listaContacto);
        }

        //data insert method
        public IActionResult Crear()
        {
            return View();
        }

        //data edit method
        public IActionResult Editar(string idcontacto)
        {
            FirebaseResponse response = client.Get("contactos/" + idcontacto);

            Contacto ocontacto = response.ResultAs<Contacto>();
            ocontacto.IdContacto = idcontacto;
            return View(ocontacto);
        }

        //data delete method
        public IActionResult Eliminar(string idcontacto)
        {
            FirebaseResponse response = client.Delete("contactos/" + idcontacto);
            return RedirectToAction("Inicio", "Mantenedor");
        }

        //data insert in firebase method
        [HttpPost]
        public IActionResult Crear(Contacto oContacto)
        {
            string IdGenerado = Guid.NewGuid().ToString("N");

            SetResponse response = client.Set("contactos/" + IdGenerado, oContacto);

            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("Inicio", "Mantenedor");
            }
            else
            {
                return View();
            }   
        }

        //data update in firebase
        [HttpPost]
        public IActionResult Editar(Contacto oContacto)
        {
            string idcontacto = oContacto.IdContacto;
            oContacto.IdContacto = null;

            FirebaseResponse response = client.Update("contactos/" + idcontacto, oContacto);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("Inicio", "Mantenedor");
            }
            else
            {
                return View();
            }
        }
    }
}
