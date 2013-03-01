using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roslyn.Scripting.CSharp;
using Scriptcs.Contracts;
using Roslyn.Scripting;

namespace Scriptcs
{
    public class ScriptExecutor : IScriptExecutor
    {
        private readonly IFileSystem _fileSystem;

        [ImportingConstructor]
        public ScriptExecutor(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void Execute(string script, IEnumerable<string> paths, IEnumerable<IScriptcsRecipe> recipes)
        {
            var engine = new ScriptEngine();
            var session = Session.Create();
            session.AddReference("System");
            var bin = _fileSystem.CurrentDirectory + @"\bin";
            session.SetReferenceSearchPaths(
                session.GetReferenceSearchPaths().Append(bin));

            if (!_fileSystem.DirectoryExists(bin))
            {
                _fileSystem.CreateDirectory(bin);
                foreach (var file in paths)
                {
                    var destFile = bin + @"\" + Path.GetFileName(file);
                    _fileSystem.Copy(file, destFile);
                    session.AddReference(destFile);
                }
            }

            var csx = _fileSystem.ReadFile(_fileSystem.CurrentDirectory + @"\" + script);
            engine.Execute(csx, session);
        }

    }
}
