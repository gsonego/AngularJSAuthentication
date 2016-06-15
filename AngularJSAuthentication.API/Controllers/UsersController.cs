using System.Collections.Generic;
using System.Web.Http;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AngularJSAuthentication.API.Controllers
{
    public class UsersController : ApiController
    {
        private readonly AuthRepository _repo;

        public UsersController()
        {
            _repo = new AuthRepository();
        }

        [Authorize(Users = "Admin")]
        public IHttpActionResult Get()
        {
            List<IdentityUser> users = _repo.GetAllUsers();

            return Ok(users);
        }
    }
}
