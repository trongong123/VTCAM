using CommunityToolkit.Mvvm.Input;
using EQX.Core.Common;
using EQX.Core.Motion;
using EQX.Core.Recipe;
using EQX.UI.Controls;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace EQX.UI.MVVM
{
    public class TeachPositionUnitViewModelBase : ViewModelBase
    {
        private bool isMoveJogMode = true;
        private double incStepSelected;
        private int jogSpeedIndexSelected = 0;

        public TeachPositionUnitViewModelBase()
        {
            IncStepSelected = IncStepList.First();
        }

        public EventHandler SelectedUnitChanged;
        public virtual ObservableCollection<IRecipe> Recipes { get; }
        public virtual ObservableCollection<IMotion> Motions { get; }

        public ICommand MovePositionTeachingCommand
        {
            get
            {
                return new RelayCommand<object>((o) =>
                {
                    if (o is SinglePositionTeaching spt == false) return;
                    string MoveTo = (string)Application.Current.Resources["str_MoveTo"];

                    string motionName = spt.SinglePosition.Motion;
                    string moveToDescription = spt.SingleRecipeDescription.Description;

                    if (MessageBoxEx.ShowDialog($"{MoveTo} {moveToDescription} ?") == true)
                    {
                        Motions.FirstOrDefault(m => m.Name.Contains(motionName))!.MoveAbs(spt.Value);
                    }
                });
            }
        }

        public ICommand GetCurrentPositionCommand
        {
            get
            {
                return new RelayCommand<object>((o) =>
                {
                    if (o is SinglePositionTeaching spt == false) return;

                    string motionName = spt.SinglePosition.Motion;

                    spt.Value = Motions.FirstOrDefault(m => m.Name.Contains(motionName))!.Status.ActualPosition;
                });
            }
        }

        public IMotion XAxis => Motions.FirstOrDefault(m => m.Name.Contains("XAxis"));
        public IMotion YAxis => Motions.FirstOrDefault(m => m.Name.Contains("YAxis"));
        public IMotion ZAxis => Motions.FirstOrDefault(m => m.Name.Contains("ZAxis"));


        public bool IsMoveJogMode
        {
            get { return isMoveJogMode; }
            set
            {
                isMoveJogMode = value;
                OnPropertyChanged();
            }
        }

        public List<double> IncStepList => new List<double>
        {
            0.001,
            0.010,
            0.1,
            1,
            10,
        };

        public double IncStepSelected
        {
            get { return incStepSelected; }
            set
            {
                incStepSelected = value;
                OnPropertyChanged();
            }
        }

        public List<string> JogSpeedList => new List<string>
        {
            "Super Slow",
            "Slow",
            "Medium",
            "Fast"
        };

        public int JogSpeedIndexSelected
        {
            get
            {
                return jogSpeedIndexSelected;
            }
            set
            {
                jogSpeedIndexSelected = value;
                OnPropertyChanged();
            }
        }
    }
}
