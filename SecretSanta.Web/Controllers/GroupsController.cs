using SecretSanta.Services.Contracts;
using SecretSanta.Web.Models.Groups;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using System.Net;
using System.Web;
using SecretSanta.Web.Infrastructure;
using System.Linq;
using SecretSanta.Web.Infrastructure.Factories;

namespace SecretSanta.Web.Controllers
{
    [RoutePrefix("api/groups")]
    public class GroupsController : ApiController
    {
        private readonly IGroupService groupService;
        private readonly ISessionService sessionService;
        private readonly IDisplayGroupViewModelFactory viewModelsFactory;
        private readonly IDisplayUserViewModelFactory userViewModelFactory;

        public GroupsController(IGroupService groupService, ISessionService sessionService, 
            IDisplayGroupViewModelFactory viewModelsFactory, IDisplayUserViewModelFactory userViewModelFactory)
        {
            this.groupService = groupService;
            this.sessionService = sessionService;
            this.viewModelsFactory = viewModelsFactory;
            this.userViewModelFactory = userViewModelFactory;
        }

        // POST ~/groups 
        [HttpPost]
        [Route("")]
        public IHttpActionResult CreateGroup(CreateGroupViewModel model)
        {
            if(model == null)
            {
                return this.BadRequest("Group name must be provided");
            }

            if(!ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var currentUser = this.sessionService.GetCurrentUser();
            if(currentUser == null)
            {
                return this.Unauthorized();
            }

            var group = this.groupService.CreateGroup(model.Name, currentUser.Id);
            if(group == null)
            {
                return this.Content(HttpStatusCode.Conflict, "Group with that name already exists.");
            }

            var members = group.Users.Select(u => this.userViewModelFactory
                .CreateDisplayUserViewModel(u.Email, u.FirstName, u.LastName, u.DisplayName, u.UserName))
                .ToList();
            var groupModel = this.viewModelsFactory.CreateDisplayGroupViewModel(group.Name, group.Admin.UserName, members);

            return this.Content(HttpStatusCode.Created, groupModel);
        }
    }
}
