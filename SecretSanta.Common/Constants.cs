namespace SecretSanta.Common
{
    public static class Constants
    {
        public const string INVALID_MODEL = "Invalid or nonexisting model.";

        public const string USERNAME_NOT_FOUND = "User with provided username was not found.";
        public const string USER_ID_NOT_FOUND = "User with provided id was not found.";
        public const string REQUIRED_USERNAME = "Username must be provided.";
        public const string USER_ALREADY_MEMBER_OF_GROUP = "User is already a member of this group.";
        public const string USER_HAS_NO_INVITATIONS_FOR_GROUP = "User has no invitations for this group.";

        public const string GROUP_NAME_NOT_FOUND = "Group with provided name was not found.";
        public const string GROUP_NAME_ALREADY_EXISTS = "Group with provided name already exists.";
        public const string GROUP_NAME_REQUIRED = "Group name must be provided";
        public const string GROUP_ACCESS_FORBIDDEN = "Users are only allowed to see their groups.";
        public const string REMOVE_PARTICIPANT_FORBIDDEN = "Only administrators are allowed to remove participants from group.";
        public const string PARTICIPANT_NOT_FOUND = "Participant does not exist in this group.";
        public const string SEND_INVITATION_FORBIDDEN = "Only admins can send invitations for groups.";

        public const string INVITATION_NOT_FOUND = "Invitation was not found.";
        public const string INVITATION_ID_NOT_FOUND = "Invitation with provided id was not found.";
        public const string INVITATION_ALREADY_EXISTS = "This user already has an invitation for this group.";
        public const string INVITATION_ACCESS_FORBIDDEN = "Users are only allowed to access their invitations.";
        public const string INVITATION_ACCEPT_SUCCESS = "Invitation was successfully accepted.";

        public const string LINK_NOT_FOUND = "Link was not found for provided group and user.";
        public const string LINKS_ACCESS_FORBIDDEN = "Users are only allowed to see their links.";
        public const string LINKING_PROCESS_NOT_STARTED = "Linking process has not started yet.";
        public const string LINKING_PROCESS_ALREADY_DONE = "Linking process has already been done for this group.";
        public const string LINKING_PROCESS_COMPLETE_SUCCESS = "Linking process has been successfully completed for this group";
        public const string LINKING_PROCESS_MEMBERS_MINIMUM_COUNT = "Linking process can start only in groups with more than 1 members.";
        public const string LINKING_PROCESS_MEMBERS_ODD_COUNT = "Linking process cannot start in a group with odd number of members.";
        public const string LINKING_PROCESS_START_FORBIDDEN = "You must be an admin to start linking.";
    }
}
