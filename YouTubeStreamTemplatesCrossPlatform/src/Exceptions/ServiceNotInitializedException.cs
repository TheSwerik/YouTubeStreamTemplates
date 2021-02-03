using System;
using System.Reflection;

namespace YouTubeStreamTemplatesCrossPlatform.Exceptions
{
    public class ServiceNotInitializedException : Exception
    {
        public ServiceNotInitializedException(MemberInfo service) : base($"{service.Name} is not initialized.") { }
    }
}