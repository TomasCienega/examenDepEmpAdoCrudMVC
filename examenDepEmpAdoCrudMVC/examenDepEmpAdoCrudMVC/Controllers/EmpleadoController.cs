using examenDepEmpAdoCrudMVC.Data;
using examenDepEmpAdoCrudMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace examenDepEmpAdoCrudMVC.Controllers
{
    public class EmpleadoController : Controller
    {
        private readonly DepartamentoData _departamentoData;
        private readonly EmpleadoData _empleadoData;

        public EmpleadoController(DepartamentoData departamentoData, EmpleadoData empleadoData)
        {
            _departamentoData = departamentoData;
            _empleadoData = empleadoData;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // 1. Cargamos los departamentos para el filtro (ESTO FALTABA)
                ViewBag.Departamentos = await _departamentoData.ListarDepartamentos();
                var _listaGenDeps = await _empleadoData.ListarEmpleados();
                if (_listaGenDeps != null)
                {
                    return View(_listaGenDeps);
                }
                else
                {
                    // Si es nula, mandamos una lista vacía para que la vista no truene
                    return View(new List<Empleado>());
                }
            }
            catch (Exception ex)
            {
                // También aquí, por seguridad, si falla la DB de empleados, 
                // intenta que al menos el filtro no truene la vista
                ViewBag.Departamentos = new List<Departamento>();
                // Si algo falla (ej. se cayó la base de datos), puedes redirigir a una página de error
                // O simplemente mandar la lista vacía para que no se detenga la app
                return View(new List<Empleado>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> FiltrarPorDepartamento(int idD)
        {
            try
            {
                // 1. Si el id es 0 (o "Todos"), mandamos al Index normal
                if (idD == 0) return RedirectToAction("Index");

                // 2. Usamos el método que ya tienes en tu capa de datos
                var _filtrarPorDep = await _empleadoData.ListarEmpPorIdDep(idD);

                ViewBag.IdSelected = idD; // Guarda el ID para que el select lo marque como seleccionado
                // Al resetear, limpiamos el nombre para que se vea que es independiente
                ViewBag.NombreActual = "";
                // 3. Volvemos a llenar los departamentos para el combo de filtro en la vista
                ViewBag.Departamentos = await _departamentoData.ListarDepartamentos();

                // 4. Mandamos la lista filtrada a la misma vista "Index"
                return View("Index", _filtrarPorDep);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<IActionResult> FiltrarPorNombre(string nombre)
        {
            try
            {
                // 1. Validamos que el nombre no venga vacío
                if (string.IsNullOrWhiteSpace(nombre)) return RedirectToAction("Index");

                // 2. Obtenemos la lista (aunque esté vacía)
                var _listaNombres = await _empleadoData.ListarEmpPorNom(nombre);

                // 3. Llenamos los ViewBags necesarios para la vista Index
                ViewBag.IdSelected = 0;
                ViewBag.NombreActual = nombre;
                ViewBag.Departamentos = await _departamentoData.ListarDepartamentos();

                // 4. Retornamos la vista Index con los resultados
                return View("Index", _listaNombres);
            }
            catch (Exception ex)
            {
                // Si algo falla, regresamos al Index original para que la app no truene
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<IActionResult> CambiarEstado(int idE)
        {
            try
            {
                // Llamamos a tu nuevo método EstadoEmpl
                bool exito = await _empleadoData.EstadoEmpl(idE);

                // 2. Regresamos al Index para ver el cambio (el empleado bajará al final de la lista)
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            try
            {
                var _listaDeps = await _departamentoData.ListarDepartamentos();
                ViewBag.Departamentos = _listaDeps ?? new List<Departamento>();
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Departamentos = new List<Departamento>();
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Empleado empleado)
        {

            if (!ModelState.IsValid)
            {
                // Si no son válidos, ni siquiera molestamos a la base de datos.
                ViewBag.Departamentos = await _departamentoData.ListarDepartamentos();
                return View(empleado);
            }
            try
            {
                var _guardarEmpls =await  _empleadoData.GuardarEmpleado(empleado);
                if (_guardarEmpls)
                {
                    TempData["AlertMessage"] = "Empleado creado correctamente";
                    TempData["AlertIcon"] = "success";
                    return RedirectToAction("Index");
                }
                else
                {
                    // ¡OJO! Si falló, hay que rellenar los departamentos
                    // antes de regresar a la vista del formulario
                    ViewBag.Departamentos = await _departamentoData.ListarDepartamentos();
                    return View(empleado);
                }
            }
            catch (Exception ex)
            {
                // Si hay error, también rellenamos departamentos y devolvemos la vista
                ViewBag.Departamentos = await _departamentoData.ListarDepartamentos();
                return View(empleado);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Editar(int idE)
        {
            try
            {
                var _empleadoEncontrado = await _empleadoData.ListarEmpPorIdEmpl(idE);
                if (_empleadoEncontrado == null)
                {
                    return NotFound();
                }
                else
                {
                    var _listaDeps = await _departamentoData.ListarDepartamentos();
                    ViewBag.Departamentos = _listaDeps ?? new List<Departamento>();
                    return View(_empleadoEncontrado);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Departamentos = new List<Departamento>();
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Empleado empleado)
        {
            if (!ModelState.IsValid)
            {
                // Si no son válidos, ni siquiera molestamos a la base de datos.
                ViewBag.Departamentos = await _departamentoData.ListarDepartamentos();
                return View(empleado);
            }
            try
            {
                var _editarEmpleado = await _empleadoData.EditarEmpleado(empleado);
                if (_editarEmpleado)
                {
                    TempData["AlertMessage"] = "Datos actualizados con éxito";
                    TempData["AlertIcon"] = "info";
                    return RedirectToAction("Index");
                }
                else
                {
                    // ¡OJO! Si falló, hay que rellenar los departamentos
                    // antes de regresar a la vista del formulario
                    ViewBag.Departamentos = await _departamentoData.ListarDepartamentos();
                    return View(empleado);
                }
            }
            catch (Exception ex)
            {
                // Si hay error, también rellenamos departamentos y devolvemos la vista
                ViewBag.Departamentos = await _departamentoData.ListarDepartamentos();
                return View(empleado);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int idE)
        {
            try
            {
                var _emplEncParaElim = await _empleadoData.ListarEmpPorIdEmpl(idE);
                if (_emplEncParaElim == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(_emplEncParaElim);
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(Empleado empleado)
        {
            try
            {
                // 1. Aquí es donde ocurre la magia: llamamos al método que BORRA
                var _empElim = await _empleadoData.EliminarEmplFisico(empleado.IdEmpleado);

                // 2. Sin importar si fue true o false (aunque podrías validar), 
                // lo mandamos de regreso a la lista principal.
                TempData["AlertMessage"] = "Registro eliminado físicamente";
                TempData["AlertIcon"] = "warning";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Si algo explota, mejor regresar a la seguridad del Index
                return RedirectToAction("Index");
            }
        }
    }
}
