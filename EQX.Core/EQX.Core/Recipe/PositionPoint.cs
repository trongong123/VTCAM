using CommunityToolkit.Mvvm.ComponentModel;
using EQX.Core.Motion;
using System.ComponentModel;
using System.Reflection;

namespace EQX.Core.Recipe
{
    public static class PositionPointHelpers
    {
        public static bool IsOnPosition(this IEnumerable<PositionPoint> positionPoints)
        {
            bool isOnPosition = true;
            foreach (var position in positionPoints)
            {
                isOnPosition &= position.Motion.IsOnPosition(position.TargetPos);
            }

            return isOnPosition;
        }
    }

    /// <summary>
    /// Đại diện cho một điểm tọa độ của một trục trong một vị trí logic đa điểm.
    /// Lớp này đóng vai trò trung gian, liên kết giá trị trên UI với một thuộc tính cụ thể trong lớp Recipe.
    /// </summary>
    public class PositionPoint : ObservableObject
    {
        private readonly (RecipeBase recipe, PropertyInfo prop) _fixedSource;
        private readonly (RecipeBase recipe, PropertyInfo prop)? _offsetSource;
        private readonly (RecipeBase recipe, PropertyInfo prop)? _modelSource;
        private double _offsetFallback;
        private readonly (object source, PropertyInfo prop)? _visionSource;

        public uint MovingOrder { get; }

        public IMotion Motion { get; }

        public double FixedPos
        {
            get => GetSourceValue(_fixedSource);
            set
            {
                if (TrySetSourceValue(_fixedSource, value))
                {
                    OnPropertyChanged(nameof(FixedPos));
                    OnPropertyChanged(nameof(TargetPos));
                }
            }
        }

        public double OffsetPos
        {
            get => _offsetSource.HasValue ? GetSourceValue(_offsetSource.Value) : _offsetFallback;
            set
            {
                if (_offsetSource.HasValue)
                {
                    if (TrySetSourceValue(_offsetSource.Value, value))
                    {
                        OnPropertyChanged(nameof(OffsetPos));
                        OnPropertyChanged(nameof(TargetPos));
                    }

                    return;
                }

                if (Math.Abs(_offsetFallback - value) > 1e-6)
                {
                    _offsetFallback = value;
                    OnPropertyChanged(nameof(OffsetPos));
                    OnPropertyChanged(nameof(TargetPos));
                }
            }
        }

        public double ModelPos => _modelSource.HasValue ? GetSourceValue(_modelSource.Value) : 0.0;

        public double VisionPos => _visionSource.HasValue ? GetSourceValue(_visionSource.Value) : 0.0;

        public double TargetPos => FixedPos + OffsetPos + ModelPos + VisionPos;

        public PositionPoint(uint movingOrder, RecipeBase recipeObject, PropertyInfo propertyInfo, IMotion motion)
            : this(movingOrder, (recipeObject, propertyInfo), null, null, null, motion)
        {
        }

        public PositionPoint(
            uint movingOrder,
            (RecipeBase recipe, PropertyInfo prop) fixedSource,
            (RecipeBase recipe, PropertyInfo prop)? offsetSource,
            (RecipeBase recipe, PropertyInfo prop)? modelSource,
            (object source, PropertyInfo prop)? visionSource,
            IMotion motion)
        {
            MovingOrder = movingOrder;
            _fixedSource = fixedSource;
            _offsetSource = offsetSource;
            _modelSource = modelSource;
            _visionSource = visionSource;
            Motion = motion;

            SubscribeSource((_fixedSource.recipe, _fixedSource.prop), nameof(FixedPos));
            if (_offsetSource.HasValue)
            {
                SubscribeSource((_offsetSource.Value.recipe, _offsetSource.Value.prop), nameof(OffsetPos));
            }

            if (_modelSource.HasValue)
            {
                SubscribeSource((_modelSource.Value.recipe, _modelSource.Value.prop), nameof(ModelPos));
            }

            if (_visionSource.HasValue)
            {
                SubscribeSource(_visionSource.Value, nameof(VisionPos));
            }
        }

        private static double GetSourceValue((RecipeBase recipe, PropertyInfo prop) source)
        {
            object? rawValue = source.prop.GetValue(source.recipe);
            return rawValue == null ? 0.0 : Convert.ToDouble(rawValue);
        }

        private static bool TrySetSourceValue((RecipeBase recipe, PropertyInfo prop) source, double value)
        {
            var currentValue = GetSourceValue(source);
            if (Math.Abs(currentValue - value) <= 1e-6)
            {
                return false;
            }

            object convertedValue = Convert.ChangeType(value, source.prop.PropertyType);
            source.prop.SetValue(source.recipe, convertedValue);
            return true;
        }

        private static double GetSourceValue((object source, PropertyInfo prop) source)
        {
            object? rawValue = source.prop.GetValue(source.source);
            return rawValue == null ? 0.0 : Convert.ToDouble(rawValue);
        }

        private void SubscribeSource((object source, PropertyInfo prop) source, string sourcePropertyName)
        {
            if (source.source is not INotifyPropertyChanged notifier)
            {
                return;
            }

            notifier.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == source.prop.Name || string.IsNullOrEmpty(e.PropertyName))
                {
                    OnPropertyChanged(sourcePropertyName);
                    OnPropertyChanged(nameof(TargetPos));
                }
            };
        }
    }
}
