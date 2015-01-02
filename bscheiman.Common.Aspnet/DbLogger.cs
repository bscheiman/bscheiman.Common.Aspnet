﻿#region
using System;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics;

#endregion

namespace bscheiman.Common.Aspnet {
    public class DbLogger : DbCommandInterceptor {
        public override void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext) {
            CommandExecuting(base.NonQueryExecuting, command, interceptionContext);
        }

        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext) {
            CommandExecuting(base.ReaderExecuting, command, interceptionContext);
        }

        public override void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext) {
            CommandExecuting(base.ScalarExecuting, command, interceptionContext);
        }

        private void CommandExecuting<T>(ExecutingMethod<T> executingMethod, DbCommand command,
            DbCommandInterceptionContext<T> interceptionContext) {
            var sw = Stopwatch.StartNew();
            executingMethod.Invoke(command, interceptionContext);
            sw.Stop();

            if (interceptionContext.Exception != null) {
                Trace.WriteLine("ERROR: " + interceptionContext.Exception,
                    String.Format("Error executing command: {0}", command.CommandText));
            } else
                Trace.WriteLine(string.Format("[{1}] {0}", command.CommandText, sw.Elapsed));
        }

        #region Nested type: ExecutingMethod
        private delegate void ExecutingMethod<T>(DbCommand command, DbCommandInterceptionContext<T> interceptionContext);
        #endregion
    }
}