namespace CleanArchitecture.Shared.Constants.User
{
    public static class UserConstants
    {
        public const string DefaultPassword = "123Pa$$word!";
        public const int MaxFailedAccessAttempts = 5;
        public const int DefaultLockoutTimeSpan = 5;
        public const int RequiredPasswordLength = 6;
    }
}