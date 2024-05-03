using Microsoft.Data.SqlClient;
using MySqlConnector;
using Npgsql;
using System.Data;
using System.Data.OracleClient;
using System.Windows;

namespace DBRefs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [System.Obsolete]
        public MainWindow()
        {
            InitializeComponent();

            IDbConnection mysqldb = new MySqlConnection();
            IDbConnection oracledb = new OracleConnection();
            IDbConnection postgresdb = new NpgsqlConnection();
            IDbConnection sqlserver = new SqlConnection();
        }
    }
}
