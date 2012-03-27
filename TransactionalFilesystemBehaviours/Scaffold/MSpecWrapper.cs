#pragma warning disable 169 // ReSharper disable StaticFieldInGenericType, InconsistentNaming
using System;
using System.Transactions;

namespace Machine.Specifications {
	public abstract class ContextAndResult<TSubject, TResult> {
		protected static Exception the_exception;
		protected static TSubject subject;
		protected static TResult result;

		/// <summary>
		/// Use like <code>Because it = ShouldFail(() => subject.do(something));</code>
		/// </summary>
		protected static Because ShouldFail (Action throwingCase) {
			return () => { the_exception = Catch.Exception(throwingCase); };
		}
	}

	public abstract class ContextOf<TSubject> : ContextAndResult<TSubject, object> { }

	public abstract class DatabaseContextOf<TSubject> : DatabaseContextAndResult<TSubject, object> { }

	public abstract class DatabaseContextAndResult<TSubject, TResult> : ContextAndResult<TSubject, TResult> {
		private static TransactionScope scope;
		Establish database_context = () => { scope = new TransactionScope(); };
		Cleanup database_cleanup = () => { if (scope != null) scope.Dispose(); };
	}
}
