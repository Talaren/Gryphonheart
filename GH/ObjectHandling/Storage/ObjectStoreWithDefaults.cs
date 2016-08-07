﻿namespace GH.ObjectHandling.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CsLuaFramework;
    using Debug;
    using Lua;
    using Misc;

    public class ObjectStoreWithDefaults<T1, T2> : IObjectStoreWithDefaults<T1, T2> where T1 : class, IIdObject<T2>
    {
        private readonly List<T1> defaultObjects;
        private readonly ISerializer serializer;
        private readonly List<T1> objects;
        private readonly ISavedDataHandler savedDataHandler;
        private bool savedDataLoaded;

        public ObjectStoreWithDefaults(ISerializer serializer, string tableName)
        {
            this.savedDataHandler = new SavedDataHandler(tableName);
            this.defaultObjects = new List<T1>();
            this.objects = new List<T1>();
            this.serializer = serializer;
        }

        public void SetDefault(T1 obj)
        {
            if (this.defaultObjects.Any(o => o.Id.Equals(obj.Id)))
            {
                throw new Exception("Default button with that id have already been set");
            }
            this.defaultObjects.Add(obj);
        }

        public T1 Get(T2 id)
        {
            this.ThrowIfSavedDataIsNotLoaded();
            var obj = this.objects.FirstOrDefault(o => o.Id.Equals(id));
            if (obj != null)
            {
                DebugTools.Msg("Got item for id", id);
                return obj;
            }
            var defaultObj = this.defaultObjects.FirstOrDefault(o => o.Id.Equals(id));
            if (defaultObj != null)
            {
                return defaultObj;
            }
            throw new Exception(Strings.strconcat("No default value found for id: ", Strings.tostring(id)));
        }

        public List<T2> GetIds()
        {
            this.ThrowIfSavedDataIsNotLoaded();
            return this.objects.Select(o => o.Id).Union(this.defaultObjects.Select(o => o.Id)).Distinct().ToList();
        }

        public void Set(T1 obj)
        {
            this.ThrowIfSavedDataIsNotLoaded();
            var id = obj.Id;
            var existing = this.objects.FirstOrDefault(o => o.Id.Equals(id));
            if (existing != null)
            {
                this.objects.Remove(existing);
            }

            this.objects.Add(obj);
            var info = this.serializer.Serialize(obj);

            var defaultObj = this.defaultObjects.FirstOrDefault(o => o.Id.Equals(id));
            if (defaultObj != null)
            {
                info = DifferenceUA(info, this.serializer.Serialize(defaultObj));
            }

            this.savedDataHandler.SetVar(obj.Id as string, info);
        }

        public void Remove(T2 id)
        {
            this.ThrowIfSavedDataIsNotLoaded();
            T1 existing = this.objects.FirstOrDefault(o => o.Id.Equals(id));
            if (existing != null)
            {
                this.objects.Remove(existing);
            }
        }

        public void LoadFromSaved()
        {
            var data = this.savedDataHandler.GetAll();
            if (data != null)
            {
                Table.Foreach(data, (key, value) => this.LoadObject(key, (NativeLuaTable)value));
            }
            this.savedDataLoaded = true;
        }

        private void ThrowIfSavedDataIsNotLoaded()
        {
            if (!this.savedDataLoaded)
            {                
                throw new Exception("It is not possible to interact with objects before the saved data is loaded.");
            }
        }

        private void LoadObject(object id, NativeLuaTable info)
        {
            T1 defaultObj = this.defaultObjects.FirstOrDefault(o => o.Id.Equals(id));
            if (defaultObj != null)
            {
                info = MergeUA(info, this.serializer.Serialize(defaultObj));
            }
            this.objects.Add(this.serializer.Deserialize<T1>(info));
        }

        /// <summary>
        ///     Merge u into a.
        /// </summary>
        /// <param name="u">The u set.</param>
        /// <param name="a">The a set.</param>
        /// <returns>U merged into a.</returns>
        private static NativeLuaTable MergeUA(NativeLuaTable u, NativeLuaTable a)
        {
            Table.Foreach(u, (key, value) =>
            {
                if (Core.type(value) == "table" && Core.type(a[key]) == "table")
                {
                    a[key] = MergeUA(value as NativeLuaTable, a[key] as NativeLuaTable);
                }
                else
                {
                    a[key] = value;
                }
            });
            return a;
        }

        /// <summary>
        ///     Returns the set of all elements in u which is not in a.
        /// </summary>
        /// <param name="u">The u set.</param>
        /// <param name="a">The a set.</param>
        /// <returns>All elements in u that are not in a.</returns>
        private static NativeLuaTable DifferenceUA(NativeLuaTable u, NativeLuaTable a)
        {
            var res = new NativeLuaTable();
            var changed = false;

            Table.Foreach(u, (key, value) =>
            {
                if (a[key] == null)
                {
                    res[key] = value;
                    changed = true;
                }
                else if (Core.type(value) == "table" && Core.type(a[key]) == "table")
                {
                    var r = DifferenceUA(value as NativeLuaTable, a[key] as NativeLuaTable);
                    if (r == null)
                    {
                        return;
                    }

                    res[key] = r;
                    changed = true;
                }
                else
                {
                    if (value.Equals(a[key]))
                    {
                        return;
                    }

                    res[key] = value;
                    changed = true;
                }
            });

            return changed ? res : null;
        }


        public List<T1> GetAll()
        {
            return this.objects.Union(this.defaultObjects).ToList();
        }
    }
}