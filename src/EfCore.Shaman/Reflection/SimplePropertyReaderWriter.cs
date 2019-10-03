using System;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace EfCore.Shaman.Reflection
{
    public class SimplePropertyReaderWriter : IPropertyValueReader, IPropertyValueWriter
    {
        private SimplePropertyReaderWriter(Type type, PropertyInfo propertyInfo, IShamanLogger logger)
        {
            _logger       = logger ?? EmptyShamanLogger.Instance;
            _propertyInfo = propertyInfo;
            try
            {
                {
                    var instance = Expression.Parameter(typeof(object), "instance");
                    Expression expr =
                        Expression.Property(Expression.Convert(instance, type), propertyInfo.Name);
                    Expression convertedToObject = Expression.Convert(expr, typeof(object));
                    _reader = Expression.Lambda<Func<object, object>>(convertedToObject, instance).Compile();
                }
            }
            catch (Exception e)
            {
                _logger.LogException(Guid.Parse("{D8AFA7E9-DC66-407C-BB61-B19B6BFF6351}"), e);
            }

            try
            {
                {
                    var instance = Expression.Parameter(typeof(object).MakeByRefType(), "instance");
                    var value    = Expression.Parameter(typeof(object), "value");

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
            catch (Exception e)
            {
                var ee = new UnableToGetReaderWriterException(
                    $"Unable to get reader/writer for property {type}.{propertyInfo.Name}. Consider using NoDirectSaverAttribute.", e);
                _logger.LogException(Guid.Parse("{81F76FA3-8CAF-4E56-AE2E-58BC006F5F18}"), ee);
            }
        }

        [CanBeNull]
        public static SimplePropertyReaderWriter Make(Type type, PropertyInfo propertyInfo, IShamanLogger logger)
        {
            if (propertyInfo.GetCustomAttribute<NoDirectSaverAttribute>() != null) 
                return null;
            return new SimplePropertyReaderWriter(type, propertyInfo, logger);
        }

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

        private readonly Func<object, object> _reader;
        private readonly PropertyInfo _propertyInfo;
        private readonly IShamanLogger _logger;
        private readonly ByRefStructAction _writer;

        /// <summary>
        ///     Inspired by
        ///     http://stackoverflow.com/questions/1272454/generate-dynamic-method-to-set-a-field-of-a-struct-instead-of-using-reflection
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        private delegate void ByRefStructAction(ref object instance, object value);
    }

    public class UnableToGetReaderWriterException : Exception
    {
        public UnableToGetReaderWriterException(string message, Exception innerException) : base(message,
            innerException)
        {
        }
    }
}