using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;

namespace NppPowerTools.Utils
{
    public class DBConfig : INotifyPropertyChanged
    {
        private static readonly string connectionsDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "DBConnectors");

        public string Id { get; set; }
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public string DBTypeName { get; set; }
        public string InitCommands { get; set; }

        public IDbConnection GetConnection() => (IDbConnection)Activator.CreateInstance(DBTypesList.Find(t => t.Name.Equals(DBTypeName)), new object[] { ConnectionString });

        [JsonIgnore]
        public static List<Type> DBTypesList { get; private set; } = new List<Type>();

        public static void InitDBTypesList()
        {
            try
            {
                DBTypesList = Directory.GetFiles(connectionsDirectory, "*.dll")
                    .Select(Assembly.UnsafeLoadFrom)
                    .Aggregate(new List<Type>(),
                        (types, assembly) =>
                        {
                            assembly.GetReferencedAssemblies().ToList().ForEach(assemblyName =>
                            {
                                string fileName = Path.Combine(connectionsDirectory, "Dependencies", assemblyName.Name.Split(',')[0].Trim() + ".dll");

                                if(File.Exists(fileName))
                                    Assembly.LoadFile(fileName);
                            });

                            return types.Concat(assembly.ExportedTypes.Where(t => typeof(IDbConnection).IsAssignableFrom(t))).ToList();
                        },
                        result => result);
            }
            catch(ReflectionTypeLoadException exception)
            {
                MessageBox.Show("Impossible to load DBConnector " + exception.Message, "NppPowerTools", MessageBoxButton.OK, MessageBoxImage.Information);

                BNpp.NotepadPP.FileNew();
                BNpp.Text = exception.Message + "\r\n" + exception.StackTrace + "\r\n\r\n" + string.Join("\r\n", exception.LoaderExceptions.Select(e => e.Message));
            }
            catch(Exception exception)
            {
                MessageBox.Show("Impossible to load DBConnector " + exception.Message, "NppPowerTools", MessageBoxButton.OK, MessageBoxImage.Information);

                BNpp.NotepadPP.FileNew();
                BNpp.Text = exception.Message + "\r\n" + exception.StackTrace;
            }

            //DBTypesList.Add(typeof(System.Data.SQLite.SQLiteConnection));
        }

        #region NotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Config.Instance?.Save();
        }

        #endregion

    }
}
