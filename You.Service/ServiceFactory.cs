﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace You.Service
{
    public class ServiceFactory
    {
        private readonly IDictionary<String, Object> factory = new Dictionary<String, Object>();
        private static ServiceFactory instance;

        private static ServiceFactory Current
        {
            get
            {
                if (instance == null) instance = new ServiceFactory();
                return instance;
            }
        }

        /// <summary>
        /// 动态映射创建实现了IService<T>接口的实例
        /// </summary>
        /// <typeparam name="T">Model实体</typeparam>
        /// <returns>实现IService<T>接口的实例对象</returns>
        public IService<T> GetService<T>() where T : class
        {
            Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集 
            Type[] ts = assembly.GetTypes();
            Type t = typeof(T);//用于获取类型的 System.Type 对象

            if (factory.ContainsKey("iGdou.Service" + t.FullName))
                return factory["iGdou.Service" + t.FullName] as IService<T>;//强行转换成泛型接口

            Type _class = ts.FirstOrDefault(o => o.Name.Contains(t.Name));
            IService<T> service;
            if (_class != null)
            {
                service = assembly.CreateInstance(_class.FullName, false) as IService<T>;
                if (service != null)
                {
                    factory.Add("iGdou.Service" + t.FullName, service);
                    return service;
                }
            }
            string className = "iGdou.Service.BaseService`1";
            className += "[[" + t.FullName + ", " + t.Assembly.FullName + "]]";
            Type type = Type.GetType(className);//获取该类名称的类型
            //service = assembly.CreateInstance(className,false) as IService<T>;
            service = Activator.CreateInstance(type) as IService<T>;
            if (service !=null)
            {
                factory.Add("iGdou.Service" + t.FullName, service);
                return service;
            }
            return null;
        }
    }
}