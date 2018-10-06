﻿using AspNetInsight.Dto;
using AspNetInsight.Repo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Serialization;

namespace AspNetInsight
{
    /// <summary>
    /// Common extensions used across AspNetInsight
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Update the Basic data with recent and recalculates its value.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="current"></param>
        public static void Update(this BasicSts @this, double current)
        {
            lock (@this)
            {
                @this.Recent = current;
                @this.Min = @this.Min == -1 ? @this.Recent : @this.Min > @this.Recent ? @this.Recent : @this.Min;
                @this.Max = @this.Max == -1 ? @this.Recent : @this.Max < @this.Recent ? @this.Recent : @this.Max;

                @this.Avg = @this.Total == 0 ? @this.Recent : ((@this.Total * @this.Avg) + @this.Recent) / (@this.Total + 1);
                @this.Total += 1;
            }
        }

        /// <summary>
        /// Get default App details from given HttpContext.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static App GetAppDetails(this HttpContext ctx)
        {
            if (ctx == null)
                throw new NullReferenceException(nameof(ctx));

            return new App()
            {
                AppId = HostingEnvironment.ApplicationID,
                AppName = HostingEnvironment.SiteName,
                Url = ctx.Request.Url.ToString(),
                MachineName = Environment.MachineName
            };
        }

        /// <summary>
        /// Converts the given basic data into key value pairs
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetDataAsKvp(this BasicSts data)
        {
            return new Dictionary<string, string>()
            {
                { "MIN", Math.Round(data.Min, 2).ToString() },
                { "AVG", Math.Round(data.Avg, 2).ToString() },
                { "MAX", Math.Round(data.Max, 2).ToString() },
                { "TOTAL", data.Total.ToString() },
                { "RECENT", Math.Round(data.Recent, 2).ToString() }
            };
        }

        /// <summary>
        /// Deep copies the given object using DataContractJsonSerializer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepCopy<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                DataContractJsonSerializer sr = new DataContractJsonSerializer(typeof(T));
                sr.WriteObject(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                return (T)sr.ReadObject(ms);

            }
        }

        /// <summary>
        /// make the given file is accessible by everyone
        /// </summary>
        /// <param name="file"></param>
        public static void GrantAccessToEveryone(this FileInfo file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            var everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            FileSecurity sec = File.GetAccessControl(file.FullName);
            sec.AddAccessRule(new FileSystemAccessRule(everyone, FileSystemRights.FullControl, AccessControlType.Allow));
            File.SetAccessControl(file.FullName, sec);
        }
    }

    public class Module
    {
        static void Main(string[] args)
        {
        }
    }
}
