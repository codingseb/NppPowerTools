using Newtonsoft.Json;
using NppPowerTools.PluginInfrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace NppPowerTools.Utils
{
    public class Config : INotifyPropertyChanged
    {
        [JsonIgnore]
        public List<ResultOut> ResultOuts { get; set; } = new List<ResultOut>()
        {
            new ResultOut()
            {
                Name = "Replace expression by result",
                SetResult = result => BNpp.SelectedText = result
            },
            new ResultOut()
            {
                Name = "Add result on a new line after expression",
                SetResult = result =>
                {
                    BNpp.Scintilla.SetSelection(BNpp.Scintilla.GetSelectionEnd(), BNpp.Scintilla.GetSelectionEnd());
                    BNpp.Scintilla.NewLine();
                    BNpp.Scintilla.InsertTextAndMoveCursor(result);
                }
            },
            new ResultOut()
            {
                Name = "Result in a new tab",
                SetResult = result =>
                {
                    BNpp.NotepadPP.FileNew();
                    BNpp.Text = result;
                    BNpp.Scintilla.DocumentEnd();
                }
            },
            new ResultOut()
            {
                Name = "No output for result",
                SetResult = result =>
                {
                    BNpp.Scintilla.SetSelection(BNpp.Scintilla.GetSelectionEnd(), BNpp.Scintilla.GetSelectionEnd());
                }
            },
        };

        public int CurrentResultOutIndex { get; set; } = 0;

        public bool OptionForceIntegerNumbersEvaluationsAsDoubleByDefault { get; set; } = false;

        public bool CaseSensitive { get; set; } = true;



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

        private static string fileName = Path.Combine(new NotepadPPGateway().PluginsConfigDirectory, "NppPowerTools", "Config.json");

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
        }

        #endregion
    }
}
