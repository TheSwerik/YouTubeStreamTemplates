using System.Reflection;

namespace YouTubeStreamTemplates.Exceptions
{
    public class AlreadyInitializingException : YouTubeStreamTemplateException
    {
        public AlreadyInitializingException() : base("This Service is already initializing.") { }
        public AlreadyInitializingException(MemberInfo service) : base($"{service.Name} is already initializing.") { }
    }
}