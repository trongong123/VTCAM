using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EQX.Core.Motion;
using EQX.UI.Controls;
using FrontCameraAssembleEquipment.Defines.Process;
using FrontCameraAssembleEquipment.Defines.Recipes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace FrontCameraAssembleEquipment.Defines
{
    public class PositionGroup : ObservableObject
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public AxisUnit AxisUnit { get; set; }
        public EMoveType MoveType { get; set; }
        public ObservableCollection<Position> Positions { get; set; } 
        public bool IsOnGroupPosition
        {
            get
            {
                int count = Positions.Count(p => p.Axis.IsOnPosition(p.PositionValue));
                bool bRet = (count == Positions.Count);
                return bRet;
            }
        }
    }

    public class Position : ObservableObject
    {
        #region Properties
        public IMotion Axis { get; set; }
        public string RecipePropertyPath { get; set; }
        public string AxisName => Axis.Name.Split('_').Last();
        public double CurrentPosition => Axis.Status.ActualPosition;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }
        public double PositionValue
        {
            get => GetValueFromRecipe();
            set
            {
                SetValueToRecipe(value);
                OnPropertyChanged();
            }
        }
        public void SetCurrentPos()
        {
            if (IsSelected)
            {
                PositionValue = CurrentPosition;
            }
        }
        #endregion

        public Position(RecipeSelector recipeSelector)
        {
            _recipeSelector = recipeSelector;

            System.Timers.Timer statusUpdateTimer = new System.Timers.Timer(500);
            statusUpdateTimer.Elapsed += StatusUpdateTimer_Elapsed;
            statusUpdateTimer.AutoReset = true;
            statusUpdateTimer.Enabled = true;
        }

        private void StatusUpdateTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            OnPropertyChanged(nameof(CurrentPosition));
        }

        #region Privates
        private readonly RecipeSelector _recipeSelector;
        #endregion
        #region Private Methods
        private double GetValueFromRecipe()
        {
            if (string.IsNullOrEmpty(RecipePropertyPath))
                return 0;

            try
            {
                var parts = RecipePropertyPath.Split('.');
                object current = _recipeSelector.CurrentRecipe;

                foreach (var part in parts)
                {
                    var prop = current.GetType().GetProperty(part);
                    if (prop == null)
                        throw new Exception($"Property '{part}' not found in '{current.GetType().Name}'");

                    current = prop.GetValue(current);

                    if (current == null)
                        throw new Exception($"Property '{part}' returned null");
                }

                return Convert.ToDouble(current);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting recipe value: {ex.Message}");
                return 0;
            }
        }

        private void SetValueToRecipe(double value)
        {
            if (string.IsNullOrEmpty(RecipePropertyPath))
                return;

            try
            {
                var parts = RecipePropertyPath.Split('.');
                object current = _recipeSelector.CurrentRecipe;

                // Navigate to the parent object
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    var prop = current.GetType().GetProperty(parts[i]);
                    if (prop == null)
                        throw new Exception($"Property '{parts[i]}' not found");

                    current = prop.GetValue(current);

                    if (current == null)
                        throw new Exception($"Property '{parts[i]}' returned null");
                }

                // Set the final property
                var finalProp = current.GetType().GetProperty(parts[parts.Length - 1]);
                if (finalProp == null)
                    throw new Exception($"Property '{parts[parts.Length - 1]}' not found");

                finalProp.SetValue(current, value);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting recipe value: {ex.Message}");
                throw;
            }
        }
        #endregion

        private bool _isSelected = true;
    }
}
