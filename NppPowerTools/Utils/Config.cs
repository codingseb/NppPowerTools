using Newtonsoft.Json;
using NppPowerTools.PluginInfrastructure;
using System;
using System.Collections.Generic;
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
            new ResultOut
            {
                Name = "No output for result",
                SetResult = _ => BNpp.Scintilla.SetSelection(BNpp.Scintilla.GetSelectionEnd(), BNpp.Scintilla.GetSelectionEnd())
            },
            new ResultOut
            {
                Name = "Replace expression by result",
                SetResult = result => BNpp.SelectedText = result.ToStringOutput(),
            },
            new ResultOut
            {
                Name = "Add result on a new line after expression",
                SetResult = result =>
                {
                    BNpp.Scintilla.SetSelection(BNpp.Scintilla.GetSelectionEnd(), BNpp.Scintilla.GetSelectionEnd());
                    BNpp.Scintilla.NewLine();
                    BNpp.Scintilla.InsertTextAndMoveCursor(result.ToStringOutput());
                }
            },
            new ResultOut
            {
                Name = "Result in a new tab",
                SetResult = result =>
                {
                    BNpp.NotepadPP.FileNew();
                    BNpp.Text = result.ToStringOutput();
                    BNpp.Scintilla.DocumentEnd();
                }
            },
            new ResultOut
            {
                Name = "Result in a MessageBox",
                SetResult = result =>
                {
                    BNpp.Scintilla.SetSelection(BNpp.Scintilla.GetSelectionEnd(), BNpp.Scintilla.GetSelectionEnd());
                    MessageBox.Show(result.ToStringOutput(), "Result");
                }
            },
            new ResultOut
            {
                Name = "Result in specific panel/windows",
                SetResult = result =>
                {
                    BNpp.Scintilla.SetSelection(BNpp.Scintilla.GetSelectionEnd(), BNpp.Scintilla.GetSelectionEnd());
                    EvaluationsResultPanelViewModel.Instance.ShowResult(result);
                }
            },
        };

        public int CurrentResultOutIndex { get; set; }

        public string TextWhenResultIsNull { get; set; } = string.Empty;

        public bool OptionForceIntegerNumbersEvaluationsAsDoubleByDefault { get; set; }

        public bool CaseSensitive { get; set; } = true;

        public bool KeepVariablesBetweenEvaluations { get; set; }

        public bool ReverseSortingInResultsWindow { get; set; }

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

        public bool ShowExceptionInMessageBox { get; set; } = true;

        public bool ShowExceptionInOutput { get; set; } = true;

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

        #region Json singleton

        private static readonly string fileName = Path.Combine(new NotepadPPGateway().PluginsConfigDirectory, "NppPowerTools", "Config.json");

        private static Config instance = null;

        public static Config Instance
        {
            get
            {
                if (instance == null)
                {
                    if (File.Exists(fileName))
                    {
                        try
                        {
                            instance = JsonConvert.DeserializeObject<Config>(File.ReadAllText(fileName));
                        }
                        catch { }
                    }

                    if (instance == null)
                    {
                        instance = new Config();
                        instance.Save();
                    }
                }

                return instance;
            }
        }

        public void Save()
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

        private Config()
        { }

        #endregion

        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            this.Save();
        }

        #endregion
    }
}
