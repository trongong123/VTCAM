using EQX.Core.Motion;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

namespace EQX.Core.Recipe
{
    /// <summary>
    /// Lớp quản lý, đóng vai trò là một factory và container cho các vị trí đa điểm.
    /// Lớp này cung cấp các phương thức để tạo các điểm vị trí (`PositionPoint`) từ các thuộc tính trong Recipe,
    /// cho phép người dùng tự xây dựng và tổ chức các nhóm vị trí (`MultiPointPosition`) một cách linh hoạt.
    /// </summary>
    public class RecipePositionManagerBase<TRecipeList> where TRecipeList : class
    {
        private readonly TRecipeList _recipeList;
        // Caches for efficient lookup
        private readonly IReadOnlyDictionary<string, (RecipeBase recipe, PropertyInfo prop)> _recipeProperties;
        private readonly IReadOnlyDictionary<string, IMotion> _motions;

        /// <summary>
        /// Tập hợp các nhóm vị trí đa điểm. Người dùng có thể tự xây dựng và gán cho thuộc tính này.
        /// </summary>
        public ObservableCollection<MultiPointPosition> GroupedPositions { get; set; }

        public RecipePositionManagerBase(TRecipeList recipeList, IEnumerable<IMotion> motions)
        {
            if (recipeList == null) throw new ArgumentNullException(nameof(recipeList));
            _recipeList = recipeList;
            _motions = motions?.ToDictionary(m => m.Name, StringComparer.OrdinalIgnoreCase) ?? throw new ArgumentNullException(nameof(motions));

            GroupedPositions = new ObservableCollection<MultiPointPosition>();
            _recipeProperties = ScanAndCacheRecipeProperties(recipeList);
        }

        /// <summary>
        /// Tạo một đối tượng PositionPoint từ một thuộc tính recipe cụ thể.
        /// </summary>
        /// <param name="movingOrder">Thứ tự ưu tiên khi di chuyển, số bé sẽ được ưu tiên di chuyển trước</param>
        /// <param name="uniqueRecipePropertyName">Tên định danh duy nhất của thuộc tính, theo định dạng "TenDoiTuongRecipe.TenThuocTinh".
        /// Ví dụ: "InjectRecipe.XAxisReadyPos" hoặc "SPDHead1_Recipe.ZAxisSafetyPos".</param>
        /// <param name="motion">Đối tượng IMotion tương ứng với trục của vị trí này.</param>
        /// <returns>Một đối tượng PositionPoint đã được liên kết.</returns>
        protected PositionPoint CreatePositionPoint(uint movingOrder, string uniqueRecipePropertyName, IMotion motion)
        {
            return CreatePositionPoint(movingOrder, uniqueRecipePropertyName, motion, null, null);
        }

        protected PositionPoint CreatePositionPoint(
            uint movingOrder,
            string fixedRecipePropertyName,
            IMotion motion,
            string? offsetRecipePropertyName,
            string? modelRecipePropertyName)
        {
            if (motion == null)
            {
                throw new ArgumentNullException(nameof(motion));
            }

            var fixedSource = ResolveTeachingRecipeProperty(fixedRecipePropertyName, nameof(fixedRecipePropertyName));
            var offsetSource = ResolveRecipePropertyOptional(offsetRecipePropertyName);
            var modelSource = ResolveRecipePropertyOptional(modelRecipePropertyName);

            return new PositionPoint(movingOrder, fixedSource, offsetSource, modelSource, null, motion);
        }

        /// <summary>
        /// Tạo một đối tượng PositionPoint từ một thuộc tính recipe cụ thể bằng cách sử dụng Expression để đảm bảo an toàn kiểu.
        /// </summary>
        /// <typeparam name="TValue">Kiểu dữ liệu của thuộc tính (thường là double).</typeparam>
        /// <param name="movingOrder">Thứ tự ưu tiên khi di chuyển, số bé sẽ được ưu tiên di chuyển trước</param>
        /// <param name="propertyExpression">Biểu thức lambda trỏ đến thuộc tính mong muốn. Ví dụ: `rl => rl.InjectRecipe.XAxisReadyPos`.</param>
        /// <param name="motion">Đối tượng IMotion tương ứng với trục của vị trí này.</param>
        /// <returns>Một đối tượng PositionPoint đã được liên kết.</returns>
        public PositionPoint CreatePositionPoint<TValue>(uint movingOrder, Expression<Func<TRecipeList, TValue>> propertyExpression, IMotion motion)
        {
            string uniqueKey = BuildUniqueRecipePropertyName(propertyExpression);
            return CreatePositionPoint(movingOrder, uniqueKey, motion);
        }

        public PositionPoint CreatePositionPoint(
            uint movingOrder,
            Expression<Func<TRecipeList, double>> fixedPosExpression,
            Expression<Func<TRecipeList, double>> offsetPosExpression,
            IMotion motion)
        {
            string fixedKey = BuildUniqueRecipePropertyName(fixedPosExpression);
            string offsetKey = BuildUniqueRecipePropertyName(offsetPosExpression);

            var fixedSource = ResolveTeachingRecipeProperty(fixedKey, nameof(fixedPosExpression));
            var offsetSource = ResolveRecipePropertyFromUniqueName(offsetKey, nameof(offsetPosExpression));

            if (motion == null)
            {
                throw new ArgumentNullException(nameof(motion));
            }

            return new PositionPoint(movingOrder, fixedSource, offsetSource, null, null, motion);
        }

        public PositionPoint CreatePositionPoint(
            uint movingOrder,
            Expression<Func<TRecipeList, double>> fixedPosExpression,
            Expression<Func<TRecipeList, double>> offsetPosExpression,
            Expression<Func<TRecipeList, double>> modelPosExpression,
            IMotion motion)
        {
            string fixedKey = BuildUniqueRecipePropertyName(fixedPosExpression);
            string offsetKey = BuildUniqueRecipePropertyName(offsetPosExpression);

            var modelSource = ResolveRecipePropertyFromExpression(modelPosExpression, nameof(modelPosExpression));
            var fixedSource = ResolveTeachingRecipeProperty(fixedKey, nameof(fixedPosExpression));
            var offsetSource = ResolveRecipePropertyFromUniqueName(offsetKey, nameof(offsetPosExpression));

            if (motion == null)
            {
                throw new ArgumentNullException(nameof(motion));
            }

            return new PositionPoint(movingOrder, fixedSource, offsetSource, modelSource, null, motion);
        }

        public PositionPoint CreatePositionPoint<TVisionSource>(
            uint movingOrder,
            Expression<Func<TRecipeList, double>> fixedPosExpression,
            Expression<Func<TRecipeList, double>> offsetPosExpression,
            Expression<Func<TRecipeList, double>> modelPosExpression,
            TVisionSource visionSource,
            Expression<Func<TVisionSource, double>> visionPosExpression,
            IMotion motion)
            where TVisionSource : class
        {
            string fixedKey = BuildUniqueRecipePropertyName(fixedPosExpression);
            string offsetKey = BuildUniqueRecipePropertyName(offsetPosExpression);

            var modelSource = ResolveRecipePropertyFromExpression(modelPosExpression, nameof(modelPosExpression));
            var fixedSource = ResolveTeachingRecipeProperty(fixedKey, nameof(fixedPosExpression));
            var offsetSource = ResolveRecipePropertyFromUniqueName(offsetKey, nameof(offsetPosExpression));
            var visionValueSource = ResolveObjectPropertyFromExpression(visionSource, visionPosExpression, nameof(visionPosExpression));

            if (motion == null)
            {
                throw new ArgumentNullException(nameof(motion));
            }

            return new PositionPoint(movingOrder, fixedSource, offsetSource, modelSource, visionValueSource, motion);
        }

        private IReadOnlyDictionary<string, (RecipeBase recipe, PropertyInfo prop)> ScanAndCacheRecipeProperties(TRecipeList recipeList)
        {
            var properties = new Dictionary<string, (RecipeBase recipe, PropertyInfo prop)>(StringComparer.OrdinalIgnoreCase);

            var recipeListProperties = recipeList.GetType().GetProperties()
                .Where(p => typeof(RecipeBase).IsAssignableFrom(p.PropertyType));

            foreach (var recipeListProp in recipeListProperties)
            {
                var recipe = recipeListProp.GetValue(recipeList) as RecipeBase;
                if (recipe == null) continue;

                var recipeObjectName = recipeListProp.Name; // e.g., "InjectRecipe", "SPDHead1_Recipe"

                var recipeProperties = recipe.GetType().GetProperties();
                foreach (var prop in recipeProperties)
                {
                    if (prop.GetCustomAttribute<SinglePositionTeachingAttribute>() != null)
                    {
                        var uniqueKey = $"{recipeObjectName}.{prop.Name}";
                        if (!properties.ContainsKey(uniqueKey))
                        {
                            properties.Add(uniqueKey, (recipe, prop));
                        }
                    }
                }
            }
            return properties;
        }

        private string BuildUniqueRecipePropertyName<TValue>(Expression<Func<TRecipeList, TValue>> propertyExpression)
        {
            if (propertyExpression.Body is not MemberExpression memberExpression)
                throw new ArgumentException("Biểu thức phải là một biểu thức truy cập thành viên (member access).", nameof(propertyExpression));

            if (memberExpression.Member is not PropertyInfo propertyInfo)
                throw new ArgumentException("Biểu thức phải trỏ đến một thuộc tính (property).", nameof(propertyExpression));

            if (memberExpression.Expression is not MemberExpression ownerExpression || ownerExpression.Member is not PropertyInfo ownerPropertyInfo)
                throw new ArgumentException("Không thể xác định đối tượng recipe từ biểu thức. Định dạng mong muốn: 'recipeList => recipeList.RecipeObject.Property'.", nameof(propertyExpression));

            return $"{ownerPropertyInfo.Name}.{propertyInfo.Name}";
        }

        private (RecipeBase recipe, PropertyInfo prop) ResolveTeachingRecipeProperty(string uniqueRecipePropertyName, string parameterName)
        {
            if (!_recipeProperties.TryGetValue(uniqueRecipePropertyName, out var recipeInfo))
            {
                throw new ArgumentException($"Không tìm thấy thuộc tính recipe '{uniqueRecipePropertyName}' hoặc thuộc tính này không được đánh dấu có thể teach.", parameterName);
            }

            var teachingAttr = recipeInfo.prop.GetCustomAttribute<SinglePositionTeachingAttribute>()
                ?? throw new InvalidOperationException($"Thuộc tính '{recipeInfo.prop.Name}' trong '{recipeInfo.recipe.GetType().Name}' thiếu attribute SinglePositionTeaching.");

            return recipeInfo;
        }

        private (RecipeBase recipe, PropertyInfo prop)? ResolveTeachingRecipePropertyOptional(string? uniqueRecipePropertyName)
        {
            if (string.IsNullOrWhiteSpace(uniqueRecipePropertyName))
            {
                return null;
            }

            return ResolveTeachingRecipeProperty(uniqueRecipePropertyName, nameof(uniqueRecipePropertyName));
        }

        private (RecipeBase recipe, PropertyInfo prop)? ResolveRecipePropertyOptional(string? uniqueRecipePropertyName)
        {
            if (string.IsNullOrWhiteSpace(uniqueRecipePropertyName))
            {
                return null;
            }

            return ResolveRecipePropertyFromUniqueName(uniqueRecipePropertyName, nameof(uniqueRecipePropertyName));
        }

        private (RecipeBase recipe, PropertyInfo prop) ResolveRecipePropertyFromExpression<TValue>(
            Expression<Func<TRecipeList, TValue>> expression,
            string parameterName)
        {
            string uniqueKey = BuildUniqueRecipePropertyName(expression);
            return ResolveRecipePropertyFromUniqueName(uniqueKey, parameterName);
        }

        private static (object source, PropertyInfo prop) ResolveObjectPropertyFromExpression<TSource, TValue>(
            TSource sourceObject,
            Expression<Func<TSource, TValue>> expression,
            string parameterName)
            where TSource : class
        {
            if (sourceObject == null)
            {
                throw new ArgumentNullException(nameof(sourceObject));
            }

            if (expression.Body is not MemberExpression memberExpression)
            {
                throw new ArgumentException("Biểu thức phải là một biểu thức truy cập thành viên (member access).", parameterName);
            }

            if (memberExpression.Member is not PropertyInfo propertyInfo)
            {
                throw new ArgumentException("Biểu thức phải trỏ đến một thuộc tính (property).", parameterName);
            }

            if (memberExpression.Expression == null)
            {
                throw new ArgumentException("Không thể xác định đối tượng chứa thuộc tính trong expression.", parameterName);
            }

            var objectSelector = Expression.Lambda<Func<TSource, object>>(
                Expression.Convert(memberExpression.Expression, typeof(object)),
                expression.Parameters);

            object ownerObject = objectSelector.Compile().Invoke(sourceObject)
                ?? throw new ArgumentException("Đối tượng chứa thuộc tính vision đang null.", parameterName);

            return (ownerObject, propertyInfo);
        }

        private (RecipeBase recipe, PropertyInfo prop) ResolveRecipePropertyFromUniqueName(string uniqueRecipePropertyName, string parameterName)
        {
            var keyParts = uniqueRecipePropertyName.Split('.');
            if (keyParts.Length != 2)
            {
                throw new ArgumentException($"Định dạng key '{uniqueRecipePropertyName}' không hợp lệ. Mong muốn: 'RecipeObject.Property'.", parameterName);
            }

            var recipeOwnerProperty = _recipeList.GetType().GetProperty(keyParts[0]);
            if (recipeOwnerProperty == null)
            {
                throw new ArgumentException($"Không tìm thấy recipe object '{keyParts[0]}' trong recipe list.", parameterName);
            }

            var recipeObject = recipeOwnerProperty.GetValue(_recipeList) as RecipeBase;
            if (recipeObject == null)
            {
                throw new ArgumentException($"'{keyParts[0]}' không phải RecipeBase hoặc đang null.", parameterName);
            }

            var recipeProperty = recipeObject.GetType().GetProperty(keyParts[1]);
            if (recipeProperty == null)
            {
                throw new ArgumentException($"Không tìm thấy thuộc tính '{keyParts[1]}' trong '{recipeObject.GetType().Name}'.", parameterName);
            }

            return (recipeObject, recipeProperty);
        }
    }
}
