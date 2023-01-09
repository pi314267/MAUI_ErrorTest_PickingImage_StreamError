using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ErrorTest.Helper
{
    public static class EmbeddedMedia
    {
        public const string cancelPng = "cross_circle.png";
    }

    public static class EmbeddedResourceProvider
    {
        static readonly Assembly assembly = typeof(EmbeddedResourceProvider).GetTypeInfo().Assembly;
        static readonly string[] resources = assembly.GetManifestResourceNames();

        public static Stream Load(string name)
        {
            name = GetFullName(name);

            if (string.IsNullOrWhiteSpace(name))
                return null;

            return assembly.GetManifestResourceStream(name);
        }

        public static ImageSource GetImageSource(string name)
        {
            name = GetFullName(name);

            if (string.IsNullOrWhiteSpace(name))
                return null;

            return ImageSource.FromResource(name, assembly);
        }

        static string GetFullName(string name)
            => resources.FirstOrDefault(n => n.EndsWith($".EmbeddedResources.{name}"));
    }
}
