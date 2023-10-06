using System.Formats.Tar;
using System.Reflection;
using System;
using System.Reflection.Emit;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace core
{

    public class TResult<T>
    {
        public T Instance { get; }
        public TResult(T val)
        {
            Instance = val;
        }

        public static implicit operator T(TResult<T> d) => d.Instance;
        public static explicit operator TResult<T>(T b) => new TResult<T>(b);
    }
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

        private static object CreateX(string Name, params object[] Args)
        {
            Type theType = Type.GetType(Name);
            object toReturn = null;
            // Code wrote quickly, I just try to call all the type constructors...
            foreach (var constructor in Type.GetType(Name).GetConstructors())
            {
                try
                {
                    string paramPrefix = "p";
                    int pIdx = 0;
                    var expParamsConsts = new List<Expression>();
                    var ctrParams = constructor.GetParameters();
                    for (int i = 0; i < constructor.GetParameters().Length; i++)
                    {
                        var param = ctrParams[i];
                        var tmpParam = Expression.Variable(param.ParameterType, paramPrefix + pIdx++);
                        var expConst = Expression.Convert(Expression.Constant(Args[i]), param.ParameterType);
                        expParamsConsts.Add(expConst);
                    }
                    // new Type(...);
                    var expConstructor = Expression.New(constructor, expParamsConsts);
                    // return new Type(...);
                    var expLblRetTarget = Expression.Label(theType);
                    var expReturn = Expression.Return(expLblRetTarget, expConstructor, theType);
                    var expLblRet = Expression.Label(expLblRetTarget, Expression.Default(theType));
                    // { return new Type(...); }
                    var expBlock = Expression.Block(expReturn, expLblRet);
                    // Build the expression and run it
                    var expFunc = Expression.Lambda<Func<dynamic>>(expBlock);
                    toReturn = expFunc.Compile().Invoke();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
            return toReturn;
        }
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
                if(_result is object result)
                {
                    var t1 = result.GetType();
                    var t2 = typeof(TResult<>);

                    //var p1 = typeof(TResult<I>);
                    //ConstructorInfo constr = myParameterizedSomeClass.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.stat)[0];

                    if(t1 == typeof(System.Int32))
                    {
                        //Type myParameterizedSomeClass = typeof(TResult<>).MakeGenericType(_result.GetType());
                        //ConstructorInfo constr = myParameterizedSomeClass.GetConstructors(BindingFlags.Public | BindingFlags.Instance)[0];
                        //var ff = constr.Invoke(new object[] { result });
                        //int g = (TResult<int>)ff;
                        //return ff;



                        //var ffff = CreateX("core.TResult`1", result);
                        //TResult<System.Int32> g = new TResult<int>(8);
                        //string str = g.GetType().FullName;
                        //int gg = 0;

                        Type typeArgument = result.GetType();
                        Type template = typeof(core.TResult<>);
                        //Type t4 = Type.GetType(fffff.FullName);
                        Type genericType = template.MakeGenericType(typeArgument);
                        var instance = Activator.CreateInstance(genericType, result);


                    }
                    
                }
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