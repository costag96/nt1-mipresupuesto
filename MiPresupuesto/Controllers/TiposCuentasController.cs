using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MiPresupuesto.Models;
using MiPresupuesto.Services;

namespace MiPresupuesto.Controllers
{
    public class TiposCuentasController: Controller
    {
        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IUsuarioService usuarioService;

        public TiposCuentasController(IRepositorioTiposCuentas repositorioTiposCuentas,
            IUsuarioService usuarioService)
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.usuarioService = usuarioService;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = usuarioService.obtenerId();
            var tiposCuentas = await repositorioTiposCuentas.ObtenerTiposCuenta(usuarioId);
            return View(tiposCuentas);

        }
        public IActionResult Crear()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoCuenta);
            }

            tipoCuenta.UsuarioId = usuarioService.obtenerId();

            var existeTipoCuenta = await repositorioTiposCuentas.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioId);

            if (existeTipoCuenta)
            {
                ModelState.AddModelError(nameof(TipoCuenta.Nombre), $"El nombre {tipoCuenta.Nombre} ya existe.");

                return View(tipoCuenta);
            }

            await repositorioTiposCuentas.Crear(tipoCuenta);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = usuarioService.obtenerId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerTipoCuentaPorId(id, usuarioId);

            if(tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(TipoCuenta tipoCuenta)
        {
            var usuarioId = usuarioService.obtenerId();
            var existeTipoCuenta = await repositorioTiposCuentas.ObtenerTipoCuentaPorId(tipoCuenta.Id, usuarioId);

            if(existeTipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioTiposCuentas.ActualizarTiposCuenta(tipoCuenta);

            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = usuarioService.obtenerId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerTipoCuentaPorId(id, usuarioId);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarTipoCuenta(int id)
        {
            var usuarioId = usuarioService.obtenerId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerTipoCuentaPorId(id, usuarioId);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioTiposCuentas.BorrarTipoCUenta(id);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ExisteTipoCuenta(string nombre)
        {
            var usuarioId = usuarioService.obtenerId();
            var existeTipoCuenta = await repositorioTiposCuentas.Existe(nombre, usuarioId);

            if (existeTipoCuenta)
            {
                return Json($"El nombre {nombre} ya existe");
            }
            return Json(true);
        }
    }
}
