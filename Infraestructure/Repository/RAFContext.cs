using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Repository
{
    public class RAFContext
    {
        private string fileName;
        private int size;
       

        public RAFContext(string fileName, int size)
        {
            this.fileName = fileName;
            this.size = size;
        }

        public Stream HeaderStream
        {
            get => File.Open($"{fileName}.hd", FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }

        public Stream DataStream
        {
            get => File.Open($"{fileName}.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }

        public void Create<T>(T t)
        {
            try
            {
                using (BinaryWriter bwHeader = new BinaryWriter(HeaderStream),
                                    bwData = new BinaryWriter(DataStream))
                {
                    int n, k;
                    using (BinaryReader brHeader = new BinaryReader(bwHeader.BaseStream))
                    {
                        if (brHeader.BaseStream.Length == 0)
                        {
                            n = 0;
                            k = 0;
                        }
                        else
                        {
                            brHeader.BaseStream.Seek(0, SeekOrigin.Begin);
                            n = brHeader.ReadInt32();
                            k = brHeader.ReadInt32();
                        }
                        //calculamos la posicion en Data
                        long pos = k * size;
                        bwData.BaseStream.Seek(pos, SeekOrigin.Begin);

                        PropertyInfo[] info = t.GetType().GetProperties();
                        foreach (PropertyInfo pinfo in info)
                        {
                            Type type = pinfo.PropertyType;
                            object obj = pinfo.GetValue(t, null);
                            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<T>))
                            {
                                continue;
                            }
                            if (!type.IsPrimitive && type.IsClass && type != Type.GetType("System.String"))
                            {
                                PropertyInfo[] infoClass = obj.GetType().GetProperties();
                                object objectClass = Activator.CreateInstance(obj.GetType());
                                foreach (PropertyInfo PInfoClass in infoClass)
                                {
                                    if (PInfoClass.Name.Equals("Id", StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        bwData.Write((int)PInfoClass.GetValue(obj));
                                        break;
                                    }
                                }
                                continue;
                            }
                            if (pinfo.Name.Equals("Id", StringComparison.CurrentCultureIgnoreCase))
                            {
                                bwData.Write(++k);
                                continue;
                            }

                            if (type == typeof(int))
                            {
                                bwData.Write((int)obj);
                            }
                            else if (type == typeof(long))
                            {
                                bwData.Write((long)obj);
                            }
                            else if (type == typeof(float))
                            {
                                bwData.Write((float)obj);
                            }
                            else if (type == typeof(double))
                            {
                                bwData.Write((double)obj);
                            }
                            else if (type == typeof(decimal))
                            {
                                bwData.Write((decimal)obj);
                            }
                            else if (type == typeof(char))
                            {
                                bwData.Write((char)obj);
                            }
                            else if (type == typeof(bool))
                            {
                                bwData.Write((bool)obj);
                            }
                            else if (type == typeof(string))
                            {
                                bwData.Write((string)obj);
                            }
                            else if (type.IsEnum)
                            {
                                bwData.Write((int)obj);
                            }
                        }

                        long posh = 8 + n * 4;
                        bwHeader.BaseStream.Seek(posh, SeekOrigin.Begin);
                        bwHeader.Write(k);

                        bwHeader.BaseStream.Seek(0, SeekOrigin.Begin);
                        bwHeader.Write(++n);
                        bwHeader.Write(k);
                        brHeader.Close();
                    }
                }
            }
            catch (IOException)
            {
                throw;
            }

        }

        public T Get<T>(int id) 
        {
            try
            {
                T newValue = (T)Activator.CreateInstance(typeof(T));
                using (BinaryReader brHeader = new BinaryReader(HeaderStream),
                                    brData = new BinaryReader(DataStream))
                {
                    //TODO Validar como en el metodo create
                    brHeader.BaseStream.Seek(0, SeekOrigin.Begin);
                    int n = brHeader.ReadInt32();
                    int k = brHeader.ReadInt32();

                    if (id <= 0 || id > k)
                    {
                        return default(T);
                    }

                    PropertyInfo[] properties = newValue.GetType().GetProperties();
                   
                    List<int> ids = new List<int>();
                    brHeader.BaseStream.Seek(8, SeekOrigin.Begin);
                    while (brHeader.BaseStream.Position < brHeader.BaseStream.Length)
                    {
                        ids.Add(brHeader.ReadInt32());
                    }
                    
                    if (ids.BinarySearch(id) < 0)
                    {
                        return default(T);
                    }

                    
                    long posd = (id - 1) * size;
                    brData.BaseStream.Seek(posd, SeekOrigin.Begin);
                    foreach (PropertyInfo pinfo in properties)
                    {
                        Type type = pinfo.PropertyType;
                        if (!type.IsPrimitive && type.IsClass && type != Type.GetType("System.String"))
                        {
                            PropertyInfo[] infoClass = type.GetProperties();
                            object objectClass = Activator.CreateInstance(pinfo.PropertyType);
                            foreach (PropertyInfo PInfoClass in infoClass)
                            {
                                if (PInfoClass.Name.Equals("Id", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    PInfoClass.SetValue(objectClass, brData.GetValue<int>(TypeCode.Int32));
                                    pinfo.SetValue(newValue, objectClass);
                                    break;
                                }
                            }
                            continue;
                        }

                        if (type == typeof(int))
                        {
                            pinfo.SetValue(newValue, brData.GetValue<int>(TypeCode.Int32));
                        }
                        else if (type == typeof(long))
                        {
                            pinfo.SetValue(newValue, brData.GetValue<long>(TypeCode.Int64));
                        }
                        else if (type == typeof(float))
                        {
                            pinfo.SetValue(newValue, brData.GetValue<float>(TypeCode.Single));
                        }
                        else if (type == typeof(double))
                        {
                            pinfo.SetValue(newValue, brData.GetValue<double>(TypeCode.Double));
                        }
                        else if (type == typeof(decimal))
                        {
                            pinfo.SetValue(newValue, brData.GetValue<decimal>(TypeCode.Decimal));
                        }
                        else if (type == typeof(char))
                        {
                            pinfo.SetValue(newValue, brData.GetValue<char>(TypeCode.Char));
                        }
                        else if (type == typeof(bool))
                        {
                            pinfo.SetValue(newValue, brData.GetValue<bool>(TypeCode.Boolean));
                        }
                        else if (type == typeof(string))
                        {
                            pinfo.SetValue(newValue, brData.GetValue<string>(TypeCode.String));
                        }
                        else if (type.IsEnum)
                        {
                            pinfo.SetValue(newValue, brData.GetValue<int>(TypeCode.Int32));
                        }
                    }
                }
                return newValue;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> GetAll<T>()
        {
            List<T> listT = new List<T>();
            int n = 0;
            try
            {
                using (BinaryReader brHeader = new BinaryReader(HeaderStream))
                {
                    if (brHeader.BaseStream.Length > 0)
                    {
                        brHeader.BaseStream.Seek(0, SeekOrigin.Begin);
                        n = brHeader.ReadInt32();
                    }
                }

                if (n == 0)
                {
                    return listT;
                }

                for (int i = 0; i < n; i++)
                {
                    int index;
                    using (BinaryReader brHeader = new BinaryReader(HeaderStream))
                    {
                        long posh = 8 + i * 4;
                        brHeader.BaseStream.Seek(posh, SeekOrigin.Begin);
                        index = brHeader.ReadInt32();
                        brHeader.Close();
                    }
                    T t = Get<T>(index);
                    listT.Add(t);
                }
                return listT;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> Find<T>(Expression<Func<T, bool>> where)
        {
            List<T> listT = new List<T>();
            int n, k;
            Func<T, bool> comparator = where.Compile();
            try
            {
                using (BinaryReader brHeader = new BinaryReader(HeaderStream))
                {
                    brHeader.BaseStream.Seek(0, SeekOrigin.Begin);
                    n = brHeader.ReadInt32();
                    k = brHeader.ReadInt32();
                    brHeader.Close();
                }

                for (int i = 0; i < n; i++)
                {
                    int index;
                    using (BinaryReader brHeader = new BinaryReader(HeaderStream))
                    {
                        long posh = 8 + i * 4;
                        brHeader.BaseStream.Seek(posh, SeekOrigin.Begin);
                        index = brHeader.ReadInt32();
                    }

                    T t = Get<T>(index);
                    if (comparator(t))
                    {
                        listT.Add(t);
                    }

                }
                return listT;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void Update<T>(T t)
        {
            try
            {
                int Id = (int)t.GetType().GetProperty("Id").GetValue(t);

                using (BinaryWriter bwHeader = new BinaryWriter(HeaderStream),
                                    bwData = new BinaryWriter(DataStream))
                {
                    long pos = (Id - 1) * size;
                    bwData.BaseStream.Seek(pos, SeekOrigin.Begin);

                    PropertyInfo[] propertyInfo = t.GetType().GetProperties();
                    foreach (PropertyInfo pinfo in propertyInfo)
                    {
                        Type type = pinfo.PropertyType;
                        object obj = pinfo.GetValue(t, null);
                        if (!type.IsPrimitive && type.IsClass && type != Type.GetType("System.String"))
                        {

                            PropertyInfo[] infoClass = obj.GetType().GetProperties();
                            object objectClass = Activator.CreateInstance(obj.GetType());
                            foreach (PropertyInfo PInfoClass in infoClass)
                            {
                                if (PInfoClass.Name.Equals("Id", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    bwData.Write((int)PInfoClass.GetValue(obj));
                                    break;
                                }

                            }
                            continue;
                        }
                        if (type == typeof(int))
                        {
                            bwData.Write((int)obj);
                        }
                        else if (type == typeof(long))
                        {
                            bwData.Write((long)obj);
                        }
                        else if (type == typeof(float))
                        {
                            bwData.Write((float)obj);
                        }
                        else if (type == typeof(double))
                        {
                            bwData.Write((double)obj);
                        }
                        else if (type == typeof(decimal))
                        {
                            bwData.Write((decimal)obj);
                        }
                        else if (type == typeof(char))
                        {
                            bwData.Write((char)obj);
                        }
                        else if (type == typeof(bool))
                        {
                            bwData.Write((bool)obj);
                        }
                        else if (type == typeof(string))
                        {
                            bwData.Write((string)obj);
                        }
                        else if (type.IsEnum)
                        {
                            bwData.Write((int)obj);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool Delete(int id)
        {
            try
            {
                using (BinaryReader brHeader = new BinaryReader(HeaderStream),
                                   brData = new BinaryReader(DataStream))
                {
                    brHeader.BaseStream.Seek(0, SeekOrigin.Begin);
                    int n = brHeader.ReadInt32();
                    int k = brHeader.ReadInt32();

                    List<int> ids = new List<int>();
                    while (brHeader.BaseStream.Position < brHeader.BaseStream.Length)
                    {
                        ids.Add(brHeader.ReadInt32());
                    }
                    int resultado = ids.BinarySearch(id);
                    if (resultado < 0)
                    {
                        return false;
                    }

                    using (BinaryWriter bwHeadertmp = new BinaryWriter(File.Open("tmp.hd", FileMode.OpenOrCreate, FileAccess.ReadWrite)))
                    {
                        bwHeadertmp.Seek(0, SeekOrigin.Begin);
                        bwHeadertmp.Write(--n);
                        bwHeadertmp.Write(k);
                        foreach (int num in ids)
                        {
                            if (num != id)
                            {
                                bwHeadertmp.Write(num);
                            }
                        }
                    }
                }
                File.Delete($"{fileName}.hd");
                File.Move("tmp.hd", $"{fileName}.hd");
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int GetLastId()
        {
            int k = 0;
            try
            {
                using (BinaryReader brHeader = new BinaryReader(HeaderStream))
                {
                    if (brHeader.BaseStream.Length == 0)
                    {
                        return 0;
                    }
                    long posh = 4;
                    brHeader.BaseStream.Seek(posh, SeekOrigin.Begin);
                    k = brHeader.ReadInt32();
                }
            }
            catch (IOException)
            {
                throw;
            }
            return k;
        }
    }
}
