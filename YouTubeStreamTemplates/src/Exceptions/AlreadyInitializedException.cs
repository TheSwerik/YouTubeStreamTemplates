using System.Reflection;

namespace YouTubeStreamTemplates.Exceptions
{
    public class AlreadyInitializedException : YouTubeStreamTemplateException
    {
        public AlreadyInitializedException() : base("This Service is already initialized.") { }
        public AlreadyInitializedException(MemberInfo service) : base($"{service.Name} is already initialized.") { }
    }
}