using SecretSanta.Models;
using SecretSanta.Web.Models.Users;
using System.Collections.Generic;
using System.Linq;
using SecretSanta.Web.Models.Groups;
using SecretSanta.Web.Models.Links;
using SecretSanta.Web.Models.Invitations;

namespace SecretSanta.Web.Mapper
{
    public class ViewModelsMapper : IMapper
    {
        public DisplayUserViewModel MapUser(User user)
        {
            var userViewModel = new DisplayUserViewModel()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DisplayName = user.DisplayName,
                UserName = user.UserName
            };

            return userViewModel;
        }

        public IEnumerable<DisplayUserViewModel> MapUsers(IEnumerable<User> users)
        {
            var usersViewModel = users.Select(user => this.MapUser(user));

            return usersViewModel;
        }

        public DisplayGroupViewModel MapGroup(Group group)
        {
            var groupViewModel = new DisplayGroupViewModel()
            {
                GroupName = group.Name,
                Admin = group.Admin.UserName,
                Members = group.Users.Select(user => this.MapUser(user)).ToList()
            };

            return groupViewModel;
        }

        public ShortGroupViewModel MapShortGroup(Group group)
        {
            var groupViewModel = new ShortGroupViewModel(group.Name, group.Admin.UserName);

            return groupViewModel;
        }

        public IEnumerable<ShortGroupViewModel> MapShortGroups(IEnumerable<Group> groups)
        {
            var groupsViewModel = groups.Select(group => this.MapShortGroup(group));

            return groupsViewModel;
        }

        public LinkViewModel MapLink(Link link)
        {
            var linkViewModel = new LinkViewModel(link.Receiver.UserName);

            return linkViewModel;
        }

        public InvitationViewModel MapInvitation(Invitation invitation)
        {
            var invitationViewModel = new InvitationViewModel()
            {
                Id = invitation.Id,
                SentDate = invitation.SentDate,
                GroupName = invitation.Group.Name,
                Receiver = invitation.Receiver.UserName
            };

            return invitationViewModel;
        }

        public IEnumerable<InvitationViewModel> MapInvitations(IEnumerable<Invitation> invitations)
        {
            var invitationsViewModel = invitations.Select(invitation => this.MapInvitation(invitation));

            return invitationsViewModel;
        }
    }
}