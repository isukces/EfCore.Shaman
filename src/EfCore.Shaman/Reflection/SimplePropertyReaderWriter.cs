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

        public SimplePropertyReaderWriter(Type type, PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
            try
            {
                {
                    var instance = Expression.Parameter(typeof(object), "instance");
                    Expression expr = Expression.Property(Expression.Convert(instance, type), propertyInfo.Name);
                    Expression convertedToObject = Expression.Convert(expr, typeof(object));
                    _reader = Expression.Lambda<Func<object, object>>(convertedToObject, instance).Compile();
                }
            }
            catch 
            {
                // use reflection
            }
            try
            {
                {
                    var instance = Expression.Parameter(typeof(object).MakeByRefType(), "instance");
                    var value = Expression.Parameter(typeof(object), "value");

                    var expr =
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
            catch 
            {
                // use reflection
            }
        }

        #endregion

        #region Instance Methods

        public object ReadPropertyValue(object obj)
        {
            if (_reader != null)
                return _reader(obj);
            return _propertyInfo.GetValue(obj);
        }

        public void WritePropertyValue(object obj, object value)
        {
            if (_writer != null)
                _writer.Invoke(ref obj, value);
            _propertyInfo.SetValue(obj, value);
        }

        #endregion

        #region Fields

        private readonly Func<object, object> _reader;
        private readonly PropertyInfo _propertyInfo;
        private readonly ByRefStructAction _writer;

        #endregion

        #region Delegates

        /// <summary>
        ///     Inspired by
        ///     http://stackoverflow.com/questions/1272454/generate-dynamic-method-to-set-a-field-of-a-struct-instead-of-using-reflection
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        private delegate void ByRefStructAction(ref object instance, object value);

        #endregion
    }
}