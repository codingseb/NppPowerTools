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
        public static List<Type> DBTypesList { get; private set; }

        public static void InitDBTypesList()
        {
            DBTypesList =  Directory.GetFiles(connectionsDirectory, "*.dll")
                .Select(fileName => Assembly.LoadFrom(fileName))
                .Aggregate(new List<Type>(),
                    (types, assembly) => types.Concat(assembly.GetTypes().Where(t => typeof(IDbConnection).IsAssignableFrom(t))).ToList(),
                    result => result);
            DBTypesList.Add(typeof(System.Data.SQLite.SQLiteConnection));
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
