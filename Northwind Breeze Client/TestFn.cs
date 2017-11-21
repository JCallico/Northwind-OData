﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Breeze.Sharp.Core;
using Breeze.Sharp;
using System.Windows.Threading;

namespace Northwind_Breeze_Client
{
    public static class ObjectExtensions
    {
        // Fast method to get value of property by name with type validation
        // Throws if property name not present or of incompatible type
        public static T GetPropValue<T>(this object component, string propertyName)
        {
            return (T)TypeDescriptor.GetProperties(component)[propertyName].GetValue(component);
        }

    }

    public static class TestFns
    {
        public static MetadataStore DefaultMetadataStore = new MetadataStore();

        public static async Task<EntityManager> NewEm(string serviceName, MetadataStore metadataStore = null)
        {
            metadataStore = metadataStore ?? DefaultMetadataStore;
            if (metadataStore.GetDataService(serviceName) == null)
            {
                var em = new EntityManager(serviceName, metadataStore);
                await em.FetchMetadata();
                return em;
            }
            else
            {
                return new EntityManager(serviceName, metadataStore);
            }
        }

        public static async Task<EntityManager> NewEm(DataService dataService, MetadataStore metadataStore = null)
        {
            metadataStore = metadataStore ?? DefaultMetadataStore;
            if (dataService.HasServerMetadata && metadataStore.GetDataService(dataService.ServiceName) == null)
            {
                var em = new EntityManager(dataService.ServiceName, metadataStore);
                await em.FetchMetadata();
                return em;
            }
            else
            {
                return new EntityManager(dataService, metadataStore);
            }
        }

        public static void RunInWpfSyncContext(Func<Task> function)
        {
            if (function == null) throw new ArgumentNullException("function");
            var prevCtx = SynchronizationContext.Current;
            try
            {
                var syncCtx = new DispatcherSynchronizationContext();
                SynchronizationContext.SetSynchronizationContext(syncCtx);

                var task = function();
                if (task == null) throw new InvalidOperationException();

                var frame = new DispatcherFrame();
                var t2 = task.ContinueWith(x => { frame.Continue = false; }, TaskScheduler.Default);
                Dispatcher.PushFrame(frame);   // execute all tasks until frame.Continue == false

                task.GetAwaiter().GetResult(); // rethrow exception when task has failed 
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(prevCtx);
            }
        }

        public static bool DEBUG_MONGO = false;
        public static bool DEBUG_ODATA = false;
        public static string EmployeeKeyName = "EmployeeID";
        public static string CustomerKeyName = "CustomerID";

        public static class WellKnownData
        {
            public static Guid AlfredsID = new Guid("785efa04-cbf2-4dd7-a7de-083ee17b6ad2");
            public static int NancyEmployeeID = 1;
            public static int ChaiProductID = 1;
            public static int DummyOrderID = 999;
            public static int DummyEmployeeID = 9999;
            public static Object[] AlfredsOrderDetailKey = new Object[] { 10643, 28 };
        }

        public static String MorphString(String val)
        {
            var suffix = "__";
            if (String.IsNullOrEmpty(val))
            {
                return suffix;
            }
            else
            {
                if (val.EndsWith(suffix))
                {
                    val = val.Substring(0, val.Length - 2);
                }
                else
                {
                    val = val + suffix;
                }
            }
            return val;
        }

        private static String Revese(String s)
        {
            var charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static String RandomSuffix(int length)
        {
            // length should be less <= 18
            var suffix = DateTime.Now.Ticks.ToString().Reverse().Take(length).ToArray();
            return new string(suffix);
        }

        public static async Task VerifyQuery(EntityQuery query, string serviceName, string testName = "unknown test")
        {
            try
            {
                var entityManager = await NewEm(serviceName);
                var results = await entityManager.ExecuteQuery(query);
                var count = results.OfType<Object>().Count();
                Assert.IsTrue(count > 0, testName + ": Should return 1 or more entities.  Returned " + count);
            }
            catch (Exception e)
            {
                var message = FormatException(e);
                Assert.Fail(message);
            }

        }

        public static string FormatException(Exception e)
        {
            string message = string.Empty;
            if (e == null)
            {
                return "Exception is null";
            }
            var exception = e as AggregateException;
            if (exception == null)
            {
                message = FormatInnerExceptions(e);
            }
            else
            {
                exception.InnerExceptions.ForEach(ie =>
                {
                    message += FormatInnerExceptions(ie);
                });
            }
            return message;
        }

        private static string FormatInnerExceptions(Exception e)
        {
            var message = e.Message + Environment.NewLine;
            var ie = e.InnerException;
            while (ie != null)
            {
                message += ie.Message + Environment.NewLine;
                ie = ie.InnerException;
            }
            return message;
        }

    }
}

