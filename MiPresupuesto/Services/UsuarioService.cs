using System.Security.Claims;

namespace MiPresupuesto.Services
{
    public interface IUsuarioService
    {
        int obtenerId();
    }
    public class UsuarioService: IUsuarioService
    {
        private readonly HttpContext httpContext;

        public UsuarioService(IHttpContextAccessor httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext;
        }
        public int obtenerId()
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var idClaim = httpContext.User
                        .Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
                var id = int.Parse(idClaim.Value);
                return id;
            }
            else
            {
                throw new ApplicationException("El usuario no está autenticado");
            }
        }
    }
}
