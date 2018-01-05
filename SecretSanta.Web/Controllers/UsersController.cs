using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using SecretSanta.Web.Models.Users;
using SecretSanta.Factories;
using System.Net;
using System.Web.Http.ModelBinding;

namespace SecretSanta.Web.Controllers
{
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        private ApplicationUserManager applicationUserManager;
        private readonly IUserFactory userFactory;

        public UsersController(IUserFactory userFactory)
        {
            this.userFactory = userFactory;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                if(this.applicationUserManager == null)
                {
                    return Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                }

                return this.applicationUserManager;
            }
            private set
            {
                this.applicationUserManager = value;
            }
        }

        //POST ~/users
        [HttpPost]
        [AllowAnonymous]
        [Route("")]
        public async Task<IHttpActionResult> RegisterUser(RegisterUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var user = this.userFactory.Create(model.Email, model.UserName, 
                model.DisplayName, model.FirstName, model.LastName);
            var identityResult = await this.UserManager.CreateAsync(user, model.Password);

            if(identityResult == null)
            {
                return InternalServerError();
            }

            if(!identityResult.Succeeded)
            {
                if (identityResult.Errors != null)
                {
                    foreach (string error in identityResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }

                if (ModelState.IsValid)
                {
                    return BadRequest();
                }

                return this.Content<ModelStateDictionary>(HttpStatusCode.Conflict, ModelState);
            }

            var userModel = new DisplayUserViewModel(user.Email, user.FirstName, 
                user.LastName, user.DisplayName, user.UserName);

            return this.Created("/api/users", userModel);
        }
    }
}
