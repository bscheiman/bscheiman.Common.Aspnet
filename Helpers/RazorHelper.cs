#region
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using bscheiman.Common.Extensions;
using RazorEngine;
using RazorEngine.Compilation;
using RazorEngine.Compilation.ReferenceResolver;
using RazorEngine.Configuration;
using RazorEngine.Templating;

#endregion

namespace bscheiman.Common.Aspnet.Helpers {
    internal class ReferenceResolver : IReferenceResolver {
        public IEnumerable<CompilerReference> GetReferences(TypeContext context, IEnumerable<CompilerReference> includeAssemblies) {
            var loadedAssemblies = (new UseCurrentAssembliesReferenceResolver()).GetReferences(context, includeAssemblies);

            foreach (var reference in loadedAssemblies)
                yield return reference;

            yield return CompilerReference.From(typeof (ReferenceResolver).Assembly);
            yield return CompilerReference.From(Assembly.GetExecutingAssembly());
        }
    }

    public static class RazorHelper {
        static RazorHelper() {
            //Engine.Razor = RazorEngineService.Create(new TemplateServiceConfiguration {
            //    Language = Language.CSharp,
            //    //ReferenceResolver = new ReferenceResolver()
            //});
        }

        public static string Transform(string fullPath, object model) {
            string template = File.ReadAllText(fullPath);

            model = model ?? new object();

            return Engine.Razor.RunCompile(template, template.ToMD5(), model.GetType(), model);
        }
    }
}