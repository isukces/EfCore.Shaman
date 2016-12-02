#region using

using System;
using System.Linq.Expressions;
using System.Reflection;

#endregion

namespace EfCore.Shaman.Reflection
{
    public class SimplePropertyReaderWriter : IPropertyValueReader, IPropertyValueWriter
    {
        #region Constructors

        /// <summary>
        /// Inspired by http://stackoverflow.com/questions/1272454/generate-dynamic-method-to-set-a-field-of-a-struct-instead-of-using-reflection
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        delegate void ByRefStructAction(ref object instance, object value);
        public SimplePropertyReaderWriter(Type type, PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
            {
                var instance = Expression.Parameter(typeof(object), "instance");
                Expression expr = Expression.Property(Expression.Convert(instance, type), propertyInfo.Name);
                Expression convertedToObject = Expression.Convert(expr, typeof(object));
                _reader = Expression.Lambda<Func<object, object>>(convertedToObject, instance).Compile();
            }
            {
                var instance = Expression.Parameter(typeof(object).MakeByRefType(), "instance");
                var value = Expression.Parameter(typeof(object), "value");

                Expression<ByRefStructAction> expr =
                    Expression.Lambda<ByRefStructAction>(
                        Expression.Assign(
                            Expression.Property(
                                Expression.Convert(instance, type),
                                propertyInfo),
                            Expression.Convert(
                                value,
                                propertyInfo.PropertyType)),
                        instance,
                        value);

                _writer = expr.Compile();
            }
        }

        #endregion

        #region Instance Methods

        public object ReadPropertyValue(object obj)
        {
            return _reader(obj);
        }

        public void WritePropertyValue(object obj, object value)
        {
            _writer.Invoke(ref obj, value);
            // _propertyInfo.SetValue(obj, value);
        }

        #endregion

        #region Fields

        private readonly Func<object, object> _reader;
        private readonly PropertyInfo _propertyInfo;
        private readonly ByRefStructAction _writer;

        #endregion
    }
}