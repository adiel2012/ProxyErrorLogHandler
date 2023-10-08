using System.Formats.Tar;
using System.Reflection;
using System;
using System.Reflection.Emit;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace core
{
    
    public static class TypeExtensions
    {
        public static object? GetDefaultValue(this Type t)
        {
            if (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
                return Activator.CreateInstance(t);
            else
                return null;
        }
    }
    public class ProxyErrorHandler<I> : DispatchProxy where I : class
    {

        public I Target { get; set; }

        public Action<MethodInfo , object[]> PreInvokeHandler = (MethodInfo targetMethod, object[] args) =>{
            Console.WriteLine($"Called {targetMethod.Name}");
        };

        public Func<System.Exception, bool> ExceptionLogger = (System.Exception ex) =>{
            Console.WriteLine(ex.Message);
            return false;
        };

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            PreInvokeHandler?.Invoke(targetMethod, args);

            

            try
            {
                var _result = targetMethod.Invoke(Target, args);
                return _result;
            }
            catch(ApplicationException ex)
            {
                if(ExceptionLogger?.Invoke(ex.InnerException) ?? true)
                    throw ex;
            }
            catch(Exception ex)
            {
                if(ExceptionLogger?.Invoke(ex) ?? true)
                    throw ex;
            }

            return targetMethod.ReturnType.GetDefaultValue();
        }

        public static ProxyErrorHandler<I> CreateProxy<T>(T target)  where T : class, I
        {
            var proxy = Create<I, ProxyErrorHandler<I>>() as ProxyErrorHandler<I>;
            proxy.Target = target;
            return proxy;
        }
    }
}