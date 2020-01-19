using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DependencyLoader
{
    public class Loader : EXILED.Plugin
    {
        public override string getName => "DependencyLoader";

        public List<Assembly> localLoaded = new List<Assembly>();

        public static List<Assembly> Loaded = null;

        public override void OnDisable()
        {
            Info("Unloading");
            Loaded = localLoaded;
            localLoaded.Clear();
            localLoaded = null;
        }

        public override void OnEnable()
        {
            Info("Loading dependencies...");
            string pl = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Plugins");
            string folder = Path.Combine(pl, "dependencies");
            Info("Searching Directory '" + folder + "'");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            string[] depends = Directory.GetFiles(folder);
            foreach (string dll in depends)
            {
                if (!dll.EndsWith(".dll"))
                    continue;
                if (IsLoaded(dll))
                    return;
                Assembly a = Assembly.LoadFrom(dll);
                localLoaded.Add(a);
                Info("Loaded dependency " + a.FullName);
            }
            Info("Complete!");
        }

        public bool IsLoaded(string a)
        {
            foreach(Assembly asm in localLoaded)
            {
                if (asm.Location == a)
                    return true;
            }
            return false;
        }

        public override void OnReload()
        {
            localLoaded = Loaded;
        }
    }
}
