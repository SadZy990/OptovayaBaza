using OptovayaBaza.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptovayaBaza.Helpers
{
    public class DbContextSingleton
    {
        private static OptovayaBazaContext _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Приватный конструктор
        /// </summary>
        private DbContextSingleton() { }

        /// <summary>
        /// Публичный метод получения экземпляра
        /// </summary>
        /// <returns></returns>
        public static OptovayaBazaContext GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new OptovayaBazaContext();
                    }
                }
            }
            return _instance;
        }

        public static void Dispose()
        {
            if(_instance != null)
            {
                _instance.Dispose();
            }
        }
    }
}
