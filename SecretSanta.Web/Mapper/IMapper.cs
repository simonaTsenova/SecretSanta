using SecretSanta.Models;
using SecretSanta.Web.Models.Groups;
using SecretSanta.Web.Models.Invitations;
using SecretSanta.Web.Models.Links;
using SecretSanta.Web.Models.Users;
using System.Collections.Generic;

namespace SecretSanta.Web.Mapper
{
    public interface IMapper
    {
        DisplayUserViewModel MapUser(User user);

        IEnumerable<DisplayUserViewModel> MapUsers(IEnumerable<User> users);

        DisplayGroupViewModel MapGroup(Group group);

        ShortGroupViewModel MapShortGroup(Group group);

        IEnumerable<ShortGroupViewModel> MapShortGroups(IEnumerable<Group> groups);

        LinkViewModel MapLink(Link link);

        InvitationViewModel MapInvitation(Invitation invitation);

        IEnumerable<InvitationViewModel> MapInvitations(IEnumerable<Invitation> invitations);
    }
}