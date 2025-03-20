using Newtonsoft.Json;
using NppPowerTools.PluginInfrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace NppPowerTools.Utils
{
    public class Config : INotifyPropertyChanged
    {
        public int CommandPanelWidth { get; set; } = 500;
        public int CommandPanelHeight { get; set; } = 500;

        [JsonIgnore]
        public List<ResultOut> ResultOuts { get; set; } = new List<ResultOut>()
        {
            new() {
                Name = "No output for result",
                SetResult = _ => Npp.Scintilla.SetSelection(Npp.Scintilla.GetSelectionEnd(), Npp.Scintilla.GetSelectionEnd())
            },
            new() {
                Name = "Replace expression by result",
                SetResult = result => Npp.SelectedText = result.ToStringOutput(),
            },
            new() {
                Name = "Add result on a new line after expression",
                SetResult = result =>
                {
                    Npp.Scintilla.SetSelection(Npp.Scintilla.GetSelectionEnd(), Npp.Scintilla.GetSelectionEnd());
                    Npp.Scintilla.NewLine();
                    Npp.Scintilla.InsertTextAndMoveCursor(result.ToStringOutput());
                }
            },
            new() {
                Name = "Result in a new tab",
                SetResult = result =>
                {
                    Npp.NotepadPP.FileNew();
                    Npp.Text = result.ToStringOutput();
                    Npp.Scintilla.DocumentEnd();
                }
            },
            new() {
                Name = "Result in a MessageBox",
                SetResult = result =>
                {
                    Npp.Scintilla.SetSelection(Npp.Scintilla.GetSelectionEnd(), Npp.Scintilla.GetSelectionEnd());
                    MessageBox.Show(result.ToStringOutput(), "Result");
                }
            },
            new() {
                Name = "Result in specific panel/windows",
                SetResult = result =>
                {
                    Npp.Scintilla.SetSelection(Npp.Scintilla.GetSelectionEnd(), Npp.Scintilla.GetSelectionEnd());
                    EvaluationsResultPanelViewModel.Instance.ShowResult(result);
                }
            },
        };

        public List<string> LastScripts { get; set; } = new List<string>();

        public int NbrOfLastScriptToKeep { get; set; } = 100;

        public bool CommandSmartSearch { get; set; } = true;

        public int CurrentResultOutIndex { get; set; }

        public string TextWhenResultIsNull { get; set; } = string.Empty;

        public bool OptionForceIntegerNumbersEvaluationsAsDoubleByDefault { get; set; }

        public bool CaseSensitive { get; set; } = true;

        public bool KeepVariablesBetweenEvaluations { get; set; }

        public bool ReverseSortingInResultsWindow { get; set; }

        public bool ShowTooltipInResultWindow { get; set; } = true;

        public bool UseProxy { get; set; }

        public bool UseDefaultProxy { get; set; }

        public string ProxyAddress { get; set; } = string.Empty;

        public int? ProxyPort { get; set; }

        public bool ProxyBypassOnLocal { get; set; }

        public string ProxyBypassList { get; set; } = string.Empty;

        public bool UseDefaultCredentials { get; set; } = true;

        public string ProxyUserName { get; set; } = string.Empty;

        public string ProxyPassword { get; set; } = string.Empty;

        public int QrCodeDefaultSize { get; set; } = 5;

        public Color QRCodeDarkColor { get; set; } = Color.Black;

        public Color QRCodeLightColor { get; set; } = Color.White;

        public string QRCodeTestText { get; set; } = "Test";

        public bool ShowExceptionInMessageBox { get; set; } = true;

        public bool ShowExceptionInOutput { get; set; } = true;

        public string ExcelDefaultFileName{ get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "NppPowerTools.xlsx");

        public string ExcelDefaultSheetName { get; set; } = "Sheet{0}";

        public string ExcelDateTimeDefaultFormat { get; set; } = "dd.mm.yyyy HH:mm:ss";

        public string PDFDefaultFileName { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "NppPowerTools.pdf");

        public int DBGridHeightInResults { get; set; } = 300;

        public bool DBAutoLimitRequests { get; set; } = true;

        public int DBAutoLimitRequestsValue { get; set; } = 100;

        public ObservableCollection<DBConfig> DBConfigs { get; set; } = new ObservableCollection<DBConfig>();

        [JsonIgnore]
        public ResultOut CurrentResultOut => ResultOuts[CurrentResultOutIndex];

        [JsonIgnore]
        public int CompleteCurrentResultOutIndexSelection
        {
            get
            {
                return CurrentResultOutIndex;
            }
            set
            {
                Main.SetEvaluationOutput(value);
            }
        }

        private static readonly string fileName = Path.Combine(new NotepadPPGateway().GetPluginConfigPath(), "NppPowerTools", "Config.json");

        private static Config instance;
        private static bool inCreation;
        private static bool canSave;

        public static Config Instance
        {
            get
            {
                if (instance == null && !inCreation)
                {
                    inCreation = true;
                    if (File.Exists(fileName))
                    {
                        try
                        {
                            instance = JsonConvert.DeserializeObject<Config>(File.ReadAllText(fileName));
                        }
                        catch { }
                    }

                    canSave = true;

                    if (instance == null)
                    {
                        instance = new Config();
                        instance.Save();
                    }
                    inCreation = false;
                }

                return instance;
            }
        }

        public void Save()
        {
            if (canSave)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                try
                {
                    File.WriteAllText(fileName, JsonConvert.SerializeObject(this, Formatting.Indented));
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.ToString(), "Config save error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private Config()
        {
            DBConfigs.CollectionChanged += DBConfigs_CollectionChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Save();
        }

        private void DBConfigs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Save();
        }
    }
}